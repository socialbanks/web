﻿using System;
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




    }
}
