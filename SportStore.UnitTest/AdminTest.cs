using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportStore.Domain.Abstract;
using SportStore.Domain.Entities;
using SportStore.WebUI.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SportStore.WebUI.Infrastructure.Abstract;
using SportStore.WebUI.Models;

namespace SportStore.UnitTest
{
    [TestClass]
    public class AdminTest
    {
        [TestMethod]
        public void Index_Contain_All_Products()
        {
            //Arrange
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Product).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1" },
                new Product {ProductID = 2, Name = "P2" },
                new Product {ProductID = 3, Name = "P3" }

            });

            AdminController target = new AdminController(mock.Object);

            //Act
            Product[] result = ((IEnumerable<Product>)target.Index().ViewData.Model).ToArray();

            //Assert
            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual("P1", result[0].Name);
            Assert.AreEqual("P2", result[1].Name);
            Assert.AreEqual("P3", result[2].Name);
        }

        [TestMethod]
        public void Can_Edit_Product()
        {
            //Arrange
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Product).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1" },
                new Product {ProductID = 2, Name = "P2" },
                new Product {ProductID = 3, Name = "P3" }

            });

            AdminController target = new AdminController(mock.Object);

            //Act
            Product result = ((Product)target.Edit(1).ViewData.Model);
            Product result2 = ((Product)target.Edit(3).ViewData.Model);


            //Assert
            Assert.AreEqual(result.Name, "P1");
            Assert.AreEqual(result2.Name, "P3");
            Assert.AreEqual(result2.ProductID, 3);
        }
        [TestMethod]
        public void CanNOT_Edit_null_Product()
        {
            //Arrange
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Product).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1" },
                new Product {ProductID = 2, Name = "P2" },
                new Product {ProductID = 3, Name = "P3" }

            });

            AdminController target = new AdminController(mock.Object);

            //Act
            Product result = ((Product)target.Edit(4).ViewData.Model);
            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Can_Update_Product_Repo()
        {
            //Arrange
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Product).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1" },
                new Product {ProductID = 2, Name = "P2" },
                new Product {ProductID = 3, Name = "P3" }

            });

            Product product = new Product() { Name = "Test" };

            AdminController target = new AdminController(mock.Object);

            //Act
            ActionResult result = target.Edit(product);

            //Assert
            mock.Verify(m => m.SaveProduct(product));
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Can_Delete_Valid_Products()
        {

            // Arrange - create a Product    
            Product prod = new Product { ProductID = 2, Name = "Test" };

            // Arrange - create the mock repository    
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Product).Returns(new Product[] {
                    new Product {ProductID = 1, Name = "P1"},
                    prod,
                    new Product {ProductID = 3, Name = "P3"},
            });

            // Arrange - create the controller    
            AdminController target = new AdminController(mock.Object);

            // Act - delete the product    
            target.Delete(prod.ProductID);

            // Assert - ensure that the repository delete method was    
            // called with the correct Product    
            mock.Verify(m => m.DeleteProduct(prod.ProductID));
        }


        [TestMethod]
        public void Can_Login_With_Valid_Credentials()
        {

            // Arrange - create a mock authentication provider            
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("admin", "secret")).Returns(true);

            // Arrange - create the view model
            LoginViewModel model = new LoginViewModel()
            {
                UserName = "admin",
                Password = "secret"
            };

            // Arrange - create the controller
            AccountController target = new AccountController(mock.Object);

            // Act - authenticate using valid credentials
            ActionResult result = target.Login(model, "/MyURL");

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.AreEqual("/MyURL", ((RedirectResult)result).Url);
        }

        [TestMethod]
        public void Cannot_Login_With_Invalid_Credentials()
        {

            // Arrange - create a mock authentication provider
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("badUser", "badPass")).Returns(false);

            // Arrange - create the view model
            LoginViewModel model = new LoginViewModel {
                UserName = "badUser",
                Password = "badPass"
            };

            // Arrange - create the controller
            AccountController target = new AccountController(mock.Object);

            // Act - authenticate using valid credentials 
            ActionResult result = target.Login(model, "/MyURL");

            // Assert            
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
        }
    }
}
