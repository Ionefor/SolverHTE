using MathNet.Numerics.LinearAlgebra.Double;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SolverHTE.Solver")]
namespace SolverHTE.SolvingEquation
{
    /// <summary>
    /// В данном классе вычисляется искомый столбец температур.
    /// </summary>
    internal class SolverEquation
    {
        /// <summary>
        /// Глобальный вектор нагрузки.
        /// </summary>
        private double[] _vectorF;

        /// <summary>
        /// Глобальная матрица жесткости.
        /// </summary>
        private double[,] _matrixK;

        /// <summary>
        /// Искомый столбец температур.
        /// </summary>
        private double[] _temperature;

        /// <summary>
        /// <inheritdoc cref="_temperature"/>
        /// </summary>
        public double[] Temperature => _temperature;

        public SolverEquation(double[,] matrixK, double[] vectorF)
        {
            _vectorF = vectorF;
            _matrixK = matrixK;
        }

        /// <summary>
        /// Вычисляет столбец температур.
        /// </summary>
        public void Solve()
        {
            var globalMatrixK = new SparseMatrix(_vectorF.Length, _vectorF.Length);
            var globalVectorF = new double[_vectorF.Length];

            for (int i = 0; i < globalVectorF.Length; i++)
            {
                globalVectorF[i] = _vectorF[i];

                for (int j = 0; j < globalVectorF.Length; j++)
                {
                    globalMatrixK[i, j] = _matrixK[i, j];
                }
            }

            _temperature = globalMatrixK.Solve(
                DenseVector.Build.DenseOfArray(globalVectorF)).ToArray();
        }     
    }
}
