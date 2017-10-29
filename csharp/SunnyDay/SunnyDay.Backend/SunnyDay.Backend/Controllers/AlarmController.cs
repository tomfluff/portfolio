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
    public class AlarmController : TableController<Alarm>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<Alarm>(context, Request);
        }

        // GET tables/Alarm
        public IQueryable<Alarm> GetAllAlarm()
        {
            // User (IPrincipal) is the current user that calls the api
            // ClaimsPrincipal claimsUser = User as ClaimsPrincipal;
            // string sid = claimsUser?.FindFirst(ClaimTypes.NameIdentifier).Value;
            // .Where(v => v.UserId == sid)
            return Query(); 
        }

        // GET tables/Alarm/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Alarm> GetAlarm(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Alarm/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Alarm> PatchAlarm(string id, Delta<Alarm> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Alarm
        public async Task<IHttpActionResult> PostAlarm(Alarm item)
        {
            Alarm current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Alarm/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteAlarm(string id)
        {
             return DeleteAsync(id);
        }
    }
}
