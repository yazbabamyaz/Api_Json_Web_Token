using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Models
{
    //identity.EntityframeworkCore kur.identityuser bunu içinde6.0.16
    public class UserApp:IdentityUser
    {
        //identity kütüphanesi customize edilebilir-ekleme yapabiliyorsun-
       public string? City { get; set; }//identity user da olmayan propertyleri eklersin.
    }

    //public class userRole:IdentityRole
    //{
    //    //istediğin role ile alakalı property ekleyebilirsin.Ama biz kullanmadık.
    //}
}
