namespace SolverHTE.SolvingEquation.Structs
{
    /// <summary>
    /// Представляет собой набор параметров на границе.
    /// </summary>
    public struct BorderData
    {
        public BorderData(double h, double T_inf, double q)
        {
            this.h = h;
            this.T_inf = T_inf;
            this.q = q;
        }
        public BorderData() { }

        /// <summary>
        /// Коэффициент теплообмена.
        /// </summary>
        public double h { get; set; }

        /// <summary>
        /// Температура окружающей среды.
        /// </summary>
        public double T_inf { get; set; }

        /// <summary>
        /// Тепловой поток.
        /// </summary>
        public double q { get; set; }
    }
}
