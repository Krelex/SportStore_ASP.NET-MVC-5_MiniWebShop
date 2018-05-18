using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportStore.Domain.Entities
{
    public class Cart
    {
        private List<CartLine> LineCollection = new List<CartLine>();

        public void AddItem(Product product, int quantity)
        {
            CartLine line = LineCollection
                .Where(p => p.Product.ProductID == product.ProductID)
                .FirstOrDefault();

            if (line == null)
            {
                LineCollection.Add(new CartLine
                {
                    Product = product,
                    Quantity = quantity
                });
            }
            else
            {
                line.Quantity += quantity;
            }
        }

        public void RemoveLine(Product product)
        {
            LineCollection.RemoveAll(l => l.Product.ProductID == product.ProductID );
        }

        public decimal ComputeTotalValue()
        {
            return LineCollection.Sum(l => l.Product.Price * l.Quantity);
        }

        public void Clear()
        {
            LineCollection.Clear();
        }
        public  IEnumerable<CartLine> Lines
        {
            get { return LineCollection; }
        }

    }

    public class CartLine
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
