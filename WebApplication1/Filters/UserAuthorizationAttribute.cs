using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace WebApplication1.Filters
{
    public class UserAuthorizationAttribute : AuthorizeAttribute
    {
        public UserAuthorizationAttribute() { }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            string accessToken = actionContext.Request.Headers.Authorization?.Parameter;

            if (accessToken == null || (accessToken != null && !Authentication.IsTokenValid(accessToken)))
            {
                return false;
            }
            return true;
        }
    }
}