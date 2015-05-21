using NBitcoin;
using NBitcoin.RPC;
using Parse;
using SocialBanks.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebJobBitcoinNetworkMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[1] Initializing WebJobBitcoinNetworkMonitor...");

            var keysFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\keys.txt";
            var apiHelper = new APIHelper();

            Console.WriteLine("  + Connecting to SocialBanks API...");
            apiHelper.Initialize(keysFilePath);

            var bitcoinHelper = new BitcoinHelper();

            var task = apiHelper.FindIncompleteTransactions();
            task.Wait();

            //var rpcClient = ConnectToBitcoinNetwork();

            //var transaction = task.Result.Where((t) => t.ObjectId == "qf36svQQ2o").First<ParseObject>();
            foreach (var transaction in task.Result)
            {
                try
                {
                    SignAndBroadCastTransaction(keysFilePath, bitcoinHelper, transaction, apiHelper);
                }
                catch (Exception e)
                {
                    var errorMessage = string.Format("ERROR: Couldn't sign the transaction. Message: {0}", e.Message);

                    Console.WriteLine(errorMessage);

                    transaction["broadcastStatus"] = "error";
                    transaction["broadcastLog"] = errorMessage;

                    var taskSave = transaction.SaveAsync();
                    taskSave.Wait();
                }

                try
                {
                    Console.WriteLine("  + Updating Transaction ParseObject");
                    var taskSave = transaction.SaveAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: Couldn't save the Transaction object. Message: " + e.Message);
                }

            }

            Task.WaitAll();

            //Console.WriteLine("[Press any key]");
            //Console.ReadKey();
        }

        private static void SignAndBroadCastTransaction(string keysFilePath, BitcoinHelper bitcoinHelper, ParseObject transaction, APIHelper apiHelper)
        {
            Console.WriteLine("[2] Processando transaction " + transaction.ObjectId);

            ParseObject senderWallet;
            ParseObject socialBank;
            GetSocialBank(transaction, out senderWallet, out socialBank);

            var receiverWallet = GetReceiverWallet(transaction);

            var keys = System.IO.File.ReadAllLines(keysFilePath);
            var password = keys[2];

            var wifServerEncrypted = socialBank.Get<string>("encryptedWIF");
            var wifClient = senderWallet.Get<string>("wif_remove");
            var receiverAddress = receiverWallet.Get<string>("correct_bitcoinAddress");
            var valueInSatoshis = transaction.Get<long>("value") * 1000;

            DtoSignedTransaction dtoSignedTrans;

            Console.WriteLine("  + Signing Bitcoin Transaction...");
            dtoSignedTrans = bitcoinHelper.CreateAndSignP2SHTransaction(wifServerEncrypted, password, wifClient, receiverAddress, valueInSatoshis);

            Console.WriteLine("==================================");
            Console.WriteLine(dtoSignedTrans.RawTx);
            Console.WriteLine("==================================");

            Console.WriteLine("  + Sending transaction data do Parse.com ...");
            var parseBitcoinTransaction = new ParseObject("BitcoinTransaction");
            parseBitcoinTransaction["hash"] = dtoSignedTrans.TxHash;
            parseBitcoinTransaction["rawData"] = dtoSignedTrans.RawTx;
            parseBitcoinTransaction["confirmed"] = false;
            parseBitcoinTransaction["broadcasted"] = false;
            {
                var taskSave = parseBitcoinTransaction.SaveAsync();
                taskSave.Wait();

                transaction["bitcoinTransaction"] = parseBitcoinTransaction;
            }

            try
            {
                Console.WriteLine("  + Broadcasting Bitcoin Transaction " + dtoSignedTrans.TxHash);
                
                //rpcClient.SendRawTransaction(new Transaction(dtoSignedTrans.RawTx));
                //rpcClient.SendRawTransaction(Hex2ByteArray(dtoSignedTrans.RawTx));
                Tuple<bool, string> result = apiHelper.BroadcastTransaction(dtoSignedTrans.RawTx);

                parseBitcoinTransaction["broadcasted"] = result.Item1;
                if (!result.Item1)
                {
                    parseBitcoinTransaction["errorLog"] = result.Item2;
                }

                {
                    var taskSave = parseBitcoinTransaction.SaveAsync();
                    taskSave.Wait();
                };

                transaction["broadcastStatus"] = "processed";

            }
            catch (Exception e)
            {
                var errorMessage = string.Format("ERROR: Couldn't broadcast transaction. Message: {0}", e.Message);

                transaction["broadcastStatus"] = "error";
                transaction["broadcastLog"] = errorMessage;

                Console.WriteLine(errorMessage);
            }
        }

        private static RPCClient ConnectToBitcoinNetwork()
        {
            //https://getaddr.bitnodes.io/nodes/?q=/Satoshi:0.10.0/a
            var credential = new NetworkCredential("username", "password");
            var host = "123.56.129.45";
            Console.WriteLine("  + Connecting to the Bitcoin Peer: " + host);
            var rpcClient = new RPCClient(credential, host, Network.Main);
            return rpcClient;
        }

        private static ParseObject GetReceiverWallet(ParseObject transaction)
        {
            var receiverWallet = transaction.Get<ParseObject>("receiverWallet");
            var taskRW = ParseObject.GetQuery("Wallet").GetAsync(transaction.Get<ParseObject>("receiverWallet").ObjectId);
            taskRW.Wait();
            receiverWallet = taskRW.Result;
            return receiverWallet;
        }

        private static void GetSocialBank(ParseObject transaction, out ParseObject senderWallet, out ParseObject socialBank)
        {
            senderWallet = transaction.Get<ParseObject>("senderWallet");
            var taskSB = ParseObject.GetQuery("SocialBank").GetAsync(senderWallet.Get<ParseObject>("socialBank").ObjectId);
            taskSB.Wait();
            socialBank = taskSB.Result;
        }

        private static byte[] Hex2ByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}


