using DemoApi.Authorization;
using DemoApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DemoApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        //[Produces("application/xml")]
        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            return Ok(productRepository.GetProducts());          
        }

        //[Produces("application/json")]
        [HttpGet("Get")]
        public ActionResult<Product> GetProduct([FromQuery] int id)
        {           
            var x = productRepository.GetProduct(id);
            if (x != null)
            {
                return Ok(x);
            }
            return NotFound($"Product with Id = {id} Not Found");
        }

        [HttpDelete("Delete")]
        public ActionResult DeleteProduct([FromQuery] int id)
        {            
            if (productRepository.IsExist(id))
            {
                productRepository.RemoveProduct(id);
                return NoContent();
            }
            return NotFound($"Cannot Delete. Product with Id = {id} Not Found");            
        }

        [HttpPost]
        public ActionResult<Product> PostProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (productRepository.IsExist(product.ProductId))
            {
                ModelState.AddModelError("Request Failed", 
                    $"Product with Product Id = {product.ProductId} Already Exists");
                return BadRequest(ModelState);
            }
            var addedProduct = productRepository.AddProduct(product);
            return CreatedAtAction(nameof(GetProduct),new { id = addedProduct.ProductId},addedProduct);
        }

        [HttpPut("{id}")]
        public ActionResult<Product> UpdateProduct([FromRoute] int id, [FromBody] Product product)
        {                       
            if (id != product.ProductId)
            {
                return BadRequest("Id and Product Id Mismatch");
            }
            if (!productRepository.IsExist(id))
            {
                return NotFound($"Product with Id = {id} Not Found");
            }
            return Ok(productRepository.UpdateProduct(product));            
        }

        [HttpPatch("{id}")]
        public ActionResult<Product> PatchProduct([FromRoute] int id, 
            [FromBody] JsonPatchDocument<Product> productPatch)
        {
            if (!productRepository.IsExist(id))
            {
                return NotFound($"Product with Id = {id} Not Found");
            }
            foreach(var x in productPatch.Operations)
            {
                if(x.path.ToLower().Contains("productid"))
                {
                    ModelState.AddModelError("Request Failed", "Product Id field cannot be updated");
                    return BadRequest(ModelState);
                }
            }
            var pdt1 = productRepository.GetProduct(id);
            var pdt = new Product(pdt1);
            productPatch.ApplyTo(pdt, ModelState);
            if (!TryValidateModel(pdt))
            {
                return BadRequest(ModelState);
            }
            return Ok(productRepository.PatchProduct(id, productPatch));
        } 
    }
}
