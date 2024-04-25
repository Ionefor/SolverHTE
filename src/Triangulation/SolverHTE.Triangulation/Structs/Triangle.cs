namespace SolverHTE.Triangulation.Structs
{
    /// <summary>
    /// Представляет треугольник, определяемый вершинами и соответсвующими им номерами узлов сетки,
    /// является конечным элементом триангуляции.
    /// </summary>
    public struct Triangle
    {
        /// <summary>
        /// Вершины треугольника.
        /// </summary>
        public PointM[]? Vertex = new PointM[3];

        /// <summary>
        /// Номера узлов сетки, соответсвующие вершинам треугольника.
        /// </summary>
        public int[]? NodesNumber = new int[3];

        public Triangle(params PointM[] vertex) => Vertex = vertex;
        public Triangle() { }
    }
}
