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

            var keysFilePath = "./keys.txt";
            var apiHelper = new APIHelper();

            Console.WriteLine("  + Connecting to SocialBanks API...");
            apiHelper.Initialize(keysFilePath);

            var bitcoinHelper = new BitcoinHelper();

            var task = apiHelper.FindIncompleteTransactions();
            task.Wait();

            //https://getaddr.bitnodes.io/nodes/?q=/Satoshi:0.10.0/a
            var credential = new NetworkCredential("username", "password");
            var host = "123.56.129.45";
            Console.WriteLine("  + Connecting to the Bitcoin Peer: " + host);
            var rpcClient = new RPCClient(credential, host, Network.Main);

            var transaction = task.Result.ElementAt(0);
            //foreach (var transaction in task.Result)
            {
                Console.WriteLine("[2] Processando transaction " + transaction.ObjectId);

                var keys = System.IO.File.ReadAllLines(keysFilePath);
                var wifServer = keys[2];

                var senderWallet = transaction.Get<ParseObject>("senderWallet");
                var wifClient = senderWallet.Get<string>("wif_remove");

                var receiverAddress = transaction.Get<string>("receiverAddress");
                var valueInSatoshis = transaction.Get<long>("value") * 1000;

                try
                {

                    Console.WriteLine("  + Signing Bitcoin Transaction...");
                    var dtoSignedTrans = bitcoinHelper.CreateAndSignP2SHTransaction(wifServer, wifClient, receiverAddress, valueInSatoshis);

                    Console.WriteLine("  + Broadcasting Bitcoin Transaction " + dtoSignedTrans.TxHash);
                    //rpcClient.SendRawTransaction(new Transaction(dtoSignedTrans.RawTx));

                    Console.WriteLine("==================================");
                    Console.WriteLine(dtoSignedTrans.RawTx);
                    Console.WriteLine("==================================");

                    transaction["bitcoinTransfered"] = true;
                    transaction["bitcoinTx"] = dtoSignedTrans.TxHash;

                    Console.WriteLine("  + Updating Transaction ParseObject");
                    //var taskSave = transaction.SaveAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }

            }

            Task.WaitAll();

            Console.WriteLine("[Press any key]");
            Console.ReadKey();
        }
    }
}


