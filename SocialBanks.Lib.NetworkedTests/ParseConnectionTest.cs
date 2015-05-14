using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parse;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace SocialBanks.Lib.NetworkedTests
{
    [TestClass]
    public class ParseConnectionTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var pathBits = path.Split('\\');
            path = string.Join("\\", pathBits, 0, (pathBits.Length - 3));
            path += "\\SocialBanksWeb\\keys.txt";

            var keys = System.IO.File.ReadAllLines(path);
            ParseClient.Initialize(keys[0], keys[1]);

            //avoid "Parse.ParseException: invalid session token"
            var task = ParseUser.LogInAsync("fabriciomatos", "123456");
            task.Wait();
        }

        [TestMethod]
        public void GetBrazilCountryByNameWorks()
        {

            var query = from country in ParseObject.GetQuery("Country")
                        where country.Get<string>("name") == "Brazil"
                        select country;

            Task<ParseObject> task = query.FirstAsync();
            task.Wait();
            ParseObject obj = task.Result;

            Assert.AreNotEqual("", obj.ObjectId);
            Assert.AreEqual("Brazil", obj.Get<string>("name"));
        }

        [TestMethod]
        public void FindAllCountriesStartingWithB()
        {

            var query = from country in ParseObject.GetQuery("Country")
                        where country.Get<string>("name").StartsWith("B")
                        select country;

            Task<IEnumerable<ParseObject>> task = query.FindAsync();
            task.Wait();
            IEnumerable<ParseObject> results = task.Result;

            Assert.AreEqual(20, results.ToList().Count);
            Assert.AreEqual("Bahamas", results.ToList().First<ParseObject>().Get<string>("name"));
        }

        [TestMethod]
        public void LoadDataForTest()
        {
            //Uncomment to delete and re-insert data for test
            //DeleteAllDataForTest();

            LoadSocialBanksForTest();
        }

        [TestMethod]
        private int ClassRecordCount(string className)
        {
            var query = ParseObject.GetQuery(className);
            var task = query.CountAsync();
            task.Wait();

            return task.Result;
        }

        private void LoadSocialBanksForTest()
        {
            if (ClassRecordCount("SocialBank") > 0)
                return;

            var rand = new Random();

            var quantity = 20;
            for (int i = 1; i <= quantity; i++)
            {
                ParseObject socialBank = new ParseObject("SocialBank");

                socialBank["name"] = "Social Bank Test #" + i.ToString("D3");
                socialBank["description"] = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
                if (false)
                {
                    ParseFile file = new ParseFile("socialbanks.jpeg", getImageBytes());
                    var taskFile = file.SaveAsync();
                    taskFile.Wait();
                    socialBank["image"] = file;
                }

                socialBank["address"] = "Rua Fulano de Tal, N 725, Itarare, Vitoria-ES, Brazil";
                socialBank["zipcode"] = "29000-000";
                socialBank["socialMoneyName"] = "MOEDA" + i.ToString("D3");
                socialBank["bitcoinAddressForDonation"] = "1FTuKcjGUrMWatFyt8i1RbmRzkY2V9TDMG";
                socialBank["bitcoinAddressForSocialMoneyIssuance"] = "1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK";
                socialBank["totalBitcoinWithdrawals"] = ((float)rand.Next(1, 10000)) / 1000.0f;
                socialBank["totalBitcoins"] = ((float)rand.Next(10, 200000)) / 1000.0f;

                var totalIssuedSocialMoney = rand.Next(1, 20) * 1000;
                socialBank["totalIssuedSocialMoney"] = totalIssuedSocialMoney;
                socialBank["totalActiveSocialMoney"] = totalIssuedSocialMoney * 0.87;
                socialBank["onlineSocialMoneyBalance"] = totalIssuedSocialMoney * 0.19;
                socialBank["approved"] = (i != quantity); //set the last one to false

                var task = socialBank.SaveAsync();
                task.Wait();
            }

            //Task.WaitAll();
        }

        private static byte[] getImageBytes()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var pathBits = path.Split('\\');
            path = string.Join("\\", pathBits, 0, (pathBits.Length - 3));
            path += "\\UnitTests\\socialbanks.jpeg";
            byte[] data = System.IO.File.ReadAllBytes(path);
            return data;
        }

        private void DeleteAllDataForTest()
        {
            DeleteAllParseRecords("SocialBank");
        }

        private void DeleteAllParseRecords(string className)
        {
            var query = ParseObject.GetQuery(className);
            var taskContinue = query.FindAsync().ContinueWith(t =>
            {
                IEnumerable<ParseObject> results = t.Result;
                foreach (var obj in results)
                {
                    var task = obj.DeleteAsync();
                    task.Wait();
                }
            });

            //Task.WaitAll();

        }

    }
}
