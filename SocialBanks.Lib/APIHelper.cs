using Parse;
using System;
using System.Collections.Generic;
using System.Linq;
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


        public async Task<string> hello()
        {
            return await ParseCloud.CallFunctionAsync<string>("hello", new Dictionary<string, object>());
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

        
    }
}