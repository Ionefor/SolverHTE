using SolverHTE.Triangulation.Structs;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SolverHTE.Solver")]
namespace SolverHTE.Nodes
{
    /// <summary>
    /// В данном классе выполняется нумерация всех узлов сетки, 
    /// а так же их сопоставление с вершинами треугольников, образующих триангуляцию.
    /// </summary>
    internal class NumberNodes
    {
        /// <summary>
        /// Список треугольников, образующих триангуляцию.
        /// </summary>
        private readonly List<Triangle> _triangles;

        /// <summary>
        /// Список узлов сетки.
        /// </summary>
        private List<GridNode> _gridNodes = new();

        /// <summary>
        /// <inheritdoc cref="_gridNodes"/>
        /// </summary>
        public List<GridNode> GridNodes { get => _gridNodes; }

        public NumberNodes(List<Triangle> triangles) => _triangles = triangles;
    
        /// <summary>
        /// Выполняет нумерацию узлов сетки.
        /// </summary>
        public void DeterminingNodeNumbers()
        {
            var countNodes = 1;
            var points = new HashSet<PointM>();

            for (int i = 0; i < _triangles.Count; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    points.Add(_triangles[i].Vertex[j]);
                }             
            }

            var uniquePoints = new List<PointM>();
            uniquePoints.AddRange(points.ToList());

            while (uniquePoints.Count > 0)
            {
                countNodes = NumberingNodes(uniquePoints, countNodes);
            }

            MappingNodesVertex();
        }

        /// <summary>
        /// Сопоставляет узлы сетки с вершинами треугольников, образующих триангуляцию области.
        /// </summary>
        private void MappingNodesVertex()
        {
            var currentTriangle = new Triangle();

            for (int i = 0; i < _triangles.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var numNode = NumberNodeIsVertex(_triangles[i].Vertex[j]);

                    if (numNode != -1)
                    {
                        currentTriangle = _triangles[i];
                        currentTriangle.NodesNumber[j] = numNode;
                        _triangles[i] = currentTriangle;
                    }
                }
            }
        }

        /// <summary>
        /// Выполняет нумерацию узлов сетки, начиная с узлов с наименьшем значением координаты Х, 
        /// и по возростанию координаты У.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="countNodes"></param>
        /// <returns></returns>
        private int NumberingNodes(List<PointM> points, int countNodes)
        {
            var minX = 100000m;
            var currentNode = new GridNode();
            var indexesPoint = new List<int>();
            var leftPoints = new List<PointM>();

            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].X < minX)
                {
                    minX = points[i].X;
                }
            }

            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].X == minX)
                {
                    indexesPoint.Add(i);
                    leftPoints.Add(points[i]);
                }
            }

            leftPoints.Sort(ComparisonPointsY);

            for (int i = 0; i < leftPoints.Count; i++)
            {
                currentNode.Node = leftPoints[i];
                currentNode.NumNode = countNodes;
                countNodes++;

                _gridNodes.Add(currentNode);
            }

            foreach (int index in indexesPoint.OrderByDescending(n => n))
            {
                points.RemoveAt(index);
            }

            return countNodes;
        }

        /// <summary>
        /// Сравнение точек по координате Y.
        /// </summary>
        /// <param name="firstPoint"></param>
        /// <param name="secondPoint"></param>
        /// <returns></returns>
        private int ComparisonPointsY(PointM firstPoint, PointM secondPoint)
        {
            if (firstPoint.Y <= secondPoint.Y)
            {
                return -1;
            }

            return 1;
        }

        /// <summary>
        /// Находит узел сетки, соответсвующий заданной вершине треугольника.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns>Номер узла сетки.</returns>
        private int NumberNodeIsVertex(PointM vertex)
        {
            var numNode = -1;

            for (int i = 0; i < _gridNodes.Count; i++)
            {
                if (vertex == _gridNodes[i].Node)
                {
                    numNode = _gridNodes[i].NumNode;
                    break;
                }
            }
            return numNode;
        }
    }
}
