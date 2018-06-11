using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportStore.Domain.Abstract;
using SportStore.Domain.Entities;
using SportStore.WebUI.Models;


namespace SportStore.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private IProductsRepository _repository;
        public int pageSize = 2 ;
        
        public ProductController(IProductsRepository repository)
        {
            this._repository = repository;
        }

        public ViewResult List(string category, int page = 1 )
        {
            ProductsListViewModel model = new Models.ProductsListViewModel();

            model.Products = _repository.Product
                .Where(p => p.Category.Trim() == category || category == null)
                .OrderBy(p => p.Price)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            model.PagingInfo = new Models.PagingInfo()
            {
                TotalItems = _repository.Product
                            .Where(p => p.Category.Trim() == category || category == null)
                            .Count(),
                ItemPerPage = pageSize,
                CurrentPage = page
            };

            model.CurrentCategory = category;

            return View(model);
        }

        public FileContentResult GetImage(int productId)
        {
            Product prod = _repository.Product
                .FirstOrDefault(p => p.ProductID == productId);

            if(prod != null)
            {
                return File(prod.ImageData, prod.ImageMimeType);
            }else
            {
                return null;
            }
        }
    }
}