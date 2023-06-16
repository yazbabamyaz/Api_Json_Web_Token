using AuthServer.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        //dbcontext dışındaki bir entity üzerinde ayar için interface implement etmek gerek.
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            //Farklı şekilde de yapılabilir. data annotation [Key] yazarsın property üzerine
            // ya da zaten Id ismini EF direk algılar ya da ProductId yazınca da algılar.
            //bestpractise buymuş
            builder.HasKey(x => x.Id);//Id nin primary key olduğunu belirtir.
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Stock).IsRequired();
            builder.Property(x => x.Price).HasColumnType("decimal(18,2)");//toplam 18 basamak virgülden saonra 2
            builder.Property(x => x.UserId).IsRequired();
            
        }
    }
}
