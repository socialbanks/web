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
        public JsonResult CreateSocialBank(SocialBankModel sb)
        {
            var socialBank = new ParseObject("SocialBank");

            socialBank["name"] = "Social Bank Test ";
            socialBank["socialMoneyName"] = "MOEDA";
            socialBank.SaveAsync().Wait();

            return Json("success");
        }


    }
}
