using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

using WixossSimulator.Crawler;
using System.Threading.Tasks; //Task

namespace WixossSimulator.test
{
    [HubName("searching")]
    public class SearchingHub : Hub
    {
        //public override Task On～

        public void GetDomainList() 
        {
            foreach (string s in Enum.GetNames(typeof(CrawledDomainAttribute)))
            {
                Clients.All.SetDomainName(s);
            }
        }

        public void GetUrls(string domain)
        {
            CrawledDomainAttribute crawledDomainAttribute = 
                (CrawledDomainAttribute)Enum.Parse(typeof(CrawledDomainAttribute), domain);

            WebCrawler webCrawler = new WebCrawler(crawledDomainAttribute);
            webCrawler.SearchAllUrls();
            foreach (string url in webCrawler.Urls)
            {
                Clients.All.SetUrl(url);
            }
        }



        public void Hello()
        {
            Clients.All.Hello();
        }
        public void HelloWorld(string name)
        {
            Clients.All.Hello(name + " > hello, world!");
        }
    }
}