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
            public string Content { get; set; }
            [JsonProperty("lastUpdated")]
            public DateTime? LastUpdated { get; set; }
            [JsonProperty("lastConfirmed")]
            public DateTime? LastConfirmed { get; set; }
            [JsonProperty("deleted")]
            public DateTime? Deleted { get; set; }

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

        //改修前
        /// <summary> Crawlingテーブルの中で、ドメインが一致するデータを全て取り出してクライアントに返します。 </summary>
        /// <param name="userId"> 接続で使用するユーザーのID。 </param>
        /// <param name="password"> 接続で使用するユーザーのパスワード。 </param>
        /// <param name="domain"> ドメインを示す文字列。 </param>
        public void GetCrawlingTable(string userId, string password, string domain)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password)) { return; }

            Clients.Caller.SetProgressPrimary(0, "データベースからドメインが一致するデータを検索します。");
            Clients.Caller.SetProgressSecondary(0, "");

            DomainAttribute = ConvertToCrawledDomainAttribute(domain);

            string connectionString = WixossCardDatabase.CreateConnectionString(userId, password);
            string query = "SELECT DomainId, LastUpdated, LastConfirmed, Deleted FROM Crawling WHERE Domain = N'" + domain + "'";
            //List<CrawlingData> crawlingTable = new List<CrawlingData>();
            List<FixedCrawlingData> fixedCrawlingTable = new List<FixedCrawlingData>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                Clients.Caller.SetProgressSecondary(0, "データベースとの接続を開いています。");
                connection.Open();

                int numOfItems = 0;
                using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Crawling WHERE Domain = N'" + domain + "'", connection))
                {
                    numOfItems = int.Parse(command.ExecuteScalar().ToString());
                }

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        int counter = 0;

                        while (reader.Read())
                        {
                            counter++;
                            Clients.Caller.SetProgressSecondary(
                                (double)(counter - 1) / numOfItems,
                                numOfItems + "件のデータを検出しました。" + counter + "件目のデータを取り込んでいます。");

                            //crawlingTable.Add(new CrawlingData(reader));
                            FixedCrawlingData crawlingData = new FixedCrawlingData();
                            //crawlingData.SetDomainIdWithUrl(reader.GetString(0), this);
                            crawlingData.DomainId = reader.GetString(0);
                            crawlingData.Url = domainStrategy.ConvertDomainIdToUrl(crawlingData.DomainId);
                            //crawlingData.Content = reader.GetString(1);
                            crawlingData.LastUpdated = reader.GetDateTime(1);
                            crawlingData.LastConfirmed = reader.GetDateTime(2);
                            crawlingData.Deleted = reader.GetValue(3) as DateTime?;
                            fixedCrawlingTable.Add(crawlingData);
                        }
                    }
                }
                Clients.Caller.SetCrawlingTable(JsonConvert.SerializeObject(fixedCrawlingTable));
                Clients.Caller.SetProgressSecondary(1, "データベースとの接続を閉じています。");
                connection.Close();
            }

            Clients.Caller.SetProgressPrimary(1, "データベースからドメインが一致するデータを取得しました。");
            Clients.Caller.SetProgressSecondary(1, "");
        }

        //改修後
        /// <summary> Crawlingテーブルの中で、ドメインが一致するデータを全て取得します。 </summary>
        /// <param name="userId"> 接続で使用するユーザーのID。 </param>
        /// <param name="password"> 接続で使用するユーザーのパスワード。 </param>
        /// <returns>Crawlingテーブルの中で、ドメインが一致するデータのリスト。 </returns>
        private List<FixedCrawlingData> GetCrawlingTable(string userId, string password)
        {
            List<FixedCrawlingData> fixedCrawlingTable = new List<FixedCrawlingData>();

            using (SqlConnection connection = new SqlConnection(WixossCardDatabase.CreateConnectionString(userId, password)))
            {
                Clients.Caller.SetProgressSecondary(0, "データベースとの接続を開いています。");
                connection.Open();

                int numOfItems = 0;
                using (SqlCommand command = new SqlCommand(
                    "SELECT COUNT(*) FROM Crawling WHERE Domain = N'" + DomainAttribute.ToString() + "'",
                    connection))
                {
                    numOfItems = int.Parse(command.ExecuteScalar().ToString());
                }

                using (SqlCommand command = new SqlCommand(
                    "SELECT DomainId, Content, LastUpdated, LastConfirmed, Deleted FROM Crawling WHERE Domain = N'" + DomainAttribute.ToString() + "'",
                    connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        int counter = 0;

                        while (reader.Read())
                        {
                            counter++;
                            Clients.Caller.SetProgressSecondary(
                                (double)(counter - 1) / numOfItems,
                                numOfItems + "件のデータを検出しました。" + counter + "件目のデータを取り込んでいます。");

                            FixedCrawlingData crawlingData = new FixedCrawlingData();
                            crawlingData.DomainId = reader.GetString(0);
                            crawlingData.Url = domainStrategy.ConvertDomainIdToUrl(crawlingData.DomainId);
                            crawlingData.Content = reader.GetString(1);
                            crawlingData.LastUpdated = reader.GetDateTime(2);
                            crawlingData.LastConfirmed = reader.GetDateTime(3);
                            crawlingData.Deleted = reader.GetValue(4) as DateTime?;
                            fixedCrawlingTable.Add(crawlingData);
                        }
                    }
                }

                //Clients.Caller.SetCrawlingTable(JsonConvert.SerializeObject(fixedCrawlingTable));
                Clients.Caller.SetProgressSecondary(1, "データベースとの接続を閉じています。");
                connection.Close();
            }
            return fixedCrawlingTable;
        }

        //改修後
        /// <summary> 指定したドメインに存在する全ての <c>DomainId</c> を検索し、Crawlingテーブルと結合させてクライアントに返します。 </summary>
        /// <param name="userId"> 接続で使用するユーザーのID。 </param>
        /// <param name="password"> 接続で使用するユーザーのパスワード。 </param>
        /// <param name="domain"> ドメインを示す文字列。 </param>
        public void SearchAllDomainId(string userId, string password, string domain)
        {
            DomainAttribute = ConvertToCrawledDomainAttribute(domain);

            Clients.Caller.SetProgressPrimary(0, "データベースからドメインが一致するデータを検索します。");
            Clients.Caller.SetProgressSecondary(0, "");
            List<FixedCrawlingData> fixedCrawlingTable = GetCrawlingTable(userId, password);


            Clients.Caller.SetProgressPrimary(0, "カード情報の探索を開始します。");
            Clients.Caller.SetProgressSecondary(0, "");

            //DateTime confirmed = DateTime.Now;

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

            Clients.Caller.SetProgressPrimary(1, "カード情報の探索が完了しました。");
            Clients.Caller.SetProgressSecondary(1, "");

            Clients.Caller.SetCrawlingTable(JsonConvert.SerializeObject(fixedCrawlingTable));
            Clients.Caller.EndSearching();
        }

        /// <summary> 指定したドメインに存在する全ての <c>DomainId</c> を検索し、Crawlingテーブルと合体させて??クライアントに返します。 </summary>
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

            Clients.Caller.SetProgressPrimary(1, "カード情報の探索が完了しました。");
            Clients.Caller.SetProgressSecondary(1, "");

            Clients.Caller.SetCrawlingTable(JsonConvert.SerializeObject(fixedCrawlingTable));
            Clients.Caller.EndSearching();
        }


        //SQLを更新して結果をクライアントに返す
        public void UpdateCrawlingTable(string userId, string password, string domain, string fixedCrawlingTableJson)
        {
            Clients.Caller.SetProgressPrimary(0, "SQLの更新を開始します。");
            Clients.Caller.SetProgressSecondary(0, "");

            DomainAttribute = ConvertToCrawledDomainAttribute(domain);

            string connectionString = WixossCardDatabase.CreateConnectionString(userId, password);
            int counter = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                Clients.Caller.SetProgressSecondary(0, "SQLとの接続を開いています。");
                connection.Open();

                List<FixedCrawlingData> fixedCrawlingTable = JsonConvert.DeserializeObject<List<FixedCrawlingData>>(fixedCrawlingTableJson);
                foreach (FixedCrawlingData fixedCrawlingData in fixedCrawlingTable)
                {
                    counter++;
                    Clients.Caller.SetProgressPrimary((double)(counter - 1) / fixedCrawlingTable.Count, counter + "件目を更新中です。");
                    Clients.Caller.SetProgressSecondary(0, "クエリ文を作成しています。");

                    //削除されたデータは更新しない
                    if (fixedCrawlingData.Deleted.HasValue) { continue; }


                    //現在のカード情報を取得
                    DateTime timeAcquired = DateTime.Now;
                    string currentContent;
                    try { currentContent = domainStrategy.GetCurrentContent(fixedCrawlingData.DomainId); }
                    catch { currentContent = null; }

                    //StringBuilder query = new StringBuilder();
                    //query.Append(CreateQuery(fixedCrawlingData, currentContent, timeAcquired));
                    string query = CreateQuery(fixedCrawlingData, currentContent, timeAcquired);
                    if (string.IsNullOrEmpty(query)) { continue; }

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        Clients.Caller.SetProgressSecondary(0.5, "クエリを発行しています");
                        command.ExecuteNonQuery();
                        //command.ExecuteNonQueryAsync();
                    }

                    Clients.Caller.SetProgressSecondary(1, "クエリを発行しました。");

                }

                Clients.Caller.SetProgressSecondary(1, "SQLとの接続を閉じています。");
                connection.Close();
            }

            Clients.Caller.SetProgressPrimary(1, "SQLの更新が完了しました。");
            Clients.Caller.SetProgressSecondary(1, "");

            GetCrawlingTable(userId, password, domain);
        }

        /// <summary> <c>fixedCrawlingData</c> と現在のカード情報から、SQLに発行するクエリを取得します。 </summary>
        /// <param name="fixedCrawlingData"> クライアント側のJavaScriptのフォーマットに従っているCrawlingテーブルのデータ。 </param>
        /// <param name="currentContent"> 現在のカード情報を示す文字列。 </param>
        /// <param name="timeAcquired"> 現在のカード情報を取得した時刻。 </param>
        /// <returns> SQLに発行するクエリ。 </returns>
        private string CreateQuery(FixedCrawlingData fixedCrawlingData, string currentContent, DateTime timeAcquired)
        {
            StringBuilder query = new StringBuilder();

            //SQLにまだ存在しないデータの場合
            if (!fixedCrawlingData.LastUpdated.HasValue)
            {
                //SQLに存在せず、カード情報が取得できない場合(リンク切れなどが考えられる??)はクエリを発行しない
                if (string.IsNullOrWhiteSpace(currentContent)) { return null; }
                //カード情報を取得できた場合はデータを挿入するクエリを発行
                query.Append("INSERT INTO Crawling ( Domain, DomainId, Content, LastUpdated, LastConfirmed )");
                query.Append(" VALUES (");
                query.Append(" N'").Append(DomainAttribute.ToString()).Append("'");
                query.Append(", N'").Append(fixedCrawlingData.DomainId).Append("'");
                query.Append(", N'").Append(currentContent).Append("'");
                query.Append(", '").Append(timeAcquired/*.ToString("O")*/).Append("'");
                query.Append(", '").Append(timeAcquired).Append("'");
                query.Append(")");
                return query.ToString();
            }

            //以下、SQLに既に存在するデータの場合
            query.Append("UPDATE Crawling");

            //SQLに存在しているが、現在のカード情報が取得できなくなっている場合
            if (string.IsNullOrWhiteSpace(currentContent))
            {
                query.Append(" SET Deleted = '").Append(timeAcquired).Append("'");
                query.Append(CreateQueryWhere(fixedCrawlingData));
                return query.ToString();
            }

            //SQLに存在しているが、現在のカード情報と一致しない＝更新されている場合
            if (currentContent != fixedCrawlingData.Content)
            {
                query.Append(" SET Content = N'").Append(currentContent).Append("'");
                query.Append(", LastUpdated = '").Append(timeAcquired).Append("'");
                query.Append(", LastConfirmed = '").Append(timeAcquired).Append("'");
                query.Append(CreateQueryWhere(fixedCrawlingData));
                return query.ToString();
            }

            //SQLに存在しているが、現在のカード情報と一致＝更新されていない場合
            query.Append(" SET LastConfirmed = '").Append(timeAcquired).Append("'");
            query.Append(CreateQueryWhere(fixedCrawlingData));
            return query.ToString();
        }

        /// <summary> <c>fixedCrawlingData</c> から、SQLに発行するクエリの検索条件を取得します。 </summary>
        /// <param name="fixedCrawlingData"> クライアント側のJavaScriptのフォーマットに従っているCrawlingテーブルのデータ。 </param>
        /// <returns> SQLに発行するクエリの検索条件。 </returns>
        private string CreateQueryWhere(FixedCrawlingData fixedCrawlingData)
        {
            StringBuilder query = new StringBuilder(" WHERE Deleted IS NULL");
            query.Append(" AND Domain = N'").Append(DomainAttribute.ToString()).Append("'");
            query.Append(" AND DomainId = N'").Append(fixedCrawlingData.DomainId).Append("'");

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