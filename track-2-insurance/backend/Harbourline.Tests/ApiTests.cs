using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Harbourline.Api.Dtos;

namespace Harbourline.Tests;

public class ApiTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public ApiTests(ApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Health_returns_ok()
    {
        var response = await _client.GetAsync("/api/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Products_list_returns_the_seeded_products()
    {
        var products = await _client.GetFromJsonAsync<List<ProductDto>>("/api/products");

        Assert.NotNull(products);
        Assert.Equal(2, products!.Count);
        Assert.Contains(products, p => p.Code == "MOT-COMP");
    }

    [Fact]
    public async Task Missing_claim_returns_404_problem_details()
    {
        var response = await _client.GetAsync("/api/claims/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Invalid_product_returns_400()
    {
        var response = await _client.PostAsJsonAsync("/api/products", new ProductRequest("", "", "Boat", 0m, -1m, true));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Policyholders_paging_returns_total_and_page_size()
    {
        var json = await _client.GetStringAsync("/api/policyholders?page=1&pageSize=3");
        using var doc = JsonDocument.Parse(json);

        Assert.Equal(7, doc.RootElement.GetProperty("total").GetInt32());
        Assert.Equal(3, doc.RootElement.GetProperty("items").GetArrayLength());
    }

    [Fact]
    public async Task Swagger_document_is_served()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
