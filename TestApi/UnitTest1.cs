using API;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using System.Text.Json;

namespace TestApi
{
    [TestClass]
    public class UnitTest1
    {
        public class ProductExample
        {
            public List<Product> CorrectProducts { get; set; }
            public List<Product> IncorrectProducts { get; set; }
            public List<Product> CorrectUpdateProducts { get; set; }
            public List<Product> IncorrectUpdateProducts { get; set; }
        }
        private API.API api = new API.API();
        //private IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile("products.json").Build();
        private ProductExample Products = JsonSerializer.Deserialize<ProductExample>(File.ReadAllText("products.json"))!;
        private List<int> ids = new List<int>();

        private bool EqalsProduct(Product? product1, Product? product2)
        {
            Assert.IsNotNull(product1, "First null");
            Assert.IsNotNull(product2, "Second null");
            Assert.AreEqual(product1.price, product2.price, $"Price {product2.price} != {product1.price}");
            Assert.AreEqual(product1.old_price, product2.old_price, $"Old price {product2.old_price} != {product1.old_price}");
            Assert.AreEqual(product1.hit, product2.hit, $"Hit {product2.hit} != {product1.hit}");
            Assert.AreEqual(product1.keywords, product2.keywords, $"Keywords {product2.keywords} != {product1.keywords}");
            Assert.AreEqual(product1.status, product2.status, $"Status {product2.status} != {product1.status}");
            Assert.AreEqual(product1.description, product2.description, $"Description {product2.description} != {product1.description}");
            Assert.AreEqual(product1.content, product2.content, $"Content {product2.content} != {product1.content}");
            Assert.AreEqual(product1.title, product2.title, $"Title {product2.title} != {product1.title}");
            return true;
        }
        private bool NotEqalsProduct(Product? product1, Product? product2)
        {
            if ((product1 == null && product2 != null) || (product2 == null && product1 != null)) return true;
            if (product1 == null && product2 == null) return true;
            if (product1.price != null && product1.price != product2.price) return true;
            if (product1.old_price != null && product1.old_price != product2.old_price) return true;
            if (product1.hit != null && product1.hit != product2.hit) return true;
            if (product1.keywords != null && product1.keywords != product2.keywords) return true;
            if (product1.status != null && product1.status != product2.status) return true;
            if (product1.description != null && product1.description != product2.description) return true;
            if (product1.content != null && product1.content != product2.content) return true;
            if (product1.title != null && product1.title != product2.title) return true;
            return false;
        }

        [TestMethod]
        public async Task CreateCorrectProduct()
        {
            var products = Products.CorrectProducts;

            foreach (var product in products)
            {
                var responseadd = await api.CreateProduct(product);
                ids.Add(responseadd!.id.Value);
                var responseget = await api.GetProducts();
                var searchedProduct = responseget!.FirstOrDefault(product => product.id == responseadd!.id.ToString());
                EqalsProduct(product, searchedProduct);
            }
        }

        [TestMethod]
        public async Task DeleteCorrectProduct()
        {
            var product = Products.CorrectProducts.First();
            var responseadd = await api.CreateProduct(product);
            var responsedelete = await api.DeleteProduct(responseadd!.id.Value);
            Assert.AreEqual(1, responsedelete!.status, $"Status is {responsedelete!.status}");
        }

        [TestMethod]
        public async Task UpdateCorrectProduct()
        {
            var product = Products.CorrectProducts.First();
            var updateProducts = Products.CorrectUpdateProducts;
            var responseadd = await api.CreateProduct(product);
            ids.Add(responseadd!.id.Value);

            foreach (var updateProduct in updateProducts)
            {
                updateProduct.id = responseadd!.id.ToString();
                var responseedit = await api.EditProduct(updateProduct);
                Assert.AreEqual(1, responseedit!.status, $"Product error {JsonSerializer.Serialize<Product>(updateProduct)}");
                var responseget = await api.GetProducts();
                var searchedProduct = responseget!.FirstOrDefault(product => product.id == responseadd!.id.ToString());
                EqalsProduct(updateProduct, searchedProduct);
            }
        }

        [TestMethod]
        public async Task UpdateIncorrectProduct()
        {
            var product = Products.CorrectProducts.First();
            var updateProducts = Products.IncorrectUpdateProducts;

            foreach (var updateProduct in updateProducts)
            {
                var responseadd = await api.CreateProduct(product);
                ids.Add(responseadd!.id.Value);
                updateProduct.id = responseadd!.id.ToString();
                var responseedit = await api.EditProduct(updateProduct);
                Assert.AreEqual(1, responseedit!.status, $"Product error {JsonSerializer.Serialize<Product>(updateProduct)}");
                var responseget = await api.GetProducts();
                var searchedProduct = responseget!.FirstOrDefault(product => product.id == responseadd!.id.ToString());
                Assert.IsTrue(NotEqalsProduct(updateProduct, searchedProduct), $"Product error {JsonSerializer.Serialize<Product>(product)}");
            }
        }

        [TestMethod]
        public async Task CreateIncorrectProduct()
        {
            var products = Products.IncorrectProducts;

            foreach (var product in products)
            {
                var responseadd = await api.CreateProduct(product);
                ids.Add(responseadd!.id.Value);
                var responseget = await api.GetProducts();
                var searchedProduct = responseget!.FirstOrDefault(product => product.id == responseadd!.id.ToString());
                Assert.IsTrue(NotEqalsProduct(product, searchedProduct), $"Product error {JsonSerializer.Serialize<Product>(product)}");
            }
        }

        [TestMethod]
        public async Task DeleteIncorrect()
        {
            var responsedelete = await api.DeleteProduct(20000);
            Assert.AreEqual(0, responsedelete.status);
        }

        [TestCleanup]
        public async Task Finish()
        {
            foreach (var id in ids)
            {
                await api.DeleteProduct(id);
            }
        }
    }
}