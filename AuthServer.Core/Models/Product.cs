using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public Decimal Price { get; set; }//decimal büyük mü
        public int Stock { get; set; }
        public string UserId { get; set; }//identity de userid string olarak tutulacak.
    }
}
