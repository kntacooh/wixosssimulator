#define Authorizationを変更可能にする

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Net;
using System.IO;
using System.Text; //StringBuilder

using System.Xml; //XmlDocument
using System.Xml.Linq; //XDocument
using System.Xml.Serialization; //XmlSerializer
using System.Collections.Specialized; //NameValueCollection
//using DropNet;

namespace WixossSimulator.SugarSync
{
    //retrieveの訳
    //1.検索する→searchの意味合いを思い起こさせる?
    //2.取得する→getっぽい?別にいい?
    //3.読み出す、4.取り出す、……というかfetchと違うのか
    /// <summary> WixossSimulator のための SugarSync API のラッパーを提供します？ </summary>
    public class SugarsyncApiWrapper
    {
#if(Authorizationを変更可能にする)
        public string Authorization { get; set; }
        public DateTime Expiration { get; set; }
        public long UserId { get; set; }
#else
        public string Authorization { get; private set; }
        public DateTime Expiration { get; private set; }
        public long UserId { get; private set; }
#endif

        public SugarsyncApiWrapper()
        {
            Expiration = DateTime.Now;
            Authorization = null;
            UserId = -1;
        }

        /// <summary>
        /// Authorizationのみ取得、レスポンスボディは無視。
        /// (レスポンスボティについては https://www.sugarsync.com/dev/api/auth-resource.html を参照)
        /// </summary>
        /// <returns></returns>
        public bool CreateAccessTokenOld()
        {
            string tokenAuthRequest;
            using (WebClient client = new WebClient())
            {
                Uri uri = new Uri("http://zeta00s.php.xdomain.jp/wixoss/sugarsync/token-auth-request.xml");
                try { tokenAuthRequest = client.DownloadString(uri); }
                catch { tokenAuthRequest = null; }
            }
            if (string.IsNullOrWhiteSpace(tokenAuthRequest)) { return false; }

            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                client.Headers.Add("Content-Type", "application/xml");
                try
                {
                    Expiration = DateTime.Now.AddHours(1);
                    client.UploadString("https://api.sugarsync.com/authorization", tokenAuthRequest);
                    Authorization = client.ResponseHeaders["Location"];
                }
                catch
                {
                    Expiration = DateTime.Now;
                    Authorization = null;
                    return false;
                }
            }

            return true;
        }

        public bool CreateAccessToken()
        {
            string tokenAuthRequest;
            using (WebClient client = new WebClient())
            {
                try { tokenAuthRequest = client.DownloadString("http://zeta00s.php.xdomain.jp/wixoss/sugarsync/token-auth-request.xml"); }
                //try { tokenAuthRequest = client.DownloadString(new Uri("http://zeta00s.php.xdomain.jp/wixoss/sugarsync/token-auth-request.xml")); }
                catch { tokenAuthRequest = null; }
            }
            if (string.IsNullOrWhiteSpace(tokenAuthRequest)) { return false; }

            try
            {
                Expiration = DateTime.MaxValue;
                var response = new HttpResponseByPostMethod<AccessTokenResource>(this, "https://api.sugarsync.com/authorization", tokenAuthRequest);

                Expiration = response.Body.Expiration;
                Authorization = response.Header["Location"];
                UserId = response.Body.UserId;
            }
            catch
            {
                Expiration = DateTime.Now;
                Authorization = null;
                UserId = -1;
                return false;
            }

            //using (WebClient client = new WebClient())
            //{
            //    client.Encoding = System.Text.Encoding.UTF8;
            //    client.Headers.Add("Content-Type", "application/xml");
            //    try
            //    {
            //        Expiration = DateTime.Now.AddHours(1);
            //        client.UploadString("https://api.sugarsync.com/authorization", tokenAuthRequest);
            //        Authorization = client.ResponseHeaders["Location"];
            //    }
            //    catch
            //    {
            //        Expiration = DateTime.Now;
            //        Authorization = null;
            //        return false;
            //    }
            //}

            return true;
        }

        /// <summary>
        /// SugarSyncユーザーについての情報を取得します。
        /// https://www.sugarsync.com/dev/api/method/get-user-info.html
        /// </summary>
        /// <param name="userId"> ユーザーID。 </param>
        /// <returns></returns>
        public HttpResponseByGetMethod<UserResource> RetrieveUserInformation(long userId)
        {
            return new HttpResponseByGetMethod<UserResource>(this, "https://api.sugarsync.com/user/" + userId.ToString());
        }

        /// <summary>
        /// ユーザーのアカウント中にあるworkspaceについての情報を取得します。
        /// https://www.sugarsync.com/dev/api/method/get-workspaces.html
        /// </summary>
        /// <param name="userId"> ユーザーID。 </param>
        /// <returns></returns>
        public HttpResponseByGetMethod<WorkspacesCollectionResource> RetrieveWorkspacesCollection(long userId)
        {
            return new HttpResponseByGetMethod<WorkspacesCollectionResource>(this, "https://api.sugarsync.com/user/" + userId.ToString() + "/workspaces/contents");
        }

        /// <summary>
        /// workspaceについての情報を取得します。
        /// https://www.sugarsync.com/dev/api/method/get-workspace-info.html
        /// </summary>
        /// <param name="workspace"> workspaceを示すID? </param>
        /// <returns></returns>
        public HttpResponseByGetMethod<WorkspaceResource> RetrieveWorkspaceCollection(string workspace)
        {
            return new HttpResponseByGetMethod<WorkspaceResource>(this, "https://api.sugarsync.com/workspace/" + workspace);
        }

        /// <summary>
        /// workspaceのコンテンツを取得します。
        /// https://www.sugarsync.com/dev/api/method/get-workspace-contents.html
        /// </summary>
        /// <param name="workspace"> workspaceを示すID? </param>
        /// <returns></returns>
        public HttpResponseByGetMethod<FoldersCollectionResource> RetrieveWorkspaceContents(string workspace)
        {
            return new HttpResponseByGetMethod<FoldersCollectionResource>(this, "https://api.sugarsync.com/workspace/" + workspace + "/contents");
        }

        /// <summary>
        /// workspaceの属性を更新します。
        /// https://www.sugarsync.com/dev/api/method/update-workspace-name.html
        /// </summary>
        /// <param name="workspace"> workspaceを示すID? </param>
        /// <param name="workspaceResource"> workspaceを示すクラス </param>
        /// <returns></returns>
        public bool UpdateWorkspaceInformation(string workspace, WorkspaceResource workspaceResource)
        {
            //workspaceはworkspaceResourceのDsidで代用可能?
            throw new NotImplementedException("未実装です。");
        }

        /// <summary>
        /// アカウントに含まれる同期フォルダについての情報を取得します。
        /// https://www.sugarsync.com/dev/api/method/get-syncfolders.html
        /// </summary>
        /// <param name="userId"> ユーザーID。 </param>
        /// <param name="start">
        /// Specifies the index within the indexed sequence of sync folders in the user's account where the client wants workspaces to be retrieved.
        /// The index starts at origin 0. The default value is 0.
        /// </param>
        /// <param name="max">
        /// The maximum number of sync folders, beginning with the sync folder at the start index, that the client wants listed in the response.
        /// If this parameter is not specified, no limit is placed on the number of retrieved sync folders.
        /// </param>
        /// <returns>  </returns>
        public HttpResponseByGetMethod<FoldersCollectionResource> RetrieveSyncFoldersCollection(long userId, long start = 0, long max = 0)
        {
            NameValueCollection getQuery = new NameValueCollection();
            if (start > 0) { getQuery.Add("start", start.ToString()); }
            if (max > 0) { getQuery.Add("max", max.ToString()); }
            return new HttpResponseByGetMethod<FoldersCollectionResource>(this, "https://api.sugarsync.com/user/" + userId.ToString() + "/folders/contents", getQuery);
        }

        /// <summary>
        /// 親フォルダに含まれるフォルダを取得します。(workspaceは <c> RetrieveWorkspaceContents </c> メソッドを使用してください?)
        /// https://www.sugarsync.com/dev/api/method/get-folders.html
        /// </summary>
        /// <param name="folder"> 親フォルダを示すID? </param>
        /// <param name="start"> 
        /// Specifies the index within the indexed sequence of folders where the client wants folders to be retrieved.
        /// The index starts at origin 0. The default value is 0.
        /// </param>
        /// <param name="max"> 
        /// The maximum number of folders, beginning with the folder at the start index, that the client wants listed in the response.
        /// If this parameter is not specified, no limit is placed on the number of retrieved folders.
        /// </param>
        /// <returns></returns>
        public HttpResponseByGetMethod<FoldersCollectionResource> RetrieveFolders(string folder, long start = 0, long max = 0)
        {
            NameValueCollection getQuery = new NameValueCollection();
            getQuery.Add("type", RetrievingFolderType.Folder.ToApiString());
            if (start > 0) { getQuery.Add("start", start.ToString()); }
            if (max > 0) { getQuery.Add("max", max.ToString()); }
            return new HttpResponseByGetMethod<FoldersCollectionResource>(this, "https://api.sugarsync.com/folder/" + folder + "/contents", getQuery);
        }

        /// <summary>
        /// フォルダについての情報を取得します。
        /// https://www.sugarsync.com/dev/api/method/get-folder-info.html
        /// </summary>
        /// <param name="folder"> フォルダを示すID? </param>
        /// <returns></returns>
        public HttpResponseByGetMethod<FolderResource> RetrieveFolderInformation(string folder)
        {
            return new HttpResponseByGetMethod<FolderResource>(this, "https://api.sugarsync.com/folder/" + folder);
        }

        /// <summary>
        /// フォルダのコンテンツを取得します。
        /// https://www.sugarsync.com/dev/api/method/get-folder-contents.html
        /// </summary>
        /// <param name="folder"> フォルダを示すID? </param>
        /// <param name="type">
        /// The type of folder contents to be listed in the response: folders or files.
        /// If no object of the listed type is in the folder, no folder contents are listed in the response.
        /// If no type is specified, both folders and files contained in the folder are listed in the response.
        /// </param>
        /// <param name="start">
        /// The index within the indexed sequence of objects in the folder to start listing folder contents.
        /// The default is 0, meaning that objects in the folder are listed starting with the first object in the sequence.
        /// </param>
        /// <param name="max">
        /// The maximum number of objects, beginning with the object at the start index, that the client wants listed in the response.
        /// If the folder contains fewer objects of the requested type than the specified maximum, the smaller number of objects is listed.
        /// The default is 500, meaning that no more than 500 objects in the folder will be listed.
        /// </param>
        /// <param name="order">
        /// The value to be used for sorting the contents, as follows:
        /// <list type="bullet">
        ///     <item> name: The display name of the items </item>
        ///     <item> last_modified: The last-modified date (if available) of the items </item>
        ///     <item> size: The size (in bytes) of the items </item>
        ///     <item> extension: The filename extension (if available) of the items </item>
        /// </list>
        /// </param>
        /// <returns></returns>
        public HttpResponseByGetMethod<FoldersCollectionResource> RetrieveFolderContents(
            string folder,
            RetrievingFolderType type = RetrievingFolderType.None,
            long start = 0, long max = 0,
            RetrievingFolderOrder order = RetrievingFolderOrder.None)
        {
            NameValueCollection getQuery = new NameValueCollection();
            if (type != RetrievingFolderType.None) { getQuery.Add("type", type.ToApiString()); }
            if (start > 0) { getQuery.Add("start", start.ToString()); }
            if (max > 0) { getQuery.Add("max", max.ToString()); }
            if (order!= RetrievingFolderOrder.None) { getQuery.Add("order", order.ToApiString()); }
            return new HttpResponseByGetMethod<FoldersCollectionResource>(this, "https://api.sugarsync.com/folder/" + folder + "/contents", getQuery);
        }

        /// <summary>
        /// フォルダの内部に別のフォルダを作成します。
        /// https://www.sugarsync.com/dev/api/method/create-folder.html
        /// </summary>
        /// <param name="folder"> 親フォルダを示すID? </param>
        /// <param name="createdFolderName"> 新しく作成されるフォルダの名前。 </param>
        /// <returns></returns>
        public bool CreateFolder(string folder, string createdFolderName)
        {
            throw new NotImplementedException("未実装です。");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folder"> 親フォルダを示すID? </param>
        /// <param name="createdFolderName"> 新しく作成されるフォルダの名前。 </param>
        /// <returns></returns>
        public bool CreateFile(string folder, string createdFolderName)
        {
            throw new NotImplementedException("未実装です。");
        }
    }

    /// <summary> HTTPサーバーにGETメソッド(GETリクエスト)でデータを送信して、そのレスポンスを格納します？ </summary>
    /// <typeparam name="T"> レスポンスボディを格納するプロパティの型。 </typeparam>
    public class HttpResponseByGetMethod<T> where T : class, new()
    {
        public WebHeaderCollection Header { get; private set; }
        public T Body { get; private set; }
        public string BodyString { get; private set; }

        /// <summary>  </summary>
        /// <param name="wrapper">  </param>
        /// <param name="url">  </param>
        public HttpResponseByGetMethod(SugarsyncApiWrapper wrapper, string url) : this(wrapper, url, new NameValueCollection()) { }

        /// <summary>  </summary>
        /// <param name="wrapper">  </param>
        /// <param name="url">  </param>
        /// <param name="getQuery">  </param>
        public HttpResponseByGetMethod(SugarsyncApiWrapper wrapper, string url, NameValueCollection getQuery)
        {
            if (wrapper.Expiration < DateTime.Now) { return; }

            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                client.Headers.Add("Authorization", wrapper.Authorization);
                client.QueryString = getQuery;

                try
                {
                    BodyString = client.DownloadString(url);
                }
                catch
                {

                }
                //using (Stream stream = client.OpenRead(url))
                //{
                //    try { using (StreamReader reader = new StreamReader(stream)) { BodyString = reader.ReadToEnd(); } }
                //    catch { BodyString = null; }
                //}

                try
                {
                    Type t = typeof(T);
                    if (t == typeof(XDocument)) { Body = XDocument.Parse(BodyString) as T; }
                    else if (t == typeof(XmlDocument)) 
                    {
                        XmlDocument document = new XmlDocument();
                        document.LoadXml(BodyString);
                        Body = document as T;
                    }
                    else
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(T));
                        using (StringReader reader = new StringReader(BodyString)) { Body = (T)serializer.Deserialize(reader); }
                    }
                }
                catch { Body = null; }

                Header = client.ResponseHeaders;
            }
        }
    }

    /// <summary> HTTPサーバーにPOSTメソッド(GETリクエスト)でXMLを送信して、そのレスポンスを格納します？ </summary>
    /// <typeparam name="T"> レスポンスボディを格納するプロパティの型。 </typeparam>
    public class HttpResponseByPostMethod<T> where T : class, new()
    {
        public WebHeaderCollection Header { get; private set; }
        public T Body { get; private set; }
        public string BodyString { get; private set; }

        /// <summary>  </summary>
        /// <param name="wrapper"></param>
        /// <param name="url"></param>
        /// <param name="requestBody"> XML宣言を含まないリクエストボディを表すオブジェクト。 </param>
        public HttpResponseByPostMethod(SugarsyncApiWrapper wrapper, string url, Object requestBody)
        {
            if (wrapper.Expiration < DateTime.Now) { return; }
            
            //リクエストボディの文字列化
            //string head = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>"; //<?xml version="1.0" encoding="UTF-8" ?>
            StringBuilder requestBodyBuilder = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            try
            {
                Type requestBodyType = requestBody.GetType();
                if (requestBodyType == typeof(string)) { requestBodyBuilder.Append((string)requestBody); }
                else if (requestBodyType == typeof(XDocument)) 
                {
                    //XDocument document = (XDocument)requestBody;
                    //requestBodyBuilder.Append(document.Declaration.ToString());
                    requestBodyBuilder.Append(((XDocument)requestBody).ToString());
                }
                else if (requestBodyType == typeof(XmlDocument)) 
                {
                    requestBodyBuilder.Append(((XmlDocument)requestBody).OuterXml);

                    //using (MemoryStream stream = new MemoryStream())
                    //{
                    //    XmlWriterSettings settings = new XmlWriterSettings();
                    //    settings
                    //    using (XmlWriter writer = XmlWriter.Create(stream, settings)) { }
                    //}
                    //using (StringWriterUTF8 writer = new StringWriterUTF8(requestBodyBuilder)) { }
                }
                else
                {
                    //最初に宣言を入れるには?
                    XmlSerializer serializer = new XmlSerializer(requestBodyType);
                    using (StringWriter writer = new StringWriter(requestBodyBuilder)) { serializer.Serialize(writer, requestBody); }
                }
            }
            catch { return; }

            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                client.Headers.Add("Authorization", wrapper.Authorization);
                client.Headers.Add("Content-Type", "application/xml");

                BodyString = client.UploadString(url, requestBodyBuilder.ToString());

                try
                {
                    if (typeof(T) == typeof(XmlDocument))
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.LoadXml(BodyString);
                        Body = xmlDocument as T;
                    }
                    else if (typeof(T) == typeof(XDocument))
                    {
                        Body = XDocument.Parse(BodyString) as T;
                    }
                    else
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(T));
                        using (StringReader reader = new StringReader(BodyString)) { Body = (T)serializer.Deserialize(reader); }
                    }
                }
                catch { Body = null; }

                Header = client.ResponseHeaders;
            }
        }
    }
}