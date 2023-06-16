using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MiniApp1.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetStock()
        {

            //Name , İstek yapıldığınca tokenın payload kısmından gelir.
            var userName = HttpContext.User.Identity.Name;
            //id yi ise şu şekilde alırız. type ı NameIdentifier olanı ver.
            var userId =User.Claims.FirstOrDefault(x=>x.Type==ClaimTypes.NameIdentifier);

            //2 si üzerinden de Veritabanında de işlem yapabiliriz. ikisi de uniq ve string
            //claim nesnesinin 2 değeri vardı type-value  biz value aldık.
            return Ok($"Stock işlemleri=>User Name:{userName}- User Id:{userId.Value}");
        }
    }
}
