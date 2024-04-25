using SolverHTE.SolvingEquation.Structs;
using SolverHTE.Triangulation.Structs;
using SolverHTE.Triangulation.Utility;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SolverHTE.Solver")]
namespace SolverHTE.SolvingEquation
{
    internal class MatrixK
    {
        /// <summary>
        /// Коэффициент теплопроводности в направлении х.
        /// </summary>
        private readonly double _Kxx;

        /// <summary>
        /// Коэффициент теплопроводности в направлении у.
        /// </summary>
        private readonly double _Kyy;

        /// <summary>
        /// Количество узлов сетки.
        /// </summary>
        private readonly int _countNodes;

        /// <summary>
        /// Начальные точки области.
        /// </summary>
        private readonly PointM[] _pointsArea;

        /// <summary>
        /// Список параметров, заданных на определенных границах.
        /// </summary>
        private List<BorderData> _borderData;

        /// <summary>
        /// Список треугольников, образующих триангуляцию.
        /// </summary>
        private readonly List<Triangle> _triangles;

        /// <summary>
        /// Глобальная матрица жесткости.
        /// </summary>
        private double[,] _matrixK;

        /// <summary>
        /// <inheritdoc cref="_matrixK"/>
        /// </summary>
        public double[,] GlobalMatrixK => _matrixK;
        public MatrixK(double Kxx, double Kyy, int countNodes, PointM[] pointsArea,
            List<BorderData> borderData, List<Triangle> triangles)
        {
            _Kxx = Kxx;
            _Kyy = Kyy;
            _borderData = borderData;
            _triangles = triangles;
            _countNodes = countNodes;
            _pointsArea = pointsArea;
            _matrixK = new double[_countNodes, _countNodes];
        }

        /// <summary>
        /// Строит глобальную матрицу жесткости.
        /// </summary>
        public void Build()
        {
            for (int i = 0; i < _triangles.Count; i++)
            {
                var currentSquare = (double)GeometryUtility.GetSquareTriangle(_triangles[i]);
                var firstIntegral = CalculateFirstIntegral(_triangles[i], currentSquare);
                var secondIntegral = CalculateSecondIntegral(_triangles[i]);
                var localMatrixK = CompilationLocalMatrix(firstIntegral, secondIntegral);

                CompilationGlobalMatrix(_triangles[i], localMatrixK);
            }
        }

        /// <summary>
        /// Вычисляет первый интеграл локальной матрицы жесткости.
        /// </summary>
        /// <param name="triangle"></param>
        /// <param name="squareTriangle"></param>
        /// <returns></returns>
        private double[,] CalculateFirstIntegral(Triangle triangle, double squareTriangle)
        {          
            var integral = new double[3, 3];

            var b = CalculateCoefficientsB(triangle);
            var c = CalculateCoefficientsC(triangle);

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    integral[i, j] = (_Kxx / (4 * squareTriangle)) * (b[i] * b[j]) + 
                        (_Kyy / (4 * squareTriangle)) * (c[i] * c[j]);
                }
            }
            return integral;
        }

        /// <summary>
        /// Вычисляет второй интеграл локальной матрицы жесткости.
        /// </summary>
        /// <param name="triangle"></param>
        /// <returns></returns>
        private double[,] CalculateSecondIntegral(Triangle triangle)
        {
            var numBorder = new int[3];
            var sideLength = new double[3];
            var integral = new double[3, 3];
            
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
                    var index = i == 0 ? 2 : (i == 1 ? 0 : 1);

                    for (int j = 0; j < 3; j++)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            if (j == index || k == index)
                            {
                                continue;
                            }
                            else if (j == k)
                            {
                                integral[j, k] += (_borderData[numBorder[i]].h * sideLength[i]) / 3;
                            }
                            else
                            {
                                integral[j, k] += (_borderData[numBorder[i]].h * sideLength[i]) / 6;
                            }
                        }
                    }                  
                }
            }

            return integral;
        }

        /// <summary>
        /// Собирает локальную матрицу, суммированием двух интегралов.
        /// </summary>
        /// <param name="firstI"></param>
        /// <param name="secondI"></param>
        /// <returns></returns>
        private static double[,] CompilationLocalMatrix(double[,] firstI, double[,] secondI)
        {
            var localMatrix = new double[3, 3];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    localMatrix[i, j] = firstI[i, j] + secondI[i, j];
                }
            }

            return localMatrix;
        }

        /// <summary>
        /// Добавляет локальную матрицу в глобальную, 
        /// путем суммирования элементов с совпадающими индексами.
        /// </summary>
        /// <param name="triangle"></param>
        /// <param name="localMatrix"></param>
        private void CompilationGlobalMatrix(Triangle triangle, double[,] localMatrix)
        {
            for (int i = 0; i < _countNodes; i++)
            {
                for (int j = 0; j < _countNodes; j++)
                {
                    for(int k = 0; k < 3; k++)
                    {
                        for (int n = 0; n < 3; n++)
                        {
                            if (i == (triangle.NodesNumber[k] - 1) && j == (triangle.NodesNumber[n] - 1))
                            {
                                _matrixK[i, j] += localMatrix[k, n];
                            }
                        }
                    }                                     
                }
            }
        }

        /// <summary>
        /// Высляет коэффициенты b для локальной матрицы жесткости.
        /// </summary>
        /// <param name="triangle"></param>
        /// <returns></returns>
        private double[] CalculateCoefficientsB(Triangle triangle)
        {
            var b = new double[3];

            b[0] = (double)(triangle.Vertex[1].Y - triangle.Vertex[2].Y);
            b[1] = (double)(triangle.Vertex[2].Y - triangle.Vertex[0].Y);
            b[2] = (double)(triangle.Vertex[0].Y - triangle.Vertex[1].Y);

            return b;
        }

        /// <summary>
        /// Высляет коэффициенты с для локальной матрицы жесткости.
        /// </summary>
        /// <param name="triangle"></param>
        /// <returns></returns>
        private double[] CalculateCoefficientsC(Triangle triangle)
        {
            var c = new double[3];

            c[0] = (double)(triangle.Vertex[2].X - triangle.Vertex[1].X);
            c[1] = (double)(triangle.Vertex[0].X - triangle.Vertex[2].X);
            c[2] = (double)(triangle.Vertex[1].X - triangle.Vertex[0].X);

            return c;
        }
    }
}
