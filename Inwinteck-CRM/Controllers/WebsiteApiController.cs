using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Inwinteck_CRM.Models;
using System.IO;
using System.Web.Http.Cors;
using System.Web.Hosting;
using System.Web;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Inwinteck_CRM.Controllers
{
    [RoutePrefix("api/WebsiteApi")]
    public class WebsiteApiController : ApiController
    {
        ApplicationDbContext db = new ApplicationDbContext();

        ApplicationDbContext1 db1 = new ApplicationDbContext1();

     //   [EnableCors(origins: "https://inwinteck.com/", headers: "*", methods: "*")]
        [HttpGet]
        public IHttpActionResult GETFERegCount()
        {
            int cnt;
            try
            {
                cnt = (from s in db.FE_Master_Personal select s).Count();
            }
            catch (Exception ex)
            {
                cnt = 0;
            }
            return Ok(cnt);
        }

        [HttpPost]
        public IHttpActionResult SendContact(Contact sa)
        {
            string body = utlity.WebEnquiry(sa.comp, sa.name, sa.desig, sa.email, sa.country, sa.state, sa.location,sa.mobile,sa.requirement,sa.msg);
            string recp = "sales@inwinteck.com";
            string subject = "Website Enquiry";
            string res = utlity.sendmailWebsite(recp, subject, body, sa.email);
            
            if (res == "true")
            {
                return Redirect("https://inwinteck.com/thank-you.html?res=1");
            }else
            {
                return Redirect("https://inwinteck.com/error.html");
            }
           // return Ok();
            
        }


        [HttpPost]
        public async Task<HttpResponseMessage> SendResume()
        {
            Career CR = new Career();
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }
                string root = HttpContext.Current.Server.MapPath("~/Upload/HR");

                var provider = new MultipartFormDataStreamProvider(root);
                await Request.Content.ReadAsMultipartAsync(provider);
                if (provider.FileData.Any())
                {
                    foreach (var fileData in provider.FileData)
                    {

                        FileInfo fi = new FileInfo(fileData.LocalFileName);
                        string fileName = "";
                        string type = "";
                        //getting the file saving path
                        string clientFileName = fileData.Headers.ContentDisposition.FileName.Replace(@"""", "");
                        if (clientFileName != "")
                        {
                            string clientExtension = clientFileName.Substring(clientFileName.LastIndexOf('.'));
                            Random _r = new Random();
                            string rand = _r.Next().ToString();
                            string strHash = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + rand;
                            type = fileData.Headers.ContentDisposition.Name;
                            string physicalPath = "";
                            string relativePath = "";
                            if (type == "\"resume\"")
                            {
                                fileName = "resume_" + strHash;
                                physicalPath = HttpContext.Current.Server.MapPath("~/Upload/HR");
                                relativePath = Path.Combine(physicalPath, fileName + clientExtension).Replace("\\", "/");

                                CR.resume = relativePath;
                            }
                            else if (type == "\"cover\"")
                            {
                                fileName = "cover_" + strHash;
                                physicalPath = HttpContext.Current.Server.MapPath("~/Upload/HR");
                                relativePath = Path.Combine(physicalPath, fileName + clientExtension).Replace("\\", "/"); CR.cover = relativePath;
                            }


                            FileInfo fiOld = new FileInfo(relativePath);
                            if (fiOld.Exists)
                                fiOld.Delete();
                            fi.MoveTo(relativePath);

                        }
                        else
                        {
                            if (fi.Exists)
                                fi.Delete();
                        }
                    }
                }
                foreach (var key in provider.FormData.AllKeys)
                {
                    foreach (var val in provider.FormData.GetValues(key))
                    {
                        Trace.WriteLine(string.Format("{0}: {1}", key, val));
                        if (key == "name")
                        {
                            CR.name = string.Format("{0}: {1}", key, val);
                        }
                        else if (key == "emailid")
                        {
                            CR.emailid = string.Format("{0}: {1}", key, val);
                        }
                        else if (key == "mobile")
                        {
                            CR.mobile = string.Format("{0}: {1}", key, val);
                        }
                        else if (key == "msg")
                        {
                            CR.msg = string.Format("{0}: {1}", key, val);
                        }
                    }
                }
                string body = utlity.WebCareer(CR.name, CR.emailid, CR.mobile, CR.msg);
                string recp = "hr@inwinteck.com";
                string subject = "Website Career";
                string res = utlity.sendmailCareer(recp, subject, body, CR.emailid, CR.resume, CR.cover);

                if (res == "true")
                {
                    var response = Request.CreateResponse(HttpStatusCode.RedirectMethod);
                    response.Headers.Location = new Uri("https://inwinteck.com/thank-you.html?res=2");
                    Website_Carrer_Enq we = new Website_Carrer_Enq();

                    we.Cover_Letter = CR.cover;
                    we.Name = CR.name;
                    we.Phone = CR.mobile;
                    we.Resume = CR.resume;
                    we.Email = CR.emailid;
                    we.Enq_Date = DateTime.Now;

                    db1.WCE.Add(we);
                    db1.SaveChanges();

                    return response;

                



                }
                else
                {
                    var response = Request.CreateResponse(HttpStatusCode.RedirectMethod);
                    response.Headers.Location = new Uri("https://inwinteck.com/error.html");
                    return response;
                }
            }
            catch (System.Exception e)
            {
                var response = Request.CreateResponse(HttpStatusCode.RedirectMethod);
                response.Headers.Location = new Uri("https://inwinteck.com/error.html");
                return response;
            }

        }



    }
}
