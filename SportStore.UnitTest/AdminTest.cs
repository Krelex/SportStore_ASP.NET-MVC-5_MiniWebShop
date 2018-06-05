using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportStore.Domain.Abstract;
using SportStore.Domain.Entities;
using SportStore.WebUI.Controllers;
using System.Collections.Generic;
using System.Linq;

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
    }
}
