using NBitcoin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialBanks.Lib
{
    //Represente the most relevant data from unspent transactions candidate to be inputed and spent by some new transaction.
    //This is used to build and sign the "imput" secction of a new transaction
    public class DtoUnspentTransaction
    {
        public string TxId { get; set; }
        public uint256 TxHash
        {
            get
            {
                return new uint256(this.TxId);
            }
        }
        public int Index { get; set; }
        public long ValueInSatoshis { get; set; }
    }

    //Represent a signed raw transaction ready to be broadcasted to the bitcoin network
    public class DtoSignedTransaction
    {
        public string RawTx { get; set; }
        public string TxHash { get; set; }
        public long TransferedValue { get; set; }
        public long FeeValue { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class BitcoinHelper
    {
        public static bool Verbose = false;

        private void IfVerboseWriteLine(string value)
        {
            //TODO: IfVerboseWriteLine should be logged to a log file
            if (Verbose)
                Console.WriteLine("[Verbose] " + value);
        }

        public static string Log = "";

        private const decimal SATOSHIS_PER_BTC = 100000000.0m;
        private const long DEFAULT_FEE = 2000; //0.00002000  ($0.02 in social money)

        //Wrapper to the /unspent GET action of blochain.info service
        //Example: https://blockchain.info/pt/unspent?active=3GYfQ22WohzvDEo9Zhigbzg4sP7BPThG92
        private async Task<List<DtoUnspentTransaction>> FindUnspentTransactionsByAddress(string address)
        {
            var result = new List<DtoUnspentTransaction>();
            var apiHelper = new APIHelper();
            
            //Invoke the apiHelper warpper wich make the httpRequest call
            Dictionary<string, object> result_unspent = await apiHelper.get_unspent(address);

            for (int i = 0; i < result_unspent.Count; i++)
            {
                //in a successful response, there is one unspent_outpts property wich is an array of unspent_output objects
                var item = result_unspent.ElementAt(i);
                if (item.Key == "unspent_outputs")
                {
                    var outputs = (item.Value as List<object>);

                    //include each unspent_output in the outputs result list, converting the Json (mapped in a Dictionary) to a native .net dto object.
                    for (int j = 0; j < outputs.Count; j++)
                    {
                        Dictionary<string, object> output = (outputs[j] as Dictionary<string, object>);

                        string tx_hash_big_endian = output["tx_hash_big_endian"].ToString();//transaction "id" (transaction hash)
                        int tx_output_n = int.Parse(output["tx_output_n"].ToString()); //index of the output in the transaction outputs
                        long value = long.Parse(output["value"].ToString()); //value unpent (in satoshis) available in this output

                        IfVerboseWriteLine("UnspentTransaction " + output["tx_output_n"].ToString());
                        IfVerboseWriteLine("   txt_output_n: " + output["tx_output_n"].ToString());
                        IfVerboseWriteLine("   value: " + output["value"].ToString());

                        result.Add(
                            new DtoUnspentTransaction()
                            {
                                TxId = tx_hash_big_endian,
                                Index = tx_output_n,
                                ValueInSatoshis = value
                            });
                    }
                }
            }

            return result;
        }

        //Invoke the blockchain to get the unspent transactions and compute the total unspent (available)
        public long GetAddressBalanceInSatoshis(string address)
        {
            var apiHelper = new APIHelper();
            
            var task = FindUnspentTransactionsByAddress(address);
            task.Wait();
            
            List<DtoUnspentTransaction> unspentTrans = task.Result;

            //sum all output's value
            long addressBalanceInSatoshis = 0;
            foreach (var trans in unspentTrans)
            {
                addressBalanceInSatoshis += trans.ValueInSatoshis;
            }
            return addressBalanceInSatoshis;
        }

        //Create and sign an S2SH transaction (mapping a MultSig Script 2-of-2), transfering the bitcoins from the multisig address to the new address.
        //IMPORTANTE: receiverAddress MUST be an ScriptAddress (P2SH)        
        //This method should receive a partial signed transaction instead of receiving the wifClient (WIF = Wallet Import Format).
        //But for now, in this proof-of-concept, we're keeping the client's private key in the server until the proper P2SH partial signature be implemented in both iOS and Android.
        public DtoSignedTransaction CreateAndSignP2SHTransaction(string wifServer, string password, string wifClient, string receiverAddress, long valueInSatoshis)
        {
            //Only the SocialBanks platform should know this privKey
            Key privKeyServer;

            if (password == "")
                privKeyServer = Key.Parse(wifServer, Network.Main); //used in test cases
            else
                privKeyServer = Key.Parse(wifServer, password, Network.Main); //production keys are stored in the database encrypted with a password

            //Calculate the SocialBank's pubKey and address
            var pubKeyServer = privKeyServer.PubKey;
            
            //////////////////////// this must be done at the client side to don't expose the clint's private key ////////////////////////////
            
            var addressServer = pubKeyServer.GetAddress(Network.Main); // Ex: 14pkzzJbAg1N3EFkEnc4o5uHQJAzCqUUFJ

            //Calculate the user's pubKey and address
            var privKeyClient1 = Key.Parse(wifClient);
            var pubKeyClient1 = privKeyClient1.PubKey;
            var AddressClient1 = privKeyClient1.PubKey.GetAddress(Network.Main);

            var receiverBtcAddress = new BitcoinScriptAddress(receiverAddress, Network.Main); //P2SH

            Script client1P2SHScript =
                PayToMultiSigTemplate.Instance.GenerateScriptPubKey(2, new[] { pubKeyServer, pubKeyClient1 });
            var client1P2SHAddress = client1P2SHScript.GetScriptAddress(Network.Main); //Ex: 3Qx7v3AQshdKGCqu81QYtkQFDwHKDqaNBi	

            var apiHelper = new APIHelper();
            var task = FindUnspentTransactionsByAddress(client1P2SHAddress.ToString());
            task.Wait();
            List<DtoUnspentTransaction> unspentTrans = task.Result;

            var selectedUnspentTrans = new List<DtoUnspentTransaction>();
            long addressBalanceInSatoshis = 0;
            foreach (var trans in unspentTrans)
            {
                selectedUnspentTrans.Add(trans);

                addressBalanceInSatoshis += trans.ValueInSatoshis;

                if (addressBalanceInSatoshis >= (valueInSatoshis + DEFAULT_FEE))
                    break;
            }

            //if balance isn't sufficient
            if (addressBalanceInSatoshis < (valueInSatoshis + DEFAULT_FEE))
            {
                throw new Exception("Insufficient funds in " + client1P2SHAddress.ToString());
            }

            //var txHash = new uint256("967f947b7f995d7f45c4ce1f6eb42baf58376d8f9ba768322d2abe858f3bd272");
            //var totalInBtc = "0.002";

            Transaction client1P2SH = new Transaction();
            bool feePayed = false;

            foreach (var unspentOutput in selectedUnspentTrans)
            {
                long valueInSatoshisWithFee = unspentOutput.ValueInSatoshis;

                if ((!feePayed) && (valueInSatoshisWithFee >= DEFAULT_FEE))
                {
                    valueInSatoshisWithFee = valueInSatoshisWithFee - DEFAULT_FEE;
                    feePayed = true;
                }


                string valueInBtc = (valueInSatoshisWithFee / SATOSHIS_PER_BTC).ToString("0.00000000", CultureInfo.CreateSpecificCulture("en-US"));
                valueInBtc = valueInBtc.Replace(',', '.');
                IfVerboseWriteLine("unspentOutput.valueInSatoshisWithFee to string valueInBtc is: " + valueInBtc);

                client1P2SH.Outputs.Add(new TxOut(valueInBtc, client1P2SHAddress));
            }

            //Coin array = new transaction input array
            Coin[] client1CoinsP2SH = client1P2SH
                .Outputs
                .Select((outp, i) => new ScriptCoin(new OutPoint(selectedUnspentTrans[i].TxHash, selectedUnspentTrans[i].Index), outp, client1P2SHScript))
                .ToArray();

            var valueInBtcStr = (valueInSatoshis / SATOSHIS_PER_BTC).ToString("0.00000000", CultureInfo.CreateSpecificCulture("en-US"));
            IfVerboseWriteLine("Total of inputs  to string valueInBtcStr is (before replace): " + valueInBtcStr);
            valueInBtcStr = valueInBtcStr.Replace(',', '.');
            IfVerboseWriteLine("Total of inputs  to string valueInBtcStr is (after replace): " + valueInBtcStr);

            //////////////////////// END of "client" code ////////////////////////////

            //create the transaction and sign with cient and server private keys (should be done in 2 phases)
            var txBuilder = new TransactionBuilder();
            var tx = txBuilder
                    .AddCoins(client1CoinsP2SH)
                    .AddKeys(privKeyClient1, privKeyServer)
                    .AddKnownRedeems(client1P2SHScript)
                    .Send(receiverBtcAddress, valueInBtcStr)
                    .SetChange(client1P2SHAddress)
                    .BuildTransaction(true); //true = sign!  If false, don't generate any "input script"

            IfVerboseWriteLine("Transaction signed");

            //serialize the transaction and prepare the result dto
            var result = new DtoSignedTransaction()
            {
                RawTx = tx.ToHex(),
                TxHash = tx.GetHash().ToString(),
                TransferedValue = valueInSatoshis,
                FeeValue = DEFAULT_FEE,
                Success = true,
                Message = ""
            };

            IfVerboseWriteLine("DtoSignedTransaction created");

            return result;
        }
    }
}
