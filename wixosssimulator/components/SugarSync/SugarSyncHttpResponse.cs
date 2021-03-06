﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Threading.Tasks; //Task

using System.IO; //Stream (Reader/Writer)
using System.Net; //HttpStatus
using System.Net.Http; //HttpClient
using System.Net.Http.Headers; //AuthenticationHeaderValue

using System.Xml; //XmlDocument
using System.Xml.Linq; //XDocument
using System.Xml.Serialization; //XmlSerializer

using System.Text; //StringBuilder
using System.Collections.Specialized; //NameValueCollection

namespace WixossSimulator.SugarSync
{
    /// <summary> SugarSyncのプラットフォームAPIを利用して、HTTPサーバーと通信を行うための静的メソッドを提供します? </summary>
    public class SugarSyncHttpClient
    {
        /// <summary> HTTPサーバーに GET 要求を送信して、そのレスポンスを格納する? </summary>
        /// <param name="wrapper">  </param>
        /// <param name="address"> 送信先のアドレス。 </param>
        /// <returns></returns>
        public static async Task<SugarSyncHttpResponse> GetAsync(SugarSyncApiWrapper wrapper, string address)
        {
            return await GetAsync(wrapper, new Uri(address)); 
        }
        /// <summary> HTTPサーバーに GET 要求を送信して、そのレスポンスを格納する? </summary>
        /// <param name="wrapper">  </param>
        /// <param name="uri"> 送信先のURI。 </param>
        /// <returns></returns>
        public static async Task<SugarSyncHttpResponse> GetAsync(SugarSyncApiWrapper wrapper, Uri uri) 
        {
            return await GetAsync<object>(wrapper, uri); 
        }
        /// <summary> HTTPサーバーに GET 要求を送信して、そのレスポンスを格納する? </summary>
        /// <typeparam name="T"> レスポンスボディを格納するプロパティの型。 </typeparam>
        /// <param name="wrapper">  </param>
        /// <param name="address"> 送信先のアドレス。 </param>
        /// <returns></returns>
        public static async Task<SugarSyncHttpResponse<T>> GetAsync<T>(SugarSyncApiWrapper wrapper, string address) where T : class,new()
        {
            return await GetAsync<T>(wrapper, new Uri(address)); 
        }
        /// <summary> HTTPサーバーに GET 要求を送信して、そのレスポンスを格納する? </summary>
        /// <typeparam name="T"> レスポンスボディを格納するプロパティの型。 </typeparam>
        /// <param name="wrapper">  </param>
        /// <param name="uri"> 送信先のURI。 </param>
        /// <returns></returns>
        public static async Task<SugarSyncHttpResponse<T>> GetAsync<T>(SugarSyncApiWrapper wrapper, Uri uri) where T : class,new()
        {
            using (HttpClient client = new HttpClient()) { return await ConnectAsync<T>(wrapper, client, async () => await client.GetAsync(uri)); }
        }

        /// <summary> HTTPサーバーに、内容がXMLである POST 要求を送信して、そのレスポンスを格納する? </summary>
        /// <param name="wrapper">  </param>
        /// <param name="address"> 送信先のアドレス。 </param>
        /// <param name="request"> XML宣言を含まないリクエストボディを表すオブジェクト。 </param>
        /// <returns></returns>
        public static async Task<SugarSyncHttpResponse> PostXmlAsync(SugarSyncApiWrapper wrapper, string address, object request) 
        {
            return await PostXmlAsync(wrapper, new Uri(address), request); 
        }
        /// <summary> HTTPサーバーに、内容がXMLである POST 要求を送信して、そのレスポンスを格納する? </summary>
        /// <param name="wrapper">  </param>
        /// <param name="uri"> 送信先のURI。 </param>
        /// <param name="request"> XML宣言を含まないリクエストボディを表すオブジェクト。 </param>
        /// <returns></returns>
        public static async Task<SugarSyncHttpResponse> PostXmlAsync(SugarSyncApiWrapper wrapper, Uri uri, object request) 
        {
            return await PostXmlAsync<object>(wrapper, uri, request); 
        }
        /// <summary> HTTPサーバーに、内容がXMLである POST 要求を送信して、そのレスポンスを格納する? </summary>
        /// <typeparam name="T"> レスポンスボディを格納するプロパティの型。 </typeparam>
        /// <param name="wrapper">  </param>
        /// <param name="address"> 送信先のアドレス。 </param>
        /// <param name="request"> XML宣言を含まないリクエストボディを表すオブジェクト。 </param>
        /// <returns></returns>
        public static async Task<SugarSyncHttpResponse<T>> PostXmlAsync<T>(SugarSyncApiWrapper wrapper, string address, object request) where T : class,new() 
        {
            return await PostXmlAsync<T>(wrapper, new Uri(address), request);
        }
        /// <summary> HTTPサーバーに、内容がXMLである POST 要求を送信して、そのレスポンスを格納する? </summary>
        /// <typeparam name="T"> レスポンスボディを格納するプロパティの型。 </typeparam>
        /// <param name="wrapper">  </param>
        /// <param name="uri"> 送信先のURI。 </param>
        /// <param name="request"> XML宣言を含まないリクエストボディを表すオブジェクト。 </param>
        /// <returns></returns>
        public static async Task<SugarSyncHttpResponse<T>> PostXmlAsync<T>(SugarSyncApiWrapper wrapper, Uri uri, object request) where T : class,new()
        {
            var requestXml = new StringContent(FormatRequestXml(request));
            requestXml.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/xml");
            requestXml.Headers.ContentType.CharSet = "UTF-8";

            using (var client = new HttpClient()) { return await ConnectAsync<T>(wrapper, client, async () => await client.PostAsync(uri, requestXml)); }
        }

        /// <summary> HTTPサーバーに、内容がXMLである PUT 要求を送信して、そのレスポンスを格納する? </summary>
        /// <param name="wrapper">  </param>
        /// <param name="address"> 送信先のアドレス。 </param>
        /// <param name="request"> XML宣言を含まないリクエストボディを表すオブジェクト。 </param>
        /// <returns></returns>
        public static async Task<SugarSyncHttpResponse> PutXmlAsync(SugarSyncApiWrapper wrapper, string address, object request)
        {
            return await PutXmlAsync(wrapper, new Uri(address), request);
        }
        /// <summary> HTTPサーバーに、内容がXMLである PUT 要求を送信して、そのレスポンスを格納する? </summary>
        /// <param name="wrapper">  </param>
        /// <param name="uri"> 送信先のURI。 </param>
        /// <param name="request"> XML宣言を含まないリクエストボディを表すオブジェクト。 </param>
        /// <returns></returns>
        public static async Task<SugarSyncHttpResponse> PutXmlAsync(SugarSyncApiWrapper wrapper, Uri uri, object request)
        {
            return await PutXmlAsync<object>(wrapper, uri, request);
        }
        /// <summary> HTTPサーバーに、内容がXMLである PUT 要求を送信して、そのレスポンスを格納する? </summary>
        /// <typeparam name="T"> レスポンスボディを格納するプロパティの型。 </typeparam>
        /// <param name="wrapper">  </param>
        /// <param name="address"> 送信先のアドレス。 </param>
        /// <param name="request"> XML宣言を含まないリクエストボディを表すオブジェクト。 </param>
        /// <returns></returns>
        public static async Task<SugarSyncHttpResponse<T>> PutXmlAsync<T>(SugarSyncApiWrapper wrapper, string address, object request) where T : class,new()
        {
            return await PutXmlAsync<T>(wrapper, new Uri(address), request);
        }
        /// <summary> HTTPサーバーに、内容がXMLである PUT 要求を送信して、そのレスポンスを格納する? </summary>
        /// <typeparam name="T"> レスポンスボディを格納するプロパティの型。 </typeparam>
        /// <param name="wrapper">  </param>
        /// <param name="uri"> 送信先のURI。 </param>
        /// <param name="request"> XML宣言を含まないリクエストボディを表すオブジェクト。 </param>
        /// <returns></returns>
        public static async Task<SugarSyncHttpResponse<T>> PutXmlAsync<T>(SugarSyncApiWrapper wrapper, Uri uri, object request) where T : class,new()
        {
            var requestXml = new StringContent(FormatRequestXml(request));
            requestXml.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/xml");
            requestXml.Headers.ContentType.CharSet = "UTF-8";

            using (var client = new HttpClient()) { return await ConnectAsync<T>(wrapper, client, async () => await client.PutAsync(uri, requestXml)); }
        }

        /// <summary> HTTPサーバーに DELETE 要求を送信して、そのレスポンスを格納する? </summary>
        /// <param name="wrapper">  </param>
        /// <param name="address"> 送信先のアドレス。 </param>
        /// <returns></returns>
        public static async Task<SugarSyncHttpResponse> DeleteAsync(SugarSyncApiWrapper wrapper, string address)
        {
            return await DeleteAsync(wrapper, new Uri(address));
        }
        /// <summary> HTTPサーバーに DELETE 要求を送信して、そのレスポンスを格納する? </summary>
        /// <param name="wrapper">  </param>
        /// <param name="uri"> 送信先のURI。 </param>
        /// <returns></returns>
        public static async Task<SugarSyncHttpResponse> DeleteAsync(SugarSyncApiWrapper wrapper, Uri uri)
        {
            return await DeleteAsync<object>(wrapper, uri);
        }
        /// <summary> HTTPサーバーに DELETE 要求を送信して、そのレスポンスを格納する? </summary>
        /// <typeparam name="T"> レスポンスボディを格納するプロパティの型。 </typeparam>
        /// <param name="wrapper">  </param>
        /// <param name="address"> 送信先のアドレス。 </param>
        /// <returns></returns>
        public static async Task<SugarSyncHttpResponse<T>> DeleteAsync<T>(SugarSyncApiWrapper wrapper, string address) where T : class,new()
        {
            return await DeleteAsync<T>(wrapper, new Uri(address));
        }
        /// <summary> HTTPサーバーに DELETE 要求を送信して、そのレスポンスを格納する? </summary>
        /// <typeparam name="T"> レスポンスボディを格納するプロパティの型。 </typeparam>
        /// <param name="wrapper">  </param>
        /// <param name="uri"> 送信先のURI。 </param>
        /// <returns></returns>
        public static async Task<SugarSyncHttpResponse<T>> DeleteAsync<T>(SugarSyncApiWrapper wrapper, Uri uri) where T : class,new()
        {
            using (HttpClient client = new HttpClient()) { return await ConnectAsync<T>(wrapper, client, async () => await client.DeleteAsync(uri)); }
        }

        /// <summary> HTTPサーバーに、HTTP 要求を送信して、そのレスポンスを格納する? </summary>
        /// <typeparam name="T"> レスポンスボディを格納するプロパティの型。 </typeparam>
        /// <param name="wrapper">  </param>
        /// <param name="client"> 接続に使用するHttpClient。 </param>
        /// <param name="connectingMethod"> HTTP要求を示すメソッド。 </param>
        /// <returns></returns>
        protected internal static async Task<SugarSyncHttpResponse<T>> ConnectAsync<T>(
            SugarSyncApiWrapper wrapper, HttpClient client, Func<Task<HttpResponseMessage>> connectingMethod,
            bool isGettingAuthorization = false) where T : class,new()
        {
            if (wrapper.Expiration < DateTime.Now) { return null; }

            client.MaxResponseContentBufferSize = 1000000;

            if (!isGettingAuthorization) { client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", wrapper.Authorization); }
            //これはエラー //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(wrapper.authorization);

            //必要?
            client.DefaultRequestHeaders.AcceptCharset.Add(StringWithQualityHeaderValue.Parse("UTF-8"));
            //client.DefaultRequestHeaders.Add("Accept-Charset", "UTF-8");

            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("(Windows NT 6.3; Win64; x64)"));
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("AppleWebKit", "537.36"));
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("(KHTML, like Gecko)"));
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Chrome", "37.0.2062.124"));
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Safari", "537.36"));

            //client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mosilla", "5.0"));
            //client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("(Windows NT 6.3; Trident/7.0; rv:11.0)"));
            //client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("like Gecko"));

            //client.DefaultRequestHeaders.UserAgent.Add(ProductInfoHeaderValue.Parse("Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko"));
            //client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko");
            //client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko");

            using (var response = await connectingMethod())
            using (var content = response.Content)
            {
                //var header = response.Headers;
                //var body = await content.ReadAsStringAsync();
                //var statusCode = response.StatusCode;
                //return new SugarSyncHttpResponse<T>(header, body, statusCode);
                return new SugarSyncHttpResponse<T>(
                    response.Headers, await content.ReadAsStringAsync(), response.StatusCode);
            }
        }

        /// <summary> リクエストボディをXMLに変換する? </summary>
        /// <param name="request"> リクエストボディを表すオブジェクト。文字列型の場合にXML宣言を含めてはいけない。それ以外の型の場合は無視? </param>
        /// <returns></returns>
        protected internal static string FormatRequestXml(object request)
        {
            var requestBody = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\n";
            try
            {
                Type requestType = request.GetType();
                if (requestType == typeof(string)) { requestBody += (string)request; }
                else if (requestType == typeof(XDocument)) { requestBody += ((XDocument)request).ToString(); }
                else if (requestType == typeof(XmlDocument)) { requestBody += ((XmlDocument)request).OuterXml; }
                else
                {
                    var serializer = new XmlSerializer(requestType);
                    var namespaces = new XmlSerializerNamespaces();
                    namespaces.Add(string.Empty, string.Empty);
                    using (var writer = new StringWriter())
                    {
                        serializer.Serialize(writer, request, namespaces);
                        var xml = writer.ToString();
                        var x = "?>";
                        xml = xml.Substring(xml.IndexOf(x) + x.Length);
                        requestBody += xml;
                    }
                    //using (var writer = new StringWriter(requestBodyBuilder)) { serializer.Serialize(writer, request, namespaces); }
                }
            }
            catch { return null; }

            return requestBody;
        }
    }

    /// <summary> SugarSyncのプラットフォームAPIにおけるサーバーからのレスポンスを格納するクラス? </summary>
    public class SugarSyncHttpResponse
    {
        private static SugarSyncHttpResponse failure = new SugarSyncHttpResponse(false);
        private static SugarSyncHttpResponse success = new SugarSyncHttpResponse(true);

        /// <summary> サーバーに要求を送る前に、失敗だと判断されるときのレスポンスを取得します。 </summary>
        public static SugarSyncHttpResponse Failure { get { return failure; } }
        /// <summary> サーバーに要求を送る必要がなく、成功だと判断されるときのレスポンスを取得します。 </summary>
        public static SugarSyncHttpResponse Success { get { return success; } }

        //後から書き込み可能なフィールドを作ってはいけない(上記のプロパティがあるため)
        private HttpStatusCode statusCode;

        /// <summary> レスポンスヘッダを取得します。 </summary>
        public HttpResponseHeaders Headers { get; protected set; }
        /// <summary> レスポンスボディの文字列として取得します。 </summary>
        public string BodyString { get; protected set; }
        /// <summary> レスポンスのステータスコードを取得します。 </summary>
        public HttpStatusCode StatusCode 
        {
            get { return statusCode; }
            protected set
            {
                statusCode = value;
                IsSuccess = (int)StatusCode / 100 == 2;
            }
        }
        /// <summary> このレスポンスが成功か失敗かを取得します。 </summary>
        public bool IsSuccess { get; protected set; }

        /// <summary> レスポンスを表す新しいインスタンスを初期化します。 </summary>
        /// <param name="headers"> レスポンスヘッダ。 </param>
        /// <param name="body"> レスポンスボディ。 </param>
        /// <param name="statusCode"> ステータスコード。 </param>
        public SugarSyncHttpResponse(HttpResponseHeaders headers, string body, HttpStatusCode statusCode)
        {
            Headers = headers;
            BodyString = body;
            StatusCode = statusCode;
        }

        protected SugarSyncHttpResponse(bool isSuccess)
        {
            Headers = null;
            BodyString = null;
            StatusCode = default(HttpStatusCode);
            IsSuccess = isSuccess;
        }
    }

    /// <summary> SugarSyncのプラットフォームAPIにおけるサーバーからのレスポンスを格納するクラス? </summary>
    /// <typeparam name="T"> レスポンスボディを格納するプロパティの型。 </typeparam>
    public class SugarSyncHttpResponse<T> : SugarSyncHttpResponse where T : class, new()
    {
        /// <summary> レスポンスボディを示すクラスを取得します。 </summary>
        public T Body { get; protected set; }

        /// <summary> レスポンスを表す新しいインスタンスを初期化します。 </summary>
        /// <param name="headers"> レスポンスヘッダ。 </param>
        /// <param name="body"> レスポンスボディ。 </param>
        /// <param name="statusCode"> ステータスコード。 </param>
        public SugarSyncHttpResponse(HttpResponseHeaders headers, string body, HttpStatusCode statusCode)
            : base(headers, body, statusCode)
        {
            Body = ToBody(BodyString);
        }

        protected static T ToBody(string bodyString)
        {
            try
            {
                Type t = typeof(T);
                if (t == typeof(XDocument)) { return XDocument.Parse(bodyString) as T; }
                else if (t == typeof(XmlDocument))
                {
                    var document = new XmlDocument();
                    document.LoadXml(bodyString);
                    return document as T;
                }
                else
                {
                    var serializer = new XmlSerializer(typeof(T));
                    using (var reader = new StringReader(bodyString)) { return (T)serializer.Deserialize(reader); }
                }
            }
            catch { return null; }
        }
    }
}