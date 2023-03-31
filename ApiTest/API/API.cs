using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API
{
    public class API
    {
        private HttpClient client;
        private const string domainURL = "http://shop.qatl.ru/";
        private const string products = "api/products";
        private const string add = "api/addproduct";
        private const string edit = "api/editproduct";
        private string delete(int id) => $"api/deleteproduct?id={id}";

        public API()
        {
            client = new HttpClient();
        }
        public async Task<IEnumerable<Product>?> GetProducts()
        {
            var response = await client.GetAsync(domainURL + API.products);
            var content = await response.Content.ReadAsStringAsync();
            if (content == null) { return null; }
            IEnumerable<Product>? products = JsonSerializer.Deserialize<IEnumerable<Product>>(content);
            return products;
        }
        public async Task<Response?> EditProduct(Product product)
        {
            var content = new StringContent(JsonSerializer.Serialize<Product>(product), Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await client.PostAsync(domainURL + edit, content);
            return JsonSerializer.Deserialize<Response>(await response.Content.ReadAsStringAsync());
        }
        public async Task<ResponseAdd?> CreateProduct(Product product)
        {
            var content = new StringContent(JsonSerializer.Serialize<Product>(product), Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await client.PostAsync(domainURL + add, content);
            return JsonSerializer.Deserialize<ResponseAdd>(await response.Content.ReadAsStringAsync());
        }
        public async Task<Response?> DeleteProduct(int id)
        {
            var response = await client.GetAsync(domainURL + delete(id));
            return JsonSerializer.Deserialize<Response>(await response.Content.ReadAsStringAsync());
        }
    }
}
