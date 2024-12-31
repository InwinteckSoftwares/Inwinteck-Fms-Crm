using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inwinteck_CRM.Controllers
{
    public class FEInvoiceController : Controller
    {
        // GET: FEInvoice
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }
    }
}