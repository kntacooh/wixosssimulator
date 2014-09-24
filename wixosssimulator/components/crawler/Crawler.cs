using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

using MySql.Data.MySqlClient;
using System.Data;

using System.Collections.ObjectModel; //ReadOnlyDictionary
using System.Text.RegularExpressions; //正規表現

//using WixossSimulator.Card;

namespace WixossSimulator.Crawling
{
    [HubName("crawler")]
    public class Crawler : Hub
    {
        private CrawledDomainAttribute crawledDomainAttribute = CrawledDomainAttribute.Unknown;
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
                    case CrawledDomainAttribute.Official:
                        domainStrategy = new OfficialStrategy();
                        break;
                    case CrawledDomainAttribute.Wiki:
                        domainStrategy = new WikiStrategy();
                        break;
                    default:
                        domainStrategy = new DomainStrategy();
                        break;
                }
            }
        }
        /// <summary> スクレイピングを行うページを表すURLの識別値を取得します。 </summary>
        public HashSet<string> DomainId = new HashSet<string>();

        //そのドメインに存在する全ての DomainId にあたる識別値を取得する
        public void SearchAllDomainId(string domain)
        {
            CrawledDomainAttribute = (CrawledDomainAttribute)Enum.Parse(typeof(CrawledDomainAttribute), domain);

            foreach (string url in domainStrategy.SearchAllDomainId())
            {
                this.DomainId.Add(url);
            }
        }


        // 指定したドメインで登録されたものに合致するカラムを全て取り出しクライアントに返す
        public void GetSqlData(string userId, string password, string domain)
        {
            //CrawledDomainAttribute = ConvertToCrawledDomainAttribute(domain);
            using (MySqlConnection conn = new MySqlConnection())
            {
                conn.ConnectionString = CardOfMySql.GetConnectionString(userId, password);
                using (MySqlDataAdapter da = new MySqlDataAdapter(
                    "SELECT * FROM crawling WHERE domain = '" + domain + "'", conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                }
            }
        }
        //DomainId から そのカード情報を持つページのURLをクライアントに返す
        public void GetUrlFromDomainId(string domain, string domainId)
        {
            CrawledDomainAttribute = ConvertToCrawledDomainAttribute(domain);
            Clients.All.SetUrl(domainStrategy.GetUrlFromDomainId(domainId));
            
        }
        //CrawlerDomainAttribute の文字列のリストをクライアントに返す
        public void GetDomainList()
        {
            foreach (string s in Enum.GetNames(typeof(CrawledDomainAttribute)))
            {
                Clients.All.SetDomainAttribute(s);
            }
        }

        private CrawledDomainAttribute ConvertToCrawledDomainAttribute(string domain)
        {
            return (CrawledDomainAttribute)Enum.Parse(typeof(CrawledDomainAttribute), domain);
        }

        public interface IDomainStrategy
        {
            //ドメインを示すURL
            string Url { get; }
            //そのドメインに存在する全てのカード情報を検索する
            List<string> SearchAllDomainId();
            //識別子に対応する一意のカード情報が記されるページのURLを取得する
            string GetUrlFromDomainId(string domainId);

            ////HTMLドキュメントの中のカードの情報部分を取得する
            //string GetHtmlCardContent(string id);
            ////Cardクラスの型に一致するように変換する
            //Card.Card ConvertToCard(string html);
        }

        public class DomainStrategy : IDomainStrategy
        {
            public string Url { get { return "http://"; } }

            public List<string> SearchAllDomainId()
            {
                throw new NotImplementedException();
            }

            public string GetUrlFromDomainId(string domainId)
            {
                return Url + domainId;
            }
        }

        public class OfficialStrategy : IDomainStrategy
        {
            public string Url { get { return "http://www.takaratomy.co.jp/"; } }

            public List<string> SearchAllDomainId()
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

            public string GetUrlFromDomainId(string domainId)
            {
                return Url + "products/wixoss/card/card_detail.php?id=" + domainId;
            }
        }

        public class WikiStrategy : IDomainStrategy
        {
            public string Url { get { return "http://wixoss.81.la/"; } }

            List<string> IDomainStrategy.SearchAllDomainId()
            {
                throw new NotImplementedException();
            }

            public string GetUrlFromDomainId(string domainId)
            {
                return Url + "?" + domainId;
            }
        }
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
}