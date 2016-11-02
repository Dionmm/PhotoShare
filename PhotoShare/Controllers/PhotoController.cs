using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace PhotoShare.Controllers
{
    [Authorize(Roles = "administrator, photographer")]
    [RoutePrefix("api/Photo")]
    public class PhotoController : ApiController
    {
        [Route("test")]
        public IHttpActionResult GetTest()
        {
            return Ok();
        }
    }
}
