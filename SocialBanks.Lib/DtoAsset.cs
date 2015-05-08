using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocialBanks.Lib
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
}
