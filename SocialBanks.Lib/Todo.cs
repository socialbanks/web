using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocialBanks.Lib
{
    public class TodoAttribute : Attribute
    {
        public TodoAttribute(string description)
        {
        }
    }
}
