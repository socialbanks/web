using Parse;
using SocialBanks.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Script.Serialization;

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

        public async Task<ActionResult> Test_hello()
        {

            ViewBag.Message = await APIHelper.hello();

            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> create_issuance()
        {
            var r = await APIHelper.create_issuance("1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK", "BRAZUCA", 10, "desc210");
            return Json(r,JsonRequestBehavior.AllowGet);
        }



        private static async Task<ParseObject> GetBrazil_Test()
        {
            var query = from country in ParseObject.GetQuery("Country")
                        where country.Get<string>("name") == "Brazil"
                        select country;

            ParseObject obj = await query.FirstAsync();
            return obj;
        }

        private static async Task<ParseUser> RegisterNewSocialBankWithUsers()
        {
            //DeleteTestUser("fabriciomatos");

            var user = new ParseUser()
            {
                Username = "fabriciomatos",
                Password = "123456",
                Email = "fabricio@qualidata.com.br"
            };

            //user["firstName"] = "Fabricio";
            //user["lastName"] = "Vargas Matos";
            //user["primaryBitcoinAddress"] = "1FTuKcjGUrMWatFyt8i1RbmRzkY2V9TDMG";
            //user["isSocialBankOfficer"] = true;

            await user.SignUpAsync();

            return user;

        }

        public async Task<ActionResult> Test_get_balances()
        {
            var resultList = new List<object[]>();

            await Test(resultList, "1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK");
            await Test(resultList, "1BdHqBSfUqv77XtBSeofH6XwHHczZxKRUF");
            await Test(resultList, "1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK", "1BdHqBSfUqv77XtBSeofH6XwHHczZxKRUF", "1sEAUJsjuYJ9P64Y2MxchwyDfw8hbQDNA");


            ViewBag.ItemTest = resultList;
            return View();
        }

        private async Task Test(List<object[]> resultList, params string[] addresses)
        {
            var o = await APIHelper.get_balances(addresses);
            var oJS = new JavaScriptSerializer();
            var result = oJS.Serialize(o);

            var it = new object[]
            {
                addresses,
                result,
            };
            resultList.Add(it);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
