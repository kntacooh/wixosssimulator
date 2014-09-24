using WixossSimulator.Crawling;

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace wixosssimulatorTest.components.crawler
{
    [TestClass]
    public class WebCrawlerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            Uri u1 = new Uri("http://www.takaratomy.co.jp/products/wixoss/card/card_list.php");
            Uri u2 = new Uri(u1, "?page=1");
            Assert.AreEqual("http://www.takaratomy.co.jp/products/wixoss/card/card_list.php?page=1", u2.AbsoluteUri);
        }
        [TestMethod]
        public void TestMethod2()
        {
            Uri u1 = new Uri("http://www.takaratomy.co.jp/products/wixoss/card/card_list.php?page=4");
            Uri u2 = new Uri(u1, "./card_detail.php?id=1");
            Assert.AreEqual("http://www.takaratomy.co.jp/products/wixoss/card/card_detail.php?id=1", u2.AbsoluteUri);
        }
    }
}
