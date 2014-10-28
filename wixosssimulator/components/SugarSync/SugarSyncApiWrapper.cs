#define Authorizationを変更可能にする

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Threading.Tasks; //Task
using System.Collections.Specialized; //NameValueCollection

//using System.Net; //WebClient
//using System.IO;
//using System.Text; //StringBuilder

//using System.Xml; //XmlDocument
//using System.Xml.Linq; //XDocument
//using System.Xml.Serialization; //XmlSerializer
//using DropNet;

namespace WixossSimulator.SugarSync
{
    //retrieveの訳
    //1.検索する→searchの意味合いを思い起こさせる?
    //2.取得する→getっぽい?別にいい?
    //3.読み出す、4.取り出す、……というかfetchと違うのか
    /// <summary> WixossSimulator のための SugarSync API のラッパーを提供します？ </summary>
    public class SugarSyncApiWrapper
    {
        private string userPrefix { get { return "https://api.sugarsync.com/user/" + userId.ToString(); } }
        private string workspacePrefix { get { return "https://api.sugarsync.com/workspace/:sc:" + userId.ToString() + ":"; } }
        private string folderPrefix { get { return "https://api.sugarsync.com/folder/:sc:" + userId.ToString() + ":"; } }
        private string filePrefix { get { return "https://api.sugarsync.com/file/:sc:" + userId.ToString() + ":"; } }

        /// <summary> ユーザーを示すアドレスを取得します。 </summary>
        /// <returns></returns>
        protected string userUrl { get { return "https://api.sugarsync.com/user/" + userId.ToString(); } }
        /// <summary> Workspaceを示すアドレスを取得します。 </summary>
        /// <param name="workspaceId"> Workspaceを示すID? </param>
        /// <returns></returns>
        protected string getWorkspaceUrl(string workspaceId) { return "https://api.sugarsync.com/workspace/:sc:" + userId.ToString() + ":" + workspaceId; }
        /// <summary> フォルダを示すアドレスを取得します。 </summary>
        /// <param name="folderId"> フォルダを示すID? </param>
        /// <returns></returns>
        protected string getFolderUrl(string folderId) { return "https://api.sugarsync.com/folder/:sc:" + userId.ToString() + ":" + folderId; }
        /// <summary> ファイルを示すアドレスを取得します。 </summary>
        /// <param name="fileId"> ファイルを示すID? </param>
        /// <returns></returns>
        protected string getFileUrl(string fileId) { return "https://api.sugarsync.com/file/:sc:" + userId.ToString() + ":" + fileId; }

#if(Authorizationを変更可能にする)
        public string authorization { get; set; }
        public long userId { get; set; }

        public DateTime Expiration { get; set; }
#else
        private string authorization { get; set; }
        private long userId { get; set; }

        public DateTime Expiration { get; private set; }
#endif

        public SugarSyncApiWrapper()
        {
            Expiration = DateTime.MinValue;
            authorization = null;
            userId = -1;
        }

        /// <summary>
        /// APIを通してユーザーのリソースにアクセスするための認証を行います。
        /// (ここで作成されるアクセストークンの有効期限は1時間)
        /// https://www.sugarsync.com/dev/api/method/create-auth-token.html
        /// </summary>
        /// <returns></returns>
        public bool CreateAccessToken()
        {
            string tokenAuthRequest;
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                try { tokenAuthRequest = client.DownloadString("http://zeta00s.php.xdomain.jp/wixoss/sugarsync/token-auth-request.xml"); }
                catch { tokenAuthRequest = null; }
            }
            if (string.IsNullOrWhiteSpace(tokenAuthRequest)) { return false; }

            try
            {
                Expiration = DateTime.MaxValue;
                var response = new SugarSyncResponseByPostOrPutMethod<AccessTokenResource>(this, "https://api.sugarsync.com/authorization", tokenAuthRequest);

                Expiration = response.Body.Expiration;
                authorization = response.Header["Location"];
                userId = response.Body.UserId;
            }
            catch
            {
                Expiration = DateTime.MinValue;
                authorization = null;
                userId = -1;
                return false;
            }

            return true;
        }

        /// <summary>
        /// SugarSyncユーザーについての情報を取得します。
        /// https://www.sugarsync.com/dev/api/method/get-user-info.html
        /// </summary>
        /// <param name="userId"> ユーザーID。 </param>
        /// <returns></returns>
        public SugarSyncResponse<UserResource> RetrieveUserInformation()
        {
            return new SugarSyncResponseByGetMethod<UserResource>(this, userPrefix);
        }

        /// <summary>
        /// SugarSyncユーザーについての情報を取得します。
        /// https://www.sugarsync.com/dev/api/method/get-user-info.html
        /// </summary>
        /// <param name="userId"> ユーザーID。 </param>
        /// <returns></returns>
        public async Task<SugarSyncHttpResponse<UserResource>> RetrieveUserInformationAsync()
        {
            return await SugarSyncHttpClient.GetAsync<UserResource>(this, new Uri(userUrl));
        }

        /// <summary>
        /// ユーザーのアカウント中にあるWorkspaceについての情報を取得します。
        /// https://www.sugarsync.com/dev/api/method/get-workspaces.html
        /// </summary>
        /// <param name="userId"> ユーザーID。 </param>
        /// <returns></returns>
        public SugarSyncResponse<WorkspacesCollectionResource> RetrieveWorkspacesCollection()
        {
            return new SugarSyncResponseByGetMethod<WorkspacesCollectionResource>(this, userPrefix + "/workspaces/contents");
        }

        /// <summary>
        /// Workspaceについての情報を取得します。
        /// https://www.sugarsync.com/dev/api/method/get-workspace-info.html
        /// </summary>
        /// <param name="workspaceId"> Workspaceを示すID? </param>
        /// <returns></returns>
        public SugarSyncResponse<WorkspaceResource> RetrieveWorkspaceInformation(string workspaceId)
        {
            return new SugarSyncResponseByGetMethod<WorkspaceResource>(this, workspacePrefix + workspaceId);
        }

        /// <summary>
        /// Workspaceのコンテンツを取得します。
        /// https://www.sugarsync.com/dev/api/method/get-workspace-contents.html
        /// </summary>
        /// <param name="workspaceId"> Workspaceを示すID? </param>
        /// <returns></returns>
        public SugarSyncResponse<FoldersCollectionResource> RetrieveWorkspaceContents(string workspaceId)
        {
            return new SugarSyncResponseByGetMethod<FoldersCollectionResource>(this, workspacePrefix + workspaceId + "/contents");
        }

        /// <summary>
        /// Workspaceの属性を更新します。
        /// https://www.sugarsync.com/dev/api/method/update-workspace-name.html
        /// </summary>
        /// <param name="workspaceResource"> 更新されたWorkspaceを示すクラス。 </param>
        /// <returns></returns>
        public bool UpdateWorkspaceInformation(WorkspaceResource workspaceResource)
        {
            string workspaceId = workspaceResource.Dsid.Replace("/sc/" + userId.ToString() + "/", "");
            return new SugarSyncResponseByPostOrPutMethod<object>(this, workspacePrefix + workspaceId, workspaceResource)
                .Header != null;
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
        public SugarSyncResponse<FoldersCollectionResource> RetrieveSyncFoldersCollection(long start = 0, long max = 0)
        {
            NameValueCollection getQuery = new NameValueCollection();
            if (start > 0) { getQuery.Add("start", start.ToString()); }
            if (max > 0) { getQuery.Add("max", max.ToString()); }
            return new SugarSyncResponseByGetMethod<FoldersCollectionResource>(this, userPrefix + "/folders/contents", getQuery);
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
        public async Task<SugarSyncHttpResponse<FoldersCollectionResource>> RetrieveSyncFoldersCollectionAsync(long start = 0, long max = 0)
        {
            var query = new NameValueCollection();
            if (start > 0) { query.Add("start", start.ToString()); }
            if (max > 0) { query.Add("max", max.ToString()); }

            var uri = new UriBuilder(userUrl + "/folders/contents");
            uri.Query = query.ToString();
            return await SugarSyncHttpClient.GetAsync<FoldersCollectionResource>(this, uri.Uri);
        }

        /// <summary>
        /// 親フォルダに含まれるフォルダを取得します。(workspaceは <c> RetrieveWorkspaceContents </c> メソッドを使用してください?)
        /// https://www.sugarsync.com/dev/api/method/get-folders.html
        /// </summary>
        /// <param name="folderId"> 親フォルダを示すID? </param>
        /// <param name="start"> 
        /// Specifies the index within the indexed sequence of folders where the client wants folders to be retrieved.
        /// The index starts at origin 0. The default value is 0.
        /// </param>
        /// <param name="max"> 
        /// The maximum number of folders, beginning with the folder at the start index, that the client wants listed in the response.
        /// If this parameter is not specified, no limit is placed on the number of retrieved folders.
        /// </param>
        /// <returns></returns>
        public SugarSyncResponse<FoldersCollectionResource> RetrieveFolders(string folderId, long start = 0, long max = 0)
        {
            NameValueCollection getQuery = new NameValueCollection();
            getQuery.Add("type", RetrievingFolderType.Folder.ToApiString());
            if (start > 0) { getQuery.Add("start", start.ToString()); }
            if (max > 0) { getQuery.Add("max", max.ToString()); }
            return new SugarSyncResponseByGetMethod<FoldersCollectionResource>(this, folderPrefix + folderId + "/contents", getQuery);
        }

        /// <summary>
        /// 親フォルダに含まれるフォルダを取得します。(workspaceは <c> RetrieveWorkspaceContents </c> メソッドを使用してください?)
        /// https://www.sugarsync.com/dev/api/method/get-folders.html
        /// </summary>
        /// <param name="folderId"> 親フォルダを示すID? </param>
        /// <param name="start"> 
        /// Specifies the index within the indexed sequence of folders where the client wants folders to be retrieved.
        /// The index starts at origin 0. The default value is 0.
        /// </param>
        /// <param name="max"> 
        /// The maximum number of folders, beginning with the folder at the start index, that the client wants listed in the response.
        /// If this parameter is not specified, no limit is placed on the number of retrieved folders.
        /// </param>
        /// <returns></returns>
        public async Task<SugarSyncHttpResponse<FoldersCollectionResource>> RetrieveFoldersAsync(string folderId, long start = 0, long max = 0)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query.Add("type", RetrievingFolderType.Folder.ToApiString());
            if (start > 0) { query.Add("start", start.ToString()); }
            if (max > 0) { query.Add("max", max.ToString()); }

            var uri = new UriBuilder(getFolderUrl(folderId) + "/contents");
            uri.Query = query.ToString();
            return await SugarSyncHttpClient.GetAsync<FoldersCollectionResource>(this, uri.Uri);            
        }

        /// <summary>
        /// フォルダについての情報を取得します。
        /// https://www.sugarsync.com/dev/api/method/get-folder-info.html
        /// </summary>
        /// <param name="folderId"> フォルダを示すID? </param>
        /// <returns></returns>
        public SugarSyncResponse<FolderResource> RetrieveFolderInformation(string folderId)
        {
            return new SugarSyncResponseByGetMethod<FolderResource>(this, folderPrefix + folderId);
        }

        /// <summary>
        /// フォルダについての情報を取得します。
        /// https://www.sugarsync.com/dev/api/method/get-folder-info.html
        /// </summary>
        /// <param name="folderId"> フォルダを示すID? </param>
        /// <returns></returns>
        public async Task<SugarSyncHttpResponse<FolderResource>> RetrieveFolderInformationAsync(string folderId)
        {
            return await SugarSyncHttpClient.GetAsync<FolderResource>(this, new Uri(getFolderUrl(folderId)));
        }

        /// <summary>
        /// フォルダのコンテンツを取得します。
        /// https://www.sugarsync.com/dev/api/method/get-folder-contents.html
        /// </summary>
        /// <param name="folderId"> フォルダを示すID? </param>
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
        public SugarSyncResponse<FoldersCollectionResource> RetrieveFolderContents(
            string folderId,
            RetrievingFolderType type = RetrievingFolderType.None,
            long start = 0, long max = 0,
            RetrievingFolderOrder order = RetrievingFolderOrder.None)
        {
            NameValueCollection getQuery = new NameValueCollection();
            if (type != RetrievingFolderType.None) { getQuery.Add("type", type.ToApiString()); }
            if (start > 0) { getQuery.Add("start", start.ToString()); }
            if (max > 0) { getQuery.Add("max", max.ToString()); }
            if (order != RetrievingFolderOrder.None) { getQuery.Add("order", order.ToApiString()); }
            return new SugarSyncResponseByGetMethod<FoldersCollectionResource>(this, folderPrefix + folderId + "/contents", getQuery);
        }

        /// <summary>
        /// フォルダのコンテンツを取得します。
        /// https://www.sugarsync.com/dev/api/method/get-folder-contents.html
        /// </summary>
        /// <param name="folderId"> フォルダを示すID? </param>
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
        public async Task<SugarSyncHttpResponse<FoldersCollectionResource>> RetrieveFolderContentsAsync(
            string folderId,
            RetrievingFolderType type = RetrievingFolderType.None,
            long start = 0, long max = 0,
            RetrievingFolderOrder order = RetrievingFolderOrder.None)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (type != RetrievingFolderType.None) { query.Add("type", type.ToApiString()); }
            if (start > 0) { query.Add("start", start.ToString()); }
            if (max > 0) { query.Add("max", max.ToString()); }
            if (order != RetrievingFolderOrder.None) { query.Add("order", order.ToApiString()); }

            var uri = new UriBuilder(getFolderUrl(folderId) + "/contents");
            uri.Query = query.ToString();
            return await SugarSyncHttpClient.GetAsync<FoldersCollectionResource>(this, uri.Uri);
        }

        /// <summary>
        /// フォルダの中に別のフォルダを作成します。
        /// https://www.sugarsync.com/dev/api/method/create-folder.html
        /// </summary>
        /// <param name="folderId"> 親フォルダを示すID? </param>
        /// <param name="displayName"> The user-visible name of the new subfolder. </param>
        /// <returns></returns>
        public bool CreateFolder(string folderId, string displayName)
        {
            string requestBody = "<folder><displayName>" + displayName + "</displayName></folder>";
            return new SugarSyncResponseByPostOrPutMethod<object>(this, folderPrefix + folderId, requestBody)
                .Header != null;
        }

        /// <summary>
        /// フォルダの中にファイルを作成します。
        /// https://www.sugarsync.com/dev/api/method/create-file.html
        /// </summary>
        /// <param name="folderId"> フォルダを示すID? </param>
        /// <param name="displayName"> The user-visible name of the file to be created. </param>
        /// <param name="mediaType"> The media type of the file to be created, such as image/jpeg. </param>
        /// <returns></returns>
        public bool CreateFile(string folderId, string displayName, string mediaType = null)
        {
            string requestBody = "<file><displayName>" + displayName + "</displayName>";
            if (!string.IsNullOrWhiteSpace(mediaType)) { requestBody += "<mediaType>" + mediaType + "</mediaType>"; }
            requestBody += "</file>";
            return new SugarSyncResponseByPostOrPutMethod<object>(this, folderPrefix + folderId, requestBody)
                .Header != null;
        }

        /// <summary>
        /// フォルダの中に既存のファイルのコピーを作成します。
        /// https://www.sugarsync.com/dev/api/method/copy-file.html
        /// </summary>
        /// <param name="folderId"> フォルダを示すID? </param>
        /// <param name="copiedFileId"> コピーされるファイルを示すID? </param>
        /// <param name="displayName"> The name of new file copy. </param>
        /// <returns></returns>
        public bool CopyFile(string folderId, string copiedFileId, string displayName)
        {
            string requestBody = "<fileCopy source=\"" + filePrefix + copiedFileId + "\">";
            requestBody += "<displayName>" + displayName + "</displayName>";
            requestBody += "</fileCopy>";
            return new SugarSyncResponseByPostOrPutMethod<object>(this, folderPrefix + folderId, requestBody)
                .Header != null;
        }


    }
}