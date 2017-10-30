using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ECommerce.API.Model;
using ECommerce.ProductCatalog.Model;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly IProductCatalogService _productCatalogService;

        public ProductsController()
        {
            _productCatalogService = ServiceProxy.Create<IProductCatalogService>(new Uri("fabric:/ECommerce/ECommerce.ProductCatalog"),
                new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(0));
        }

        [HttpGet]
        public async Task<IEnumerable<ApiProduct>> Get()
        {
            var allProducts = await _productCatalogService.GetAllProducts();
            return allProducts.Select(ap => new ApiProduct
            {
                Id = ap.Id,
                Name = ap.Name,
                Description = ap.Description,
                Price = ap.Price,
                IsAvailable = ap.Availability > 0
            });
        }     
        
        [HttpPost]
        public async Task Post([FromBody] ApiProduct product)
        {
            var newProduct = new Product
            {
                Id = Guid.NewGuid(),
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Availability = 100
            };

            await _productCatalogService.AddProduct(newProduct);
        }
    }
}
