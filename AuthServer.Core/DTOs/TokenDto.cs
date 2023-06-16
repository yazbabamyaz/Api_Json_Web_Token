using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.DTOs
{
    public class TokenDto
    {
        public string AccessToken { get; set; }//token string bir ifadedir 3 parçadan oluşan.base64 ile encode edilmiş.
        public DateTime AccessTokenExpiration { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set;}
    }
}
