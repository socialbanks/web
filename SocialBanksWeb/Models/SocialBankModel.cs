using Parse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocialBanksWeb.Models
{
    public class SocialBankModel
    {
        public SocialBankModel(ParseObject obj) {
            name = obj.Get<string>("name");
            description = obj.Get<string>("description");
            image = obj.Get<ParseFile>("image").Url;
            address = obj.Get<string>("address");
            zipcode = obj.Get<string>("zipcode");
            socialMoneyName = obj.Get<string>("socialMoneyName");
            onlineSocialMoneyBalance = obj.Get<int>("onlineSocialMoneyBalance");
            bitcoinAddressForSocialMoneyIssuance = obj.Get<string>("bitcoinAddressForDonation");
        }
        public string name {get; private set;}
        public string description { get; private set; }
        public System.Uri image { get; private set;}
        public string address { get; private set; }
        public string zipcode { get; private set; }
        public string socialMoneyName { get; private set; }
        public int onlineSocialMoneyBalance { get; private set; }
        public string bitcoinAddressForSocialMoneyIssuance { get; private set; }
    }
}
