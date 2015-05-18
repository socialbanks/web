using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBitcoin;
using NBitcoin.DataEncoders;
using Parse;
using SocialBanks.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialBanksLib.NetworkedTests
{

    [TestClass]
    public class NBitcoinTest
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
        public void SignTransaction()
        {

            //chave privada da 1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK (counterparty)
            var privKey = Key.Parse("L2BkJmqFfEuDiaGxcTmA8vrrZnvoP523SMrZKzB8seHjKPwYX8Df");
            var pubKeyHash = privKey.PubKey.Hash.ToString();

            var rawTrans = "01000000012dce3e102e770b30aa0ecddc99fd1cbdcb8fd9a899e3d3083fc9b76019797f66030000001976a914ce27246a0a6ca54dfa1f780ccd5cb3d0c73a75b288acffffffff0436150000000000001976a914748e483222863a836a421df1a9395bbd835bdfda88ac36150000000000001976a9142824a385d37205caf61c8cffc6b0c95d27594d5e88ac36150000000000001976a9142a24a385d37205caf6508cffc6b0cca8c64737c388ac46e17800000000001976a914ce27246a0a6ca54dfa1f780ccd5cb3d0c73a75b288ac00000000";
            //var rawTrans = "0100000002b6265db14210788b0185b8b1911ffccf3b9752a223a83f1a7d1e0cd31693eef8000000001976a914c18af8d3c8b50db89677366477f20c0a2be5493288acffffffff29687a3cf0618d9dc229d11fa30f3d819523b0f0ba07361905043f53b23d5c02000000001976a914c18af8d3c8b50db89677366477f20c0a2be5493288acffffffff0436150000000000001976a914ce27246a0a6ca54dfa1f780ccd5cb3d0c73a75b288ac36150000000000001976a9142008b00916eae89d44f8a2d4e6d87f4adbbc553188ac36150000000000001976a9142208b00916eae89d44b4a2d4e6d82622f4a22fac88aca44f0700000000001976a914c18af8d3c8b50db89677366477f20c0a2be5493288ac00000000";
            var trans = new Transaction(rawTrans);

            for (int i = 0; i < trans.Inputs.Count; i++)
            {
                //trans.SignInput(privKey, privKey.ScriptPubKey, i);
            }



            trans.Sign(privKey, false);

            {
                var txRawHex = Encoders.Hex.EncodeData(trans.ToBytes());
                Assert.AreNotEqual(rawTrans, txRawHex);
                var a = (txRawHex);
                Console.WriteLine(a);
            }

            //var txBuilder = new TransactionBuilder();
            //txBuilder.AddKeys(privKey);//.SignTransaction(trans);
            //Assert.IsTrue(txBuilder.Verify(trans));    

        }

        [TestMethod]
        public void CreateAndSignTransaction()
        {

            //chave privada da 1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK (counterparty)
            var privKey = Key.Parse("L2BkJmqFfEuDiaGxcTmA8vrrZnvoP523SMrZKzB8seHjKPwYX8Df");
            var pubKeyHash = privKey.PubKey.Hash.ToString();

            var d = new Dictionary<string, object>();
            d["source"] = "1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK";
            d["destination"] = "1BdHqBSfUqv77XtBSeofH6XwHHczZxKRUF";
            d["quantity"] = 100000000;
            d["asset"] = "BRAZUCA";
            //d["pubkey"] = pubKeyHash;

            var t = ParseCloud.CallFunctionAsync<Dictionary<string, object>>("send", d);
            t.Wait();

            var result = (Dictionary<string, object>)t.Result;
            Assert.AreEqual(3, result.Count);

            if (result.Keys.Contains("error"))
            {
                var error = result["error"] as Dictionary<string, object>;
                Assert.AreEqual(0, error.Keys.Count);
            }

            var rawTrans = t.Result["result"].ToString();
            Assert.IsTrue(rawTrans.Length > 300);

            var trans = new Transaction(rawTrans);

            //for (int i = 0; i < trans.Inputs.Count; i++)
            //{
            //    trans.SignInput(privKey, privKey.ScriptPubKey, i);
            //}

            trans.Sign(privKey, false);

            var txRawHex = Encoders.Hex.EncodeData(trans.ToBytes());
            Console.WriteLine(txRawHex);

            Assert.AreNotEqual(rawTrans, txRawHex);


            //var txBuilder = new TransactionBuilder();
            //Assert.IsTrue(txBuilder.Verify(trans));    

        }


        public byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

    }
}
