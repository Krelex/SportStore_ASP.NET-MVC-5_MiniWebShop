using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportStore.Domain.Entities;
using System.Linq;
using Moq;
using SportStore.Domain.Abstract;
using SportStore.WebUI.Controllers;
using System.Web.Mvc;
using SportStore.WebUI.Models;

namespace SportStore.UnitTest
{
    /// <summary>
    /// Summary description for CartTest
    /// </summary>
    [TestClass]
    public class CartTest
    {
        [TestMethod]
        public void CAN_Add_new_lines_and_can_add_existing()
        {
            Product prod1 = new Product { ProductID = 1, Price = 100, Name = "Carot" };
            Product prod2 = new Product { ProductID = 2, Price = 10, Name = "Carot" };

            Cart kosara = new Cart();
            kosara.AddItem(prod1, 1);
            kosara.AddItem(prod2, 2);
            kosara.AddItem(prod1, 1);
            kosara.AddItem(prod2, 2);

            CartLine[] kosaraArray = kosara.Lines.ToArray();

            Assert.AreEqual(kosara.Lines.Count(), 2);
            Assert.AreEqual(kosaraArray.Length, 2);
            Assert.AreEqual(kosaraArray[0].Product.Name, "Carot");
            Assert.AreEqual(kosaraArray[1].Quantity, 4);


        }

        [TestMethod]
        public void CAN_Remove_Item()
        {
            Product prod1 = new Product { ProductID = 1, Price = 100, Name = "Carot" };
            Product prod2 = new Product { ProductID = 2, Price = 10, Name = "Carot2" };
            Product prod3 = new Product { ProductID = 3, Price = 10, Name = "Carot3" };


            Cart kosara = new Cart();
            kosara.AddItem(prod1, 1);
            kosara.AddItem(prod2, 2);
            kosara.AddItem(prod1, 1);
            kosara.AddItem(prod2, 2);
            kosara.AddItem(prod3, 2);

            kosara.RemoveLine(prod2);
            kosara.RemoveLine(prod1);


            CartLine[] kosaraArray = kosara.Lines.ToArray();


            Assert.AreEqual(kosara.Lines.Count(), 1);
            Assert.AreEqual(kosara.Lines.First().Product.ProductID, 3);

        }

        [TestMethod]
        public void CAN_Compute_items()
        {
            Product prod1 = new Product { ProductID = 1, Price = 100, Name = "Carot" };
            Product prod2 = new Product { ProductID = 2, Price = 10, Name = "Carot" };

            Cart kosara = new Cart();
            kosara.AddItem(prod1, 1);
            kosara.AddItem(prod2, 2);
            kosara.AddItem(prod1, 1);
            kosara.AddItem(prod2, 2);


            CartLine[] kosaraArray = kosara.Lines.ToArray();


            Assert.AreEqual(kosara.ComputeTotalValue(), 240);

        }

        [TestMethod]
        public void CAN_Clear_Cart()
        {
            Product prod1 = new Product { ProductID = 1, Price = 100, Name = "Carot" };
            Product prod2 = new Product { ProductID = 2, Price = 10, Name = "Carot2" };
            Product prod3 = new Product { ProductID = 3, Price = 10, Name = "Carot3" };


            Cart kosara = new Cart();
            kosara.AddItem(prod1, 1);
            kosara.AddItem(prod2, 2);
            kosara.AddItem(prod1, 1);
            kosara.AddItem(prod2, 2);
            kosara.AddItem(prod3, 2);
            kosara.AddItem(prod2, 2);

            kosara.Clear();


            CartLine[] kosaraArray = kosara.Lines.ToArray();

            Assert.AreEqual(kosara.Lines.Count(), 0);
            Assert.AreEqual(kosaraArray.Length, 0);

        }

        [TestMethod]
        public void CAN_Add_To_Cart()
        {
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Product).Returns(new Product[] {
                new Product { ProductID = 1, Price = 100, Name = "Carot" },
                new Product { ProductID = 2, Price = 10, Name = "Carot2" },
                new Product { ProductID = 3, Price = 10, Name = "Carot3" }
            }.AsQueryable());

            Cart cart = new Cart();

            CartController target = new CartController(mock.Object, null);

            target.AddToCart(cart, 3, null);

            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 3);

        }

        [TestMethod]
        public void Adding_Product_Goes_To_Cart_Screen()
        {
            //Arrange
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Product).Returns(new Product[] {
                new Product { ProductID = 1, Price = 100, Name = "Carot" },
                new Product { ProductID = 2, Price = 10, Name = "Carot2" },
                new Product { ProductID = 3, Price = 10, Name = "Carot3" }
            }.AsQueryable());

            Cart cart = new Cart();

            CartController target = new CartController(mock.Object, null);

            //Act
            RedirectToRouteResult result = target.AddToCart(cart, 2, "myUrl");

            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");

        }

        [TestMethod]
        public void Can_View_Cart_Contents()
        {
            //Arrange

            Cart cart = new Cart();

            CartController traget = new CartController(null, null);

            //Act
            CartIndexViewModel result = (CartIndexViewModel)traget.Index(cart, "Myurl").ViewData.Model;


            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "Myurl");

        }

        [TestMethod]
        public void Cannot_Checkout_Empty_Cart()
        {
            // Arrange - create a mock order processor
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            // Arrange - create an empty cart
            Cart cart = new Cart();
            // Arrange - create shipping details
            ShippingDetails shippingDetails = new ShippingDetails();
            // Arrange - create an instance of the controller
            CartController target = new CartController(null, mock.Object);
            // Act
            ViewResult result = target.Chekout(cart, shippingDetails);
            // Assert - check that the order hasn't been passed on to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()),
            Times.Never());
            // Assert - check that the method is returning the default view
            Assert.AreEqual("", result.ViewName);
            // Assert - check that I am passing an invalid model to the view
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Cannot_Checkout_Invalid_ShippingDetails()
        {
            // Arrange - create a mock order processor
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            // Arrange - create a cart with an item
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            // Arrange - create an instance of the controller
            CartController target = new CartController(null, mock.Object);
            // Arrange - add an error to the model
            target.ModelState.AddModelError("error", "error");
            // Act - try to checkout
            ViewResult result = target.Chekout(cart, new ShippingDetails());
            // Assert - check that the order hasn't been passed on to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()),
            Times.Never());
            // Assert - check that the method is returning the default view
            Assert.AreEqual("", result.ViewName);
            // Assert - check that I am passing an invalid model to the view
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Can_Checkout_And_Submit_Order()
        {
            // Arrange - create a mock order processor
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            // Arrange - create a cart with an item
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            // Arrange - create an instance of the controller
            CartController target = new CartController(null, mock.Object);
            // Act - try to checkout
            ViewResult result = target.Chekout(cart, new ShippingDetails());
            // Assert - check that the order has been passed on to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()),
            Times.Once());
            // Assert - check that the method is returning the Completed view
            Assert.AreEqual("Completed", result.ViewName);
            // Assert - check that I am passing a valid model to the view
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
        }
     }
}
