using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml.Serialization;
using Microsoft.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataServiceHttpUnitTests.AdoNetDataServicesClients;

namespace DataServiceHttpUnitTests
{
    /// <summary>
    /// Summary description for DataServiceHttpTests
    /// </summary>
    [TestClass]
    public class DataServiceHttpTests
    {
        private const string BaseUrl = "http://127.0.0.1:81/"; //local url for Azure Tools
        private const string V1BaseUrl = BaseUrl + "v1";
        private const string V1DCBaseUrl = V1BaseUrl + "/dc";
        private readonly AtomPubClient _appClient;
        private readonly dcDataService _dummyDCDataService;

        public DataServiceHttpTests()
        {
            _appClient = new AtomPubClient();
            _dummyDCDataService = new dcDataService(new Uri(BaseUrl + V1BaseUrl + V1DCBaseUrl));
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion

        private ServiceDocument GetServiceDocument(string uriString)
        {
            ServiceDocument doc = _appClient.GetServiceDocument(new Uri(uriString));
            return doc;
        }

        private SyndicationFeed GetFeed(string uriString)
        {
            SyndicationFeed feed = _appClient.GetFeed(new Uri(uriString));
            return feed;
        }

        [TestMethod]
        public void ReturnsValidV1ServiceDocument()
        {
            ServiceDocument doc = GetServiceDocument(V1BaseUrl);
            Assert.AreEqual("Default", doc.Workspaces[0].Title.Text,
                            "Service Document workspace title should be 'Default'");
            ResourceCollectionInfo collection = doc.Workspaces[0].Collections[0];
            Assert.AreEqual("dc", collection.Link.OriginalString,
                            "First collection href should equal 'dc'");
            Assert.AreEqual("District of Columbia", collection.Title.Text,
                            "First collection title should equal 'District of Columbia'");
            Assert.AreEqual("application/atomsvc+xml", collection.Accepts[0],
                            "First collection should accept 'application/atomsvc+xml'");
        }

        [TestMethod]
        public void ReturnsValidV1DCServiceDocument()
        {
            ServiceDocument doc = GetServiceDocument(V1DCBaseUrl);
            Assert.AreEqual("Default", doc.Workspaces[0].Title.Text,
                            "DC - Service Document workspace title should be 'Default'");
            ResourceCollectionInfo collection = doc.Workspaces[0].Collections[0];
            Assert.AreEqual("AmbulatorySurgicalCenters", collection.Link.OriginalString,
                            "DC - First collection href should equal 'AmbulatorySurgicalCenters'");
            Assert.AreEqual("AmbulatorySurgicalCenters", collection.Title.Text,
                            "DC - First collection title should equal 'AmbulatorySurgicalCenters'");
        }

        [TestMethod]
        public void ReturnsValidMetadataDocument()
        {
            var http = new HttpClient(V1DCBaseUrl + "/");
            var response = http.Get("$metadata");
            response.EnsureStatusIsSuccessful();
            var edmx = response.Content.ReadAsXmlSerializable<Edmx>();

            Assert.IsNotNull(edmx, "Service did not return a valid metadata document");
        }

        [TestMethod]
        public void ReturnsValidV1DCBankLocationsFeed()
        {
            var query = from b in _dummyDCDataService.BankLocations
                       select b;

            SyndicationFeed feed = GetFeed(query.ToString());
            Assert.AreEqual("BankLocations", feed.Title.Text,
                            "Feed title should be 'BankLocations'");
            Assert.IsTrue(feed.Items.Count() > 0,
                          "Feed should have more than 0 entry items");
            Assert.AreEqual("OGDI.dc.BankLocationsItem", feed.Items.FirstOrDefault().Categories[0].Name,
                            @"First \entry\category\term value should be 'OGDI.dc.BankLocationsItem'");
        }

        [TestMethod]
        public void ReturnsValidV1DCBankLocationsAdamsBankFilterQueryFeed()
        {
            var query = from b in _dummyDCDataService.BankLocations
                        where b.name == "Adams Bank"
                        select b;

            SyndicationFeed feed = GetFeed(query.ToString());
            Assert.AreEqual("BankLocations", feed.Title.Text,
                            "Feed title should be 'BankLocations'");
            Assert.IsTrue(feed.Items.Count() > 0,
                          "Feed should have more than 0 entry items");
            Assert.AreEqual("OGDI.dc.BankLocationsItem", feed.Items.FirstOrDefault().Categories[0].Name,
                            @"First \entry\category\term value should be 'OGDI.dc.BankLocationsItem'");
        }
    }
}