using Play.Inventory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Play.Inventory.Clients
{
    public class CatalogClient
    {
        private readonly HttpClient _httpClient;


        public CatalogClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyCollection<CatalogItem>> GetCatalogItemsAsync()
        {
            var items = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<CatalogItem>>("/items");
            return items;
        }

    }
}
