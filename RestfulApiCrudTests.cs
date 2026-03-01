using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using RestfulApiDev.Tests.Infrastructure;
using RestfulApiDev.Tests.Models;
using Xunit;

[assembly: TestCaseOrderer("RestfulApiDev.Tests.Infrastructure.PriorityOrderer", "RestfulApiDev.Tests")]

namespace RestfulApiDev.Tests;

public sealed class RestfulApiCrudTests
{
    private static readonly HttpClient Client = new()
    {
        BaseAddress = new Uri("https://api.restful-api.dev/")
    };

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    [Priority(1)]
    public async Task Get_list_of_all_objects_should_return_ok_and_non_empty_list()
    {
        using var response = await Client.GetAsync("objects");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<List<ApiObject>>(JsonOptions);
        Assert.NotNull(body);
        Assert.NotEmpty(body!);

        Assert.All(body!, o =>
        {
            Assert.False(string.IsNullOrWhiteSpace(o.Id));
            Assert.False(string.IsNullOrWhiteSpace(o.Name));
        });
    }

    [Fact]
    [Priority(2)]
    public async Task Add_object_using_post_should_return_created_object_with_id_and_createdAt()
    {
        var uniqueName = $"Test Object - xUnit - {Guid.NewGuid():N}";

        var payload = new
        {
            name = uniqueName,
            data = new Dictionary<string, object?>
            {
                ["year"] = 2026,
                ["price"] = 1234.56,
                ["CPU model"] = "Intel Core i9",
                ["Hard disk size"] = "1 TB"
            }
        };

        using var response = await Client.PostAsJsonAsync("objects", payload, JsonOptions);

        Assert.True(
            response.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created,
            $"Expected 200 or 201, got {(int)response.StatusCode} {response.StatusCode}");

        var created = await response.Content.ReadFromJsonAsync<ApiObject>(JsonOptions);
        Assert.NotNull(created);

        Assert.False(string.IsNullOrWhiteSpace(created!.Id));
        Assert.Equal(uniqueName, created.Name);
        Assert.NotNull(created.Data);
        Assert.True(created.Data!.ContainsKey("year"));
        Assert.True(created.CreatedAt.HasValue);

        TestState.CreatedId = created.Id;
        TestState.CreatedName = uniqueName;
    }

    [Fact]
    [Priority(3)]
    public async Task Get_single_object_using_created_id_should_match_posted_data()
    {
        Assert.False(string.IsNullOrWhiteSpace(TestState.CreatedId));
        Assert.False(string.IsNullOrWhiteSpace(TestState.CreatedName));

        using var response = await Client.GetAsync($"objects/{TestState.CreatedId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var fetched = await response.Content.ReadFromJsonAsync<ApiObject>(JsonOptions);
        Assert.NotNull(fetched);

        Assert.Equal(TestState.CreatedId, fetched!.Id);
        Assert.Equal(TestState.CreatedName, fetched.Name);

        Assert.NotNull(fetched.Data);
        Assert.True(fetched.Data!.ContainsKey("year"));
        Assert.True(fetched.Data.ContainsKey("price"));
    }

    [Fact]
    [Priority(4)]
    public async Task Update_object_using_put_should_return_updatedAt_and_new_values()
    {
        Assert.False(string.IsNullOrWhiteSpace(TestState.CreatedId));

        var updatedName = $"{TestState.CreatedName} (Updated)";

        var updatePayload = new
        {
            name = updatedName,
            data = new Dictionary<string, object?>
            {
                ["year"] = 2026,
                ["price"] = 2222.22,
                ["CPU model"] = "Intel Core i9",
                ["Hard disk size"] = "2 TB",
                ["color"] = "silver"
            }
        };

        using var response = await Client.PutAsJsonAsync($"objects/{TestState.CreatedId}", updatePayload, JsonOptions);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await response.Content.ReadFromJsonAsync<ApiObject>(JsonOptions);
        Assert.NotNull(updated);

        Assert.Equal(TestState.CreatedId, updated!.Id);
        Assert.Equal(updatedName, updated.Name);
        Assert.NotNull(updated.Data);
        Assert.True(updated.Data!.ContainsKey("color"));
        Assert.True(updated.UpdatedAt.HasValue);

        using var getResponse = await Client.GetAsync($"objects/{TestState.CreatedId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await getResponse.Content.ReadFromJsonAsync<ApiObject>(JsonOptions);
        Assert.NotNull(fetched);

        Assert.Equal(updatedName, fetched!.Name);
        Assert.NotNull(fetched.Data);
        Assert.True(fetched.Data!.ContainsKey("color"));
    }

    [Fact]
    [Priority(5)]
    public async Task Delete_object_using_delete_should_return_confirmation_message()
    {
        Assert.False(string.IsNullOrWhiteSpace(TestState.CreatedId));

        using var response = await Client.DeleteAsync($"objects/{TestState.CreatedId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var deleted = await response.Content.ReadFromJsonAsync<ApiObject>(JsonOptions);
        Assert.NotNull(deleted);

        Assert.False(string.IsNullOrWhiteSpace(deleted!.Message));
        Assert.Contains(TestState.CreatedId!, deleted.Message!, StringComparison.OrdinalIgnoreCase);
    }
}