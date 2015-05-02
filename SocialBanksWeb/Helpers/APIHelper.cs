using Parse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SocialBanksWeb.Helpers
{
    public class DtoAssets
    {
        public List<DtoAsset> Result = new List<DtoAsset> { };
        public int ErrorCode;
        public string ErrorMessage;
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


        public async Task<DtoAssets> get_balances(params string[] adresses)
        {
            var d = new Dictionary<string, object>();
            d["address"] = adresses;

            if (this.CauseError)
            {
                d["cause_error"] = true;
            }

            var r = await ParseCloud.CallFunctionAsync<Dictionary<string, object>>("get_balances", d);

            var result = new DtoAssets { };

            if (r.ContainsKey("error"))
            {
                var e = r["error"] as Dictionary<string, object>;
                result.ErrorCode = int.Parse(e["code"].ToString());
                result.ErrorMessage = e["message"].ToString();
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

        /*
        //TODO: Eliminar esse metodo!
        public Dictionary<string, object> GetBalances_OLD(string address)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("address", address);

            var task = ParseCloud.CallFunctionAsync<Dictionary<string, object>>("get_balances", parameters);

            task.Wait();
            return task.Result;
        }


        public List<DtoAsset> GetBalances(string address)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("address", address);

            var task = ParseCloud.CallFunctionAsync<Dictionary<string, object>>("get_balances", parameters);

            task.Wait();

            var apiResult = task.Result["result"] as List<object>;
            var result = new List<DtoAsset>();

            for (int i = 0; i < apiResult.Count; i++)
            {
                var dicAsset = apiResult[i] as Dictionary<string, object>;
                result.Add(new DtoAsset(dicAsset["address"] as string, dicAsset["asset"] as string, (long)dicAsset["quantity"]));
            }

            return result;
        }
        */
        //public create_new_currency() ...
    }
}