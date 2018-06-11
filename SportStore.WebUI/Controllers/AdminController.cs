using SportStore.Domain.Abstract;
using SportStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportStore.WebUI.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private IProductsRepository _repository;

        public AdminController(IProductsRepository repo)
        {
            _repository = repo;
        }
        // GET: Admin
        public ViewResult Index()
        {
            return View(_repository.Product);
        }

        public ViewResult Edit(int id)
        {
            Product prod = _repository.Product.Where(p => p.ProductID == id).SingleOrDefault();

            return View(prod);
        }

        [HttpPost]
        public ActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _repository.SaveProduct(product);
                TempData["message"] = string.Format("{0} has been saved", product.Name);
                return RedirectToAction("Index");
            }else
            {
                return View(product);
            }
        }

        public ActionResult Create()
        {
            return View("Edit", new Product());
        }

        [HttpPost]
        public ActionResult Delete(int productId)
        {
            //int productId =int.Parse(Request.Form["productId"]);
            Product deleteProduct = _repository.Product.Where(pr => pr.ProductID == productId).SingleOrDefault();

            if(deleteProduct != null)
            {
                TempData["messageDel"] = string.Format("{0} was deleted", deleteProduct.Name);
                _repository.DeleteProduct(productId);
            }

            return RedirectToAction("Index");
        }

    }
}