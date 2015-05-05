using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parse;
using SocialBanksWeb.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class APIHelperTest
    {
        APIHelper ObjectUnderTest;
        [TestInitialize]
        public void TestInitialize()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var pathBits = path.Split('\\');
            path = string.Join("\\", pathBits, 0, (pathBits.Length - 3));
            path += "\\SocialBanksWeb\\keys.txt";

            ObjectUnderTest = new APIHelper(path);

            //avoid "Parse.ParseException: invalid session token"
            var task = ParseUser.LogInAsync("fabriciomatos", "123456");
            task.Wait();
        }

        [TestMethod]
        public void HelloWorld()
        {
            var v = ObjectUnderTest.hello();
            v.Wait();

            Assert.AreEqual("Hello world!", v.Result);
        }


        [TestMethod]
        public void GetBalances_Returns_2Items()
        {
            var q = ObjectUnderTest.get_balances("1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK");
            q.Wait();
            var result = q.Result.Result;

            Assert.AreEqual(2, result.Count);

            //BRAZUCA
            Assert.AreEqual("1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK", result[0].Address);
            Assert.AreEqual("BRAZUCA", result[0].Name);
            Assert.AreEqual((long)49000000000, result[0].Quantity);

            //XCP
            Assert.AreEqual("1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK", result[1].Address);
            Assert.AreEqual("XCP", result[1].Name);
            Assert.AreEqual((long)1016500000, result[1].Quantity);

            //BITCOIN
            //TODO: Retornar tambem bitcoin
        }

        [TestMethod]
        public void GetBalances_Returns_Error()
        {
            //arrange
            ObjectUnderTest.CauseError = true;

            //act
            var q = ObjectUnderTest.get_balances("1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK");
            q.Wait();
            var result = q.Result;

            Assert.AreEqual(0, result.Result.Count);
            Assert.AreEqual((long)-32601, result.Error["code"]);
            Assert.AreEqual("Method not found", result.Error["message"]);

        }



        [TestMethod]
        public void create_issuance_Returns_Error()
        {
            //arrange

            //act
            var q = ObjectUnderTest.create_issuance("", "", 0, "");
            q.Wait();
            var result = q.Result;

            Assert.AreEqual((long)-32000, result.Error["code"]);
            Assert.AreEqual("Server error", result.Error["message"]);
            var data = result.Error["data"] as Dictionary<string, object>;
            Assert.AreEqual("AddressError", data["type"]);
            Assert.AreEqual("invalid public key: ", data["message"]);
            Assert.AreEqual("invalid public key: ", (data["args"] as List<object>)[0]);

        }

        [TestMethod]
        public void create_issuance_Returns_raw_tx1()
        {
            //arrange

            //act
            var q = ObjectUnderTest.create_issuance("1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK", "BRAZUCA", 1000, "");


            q.Wait();
            var result = q.Result;

            Assert.IsTrue(result.Result.StartsWith("010000000"));

        }

        [TestMethod]
        public void create_issuance_Returns_raw_tx2()
        {
            //arrange

            //act
            var q = ObjectUnderTest.create_issuance("1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK", "BRAZUCA", 2000, "");


            q.Wait();
            var result = q.Result;

            Assert.IsTrue(result.Result.StartsWith("010000000"));

        }
    }
}
