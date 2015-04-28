using Parse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialBanksWeb.Helpers
{
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
            return "Hello World!";
        }

        //public create_new_currency() ...
    }
}