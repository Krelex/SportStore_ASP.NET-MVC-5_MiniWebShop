﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportStore.Domain.Abstract;
using SportStore.Domain.Entities;

namespace SportStore.Domain.Concrete
{
    public class EFProductRepository : IProductsRepository 
    {
        private EFDbContext contex = new EFDbContext();

        public IEnumerable<Product> Product
        {
            get
            {
                return contex.Products;
            }
        }

        public Product DeleteProduct(int productID)
        {
            Product DelProduct = contex.Products.Find(productID);
            if(DelProduct != null)
            {
                contex.Products.Remove(DelProduct);
                contex.SaveChanges();
            }

            return DelProduct;
        }

        public void SaveProduct(Product product)
        {
            if (product.ProductID == 0)
            {
                //int maxID = contex.Products.Max(p => p.ProductID);
                //product.ProductID = maxID + 1;
                contex.Products.Add(product);
            }else
            {
                Product dbEntry = contex.Products.Find(product.ProductID);
                if(dbEntry != null)
                {
                    dbEntry.Name = product.Name;
                    dbEntry.Description = product.Description;
                    dbEntry.Category = product.Category;
                    dbEntry.Price = product.Price;
                }
            }
            contex.SaveChanges();
        }
    }
}
