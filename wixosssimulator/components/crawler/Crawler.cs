#define 検索を20件で中止

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

//using System.Collections.ObjectModel; //ReadOnlyDictionary
using System.Text; //StringBuilder
using System.Text.RegularExpressions; //正規表現

using System.Data.SqlClient;
using Newtonsoft.Json;
//using MySql.Data.MySqlClient;
//using System.Data; //DataTable

using WixossSimulator.Sql;
//using WixossSimulator.Card;

namespace WixossSimulator.Crawling
{
    /// <summary> スクレイピングを行うサイトのドメインを示す列挙値を提供します。 </summary>
    public enum DomainAttribute
    {
        /// <summary> 不明。 </summary>
        Unknown,
        /// <summary> 公式ホームページ。 </summary>
        Official,
        /// <summary> 現在の時点で信頼があると思われるWiki。 </summary>
        Wiki,
    }

    [HubName("crawler")]
    public class Crawler : Hub
    {
        private DomainAttribute domainAttribute = DomainAttribute.Unknown;
        /// <summary> ドメインの種類に応じて呼び出されるメソッドを取得します。CrawledDomainAttribute に応じて自動的に設定されます。 </summary>
        protected IDomainStrategy domainStrategy = new UnknownStrategy();
        /// <summary> ドメインを示す列挙値を取得します。 </summary>
        public DomainAttribute DomainAttribute
        {
            get { return this.domainAttribute; }
            set
            {
                this.domainAttribute = value;
                switch (this.domainAttribute)
                {
                    case DomainAttribute.Official:
                        domainStrategy = new OfficialStrategy();
                        break;
                    case DomainAttribute.Wiki:
                        domainStrategy = new WikiStrategy();
                        break;
                    default:
                        domainStrategy = new UnknownStrategy();
                        break;
                }
            }
        }

        /// <summary> Crawlingテーブルのデータをクライアント側のJavaScriptのフォーマットに従って保持します。 </summary>
        protected class FixedCrawlingData
        {
            [JsonProperty("domainId")]
            public string DomainId { get; /*private*/ set; }
            [JsonProperty("url")]
            public string Url { get; /*private*/ set; }
            [JsonProperty("content")]
            public string Content { get; set; }
            [JsonProperty("lastUpdated")]
            public DateTime? LastUpdated { get; set; }
            [JsonProperty("lastConfirmed")]
            public DateTime? LastConfirmed { get; set; }
            [JsonProperty("deleted")]
            public DateTime? deleted { get; set; }

            //public void SetDomainIdWithUrl(string domainId, Crawler crawler)
            //{
            //    this.domainId = domainId;
            //    this.url = crawler.domainStrategy.ConvertDomainIdToUrl(this.domainId);
            //}
        }

        /// <summary> <c>CrawlerDomainAttribute</c> を示す文字列の一覧をクライアントに返します。 </summary>
        public void GetDomainList()
        {
            foreach (string s in Enum.GetNames(typeof(DomainAttribute)))
            {
                Clients.Caller.SetDomainAttribute(s);
            }
        }

        //ゴミ
        ///// <summary> <c>DomainId</c> に対応するURLをクライアントに返します。 </summary>
        //public void GetUrlFromDomainId(string domain, string domainId)
        //{
        //    CrawledDomainAttribute = ConvertToCrawledDomainAttribute(domain); // GetUrlFromDomainId メソッドを使うため
        //    Clients.Caller.SetUrl(domainStrategy.ConvertDomainIdToUrl(domainId));
        //}

        ///// <summary> Crawlingテーブルのデータの中で、ドメインが一致するものを全て取り出してクライアントに返します。 </summary>
        ///// <param name="userId"> 接続で使用するユーザーのID。 </param>
        ///// <param name="password"> 接続で使用するユーザーのパスワード。 </param>
        ///// <param name="domain"> ドメインを示す文字列。 </param>
        //public void GetCrawlingTable2(string userId, string password, string domain)
        //{
        //    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password)) { return; }

        //    CrawledDomainAttribute = ConvertToCrawledDomainAttribute(domain); // GetUrlFromDomainId メソッドを使うため

        //    string connectionString = WixossCardDatabase.CreateConnectionString(userId, password);
        //    string commandString = "SELECT DomainId, Content, LastUpdated, LastConfirmed FROM Crawling WHERE Domain = '" + domain + "'";

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        using (SqlDataAdapter da = new SqlDataAdapter(commandString, connection))
        //        {
        //            DataTable dt = new DataTable();
        //            da.Fill(dt);

        //            foreach (DataRow dr in dt.Rows)
        //            {
        //                Clients.Caller.SetSqlData(dr["DomainId"], domainStrategy.ConvertDomainIdToUrl((string)dr["DomainId"]), dr["lastUpdated"], dr["lastConfirmed"]);
        //            }
        //        }
        //    }

        //    ////string connectionString = CardOfMySql.GetConnectionString(userId, password);
        //    //using (MySqlConnection conn = new MySqlConnection())
        //    //{
        //    //    conn.ConnectionString = Sql.GetConnectionStringForMySql(userId, password);
        //    //    //conn.Open();
        //    //    using (MySqlDataAdapter da = new MySqlDataAdapter(
        //    //        "SELECT domain, domain_id, last_updated, last_confirmed FROM crawling WHERE domain = '" + domain + "'", conn))
        //    //    {
        //    //        DataTable dt = new DataTable();
        //    //        da.Fill(dt);

        //    //        foreach (DataRow dr in dt.Rows)
        //    //        {
        //    //            Clients.Caller.SetSqlData(dr["domain"], dr["domain_id"], dr["last_updated"], dr["last_confirmed"]);
        //    //        }
        //    //    }
        //    //    //conn.Close();
        //    //}
        //}

        ////そのドメインに存在する全てのカード情報を取得してクライアントに返す
        //public HashSet<string> SearchAllDomainId2(string domain)
        //{
        //    Clients.Caller.SetProgressPrimary(0, "カード情報の探索を開始します。");
        //    Clients.Caller.SetProgressSecondary(0, "");

        //    CrawledDomainAttribute = (CrawledDomainAttribute)Enum.Parse(typeof(CrawledDomainAttribute), domain);
        //    HashSet<string> domainId = domainStrategy.SearchAllDomainId(this);

        //    Clients.Caller.SetProgressPrimary(1, "探索が完了しました。");
        //    Clients.Caller.SetProgressSecondary(0, "");

        //    foreach (string d in domainId)
        //    {
        //        Clients.Caller.AddCrawledData(d, domainStrategy.ConvertDomainIdToUrl(d));
        //    }

        //    Clients.Caller.EndSearching();

        //    return domainId;
        //}

        /// <summary> Crawlingテーブルの中で、ドメインが一致するデータを全て取り出してクライアントに返します。 </summary>
        /// <param name="userId"> 接続で使用するユーザーのID。 </param>
        /// <param name="password"> 接続で使用するユーザーのパスワード。 </param>
        /// <param name="domain"> ドメインを示す文字列。 </param>
        public void GetCrawlingTable(string userId, string password, string domain)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password)) { return; }

            DomainAttribute = ConvertToCrawledDomainAttribute(domain);

            string connectionString = WixossCardDatabase.CreateConnectionString(userId, password);
            string query = "SELECT DomainId, Content, LastUpdated, LastConfirmed, Deleted FROM Crawling WHERE Domain = N'" + domain + "'";
            //List<CrawlingData> crawlingTable = new List<CrawlingData>();
            List<FixedCrawlingData> fixedCrawlingTable = new List<FixedCrawlingData>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //crawlingTable.Add(new CrawlingData(reader));
                            FixedCrawlingData crawlingData = new FixedCrawlingData();
                            //crawlingData.SetDomainIdWithUrl(reader.GetString(0), this);
                            crawlingData.DomainId = reader.GetString(0);
                            crawlingData.Url = domainStrategy.ConvertDomainIdToUrl(crawlingData.DomainId);
                            crawlingData.Content = reader.GetString(1);
                            crawlingData.LastUpdated = reader.GetDateTime(2);
                            crawlingData.LastConfirmed = reader.GetDateTime(3);
                            crawlingData.deleted = reader.GetValue(4) as DateTime?;
                            fixedCrawlingTable.Add(crawlingData);
                        }
                    }
                }
                Clients.Caller.SetCrawlingTable(JsonConvert.SerializeObject(fixedCrawlingTable));
                connection.Close();
            }
        }

        /// <summary> 指定したドメインに存在する全てのDomainIdを検索し、Crawlingテーブルと合体させて??クライアントに返します。 </summary>
        /// <param name="domain"> ドメインを示す文字列。 </param>
        /// <param name="fixedCrawlingTableJson"> JavaScriptのcrawlingTableを表すJSON。 </param>
        public void SearchAllDomainId(string domain, string fixedCrawlingTableJson)
        {
            Clients.Caller.SetProgressPrimary(0, "カード情報の探索を開始します。");
            Clients.Caller.SetProgressSecondary(0, "");

            DomainAttribute = (DomainAttribute)Enum.Parse(typeof(DomainAttribute), domain);

            //DateTime confirmed = DateTime.Now;
            List<FixedCrawlingData> fixedCrawlingTable = JsonConvert.DeserializeObject<List<FixedCrawlingData>>(fixedCrawlingTableJson);

            foreach (string domainId in domainStrategy.SearchAllDomainId(this))
            {
                if (!fixedCrawlingTable.Any(f => f.DomainId == domainId))
                {
                    FixedCrawlingData fixedCrawlingData = new FixedCrawlingData();
                    fixedCrawlingData.DomainId = domainId;
                    fixedCrawlingData.Url = domainStrategy.ConvertDomainIdToUrl(domainId);
                    fixedCrawlingTable.Add(fixedCrawlingData);
                }
            }

            Clients.Caller.SetProgressPrimary(1, "探索が完了しました。");
            Clients.Caller.SetProgressSecondary(0, "");

            Clients.Caller.SetCrawlingTable(JsonConvert.SerializeObject(fixedCrawlingTable));
            Clients.Caller.EndSearching();
        }


        //SQLを更新して結果をクライアントに返す
        public void UpdateCrawlingTable(string userId, string password, string domain, string fixedCrawlingTableJson)
        {
            DomainAttribute = ConvertToCrawledDomainAttribute(domain);

            string connectionString = WixossCardDatabase.CreateConnectionString(userId, password);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                //List<FixedCrawlingData> fixedCrawlingTable = JsonConvert.DeserializeObject<List<FixedCrawlingData>>(fixedCrawlingTableJson);
                foreach (FixedCrawlingData fixedCrawlingData in JsonConvert.DeserializeObject<List<FixedCrawlingData>>(fixedCrawlingTableJson))
                {
                    //現在のカード情報を取得
                    string currentContent;
                    DateTime timeAcquired = DateTime.Now;
                    try { currentContent = domainStrategy.GetCurrentContent(fixedCrawlingData.DomainId); }
                    catch { currentContent = null; }

                    string query = CreateUpdateQuery(fixedCrawlingData, currentContent, timeAcquired);
                    if (string.IsNullOrEmpty(query)) { continue; }

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                        //command.ExecuteNonQueryAsync();
                    }
                }

                connection.Close();
            }

        }

        /// <summary> <c>fixedCrawlingData</c> と現在のカード情報から、SQLに発行するクエリを取得します。 </summary>
        /// <param name="fixedCrawlingData"> クライアント側のJavaScriptのフォーマットに従っているCrawlingテーブルのデータ。 </param>
        /// <param name="currentContent"> 現在のカード情報を示す文字列。 </param>
        /// <param name="timeAcquired"> 現在のカード情報を取得した時刻。 </param>
        /// <returns> SQLに発行するクエリ。 </returns>
        private string CreateUpdateQuery(FixedCrawlingData fixedCrawlingData, string currentContent, DateTime timeAcquired)
        {
            StringBuilder query = new StringBuilder("UPDATE Crawling");

            //SQLにまだ存在しないデータの場合
            if (!fixedCrawlingData.LastUpdated.HasValue)
            {
                //SQLに存在せず、カード情報が取得できない場合(リンク切れなどが考えられる??)はクエリを発行しない
                if (string.IsNullOrWhiteSpace(currentContent)) { return null; }
                //カード情報が存在する場合はデータを挿入

                return query.ToString();
            }


            //以下、SQLに既に存在するデータの場合
            //SQLに存在しているが、現在のカード情報が取得できなくなっている場合
            if (string.IsNullOrWhiteSpace(currentContent))
            {
                query.Append(" SET Deleted = ").Append(timeAcquired);
                query.Append(" WHERE Deleted IS NULL");
                query.Append(" AND Domain = N'").Append(DomainAttribute.ToString()).Append("'");
                query.Append(" AND DomainId = N'").Append(fixedCrawlingData.DomainId).Append("'");

                return query.ToString();
            }

            //SQLに存在しているが、現在のカード情報と一致しない＝更新されている場合
            if (currentContent != fixedCrawlingData.Content)
            {
                query.Append(" SET Content = ").Append(currentContent);
                query.Append(", LastUpdated = ").Append(timeAcquired);
                query.Append(" WHERE Deleted IS NULL");
                query.Append(" AND Domain = N'").Append(DomainAttribute.ToString()).Append("'");
                query.Append(" AND DomainId = N'").Append(fixedCrawlingData.DomainId).Append("'");

                return query.ToString();
            }

            //SQLに存在しているが、現在のカード情報と一致＝更新されていない場合

            return query.ToString();
        }

        /// <summary> 文字列からドメインを示す列挙値に変換します。 </summary>
        /// <param name="domain"> ドメインを示す文字列。 </param>
        /// <returns> ドメインを示す列挙値。 </returns>
        private DomainAttribute ConvertToCrawledDomainAttribute(string domain)
        {
            return (DomainAttribute)Enum.Parse(typeof(DomainAttribute), domain);
        }
    }








    public interface IDomainStrategy
    {
        /// <summary>ドメインを示すURLを取得します。 </summary>
        string Url { get; }
        /// <summary> そのドメインに存在する全てのカード情報を識別する <c>DomainId</c> を取得します。 </summary>
        /// <param name="crawler"> 進捗状況をクライアントに返すためのハブであるクラス。 </param>
        /// <returns> カード情報を識別する <c>DomainId</c> の一覧。 </returns>
        HashSet<string> SearchAllDomainId(Crawler crawler);
        /// <summary> カード情報を識別する <c>DomainId</c> に対応するURLを取得します。 </summary>
        /// <param name="domainId"> カード情報を識別する <c>DomainId</c>。 </param>
        /// <returns> カード情報を示すURL。 </returns>
        string ConvertDomainIdToUrl(string domainId);
        /// <summary> カード情報を示すURLから対応する <c>DomainId</c> を取得します。 </summary>
        /// <param name="url"> カード情報を示すURL。 </param>
        /// <returns> カード情報を識別する <c>DomainId</c>。 </returns>
        string ConvertUrlToDomainId(string url);

        /// <summary> 現在のカード情報を取得します。 </summary>
        /// <param name="domainId"> カード情報を識別する <c>DomainId</c>。 </param>
        /// <returns> 現在のカード情報を示す文字列。 </returns>
        string GetCurrentContent(string domainId);
        ////Cardクラスの型に一致するように変換する
        //Card.Card ConvertToCard(string html);
    }

    public class UnknownStrategy : IDomainStrategy
    {
        public string Url { get { return "http://"; } }

        public HashSet<string> SearchAllDomainId(Crawler crawler)
        {
            return new HashSet<string>();
        }

        public string ConvertDomainIdToUrl(string domainId)
        {
            return Url + domainId;
        }

        public string ConvertUrlToDomainId(string url)
        {
            if (url.StartsWith(Url)) { return url.Substring(Url.Length); }
            else { throw new ArgumentException(); }
        }

        public string GetCurrentContent(string domainId)
        {
            return HtmlStream.GetDocument(ConvertDomainIdToUrl(domainId));
        }
    }

    public class OfficialStrategy : IDomainStrategy
    {
        public string Url { get { return "http://www.takaratomy.co.jp/"; } }

        public HashSet<string> SearchAllDomainId(Crawler crawler)
        {
            HashSet<string> results = new HashSet<string>();
            Regex regexCardUrl = new Regex(@"href\s*=\s*""(?<relativePath>.*?)""",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            crawler.Clients.Caller.SetProgressPrimary(0, "カードの検索結果を取得中です。");

            Uri uriBase = new Uri(Url + "products/wixoss/card/card_list.php");
            string htmlPageUrl = HtmlStream.GetDocument(uriBase);
            int numOfItems = int.Parse(Regex.Match(htmlPageUrl, @"<p class=""center"">[^\d]*(\d*)件.*?</p>", RegexOptions.IgnoreCase | RegexOptions.Singleline).Groups[1].Value);

            htmlPageUrl = Regex.Match(htmlPageUrl, @"<ul class=""simple-pagination compact-theme"">(.*?)</ul>", RegexOptions.IgnoreCase | RegexOptions.Singleline).Groups[1].Value;
            //htmlPageUrl = htmlPageUrl.Substring(
            //    htmlPageUrl.IndexOf(@"<ul class=""simple-pagination compact-theme"">", StringComparison.OrdinalIgnoreCase));
            //htmlPageUrl = htmlPageUrl.Substring(0,
            //    htmlPageUrl.IndexOf(@"</ul>", StringComparison.OrdinalIgnoreCase));

            Regex regexPageUrl = new Regex(@"<li>[^<]*<a[^>]*href=""(?<relativePath>.*?)""[^>]*>[^<]*</a>[^<]*</li>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            crawler.Clients.Caller.SetProgressSecondary(0, numOfItems + "件のURLを検出しました。個々のURLを取得しています。");
            int counter = 0;

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
                    results.Add(ConvertUrlToDomainId(uriCard.AbsoluteUri));

                    counter++;
                    crawler.Clients.Caller.SetProgressSecondary(
                        (double)counter / numOfItems,
                        numOfItems + "件のURLを検出しました。個々のURLを取得しています。/ " + counter + "件目: " + uriCard.AbsoluteUri);
                }
#if(検索を20件で中止)
                if (counter >= 20) { break; }
#endif
            }

            crawler.Clients.Caller.SetProgressSecondary(1, "検索完了");

            return results;
        }

        private readonly string stringToConnectId = "products/wixoss/card/card_detail.php?id=";

        public string ConvertDomainIdToUrl(string domainId)
        {
            return Url + stringToConnectId + domainId;
        }

        public string ConvertUrlToDomainId(string url)
        {
            if (url.StartsWith(Url + stringToConnectId)) { return url.Substring(Url.Length + stringToConnectId.Length); }
            else { throw new ArgumentException(); }
        }

        public string GetCurrentContent(string domainId)
        {
            string content = HtmlStream.GetDocument(ConvertDomainIdToUrl(domainId));
            return Regex.Match(content, @"<body>(.*)</body>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline).Groups[1].Value;
        }
    }

    public class WikiStrategy : IDomainStrategy
    {
        public string Url { get { return "http://wixoss.81.la/"; } }

        HashSet<string> IDomainStrategy.SearchAllDomainId(Crawler crawler)
        {
            HashSet<string> results = new HashSet<string>();
            Regex regexCardUrl = new Regex(@"<a[^>]*href=""(?<absolutePath>http://wixoss.81.la/\?%A1%D4.*?)""[^>]*?>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            crawler.Clients.Caller.SetProgressPrimary(0, "ページリストを取得中です。");

            Uri uriBase = new Uri(Url + "?cmd=list");
            string htmlCardUrl = HtmlStream.GetDocument(uriBase);
            htmlCardUrl = Regex.Match(htmlCardUrl, @"<div id=""body"">.*?<div id=""top"".*?</div>.*?(<ul>.*</ul>)[^<]*</div>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline).Groups[1].Value;

            MatchCollection matchCollectionCardUrl = regexCardUrl.Matches(htmlCardUrl);
            int numOfItems = matchCollectionCardUrl.Count;

            crawler.Clients.Caller.SetProgressSecondary(0, numOfItems + "件のURLを検出しました。個々のURLを取得しています。");
            int counter = 0;

            foreach (Match matchCardUrl in matchCollectionCardUrl)
            {
                Uri uriCard = new Uri(matchCardUrl.Groups["absolutePath"].Value);
                results.Add(ConvertUrlToDomainId(uriCard.AbsoluteUri));

                counter++;
                crawler.Clients.Caller.SetProgressSecondary(
                    (double)counter / numOfItems,
                    numOfItems + "件のURLを検出しました。個々のURLを取得しています。/ " + counter + "件目: " + uriCard.AbsoluteUri);

#if(検索を20件で中止)
                if (counter >= 20) { break; }
#endif
            }

            crawler.Clients.Caller.SetProgressSecondary(1, "検索完了");

            return results;
        }

        public string ConvertDomainIdToUrl(string domainId)
        {
            return Url + "?" + domainId;
        }

        public string ConvertUrlToDomainId(string url)
        {
            if (url.StartsWith(Url + "?")) { return url.Substring(Url.Length + 1); }
            else { throw new ArgumentException(); }
        }

        public string GetCurrentContent(string domainId)
        {
            string content = HtmlStream.GetDocument(ConvertDomainIdToUrl(domainId));
            return Regex.Match(content, @"<td valign=""top"">(.*)</td>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline).Groups[1].Value;
        }
    }
}