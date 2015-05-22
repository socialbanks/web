using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SocialBanksWeb.Models;
using Parse;
using SocialBanks.Lib;
using System.Web.Hosting;
using System.Threading.Tasks;
using NBitcoin;

namespace SocialBanksWeb.Controllers
{
    public class HomeController : Controller
    {
        APIHelper APIHelper;
        string KeysFilePath;
        public HomeController()
        {
            KeysFilePath = HostingEnvironment.MapPath("~/keys.txt");
            APIHelper = new APIHelper();
            APIHelper.Initialize(KeysFilePath);
        }

        public async Task<ActionResult> Index()
        {
            var banks = await APIHelper.get_socialbanks();

            List<SocialBankModel> sbs = new List<SocialBankModel>();

            foreach (ParseObject obj in banks)
            {
                var sb = new SocialBankModel(obj);
                sbs.Add(sb);
            }

            return View(sbs);
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

        /*
        [HttpPost]
        public string sign_transaction_server(string tx)
        {
            var keys = System.IO.File.ReadAllLines(KeysFilePath);
            
            var privKeyServer = Key.Parse(keys[2], Network.Main);
            var txClient = new Transaction(tx);

            var txBuilder = new TransactionBuilder();
            var transactionToSign = txBuilder
                    .AddKeys(privKeyServer)
                    .SignTransaction(txClient);

            transactionToSign.Sign(privKeyServer, true);

            return transactionToSign.ToHex();
        }

        //It's important to send the serverWIF because anyone can invoke this method. Otherwise we expose the system security.
        //Must be HTTPS!!!!!
        [HttpPost]
        public JsonResult create_and_sign_transaction(string receiverAddr, string valueInSatoshis, string clientWIF )
        {
            var result = new DtoSignedTransaction()
            {
                RawTx = "",
                TransferedValue = 0,
                FeeValue = 0,
                Success = true,
                Message = "Running async"
            };

            try
            {

                var apiHelper = new APIHelper();
                apiHelper.set_transaction_broadcasted();

                BitcoinHelper.Log = "Controle - Ponto 1";

                var keys = System.IO.File.ReadAllLines(KeysFilePath);
                var privKeyServer = keys[2];

                BitcoinHelper.Log += " | Controle - Ponto 2";

                var btcHelper = new BitcoinHelper();

                BitcoinHelper.Log += " | Controle - Ponto 3";
                BitcoinHelper.Log += " | Params: " + privKeyServer + "," + clientWIF + "," + receiverAddr + "," + valueInSatoshis;
                
                var task = btcHelper.CreateAndSignP2SHTransactionAsync(privKeyServer, clientWIF, receiverAddr, long.Parse(valueInSatoshis));
            }
            catch (Exception e)
            {
                result = new DtoSignedTransaction()
                {
                    RawTx = "",
                    TransferedValue = 0,
                    FeeValue = 0,
                    Success = false,
                    Message = e.Message + " :: " + BitcoinHelper.Log
                };
            }


            return Json(result);
        }

        [HttpPost]
        public JsonResult create_and_sign_transaction_ORIGINAL(string receiverAddr, string valueInSatoshis, string clientWIF)
        {
            var result = new DtoSignedTransaction();

            //result = new DtoSignedTransaction()
            //{
            //    RawTx = "TESTE",
            //    TransferedValue = 0,
            //    FeeValue = 0,
            //    Success = false,
            //    Message = ""
            //};
            //return Json(result);

            try
            {
                BitcoinHelper.Log = "Controle - Ponto 1";

                var keys = System.IO.File.ReadAllLines(KeysFilePath);
                var privKeyServer = keys[2];

                BitcoinHelper.Log += " | Controle - Ponto 2";

                var btcHelper = new BitcoinHelper();

                BitcoinHelper.Log += " | Controle - Ponto 3";
                BitcoinHelper.Log += " | Params: " + privKeyServer + "," + clientWIF + "," + receiverAddr + "," + valueInSatoshis;

                result = btcHelper.CreateAndSignP2SHTransaction(privKeyServer, "<password>", clientWIF, receiverAddr, long.Parse(valueInSatoshis));
            }
            catch (Exception e)
            {
                result = new DtoSignedTransaction()
                {
                    RawTx = "",
                    TransferedValue = 0,
                    FeeValue = 0,
                    Success = false,
                    Message = e.Message + " :: " + BitcoinHelper.Log
                };
            }


            return Json(result);
        }

*/

        /*
                $.ajax({
                    url: "/home/create_and_sign_transaction",
                    method: "POST",
                    data:
                        {
                            receiverAddr: "1FTuKcjGUrMWatFyt8i1RbmRzkY2V9TDMG",
                            valueInSatoshis: 10000,
                            clientWIF: "KxyACdWtFEY6p2nAbSAZv9NXgmJNm4i6HDUjgoy1YtVFTskV75KX"
                        }
                });

         */


    }
}
