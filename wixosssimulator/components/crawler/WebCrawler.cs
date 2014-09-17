using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Collections.ObjectModel; //ReadOnlyDictionary

namespace WixossSimulator.Crawler
{
    public class WebCrawler
    {
        //スクレイピングを行うドメインの列挙値
        private CrawledDomain crawledDomain;
        //サイトごとのメソッドの動作を規定するストラテジー
        private ICrawledDomainStrategy methodForEachDomain;
        /// <summary>  </summary>
        private string[] relativePathList;

        /// <summary>  </summary>
        public CrawledDomain CrawledDomain
        {
            get { return this.crawledDomain; }
            set
            {
                this.crawledDomain = value;
                switch (this.crawledDomain)
                {
                    case WixossSimulator.Crawler.CrawledDomain.Official:
                        methodForEachDomain = new OfficialStrategy();
                        break;
                    case WixossSimulator.Crawler.CrawledDomain.Wiki:
                        methodForEachDomain = new WikiStrategy();
                        break;
                    default:
                        methodForEachDomain = new CrawledDomainStrategy();
                        break;
                }
            }
        }

        /// <summary>  </summary>
        private static ReadOnlyDictionary<CrawledDomain, string> domain = new ReadOnlyDictionary<CrawledDomain, string>(
            new Dictionary<CrawledDomain, string>()
            {
                {CrawledDomain.Wiki, "www.takaratomy.co.jp"},
                {CrawledDomain.Wiki, "wixoss.81.la"},
            });


        //新しいインスタンスを初期化する
        protected WebCrawler(); //不要?
        public WebCrawler(string url);
        public WebCrawler(CrawledDomain domain);
        public WebCrawler(string domain, string relativePath);
        public WebCrawler(CrawledDomain domain, string relativePath);

        //そのドメインに存在する全てのカード情報を検索する
        private string[] SearchAllCardResource() { return methodForEachDomain.SearchAllCardResource(); }
        //HTMLドキュメントの中のカードの情報部分を取得する
        private string GetHtmlCardInformation(string relativePath) { return methodForEachDomain.GetHtmlCardInformation(relativePath); }
        //Cardクラスの型に一致するように変換する
        private Card.Card ConvertToCard(string html) { return methodForEachDomain.ConvertToCard(html); }


    }

    /// <summary> スクレイピングを行うドメインを示す列挙値を提供します。 </summary>
    public enum CrawledDomain
    {
        /// <summary> 公式ホームページ。 </summary>
        Official,
        /// <summary> 現在の時点で信頼があると思われるWiki。 </summary>
        Wiki,
    }

    public interface ICrawledDomainStrategy
    {
        //識別子に対応する一意のカード情報が記されるページのURLを取得する
        string GetResourceUrlFromId(string id);
        //そのドメインに存在する全てのカード情報を検索する
        string[] SearchAllCardResource();
        //HTMLドキュメントの中のカードの情報部分を取得する
        string GetHtmlCardInformation(string relativePath);
        //Cardクラスの型に一致するように変換する
        Card.Card ConvertToCard(string html);
    }

    public class CrawledDomainStrategy : ICrawledDomainStrategy
    {
        public string GetResourceUrlFromId(string id)
        {
            throw new NotImplementedException();
        }

        public string[] SearchAllCardResource()
        {
            throw new NotImplementedException();
        }

        public string GetHtmlCardInformation(string relativePath)
        {
            throw new NotImplementedException();
        }

        public Card.Card ConvertToCard(string html)
        {
            throw new NotImplementedException();
        }
    }

    public class OfficialStrategy : ICrawledDomainStrategy
    {

    }

    public class WikiStrategy : ICrawledDomainStrategy
    {

    }
}