using SolverHTE.Triangulation.Structs;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SolverHTE.SolvingEquation")]
namespace SolverHTE.Triangulation.Utility
{
    /// <summary>
    /// Данный класс предоставляет методы для выполнения элементов триангуляции двумерной области.
    /// </summary>
    internal static class TriangulationUtility
    {
        /// <summary>
        /// Находит треугольники на сторонах которых лежит заданная точка, и 
        /// возвращает номера этих треугольников и их ребер.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="triangles"></param>
        /// <returns></returns>
        public static List<EdgeTriangleNums> PointOnSideTriangle(PointM point, List<Triangle> triangles)
        {
            var current = new EdgeTriangleNums();
            var edgesAndTriangles = new List<EdgeTriangleNums>();

            for (int i = 0; i < triangles.Count; i++)
            {
                if (point != triangles[i].Vertex[0] && point != triangles[i].Vertex[1] && point != triangles[i].Vertex[2])
                {
                    for(int j = 0; j < 3; j++)
                    {
                        var firstSideIndex  = j;
                        var lastSideIndex = j == 2 ? 0 : j + 1;

                        if (Math.Abs(GeometryUtility.GetDistancePoints(triangles[i].Vertex[firstSideIndex], point) +
                            GeometryUtility.GetDistancePoints(triangles[i].Vertex[lastSideIndex], point) -
                            GeometryUtility.GetDistancePoints(triangles[i].Vertex[firstSideIndex], 
                            triangles[i].Vertex[lastSideIndex])) <= 0.00000001m)
                        {
                            current.NumTriangle = i;
                            current.NumEdge = lastSideIndex == 0 ? 3 : lastSideIndex;

                            if (!ContainsEdgeTriangleNums(edgesAndTriangles, current))
                            {
                                edgesAndTriangles.Add(current);
                            }
                        }
                    }                 
                }
            }

            return edgesAndTriangles;
        }

        /// <summary>
        /// Проверяет содержится ли элемент в списке.
        /// </summary>
        /// <param name="edgesTrianglesNums"></param>
        /// <param name="edgesTriangleNums"></param>
        /// <returns></returns>
        private static bool ContainsEdgeTriangleNums(List<EdgeTriangleNums> edgesTrianglesNums, EdgeTriangleNums edgeTriangleNums)
        {
            for (int j = 0; j < edgesTrianglesNums.Count; j++)
            {
                if (edgesTrianglesNums[j].NumTriangle == edgeTriangleNums.NumTriangle)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Проверяет лежит ли заданная точка внутри треугольника.
        /// </summary>
        /// <param name="triangle"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool PointOnTriangle(Triangle triangle, PointM point)
        {
            var t = new decimal[3];

            t[0] = (triangle.Vertex[0].X - point.X) * (triangle.Vertex[1].Y - triangle.Vertex[0].Y) -
                (triangle.Vertex[1].X - triangle.Vertex[0].X) * (triangle.Vertex[0].Y - point.Y);

            t[1] = (triangle.Vertex[1].X - point.X) * (triangle.Vertex[2].Y - triangle.Vertex[1].Y) -
                (triangle.Vertex[2].X - triangle.Vertex[1].X) * (triangle.Vertex[1].Y - point.Y);

            t[2] = (triangle.Vertex[2].X - point.X) * (triangle.Vertex[0].Y - triangle.Vertex[2].Y) -
                (triangle.Vertex[0].X - triangle.Vertex[2].X) * (triangle.Vertex[2].Y - point.Y);

            return Math.Sign(t[0]) == Math.Sign(t[1]) &&
                Math.Sign(t[0]) == Math.Sign(t[2]);
        }

        /// <summary>
        /// Возвращает номер ребра, на котором не содержится заданная точка.
        /// </summary>
        /// <param name="triangle"></param>
        /// <param name="point"></param>
        /// <returns>Номер ребра, не содержащий точку или -1 в другом случае.</returns>
        /// 
        public static int GetNumEdgeWithoutPoint(Triangle triangle, PointM point)
        {
            for(int i = 0; i < triangle.Vertex.Length; i++)
            {
                if(point == triangle.Vertex[i])
                {
                    return (i + 2) == 4 ? 1 : i + 2;
                }
            }

            return -1;
        }

        /// <summary>
        ///  Находит номер соседнего треугольника для заданного и их общее ребро.
        /// </summary>
        /// <param name="indexEdge"></param>
        /// <param name="triangle"></param>
        /// <param name="triangles"></param>
        /// <returns></returns>
        public static EdgeTriangleNums GetNeighborEdgeTriangleNums(int numEdge, Triangle triangle, List<Triangle> triangles)
        {
            var edgeTriangleNums = new EdgeTriangleNums();

            var firstSideIndex = numEdge - 1;
            var lastSideIndex = numEdge == 3 ? 0 : numEdge;

            var firstPoint = triangle.Vertex[firstSideIndex];
            var lastPoint = triangle.Vertex[lastSideIndex];
            var otherPoint = triangle.Vertex[3 - firstSideIndex - lastSideIndex];

            for (int i = 0; i < triangles.Count; i++)
            {
                for (int j = 0; j < triangles[i].Vertex.Length; j++)
                {
                    var currentFirstPoint = triangles[i].Vertex[j];

                    var currentLastPoint = j == 2 ? triangles[i].Vertex[0] :
                        triangles[i].Vertex[j + 1];

                    var currentOtherPoint = j == 2 ? triangles[i].Vertex[3 - j] :
                        triangles[i].Vertex[3 - j - j - 1];

                    if ((firstPoint == currentFirstPoint && lastPoint == currentLastPoint && otherPoint != currentOtherPoint) ||
                        firstPoint == currentLastPoint && lastPoint == currentFirstPoint && otherPoint != currentOtherPoint)
                    {
                        edgeTriangleNums.NumTriangle = i;
                        edgeTriangleNums.NumEdge = j == 2 ? 3 : j + 1;
                        break;
                    }
                }
            }

            return edgeTriangleNums;
        }

        /// <summary>
        /// Проверяет находится ли заданная точка за границей области.
        /// </summary>
        /// <param name="addPoint"></param>
        /// <param name="triangles"></param>
        /// <returns>Истину, если точка находится за область, и ложь в ином случае.</returns>
        public static bool PointIsBeyondArea(PointM addPoint, List<Triangle> triangles)
        {
            var edgeTriangleNums = PointOnSideTriangle(addPoint, triangles);

            if (edgeTriangleNums.Count == 0)
            {
                for (int i = 0; i < triangles.Count; i++)
                {
                    if (!PointOnTriangle(triangles[i], addPoint))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Проверяет находится ли вершина соседнего треугольника, в описанной окружности заданного треугольника.
        /// </summary>
        /// <param name="neighborTriangle"></param>
        /// <param name="triangle"></param>
        /// <param name="triangles"></param>
        /// <returns>Истину, если находится, ложь в ином случае.</returns>
        public static bool PointInCircleNeighborTriangle(EdgeTriangleNums neighborTriangle, Triangle triangle, List<Triangle> triangles)
        {
            var radius = GeometryUtility.GetProductSidesTriangle(triangle) /
                (4 * GeometryUtility.GetSquareTriangle(triangle));

            var index = neighborTriangle.NumEdge == 2 ? 0 : neighborTriangle.NumEdge == 3 ? 1 :
                neighborTriangle.NumEdge + 1;

            if (GeometryUtility.GetDistancePoints(triangles[neighborTriangle.NumTriangle].Vertex[index],
                   GeometryUtility.GetCenterPointCircleTriangle(triangle)) <= radius)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Находит точку пересечения высоты с основанием, выпущенной из наибольшего угла треугольника.
        /// </summary>
        /// <param name="triangle"></param>
        /// <returns></returns>
        public static PointM FindNewVertexHeightTriangle(Triangle triangle)
        {
            var numSide = GeometryUtility.GetNumSideTriangle(triangle);
            var h = 2 * GeometryUtility.GetSquareTriangle(triangle) / GeometryUtility.GetSideLenght(numSide, triangle);

            var vector = new PointM();
            var newVertex = new PointM();

            var currentIndex = numSide == 3 ? 0 : numSide;
            var prevIndex = currentIndex == 0 ? 2 : currentIndex - 1;
            var nextIndex = currentIndex == 2 ? 0 : currentIndex + 1;


            var cat = (decimal)Math.Sqrt((double)
                    (GeometryUtility.GetDistancePoints(triangle.Vertex[currentIndex], triangle.Vertex[nextIndex]) *
                    GeometryUtility.GetDistancePoints(triangle.Vertex[currentIndex], triangle.Vertex[nextIndex]) - h * h));

            vector.X = (triangle.Vertex[prevIndex].X - triangle.Vertex[currentIndex].X) /
                   GeometryUtility.GetDistancePoints(triangle.Vertex[prevIndex], triangle.Vertex[currentIndex]);

            vector.Y = (triangle.Vertex[prevIndex].Y - triangle.Vertex[currentIndex].Y) /
                GeometryUtility.GetDistancePoints(triangle.Vertex[prevIndex], triangle.Vertex[currentIndex]);

            newVertex.X = triangle.Vertex[currentIndex].X + vector.X * cat;
            newVertex.Y = triangle.Vertex[currentIndex].Y + vector.Y * cat;

            return newVertex;
        }

        /// <summary>
        /// Проверяет является ли точка необходимой для дальнейшего разделения области.
        /// </summary>
        /// <param name="currentPoint"></param>
        /// <param name="triangles"></param>
        /// <param name="splitPoints"></param>
        /// <returns>Истину, если точка небходима, и ложь в ином случае.</returns>
        public static bool PointIsRequired(int indexPoint, List<Triangle> triangles, List<PointM> splitPoints)
        {
            var firstT = false;
            var secondT = false;

            var firstIndex = indexPoint == 0 ? splitPoints.Count - 1 : indexPoint - 1;
            var secondIndex = indexPoint == splitPoints.Count - 1 ? 0 : indexPoint + 1;

            for (int i = 0; i < triangles.Count; i++)
            {
                if (splitPoints[firstIndex] == triangles[i].Vertex[0] ||
                            splitPoints[firstIndex] == triangles[i].Vertex[1] ||
                            splitPoints[firstIndex] == triangles[i].Vertex[2])
                {
                    firstT = true;
                }

                if (splitPoints[secondIndex] == triangles[i].Vertex[0] ||
                    splitPoints[secondIndex] == triangles[i].Vertex[1] ||
                    splitPoints[secondIndex] == triangles[i].Vertex[2])
                {
                    secondT = true;
                }
            }

            if (firstT && secondT)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Переворачивает общее ребро текущего треугольника с соседним,
        /// если новая точка попадает в описанную окружность текущего треугольника. 
        /// Затем рекурсивно проверяются два новых треугольника.
        /// </summary>
        /// <param name="triangle"></param>
        /// <param name="newPoint"></param>
        public static void FlipBadEdges(Triangle triangle, PointM newPoint, List<Triangle> triangles)
        {
            var tempTriangle = new Triangle[2];
            tempTriangle[0] = new Triangle();
            tempTriangle[1] = new Triangle();

            var commonEdgeIndex = GetNumEdgeWithoutPoint(triangle, newPoint);
            var edgesAndTriangleNeighbor = GetNeighborEdgeTriangleNums(commonEdgeIndex, triangle, triangles);
           
            if (edgesAndTriangleNeighbor.NumEdge != 0)
            {
                if (PointInCircleNeighborTriangle(edgesAndTriangleNeighbor, triangle, triangles))
                {
                    var firstIndexCurrentTriangle = commonEdgeIndex - 1;
                    var lastIndexCurrentTriangle = commonEdgeIndex == 3 ? 0 : commonEdgeIndex;
                    var indexNeighborTriangle = edgesAndTriangleNeighbor.NumEdge == 1 ? 2 :
                        edgesAndTriangleNeighbor.NumEdge == 2 ? 0 : 1;

                    tempTriangle[0].Vertex[0] = newPoint;
                    tempTriangle[0].Vertex[1] = triangle.Vertex[firstIndexCurrentTriangle];
                    tempTriangle[0].Vertex[2] = triangles[edgesAndTriangleNeighbor.NumTriangle].Vertex[indexNeighborTriangle];

                    triangles.Add(tempTriangle[0]);

                    tempTriangle[1].Vertex[0] = triangles[edgesAndTriangleNeighbor.NumTriangle].Vertex[indexNeighborTriangle];
                    tempTriangle[1].Vertex[1] = triangle.Vertex[lastIndexCurrentTriangle];
                    tempTriangle[1].Vertex[2] = newPoint;

                    triangles.Add(tempTriangle[1]);

                    triangles.Remove(triangle);
                    triangles.Remove(triangles[edgesAndTriangleNeighbor.NumTriangle]);

                    FlipBadEdges(triangles[triangles.Count - 2], newPoint, triangles);
                    FlipBadEdges(triangles[triangles.Count - 1], newPoint, triangles);
                }
            }
        }
    
        /// <summary>
        /// Определяет лежит ли ребро треугольника на границе области, 
        /// если лежит, то возращает номер границы.
        /// </summary>
        /// <param name="firstVertex"></param>
        /// <param name="secondVertex"></param>
        /// <param name="pointsArea"></param>
        /// <returns></returns>
        public static int EdgeTriangleOnBorder(PointM firstVertex, PointM secondVertex, PointM[] pointsArea)
        {
            for (int i = 0; i < pointsArea.Length; i++)
            {
                var index = i == pointsArea.Length - 1 ? 0 : i + 1;

                if (((GeometryUtility.GetDistancePoints(pointsArea[i], firstVertex) +
                        GeometryUtility.GetDistancePoints(firstVertex, pointsArea[index])) ==
                        GeometryUtility.GetDistancePoints(pointsArea[i], pointsArea[index])) &&

                        ((GeometryUtility.GetDistancePoints(pointsArea[i], secondVertex) +
                        GeometryUtility.GetDistancePoints(secondVertex, pointsArea[index])) ==
                        GeometryUtility.GetDistancePoints(pointsArea[i], pointsArea[index])))
                {
                    return i;
                }             
            }
            return -1;
        }
    }
}
