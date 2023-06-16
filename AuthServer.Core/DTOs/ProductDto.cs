using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public Decimal Price { get; set; }//decimal büyük mü
      
        public string UserId { get; set; }//identity de userid string olarak tutulacak.
    }
}
