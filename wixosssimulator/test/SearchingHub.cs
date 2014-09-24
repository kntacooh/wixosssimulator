using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

using WixossSimulator.Crawling;
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
            Crawler webCrawler = new Crawler();
            webCrawler.SearchAllDomainId(domain);
            foreach (string url in webCrawler.DomainId)
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