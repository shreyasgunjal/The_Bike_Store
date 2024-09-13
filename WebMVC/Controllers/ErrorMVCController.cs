using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMVC.Controllers
{
    public class ErrorMVCController : Controller
    {
        // GET: ErrorMVC
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PageNotFound() 
        {
            return View();
        }
    }
}