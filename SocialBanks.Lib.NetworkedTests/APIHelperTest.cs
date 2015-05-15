using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialBanks.Lib;

namespace SocialBanks.Lib.NetworkedTests
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

            ObjectUnderTest = new APIHelper();
            ObjectUnderTest.Initialize(path);

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


        [Todo("also bring bitcoin")]
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

        [Todo("also bring bitcoin")]
        [TestMethod]
        public void get_credits()
        {
            var q = ObjectUnderTest.get_credits("1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK");
            q.Wait();
            var result = q.Result.Result;

            Assert.AreEqual(5, result.Count);

            //BRAZUCA
            Assert.AreEqual("1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK", result[0].Address);
            Assert.AreEqual("XCP", result[0].Name);
            Assert.AreEqual((long)88500000, result[0].Quantity);

        }

        [Todo("also bring bitcoin")]
        [TestMethod, Ignore]
        public void get_debits()
        {
            var q = ObjectUnderTest.get_debits("1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK");
            q.Wait();
            var result = q.Result.Result;

            Assert.AreEqual(5, result.Count);

            //BRAZUCA
            Assert.AreEqual("1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK", result[0].Address);
            Assert.AreEqual("XCP", result[0].Name);
            Assert.AreEqual((long)88500000, result[0].Quantity);

        }

        [TestMethod]
        public void SendSocialMoneyFromNotOwnedWallet()
        {
            decimal value = 10.15m;
            var q = ObjectUnderTest.send_social_money("BRAZUCA", "addr1", "addr3", (int)Math.Truncate(value * 100), "Wallmart", "tx_id", "tx....");

            try
            {
                q.Wait();
            }
            catch (System.AggregateException e)
            {
                Assert.IsInstanceOfType(e.InnerException, typeof(ParseException));
                Assert.AreEqual("User tried to access another user wallet", e.InnerException.Message);
                return; 
            }
            catch (Exception e)
            {
                throw e;
            }
            Assert.Fail("Exeption expected");
        }

        [TestMethod]
        public void SendSocialMoneyFromWalletWithoutSufficientBalance()
        {
            decimal value = 1000m;
            var q = ObjectUnderTest.send_social_money("BRAZUCA", "addrF", "addr3", (int)Math.Truncate(value * 100), "Wallmart", "tx_id", "tx....");

            try
            {
                q.Wait();
            }
            catch (System.AggregateException e)
            {
                Assert.IsInstanceOfType(e.InnerException, typeof(ParseException));
                Assert.AreEqual("Wallet don't have sufficient balance to withdraw 100000", e.InnerException.Message);
                return;
            }
            catch (Exception e)
            {
                throw e;
            }
            Assert.Fail("Exeption expected");
        }


        [TestMethod]
        public void SendSocialMoneyToAnotherBankWallet()
        {
            decimal value = 1m;
            var q = ObjectUnderTest.send_social_money("BRAZUCA", "addrF", "addr2", (int)Math.Truncate(value * 100), "Wallmart", "tx_id", "tx....");

            try
            {
                q.Wait();
            }
            catch (System.AggregateException e)
            {
                Assert.IsInstanceOfType(e.InnerException, typeof(ParseException));
                Assert.AreEqual("Receiver wallet isn't of the same social bank currency", e.InnerException.Message);
                return;
            }
            catch (Exception e)
            {
                throw e;
            }
            Assert.Fail("Exeption expected");
        }


        public int GetWalletBalance(string address)
        {

            var query = from wallet in ParseObject.GetQuery("Wallet")
                        where wallet.Get<string>("bitcoinAddress") == address
                        select wallet;
            var q = query.FindAsync();
            q.Wait();


            ParseObject obj = q.Result.First<ParseObject>();

            return obj.Get<int>("balance");
        }

        [TestMethod]
        public void WalletIsReadOnly()
        {

            var query = from wallet in ParseObject.GetQuery("Wallet")
                        where wallet.Get<string>("bitcoinAddress") == "addrF"
                        select wallet;
            var q = query.FindAsync();
            q.Wait();


            ParseObject obj = q.Result.First<ParseObject>();

            obj["balance"] = obj.Get<int>("balance") + 1000;

            var t = obj.SaveAsync();

            try
            {
                t.Wait();
            }
            catch (Exception e)
            {
                Assert.AreEqual("This user is not allowed to perform the update operation on Wallet. You can change this setting in the Data Browser.", e.InnerException.Message);
                return;
            }
            Assert.Fail("Exception Expected");
        }

        [TestMethod]
        public void SendSocialMoneyWorks()
        {
            var senderOriginalBalance = GetWalletBalance("addrF");
            var receiverOriginalBalance = GetWalletBalance("addr1");


            decimal value = 0.01m;
            int valueInCents = (int)Math.Truncate(value * 100);
            var q = ObjectUnderTest.send_social_money("BRAZUCA", "addrF", "addr1", valueInCents, "Wallmart", "tx_id", "tx....");
            q.Wait();
            var result = q.Result.Result;

            var senderBalance = GetWalletBalance("addrF");
            var receiverBalance = GetWalletBalance("addr1");

            Assert.AreEqual(senderBalance, senderOriginalBalance - valueInCents, "Sender balance");
            Assert.AreEqual(receiverBalance, receiverOriginalBalance + valueInCents, "Receiver balance");
            //Assert.AreEqual("", result);
        }

    }
}
