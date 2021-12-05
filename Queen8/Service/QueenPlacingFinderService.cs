using Queen8.Data;
using System.Collections.Concurrent;

namespace Queen8.Service
{
    public class QueenPlacingFinderService
    {
        public ushort MaxDepth { get; } = 8;

        public ushort MaxVariantsCount { get; } = 8;

        public ConcurrentBag<List<QFigure>> Results { get; }

        public QueenPlacingFinderService()
        {
            Results = new ConcurrentBag<List<QFigure>>();
        }

        /// <summary>
        /// Создаёт новую симуляцию с заданными парамтерами
        /// </summary>
        /// <param name="x">Размер поля по горизонтали</param>
        /// <param name="y">Размер поля по вертикали</param>
        public QueenPlacingFinderService(ushort x, ushort y) : this()
        {
            MaxDepth = x;
            MaxVariantsCount = y;
        }

        #region PreGenerate

        private static IEnumerable<IEnumerable<QFigure>> Seed => new List<List<QFigure>> { new List<QFigure>() };

        private ushort GetLevelByNumber(double n, ushort count = 0) => Math.Abs(n) <= 1 ? count : GetLevelByNumber(n / 8, ++count);

        private IEnumerable<QFigure> GetLayerEnum(ushort depth) => Enumerable.Range(0, MaxVariantsCount).Select(variant => new QFigure(depth, (ushort)variant));

        private ParallelQuery<IEnumerable<QFigure>> Generate(ParallelQuery<IEnumerable<QFigure>> prevStep, ushort depth, ushort maxDepth)
        {
            if (depth == maxDepth)
            {
                return prevStep;
            }

            var currentStep = prevStep.SelectMany(seq => GetLayerEnum(depth), (seq, figure) => seq.Append(figure));

            return Generate(currentStep, (ushort)(depth + 1), maxDepth);
        }

        private IEnumerable<IEnumerable<QFigure>> Generate(ushort maxDepth) => Generate(Seed.AsParallel(), 0, maxDepth);

        #endregion PreGenerate

        /// <summary>
        /// Запускает вычисление расстановки фигур, не бьющих друг друга
        /// </summary>
        /// <returns></returns>
        public bool FindPositions()
        {
            var depth = GetLevelByNumber(Environment.ProcessorCount);
            var pregenered = Generate(depth);

            return FindPositions(pregenered, depth);
        }

        /// <summary>
        /// Запускает парралельное вычисление для прегенерированных фигур
        /// </summary>
        /// <param name="fiqureSequenses">Коллекцию прегенерированных последовательностей фигур равной длины</param>
        /// <param name="depth">Текущая глубина (длина последовательности)</param>
        /// <returns>Список фигур, расположение которых не затрагивает друг друга. Если такой набор фигур не найден, возвращает <c>null</c></returns>
        private bool FindPositions(IEnumerable<IEnumerable<QFigure>> fiqureSequenses, ushort depth)
        {
            ParallelOptions parallelOptions = new()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };

            Parallel.ForEach(fiqureSequenses, parallelOptions, fiqures =>
            {
                var subResults = FindPosititonsIterative(fiqures, depth);

                foreach (var result in subResults)
                {
                    Results.Add(result);
                }
            });

            return !Results.IsEmpty;
        }

        /// <summary>
        /// Итеративно перебирает последовательности в глубину (достраевая до максимальной длины) и проверяет на наличие взаимобесопасной комбинации
        /// </summary>
        /// <param name="figures">Предыдущая последовательность из фигур</param>
        /// <param name="depth">Текущая глубина (длина последовательности)</param>
        /// <returns>Список фигур, расположение которых не затрагивает друг друга. Если такой набор фигур не найден, возвращает <c>null</c></returns>
        private IEnumerable<List<QFigure>> FindPosititonsIterative(IEnumerable<QFigure> figures, ushort depth)
        {
            List<List<QFigure>> results = new();

            if (depth == MaxDepth)
            {
                var combs = from figure1 in figures
                            from figure2 in figures
                            where !figure1.Equals(figure2)
                            select (figure1, figure2);

                var isFound = combs.All((pair) => !pair.figure1.IsHit(pair.figure2));

                if (isFound)
                {
                    results.Add(figures.ToList());
                }

                return results;
            }

            ushort newDepth = (ushort)(depth + 1);

            for (ushort y = 0; y < MaxVariantsCount; y++)
            {
                var currentSeq = figures.Append(new QFigure(depth, y));
                var subResults = FindPosititonsIterative(currentSeq, newDepth);

                results.AddRange(subResults);
            }

            return results;
        }
    }
}
