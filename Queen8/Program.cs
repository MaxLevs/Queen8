using Queen8.Data;
using Queen8.Data.Dto;
using Queen8.Service;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

QueenPlacingFinderService chess = new();
Stopwatch stopwatch = new();

Console.WriteLine($"Количество доступных процессов: {Environment.ProcessorCount}");
Console.WriteLine($"Занято памяти до начала вычисления: {GetMemoryAllocMessage(true)} МБ.");
Console.WriteLine("Запускаю вычисление...");
Console.WriteLine();

stopwatch.Start();
var isSuccessed = chess.FindPositions();
stopwatch.Stop();

if (isSuccessed)
{
    Console.WriteLine($"Всего найдено решений: {chess.Results.Count}");

    var width = chess.Results.Count.ToString().Length;
    var messageFormat = GetMessageFormat(width);
    StringBuilder resultStringBuilder = new();

    foreach (var elem in chess.Results.Select((seq, i) => new { i, seq }))
    {
        var resultLine = string.Format(messageFormat, elem.i, GetFiguresCombinationMessage(elem.seq));
        resultStringBuilder.AppendLine(resultLine);
        Console.WriteLine(resultLine);
    }

    var resultsPath = Path.Combine(Environment.CurrentDirectory, "results");
    Directory.CreateDirectory(resultsPath);
    File.WriteAllText(Path.Combine(resultsPath, "queen_placements_list.txt"), resultStringBuilder.ToString(), Encoding.UTF8);
    SaveResultsToJson(chess.Results, resultsPath);
}

else
{
    Console.WriteLine("Решение не найдено");
}

Console.WriteLine($"\nЗанято памяти после окончания вычисления: {GetMemoryAllocMessage(false)} МБ.");
PrintCalculationInfo(stopwatch);

static string GetMemoryAllocMessage(bool forceCollect)
{
    var allocatedMemory = GC.GetTotalMemory(forceCollect) / 1024.0 / 1024.0;
    return $"{allocatedMemory:F2}";
}

static string GetFiguresCombinationMessage(IEnumerable<QFigure> figures)
{
    return figures.Aggregate("", (acc, figure) => acc + " " + GetFigurePositionMessage(figure));
}

static string GetFigurePositionMessage(QFigure figure)
{
    return $"{Convert.ToChar(Convert.ToChar(figure.X) + 'A')}{figure.Y + 1}";
}

static void SaveResultsToJson(IEnumerable<IEnumerable<QFigure>> figures, string path)
{
    using var jsonFile = File.OpenWrite(Path.Combine(path, "queen_placements_serrialized.json"));
    using var writer = new Utf8JsonWriter(jsonFile);

    FiguresContext.Default.IEnumerableIEnumerableQFigure.SerializeHandler?.Invoke(writer, figures);
}

static string GetMessageFormat(int width) => "{0," + width + "}. {1}";

static void PrintCalculationInfo(Stopwatch stopwatch) => Console.WriteLine($"Время затраченное на поиск решений: {stopwatch.Elapsed}");
