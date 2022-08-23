using System.Diagnostics;
using Play.Inventory.Service.Dtos;

namespace Play.Inventory.Service.Clients;

public class CatalogClient
{
    private readonly HttpClient _httpClient;

    public CatalogClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyCollection<CatalogItemDto>> GetCatalogItemsAsync()
    {
        var items = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<CatalogItemDto>>("/items");
        Debug.Assert(items != null, nameof(items) + " != null");
        return items;
    }
}
