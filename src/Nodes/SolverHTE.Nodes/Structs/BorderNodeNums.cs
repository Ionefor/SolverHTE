namespace SolverHTE.Nodes
{
    /// <summary>
    /// Представляет номер узла и номер границы, на которой он находится.
    /// </summary>
    public struct BorderNodeNums
    {
        /// <summary>
        /// Номер границы области.
        /// </summary>
        public int NumBorder { get; set; }

        /// <summary>
        /// Номер узла сетки.
        /// </summary>
        public int NumNode { get; set; }
    }
}
