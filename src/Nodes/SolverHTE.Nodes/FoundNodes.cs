using SolverHTE.Nodes.Structs;
using SolverHTE.Triangulation.Structs;
using SolverHTE.Triangulation.Utility;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SolverHTE.Solver")]
namespace SolverHTE.Nodes
{
    /// <summary>
    /// В данном классе определяются узлы, которые лежат на границах с заданной температурой.
    /// </summary>
    internal class FoundNodes
    {
        /// <summary>
        /// Список узлов сетки.
        /// </summary>
        private readonly List<GridNode> _gridNodes;

        /// <summary>
        /// Температура на заданных границах.
        /// </summary>
        private readonly List<TemperatureBorder> _temperatureBorder;

        /// <summary>
        /// Начальные точки области.
        /// </summary>
        private readonly PointM[] _points;

        /// <summary>
        /// Список, содержащий номера узлов и границ, на которых они находятся.
        /// </summary>
        private readonly List<BorderNodeNums> _borderNodes = new();

        /// <summary>
        /// <inheritdoc cref="_borderNodes"/>
        /// </summary>
        public List<BorderNodeNums> BorderNodes { get => _borderNodes; }
        public FoundNodes(List<GridNode> gridNodes, PointM[] points, List<TemperatureBorder> temperatureBorder)
        {
            _gridNodes = gridNodes;
            _points = points;
            _temperatureBorder = temperatureBorder;
        }

        /// <summary>
        /// Находит номера узлов, которые лежат на заданных границах.
        /// </summary>
        public void FoundNodesNumber()
        {
            for (int i = 0; i < _gridNodes.Count; i++)
            {
                var numBorder = DeterminingBorderNum(_gridNodes[i]);

                if (BorderIsTempBorder(numBorder))
                {
                    var current = new BorderNodeNums();

                    current.NumBorder = numBorder;
                    current.NumNode = _gridNodes[i].NumNode - 1;
                    _borderNodes.Add(current);
                }
            }
        }

        /// <summary>
        /// Проверяет заданна ли температура на данной границе.
        /// </summary>
        /// <param name="numBorder"></param>
        /// <returns>Истину, если данная граница имеет заданную температуру, и ложь в ином случае.</returns>
        private bool BorderIsTempBorder(int numBorder)
        {
            for(int i = 0; i < _temperatureBorder.Count; i++)
            {
                if (_temperatureBorder[i].NumBorder == numBorder)
                {
                    return true;
                }
            }
           
            return false;
        }

        /// <summary>
        /// Определяет номер границы, на которой лежит заданный узел.
        /// </summary>
        /// <param name="node"></param>
        /// <returns>Номер узла или -1 в случае, если узел не лежит на границе области.</returns>
        private int DeterminingBorderNum(GridNode node)
        {
            for (int i = 0; i < _points.Length; i++)
            {
                var indexPoint = i == _points.Length - 1 ? 0 : i + 1;

                if ((GeometryUtility.GetDistancePoints(_points[i], node.Node) +
                       GeometryUtility.GetDistancePoints(node.Node, _points[indexPoint])) ==
                       GeometryUtility.GetDistancePoints(_points[i], _points[indexPoint]))
                {
                    return i;
                }             
            }
            return -1;
        }
    }
}
