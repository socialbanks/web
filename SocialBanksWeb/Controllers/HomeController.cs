using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SocialBanksWeb.Models;
using Parse;
using SocialBanks.Lib;
using System.Web.Hosting;

namespace SocialBanksWeb.Controllers
{
    public class HomeController : Controller
    {
        APIHelper APIHelper;
        public HomeController()
        {
            var keysFilePath = HostingEnvironment.MapPath("~/keys.txt");
            APIHelper = new APIHelper();
            APIHelper.Initialize(keysFilePath);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateSocialBank()
        {
            return View();
        }

        [HttpPost]
        public JsonResult PostCreateSocialBank()
        {
            var f = Request.Form;

            var socialBank = new ParseObject("SocialBank");

            foreach (var k in f.AllKeys)
            {
                socialBank[k] = f[k];
            }

            socialBank.SaveAsync().Wait();

            return Json("success");
        }


    }
}
