#define 検索を20件で中止

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

using System.Threading.Tasks; //Task

using System.Text; //StringBuilder
using System.Text.RegularExpressions; //正規表現

using System.Data.SqlClient;
using Newtonsoft.Json;
//using MySql.Data.MySqlClient;
//using System.Data; //DataTable

using WixossSimulator.Sql;
using WixossSimulator.SugarSync;
//using WixossSimulator.Card;

using System.Net.Http;
using System.Net.Http.Headers;

namespace WixossSimulator.Crawling
{
    /// <summary> スクレイピングを行うサイトのドメインを示す列挙値を提供します。 </summary>
    public enum DomainKind
    {
        /// <summary> 不明。 </summary>
        Unknown,
        /// <summary> 公式ホームページ。 </summary>
        Official,
        /// <summary> 現在の時点で信頼があると思われるWiki。 </summary>
        Wiki,
    }

    /// <summary> ドメインを表す列挙値と、その種類に応じて行う処理を提供します。 </summary>
    public class DomainAttribute
    {
        private DomainKind value;
        private IDomainStrategy domainStrategy;

        /// <summary> ドメインを示す列挙値を取得します。 </summary>
        public DomainKind Value
        {
            get { return this.value; }
            set
            {
                this.value = value;
                switch (this.value)
                {
                    case DomainKind.Official:
                        domainStrategy = new OfficialDomainStrategy();
                        break;
                    case DomainKind.Wiki:
                        domainStrategy = new WikiDomainStrategy();
                        break;
                    default:
                        domainStrategy = new UnknownDomainStrategy();
                        break;
                }
            }
        }
        /// <summary> ドメインを示す文字列を取得します。 </summary>
        public string Text
        {
            get { return this.Value.ToString(); }
        }

        /// <summary> ドメインを示す列挙値を指定して、新しいインスタンスを初期化します。 </summary>
        /// <param name="domainKind"> ドメインを示す列挙値。 </param>
        public DomainAttribute(DomainKind domainKind) { this.Value = domainKind; }
        /// <summary> ドメインを示す文字列を指定して、新しいインスタンスを初期化します。 </summary>
        /// <param name="domainKind"> ドメインを示す文字列。 </param>
        public DomainAttribute(string domain) { this.Value = (DomainKind)Enum.Parse(typeof(DomainKind), domain); }

        /// <summary> そのドメインに存在するカード情報を識別する一意の <c>DomainId</c> の一覧を取得します。 </summary>
        /// <param name="crawler"> 進捗状況をクライアントに返すためのハブを示すクラス。 </param>
        /// <returns> カード情報を識別する一意の <c>DomainId</c> の一覧。 </returns>
        public HashSet<string> SearchAllDomainId(Crawler crawler) { return domainStrategy.SearchAllDomainId(crawler); }
        /// <summary> カード情報を識別する <c>DomainId</c> を、カード情報のアドレスを示すURLに変換します。 </summary>
        /// <param name="domainId"> カード情報を識別する <c>DomainId</c>。 </param>
        /// <returns> カード情報のアドレスを示すURL。 </returns>
        public string ToUrl(string domainId) { return domainStrategy.ConvertDomainIdToUrl(domainId); }
        /// <summary> カード情報のアドレスを示すURLを、カード情報を識別する <c>DomainId</c> に変換します。 </summary>
        /// <param name="url"> カード情報のアドレスを示すURL。 </param>
        /// <returns> カード情報を識別する <c>DomainId</c>。 </returns>
        public string ToDomainId(string url) { return domainStrategy.ConvertUrlToDomainId(url); }
        /// <summary> 現在のカード情報を取得します。 </summary>
        /// <param name="domainId"> カード情報を識別する <c>DomainId</c>。 </param>
        /// <returns> 現在のカード情報を示す文字列。 </returns>
        public string GetCurrentContent(string domainId) { return domainStrategy.GetCurrentContent(domainId); }
    }

    [HubName("crawler")]
    public class Crawler : Hub
    {
        protected DomainAttribute domainAttribute;

        [Obsolete]
        private DomainKind domainAttributeOld = DomainKind.Unknown;
        /// <summary> ドメインの種類に応じて呼び出されるメソッドを取得します。CrawledDomainAttribute に応じて自動的に設定されます。 </summary>
        [Obsolete]
        protected IDomainStrategy domainStrategyOld = new UnknownDomainStrategy();
        /// <summary> ドメインを示す列挙値を取得します。 </summary>
        [Obsolete]
        public DomainKind DomainAttributeOld
        {
            get { return this.domainAttributeOld; }
            set
            {
                this.domainAttributeOld = value;
                switch (this.domainAttributeOld)
                {
                    case DomainKind.Official:
                        domainStrategyOld = new OfficialDomainStrategy();
                        break;
                    case DomainKind.Wiki:
                        domainStrategyOld = new WikiDomainStrategy();
                        break;
                    default:
                        domainStrategyOld = new UnknownDomainStrategy();
                        break;
                }
            }
        }

        /// <summary> Crawlingテーブルのデータをクライアント側のJavaScriptのフォーマットに従って保持します。 </summary>
        [Obsolete]
        protected class FixedCrawlingData
        {
            [JsonProperty("domainId")]
            public string DomainId { get; set; }
            [JsonProperty("url")]
            public string Url { get; set; }
            //public string Content { get; set; }
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

        protected async Task<string> GetTokenAuthRequestString()
        {
            using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.AcceptCharset.Add(System.Net.Http.Headers.StringWithQualityHeaderValue.Parse("UTF-8"));
                    using (var response = await client.GetAsync("http://zeta00s.php.xdomain.jp/wixoss/sugarsync/token-auth-request.xml"))
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                }
                catch { return null; }
            }
        }

        protected async Task<TokenAuthRequestResource> GetTokenAuthRequest()
        {
            try 
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(TokenAuthRequestResource));
                using (var reader = new System.IO.StringReader(await GetTokenAuthRequestString())) { return (TokenAuthRequestResource)serializer.Deserialize(reader); }
            }
            catch { return null; }
        }

        public async Task Testing()
        {
            SugarSyncApiWrapper sugarsync = new SugarSyncApiWrapper();
            //if (!await sugarsync.CreateAccessTokenAsync(await GetTokenAuthRequest()))
            if (!await sugarsync.CreateAccessTokenAsync(await GetTokenAuthRequestString()))
            {
                Clients.Caller.SetProgressPrimary(1, "アクセストークンを取得できませんでした。");
                return;
            }

            //s.Authorization = "";
            //s.Expiration = DateTime.Parse("2014/10/14 1:15:04");


            var result = "";

            //string folder0 = "";
            //var result01 = await sugarsync.RetrieveUserInformationAsync();
            //var result02 = await sugarsync.RetrieveSyncFoldersCollectionAsync();
            //var result03 = await sugarsync.RetrieveFoldersAsync(folder0);
            //var result04 = await sugarsync.RetrieveFolderInformationAsync(folder0);
            //var result05 = await sugarsync.RetrieveFolderContentsAsync(folder0, RetrievingFolderType.None, 5, 10, RetrievingFolderOrder.LastModified);
            //var result06 = await sugarsync.RetrieveFolderContentsAsync(folder0, RetrievingFolderType.File, 5, 10, RetrievingFolderOrder.LastModified);
            //result += "----- 1. " + result01.BodyString;
            //result += "----- 2. " + result02.BodyString;
            //result += "----- 3. " + result03.BodyString;
            //result += "----- 4. " + result04.BodyString;
            //result += "----- 5. " + result05.BodyString;
            //result += "----- 6. " + result06.BodyString;

            string folderId1 = "";
            //string fileId1 = "";
            //var result11 = await sugarsync.CreateFolderAsync(folderId1, "testFolder2");
            //var result12 = await sugarsync.CreateFileAsync(folderId1, "test03.txt", "text/plain");
            //var result13 = await sugarsync.CopyFileAsync(folderId1, fileId1, "copy02.png");
            //var result14 = await sugarsync.CopyFileAsync(folderId1, fileId1, "copy01.png");
            //var result15 = await sugarsync.CreateFolderAsync(folderId1, "  ");
            var result16 = await sugarsync.RetrieveFolderContentsAsync(folderId1);
            //result += "----- 11. " + result11.StatusCode;
            //result += "----- 12. " + result12.StatusCode;
            //result += "----- 13. " + result13.StatusCode;
            //result += "----- 14. " + result14.StatusCode;
            //result += "----- 15. " + result15.StatusCode;
            result += "----- 16. " + result16.BodyString;

            //var result19 = await sugarsync.DeleteFolderAsync(
            //    sugarsync.GetContentId(result16.Body.Collection[0].ReferenceUrl));
            //result += "----- 19. " + result19.StatusCode;

            string folderId2 = "";
            string fileId2 = "";
            var result21 = await sugarsync.CreateFolderAsync(folderId2, "testFolder02");
            var result22 = await sugarsync.DeleteFolderAsync(
                sugarsync.GetContentId(result21.Headers.Location.AbsoluteUri));
            //var result13 = await sugarsync.CopyFileAsync(folderId1, fileId1, "copy02.png");
            //var result14 = await sugarsync.CopyFileAsync(folderId1, fileId1, "copy01.png");
            //var result15 = await sugarsync.RetrieveFolderContentsAsync(folderId1);
            //result += "----- 11. " + result21.ToString();
            //result += "----- 12. " + result12.ToString();
            //result += "----- 13. " + result13.ToString();
            //result += "----- 14. " + result14.ToString();
            //result += "----- 15. " + result15.BodyString;

            Clients.Caller.SetProgressPrimary(1, result);
        }

        public void Testing2()
        {
            string content = @"日本語テストコンテンツ
a

aa

aaa
改行込み";
            string postData = "mode=write&id=2&content=" + HttpUtility.UrlEncode(content);
            string result = HtmlStream.GetDocumentByPosting(postData, "http://zeta00s.php.xdomain.jp/wixoss/content-handler.php");
            Clients.Caller.SetProgressPrimary(1, result);
        }

        /// <summary> <c>CrawlerDomainAttribute</c> を示す文字列の一覧をクライアントに返します。 </summary>
        public void GetDomainList()
        {
            foreach (string s in Enum.GetNames(typeof(DomainKind)))
            {
                Clients.Caller.SetDomainAttribute(s);
            }
        }

        /// <summary> Crawlingテーブルの中で、ドメインが一致するデータを全て取得します。 </summary>
        /// <param name="userId"> 接続で使用するユーザーのID。 </param>
        /// <param name="password"> 接続で使用するユーザーのパスワード。 </param>
        /// <returns>Crawlingテーブルの中で、ドメインが一致するデータのリスト。 </returns>
        [Obsolete]
        private List<FixedCrawlingData> GetCrawlingTable(string userId, string password)
        {
            List<FixedCrawlingData> fixedCrawlingTable = new List<FixedCrawlingData>();

            using (SqlConnection connection = new SqlConnection(WixossCardDatabase.CreateConnectionString(userId, password)))
            {
                Clients.Caller.SetProgressSecondary(0, "データベースとの接続を開いています。");
                connection.Open();

                int numOfItems = 0;
                using (SqlCommand command = new SqlCommand(
                    "SELECT COUNT(*) FROM Crawling WHERE Domain = N'" + DomainAttributeOld.ToString() + "'",
                    connection))
                {
                    numOfItems = int.Parse(command.ExecuteScalar().ToString());
                }

                using (SqlCommand command = new SqlCommand(
                    "SELECT DomainId, LastUpdated, LastConfirmed, Deleted FROM Crawling WHERE Domain = N'" + DomainAttributeOld.ToString() + "'",
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
                            crawlingData.Url = domainStrategyOld.ConvertDomainIdToUrl(crawlingData.DomainId);
                            crawlingData.LastUpdated = reader.GetDateTime(1);
                            crawlingData.LastConfirmed = reader.GetDateTime(2);
                            crawlingData.Deleted = reader.GetValue(3) as DateTime?;
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

        /// <summary> Crawlingテーブルの中で、ドメインが一致するデータを全て取得します。 </summary>
        /// <returns>Crawlingテーブルの中で、ドメインが一致するデータのリスト。 </returns>
        private List<CrawlingData> GetCrawlingTable()
        {
            string query = "SELECT id, domain, domainId, lastUpdated, lastConfirmed, deleted FROM crawling WHERE domain = '" + domainAttribute.Text + "'";
            //string query = "SELECT * FROM crawling WHERE domain = '" + domainAttribute.Text + "'";
            //string postData = "query=" + HttpUtility.UrlEncode(query);

            Clients.Caller.SetProgressSecondary(0, "クエリを実行しています。");
            string json = HtmlStream.GetDocumentByPosting("query=" + HttpUtility.UrlEncode(query), "http://zeta00s.php.xdomain.jp/mysql/query.php");

            List<CrawlingData> crawlingTable = JsonConvert.DeserializeObject<List<CrawlingData>>(json);
            
            Clients.Caller.SetProgressSecondary(1, "クエリを実行しました。");
            return crawlingTable ?? new List<CrawlingData>(0);
        }

        /// <summary> 指定したドメインに存在する全ての <c>DomainId</c> を検索し、Crawlingテーブルと結合させて、そのテーブルをクライアントに返します。 </summary>
        /// <param name="domain"> ドメインを示す文字列。 </param>
        public void SearchAllDomainId(string domain)
        {
            domainAttribute = new DomainAttribute(domain);

            Clients.Caller.SetProgressPrimary(0, "データベースからドメインが一致するデータを検索します。");
            Clients.Caller.SetProgressSecondary(0, "");
            List<CrawlingData> crawlingTable = GetCrawlingTable();


            Clients.Caller.SetProgressPrimary((double)1 / 2, "ドメイン内からカード情報を探索します。");
            Clients.Caller.SetProgressSecondary(0, "");

            foreach (string domainId in domainAttribute.SearchAllDomainId(this))
            {
                if (!crawlingTable.Any(f => f.DomainId == domainId))
                {
                    CrawlingData crawlingData = new CrawlingData();
                    crawlingData.Domain = domainAttribute.Text;
                    crawlingData.DomainId = domainId;
                    crawlingTable.Add(crawlingData);
                }
            }

            Clients.Caller.SetProgressPrimary(1, "カード情報の探索が完了しました。");
            Clients.Caller.SetProgressSecondary(1, "");

            Clients.Caller.SetCrawlingTable(JsonConvert.SerializeObject(crawlingTable));
            Clients.Caller.EndSearching();
        }

        /// <summary> 指定したドメインに存在する全ての <c>DomainId</c> を検索し、Crawlingテーブルと結合させて、そのテーブルをクライアントに返します。 </summary>
        /// <param name="userId"> 接続で使用するユーザーのID。 </param>
        /// <param name="password"> 接続で使用するユーザーのパスワード。 </param>
        /// <param name="domain"> ドメインを示す文字列。 </param>
        [Obsolete]
        public void SearchAllDomainIdOld(string userId, string password, string domain)
        {
            DomainAttributeOld = ConvertToCrawledDomainAttribute(domain);

            Clients.Caller.SetProgressPrimary(0, "データベースからドメインが一致するデータを検索します。");
            Clients.Caller.SetProgressSecondary(0, "");
            List<FixedCrawlingData> fixedCrawlingTable = GetCrawlingTable(userId, password);


            Clients.Caller.SetProgressPrimary((double)1 / 2, "ドメイン内からカード情報を探索します。");
            Clients.Caller.SetProgressSecondary(0, "");

            foreach (string domainId in domainStrategyOld.SearchAllDomainId(this))
            {
                if (!fixedCrawlingTable.Any(f => f.DomainId == domainId))
                {
                    FixedCrawlingData fixedCrawlingData = new FixedCrawlingData();
                    fixedCrawlingData.DomainId = domainId;
                    fixedCrawlingData.Url = domainStrategyOld.ConvertDomainIdToUrl(domainId);
                    fixedCrawlingTable.Add(fixedCrawlingData);
                }
            }

            Clients.Caller.SetProgressPrimary(1, "カード情報の探索が完了しました。");
            Clients.Caller.SetProgressSecondary(1, "");

            Clients.Caller.SetCrawlingTable(JsonConvert.SerializeObject(fixedCrawlingTable));
            Clients.Caller.EndSearching();
        }

        /// <summary> クライアントのテーブルを基にデータベースを更新して、更新されたCrawlingテーブルをクライアントに返します。 </summary>
        /// <param name="userId"> 接続で使用するユーザーのID。 </param>
        /// <param name="password"> 接続で使用するユーザーのパスワード。 </param>
        /// <param name="domain"> ドメインを示す文字列。 </param>
        /// <param name="crawlingTableJson"> クライアント側のJavaScriptのフォーマットに従っているCrawlingテーブルのデータ。 </param>
        public void UpdateCrawlingTableNew(string userId, string password, string domain, string crawlingTableJson)
        {
            Clients.Caller.SetProgressPrimary(0, "データベースの更新を開始します。");
            Clients.Caller.SetProgressSecondary(0, "");

            domainAttribute = new DomainAttribute(domain);

            //Clients.Caller.SetProgressSecondary(0, "データベースとの接続を開いています。");
            List<CrawlingData> crawlingTable = JsonConvert.DeserializeObject<List<CrawlingData>>(crawlingTableJson);
            int counter = 0;

            foreach (CrawlingData crawlingData in crawlingTable)
            {
                counter++;
                Clients.Caller.SetProgressPrimary((double)(counter - 1) / crawlingTable.Count, counter + "件目を更新中です。");

                //削除されたデータは更新しない
                if (crawlingData.Deleted.HasValue) { continue; }

                string currentContent = null;

                Clients.Caller.SetProgressSecondary((double)1 / 4, "ドメイン内のカード情報を取得しています。");
                DateTime timeAcquired = DateTime.Now;

                Clients.Caller.SetProgressSecondary((double)2 / 4, "クエリ文を作成しています。");

                string query = CreateQuery(crawlingData, timeAcquired);
                if (string.IsNullOrEmpty(query)) { continue; }

                //using (SqlCommand command = new SqlCommand(query, connection))
                //{
                //    Clients.Caller.SetProgressSecondary((double)3 / 4, "クエリを発行しています");
                //    command.ExecuteNonQuery();
                //    //command.ExecuteNonQueryAsync();
                //}

                Clients.Caller.SetProgressSecondary(1, "クエリを発行しました。");
            }

            Clients.Caller.SetProgressSecondary(1, "データベースとの接続を閉じています。");

            Clients.Caller.SetCrawlingTable(JsonConvert.SerializeObject(GetCrawlingTable()));

            Clients.Caller.SetProgressPrimary(1, "データベースの更新が完了しました。");
            Clients.Caller.SetProgressSecondary(1, "");
        }

        /// <summary> <c>fixedCrawlingData</c> とテーブル内、ドメイン内それぞれのカード情報から、SQL Server に発行するクエリを取得します。 </summary>
        /// <param name="crawlingData"> クライアント側のJavaScriptのフォーマットに従っているCrawlingテーブルのデータ。 </param>
        /// <param name="timeAcquired"> 現在のカード情報を取得した時刻。 </param>
        /// <returns> SQL Server に発行するクエリ。 </returns>
        private string CreateQuery(CrawlingData crawlingData, DateTime timeAcquired)
        {
            StringBuilder query = new StringBuilder();

            string currentContent = null;
            try { currentContent = domainAttribute.GetCurrentContent(crawlingData.DomainId); }
            catch { currentContent = null; }

            //テーブル内にまだ存在しないデータの場合
            if (!crawlingData.LastUpdated.HasValue)
            {
                //データベースに存在せず、カード情報が取得できない場合(リンク切れなどが考えられる??)はクエリを発行しない
                if (string.IsNullOrWhiteSpace(currentContent)) { return null; }
                //カード情報を取得できた場合はデータを挿入するクエリを発行
                query.Append("INSERT INTO Crawling ( Domain, DomainId, Content, LastUpdated, LastConfirmed )");
                query.Append(" VALUES (");
                query.Append(" N'").Append(domainAttribute.Text).Append("'");
                query.Append(", N'").Append(crawlingData.DomainId).Append("'");
                query.Append(", N'").Append(currentContent).Append("'");
                query.Append(", '").Append(timeAcquired).Append("'");
                query.Append(", '").Append(timeAcquired).Append("'");
                query.Append(")");
                return query.ToString();
            }

            //以下、データベースに既に存在するデータの場合

            //テーブル内のカード情報を取得
            string tableContent = null;


            query.Append("UPDATE Crawling");

            //現在のカード情報がドメイン上から取得できなくなっている場合
            if (string.IsNullOrWhiteSpace(currentContent))
            {
                query.Append(" SET Deleted = '").Append(timeAcquired).Append("'");
                query.Append(CreateQueryWhere(crawlingData));
                return query.ToString();
            }

            //現在のカード情報とデータベースの情報が一致しない＝更新されている場合
            if (currentContent != tableContent)
            {
                query.Append(" SET Content = N'").Append(currentContent).Append("'");
                query.Append(", LastUpdated = '").Append(timeAcquired).Append("'");
                query.Append(", LastConfirmed = '").Append(timeAcquired).Append("'");
                query.Append(CreateQueryWhere(crawlingData));
                return query.ToString();
            }

            //現在のカード情報とデータベースの情報が一致する＝更新されていない場合
            query.Append(" SET LastConfirmed = '").Append(timeAcquired).Append("'");
            query.Append(CreateQueryWhere(crawlingData));
            return query.ToString();
        }

        /// <summary> <c>fixedCrawlingData</c> から、SQL Server に発行するクエリの検索条件を取得します。 </summary>
        /// <param name="crawlingData"> クライアント側のJavaScriptのフォーマットに従っているCrawlingテーブルのデータ。 </param>
        /// <returns> SQL Server に発行するクエリの検索条件。 </returns>
        private string CreateQueryWhere(CrawlingData crawlingData)
        {
            StringBuilder query = new StringBuilder(" WHERE Deleted IS NULL");
            query.Append(" AND domain = '").Append(domainAttribute.Text).Append("'");
            query.Append(" AND domainId = '").Append(crawlingData.DomainId).Append("'");

            return query.ToString();
        }

        /// <summary> クライアントのテーブルを基にデータベースを更新して、更新されたCrawlingテーブルをクライアントに返します。 </summary>
        /// <param name="userId"> 接続で使用するユーザーのID。 </param>
        /// <param name="password"> 接続で使用するユーザーのパスワード。 </param>
        /// <param name="domain"> ドメインを示す文字列。 </param>
        /// <param name="fixedCrawlingTableJson"> クライアント側のJavaScriptのフォーマットに従っているCrawlingテーブルのデータ。 </param>
        [Obsolete]
        public void UpdateCrawlingTable(string userId, string password, string domain, string fixedCrawlingTableJson)
        {
            Clients.Caller.SetProgressPrimary(0, "データベースの更新を開始します。");
            Clients.Caller.SetProgressSecondary(0, "");

            DomainAttributeOld = ConvertToCrawledDomainAttribute(domain);

            string connectionString = WixossCardDatabase.CreateConnectionString(userId, password);
            int counter = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                Clients.Caller.SetProgressSecondary(0, "データベースとの接続を開いています。");
                connection.Open();

                List<FixedCrawlingData> fixedCrawlingTable = JsonConvert.DeserializeObject<List<FixedCrawlingData>>(fixedCrawlingTableJson);
                foreach (FixedCrawlingData fixedCrawlingData in fixedCrawlingTable)
                {
                    counter++;
                    Clients.Caller.SetProgressPrimary((double)(counter - 1) / fixedCrawlingTable.Count, counter + "件目を更新中です。");

                    //削除されたデータは更新しない
                    if (fixedCrawlingData.Deleted.HasValue) { continue; }

                    string tableContent = null;
                    string currentContent = null;

                    Clients.Caller.SetProgressSecondary(0, "テーブル内のカード情報を取得しています。");
                    using (SqlCommand command = new SqlCommand(
                        "SELECT Content FROM Crawling WHERE Domain = N'" + DomainAttributeOld.ToString() + "' And DomainId = N'" + fixedCrawlingData.DomainId + "'",
                        connection))
                    {
                        tableContent = command.ExecuteScalar() as string; //NULLチェック
                        //ただし CreateQuery メソッド内で１回使われるのみ
                    }

                    Clients.Caller.SetProgressSecondary((double)1 / 4, "ドメイン内のカード情報を取得しています。");
                    DateTime timeAcquired = DateTime.Now;
                    try { currentContent = domainStrategyOld.GetCurrentContent(fixedCrawlingData.DomainId); }
                    catch { currentContent = null; }

                    Clients.Caller.SetProgressSecondary((double)2 / 4, "クエリ文を作成しています。");
                    string query = CreateQuery(fixedCrawlingData, tableContent, currentContent, timeAcquired);
                    if (string.IsNullOrEmpty(query)) { continue; }

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        Clients.Caller.SetProgressSecondary((double)3 / 4, "クエリを発行しています");
                        command.ExecuteNonQuery();
                        //command.ExecuteNonQueryAsync();
                    }

                    Clients.Caller.SetProgressSecondary(1, "クエリを発行しました。");
                }

                Clients.Caller.SetProgressSecondary(1, "データベースとの接続を閉じています。");
                connection.Close();
            }

            Clients.Caller.SetCrawlingTable(JsonConvert.SerializeObject(GetCrawlingTable(userId, password)));

            Clients.Caller.SetProgressPrimary(1, "データベースの更新が完了しました。");
            Clients.Caller.SetProgressSecondary(1, "");
        }

        /// <summary> <c>fixedCrawlingData</c> とテーブル内、ドメイン内それぞれのカード情報から、SQL Server に発行するクエリを取得します。 </summary>
        /// <param name="fixedCrawlingData"> クライアント側のJavaScriptのフォーマットに従っているCrawlingテーブルのデータ。 </param>
        /// <param name="currentContent"> 現在のカード情報を示す文字列。 </param>
        /// <param name="timeAcquired"> 現在のカード情報を取得した時刻。 </param>
        /// <returns> SQL Server に発行するクエリ。 </returns>
        [Obsolete]
        private string CreateQuery(FixedCrawlingData fixedCrawlingData, string tableContent, string currentContent, DateTime timeAcquired)
        {
            StringBuilder query = new StringBuilder();

            //テーブル内にまだ存在しないデータの場合
            if (!fixedCrawlingData.LastUpdated.HasValue)
            {
                //データベースに存在せず、カード情報が取得できない場合(リンク切れなどが考えられる??)はクエリを発行しない
                if (string.IsNullOrWhiteSpace(currentContent)) { return null; }
                //カード情報を取得できた場合はデータを挿入するクエリを発行
                query.Append("INSERT INTO Crawling ( Domain, DomainId, Content, LastUpdated, LastConfirmed )");
                query.Append(" VALUES (");
                query.Append(" N'").Append(DomainAttributeOld.ToString()).Append("'");
                query.Append(", N'").Append(fixedCrawlingData.DomainId).Append("'");
                query.Append(", N'").Append(currentContent).Append("'");
                query.Append(", '").Append(timeAcquired).Append("'");
                query.Append(", '").Append(timeAcquired).Append("'");
                query.Append(")");
                return query.ToString();
            }

            //以下、データベースに既に存在するデータの場合
            query.Append("UPDATE Crawling");

            //現在のカード情報がドメイン上から取得できなくなっている場合
            if (string.IsNullOrWhiteSpace(currentContent))
            {
                query.Append(" SET Deleted = '").Append(timeAcquired).Append("'");
                query.Append(CreateQueryWhere(fixedCrawlingData));
                return query.ToString();
            }

            //現在のカード情報とデータベースの情報が一致しない＝更新されている場合
            if (currentContent != tableContent)
            {
                query.Append(" SET Content = N'").Append(currentContent).Append("'");
                query.Append(", LastUpdated = '").Append(timeAcquired).Append("'");
                query.Append(", LastConfirmed = '").Append(timeAcquired).Append("'");
                query.Append(CreateQueryWhere(fixedCrawlingData));
                return query.ToString();
            }

            //現在のカード情報とデータベースの情報が一致する＝更新されていない場合
            query.Append(" SET LastConfirmed = '").Append(timeAcquired).Append("'");
            query.Append(CreateQueryWhere(fixedCrawlingData));
            return query.ToString();
        }

        /// <summary> <c>fixedCrawlingData</c> から、SQL Server に発行するクエリの検索条件を取得します。 </summary>
        /// <param name="fixedCrawlingData"> クライアント側のJavaScriptのフォーマットに従っているCrawlingテーブルのデータ。 </param>
        /// <returns> SQL Server に発行するクエリの検索条件。 </returns>
        [Obsolete]
        private string CreateQueryWhere(FixedCrawlingData fixedCrawlingData)
        {
            StringBuilder query = new StringBuilder(" WHERE Deleted IS NULL");
            query.Append(" AND Domain = N'").Append(DomainAttributeOld.ToString()).Append("'");
            query.Append(" AND DomainId = N'").Append(fixedCrawlingData.DomainId).Append("'");

            return query.ToString();
        }

        /// <summary> 文字列からドメインを示す列挙値に変換します。 </summary>
        /// <param name="domain"> ドメインを示す文字列。 </param>
        /// <returns> ドメインを示す列挙値。 </returns>
        [Obsolete]
        private DomainKind ConvertToCrawledDomainAttribute(string domain)
        {
            return (DomainKind)Enum.Parse(typeof(DomainKind), domain);
        }
    }








    public interface IDomainStrategy
    {
        /// <summary>ドメインを示すURLを取得します。 </summary>
        string Url { get; }

        HashSet<string> SearchAllDomainId(Crawler crawler);
        string ConvertDomainIdToUrl(string domainId);
        string ConvertUrlToDomainId(string url);
        string GetCurrentContent(string domainId);
        ////Cardクラスの型に一致するように変換する
        //Card.Card ConvertToCard(string html);
    }

    public class UnknownDomainStrategy : IDomainStrategy
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

    public class OfficialDomainStrategy : IDomainStrategy
    {
        public string Url { get { return "http://www.takaratomy.co.jp/"; } }

        public HashSet<string> SearchAllDomainId(Crawler crawler)
        {
            HashSet<string> results = new HashSet<string>();
            Regex regexCardUrl = new Regex(@"href\s*=\s*""(?<relativePath>.*?)""",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            crawler.Clients.Caller.SetProgressSecondary(0, "カードの検索結果を取得中です。");

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

    public class WikiDomainStrategy : IDomainStrategy
    {
        public string Url { get { return "http://wixoss.81.la/"; } }

        HashSet<string> IDomainStrategy.SearchAllDomainId(Crawler crawler)
        {
            HashSet<string> results = new HashSet<string>();
            Regex regexCardUrl = new Regex(@"<a[^>]*href=""(?<absolutePath>http://wixoss.81.la/\?%A1%D4.*?)""[^>]*?>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            crawler.Clients.Caller.SetProgressSecondary(0, "ページリストを取得中です。");

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
            content = Regex.Match(content, @"<td valign=""top"">(.*)</td>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline).Groups[1].Value;
            return Regex.Replace(content, @"(<a[^>]*title=""[^""]*) \(.*?\)(""[^>]*>.*?</a>)", "$1$2",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

        }
    }
}