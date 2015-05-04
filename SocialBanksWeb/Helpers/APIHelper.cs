using Parse;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SocialBanksWeb.Helpers
{
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
        private static APIHelper instance;

        //TODO: Move to web.config and generate new keys (don't publish our keys!)
        private const string ApplicationId_DEV = "bCOd9IKjrpxCPGYQfyagabirn7pYFjYTvJqkq1x1";
        private const string DotnetKey_DEV = "GYMOAhUQ55yYAuEehlecpipu90RFeaPSPn3zcFZ6";

        static APIHelper()
        {
            ParseClient.Initialize(ApplicationId_DEV, DotnetKey_DEV);
        }

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

        public string Hello()
        {
            Task<string> task = ParseCloud.CallFunctionAsync<string>("hello", new Dictionary<string, object>());

            task.Wait();
            return task.Result;
        }

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
    }
}
