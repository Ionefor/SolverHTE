using SolverHTE.Triangulation.Structs;
using SolverHTE.Triangulation.Utility;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SolverHTE.Solver")]
namespace SolverHTE.Triangulation
{
    /// <summary>
    ///  В данном классе выполняется начальное разбиение области.
    /// </summary>
    internal class DivisionArea 
    {
        /// <summary>
        /// Список треугольников, образующих начальное разбиение.
        /// </summary>
        private List<Triangle> _trianglesDivision = new();

        /// <summary>
        /// Начальные точки области.
        /// </summary>
        private PointM[] _points;

        /// <summary>
        /// <inheritdoc cref="_trianglesDivisionDivision"/> 
        /// </summary>
        public List<Triangle> TrianglesDivision { get => _trianglesDivision; }

        public DivisionArea(params PointM[] points) => _points = points;

        /// <summary>
        /// Выполняет начальное разбиение области, на треугольники.
        /// </summary>
        public void Division()
        {
            var splitPoints = SplitEdgesArea(_points);
            var countPoints = splitPoints.Count;

            var jIndexes = new int[3];
            var iIndexes = new int[3];
            var foundIndex = -1;
            var minAngle = 360m;

            for (int i = 0; i < countPoints - 2; i++)
            {
                for (int j = 0; j < splitPoints.Count; j++)
                {
                    if ((j + 1) >= splitPoints.Count)
                    {
                        jIndexes[0] = j;
                        jIndexes[1] = Math.Abs(splitPoints.Count - j - 1);
                        jIndexes[2] = Math.Abs(splitPoints.Count - j - 2);
                    }
                    else if ((j + 2) >= splitPoints.Count)
                    {
                        jIndexes[0] = j;
                        jIndexes[1] = j + 1;
                        jIndexes[2] = Math.Abs(splitPoints.Count - j - 2);
                    }
                    else
                    {
                        jIndexes[0] = j;
                        jIndexes[1] = j + 1;
                        jIndexes[2] = j + 2;
                    }

                    var vectors = GeometryUtility.GetVectors(jIndexes[0], jIndexes[1], jIndexes[2], splitPoints);
                    var currentAngle = GeometryUtility.GetAngleVectors(vectors[0], vectors[1]);
                   
                    if ((splitPoints[jIndexes[0]].X * splitPoints[jIndexes[1]].Y + splitPoints[jIndexes[2]].X * splitPoints[jIndexes[0]].Y + splitPoints[jIndexes[2]].Y * splitPoints[jIndexes[1]].X
                         - splitPoints[jIndexes[2]].X * splitPoints[jIndexes[1]].Y - splitPoints[jIndexes[0]].X * splitPoints[jIndexes[2]].Y - splitPoints[jIndexes[1]].X * splitPoints[jIndexes[0]].Y) <= 0)
                    {
                        currentAngle = 360 - currentAngle;
                    }

                    if (currentAngle < minAngle)
                    {
                        minAngle = currentAngle;
                        foundIndex = jIndexes[0];
                    }
                }

                if ((foundIndex + 1) >= splitPoints.Count)
                {
                    iIndexes[0] = foundIndex;
                    iIndexes[1] = Math.Abs(splitPoints.Count - foundIndex - 1);
                    iIndexes[2] = Math.Abs(splitPoints.Count - foundIndex - 2);
                }
                else if ((foundIndex + 2) >= splitPoints.Count)
                {
                    iIndexes[0] = foundIndex;
                    iIndexes[1] = foundIndex + 1;
                    iIndexes[2] = Math.Abs(splitPoints.Count - foundIndex - 2);
                }
                else
                {
                    iIndexes[0] = foundIndex;
                    iIndexes[1] = foundIndex + 1;
                    iIndexes[2] = foundIndex + 2;
                }

                _trianglesDivision.Add(new Triangle(splitPoints[iIndexes[0]], 
                    splitPoints[iIndexes[1]], splitPoints[iIndexes[2]]));

                TriangulationUtility.FlipBadEdges(_trianglesDivision[_trianglesDivision.Count - 1], 
                    GeometryUtility.VertexMaxAngle(_trianglesDivision[_trianglesDivision.Count - 1]), _trianglesDivision);

                if (!TriangulationUtility.PointIsRequired(iIndexes[1], _trianglesDivision, splitPoints))
                {
                    splitPoints.RemoveAt(iIndexes[1]);
                }
                minAngle = 360;
            }
        }

        /// <summary>
        /// Разбивает ребра области на более мелкие.
        /// </summary>
        /// <param name="points"></param>
        /// <returns>Список точек для дальнейшей триангуляции. </returns>
        private List<PointM> SplitEdgesArea(PointM[] points)
        {
            var splitPoints = new List<PointM>();
            var len = 1;

            for (int i = 0; i < points.Length; i++)
            {
                var index = i != points.Length - 1 ? i + 1 : 0;
                var quantityPoints = GeometryUtility.GetDistancePoints(points[i], points[index]) / len;
                var newPoint = points[i];

                for (int j = 0; j < quantityPoints; j++)
                {
                    splitPoints.Add(newPoint);

                    newPoint.X += (points[index].X - points[i].X) / (decimal)quantityPoints;
                    newPoint.Y += (points[index].Y - points[i].Y) / (decimal)quantityPoints;
                }
            }
            return splitPoints;
        }    
    }
}
