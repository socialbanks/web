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

        private static ParseUser CurrentUser;

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
            CurrentUser = task.Result;
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
            var valueInCents = 1;
            var addrA = "addr1";
            var addrB = "addrF";

            try
            {
                AddNewTransaction(valueInCents, addrA, addrB);
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
            var valueInCents = 1000000;
            var addrA = "addrF";
            var addrB = "addr1";

            try
            {
                AddNewTransaction(valueInCents, addrA, addrB);
            }
            catch (System.AggregateException e)
            {
                Assert.IsInstanceOfType(e.InnerException, typeof(ParseException));
                Assert.AreEqual("Wallet don't have sufficient balance to withdraw 1000000", e.InnerException.Message);
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
            var valueInCents = 1;
            var addrA = "addrF";
            var addrB = "addr2";

            try
            {
                AddNewTransaction(valueInCents, addrA, addrB);
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
        public void TransactionShouldUpdateBalances()
        {
            var valueInCents = 1;
            var addrA = "addrF";
            var addrB = "addr1";

            var senderOriginalBalance = GetWalletBalance(addrA);
            var receiverOriginalBalance = GetWalletBalance(addrB);

            AddNewTransaction(valueInCents, addrA, addrB);

            var senderBalance = GetWalletBalance(addrA);
            var receiverBalance = GetWalletBalance(addrB);

            Assert.AreEqual(senderBalance, senderOriginalBalance - valueInCents, "Sender balance");
            Assert.AreEqual(receiverBalance, receiverOriginalBalance + valueInCents, "Receiver balance");
        }

        private void AddNewTransaction(int valueInCents, string addrA, string addrB)
        {
            var senderWallet = GetWalletByAddress(addrA);
            var receiverWallet = GetWalletByAddress(addrB);

            ParseObject tran = new ParseObject("Transaction");
            tran["value"] = valueInCents;
            tran["senderDescription"] = "Wallmart Test";
            tran["senderWallet"] = senderWallet;
            tran["receiverWallet"] = receiverWallet;
            tran["user"] = CurrentUser;

            var q = tran.SaveAsync();
            q.Wait();
        }

        public ParseObject GetWalletByAddress(string address)
        {

            var query = from wallet in ParseObject.GetQuery("Wallet")
                        where wallet.Get<string>("bitcoinAddress") == address
                        select wallet;
            var q = query.FindAsync();
            q.Wait();


            return q.Result.First<ParseObject>();
        }

        public int GetWalletBalance(string address)
        {
            var wallet = GetWalletByAddress(address);

            return wallet.Get<int>("balance");
        }
    }
}
