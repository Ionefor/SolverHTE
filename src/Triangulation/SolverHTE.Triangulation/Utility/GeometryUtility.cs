using System.Runtime.CompilerServices;
using SolverHTE.Triangulation.Structs;

[assembly: InternalsVisibleTo("SolverHTE.Nodes")]
[assembly: InternalsVisibleTo("SolverHTE.SolvingEquation")]

namespace SolverHTE.Triangulation.Utility
{
    /// <summary>
    /// Данный класс предоставляет методы для выполнения геометрических операций.
    /// </summary>
    internal static class GeometryUtility
    {
        /// <summary>
        /// Составляет два вектора по точкам, с заданными индексами.
        /// </summary>
        /// <param name="firstIndex"></param>
        /// <param name="secondIndex"></param>
        /// <param name="thirdIndex"></param>
        /// <param name="points"></param>
        /// <returns>Массив двух векторов.</returns>
        public static PointM[] GetVectors(int firstIndex, int secondIndex, int thirdIndex, List<PointM> points)
        {
            var vectors = new PointM[2];

            vectors[0].X = points[firstIndex].X - points[secondIndex].X;
            vectors[0].Y = points[firstIndex].Y - points[secondIndex].Y;

            vectors[1].X = points[thirdIndex].X - points[secondIndex].X;
            vectors[1].Y = points[thirdIndex].Y - points[secondIndex].Y;

            return vectors;
        }

        /// <summary>
        /// Составляет два вектора по сторонам треугольника, в зависимости от заданной строны.
        /// </summary>
        /// <param name="triangle"></param>
        /// <param name="sideNums"></param>
        /// <returns>Массив двух векторов</returns>
        public static PointM[] GetVectors(Triangle triangle, int sideNums)
        {
            var vectors = new PointM[2];

            var prevIndex = sideNums - 1;
            var currentIndex = sideNums == 3 ? 0 : sideNums;
            var nextIndex = currentIndex == 2 ? 0 : currentIndex + 1;

            vectors[0].X = triangle.Vertex[currentIndex].X - triangle.Vertex[prevIndex].X;
            vectors[0].Y = triangle.Vertex[currentIndex].Y - triangle.Vertex[prevIndex].Y;

            vectors[1].X = triangle.Vertex[nextIndex].X - triangle.Vertex[prevIndex].X;
            vectors[1].Y = triangle.Vertex[nextIndex].Y - triangle.Vertex[prevIndex].Y;

            return vectors;
        }

        /// <summary>
        /// Вычисляет точку, которая является центром описанной окружности треугольника.
        /// </summary>
        /// <param name="triangle"></param>
        /// <returns></returns>
        public static PointM GetCenterPointCircleTriangle(Triangle triangle)
        {
            PointM centerCirclePoint = new();

            centerCirclePoint.X = -(triangle.Vertex[0].Y * (triangle.Vertex[1].X * triangle.Vertex[1].X + triangle.Vertex[1].Y * triangle.Vertex[1].Y - triangle.Vertex[2].X * triangle.Vertex[2].X - triangle.Vertex[2].Y * triangle.Vertex[2].Y) +
                triangle.Vertex[1].Y * (triangle.Vertex[2].X * triangle.Vertex[2].X + triangle.Vertex[2].Y * triangle.Vertex[2].Y - triangle.Vertex[0].X * triangle.Vertex[0].X - triangle.Vertex[0].Y * triangle.Vertex[0].Y) +
                triangle.Vertex[2].Y * (triangle.Vertex[0].X * triangle.Vertex[0].X + triangle.Vertex[0].Y * triangle.Vertex[0].Y - triangle.Vertex[1].X * triangle.Vertex[1].X - triangle.Vertex[1].Y * triangle.Vertex[1].Y)) /
                (2 * (triangle.Vertex[0].X * (triangle.Vertex[1].Y - triangle.Vertex[2].Y) + triangle.Vertex[1].X * (triangle.Vertex[2].Y - triangle.Vertex[0].Y) + triangle.Vertex[2].X * (triangle.Vertex[0].Y - triangle.Vertex[1].Y)));

            centerCirclePoint.Y = (triangle.Vertex[0].X * (triangle.Vertex[1].X * triangle.Vertex[1].X + triangle.Vertex[1].Y * triangle.Vertex[1].Y - triangle.Vertex[2].X * triangle.Vertex[2].X - triangle.Vertex[2].Y * triangle.Vertex[2].Y) +
               triangle.Vertex[1].X * (triangle.Vertex[2].X * triangle.Vertex[2].X + triangle.Vertex[2].Y * triangle.Vertex[2].Y - triangle.Vertex[0].X * triangle.Vertex[0].X - triangle.Vertex[0].Y * triangle.Vertex[0].Y) +
               triangle.Vertex[2].X * (triangle.Vertex[0].X * triangle.Vertex[0].X + triangle.Vertex[0].Y * triangle.Vertex[0].Y - triangle.Vertex[1].X * triangle.Vertex[1].X - triangle.Vertex[1].Y * triangle.Vertex[1].Y)) /
               (2 * (triangle.Vertex[0].X * (triangle.Vertex[1].Y - triangle.Vertex[2].Y) + triangle.Vertex[1].X * (triangle.Vertex[2].Y - triangle.Vertex[0].Y) + triangle.Vertex[2].X * (triangle.Vertex[0].Y - triangle.Vertex[1].Y)));

            return centerCirclePoint;
        }

        /// <summary>
        /// Вычисляет углы заданного треугольника.
        /// </summary>
        /// <param name="triangle"></param>
        /// <returns>Массив углов треугольника.</returns>
        public static decimal[] GetAngelsTriangle(Triangle triangle)
        {
            var angles = new decimal[3];

            for (int i = 0; i < 3; i++)
            {
                var vectors = GetVectors(triangle, i + 1);
                angles[i] = GetAngleVectors(vectors[0], vectors[1]);
            }

            return angles;
        }

        /// <summary>
        /// Вычисляет максимальный угол в треугольнике и возвращает вершину, лежащую напротив него.
        /// </summary>
        /// <param name="triangle"></param>
        /// <returns>Вершина, лежащая напротив наибольшего угла</returns>
        public static PointM VertexMaxAngle(Triangle triangle)
        {
            var angles = GetAngelsTriangle(triangle);

            return angles[0] >= angles[1] ? angles[0] >= angles[2] ? triangle.Vertex[0] :
                triangle.Vertex[2] : angles[1] >= angles[2] ? triangle.Vertex[1] : triangle.Vertex[2];
        }

        /// <summary>
        /// Вычисляет максимальный угол, и возращает номер стороны напротив него.
        /// </summary>
        /// <param name="triangle"></param>
        /// <returns></returns>
        public static int GetNumSideTriangle(Triangle triangle)
        {
            var angles = GetAngelsTriangle(triangle);

            return angles[0] >= angles[1] ? angles[0] >= angles[2] ? 2 : 1
                : angles[1] >= angles[2] ? 3 : 1;
        }

        /// <summary>
        /// Вычисляет угол между заданными векторами.
        /// </summary>
        /// <param name="firstVector"></param>
        /// <param name="secondVector"></param>
        /// <returns>Угол между векторами.returns>
        public static decimal GetAngleVectors(PointM firstVector, PointM secondVector)
        {
            var result = 180 * Math.Acos((double)(firstVector.X * secondVector.X + firstVector.Y * secondVector.Y) /
                Math.Sqrt((double)((firstVector.X * firstVector.X + firstVector.Y * firstVector.Y) *
                        (secondVector.X * secondVector.X + secondVector.Y * secondVector.Y)))) / Math.PI;

            return (decimal)result;
        }

        /// <summary>
        /// Вычисляет расстояние между двумя заданными точками.
        /// </summary>
        /// <param name="firstPoint"></param>
        /// <param name="secondPoint"></param>
        /// <returns>Расстояние между двумя точками.</returns>
        public static decimal GetDistancePoints(PointM firstPoint, PointM secondPoint)
        {
            return (decimal)Math.Sqrt(
                (double)((firstPoint.X - secondPoint.X) * (firstPoint.X - secondPoint.X) + 
                (firstPoint.Y - secondPoint.Y) * (firstPoint.Y - secondPoint.Y)));
        }

        /// <summary>
        /// Вычисляет длину самого короткого ребра треугольника.
        /// </summary>
        /// <param name="triangle"></param>
        /// <returns>Длина кратчайшего ребра.</returns>
        public static decimal ShortestEdgeTriangle(Triangle triangle)
        {
            var edgesTriangle = new decimal[3];

            for (int i = 0; i < 3; i++)
            {
                var nextIndex = i == 2 ? 0 : i + 1;
                edgesTriangle[i] = GetDistancePoints(triangle.Vertex[i], triangle.Vertex[nextIndex]);
            }

            return edgesTriangle[0] < edgesTriangle[1] ?
                edgesTriangle[0] < edgesTriangle[2] ? edgesTriangle[0] : edgesTriangle[2] :
                edgesTriangle[1] < edgesTriangle[2] ? edgesTriangle[1] : edgesTriangle[2];
        }

        /// <summary>
        /// Вычисляет длину самого короткого ребра из всего списка треугольников.
        /// </summary>
        /// <param name="triangles"></param>
        /// <returns>Длина самого короткого ребра.</returns>
        public static decimal ShortestEdgeTriangles(List<Triangle> triangles)
        {
            var minLenght = 1000000m;
            var currentLenght = 0m;

            for (int i = 0; i < triangles.Count; i++)
            {
                currentLenght = ShortestEdgeTriangle(triangles[i]);

                if (currentLenght < minLenght)
                {
                    minLenght = currentLenght;
                }
            }

            return minLenght;
        }

        /// <summary>
        /// Вычисляет сумму длин сторон треугольника.
        /// </summary>
        /// <param name="triangle"></param>
        /// <returns>Сумма сторон треугольника.</returns>
        public static decimal GetSumSidesTriangle(Triangle triangle)
        {
            var sideLenght = new decimal[3];

            sideLenght[0] = GetDistancePoints(triangle.Vertex[0], triangle.Vertex[1]);
            sideLenght[1] = GetDistancePoints(triangle.Vertex[1], triangle.Vertex[2]);
            sideLenght[2] = GetDistancePoints(triangle.Vertex[2], triangle.Vertex[0]);

            return sideLenght[0] + sideLenght[1] + sideLenght[2];
        }

        /// <summary>
        /// Вычисляет произведение длин сторон треугольника.
        /// </summary>
        /// <param name="triangle"></param>
        /// <returns></returns>
        public static decimal GetProductSidesTriangle(Triangle triangle)
        {
            var sideLenght = new decimal[3];

            sideLenght[0] = GetDistancePoints(triangle.Vertex[0], triangle.Vertex[1]);
            sideLenght[1] = GetDistancePoints(triangle.Vertex[1], triangle.Vertex[2]);
            sideLenght[2] = GetDistancePoints(triangle.Vertex[2], triangle.Vertex[0]);

            return sideLenght[0] * sideLenght[1] * sideLenght[2];
        }

        /// <summary>
        /// Вычисляет радиус описанной окружности треугольника.
        /// </summary>
        /// <param name="triangle"></param>
        /// <returns>Радиус описанной окружности.</returns>
        public static decimal GetCircumRadius(Triangle triangle)
        {
            return GetProductSidesTriangle(triangle) / (4 * GetSquareTriangle(triangle));
        }

        /// <summary>
        /// Вычисляет отношение радиуса описанной окружности к самому короткому ребру треугольника.
        /// </summary>
        /// <param name="triangle"></param>
        /// <returns></returns>
        public static decimal GetCircumRadiusShortestEdgeRatio(Triangle triangle)
        {
            return GetCircumRadius(triangle) / ShortestEdgeTriangle(triangle);
        }

        /// <summary>
        /// Вычисляет площадь треугольника.
        /// </summary>
        /// <param name="triangle"></param>
        /// <returns>Площадь треугольника.</returns>
        public static decimal GetSquareTriangle(Triangle triangle)
        {
            return Math.Abs(
                triangle.Vertex[0].X * triangle.Vertex[1].Y + triangle.Vertex[1].X * triangle.Vertex[2].Y + 
                triangle.Vertex[2].X * triangle.Vertex[0].Y - (triangle.Vertex[1].X * triangle.Vertex[0].Y + 
                triangle.Vertex[2].X * triangle.Vertex[1].Y + triangle.Vertex[0].X * triangle.Vertex[2].Y)) / 2;
        }

        /// <summary>
        /// Вычисляет длину заданной стороны треугольника.
        /// </summary>
        /// <param name="numSide"></param>
        /// <param name="triangle"></param>
        /// <returns></returns>
        public static decimal GetSideLenght(int numSide, Triangle triangle)
        {
            var index = numSide == 3 ? 0 : numSide;

            return GetDistancePoints(triangle.Vertex[numSide - 1], triangle.Vertex[index]);
        }

        /// <summary>
        /// Проверяет содержится ли точка в диаметральной окружности треугольника.
        /// </summary>
        /// <param name="triangle"></param>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns>Истину, если точка содержится, и ложь в ином случае.</returns>
        public static bool PointIsDiametralCircle(Triangle triangle, PointM startPoint, PointM endPoint)
        {
            PointM midPoint = new()
            {
                X = (startPoint.X + endPoint.X) / 2,
                Y = (startPoint.Y + endPoint.Y) / 2
            };

            if (GetDistancePoints(GetCenterPointCircleTriangle(triangle), midPoint) <=
                GetDistancePoints(midPoint, startPoint))
            {
                return true;
            }
            return false;
        }
    }
}
