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
        public JsonResult PostSocialBank()
        {
            var f = Request.Form;

            var o = new ParseObject("SocialBank");

            foreach (var k in f.AllKeys)
            {
                o[k] = f[k];
            }

            o.SaveAsync().Wait();

            return Json("success");
        }

        [HttpPost]
        public JsonResult PostUser()
        {
            var f = Request.Form;

            var o = new ParseUser();

            foreach (var k in f.AllKeys)
            {
                o[k] = f[k];
            }

            o["username"] = o["email"];
            o.SignUpAsync().Wait();

            return Json("success");
        }


    }
}
