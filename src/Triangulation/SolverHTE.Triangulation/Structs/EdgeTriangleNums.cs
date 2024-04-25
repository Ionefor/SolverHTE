namespace SolverHTE.Triangulation.Structs
{
    /// <summary>
    /// Представляет собой пару номера треугольника и его ребра.
    /// </summary>
    internal struct EdgeTriangleNums
    {
        /// <summary>
        /// Номер треугольника.
        /// </summary>
        public int NumTriangle { get; set; }

        /// <summary>
        /// Номер ребра треугольника.
        /// </summary>
        public int NumEdge { get; set; }
    }
}
