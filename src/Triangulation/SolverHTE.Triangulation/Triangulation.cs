using SolverHTE.Triangulation.Structs;
using SolverHTE.Triangulation.Utility;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SolverHTE.Solver")]
namespace SolverHTE.Triangulation
{
    /// <summary>
    /// В данном классе выполняется триангуляция двумерной области.
    /// </summary>
    internal class Triangulation
    {
        /// <summary>
        /// Список треугольников, образующих триангуляцию.
        /// </summary>
        private List<Triangle> _triangles = new();

        /// <summary>
        /// Размер сетки.
        /// </summary>
        private readonly int _sizeGrid;

        /// <summary>
        ///<inheritdoc cref="_triangles"/>   
        /// </summary>
        public List<Triangle> Triangles { get => _triangles; }
        public Triangulation(List<Triangle> triangles, int sizeGrid)
        {
            _triangles = triangles;
            _sizeGrid = sizeGrid;           
        } 

        /// <summary>
        /// Выполняет триангуляцию области.
        /// </summary>
        public void TriangulationArea()
        {
            var midPoint = new PointM();
            var hInitial = GeometryUtility.ShortestEdgeTriangles(_triangles);

            step_two:;
            
            var indexWorstTriangle = FindIndexWorstTriangle(hInitial);

            if(indexWorstTriangle != -1)
            {         
                for(int i = 0; i < _triangles.Count; i++)
                {
                    if (i != indexWorstTriangle)
                    {
                        for(int j = 0; j < 3; j++)
                        {
                            var firstIndex = j;
                            var lastIndex = j == 2 ? 0 : j + 1;

                            if(GeometryUtility.PointIsDiametralCircle(_triangles[indexWorstTriangle], 
                                _triangles[i].Vertex[firstIndex], _triangles[i].Vertex[lastIndex]))
                            {
                                midPoint.X = (_triangles[i].Vertex[firstIndex].X + _triangles[i].Vertex[lastIndex].X) / 2;
                                midPoint.Y = (_triangles[i].Vertex[firstIndex].Y + _triangles[i].Vertex[lastIndex].Y) / 2;

                                AddPoint(midPoint);
                                goto step_two;
                            }
                        }
                    }
                }

                if (!TriangulationUtility.PointIsBeyondArea
                    (GeometryUtility.GetCenterPointCircleTriangle(_triangles[indexWorstTriangle]), _triangles))
                {
                    AddPoint(GeometryUtility.GetCenterPointCircleTriangle(_triangles[indexWorstTriangle]));
                }
                else
                {
                    SplitHeightTriangle(indexWorstTriangle);
                }

                goto step_two;
            }
        }

        /// <summary>
        /// Выполняет поиск наихудшего треугольника из всего списка треугольников.
        /// </summary>
        /// <param name="h">Длина наименьшего ребра, среди всех треугольников.</param>
        /// <returns>Индекс наихудшего треугольника.</returns>
        private int FindIndexWorstTriangle(decimal hInitial)
        {
            decimal h, desiredSumSides, B = (decimal)Math.Sqrt(2);
            var maxSumSides = 0m;
            var indexBadTriangle = -1;

            ///Подобрать лучшие параметры
            if (_sizeGrid == 1)
            {
                h = hInitial;
                desiredSumSides = hInitial * 3;
            }
            else if (_sizeGrid == 2)
            {
                h = hInitial / 4;
                desiredSumSides = hInitial * 2;
            }
            else
            {
                h = hInitial / 5;
                desiredSumSides = hInitial * 1.3m;
            }

            for (int i = 0; i < _triangles.Count; i++)
            {
                if (GeometryUtility.GetCircumRadiusShortestEdgeRatio(_triangles[i]) > B
                    && GeometryUtility.GetSumSidesTriangle(_triangles[i]) > desiredSumSides)
                {
                }
                else if (GeometryUtility.GetCircumRadius(_triangles[i]) > h
                    && GeometryUtility.ShortestEdgeTriangle(_triangles[i]) > h
                    && GeometryUtility.GetSumSidesTriangle(_triangles[i]) > desiredSumSides)
                {
                }
                else
                {
                    continue;
                }

                if (GeometryUtility.GetSumSidesTriangle(_triangles[i]) > maxSumSides)
                {
                    maxSumSides = GeometryUtility.GetSumSidesTriangle(_triangles[i]);
                    indexBadTriangle = i;
                }
            }

            return indexBadTriangle;
        }

        /// <summary>
        /// Добавляет новую точку в триангуляцию.
        /// </summary>
        /// <param name="addedPoint"></param>
        private void AddPoint(PointM addedPoint)
        {
            List<EdgeTriangleNums> edgesAndTriangles = TriangulationUtility.PointOnSideTriangle(addedPoint, _triangles);

            if (edgesAndTriangles.Count != 0)
            {
                for (int i = 0; i < edgesAndTriangles.Count; i++)
                {
                    SplitTriangle(edgesAndTriangles[i].NumTriangle, 
                        edgesAndTriangles[i].NumEdge, addedPoint);
                }

                if (edgesAndTriangles.Count != 1)
                {
                    if (edgesAndTriangles[0].NumTriangle > edgesAndTriangles[1].NumTriangle)
                    {
                        _triangles.RemoveAt(edgesAndTriangles[0].NumTriangle);
                        _triangles.RemoveAt(edgesAndTriangles[1].NumTriangle);
                    }
                    else
                    {
                        _triangles.RemoveAt(edgesAndTriangles[1].NumTriangle);
                        _triangles.RemoveAt(edgesAndTriangles[0].NumTriangle);
                    }

                    TriangulationUtility.FlipBadEdges(_triangles[_triangles.Count - 4], addedPoint, _triangles);
                    TriangulationUtility.FlipBadEdges(_triangles[_triangles.Count - 3], addedPoint, _triangles);
                    TriangulationUtility.FlipBadEdges(_triangles[_triangles.Count - 2], addedPoint, _triangles);
                    TriangulationUtility.FlipBadEdges(_triangles[_triangles.Count - 1], addedPoint, _triangles);
                }
                else
                {
                    _triangles.RemoveAt(edgesAndTriangles[0].NumTriangle);

                    TriangulationUtility.FlipBadEdges(_triangles[_triangles.Count - 2], addedPoint, _triangles);
                    TriangulationUtility.FlipBadEdges(_triangles[_triangles.Count - 1], addedPoint, _triangles);
                }
            }
            else
            {
                for (int i = 0; i < _triangles.Count; i++)
                {
                    if (TriangulationUtility.PointOnTriangle(_triangles[i], addedPoint))
                    {
                        SplitInnerPointTriangle(i, addedPoint);
                        _triangles.RemoveAt(i);

                        TriangulationUtility.FlipBadEdges(_triangles[_triangles.Count - 3], addedPoint, _triangles);
                        TriangulationUtility.FlipBadEdges(_triangles[_triangles.Count - 2], addedPoint, _triangles);
                        TriangulationUtility.FlipBadEdges(_triangles[_triangles.Count - 1], addedPoint, _triangles);
                        break;
                    }
                }
            }
        }
    
        /// <summary>
        /// Разбивает треугольник на три, с помощью внутренней точки.
        /// </summary>
        /// <param name="numTriangle"></param>
        /// <param name="addedPoint"></param>
        private void SplitInnerPointTriangle(int numTriangle, PointM addedPoint)
        {
            var newTriangles = new Triangle[3];
            newTriangles[0] = new Triangle();
            newTriangles[1] = new Triangle();
            newTriangles[2] = new Triangle();

            newTriangles[0].Vertex[0] = _triangles[numTriangle].Vertex[0];
            newTriangles[0].Vertex[1] = _triangles[numTriangle].Vertex[1];
            newTriangles[0].Vertex[2] = addedPoint;

            _triangles.Add(newTriangles[0]);

            newTriangles[1].Vertex[0] = _triangles[numTriangle].Vertex[0];
            newTriangles[1].Vertex[1] = addedPoint;
            newTriangles[1].Vertex[2] = _triangles[numTriangle].Vertex[2];

            _triangles.Add(newTriangles[1]);

            newTriangles[2].Vertex[0] = addedPoint;
            newTriangles[2].Vertex[1] = _triangles[numTriangle].Vertex[1];
            newTriangles[2].Vertex[2] = _triangles[numTriangle].Vertex[2];

            _triangles.Add(newTriangles[2]);
        }

        /// <summary>
        /// Разбивает треугольник на два, путем проведения высоты из наибольшего угла.
        /// </summary>
        /// <param name="triangle"></param>
        private void SplitHeightTriangle(int numTriangle)
        {
            var newPoint = TriangulationUtility.FindNewVertexHeightTriangle(_triangles[numTriangle]);
            var numSide = GeometryUtility.GetNumSideTriangle(_triangles[numTriangle]);

            SplitTriangle(numTriangle, numSide, newPoint);

            _triangles.Remove(_triangles[numTriangle]);
        }

        /// <summary>
        /// Разбивает треугольник на два с помощью точки на его ребре.
        /// </summary>
        /// <param name="numTriangle"></param>
        /// <param name="numEdge"></param>
        /// <param name="newPoint"></param>
        private void SplitTriangle(int numTriangle, int numEdge, PointM newPoint)
        {
            var newTriangles = new Triangle[2];
            newTriangles[0] = new Triangle();
            newTriangles[1] = new Triangle();

            if (numEdge == 1)
            {
                newTriangles[0].Vertex[0] = _triangles[numTriangle].Vertex[0];
                newTriangles[0].Vertex[1] = newPoint;
                newTriangles[0].Vertex[2] = _triangles[numTriangle].Vertex[2];

                _triangles.Add(newTriangles[0]);

                newTriangles[1].Vertex[0] = newPoint;
                newTriangles[1].Vertex[1] = _triangles[numTriangle].Vertex[1];
                newTriangles[1].Vertex[2] = _triangles[numTriangle].Vertex[2];

                _triangles.Add(newTriangles[1]);
            }
            else if (numEdge == 2)
            {
                newTriangles[0].Vertex[0] = _triangles[numTriangle].Vertex[0];
                newTriangles[0].Vertex[1] = _triangles[numTriangle].Vertex[1];
                newTriangles[0].Vertex[2] = newPoint;

                _triangles.Add(newTriangles[0]);

                newTriangles[1].Vertex[0] = newPoint;
                newTriangles[1].Vertex[1] = _triangles[numTriangle].Vertex[2];
                newTriangles[1].Vertex[2] = _triangles[numTriangle].Vertex[0];

                _triangles.Add(newTriangles[1]);
            }
            else if (numEdge == 3)
            {
                newTriangles[0].Vertex[0] = _triangles[numTriangle].Vertex[0];
                newTriangles[0].Vertex[1] = _triangles[numTriangle].Vertex[1];
                newTriangles[0].Vertex[2] = newPoint;

                _triangles.Add(newTriangles[0]);

                newTriangles[1].Vertex[0] = newPoint;
                newTriangles[1].Vertex[1] = _triangles[numTriangle].Vertex[1];
                newTriangles[1].Vertex[2] = _triangles[numTriangle].Vertex[2];

                _triangles.Add(newTriangles[1]);
            }
        }        
    }
}
