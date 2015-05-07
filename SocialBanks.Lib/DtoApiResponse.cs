using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocialBanks.Lib
{
    public class DtoApiResponse<TDto>
    {
        public TDto Result;
        public Dictionary<string, object> Error;
    }
}
