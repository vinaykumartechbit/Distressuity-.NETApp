using DISTRESSUITY.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DISTRESSUITY.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View("/App/Views/Shared/Index.cshtml");
        }
    }
}
