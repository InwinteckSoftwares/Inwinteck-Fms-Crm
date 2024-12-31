using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Security.Principal;
using Inwinteck_CRM.Models;
namespace Inwinteck_CRM.ApiAuthorization
{
    public class InwinAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            
            if (actionContext.Request.Headers.Authorization == null)
            {
                generalresponse gr = new generalresponse();
                gr.statuscode = 1;
                gr.statusmessage = "Unauthorised Access.";
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, gr);
            }
            else
            {
                string authenticationToken = actionContext.Request.Headers.Authorization.Parameter;

                string decodedAuthenticationToken = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationToken));
                string[] usernamePasswordArray = decodedAuthenticationToken.Split(':');
                string username = usernamePasswordArray[0];
                string password = usernamePasswordArray[1];
                if (Security.SourceLogin(username, password))
                {
                    Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(username), null);
                }
                else
                {
                    generalresponse gr = new generalresponse();
                    gr.statuscode = 1;
                    gr.statusmessage = "Unauthorised Access.";
                 actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized,gr);
                }
            }

        }
    }
}