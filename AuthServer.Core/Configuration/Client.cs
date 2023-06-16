using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Configuration
{
    //apiye istekte bulunan cihazlar-kullanıcılar.
    public class Client
    {
        public string Id { get; set; }
        public string Secret { get; set; }
        public List<String> Audiences { get; set; }//string s si buyuk mü? client makine hangi apiye ulaşacak.
    }
}
