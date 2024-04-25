using SolverHTE.Triangulation.Structs;

namespace SolverHTE.Nodes
{
    /// <summary>
    /// Представляет собой узел сетки
    /// </summary>
    public struct GridNode
    {
        /// <summary>
        /// Точка области представляющая собой, координаты узла сетки.
        /// </summary>
        public PointM Node { get; set; }

        /// <summary>
        /// Номер узла сетки.
        /// </summary>
        public int NumNode { get; set; }
    }
}
