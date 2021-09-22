using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Generic;

namespace DemoApi.Models
{
    public interface IProductRepository
    {
        Product AddProduct(Product product);
        Product UpdateProduct(Product product);
        Product PatchProduct(int id, JsonPatchDocument<Product> productPatch);
        Product GetProduct(int id);
        IEnumerable<Product> GetProducts();
        bool RemoveProduct(int id);
        bool IsExist(int id);
    }
}
