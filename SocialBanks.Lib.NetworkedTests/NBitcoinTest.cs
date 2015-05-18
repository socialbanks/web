﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBitcoin;
using NBitcoin.DataEncoders;
using NBitcoin.Protocol;
using NBitcoin.RPC;
using Parse;
using SocialBanks.Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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

        [TestMethod]
        public void CreateAndSignTransaction2()
        {

            //chave privada da 1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK (counterparty)
            var privKey = Key.Parse("L2BkJmqFfEuDiaGxcTmA8vrrZnvoP523SMrZKzB8seHjKPwYX8Df");
            var pubKeyHash = privKey.PubKey.Hash.ToString();

            var d = new Dictionary<string, object>();
            d["source"] = "1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK";
            d["quantity"] = 100000000;
            d["asset"] = "BRAZUCA";
            d["description"] = "BRAZUCA";
            //d["pubkey"] = pubKeyHash;

            var t = ParseCloud.CallFunctionAsync<Dictionary<string, object>>("create_issuance", d);
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

        [TestMethod]
        public void CreateAndSignTransaction3()
        {

            //chave privada da 1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK (counterparty)
            var privKey = Key.Parse("L2BkJmqFfEuDiaGxcTmA8vrrZnvoP523SMrZKzB8seHjKPwYX8Df");
            var pubKeyHash = privKey.PubKey.Hash.ToString();

            var d = new Dictionary<string, object>();
            d["source"] = "1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK";
            d["quantity"] = (long)10000 * (long)100000000;
            d["asset"] = "MOQUECA";
            d["description"] = "MOQUECA";
            //d["pubkey"] = pubKeyHash;

            var t = ParseCloud.CallFunctionAsync<Dictionary<string, object>>("create_issuance", d);
            t.Wait();

            var result = (Dictionary<string, object>)t.Result;

            if (result.Keys.Contains("error"))
            {
                var error = result["error"] as Dictionary<string, object>;
                Assert.AreEqual(0, error.Keys.Count);
            }

            Assert.AreEqual(3, result.Count);

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

            var a = 1;
            //var txBuilder = new TransactionBuilder();
            //Assert.IsTrue(txBuilder.Verify(trans));    

        }

        [TestMethod]
        public void CreateMultiSig()
        {
            //Only SocialBanks know this privKey
            var privKeyServer = Key.Parse("KwPGv91ZJUB3UShXBWAZAzBXjYCkMgpoXbryW3dwW3B66pWivMRE", Network.Main);
            var strPubKey = privKeyServer.PubKey.ToHex(); //0213cc3e8aa13da9fdced6ac55737984b71a0ea6a9c1817cc15f687163813e44c8

            ////////////////////////////////////////ok
            //Client-side part (mobile wallet)
            ////////////////////////////////////////

            var pubKeyServer = new PubKey("0213cc3e8aa13da9fdced6ac55737984b71a0ea6a9c1817cc15f687163813e44c8");
            var addressServer = pubKeyServer.GetAddress(Network.Main); // => 14pkzzJbAg1N3EFkEnc4o5uHQJAzCqUUFJ

            var privKeyClient1 = Key.Parse("KxyACdWtFEY6p2nAbSAZv9NXgmJNm4i6HDUjgoy1YtVFTskV75KX");
            var pubKeyClient1 = privKeyClient1.PubKey;
            var AddressClient1 = privKeyClient1.PubKey.GetAddress(Network.Main); // => AWXoDzdqqSbf3Fo7yKozXX2aP9nvmsVse

            var addrFabricioWallet = new BitcoinAddress("1FTuKcjGUrMWatFyt8i1RbmRzkY2V9TDMG");

            string rawTx;

            {
                //{2 0213cc3e8aa13da9fdced6ac55737984b71a0ea6a9c1817cc15f687163813e44c8 03d4e7ffa6ebedc601a5e9ca48b9d9110bef80c15ce45039a08a513801712579de 2 OP_CHECKMULTISIG}
                Script client1P2SHScript = 
                    PayToMultiSigTemplate.Instance.GenerateScriptPubKey(2, new[] { pubKeyServer, pubKeyClient1 });
                var client1P2SHAddress = client1P2SHScript.GetScriptAddress(Network.Main);// => 3Qx7v3AQshdKGCqu81QYtkQFDwHKDqaNBi	



                /*
                    https://blockchain.info/pt/unspent?active=3Qx7v3AQshdKGCqu81QYtkQFDwHKDqaNBi
                    "unspent_outputs":[
	
                        {
                            "tx_hash":"dd4117eab5a18cc1c1d3580822faf632f4bcec1fc079b935ef4ea1958b37cfb6",
                        ==> "tx_hash_big_endian":"b6cf378b95a14eef35b979c01fecbcf432f6fa220858d3c1c18ca1b5ea1741dd",
                            "tx_index":87264463,
                            "tx_output_n": 0,
                            "script":"a914ff26223bbaa71dbaec1693059c1feb5d1e14b8f487",
                            "value": 1000000,
                            "value_hex": "0f4240",
                            "confirmations":35
                        }	  
                    ]
                */
                //We should be able to offer the "b6cf378b95a14eef35b979c01fecbcf432f6fa220858d3c1c18ca1b5ea1741dd" unspend transaction.
                var txHash = new uint256("b6cf378b95a14eef35b979c01fecbcf432f6fa220858d3c1c18ca1b5ea1741dd");

                Transaction client1P2SH = new Transaction()
                {
                    Outputs = { new TxOut("0.00101000", client1P2SHAddress) }
                };

                //Coin array = new transaction input array
                Coin[] client1CoinsP2SH = client1P2SH
                    .Outputs
                    //.Select((outp, i) => new ScriptCoin(new OutPoint(client1P2SH.GetHash(), i), outp, client1P2SHScript))
                    .Select((outp, i) => new ScriptCoin(new OutPoint(txHash, i), outp, client1P2SHScript))
                    .ToArray();

                var txBuilder = new TransactionBuilder();

                var tx = txBuilder
                        .AddCoins(client1CoinsP2SH)
                        .AddKeys(privKeyClient1)
                        .AddKnownRedeems(client1P2SHScript)
                        .Send(addrFabricioWallet, "0.001")
                        .SetChange(client1P2SHAddress)
                        .BuildTransaction(true);
                       
                tx = txBuilder.AddKeys(privKeyClient1).SignTransaction(tx);

                var rawTx1 = tx.ToHex();
                tx.Sign(privKeyClient1, true);

                Assert.IsTrue(!txBuilder.Verify(tx)); //Well, only one signature on the two required...

                //emulate send tx to the api
                rawTx = tx.ToHex();
            }
            /*
                        0000483045022100a17271d87dc1ab36ebf9aa449cd1daae33aa4ad44b55f4a661b1a01e90b6411002200384b19d8246f8cdb8f5d7ac04a1e25730023dc912d57b1c9a8c70eb587787c8014752210213cc3e8aa13da9fdced6ac55737984b71a0ea6a9c1817cc15f687163813e44c82103d4e7ffa6ebedc601a5e9ca48b9d9110bef80c15ce45039a08a513801712579de52ae
                          00483045022100a17271d87dc1ab36ebf9aa449cd1daae33aa4ad44b55f4a661b1a01e90b6411002200384b19d8246f8cdb8f5d7ac04a1e25730023dc912d57b1c9a8c70eb587787c8014752210213cc3e8aa13da9fdced6ac55737984b71a0ea6a9c1817cc15f687163813e44c82103d4e7ffa6ebedc601a5e9ca48b9d9110bef80c15ce45039a08a513801712579de52ae
  
             */
            ////////////////////////////////////////
            //Server-side part (socialbanks api)
            ////////////////////////////////////////
            {

                var txClient = new Transaction(rawTx);

                var txBuilder = new TransactionBuilder();
                var tx = txBuilder
                        .AddKeys(privKeyServer)
                        .SignTransaction(txClient);

                tx.Sign(privKeyServer, true);

                Assert.AreNotEqual(rawTx, tx.ToHex());
                //Assert.IsTrue(txBuilder.Verify(tx));
            
            }

            //Input should be tx "b6cf378b95a14eef35b979c01fecbcf432f6fa220858d3c1c18ca1b5ea1741dd"
            //But is             "71b1f9104951d81a8016e78e5804dea7acc419af93712b0bfa059b0439192eb6"

            //Signed Transaction:
            //0100000001dd4117eab5a18cc1c1d3580822faf632f4bcec1fc079b935ef4ea1958b37cfb600000000910047304402207e7d55441fc23843863a50925235c1a3e7a8a311a379e052e04ec8c37f58eaab02204dda0f748e0b44b9b5e9c11ca74f63911e7e417a696c9d570b979808029841d5014752210213cc3e8aa13da9fdced6ac55737984b71a0ea6a9c1817cc15f687163813e44c82103d4e7ffa6ebedc601a5e9ca48b9d9110bef80c15ce45039a08a513801712579de52aeffffffff02e80300000000000017a914ff26223bbaa71dbaec1693059c1feb5d1e14b8f487a0860100000000001976a9149ea84056a5a9e294d93f11300be51d51868da69388ac00000000

        }


        [TestMethod]
        public void GetUnspendTransactions()
        {
            var outputs = this.ObjectUnderTest.FindUnspendTransactions("3Qx7v3AQshdKGCqu81QYtkQFDwHKDqaNBi");
            Assert.AreEqual(1, outputs.Count);

            //var json = HttpGet("https://blockchain.info/pt/unspent?active=3Qx7v3AQshdKGCqu81QYtkQFDwHKDqaNBi");
            //Assert.AreEqual("", json);
        }


        public byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

    }

  

}
