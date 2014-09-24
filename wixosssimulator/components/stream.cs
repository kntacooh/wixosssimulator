using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace WixossSimulator
{
    /// <summary> HTMLドキュメントのストリーム操作を行うための静的メソッドを提供します。 </summary>
    public class HtmlStream
    {
        /// <summary> HTMLドキュメントの内容を文字コードを合わせて取得します。 </summary>
        /// <param name="path"> ローカルまたはインターネット上のアドレスを表す絶対パス。 </param>
        /// <returns> HTMLドキュメント。 </returns>
        public static string GetDocument(string path)
        {
            Uri uri = new Uri(path);
            return GetDocument(uri);
        }
        /// <summary> HTMLドキュメントの内容を文字コードを合わせて取得します。 </summary>
        /// <param name="uri"> ローカルまたはインターネット上のアドレスを表すURIクラスのインスタンス。 </param>
        /// <returns> HTMLドキュメント。 </returns>
        public static string GetDocument(Uri uri)
        {
            if (!(uri.IsFile || uri.IsAbsoluteUri)) { throw new NotImplementedException(); }
            string address = uri.AbsoluteUri;

            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                string tempDocument = client.DownloadString(address);

                // charsetを検索
                Regex r = new System.Text.RegularExpressions.Regex(@"<head>.*?charset=""?(?<charset>.*?)[\s"";].*?</head>",
                    RegexOptions.IgnoreCase | RegexOptions.Singleline);
                Match m = r.Match(tempDocument);
                if (m.Success)
                {
                    try { client.Encoding = Encoding.GetEncoding(m.Groups["charset"].Value); }
                    catch { client.Encoding = Encoding.UTF8; }
                }
                if (client.Encoding == Encoding.UTF8) { return tempDocument; }
                else { return client.DownloadString(address); }

            }
        }
    }
}