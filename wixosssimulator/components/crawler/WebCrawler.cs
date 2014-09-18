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
        private CrawledDomain crawledDomain;
        //ドメインごとの動作を規定するストラテジー
        private ICrawledDomainStrategy crawledDomainStrategy = new CrawledDomainStrategy();

        /// <summary> スクレイピングを行うサイトのドメインを示す列挙値を取得します。 </summary>
        public CrawledDomain CrawledDomain
        {
            get { return this.crawledDomain; }
            set
            {
                this.crawledDomain = value;
                switch (this.crawledDomain)
                {
                    case WixossSimulator.Crawler.CrawledDomain.Official:
                        crawledDomainStrategy = new OfficialStrategy();
                        break;
                    case WixossSimulator.Crawler.CrawledDomain.Wiki:
                        crawledDomainStrategy = new WikiStrategy();
                        break;
                    default:
                        crawledDomainStrategy = new CrawledDomainStrategy();
                        break;
                }
            }
        }
        /// <summary> スクレイピングを行うページのURLのリストを取得します。 </summary>
        public HashSet<string> Urls = new HashSet<string>();

        //public WebCrawler() { }
        /// <summary> スクレイピングを行うサイトのドメインを示す列挙値を指定して、新しいインスタンスを初期化します。 </summary>
        /// <param name="domain"> スクレイピングを行うサイトのドメインを示す列挙値。 </param>
        public WebCrawler(CrawledDomain domain)
        {
            this.CrawledDomain = domain;
        }

        //そのドメインに存在する全てのカード情報をURLに追加
        public void SearchAllCardResource() 
        {
            foreach (string url in crawledDomainStrategy.SearchAllUrls())
            {
                this.Urls.Add(url);
            }
        }

        //HTMLドキュメントの中のカードの情報部分を取得する
        private string GetHtmlCardInformation(string url) { return crawledDomainStrategy.GetHtmlCardInformation(url); }
        //Cardクラスの型に一致するように変換する
        private Card.Card ConvertToCard(string html) { return crawledDomainStrategy.ConvertToCard(html); }


    }

    /// <summary> スクレイピングを行うサイトのドメインを示す列挙値を提供します。 </summary>
    public enum CrawledDomain
    {
        /// <summary> 不明。 </summary>
        Unknown,
        /// <summary> 公式ホームページ。 </summary>
        Official,
        /// <summary> 現在の時点で信頼があると思われるWiki。 </summary>
        Wiki,
    }

    public interface ICrawledDomainStrategy
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

    public class CrawledDomainStrategy : ICrawledDomainStrategy
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

    public class OfficialStrategy : ICrawledDomainStrategy
    {
        public string Url { get { return "http://www.takaratomy.co.jp/"; } }

        List<string> ICrawledDomainStrategy.SearchAllUrls()
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

        string ICrawledDomainStrategy.GetHtmlCardInformation(string url)
        {
            throw new NotImplementedException();
        }

        Card.Card ICrawledDomainStrategy.ConvertToCard(string html)
        {
            throw new NotImplementedException();
        }
    }

    public class WikiStrategy : ICrawledDomainStrategy
    {
        public string Url { get { return "http://wixoss.81.la/"; } }

        List<string> ICrawledDomainStrategy.SearchAllUrls()
        {
            throw new NotImplementedException();
        }

        string ICrawledDomainStrategy.GetHtmlCardInformation(string url)
        {
            throw new NotImplementedException();
        }

        Card.Card ICrawledDomainStrategy.ConvertToCard(string html)
        {
            throw new NotImplementedException();
        }
    }
}