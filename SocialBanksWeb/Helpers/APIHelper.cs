using Parse;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SocialBanksWeb.Helpers
{
    public class DtoApiResponse<TDto>
    {
        public TDto Result;
        public Dictionary<string, object> Error;
    }

    public class DtoAsset
    {
        public string Address { get; private set; }
        public string Name { get; private set; }
        public long Quantity { get; private set; }

        public DtoAsset(string address, string name, long quantity)
        {
            Address = address;
            Name = name;
            Quantity = quantity;
        }
    }

    public class APIHelper
    {
        public bool CauseError = false;
        public APIHelper(string keysFilePath)
        {
            var keys = System.IO.File.ReadAllLines(keysFilePath);
            ParseClient.Initialize(keys[0], keys[1]);
        }

        /*
        public static APIHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new APIHelper();
                }
                return instance;
            }
        }
        */

        public async Task<string> hello()
        {
            return await ParseCloud.CallFunctionAsync<string>("hello", new Dictionary<string, object>());
        }

        /*
        public string Hello()
        {
            Task<string> task = ParseCloud.CallFunctionAsync<string>("hello", new Dictionary<string, object>());

            task.Wait();
            return task.Result;
        }
         * */


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
    }
}
