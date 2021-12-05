using System.Text.Json.Serialization;

namespace Queen8.Data.Dto
{
    [JsonSerializable(typeof(IEnumerable<IEnumerable<QFigure>>))]
    internal partial class FiguresContext : JsonSerializerContext
    {
    }
}
