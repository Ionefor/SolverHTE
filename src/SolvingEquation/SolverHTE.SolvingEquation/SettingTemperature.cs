using SolverHTE.Nodes;
using System.Runtime.CompilerServices;
using SolverHTE.Nodes.Structs;

[assembly: InternalsVisibleTo("SolverHTE.Solver")]
namespace SolverHTE.SolvingEquation
{
    internal class SettingTemperature
    {
        /// <summary>
        /// Список температур, заданных на определенных границах.
        /// </summary>
        private readonly List<TemperatureBorder> _temperatureBorder;

        /// <summary>
        /// Номера узлов и границ на которых они находятся, 
        /// в этих узлах заданы начальные значения температур.
        /// </summary>
        private List<BorderNodeNums> _borderNodeNums;

        /// <summary>
        /// Глобальная матрица жесткости.
        /// </summary>
        private double[,] _globalMatrixK;

        /// <summary>
        /// Глобальный вектор нагрузки.
        /// </summary>
        private double[] _globalVectorF;

        /// <summary>
        /// <inheritdoc cref="_globalMatrixK"/>
        /// </summary>
        public double[,] GlobalMatrixK => _globalMatrixK;

        /// <summary>
        /// <inheritdoc cref="_globalVectorF"/>
        /// </summary>
        public double[] GlobalVectorF => _globalVectorF;

        public SettingTemperature(double[,] matrixK, double[] vectorF, List<TemperatureBorder>  temperatureBorder,
            List<BorderNodeNums> borderNodeNums)
        {
            _globalMatrixK = matrixK;
            _globalVectorF = vectorF;
            _temperatureBorder = temperatureBorder;
            _borderNodeNums = borderNodeNums;
        }

        /// <summary>
        /// Устанавливает заданную температуру на границах, в нужные узлы.
        /// </summary>
        public void SetTemperatureNode()
        {
            for (int i = 0; i < _borderNodeNums.Count; i++)
            {
                var temperatureNode = _globalMatrixK[_borderNodeNums[i].NumNode, _borderNodeNums[i].NumNode];

                _globalMatrixK[_borderNodeNums[i].NumNode, _borderNodeNums[i].NumNode] =
                    _globalMatrixK[_borderNodeNums[i].NumNode, _borderNodeNums[i].NumNode] * 1000000000000000000000.0;

                _globalVectorF[_borderNodeNums[i].NumNode] = temperatureNode *
                    _temperatureBorder[_borderNodeNums[i].NumBorder].Temperature * 1000000000000000000000.0;
            }
        }
    }
}
