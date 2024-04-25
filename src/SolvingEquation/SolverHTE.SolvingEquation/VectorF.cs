using SolverHTE.SolvingEquation.Structs;
using SolverHTE.Triangulation.Structs;
using SolverHTE.Triangulation.Utility;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SolverHTE.Solver")]
namespace SolverHTE.SolvingEquation
{
    /// <summary>
    /// В данном классе строится глобальный вектор-столбец нагрузки.
    /// </summary>
    internal class VectorF
    {
        /// <summary>
        ///Источник тепла внутри тела
        /// </summary>
        private readonly double _Q;

        /// <summary>
        /// Количество узлов сетки.
        /// </summary>
        private readonly int _countNodes;        

        /// <summary>
        /// Список параметров, заданных на определенных границах.
        /// </summary>
        private List<BorderData> _borderData;        

        /// <summary>
        /// Список треугольников, образующих триангуляцию.
        /// </summary>
        private readonly List<Triangle> _triangles;

        /// <summary>
        /// Начальные точки области.
        /// </summary>
        private readonly PointM[] _pointsArea;

        /// <summary>
        /// Глобальный вектор-столбец нагрузки.
        /// </summary>
        private double[] _vectorF;

        /// <summary>
        /// <inheritdoc cref="_vectorF"/>
        /// </summary>
        public double[] GlobalVectorF => _vectorF;
        public VectorF(double Q, int countNodes, PointM[] pointsArea,
            List<BorderData> borderData, List<Triangle> triangles)
        {
            _Q = Q;
            _borderData = borderData;
            _triangles = triangles;
            _countNodes = countNodes;
            _pointsArea = pointsArea;
            _vectorF = new double[_countNodes];
        }

        /// <summary>
        /// Строит глобальный вектор нагрузки.
        /// </summary>
        public void Build()
        {
            for(int i = 0; i < _triangles.Count; i++)
            {
                var currentSquare = (double)GeometryUtility.GetSquareTriangle(_triangles[i]);
                var firstIntegral = CalculateFirstIntegral(currentSquare);
                var secondIntegral = CalculateSecondIntegral(_triangles[i]);
                var localVectorF = CompilationLocalVector(firstIntegral, secondIntegral);

                CompilationGlobalVector(_triangles[i], localVectorF);
            }

        }

        /// <summary>
        /// Вычисляет первый интеграл вектора нагрузки.
        /// </summary>
        /// <param name="squareTriangle"></param>
        /// <returns></returns>
        private double[] CalculateFirstIntegral(double squareTriangle)
        {
            var integral = new double[3];

            for (int i = 0; i < 3; i++)
            {
                integral[i] = -(_Q * squareTriangle) / 3;
            }

            return integral;
        }

        /// <summary>
        /// Вычисляет второй интеграл вектора нагрузки.
        /// </summary>
        /// <param name="triangle"></param>
        /// <returns></returns>
        private double[] CalculateSecondIntegral(Triangle triangle)
        {
            var numBorder = new int[3];
            var sideLength = new double[3];
            var integral = new double[3];

            numBorder[0] = TriangulationUtility.EdgeTriangleOnBorder(
                triangle.Vertex[0], triangle.Vertex[1], _pointsArea);

            numBorder[1] = TriangulationUtility.EdgeTriangleOnBorder(
                triangle.Vertex[1], triangle.Vertex[2], _pointsArea);

            numBorder[2] = TriangulationUtility.EdgeTriangleOnBorder(
                triangle.Vertex[2], triangle.Vertex[0], _pointsArea);

            sideLength[0] = (double)GeometryUtility.GetDistancePoints(
                triangle.Vertex[0], triangle.Vertex[1]);

            sideLength[1] = (double)GeometryUtility.GetDistancePoints(
                triangle.Vertex[1], triangle.Vertex[2]);

            sideLength[2] = (double)GeometryUtility.GetDistancePoints(
                triangle.Vertex[2], triangle.Vertex[0]);

            for (int i = 0; i < numBorder.Length; i++)
            {
                if (numBorder[i] != -1)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        var index = i == 0 ? 2 : (i == 1 ? 0 : 1);

                        if (j != index)
                        {
                            integral[j] += sideLength[i] * (_borderData[numBorder[i]].q -
                                _borderData[numBorder[i]].h * _borderData[numBorder[i]].T_inf) / 2;
                        }
                    }                   
                }
            }

            return integral;
        }

        /// <summary>
        /// Собирает локальный вектор, суммированием двух интегралов.
        /// </summary>
        /// <param name="firstIntegral"></param>
        /// <param name="secondIntegral"></param>
        /// <returns></returns>
        private static double[] CompilationLocalVector(double[] firstIntegral, double[] secondIntegral)
        {
            var localVector = new double[3];

            for (int i = 0; i < 3; i++)
            {
                localVector[i] = firstIntegral[i] + secondIntegral[i];
            }

            return localVector;
        }

        /// <summary>
        /// Добавляет локальный вектор в глобальный, 
        /// путем суммирования элементов с совпадающими индексами.
        /// </summary>
        /// <param name="triangle"></param>
        /// <param name="localVector"></param>
        private void CompilationGlobalVector(Triangle triangle, double[] localVector)
        {
            for (int i = 0; i < _countNodes; i++)
            {
                for(int j =0; j < 3; j++)
                {
                    if (i == (triangle.NodesNumber[j] - 1))
                    {
                        _vectorF[i] -= localVector[j];
                    }
                }
            }
        }      
    }
}
