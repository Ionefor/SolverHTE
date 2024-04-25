using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace SolverHTE.Triangulation.Structs
{
    /// <summary>
    /// Представляет упорядоченную пару координат X и Y с плавающей запятой типа decimal,
    /// которая определяет точку на двумерной плоскости.
    /// </summary>
    public struct PointM : IEquatable<PointM>
    {
        /// <summary>
        /// Creates a new instance of the <see cref='PointM'/> class with member data left uninitialized.
        /// </summary>
        public static readonly PointM Empty;
        private decimal x; // Do not rename (binary serialization)
        private decimal y; // Do not rename (binary serialization)

        /// <summary>
        /// Initializes a new instance of the <see cref='PointM'/> class with the specified coordinates.
        /// </summary>
        public PointM(decimal x, decimal y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref='PointM'/> is empty.
        /// </summary>
        [Browsable(false)]
        public readonly bool IsEmpty => x == 0m && y == 0m;

        /// <summary>
        /// Gets the x-coordinate of this <see cref='PointM'/>.
        /// </summary>
        public decimal X
        {
            readonly get => x;
            set => x = value;
        }

        /// <summary>
        /// Gets the y-coordinate of this <see cref='PointM'/>.
        /// </summary>
        public decimal Y
        {
            readonly get => y;
            set => y = value;
        }

        /// <summary>
        /// Compares two <see cref='PointM'/> objects. The result specifies whether the values of the
        /// <see cref='X'/> and <see cref='Y'/> properties of the two
        /// <see cref='PointM'/> objects are equal.
        /// </summary>
        public static bool operator ==(PointM left, PointM right) => left.X == right.X && left.Y == right.Y;

        /// <summary>
        /// Compares two <see cref='PointM'/> objects. The result specifies whether the values of the
        /// <see cref='X'/> or <see cref='Y'/> properties of the two
        /// <see cref='PointM'/> objects are unequal.
        /// </summary>
        public static bool operator !=(PointM left, PointM right) => !(left == right);
        public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is PointM && Equals((PointM)obj);
        public readonly bool Equals(PointM other) => this == other;
        public override readonly int GetHashCode() => HashCode.Combine(X.GetHashCode(), Y.GetHashCode());
        public override readonly string ToString() => $"{{X={x}, Y={y}}}";
    }
}
