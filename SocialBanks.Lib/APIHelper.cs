using Parse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SocialBanks.Lib
{

    public class APIHelper
    {
        public bool CauseError = false;
        public APIHelper()
        {
        }

        public void Initialize(string keysFilePath)
        {
            var keys = System.IO.File.ReadAllLines(keysFilePath);
            ParseClient.Initialize(keys[0], keys[1]);
        }


        public async Task<IEnumerable<ParseObject>> get_socialbanks()
        {
            var query = from bank in ParseObject.GetQuery("SocialBank")
                        where bank.Get<bool>("approved") == true
                        select bank;
            var q = await query.FindAsync();

            return q;
        }

        public async Task<IEnumerable<ParseObject>> FindIncompleteTransactions()
        {
            var query = from trans in ParseObject.GetQuery("Transaction").Include("senderWallet")
                        where trans.Get<string>("broadcastStatus") == "pending"
                        select trans;
            var q = await query.FindAsync();

            return q;
        }

        public async Task<string> hello()
        {
            return await ParseCloud.CallFunctionAsync<string>("hello", new Dictionary<string, object>());
        }

        public async Task<Dictionary<string, object>> get_unspent(string address)
        {
            var d = new Dictionary<string, object>();
            d["addr"] = address;

            return await ParseCloud.CallFunctionAsync<Dictionary<string, object>>("get_unspent", d);
        }

        public async void set_transaction_broadcasted()
        {
            var d = new Dictionary<string, object>();

            await ParseCloud.CallFunctionAsync<Dictionary<string, object>>("set_transaction_broadcasted", d);
        }



        public async Task<DtoApiResponse<List<DtoAsset>>> get_balances(params string[] adresses)
        {
            var d = new Dictionary<string, object>();
            d["address"] = adresses;

            if (this.CauseError)
            {
                d["cause_error"] = true;
            }

            var r = await ParseCloud.CallFunctionAsync<Dictionary<string, object>>("get_balances", d);

            var result = new DtoApiResponse<List<DtoAsset>> { };
            result.Result = new List<DtoAsset> { };

            if (r.ContainsKey("error"))
            {
                result.Error = r["error"] as Dictionary<string, object>;
                return result;
            }

            var apiResult = r["result"] as List<object>;

            for (int i = 0; i < apiResult.Count; i++)
            {
                var dicAsset = apiResult[i] as Dictionary<string, object>;
                result.Result.Add(new DtoAsset(dicAsset["address"] as string, dicAsset["asset"] as string, (long)dicAsset["quantity"]));
            }

            return result;
        }

        public async Task<DtoApiResponse<string>> create_issuance(string source, string asset, long quantity, string description)
        {
            var d = new Dictionary<string, object>();
            d["source"] = source;
            d["asset"] = asset;
            d["quantity"] = quantity;
            d["description"] = description;

            if (this.CauseError)
            {
                d["cause_error"] = true;
            }

            var r = await ParseCloud.CallFunctionAsync<Dictionary<string, object>>("create_issuance", d);

            var result = new DtoApiResponse<string> { };

            if (r.ContainsKey("error"))
            {
                result.Error = r["error"] as Dictionary<string, object>;
                return result;
            }

            result.Result = r["result"].ToString();


            return result;
        }

        public async Task<DtoApiResponse<string>> send(string source, string asset, long quantity, string destination)
        {
            var d = new Dictionary<string, object>();
            d["source"] = source;
            d["asset"] = asset;
            d["quantity"] = quantity;
            d["destination"] = destination;

            if (this.CauseError)
            {
                d["cause_error"] = true;
            }

            var r = await ParseCloud.CallFunctionAsync<Dictionary<string, object>>("send", d);

            var result = new DtoApiResponse<string> { };

            if (r.ContainsKey("error"))
            {
                result.Error = r["error"] as Dictionary<string, object>;
                return result;
            }

            result.Result = r["result"].ToString();


            return result;
        }

        public async Task<DtoApiResponse<List<DtoAsset>>> get_credits(params string[] adresses)
        {
            var d = new Dictionary<string, object>();
            d["address"] = adresses;

            if (this.CauseError)
            {
                d["cause_error"] = true;
            }

            var r = await ParseCloud.CallFunctionAsync<Dictionary<string, object>>("get_credits", d);

            var result = new DtoApiResponse<List<DtoAsset>> { };
            result.Result = new List<DtoAsset> { };

            if (r.ContainsKey("error"))
            {
                result.Error = r["error"] as Dictionary<string, object>;
                return result;
            }

            var apiResult = r["result"] as List<object>;

            for (int i = 0; i < apiResult.Count; i++)
            {
                var dicAsset = apiResult[i] as Dictionary<string, object>;
                result.Result.Add(new DtoAsset(dicAsset["address"] as string, dicAsset["asset"] as string, (long)dicAsset["quantity"]));
            }

            return result;
        }

        public async Task<DtoApiResponse<List<DtoAsset>>> get_debits(params string[] adresses)
        {
            throw new NotImplementedException();
        }

        public async Task<DtoApiResponse<string>> send_social_money(string asset, string senderAddress, string receiverAddress,
            int valueInCents, string description, string txId, string signedRawTransaction)
        {
            var d = new Dictionary<string, object>();
            //d["asset"] = asset;
            d["senderAddress"] = senderAddress;
            d["receiverAddress"] = receiverAddress;
            d["valueInCents"] = valueInCents;
            d["description"] = description;
            d["txId"] = txId;
            d["signedRawTransaction"] = signedRawTransaction;

            if (this.CauseError)
            {
                d["cause_error"] = true;
            }

            var r = await ParseCloud.CallFunctionAsync<string>("send_social_money", d);

            var result = new DtoApiResponse<string> { };

            //if (r.ContainsKey("error"))
            //{
            //    result.Error = r["error"] as Dictionary<string, object>;
            //    return result;
            //}

            result.Result = r;

            return result;
        }

        public string HttpGet(string URI)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(URI);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }

        public Tuple<bool, string> BroadcastTransaction(string rawTransaction)
        {
            var uri = "https://blockchain.info/pushtx";
            var values = new Dictionary<string, string>
                {
                   { "tx", rawTransaction }
                };

            var t = HttpPost(uri, values);
            t.Wait();

            var result = false;
            //Sucesso
            if (t.Result.StartsWith("Transaction Submitted"))
            {
                result = true;
            }


            //Erros identificados:
            //An outpoint is already spent in [87172736]
            //Parse: exception decoding Hex string: invalid characters encountered in Hex string
            //Parse: Error Parsing Transaction

            return new Tuple<bool, string>(result, t.Result);
        }


        public async Task<string> HttpPost(string uri, Dictionary<string, string> formParameters)
        {
            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(formParameters);

                var response = await client.PostAsync(uri, content);

                return await response.Content.ReadAsStringAsync();
            }
        }


        //curl https://testnet.helloblock.io/v1/addresses/unspents?addresses=mvaRDyLUeF4CP7Lu9umbU3FxehyC5nUz3L&addresses=mpjuaPusdVC5cKvVYCFX94bJX1SNUY8EJo&limit=2

        public List<UnspentOutput> FindUnspendTransactions(string address)
        {
            var strJSON = HttpGet("https://blockchain.info/pt/unspent?active=" + address);


            var unspendList = DeserializeUnspentOutputList(strJSON);

            return unspendList.outputs;

        }

        public static UnspentOutputList DeserializeUnspentOutputList(string json)
        {
            var result = new UnspentOutputList();
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(UnspentOutputList));
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            js.WriteObject(ms, result);

            return result;
        }


    }

    [DataContract]
    public class UnspentOutput
    {
        [DataMember]
        public string tx_hash { get; set; }
        [DataMember]
        public string tx_hash_big_endian { get; set; }
        [DataMember]
        public int tx_index { get; set; }
        [DataMember]
        public int tx_output_n { get; set; }
        [DataMember]
        public string script { get; set; }
        [DataMember]
        public int value { get; set; }
        [DataMember]
        public string value_hex { get; set; }
        [DataMember]
        public int confirmations { get; set; }
    }

    [DataContract]
    public class UnspentOutputList
    {
        public UnspentOutputList()
        {
            outputs = new List<UnspentOutput>();
        }

        [DataMember]
        public List<UnspentOutput> outputs { get; set; }
    }
}