using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inwinteck_CRM
{




   

    public class DecryptParameterAttribute : ActionFilterAttribute
    {
        private readonly string _parameterName;

        public DecryptParameterAttribute(string parameterName)
        {
            _parameterName = parameterName;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionParameters.ContainsKey(_parameterName))
            {
                var encryptedValue = filterContext.ActionParameters[_parameterName] as string;
                if (!string.IsNullOrEmpty(encryptedValue))
                {
                    try
                    {
                        filterContext.ActionParameters[_parameterName] = UrlEncryptionHelper.Decrypt(encryptedValue);
                    }
                    catch (Exception)
                    {
                        filterContext.Result = new HttpStatusCodeResult(400, "Invalid encrypted parameter");
                        return;
                    }
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }

}
