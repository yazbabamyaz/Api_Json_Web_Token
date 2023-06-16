using AuthServer.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data
{
    //üyelik sistemi ile alakalı tabloları da tutacağı için IdentityDbContext dedik
    //db de tablo oluşurken hangi modele göre oluşacak.<UserApp>
    //IdentityRole hazır sınıftır
    //primary key için type belirt diyor:Biz string dedik best practise
    public class AppDbContext:IdentityDbContext<UserApp,IdentityRole,string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
            //base constructora optionsı gönderiyoruz.
            //constructor da dbcontext options alacak biz bunu program.cs de doldurcaz.
        }
        
        public DbSet<Product> Products { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        //kullanıcıyla alakalı diğer tüm dbsetler miras olarak geliyor.
        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            ////db de tablolar oluşurken sütunların yapılarının ne olacağı ayarları için (required vs gibi):
            //20 tane dbset varsa hepsinin özelliklerini burda yazmak burayı şişirir.Bu yüzden ilgili entitylerle alakalı configuration dosyaları oluşturcaz.

            //bana bir assembly ver ben onun içindeki tüm configuration dosyalarını(yani IEntityconfg.. implemente etmiş tüm classları) bulup eklicem.
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            base.OnModelCreating(builder);
        }

    }
}
