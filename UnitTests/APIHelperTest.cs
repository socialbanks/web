using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialBanksWeb.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class APIHelperTest
    {
        [TestMethod]
        public void InstanceIsntNull()
        {
            var instance = APIHelper.Instance;
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void HelloWorld()
        {
            Assert.AreEqual("Hello World!", APIHelper.Instance.Hello());
        }
    }
}
