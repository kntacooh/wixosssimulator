using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Net; //Web~
using System.IO; //Stream(Reader/Writer)
using System.Net.Http; //HttpClient

using System.Xml; //XmlDocument
using System.Xml.Linq; //XDocument
using System.Xml.Serialization; //XmlSerializer

using System.Text; //StringBuilder
using System.Collections.Specialized; //NameValueCollection

namespace WixossSimulator.SugarSync
{
    public class SugarSyncResponse<T> where T : class, new()
    {
        public WebHeaderCollection Header { get; protected set; }
        public T Body { get; protected set; }
        public string BodyString { get; protected set; }

        protected SugarSyncResponse(SugarSyncApiWrapper wrapper)
        {
            //if (wrapper.Expiration < DateTime.Now) { return; }
        }

        protected void SetBodyFromBodyString()
        {
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
        }
    }

    ///// <summary> HTTPサーバーにGETメソッド(GETリクエスト)でデータを送信して、そのレスポンスを格納します？ </summary>
    ///// <typeparam name="T"> レスポンスボディを格納するプロパティの型。 </typeparam>
    //public class SugarSyncResponseByGetMethod<T> : SugarSyncResponse<T> where T : class, new()
    //{
    //    /// <summary>  </summary>
    //    /// <param name="wrapper">  </param>
    //    /// <param name="url">  </param>
    //    public SugarSyncResponseByGetMethod(SugarSyncApiWrapper wrapper, string url) : this(wrapper, url, new NameValueCollection()) { }

    //    /// <summary>  </summary>
    //    /// <param name="wrapper">  </param>
    //    /// <param name="url">  </param>
    //    /// <param name="getQuery">  </param>
    //    public SugarSyncResponseByGetMethod(SugarSyncApiWrapper wrapper, string url, NameValueCollection getQuery)
    //        : base(wrapper)
    //    {
    //        if (wrapper.Expiration < DateTime.Now) { return; }

    //        using (WebClient client = new WebClient())
    //        {
    //            client.Encoding = System.Text.Encoding.UTF8;
    //            client.Headers.Add("Authorization", wrapper.authorization);
    //            client.QueryString = getQuery;

    //            try { BodyString = client.DownloadString(url); }
    //            catch (WebException e)
    //            {
    //                BodyString = null;
    //                //HttpWebResponse r = (HttpWebResponse)e.Response;
    //                //BodyString = r.StatusCode.ToString() + " " + r.StatusDescription;
    //                return;
    //            }

    //            SetBodyFromBodyString();

    //            Header = client.ResponseHeaders;
    //        }
    //    }
    //}

    /// <summary> HTTPサーバーにPOSTメソッド(POSTリクエスト)でXMLを送信して、そのレスポンスを格納します？ </summary>
    /// <typeparam name="T"> レスポンスボディを格納するプロパティの型。 </typeparam>
    public class SugarSyncResponseByPostOrPutMethod<T> : SugarSyncResponse<T> where T : class, new()
    {
        protected SugarSyncResponseByPostOrPutMethod(SugarSyncApiWrapper wrapper) : base(wrapper) { }

        /// <summary>  </summary>
        /// <param name="wrapper">  </param>
        /// <param name="url">  </param>
        /// <param name="requestBody"> XML宣言を含まないリクエストボディを表すオブジェクト。 </param>
        public SugarSyncResponseByPostOrPutMethod(SugarSyncApiWrapper wrapper, string url, object requestBody)
            : base(wrapper)
        {
            if (wrapper.Expiration < DateTime.Now) { return; }

            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                client.Headers.Add("Authorization", wrapper.authorization);
                client.Headers.Add("Content-Type", "application/xml");

                try { BodyString = client.UploadString(url, FormatRequestBody(requestBody)); }
                catch (WebException e)
                {
                    BodyString = null;
                    //HttpWebResponse r = (HttpWebResponse)e.Response;
                    //BodyString = r.StatusCode.ToString() + " " + r.StatusDescription;
                    return;
                }

                SetBodyFromBodyString();

                Header = client.ResponseHeaders;
            }
        }

        /// <summary> リクエストボディをXMLに変換? </summary>
        /// <param name="requestBody"> XML宣言を含まないリクエストボディを表すオブジェクト。 </param>
        /// <returns></returns>
        protected string FormatRequestBody(object requestBody)
        {
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
            catch { return null; }

            return requestBodyBuilder.ToString();
        }
    }

    //public class SugarSyncResponseByPutMethod : SugarSyncResponseByPostOrPutMethod<bool>
    //{
    //    /// <summary>  </summary>
    //    /// <param name="wrapper">  </param>
    //    /// <param name="url">  </param>
    //    /// <param name="requestBody"> XML宣言を含まないリクエストボディを表すオブジェクト。 </param>
    //    public SugarSyncResponseByPutMethod(SugarsyncApiWrapper wrapper, string url, object requestBody)
    //        : base(wrapper)
    //    {
    //        if (wrapper.Expiration < DateTime.Now) { return; }

    //        using (WebClient client = new WebClient())
    //        {
    //            client.Encoding = System.Text.Encoding.UTF8;
    //            client.Headers.Add("Authorization", wrapper.authorization);
    //            client.Headers.Add("Content-Type", "application/xml");

    //            try { client.UploadString(url, FormatRequestBody(requestBody)); }
    //            catch (WebException e)
    //            {
    //                Body = false;
    //                return; 
    //            }

    //            Header = client.ResponseHeaders;
    //            Body = true;
    //        }
    //    }
    //}
}