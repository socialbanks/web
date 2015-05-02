using Parse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SocialBanksWeb.Controllers
{
    public class HomeController : Controller
    {
        private const string ApplicationId_DEV = "bCOd9IKjrpxCPGYQfyagabirn7pYFjYTvJqkq1x1";
        private const string DotnetKey_DEV = "GYMOAhUQ55yYAuEehlecpipu90RFeaPSPn3zcFZ6";

        public async Task<ActionResult> Index()
        {
            ParseClient.Initialize(ApplicationId_DEV, DotnetKey_DEV);
           
            var message = "TEST: ";

            ParseObject country = await GetBrazil_Test();
            message += "; " + country.Get<string>("name");

            //ParseUser user = await RegisterNewSocialBankWithUsers();
            //message += "; " + user.Get<string>("username");


            ViewBag.Message = message;

            return View();
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

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
