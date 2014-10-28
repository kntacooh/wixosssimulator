using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Threading.Tasks; //Task

//using System.Net; //Web~
using System.IO; //Stream(Reader/Writer)
using System.Net.Http; //HttpClient
using System.Net.Http.Headers; //AuthenticationHeaderValue

using System.Xml; //XmlDocument
using System.Xml.Linq; //XDocument
using System.Xml.Serialization; //XmlSerializer

using System.Text; //StringBuilder
using System.Collections.Specialized; //NameValueCollection

namespace WixossSimulator.SugarSync
{
    using System.Net;

    public class SugarSyncHttpClient
    {
        public static async Task<SugarSyncHttpResponse> GetAsync(SugarSyncApiWrapper wrapper, Uri uri)
        {
            return await GetAsync<object>(wrapper, uri);
        }

        public static async Task<SugarSyncHttpResponse<T>> GetAsync<T>(SugarSyncApiWrapper wrapper, Uri uri) where T : class,new()
        {
            if (wrapper.Expiration < DateTime.Now) { return null; }

            //using (var client = AllocateClient(wrapper.authorization))
            //{
            //    using (var response = await client.GetAsync(uri))
            //    using (var content = response.Content)
            //    {
            //        return new SugarSyncHttpResponse<T>(
            //            response.Headers, await content.ReadAsStringAsync(), response.StatusCode);
            //    }
            //}

            using (HttpClient client = new HttpClient())
            {
                client.MaxResponseContentBufferSize = 1000000;

                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", wrapper.authorization);
                //これはエラー //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(wrapper.authorization);

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

                using (var response = await client.GetAsync(uri))
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
        }

        public static async Task<SugarSyncHttpResponse> PostXmlAsync(SugarSyncApiWrapper wrapper, Uri uri, object request)
        {
            return await PostXmlAsync<object>(wrapper, uri, request);
        }

        public static async Task<SugarSyncHttpResponse<T>> PostXmlAsync<T>(SugarSyncApiWrapper wrapper, Uri uri, object request) where T : class,new()
        {
            if (wrapper.Expiration < DateTime.Now) { return null; }

            //using (var client = new HttpClient())
            //{
            //    AllocateClient(client, wrapper.authorization);

            //    //client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/xml");
            //    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            //    var requestXml = new StringContent(FormatRequestXml(request));
            //    requestXml.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/xml");
            //    requestXml.Headers.ContentType.CharSet = "UTF-8";

            //    using (var response = await client.PostAsync(uri, requestXml))
            //    using (var content = response.Content)
            //    {
            //        return new SugarSyncHttpResponse<T>(
            //            response.Headers, await content.ReadAsStringAsync(), response.StatusCode);
            //    }
            //}

            using (HttpClient client = new HttpClient())
            {
                client.MaxResponseContentBufferSize = 1000000;

                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", wrapper.authorization);

                client.DefaultRequestHeaders.AcceptCharset.Add(StringWithQualityHeaderValue.Parse("UTF-8"));

                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("(Windows NT 6.3; Win64; x64)"));
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("AppleWebKit", "537.36"));
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("(KHTML, like Gecko)"));
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Chrome", "37.0.2062.124"));
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Safari", "537.36"));

                //client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/xml");
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                var requestXml = new StringContent(FormatRequestXml(request));
                requestXml.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/xml");
                requestXml.Headers.ContentType.CharSet = "UTF-8";

                using (var response = await client.PostAsync(uri, requestXml))
                using (var content = response.Content)
                {

                    return new SugarSyncHttpResponse<T>(
                        response.Headers, await content.ReadAsStringAsync(), response.StatusCode);
                }
            }
        }

        protected static async Task<SugarSyncHttpResponse<T>> HttpAsync<T>(SugarSyncApiWrapper wrapper, Uri uri, object request) where T : class,new()
        {
            return null;
        }

        protected static HttpClient AllocateClient(HttpClient client, string authorization)
        {
            client.MaxResponseContentBufferSize = 1000000;

            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorization);
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

            return client;
        }

        /// <summary> リクエストボディをXMLに変換する? </summary>
        /// <param name="request"> リクエストボディを表すオブジェクト。文字列型の場合にXML宣言を含めてはいけない。それ以外の型の場合は無視? </param>
        /// <returns></returns>
        protected static string FormatRequestXml(object request)
        {
            StringBuilder requestBodyBuilder = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            try
            {
                Type requestBodyType = request.GetType();
                if (requestBodyType == typeof(string)) { requestBodyBuilder.Append((string)request); }
                else if (requestBodyType == typeof(XDocument)) { requestBodyBuilder.Append(((XDocument)request).ToString()); }
                else if (requestBodyType == typeof(XmlDocument)) { requestBodyBuilder.Append(((XmlDocument)request).OuterXml); }
                else
                {
                    XmlSerializer serializer = new XmlSerializer(requestBodyType);
                    using (StringWriter writer = new StringWriter(requestBodyBuilder)) { serializer.Serialize(writer, request); } // StringBuilder初期化時のXML宣言は有効?
                }
            }
            catch { return null; }

            return requestBodyBuilder.ToString();
        }
    }

    public class SugarSyncHttpResponse
    {
        public HttpResponseHeaders Headers { get; protected set; }
        public string BodyString { get; protected set; }
        public HttpStatusCode StatusCode { get; protected set; }

        public SugarSyncHttpResponse(HttpResponseHeaders headers, string body, HttpStatusCode statusCode)
        {
            Headers = headers;
            BodyString = body;
            StatusCode = statusCode;
        }
    }

    public class SugarSyncHttpResponse<T> : SugarSyncHttpResponse where T : class, new()
    {
        public T Body { get; private set; }

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
                    XmlDocument document = new XmlDocument();
                    document.LoadXml(bodyString);
                    return document as T;
                }
                else
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    using (StringReader reader = new StringReader(bodyString)) { return (T)serializer.Deserialize(reader); }
                }
            }
            catch { return null; }
        }
    }
}