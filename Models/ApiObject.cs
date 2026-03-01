using System.Text.Json.Serialization;
using System.Text.Json;

namespace RestfulApiDev.Tests.Models;

public sealed class ApiObject
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("data")]
    public Dictionary<string, object?>? Data { get; set; }

    // [JsonPropertyName("createdAt")]
    // public DateTimeOffset? CreatedAt { get; set; }

    // [JsonPropertyName("updatedAt")]
    // public DateTimeOffset? UpdatedAt { get; set; }

    [JsonPropertyName("createdAt")]
    public JsonElement? CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public JsonElement? UpdatedAt { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}