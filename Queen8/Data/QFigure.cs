namespace Queen8.Data
{
    /// <summary>
    /// Represents of a Queen chess figure
    /// </summary>
    public class QFigure
    {
        /// <summary>
        /// Default constructor of <see cref="QFigure"/>
        /// </summary>
        public QFigure()
        {
            X = 0;
            Y = 0;
        }

        /// <summary>
        /// Basic constructor of <see cref="QFigure"/>
        /// </summary>
        /// <param name="x">Represents horisontal position as number (it's A, B, C, etc. in chess)</param>
        /// <param name="y">Represents vertical position</param>
        public QFigure(ushort x, ushort y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Copy contructor
        /// </summary>
        /// <param name="figure"></param>
        public QFigure(QFigure figure)
        {
            X = figure.X;
            Y = figure.Y;
        }

        /// <summary>
        /// Represents horisontal position as number (it's A, B, C, etc. in chess)
        /// </summary>
        public ushort X { get; private set; }

        /// <summary>
        /// Represents vertical position
        /// </summary>
        public ushort Y { get; private set; }

        /// <summary>
        /// Checks if this figure hist another one
        /// </summary>
        /// <param name="figure"></param>
        /// <returns><c>true</c> if this figure and another one stays on the same line (vertical, horisontal or diagonal); otherwise - <c>false</c>></returns>
        public bool IsHit(QFigure figure)
        {
            return X == figure.X
                || Y == figure.Y
                || Math.Abs(Y - figure.Y) == Math.Abs(X - figure.X);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is QFigure figure && figure.X == X && figure.Y == Y;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}
