using SportStore.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportStore.WebUI.Controllers
{
    public class NavController : Controller
    {
        private IProductsRepository _repository;

        public NavController(IProductsRepository repo)
        {
            _repository = repo;
        }


        // GET: Nav
        public PartialViewResult Menu(string category)
        {
            ViewBag.categorie = category;
            IEnumerable<string> categories = _repository.Product
                                            .Select(p => p.Category.Trim())
                                            .Distinct()
                                            .OrderBy(p => p);

            return PartialView(categories);
        }
    }
}