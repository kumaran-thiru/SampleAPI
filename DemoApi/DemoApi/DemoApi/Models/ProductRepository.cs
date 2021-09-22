using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Generic;
using System.Linq;

namespace DemoApi.Models
{
    public class ProductRepository : IProductRepository
    {
        List<Product> products;
        public ProductRepository()
        {
            products = new List<Product>
            {
                new Product{ProductId = 1001, Name="Book", Quantity = 100, UnitCost = 200},
                new Product{ProductId = 1005, Name="Pen", Quantity = 40, UnitCost = 15},
                new Product{ProductId = 1010, Name="Samsung Galaxy M31", Quantity = 20, UnitCost = 22999},
                new Product{ProductId = 1015, Name="Apple Ear Buds", Quantity = 12, UnitCost = 7989},
            };
        }

        public Product AddProduct(Product product)
        {
            products.Add(product);
            return products.Find(x => x.ProductId == product.ProductId);
        }

        public Product GetProduct(int id)
        {
            var pdt = products.FirstOrDefault(x => x.ProductId == id);
            return pdt;
        }

        public IEnumerable<Product> GetProducts()
        {
            return products;
        }

        public bool IsExist(int id)
        {
            return products.Exists(x => x.ProductId == id);
        }

        public Product PatchProduct(int id, JsonPatchDocument<Product> productPatch)
        {
            Product prod = products.Find(x => x.ProductId == id);
            if (prod != null)
            {
                productPatch.ApplyTo(prod);
            }
            return prod;
        }

        public bool RemoveProduct(int id)
        {
            var x = products.RemoveAll(x => x.ProductId == id);
            if (x == 0)
            {
                return false;
            }
            return true;
        }

        public Product UpdateProduct(Product product)
        {
            Product prod = products.Find(x => x.ProductId == product.ProductId);
            prod.Name = product.Name;
            prod.Quantity = product.Quantity;
            prod.UnitCost = product.UnitCost;
            return prod;
        }
    }
}
