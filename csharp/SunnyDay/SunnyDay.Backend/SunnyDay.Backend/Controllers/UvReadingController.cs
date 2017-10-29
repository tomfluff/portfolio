using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using SunnyDay.Backend.DataObjects;
using SunnyDay.Backend.Models;

namespace SunnyDay.Backend.Controllers
{
    [Authorize]
    public class UvReadingController : TableController<UvReading>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<UvReading>(context, Request);
        }

        // GET tables/UvReading
        public IQueryable<UvReading> GetAllUvReading()
        {
            // User (IPrincipal) is the current user that calls the api
            // ClaimsPrincipal claimsUser = User as ClaimsPrincipal;
            // string sid = claimsUser?.FindFirst(ClaimTypes.NameIdentifier).Value;
            // .Where(v => v.UserId == sid)

            return Query(); 
        }

        // GET tables/UvReading/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<UvReading> GetUvReading(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/UvReading/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<UvReading> PatchUvReading(string id, Delta<UvReading> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/UvReading
        public async Task<IHttpActionResult> PostUvReading(UvReading item)
        {
            UvReading current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/UvReading/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteUvReading(string id)
        {
             return DeleteAsync(id);
        }
    }
}
