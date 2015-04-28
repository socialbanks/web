using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parse;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class ParseConnectionTest
    {
        private const string ApplicationId_DEV = "bCOd9IKjrpxCPGYQfyagabirn7pYFjYTvJqkq1x1";
        private const string DotnetKey_DEV = "GYMOAhUQ55yYAuEehlecpipu90RFeaPSPn3zcFZ6";

        [TestInitialize]
        public void TestInitialize()
        {
            ParseClient.Initialize(ApplicationId_DEV, DotnetKey_DEV);
        }

        [TestMethod]
        public void GetBrazilCountryByNameWorks()
        {

            var query = from country in ParseObject.GetQuery("Country")
                        where country.Get<string>("name") == "Brazil"
                        select country;

            Task<ParseObject> task = query.FirstAsync();
            task.Wait();
            ParseObject obj = task.Result;
            
            Assert.AreNotEqual("", obj.ObjectId);
            Assert.AreEqual("Brazil", obj.Get<string>("name"));
        }

        [TestMethod]
        public void FindAllCountriesStartingWithB()
        {

            var query = from country in ParseObject.GetQuery("Country")
                        where country.Get<string>("name").StartsWith("B")
                        select country;

            Task<IEnumerable<ParseObject>> task = query.FindAsync();
            task.Wait();
            IEnumerable<ParseObject> results = task.Result;

            Assert.AreEqual(20, results.ToList().Count);
            Assert.AreEqual("Bahamas", results.ToList().First<ParseObject>().Get<string>("name"));
        }


    }
}
