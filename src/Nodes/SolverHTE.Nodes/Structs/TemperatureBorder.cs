namespace SolverHTE.Nodes.Structs
{
    /// <summary>
    /// Представляет собой значение температуры на определенной границе.
    /// </summary>
    public struct TemperatureBorder
    {
        public TemperatureBorder(int numBorder, double temperature)
        {
            NumBorder = numBorder;
            Temperature = temperature;
        }
        public TemperatureBorder() { }

        /// <summary>
        /// Номер границы.
        /// </summary>
        public int NumBorder { get; set; }

        /// <summary>
        /// Значение температуры.
        /// </summary>
        public double Temperature { get; set; }
    }
}
