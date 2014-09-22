using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Collections.ObjectModel; //ReadOnlyDictionary
using System.Text.RegularExpressions; //正規表現

namespace WixossSimulator.Crawler
{
    public class WebCrawler
    {
        private CrawledDomainAttribute crawledDomainAttribute;
        //ドメインごとの動作を規定するストラテジー
        private IDomainStrategy domainStrategy = new DomainStrategy();

        /// <summary> スクレイピングを行うサイトのドメインを示す列挙値を取得します。 </summary>
        public CrawledDomainAttribute CrawledDomainAttribute
        {
            get { return this.crawledDomainAttribute; }
            set
            {
                this.crawledDomainAttribute = value;
                switch (this.crawledDomainAttribute)
                {
                    case WixossSimulator.Crawler.CrawledDomainAttribute.Official:
                        domainStrategy = new OfficialStrategy();
                        break;
                    case WixossSimulator.Crawler.CrawledDomainAttribute.Wiki:
                        domainStrategy = new WikiStrategy();
                        break;
                    default:
                        domainStrategy = new DomainStrategy();
                        break;
                }
            }
        }
        /// <summary> スクレイピングを行うページのURLのリストを取得します。 </summary>
        public HashSet<string> Urls = new HashSet<string>();

        //public WebCrawler() { }
        /// <summary> スクレイピングを行うサイトのドメインを示す列挙値を指定して、新しいインスタンスを初期化します。 </summary>
        /// <param name="domain"> スクレイピングを行うサイトのドメインを示す列挙値。 </param>
        public WebCrawler(CrawledDomainAttribute domain)
        {
            this.CrawledDomainAttribute = domain;
        }

        //そのドメインに存在する全てのカード情報をURLに追加
        public void SearchAllUrls() 
        {
            foreach (string url in domainStrategy.SearchAllUrls())
            {
                this.Urls.Add(url);
            }
        }

        //HTMLドキュメントの中のカードの情報部分を取得する
        private string GetHtmlCardInformation(string url) { return domainStrategy.GetHtmlCardInformation(url); }
        //Cardクラスの型に一致するように変換する
        private Card.Card ConvertToCard(string html) { return domainStrategy.ConvertToCard(html); }


    }

    /// <summary> スクレイピングを行うサイトのドメインを示す列挙値を提供します。 </summary>
    public enum CrawledDomainAttribute
    {
        /// <summary> 不明。 </summary>
        Unknown,
        /// <summary> 公式ホームページ。 </summary>
        Official,
        /// <summary> 現在の時点で信頼があると思われるWiki。 </summary>
        Wiki,
    }

    public interface IDomainStrategy
    {
        //ドメインを示すURL
        string Url { get; }
        //識別子に対応する一意のカード情報が記されるページのURLを取得する
        //string GetResourceUrlFromId(string id);
        //そのドメインに存在する全てのカード情報を検索する
        List<string> SearchAllUrls();
        //HTMLドキュメントの中のカードの情報部分を取得する
        string GetHtmlCardInformation(string url);
        //Cardクラスの型に一致するように変換する
        Card.Card ConvertToCard(string html);
    }

    public class DomainStrategy : IDomainStrategy
    {
        public string Url { get { return "http://"; } }

        public List<string> SearchAllUrls()
        {
            throw new NotImplementedException();
        }

        public string GetHtmlCardInformation(string url)
        {
            throw new NotImplementedException();
        }

        public Card.Card ConvertToCard(string html)
        {
            throw new NotImplementedException();
        }
    }

    public class OfficialStrategy : IDomainStrategy
    {
        public string Url { get { return "http://www.takaratomy.co.jp/"; } }

        List<string> IDomainStrategy.SearchAllUrls()
        {
            List<string> results = new List<string>();
            Regex regexCardUrl = new Regex(@"href\s*=\s*""(?<relativePath>.*?)""",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            Uri uriBase = new Uri(this.Url + "products/wixoss/card/card_list.php");

            string htmlPageUrl = HtmlStream.GetDocument(uriBase);
            htmlPageUrl = htmlPageUrl.Substring(
                htmlPageUrl.IndexOf(@"<ul class=""simple-pagination compact-theme"">", StringComparison.OrdinalIgnoreCase));
            htmlPageUrl = htmlPageUrl.Substring(0,
                htmlPageUrl.IndexOf(@"</ul>", StringComparison.OrdinalIgnoreCase));

            Regex regexPageUrl = new Regex(@"<li>[^<]*<a[^>]*href=""(?<relativePath>.*?)""[^>]*>[^<]*</a>[^<]*</li>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            foreach (Match matchPageUrl in regexPageUrl.Matches(htmlPageUrl))
            {
                Uri uriPage = new Uri(uriBase, matchPageUrl.Groups["relativePath"].Value);

                string htmlCardUrl = HtmlStream.GetDocument(uriPage);
                htmlCardUrl = htmlCardUrl.Substring(
                    htmlCardUrl.IndexOf(@"<table class=""table_layout_01"">", StringComparison.OrdinalIgnoreCase));
                htmlCardUrl = htmlCardUrl.Substring(0,
                    htmlCardUrl.IndexOf(@"</table>", StringComparison.OrdinalIgnoreCase));

                foreach (Match matchCardUrl in regexCardUrl.Matches(htmlCardUrl))
                {
                    Uri uriCard = new Uri(uriPage, matchCardUrl.Groups["relativePath"].Value);
                    results.Add(uriCard.AbsoluteUri);
                }
            }

            return results;
        }

        string IDomainStrategy.GetHtmlCardInformation(string url)
        {
            throw new NotImplementedException();
        }

        Card.Card IDomainStrategy.ConvertToCard(string html)
        {
            throw new NotImplementedException();
        }
    }

    public class WikiStrategy : IDomainStrategy
    {
        public string Url { get { return "http://wixoss.81.la/"; } }

        List<string> IDomainStrategy.SearchAllUrls()
        {
            throw new NotImplementedException();
        }

        string IDomainStrategy.GetHtmlCardInformation(string url)
        {
            throw new NotImplementedException();
        }

        Card.Card IDomainStrategy.ConvertToCard(string html)
        {
            throw new NotImplementedException();
        }
    }
}