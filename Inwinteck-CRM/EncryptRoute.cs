using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

//public class EncryptedRoute : RouteBase
//{
    //public override RouteData GetRouteData(HttpContextBase httpContext)
    //{
    //    string path = httpContext.Request.Path.TrimStart('/');

    //    try
        
    //    {
    //        string decryptedPath = UrlEncryptionHelper.Decrypt(path);
    //        string[] segments = decryptedPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

    //        if (segments.Length < 2)
    //            return null;

    //        RouteData routeData = new RouteData(this, new MvcRouteHandler());
    //        routeData.Values["controller"] = segments[0];
    //        routeData.Values["action"] = segments[1];

    //        for (int i = 2; i < segments.Length; i += 2)
    //        {
    //            if (i + 1 < segments.Length)
    //            {
    //                routeData.Values[segments[i]] = segments[i + 1];
    //            }
    //        }

    //        return routeData;
    //    }
    //    catch
    //    {
    //        return null;
    //    }
    //}

    //public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
    //{
    //    string controller = values["controller"]?.ToString();
    //    string action = values["action"]?.ToString();

    //    if (string.IsNullOrEmpty(controller) || string.IsNullOrEmpty(action))
    //        return null;

    //    StringBuilder pathBuilder = new StringBuilder();
    //    pathBuilder.Append(controller).Append('/').Append(action);

    //    foreach (var kvp in values)
    //    {
    //        if (kvp.Key == "controller" || kvp.Key == "action")
    //            continue;

    //        pathBuilder.Append('/').Append(kvp.Key).Append('/').Append(kvp.Value);
    //    }

    //    string encryptedPath = UrlEncryptionHelper.Encrypt(pathBuilder.ToString());
    //    return new VirtualPathData(this, encryptedPath);
    //}
//}
