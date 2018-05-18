using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportStore.Domain.Abstract;
using SportStore.Domain.Entities;
using System.Collections.Generic;
using SportStore.WebUI.Controllers;
using System.Linq;
using System.Web.Mvc;
using SportStore.WebUI.Models;
using SportStore.WebUI.PagingInfo;
using Microsoft.CSharp;

namespace SportStore.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Pagination()
        {
            // Arrange
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(p => p.Product).Returns(new List<Product> 
            {
                new Product {ProductID = 1, Name = "P1", Category="Chess"},
                new Product {ProductID = 2, Name = "P2", Category="Soccer"},
                new Product {ProductID = 3, Name = "P3", Category="Chess"},
                new Product {ProductID = 4, Name = "P4", Category="Chess"},
                new Product {ProductID = 5, Name = "P5", Category="Soccer"},
                new Product {ProductID = 6, Name = "P6", Category="Chess"}
            });
           
            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 6;

            // Act
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 1).Model;

            // Assert
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 6);
            Assert.AreEqual(prodArray[0].Name, "P1");
            Assert.AreEqual(prodArray[0].ProductID, 1);

        }

        [TestMethod]
        public void Page_links()
        {
            //Arrange
            HtmlHelper myHelper = null;

            PagingInfo pagingInfo = new PagingInfo()
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemPerPage = 10
            };

            int result1 = pagingInfo.TotalPage;
            Func<int, string> pageUrl = i => "Page" + i; 

            //Act
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrl);

            //Assert
            Assert.AreEqual(3, result1);
            Assert.AreEqual(@"<a class=""btn btn-outline-secondary"" href=""Page1"">1</a>"
            + @"<a class=""btn btn-outline-secondary btn-primary active"" href=""Page2"">2</a>"
            + @"<a class=""btn btn-outline-secondary"" href=""Page3"">3</a>"
            , result.ToString());
        }


        [TestMethod]
        public void Page_Model_View_Model()
        {
            //Arrange
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(p => p.Product).Returns(new List<Product>
            {
               new Product {ProductID = 1, Name = "P1", Category="Chess"},
                new Product {ProductID = 2, Name = "P2", Category="Soccer"},
                new Product {ProductID = 3, Name = "P3", Category="Chess"},
                new Product {ProductID = 4, Name = "P4", Category="Chess"},
                new Product {ProductID = 5, Name = "P5", Category="Soccer"},
                new Product {ProductID = 6, Name = "P6", Category="Chess"}
            });

            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 2;
            //Act

            ProductsListViewModel result = (ProductsListViewModel)controller.List(null,2).Model;

            //Assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(6, pageInfo.TotalItems);
            Assert.AreEqual(3, pageInfo.TotalPage);
            Assert.AreEqual(2, pageInfo.ItemPerPage);
            Assert.AreEqual(2, pageInfo.CurrentPage);
        }

        [TestMethod]
        public void string_filter()
        {
            // Arrange
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(p => p.Product).Returns(new List<Product>
            {
                new Product {ProductID = 1, Name = "P1", Category="Chess"},
                new Product {ProductID = 2, Name = "P2", Category="Soccer"},
                new Product {ProductID = 3, Name = "P3", Category="Chess"},
                new Product {ProductID = 4, Name = "P4", Category="Chess"},
                new Product {ProductID = 5, Name = "P5", Category="Soccer"},
                new Product {ProductID = 6, Name = "P6", Category="Chess"}
            });

            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 6;

            // Act
            ProductsListViewModel result = (ProductsListViewModel)controller.List("Chess", 1).Model;

            // Assert
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 4);
            Assert.AreEqual(prodArray[0].Name, "P1");
            Assert.AreEqual(prodArray[0].ProductID, 1);
            Assert.AreEqual(prodArray[3].Name, "P6");
            Assert.AreEqual(prodArray[3].ProductID, 6);
            Assert.IsTrue(prodArray[1].Name == "P3" && prodArray[1].Category == "Chess");

        }

        [TestMethod]
        public void Create_Categories()
        {
            // Arrange
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(p => p.Product).Returns(new List<Product>
            {
                new Product {ProductID = 1, Name = "P1", Category="Chess"},
                new Product {ProductID = 2, Name = "P2", Category="Soccer"},
                new Product {ProductID = 3, Name = "P3", Category="Chess"},
                new Product {ProductID = 4, Name = "P4", Category="Chess"},
                new Product {ProductID = 5, Name = "P5", Category="Soccer"},
                new Product {ProductID = 6, Name = "P6", Category="Chess"}
            });

            NavController controller = new NavController(mock.Object);
            

            // Act
            IEnumerable<string> result = (IEnumerable<string>)controller.Menu(null).Model;

            // Assert
            string[] prodArray = result.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0], "Chess");
            Assert.AreEqual(prodArray[1], "Soccer");
        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            // Arrange
            // - create the mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Product).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Apples"},
                new Product {ProductID = 4, Name = "P2", Category = "Oranges"},
                    });
            // Arrange - create the controller
            NavController target = new NavController(mock.Object);
            // Arrange - define the category to selected
            string categoryToSelect = "Apples";
            // Action
            string result = target.Menu(categoryToSelect).ViewBag.categorie;
            // Assert
            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Filter_Categories()
        {
            // Arrange
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(p => p.Product).Returns(new List<Product>
            {
                new Product {ProductID = 1, Name = "P1", Category="Chess"},
                new Product {ProductID = 2, Name = "P2", Category="Soccer"},
                new Product {ProductID = 3, Name = "P3", Category="Chess"},
                new Product {ProductID = 4, Name = "P4", Category="Chess"},
                new Product {ProductID = 5, Name = "P5", Category="Soccer"},
                new Product {ProductID = 6, Name = "P6", Category="Chess"}
            });

            ProductController controller = new ProductController(mock.Object);


            // Act
            ProductsListViewModel result1 = (ProductsListViewModel)controller.List("Chess" ,1).Model;
            ProductsListViewModel result2 = (ProductsListViewModel)controller.List("Soccer", 2).Model;
            ProductsListViewModel resultAll = (ProductsListViewModel)controller.List(null).Model;


            // Assert
            int totalItems1 = result1.PagingInfo.TotalItems;
            int totalItems2 = result2.PagingInfo.TotalItems;
            int totalItemsAll = resultAll.PagingInfo.TotalItems;


            Assert.AreEqual(4 , totalItems1);
            Assert.AreEqual(2, totalItems2);
            Assert.AreEqual(6, totalItemsAll);
        }

       
    }
}
