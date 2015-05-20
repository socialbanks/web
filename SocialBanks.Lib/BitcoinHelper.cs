using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialBanks.Lib
{
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
        public static string Log = "";

        private const decimal SATOSHIS_PER_BTC = 100000000.0m;
        private const long DEFAULT_FEE = 2000; //0.00002000  (R$0.02 em moeda social)

        //https://blockchain.info/pt/unspent?active=3Qx7v3AQshdKGCqu81QYtkQFDwHKDqaNBi
        private async Task<List<DtoUnspentTransaction>> FindUnspentTransactionsByAddress(string address)
        {
            BitcoinHelper.Log += " | FindUnspentTransactionsByAddress";

            var result = new List<DtoUnspentTransaction>();
/*
            result.Add(
                new DtoUnspentTransaction()
                {
                    TxId = "bf6345df158ebfe9f43f8033b26a352113185a2204d37176e9696a8dc044fe18",
                    Index = 0,
                     ValueInSatoshis = 49000
                });
*/            
            var apiHelper = new APIHelper();
            Dictionary<string, object> result_unspent = await apiHelper.get_unspent(address);

            BitcoinHelper.Log += " | Find... Ponto 1";

            for (int i = 0; i < result_unspent.Count; i++)
            {
                BitcoinHelper.Log += " | Find... Ponto 2";

                var item = result_unspent.ElementAt(i);
                if (item.Key == "unspent_outputs")
                {
                    BitcoinHelper.Log += " | Find... Ponto 3";


                    var outputs = (item.Value as List<object>);
                    for (int j = 0; j < outputs.Count; j++)
                    {
                        BitcoinHelper.Log += " | Find... Ponto 4";

                        Dictionary<string, object> output = (outputs[j] as Dictionary<string, object>);

                        string tx_hash_big_endian = output["tx_hash_big_endian"].ToString();
                        int tx_output_n = int.Parse(output["tx_output_n"].ToString());
                        long value = long.Parse(output["value"].ToString());

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

            BitcoinHelper.Log += " | Find... Ponto 5";

            return result;
        }

        public long GetAddressBalanceInSatoshis(string address)
        {
            var apiHelper = new APIHelper();
            var task = FindUnspentTransactionsByAddress(address);
            task.Wait();
            List<DtoUnspentTransaction> unspentTrans = task.Result;

            var selectedUnspentTrans = new List<DtoUnspentTransaction>();
            long addressBalanceInSatoshis = 0;
            foreach (var trans in unspentTrans)
            {
                selectedUnspentTrans.Add(trans);

                addressBalanceInSatoshis += trans.ValueInSatoshis;
            }
            return addressBalanceInSatoshis;
        }

        public DtoSignedTransaction CreateAndSignP2SHTransaction(string wifServer, string wifClient, string receiverAddress, long valueInSatoshis)
        {
            //Only SocialBanks know this privKey
            var privKeyServer = Key.Parse(wifServer, Network.Main);
            var pubKeyServer = privKeyServer.PubKey;
            var addressServer = pubKeyServer.GetAddress(Network.Main); // => 14pkzzJbAg1N3EFkEnc4o5uHQJAzCqUUFJ

            var privKeyClient1 = Key.Parse(wifClient);
            var pubKeyClient1 = privKeyClient1.PubKey;
            var AddressClient1 = privKeyClient1.PubKey.GetAddress(Network.Main); // => AWXoDzdqqSbf3Fo7yKozXX2aP9nvmsVse


            //var receiverBtcAddress = new BitcoinAddress(receiverAddress, Network.Main); //P2PKH
            var receiverBtcAddress = new BitcoinScriptAddress(receiverAddress, Network.Main); //P2SH

            Script client1P2SHScript =
                PayToMultiSigTemplate.Instance.GenerateScriptPubKey(2, new[] { pubKeyServer, pubKeyClient1 });
            var client1P2SHAddress = client1P2SHScript.GetScriptAddress(Network.Main);// => 3Qx7v3AQshdKGCqu81QYtkQFDwHKDqaNBi	

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
                throw new Exception("Insufficient funds in " + receiverAddress);
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

                string valueInBtc = (valueInSatoshisWithFee / SATOSHIS_PER_BTC).ToString("0.00000000");
                client1P2SH.Outputs.Add(new TxOut(valueInBtc, client1P2SHAddress));
            }

            //Coin array = new transaction input array
            Coin[] client1CoinsP2SH = client1P2SH
                .Outputs
                .Select((outp, i) => new ScriptCoin(new OutPoint(selectedUnspentTrans[i].TxHash, selectedUnspentTrans[i].Index), outp, client1P2SHScript))
                .ToArray();

            var valueInBtcStr = (valueInSatoshis / SATOSHIS_PER_BTC).ToString("0.00000000");

            var txBuilder = new TransactionBuilder();
            var tx = txBuilder
                    .AddCoins(client1CoinsP2SH)
                    .AddKeys(privKeyClient1, privKeyServer)
                    .AddKnownRedeems(client1P2SHScript)
                    .Send(receiverBtcAddress, valueInBtcStr)
                    .SetChange(client1P2SHAddress)
                    .BuildTransaction(true); //false => don't generate any "input script"
            
            var result = new DtoSignedTransaction()
            {
                RawTx = tx.ToHex(),
                TxHash = tx.GetHash().ToString(),
                TransferedValue = valueInSatoshis,
                FeeValue = DEFAULT_FEE,
                Success = true,
                Message = ""
            };

            return result;
        }

        public async Task<DtoSignedTransaction> CreateAndSignP2SHTransactionAsync(string wifServer, string wifClient, string receiverAddress, long valueInSatoshis)
        {
            BitcoinHelper.Log += " | Inicio";
            //Only SocialBanks know this privKey
            var privKeyServer = Key.Parse(wifServer, Network.Main);
            var pubKeyServer = privKeyServer.PubKey;
            var addressServer = pubKeyServer.GetAddress(Network.Main); // => 14pkzzJbAg1N3EFkEnc4o5uHQJAzCqUUFJ

            var privKeyClient1 = Key.Parse(wifClient);
            var pubKeyClient1 = privKeyClient1.PubKey;
            var AddressClient1 = privKeyClient1.PubKey.GetAddress(Network.Main); // => AWXoDzdqqSbf3Fo7yKozXX2aP9nvmsVse

            var receiverBtcAddress = new BitcoinAddress(receiverAddress);

            BitcoinHelper.Log += " | Ponto 1";

            Script client1P2SHScript =
                PayToMultiSigTemplate.Instance.GenerateScriptPubKey(2, new[] { pubKeyServer, pubKeyClient1 });
            var client1P2SHAddress = client1P2SHScript.GetScriptAddress(Network.Main);// => 3Qx7v3AQshdKGCqu81QYtkQFDwHKDqaNBi	

            BitcoinHelper.Log += " | Ponto 2";

            var apiHelper = new APIHelper();
            List<DtoUnspentTransaction> unspentTrans = await FindUnspentTransactionsByAddress(client1P2SHAddress.ToString());

            BitcoinHelper.Log += " | Ponto 3";

            var selectedUnspentTrans = new List<DtoUnspentTransaction>();
            long addressBalanceInSatoshis = 0;
            foreach (var trans in unspentTrans)
            {
                selectedUnspentTrans.Add(trans);

                addressBalanceInSatoshis += trans.ValueInSatoshis;

                if (addressBalanceInSatoshis >= (valueInSatoshis + DEFAULT_FEE))
                    break;
            }

            BitcoinHelper.Log += " | Ponto 4";

            //if balance isn't sufficient
            if (addressBalanceInSatoshis < (valueInSatoshis + DEFAULT_FEE))
            {
                throw new Exception("Insufficient funds in " + receiverAddress);
            }

            //var txHash = new uint256("967f947b7f995d7f45c4ce1f6eb42baf58376d8f9ba768322d2abe858f3bd272");
            //var totalInBtc = "0.002";

            BitcoinHelper.Log += " | Ponto 5";

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

                string valueInBtc = (valueInSatoshisWithFee / SATOSHIS_PER_BTC).ToString("0.00000000");
                client1P2SH.Outputs.Add(new TxOut(valueInBtc, client1P2SHAddress));
            }

            //Coin array = new transaction input array
            Coin[] client1CoinsP2SH = client1P2SH
                .Outputs
                .Select((outp, i) => new ScriptCoin(new OutPoint(selectedUnspentTrans[i].TxHash, selectedUnspentTrans[i].Index), outp, client1P2SHScript))
                .ToArray();

            var valueInBtcStr = (valueInSatoshis / SATOSHIS_PER_BTC).ToString("0.00000000");

            var txBuilder = new TransactionBuilder();
            var tx = txBuilder
                    .AddCoins(client1CoinsP2SH)
                    .AddKeys(privKeyClient1, privKeyServer)
                    .AddKnownRedeems(client1P2SHScript)
                    .Send(receiverBtcAddress, valueInBtcStr)
                    .SetChange(client1P2SHAddress)
                    .BuildTransaction(true); //false => don't generate any "input script"


            BitcoinHelper.Log += " | Ponto 6";

            

            var result = new DtoSignedTransaction()
            {
                RawTx = tx.ToHex(),
                TxHash = tx.GetHash().ToString(),
                TransferedValue = valueInSatoshis,
                FeeValue = DEFAULT_FEE,
                Success = true,
                Message = ""
            };

            //notify the Parse server
            //apiHelper.set_transaction_broadcasted();
            
            return result;

        }

    }
}
