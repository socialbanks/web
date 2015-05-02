using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parse;
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
        private const string ApplicationId_DEV = "bCOd9IKjrpxCPGYQfyagabirn7pYFjYTvJqkq1x1";
        private const string DotnetKey_DEV = "GYMOAhUQ55yYAuEehlecpipu90RFeaPSPn3zcFZ6";

        [TestInitialize]
        public void TestInitialize()
        {
            ParseClient.Initialize(ApplicationId_DEV, DotnetKey_DEV);

            Task task = ParseUser.LogInAsync("fabriciomatos", "123456");
            task.Wait();
            
        }

        [TestMethod]
        public void InstanceIsntNull()
        {
            var instance = APIHelper.Instance;
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void HelloWorld()
        {
            Assert.AreEqual("Hello world!", APIHelper.Instance.Hello());
        }

        [TestMethod]
        public void GetBalances_OLD()
        {
            Dictionary<string, object> result = APIHelper.Instance.GetBalances_OLD("1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK");

            var list = result["result"] as List<object>;

            Assert.AreEqual(2, list.Count);

            var result0 = list[0] as Dictionary<string, object>;
            Assert.AreEqual("1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK", result0["address"]);
            Assert.AreEqual("BRAZUCA", result0["asset"]);
            Assert.AreEqual((Int64)49000000000, result0["quantity"]);

            var result1 = list[1] as Dictionary<string, object>;
            Assert.AreEqual("1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK", result1["address"]);
            Assert.AreEqual("XCP", result1["asset"]);
            Assert.AreEqual((Int64)38500000, result1["quantity"]);
        }

        [TestMethod]
        public void GetBalances()
        {
            List<DtoAsset> result = APIHelper.Instance.GetBalances("1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK");

            Assert.AreEqual(2, result.Count);

            //BRAZUCA
            Assert.AreEqual("1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK", result[0].Address);
            Assert.AreEqual("BRAZUCA", result[0].Name);
            Assert.AreEqual((long)49000000000, result[0].Quantity);

            //XCP
            Assert.AreEqual("1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK", result[1].Address);
            Assert.AreEqual("XCP", result[1].Name);
            Assert.AreEqual((long)38500000, result[1].Quantity);

            //BITCOIN
            //TODO: Retornar tambem bitcoin
        }
    }
}
