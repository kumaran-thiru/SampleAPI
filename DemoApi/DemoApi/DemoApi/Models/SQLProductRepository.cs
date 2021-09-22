using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Generic;
using System.Linq;

namespace DemoApi.Models
{
    public class SQLProductRepository : IProductRepository
    {
        private readonly ApiDbContext context;

        public SQLProductRepository(ApiDbContext context)
        {
            this.context = context;
        }

        public Product AddProduct(Product product)
        {
            var res = context.Products.Add(product);
            context.SaveChanges();
            return res.Entity;
        }

        public Product GetProduct(int id)
        {
            return context.Products.Find(id);
        }

        public IEnumerable<Product> GetProducts()
        {
            return context.Products;
        }

        public bool IsExist(int id)
        {
            return context.Products.Any(x => x.ProductId == id);
        }

        public Product PatchProduct(int id, JsonPatchDocument<Product> productPatch)
        {
            var pdt = GetProduct(id);
            if (pdt != null)
            {
                productPatch.ApplyTo(pdt);
                context.SaveChanges();
            }
            return pdt;
        }

        public bool RemoveProduct(int id)
        {
            var pdt = GetProduct(id);
            if (pdt != null) 
            {
                context.Products.Remove(pdt);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public Product UpdateProduct(Product product)
        {            
            context.Products.Update(product);
            context.SaveChanges();
            return product;
        }
    }
}
