#define Authorizationを変更可能にする

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Threading.Tasks; //Task
using System.Collections.Specialized; //NameValueCollection

using System.Net.Http; //HttpClient
using System.Net.Http.Headers; //MediaTypeWithQualityHeaderValue

namespace WixossSimulator.SugarSync
{
    //retrieveの訳
    //1.検索する→searchの意味合いを思い起こさせる?
    //2.取得する→getっぽい?別にいい?
    //3.読み出す、4.取り出す、……というかfetchと違うのか
    /// <summary> SugarSyncのプラットフォームAPIのラッパーを提供します？ </summary>
    public class SugarSyncApiWrapper
    {
        private long userId;

        private string userPrefix;
        private string workspacePrefix;
        private string folderPrefix;
        private string filePrefix;
        private string receivedSharePrefix;
        private string contactPrefix;

        /// <summary> ユーザーを示すアドレスを取得します。 </summary>
        /// <returns></returns>
        protected string UserUrl() { return userPrefix; }
        /// <summary> Workspaceを示すアドレスを取得します。 </summary>
        /// <param name="id"> Workspaceを示すID? </param>
        /// <returns></returns>
        protected string WorkspaceUrl(string id) { return id == null ? null : workspacePrefix + id; }
        /// <summary> フォルダを示すアドレスを取得します。 </summary>
        /// <param name="id"> フォルダを示すID? </param>
        /// <returns></returns>
        protected string FolderUrl(string id) { return id == null ? null : folderPrefix + id; }
        /// <summary> ファイルを示すアドレスを取得します。 </summary>
        /// <param name="id"> ファイルを示すID? </param>
        /// <returns></returns>
        protected string FileUrl(string id) { return id == null ? null : filePrefix + id; }
        /// <summary> ReceivedShareを示すアドレスを取得します。 </summary>
        /// <param name="userId"> ReceivedShareを所有するユーザー?のID? </param>
        /// <param name="receivedShareId"> ReceivedShareを示すID? </param>
        /// <returns></returns>
        protected string ReceivedShareUrl(long userId, string receivedShareId) 
        {
            if (userId < 0 || receivedShareId == null) { return null; }
            return receivedSharePrefix + userId.ToString() + ":" + receivedShareId; 
        }
        /// <summary> Contactを示すアドレスを取得します。 </summary>
        /// <param name="userId"> ユーザーを示すID? </param>
        /// <returns></returns>
        protected string ContactUrl(long userId) { return userId < 0 ? null : contactPrefix + userId; }

        //不必要?
        /// <summary> コンテンツを示すアドレスからID? を取得します。 </summary>
        /// <param name="contentUrl"> コンテンツを示すアドレス。 </param>
        /// <returns></returns>
        public string GetContentId(string contentUrl)
        {
            var p = ":sc:" + UserId.ToString() + ":";
            var r = contentUrl.Substring(contentUrl.IndexOf(p) + p.Length);
            var i = r.IndexOf('/');
            if (i == -1) { return r; }
            else { return r.Remove(i); }
        } 

#if(Authorizationを変更可能にする)
        public DateTime Expiration { get; set; }

        public UserResource UserResource { get; set; }

        public string Authorization { get; set; }
        public long UserId 
        {
            get { return userId; }
            set
            {
#else
        public DateTime Expiration { get; private set; }

        protected UserResource UserResource { get; private set; }

        protected string Authorization { get; private set; }
        protected long UserId 
        {
            get { return userId; }
            private set
            {
#endif
                userId = value;
                if (UserId >= 0)
                {
                    userPrefix = "https://api.sugarsync.com/user/" + UserId.ToString();
                    workspacePrefix = "https://api.sugarsync.com/workspace/:sc:" + UserId.ToString() + ":";
                    folderPrefix = "https://api.sugarsync.com/folder/:sc:" + UserId.ToString() + ":";
                    filePrefix = "https://api.sugarsync.com/file/:sc:" + UserId.ToString() + ":";
                    receivedSharePrefix = "https://api.sugarsync.com/receivedShare/" + UserId.ToString() + "/:sc:";
                    contactPrefix = "https://api.sugarsync.com/contact/" + UserId.ToString() + "/";
                }
                else
                {
                    userPrefix = null;
                    workspacePrefix = null;
                    folderPrefix = null;
                    filePrefix = null;
                    receivedSharePrefix = null;
                    contactPrefix = null;
                }
            }
        }



        public SugarSyncApiWrapper()
        {
            Expiration = DateTime.MinValue;
            Authorization = null;
            UserId = -1;
        }

        public async Task<SugarSyncHttpResponse> CreateRefreshToken(AppAuthorizationResource appAuthorization)
        {
            return await SugarSyncHttpClient.PostXmlAsync(this, "https://api.sugarsync.com/app-authorization", appAuthorization);
        }

        /// <summary>
        /// プラットフォームAPIを介してユーザーのリソースにアクセスするための認証を行います。
        /// (ここで作成されるアクセストークンの有効期限は1時間です。)
        /// https://www.sugarsync.com/dev/api/method/create-auth-token.html
        /// </summary>
        /// <param name="tokenAuthRequest"> ユーザーの認証情報を示すXMLインスタンスである文字列。XML宣言を含めてはいけない。 </param>
        /// <returns></returns>
        public async Task<bool> CreateAccessTokenAsync(string tokenAuthRequest)
        {
            return await GetAccessTokenResourceAsync(tokenAuthRequest);
        }

        /// <summary>
        /// プラットフォームAPIを介してユーザーのリソースにアクセスするための認証を行います。
        /// (ここで作成されるアクセストークンの有効期限は1時間です。)
        /// https://www.sugarsync.com/dev/api/method/create-auth-token.html
        /// </summary>
        /// <param name="tokenAuthRequest"> ユーザーの認証情報を示すクラス。 </param>
        /// <returns></returns>
        public async Task<bool> CreateAccessTokenAsync(TokenAuthRequestResource tokenAuthRequest)
        {
            return await GetAccessTokenResourceAsync(tokenAuthRequest);
        }

        private async Task<bool> GetAccessTokenResourceAsync(object tokenAuthRequest)
        {
            Expiration = DateTime.MaxValue;
            Authorization = null;

            try
            {
                var requestXml = new StringContent(SugarSyncHttpClient.FormatRequestXml(tokenAuthRequest));
                requestXml.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/xml");
                requestXml.Headers.ContentType.CharSet = "UTF-8";

                using (var client = new HttpClient())
                {
                    var response = await SugarSyncHttpClient.ConnectAsync<AccessTokenResource>(
                        this, client, async () => await client.PostAsync("https://api.sugarsync.com/authorization", requestXml), true);

                    Expiration = response.Body.Expiration;
                    Authorization = response.Headers.Location.AbsoluteUri;
                    UserId = response.Body.UserId;
                }

                UserResource = (await RetrieveUserInformationAsync()).Body;
            }
            catch
            {
                Expiration = DateTime.MinValue;
                Authorization = null;
                UserId = -1;

                UserResource = null;

                return false;
            }

            return true;
        }

        /// <summary>
        /// SugarSyncユーザーについての情報を取得します。
        /// https://www.sugarsync.com/dev/api/method/get-user-info.html
        /// </summary>
        /// <returns></returns>
        public async Task<SugarSyncHttpResponse<UserResource>> RetrieveUserInformationAsync()
        {
            return await SugarSyncHttpClient.GetAsync<UserResource>(this, new Uri(UserUrl()));
        }

        /// <summary>
        /// ユーザーのアカウント内のWorkspaceについての情報を取得します。
        /// https://www.sugarsync.com/dev/api/method/get-workspaces.html
        /// </summary>
        /// <returns></returns>
        public async Task<SugarSyncHttpResponse<WorkspacesCollectionResource>> RetrieveWorkspacesCollectionAsync()
        {
            return await SugarSyncHttpClient.GetAsync<WorkspacesCollectionResource>(this, UserResource.Workspaces);
            //return await SugarSyncHttpClient.GetAsync<WorkspacesCollectionResource>(this, UserUrl() + "/workspaces/contents");
        }

        /// <summary>
        /// Workspaceについての情報を取得します。
        /// https://www.sugarsync.com/dev/api/method/get-workspace-info.html
        /// </summary>
        /// <param name="workspaceId"> Workspaceを示すID? </param>
        /// <returns></returns>
        public async Task<SugarSyncHttpResponse<WorkspaceResource>> RetrieveWorkspaceInformationAsync(string workspaceId)
        {
            return await SugarSyncHttpClient.GetAsync<WorkspaceResource>(this, WorkspaceUrl(workspaceId));
        }

        /// <summary>
        /// Workspaceのコンテンツを取得します。
        /// https://www.sugarsync.com/dev/api/method/get-workspace-contents.html
        /// </summary>
        /// <param name="workspaceId"> Workspaceを示すID? </param>
        /// <returns></returns>
        public async Task<SugarSyncHttpResponse<FoldersCollectionResource>> RetrieveWorkspaceContentsAsync(string workspaceId)
        {
            return await SugarSyncHttpClient.GetAsync<FoldersCollectionResource>(this, WorkspaceUrl(workspaceId) + "/contents");
        }

        /// <summary>
        /// Workspaceの属性を更新します。現在のところ、更新することができる属性は <c>DisplayName</c> のみです。
        /// https://www.sugarsync.com/dev/api/method/update-workspace-name.html
        /// </summary>
        /// <param name="workspaceResource"> 更新されるWorkspaceを示すクラス。 </param>
        /// <returns></returns>
        public async Task<SugarSyncHttpResponse> UpdateWorkspaceInformationAsync(WorkspaceResource workspaceResource)
        {
            return await SugarSyncHttpClient.PutXmlAsync(this, WorkspaceUrl(workspaceResource.GetWorkspaceId()), workspaceResource);
        }

        /// <summary>
        /// Workspaceの属性を更新します。現在のところ、更新することができる属性は <c>DisplayName</c> のみです。
        /// https://www.sugarsync.com/dev/api/method/update-workspace-name.html
        /// </summary>
        /// <param name="workspaceId"> Workspaceを示すID? </param>
        /// <param name="updatedDisplayName"> 更新されるWorkspaceの名前。 nullである場合は更新しません。 </param>
        /// <returns></returns>
        public async Task<SugarSyncHttpResponse> UpdateWorkspaceInformationAsync(string workspaceId, string updatedDisplayName)
        {
            if (isEmptyOrWhiteSpaceString(updatedDisplayName)) { return SugarSyncHttpResponse.Failure; /*名前を空白にしておくことはできません。*/ }

            var apiResponse = await RetrieveWorkspaceInformationAsync(workspaceId);
            if (!apiResponse.IsSuccess) { return SugarSyncHttpResponse.Failure; }
            var workspaceResource = apiResponse.Body;

            updatedDisplayName = updatedDisplayName ?? workspaceResource.DisplayName; //他との整合性のために、nullは変更しないことと定義
            if (workspaceResource.DisplayName == updatedDisplayName) { return SugarSyncHttpResponse.Success; } //名前が一致する場合は更新不要
            workspaceResource.DisplayName = updatedDisplayName;

            return await SugarSyncHttpClient.PutXmlAsync(this, WorkspaceUrl(workspaceId), workspaceResource);
        }

        /// <summary>
        /// ユーザーのアカウント内の同期フォルダについての情報を取得します。
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

            //var uri = new UriBuilder(UserUrl() + "/folders/contents");
            var uri = new UriBuilder(UserResource.Syncfolders);
            uri.Query = query.ToString();
            return await SugarSyncHttpClient.GetAsync<FoldersCollectionResource>(this, uri.Uri);
        }

        /// <summary>
        /// 親フォルダに含まれるフォルダを取得します。(workspaceは <c>RetrieveWorkspaceContents</c> メソッドを使用してください?)
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

            var uri = new UriBuilder(FolderUrl(folderId) + "/contents");
            uri.Query = query.ToString();
            return await SugarSyncHttpClient.GetAsync<FoldersCollectionResource>(this, uri.Uri);            
        }

        /// <summary>
        /// フォルダについての情報を取得します。
        /// https://www.sugarsync.com/dev/api/method/get-folder-info.html
        /// </summary>
        /// <param name="folderId"> フォルダを示すID? </param>
        /// <returns></returns>
        public async Task<SugarSyncHttpResponse<FolderResource>> RetrieveFolderInformationAsync(string folderId)
        {
            return await SugarSyncHttpClient.GetAsync<FolderResource>(this, FolderUrl(folderId));
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

            var uri = new UriBuilder(FolderUrl(folderId) + "/contents");
            uri.Query = query.ToString();
            return await SugarSyncHttpClient.GetAsync<FoldersCollectionResource>(this, uri.Uri);
        }

        /// <summary>
        /// フォルダ内に別のフォルダを作成します。
        /// https://www.sugarsync.com/dev/api/method/create-folder.html
        /// </summary>
        /// <param name="folderId"> 親フォルダを示すID? </param>
        /// <param name="displayName"> The user-visible name of the new subfolder. </param>
        /// <returns></returns>
        public async Task<SugarSyncHttpResponse> CreateFolderAsync(string folderId, string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName)) { return SugarSyncHttpResponse.Failure; }

            string request = "<folder><displayName>" + displayName + "</displayName></folder>";
            return await SugarSyncHttpClient.PostXmlAsync(this, FolderUrl(folderId), request);
        }

        /// <summary>
        /// フォルダ内にファイルを作成します。
        /// https://www.sugarsync.com/dev/api/method/create-file.html
        /// </summary>
        /// <param name="folderId"> フォルダを示すID? </param>
        /// <param name="displayName"> The user-visible name of the file to be created. </param>
        /// <param name="mediaType"> The media type of the file to be created, such as image/jpeg. </param>
        /// <returns></returns>
        public async Task<SugarSyncHttpResponse> CreateFileAsync(string folderId, string displayName, string mediaType = null)
        {
            if (string.IsNullOrWhiteSpace(displayName)) { return SugarSyncHttpResponse.Failure; }

            string request = "<file><displayName>" + displayName + "</displayName>";
            if (!string.IsNullOrWhiteSpace(mediaType)) { request += "<mediaType>" + mediaType + "</mediaType>"; }
            request += "</file>";
            return await SugarSyncHttpClient.PostXmlAsync(this, FolderUrl(folderId), request);

        }

        /// <summary>
        /// フォルダ内に既存のファイルのコピーを作成します。
        /// https://www.sugarsync.com/dev/api/method/copy-file.html
        /// </summary>
        /// <param name="folderId"> フォルダを示すID? </param>
        /// <param name="fileId"> コピーされるファイルを示すID? </param>
        /// <param name="newDisplayName"> The name of new file copy. </param>
        /// <returns></returns>
        public async Task<SugarSyncHttpResponse> CopyFileAsync(string folderId, string fileId, string newDisplayName)
        {
            if (string.IsNullOrWhiteSpace(newDisplayName)) { return SugarSyncHttpResponse.Failure; }

            string request = "<fileCopy source=\"" + FileUrl(fileId) + "\">";
            request += "<displayName>" + newDisplayName + "</displayName>";
            request += "</fileCopy>";
            return await SugarSyncHttpClient.PostXmlAsync(this, FolderUrl(folderId), request);
        }

        /// <summary>
        /// フォルダを完全に削除します。
        /// https://www.sugarsync.com/dev/api/method/delete-folder.html
        /// </summary>
        /// <param name="folderId"> フォルダを示すID? </param>
        /// <returns></returns>
        public async Task<SugarSyncHttpResponse> DeleteFolderAsync(string folderId)
        {
            return await SugarSyncHttpClient.DeleteAsync(this, FolderUrl(folderId));
        }

        /// <summary>
        /// フォルダの個々の属性を更新します。
        /// 更新することができる属性は、<c>DisplayName</c>, <c>Parent</c> です。その他のフォルダの属性は変更することができません。
        /// 
        /// 備考: <c>Parent</c> 要素の値を変更することで、フォルダを別の親コレクション(Workspaceまたはフォルダ)に移動させることができます。
        /// https://www.sugarsync.com/dev/api/method/update-folder-info.html
        /// </summary>
        /// <param name="folderResource"> 更新されるフォルダを示すクラス。 </param>
        /// <returns></returns>
        public async Task<SugarSyncHttpResponse> UpdateFolderInformationAsync(FolderResource folderResource)
        {
            return await SugarSyncHttpClient.PutXmlAsync(this, FolderUrl(folderResource.GetFolderId()), folderResource);
        }

        /// <summary>
        /// フォルダの個々の属性を更新します。
        /// 更新することができる属性は、<c>DisplayName</c>, <c>Parent</c> です。その他のフォルダの属性は変更することができません。
        /// 
        /// 備考: <c>Parent</c> 要素の値を変更することで、フォルダを別の親コレクション(Workspaceまたはフォルダ)に移動させることができます。
        /// https://www.sugarsync.com/dev/api/method/update-folder-info.html
        /// </summary>
        /// <param name="folderId"> フォルダを示すID? </param>
        /// <param name="displayName"> 更新されるフォルダの名前。nullである場合は更新しません。 </param>
        /// <param name="parentDirectoryId"> 更新されるフォルダの親ディレクトリを示すID? nullである場合は更新しません。 </param>
        /// <returns></returns>
        public async Task<SugarSyncHttpResponse> UpdateFolderInformationAsync(string folderId, string displayName = null, string parentDirectoryId = null)
        {
            if (isEmptyOrWhiteSpaceString(displayName, parentDirectoryId)) { return SugarSyncHttpResponse.Failure; }
            //if ((updatedDisplayName != null && string.IsNullOrWhiteSpace(updatedDisplayName))
            //    || (parentFolderId != null && string.IsNullOrWhiteSpace(parentFolderId))) { return false; }

            var apiResponse = await RetrieveFolderInformationAsync(folderId);
            if (!apiResponse.IsSuccess) { return SugarSyncHttpResponse.Failure; }
            var folderResource = apiResponse.Body;

            //if (updatedDisplayName == null && parentDirectoryId == null) { return SugarSyncHttpResponse.Success; }
            displayName = displayName ?? folderResource.DisplayName;
            var parent = FolderUrl(parentDirectoryId) ?? folderResource.Parent;
            if (folderResource.DisplayName == displayName && folderResource.Parent == parent) { return SugarSyncHttpResponse.Success; }
            folderResource.DisplayName = displayName;
            folderResource.Parent = parent;

            return await SugarSyncHttpClient.PutXmlAsync(this, FolderUrl(folderId), folderResource);
        }

        private bool isEmptyOrWhiteSpaceString(params string[] args)
        {
            foreach (var arg in args) { if (arg != null && string.IsNullOrWhiteSpace(arg)) { return true; } }
            return false;
        }

        /// <summary>
        /// RecievedShare(他のユーザーによって所有され、その所有ユーザーによってユーザーにアクセス権が与えられた共有フォルダ?)についての情報を取得します。
        /// https://www.sugarsync.com/dev/api/method/get-received-shares.html
        /// </summary>
        /// <param name="userId"> ReceivedShareを所有するユーザー?のID? </param>
        /// <param name="receivedShareId"> ReceivedShareを示すID? </param>
        /// <returns></returns>
        public async Task<SugarSyncHttpResponse<ReceivedShareResource>> RetrievieReceivedShareInformationAsync(long userId, string receivedShareId)
        {
            return await SugarSyncHttpClient.GetAsync<ReceivedShareResource>(this, ReceivedShareUrl(userId, receivedShareId));
        }

        /// <summary>
        /// RetrievedShareの一覧を取得します。
        /// https://www.sugarsync.com/dev/api/method/get-received-shares-list.html
        /// </summary>
        /// <returns></returns>
        public async Task<SugarSyncHttpResponse<ReceivedSharesListResource>> RetrieveReceivedSharesListAsync()
        {
            return await SugarSyncHttpClient.GetAsync<ReceivedSharesListResource>(this, UserResource.ReceivedShares);
            //return await SugarSyncHttpClient.GetAsync<ReceivedSharesListResource>(this, UserUrl() + "/receivedShares/contents");
        }

        /// <summary>
        /// Contact(ユーザーと単一または複数のフォルダを共有している他のSugarSyncユーザーを表します)についての情報を取得します。
        /// https://www.sugarsync.com/dev/api/method/get-contacts.html
        /// </summary>
        /// <param name="userId"> Contact?ユーザーのID? </param>
        /// <returns></returns>
        public async Task<SugarSyncHttpResponse<ContactResource>> RetrieveContactInformationAsync(long userId)
        {
            return await SugarSyncHttpClient.GetAsync<ContactResource>(this, ContactUrl(userId));
        }

        /// <summary>
        /// ファイルについての情報を取得します。
        /// https://www.sugarsync.com/dev/api/method/get-file-info.html
        /// </summary>
        /// <param name="fileId"> ファイルを示すID? </param>
        /// <returns></returns>
        public async Task<SugarSyncHttpResponse<FileResource>> RetrieveFileInformationAsync(string fileId)
        {
            return await SugarSyncHttpClient.GetAsync<FileResource>(this, FileUrl(fileId));
        }

        /// <summary>
        /// ファイルの個々の属性を更新します。
        /// 更新することができる属性は、<c>DisplayName</c>, <c>MediaType</c>, <c>Parent</c>, <c>PublicLink</c> です。その他のフォルダの属性は変更することができません。
        /// 
        /// 備考: <c>Parent</c> 要素の値を変更することで、ファイルを別の親フォルダに移動させることができます。
        /// 加えて、<c>PublicLink</c> 要素内の <c>Enabled</c> の値を true に設定することで、ファイルへのパブリックアクセスを提供することができます。
        /// これはファイルへのパブリックリンクを作成します。
        /// <c>PublicLink</c> 要素内の <c>Enabled</c> の値を false に設定することで、ファイルへのパブリックアクセスを削除することができます。
        /// これはファイルへのパブリックリンクを破棄します。
        /// https://www.sugarsync.com/dev/api/method/update-file-info.html
        /// </summary>
        /// <param name="fileResource"> 更新されるファイルを示すクラス。 </param>
        /// <returns></returns>
        public async Task<SugarSyncHttpResponse> UpdateFileInformationAsync(FileResource fileResource)
        {
            return await SugarSyncHttpClient.PutXmlAsync(this, FileUrl(fileResource.GetFileId()), fileResource);
        }

        /// <summary>
        /// ファイルの個々の属性を更新します。
        /// 更新することができる属性は、<c>DisplayName</c>, <c>MediaType</c>, <c>Parent</c>, <c>PublicLink</c> です。その他のフォルダの属性は変更することができません。
        /// 
        /// 備考: <c>Parent</c> 要素の値を変更することで、ファイルを別の親フォルダに移動させることができます。
        /// 加えて、<c>PublicLink</c> 要素内の <c>Enabled</c> の値を true に設定することで、ファイルへのパブリックアクセスを提供することができます。
        /// これはファイルへのパブリックリンクを作成します。
        /// <c>PublicLink</c> 要素内の <c>Enabled</c> の値を false に設定することで、ファイルへのパブリックアクセスを削除することができます。
        /// これはファイルへのパブリックリンクを破棄します。
        /// https://www.sugarsync.com/dev/api/method/update-file-info.html
        /// </summary>
        /// <param name="fileId"> ファイルを示すID? </param>
        /// <param name="displayName"> 更新されるファイルの名前。nullである場合は更新しません。 </param>
        /// <param name="mediaType"> 更新されるファイルのMediaType。 nullである場合は更新しません。 </param>
        /// <param name="parentDirectoryId"> 更新されるフォルダの親ディレクトリを示すID? nullである場合は更新しません。 </param>
        /// <param name="isEnabledPublicLink"> 更新されるファイルのパブリックリンクの有無。nullである場合は更新しません。 </param>
        /// <returns></returns>
        public async Task<SugarSyncHttpResponse> UpdateFileInformationAsync(
            string fileId, string displayName = null, string mediaType = null, string parentDirectoryId = null, bool? isEnabledPublicLink = null)
        {
            if (isEmptyOrWhiteSpaceString(displayName, mediaType, parentDirectoryId)) { return SugarSyncHttpResponse.Failure; }

            var apiResponse = await RetrieveFileInformationAsync(fileId);
            if (!apiResponse.IsSuccess) { return SugarSyncHttpResponse.Failure; }
            var fileResource = apiResponse.Body;

            displayName = displayName ?? fileResource.DisplayName;
            mediaType = mediaType ?? fileResource.MediaType;
            var parent = FolderUrl(parentDirectoryId) ?? fileResource.Parent;
            var enabled = isEnabledPublicLink ?? fileResource.PublicLink.Enabled;
            if (fileResource.DisplayName == displayName && fileResource.MediaType == mediaType
                && fileResource.Parent == parent && fileResource.PublicLink.Enabled == enabled) { return SugarSyncHttpResponse.Success; }
            fileResource.DisplayName = displayName;
            fileResource.MediaType = mediaType;
            fileResource.Parent = parent;
            fileResource.PublicLink.Enabled = enabled;

            return await SugarSyncHttpClient.PutXmlAsync(this, FileUrl(fileId), fileResource);
        }

        /// <summary>
        /// ファイルを完全に削除します。
        /// https://www.sugarsync.com/dev/api/method/delete-file.html
        /// </summary>
        /// <param name="fileId"> ファイルを示すID? </param>
        /// <returns></returns>
        public async Task<SugarSyncHttpResponse> DeleteFileAsync(string fileId)
        {
            return await SugarSyncHttpClient.DeleteAsync(this, FileUrl(fileId));
        }

        /// <summary>
        /// バージョン履歴はSugarSyncによって保持されているファイルの全てのバージョンの履歴を表します。
        /// SugarSyncはファイルの過去5バージョンを保持していることに注意してください。
        /// 
        /// 提出されたファイルへのバージョン履歴にアクセスします。
        /// https://www.sugarsync.com/dev/api/method/get-version-history.html
        /// </summary>
        /// <param name="fileId"> ファイルを示すID? </param>
        /// <returns></returns>
        public async Task<SugarSyncHttpResponse<FileVersionsResource>> RetrieveVersionHistoryAsync(string fileId)
        {
            return await SugarSyncHttpClient.GetAsync<FileVersionsResource>(this, FileUrl(fileId) + "/version");
        }
    }
}