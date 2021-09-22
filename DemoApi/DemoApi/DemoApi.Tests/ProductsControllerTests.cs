using DemoApi.Controllers;
using DemoApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace DemoApi.Tests
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductRepository> mockProductRepo;
        private readonly ProductsController controller;

        public ProductsControllerTests()
        {
            mockProductRepo = new Mock<IProductRepository>();
            controller = new ProductsController(mockProductRepo.Object);
        }

        private List<Product> GetList()
        {
            return new List<Product>
            {
                new Product{ProductId = 1001, Name="Book", Quantity = 100, UnitCost = 200},
                new Product{ProductId = 1005, Name="Pen", Quantity = 40, UnitCost = 15},
                new Product{ProductId = 1010, Name="Samsung Galaxy M31", Quantity = 20, UnitCost = 22999},
                new Product{ProductId = 1015, Name="Apple Ear Buds", Quantity = 12, UnitCost = 7989},
            };
        }

        //Get All
        [Fact]
        public void GetProducts_WhenCalled_ReturnsOkResult()
        {
            //Act
            var res = controller.GetProducts();
            //Assert
            Assert.IsType<OkObjectResult>(res.Result);
        }

        [Fact]
        public void GetProducts_WhenCalled_ReturnsListOfProducts()
        {
            //Arrange
            mockProductRepo.Setup(repo => repo.GetProducts())
                .Returns(GetList());

            //Act
            var res = controller.GetProducts().Result as OkObjectResult;
            //Assert
            var list = Assert.IsType<List<Product>>(res.Value);
            Assert.Equal(GetList().Count, list.Count);
        }

        //Get
        [Fact]
        public void GetProduct_WhenCalled_ReturnsOkResult()
        {
            //Arrange
            var prod = GetList()[0];
            mockProductRepo.Setup(p => p.GetProduct(1001)).Returns(prod);
            //Act
            var res = controller.GetProduct(1001);
            //Assert
            Assert.IsType<OkObjectResult>(res.Result);
        }

        [Fact]
        public void GetProduct_ValidIdPassed_ReturnsValidProduct()
        {
            //Arrange
            var prod = GetList()[0];
            mockProductRepo.Setup(p => p.GetProduct(1001)).Returns(prod);
            //Act
            var res = controller.GetProduct(1001).Result as OkObjectResult;
            //Assert
            Assert.IsType<Product>(res.Value);
            Assert.Equal(prod, res.Value);
        }

        [Theory]
        [InlineData(500)]
        [InlineData(10000)]
        [InlineData(1003)]
        public void GetProduct_InValidIdPassed_ReturnsNotFound(int id)
        {
            //Arrange
            mockProductRepo.Setup(p => p.GetProduct(id)).Returns(GetList().Find(x => x.ProductId == id));
            //Act
            var res = controller.GetProduct(id);
            //Assert
            Assert.IsType<NotFoundObjectResult>(res.Result);
        }

        //Delete
        [Fact]
        public void DeleteProduct_ValidIdPassed_ReturnsNoContent()
        {
            //Arrange
            var stat = GetList().Exists(x => x.ProductId == 1001);
            mockProductRepo.Setup(p => p.RemoveProduct(1001)).Returns(stat);
            mockProductRepo.Setup(p => p.IsExist(1001)).Returns(stat);
            //Act
            var res = controller.DeleteProduct(1001);
            //Assert
            Assert.IsType<NoContentResult>(res);
        }

        [Theory]
        [InlineData(500)]
        [InlineData(10000)]
        [InlineData(1003)]
        public void DeleteProduct_InValidIdPassed_ReturnsNotFound(int id)
        {
            //Arrange
            var stat = GetList().Exists(x => x.ProductId == id);
            mockProductRepo.Setup(p => p.RemoveProduct(id)).Returns(stat);
            mockProductRepo.Setup(p => p.IsExist(id)).Returns(stat);
            //Act
            var res = controller.DeleteProduct(id);
            //Assert
            Assert.IsType<NotFoundObjectResult>(res);
        }

        //Post
        [Fact]
        public void PostProduct_ValidObjectPassed_ReturnsCreatedAtAction()
        {
            //Arrange
            Product pdt = new Product { ProductId = 1020, Name = "Sample Product", Quantity = 20, UnitCost = 5000 };
            var stat = GetList().Exists(x => x.ProductId == pdt.ProductId);
            mockProductRepo.Setup(p => p.AddProduct(pdt)).Returns(pdt);
            mockProductRepo.Setup(p => p.IsExist(pdt.ProductId)).Returns(stat);
            //Act
            var res = controller.PostProduct(pdt);
            //Assert
            Assert.IsType<CreatedAtActionResult>(res.Result);
        }

        [Fact]
        public void PostProduct_ValidObjectPassed_ReturnsCreatedObject()
        {
            //Arrange
            Product pdt = new Product { ProductId = 1020, Name = "Sample Product", Quantity = 20, UnitCost = 5000 };
            var stat = GetList().Exists(x => x.ProductId == pdt.ProductId);
            mockProductRepo.Setup(p => p.AddProduct(pdt)).Returns(pdt);
            mockProductRepo.Setup(p => p.IsExist(pdt.ProductId)).Returns(stat);
            //Act
            var res = controller.PostProduct(pdt).Result as CreatedAtActionResult;
            //Assert
            Assert.IsType<Product>(res.Value);
            Assert.Equal(pdt, res.Value);
        }

        [Fact]
        public void PostProduct_InvalidModelState_ReturnsBadRequest()
        {
            //Arrange
            Product pdt = new Product { ProductId = 1020, Quantity = 20, UnitCost = 5000 };
            var stat = GetList().Exists(x => x.ProductId == pdt.ProductId);
            mockProductRepo.Setup(p => p.AddProduct(pdt)).Returns(pdt);
            mockProductRepo.Setup(p => p.IsExist(pdt.ProductId)).Returns(stat);
            //Act
            controller.ModelState.AddModelError("Name", "Required");
            var res = controller.PostProduct(pdt);
            //Assert
            Assert.IsType<BadRequestObjectResult>(res.Result);
        }

        [Fact]
        public void PostProduct_ExistingItem_ReturnsBadRequest()
        {
            //Arrange
            Product pdt = new Product { ProductId = 1001, Name = "Sample Product", Quantity = 20, UnitCost = 5000 };
            var stat = GetList().Exists(x => x.ProductId == pdt.ProductId);
            mockProductRepo.Setup(p => p.AddProduct(pdt)).Returns(pdt);
            mockProductRepo.Setup(p => p.IsExist(pdt.ProductId)).Returns(stat);
            //Act
            var res = controller.PostProduct(pdt);
            //Assert
            Assert.IsType<BadRequestObjectResult>(res.Result);
        }

        //Put
        [Fact]
        public void UpdateProduct_ValidObjectPassed_ReturnsOkResult()
        {
            //Arrange
            Product pdt = new Product { ProductId = 1001, Name = "Sample Product", Quantity = 20, UnitCost = 5000 };
            var stat = GetList().Exists(x => x.ProductId == pdt.ProductId);
            mockProductRepo.Setup(p => p.UpdateProduct(pdt)).Returns(pdt);
            mockProductRepo.Setup(p => p.IsExist(pdt.ProductId)).Returns(stat);
            //Act
            var res = controller.UpdateProduct(1001, pdt);
            //Assert
            Assert.IsType<OkObjectResult>(res.Result);
        }

        [Fact]
        public void UpdateProduct_ValidObjectPassed_ReturnsUpdatedObject()
        {
            //Arrange
            Product pdt = new Product { ProductId = 1001, Name = "Sample Product", Quantity = 20, UnitCost = 5000 };
            var stat = GetList().Exists(x => x.ProductId == pdt.ProductId);
            mockProductRepo.Setup(p => p.UpdateProduct(pdt)).Returns(pdt);
            mockProductRepo.Setup(p => p.IsExist(pdt.ProductId)).Returns(stat);
            //Act
            var res = controller.UpdateProduct(1001, pdt).Result as OkObjectResult;
            //Assert
            Assert.IsType<Product>(res.Value);
            Assert.Equal(pdt, res.Value);
        }

        [Fact]
        public void UpdateProduct_IdMismatch_ReturnsBadRequest()
        {
            //Arrange
            Product pdt = new Product { ProductId = 1001, Name = "Sample Product", Quantity = 20, UnitCost = 5000 };
            var stat = GetList().Exists(x => x.ProductId == pdt.ProductId);
            mockProductRepo.Setup(p => p.UpdateProduct(pdt)).Returns(pdt);
            mockProductRepo.Setup(p => p.IsExist(pdt.ProductId)).Returns(stat);
            //Act
            var res = controller.UpdateProduct(1002, pdt);
            //Assert
            Assert.IsType<BadRequestObjectResult>(res.Result);
        }

        [Fact]
        public void UpdateProduct_NotExistObject_ReturnsNotFound()
        {
            //Arrange
            Product pdt = new Product { ProductId = 1000, Name = "Sample Product", Quantity = 20, UnitCost = 5000 };
            var stat = GetList().Exists(x => x.ProductId == pdt.ProductId);
            mockProductRepo.Setup(p => p.UpdateProduct(pdt)).Returns(pdt);
            mockProductRepo.Setup(p => p.IsExist(pdt.ProductId)).Returns(stat);
            //Act
            var res = controller.UpdateProduct(pdt.ProductId, pdt);
            //Assert
            Assert.IsType<NotFoundObjectResult>(res.Result);
        }

        //Patch
        [Fact]
        public void PatchProduct_NotExistObject_ReturnsNotFound()
        {
            //Arrange
            JsonPatchDocument<Product> patchdoc = new JsonPatchDocument<Product>();
            patchdoc.Replace(x => x.Name, "Patched Name");
            patchdoc.Replace(x => x.UnitCost, 120);
            Product product = new Product { Name = "Patched Name", ProductId = 1000, Quantity = 1234, UnitCost = 120 };

            var stat = GetList().Exists(x => x.ProductId == 1000);
            mockProductRepo.Setup(p => p.PatchProduct(1000, patchdoc)).Returns(product);
            mockProductRepo.Setup(p => p.IsExist(1000)).Returns(stat);
            //Act
            var res = controller.PatchProduct(1000, patchdoc);
            //Assert
            Assert.IsType<NotFoundObjectResult>(res.Result);
        }

        [Fact]
        public void PatchProduct_ReplaceIdError_ReturnsBadRequest()
        {
            //Arrange
            JsonPatchDocument<Product> patchdoc = new JsonPatchDocument<Product>();
            patchdoc.Replace(x => x.Name, "Patched Name");
            patchdoc.Replace(x => x.UnitCost, 120);
            patchdoc.Replace(x => x.ProductId, 1005);

            Product product = new Product { Name = "Patched Name", ProductId = 1005, Quantity = 40, UnitCost = 120 };

            var stat = GetList().Exists(x => x.ProductId == 1001);
            mockProductRepo.Setup(p => p.PatchProduct(1001, patchdoc)).Returns(product);
            mockProductRepo.Setup(p => p.IsExist(1001)).Returns(stat);
            //Act
            var res = controller.PatchProduct(1001, patchdoc);
            //Assert
            Assert.IsType<BadRequestObjectResult>(res.Result);
        }

        [Fact]
        public void PatchProduct_ValidObjectPassed_ReturnsOkResult()
        {
            //Arrange
            JsonPatchDocument<Product> patchdoc = new JsonPatchDocument<Product>();
            patchdoc.Replace(x => x.Name, "Patched Name");
            patchdoc.Replace(x => x.UnitCost, 120);
            patchdoc.Replace(x => x.Quantity, 30);

            Product product = new Product { Name = "Patched Name", ProductId = 1001, Quantity = 30, UnitCost = 120 };

            var stat = GetList().Exists(x => x.ProductId == 1001);
            mockProductRepo.Setup(p => p.PatchProduct(1001, patchdoc)).Returns(product);
            mockProductRepo.Setup(p => p.GetProduct(1001)).Returns(GetList()[0]);
            mockProductRepo.Setup(p => p.IsExist(1001)).Returns(stat);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                              It.IsAny<ValidationStateDictionary>(),
                                              It.IsAny<string>(),
                                              It.IsAny<Object>()));
            controller.ObjectValidator = objectValidator.Object;

            //Act
            var res = controller.PatchProduct(1001, patchdoc);
            //Assert
            Assert.IsType<OkObjectResult>(res.Result);
        }

        [Fact]
        public void PatchProduct_ValidObjectPassed_ReturnsUpdatedObject()
        {
            //Arrange
            JsonPatchDocument<Product> patchdoc = new JsonPatchDocument<Product>();
            patchdoc.Replace(x => x.Name, "Patched Name");
            patchdoc.Replace(x => x.UnitCost, 120);
            patchdoc.Replace(x => x.Quantity, 30);

            Product product = new Product { Name = "Patched Name", ProductId = 1001, Quantity = 30, UnitCost = 120 };

            var stat = GetList().Exists(x => x.ProductId == 1001);
            mockProductRepo.Setup(p => p.PatchProduct(1001, patchdoc)).Returns(product);
            mockProductRepo.Setup(p => p.GetProduct(1001)).Returns(GetList()[0]);
            mockProductRepo.Setup(p => p.IsExist(1001)).Returns(stat);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                              It.IsAny<ValidationStateDictionary>(),
                                              It.IsAny<string>(),
                                              It.IsAny<Object>()));
            controller.ObjectValidator = objectValidator.Object;

            //Act
            var res = controller.PatchProduct(1001, patchdoc).Result as OkObjectResult;
            //Assert
            Assert.IsType<Product>(res.Value);
            Assert.Equal(product, res.Value);
        }

    }
}
