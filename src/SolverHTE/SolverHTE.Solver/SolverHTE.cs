using SolverHTE.Nodes;
using SolverHTE.Nodes.Structs;
using SolverHTE.SolvingEquation;
using SolverHTE.SolvingEquation.Structs;
using SolverHTE.Triangulation;
using SolverHTE.Triangulation.Structs;

namespace SolverHTE.Solver
{
    public class SolverHTE
    {
        /// <summary>
        ///Источник тепла внутри тела
        /// </summary>
        private readonly double _Q;

        /// <summary>
        /// Коэффициент теплопроводности в направлении х.
        /// </summary>
        private readonly double _Kxx;

        /// <summary>
        /// Коэффициент теплопроводности в направлении у.
        /// </summary>
        private readonly double _Kyy;

        /// <summary>
        /// Размер сетки.
        /// </summary>
        private int _sizeGrid;

        /// <summary>
        /// Начальные точки области.
        /// </summary>
        private PointM[] _points;
       
        /// <summary>
        /// Температура на заданных границах.
        /// </summary>
        private List<TemperatureBorder> _temperatureBorder;

        /// <summary>
        /// Список параметров, заданных на определенных границах.
        /// </summary>
        private List<BorderData> _borderData;

        /// <summary>
        /// Список треугольников, образующих триангуляцию.
        /// </summary>
        public List<Triangle> Triangles { get; private set; }

        /// <summary>
        /// Список узлов сетки.
        /// </summary>
        public List<GridNode> GridNodes { get; private set; }

        public SolverHTE(double Q, double Kxx, double Kyy, int sizeGrid, PointM[] points,
            List<TemperatureBorder> temperatureBorder, List<BorderData> borderData)
        {
            _Q = Q;
            _Kxx = Kxx;
            _Kyy = Kyy;
            _sizeGrid = sizeGrid;
            _points = points;
            _temperatureBorder = temperatureBorder;
            _borderData = borderData;
        }

        /// <summary>
        /// Решает двумерное уравнение теплопроводности, для заданной области с заданными параметрами. 
        /// Использует метод конечных элементов, так же выполняет триангуляцию области.
        /// </summary>
        /// <returns>Искомый столбец температур.</returns>
        public double[] Solve()
        {
            var divisionArea = new DivisionArea(_points);
            divisionArea.Division();

            var triangulation = new Triangulation.Triangulation(divisionArea.TrianglesDivision, _sizeGrid);
            triangulation.TriangulationArea();
            Triangles = triangulation.Triangles;

            var numNodes = new NumberNodes(triangulation.Triangles);
            numNodes.DeterminingNodeNumbers();
            GridNodes = numNodes.GridNodes;

            var borderNodes = new FoundNodes(numNodes.GridNodes, _points, _temperatureBorder);
            borderNodes.FoundNodesNumber();

            var matrix = new MatrixK(_Kxx, _Kyy, numNodes.GridNodes.Count, _points, _borderData, triangulation.Triangles);
            var vector = new VectorF(_Q, numNodes.GridNodes.Count, _points, _borderData, triangulation.Triangles);

            matrix.Build();
            vector.Build();

            var settingTemperature = new SettingTemperature(matrix.GlobalMatrixK, vector.GlobalVectorF, 
                _temperatureBorder, borderNodes.BorderNodes);
            settingTemperature.SetTemperatureNode();

            var solvingEq = new SolverEquation(settingTemperature.GlobalMatrixK, settingTemperature.GlobalVectorF);
            solvingEq.Solve();

            return solvingEq.Temperature;
        }
    }
}
