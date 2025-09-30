using System.Text.Json.Serialization;

namespace Sales.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderStatus
{
    Created,
    Confirmed,
    Rejected,
    InSeparation,
    Cancelled,
    Finished
}