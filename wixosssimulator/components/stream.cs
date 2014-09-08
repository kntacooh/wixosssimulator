using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace wixosssimulator.components
{
    public class Stream
    {
        /// <summary> HTMLファイルの内容を文字コードを合わせて取得します。 </summary>
        /// <param name="path"> ローカルまたはインターネット上のアドレス。 </param>
        /// <returns></returns>
        public static string GetHtmlDocument(string path)
        {
            Uri pathUri = new Uri(path);
            if (!(pathUri.IsFile || pathUri.IsAbsoluteUri)) return null;
            string address = pathUri.AbsoluteUri;

            try
            {
                // 仮にHTMLドキュメントを取得
                WebClient client = new WebClient();
                string tempDocument = client.DownloadString(address);

                // charsetを検索
                Regex r = new System.Text.RegularExpressions.Regex(@"<head>.*?charset=(?<charset>.*?)[\s"";].*?</head>",
                    RegexOptions.IgnoreCase | RegexOptions.Singleline);
                Match m = r.Match(tempDocument);

                if (m.Success)
                {
                    try { client.Encoding = Encoding.GetEncoding(m.Groups["charset"].Value); }
                    catch { client.Encoding = Encoding.UTF8; }
                }
                else
                {
                    client.Encoding = Encoding.UTF8;
                }
                // HTMLテキスト…？を取得
                return client.DownloadString(address);
            }
            catch
            {
                //どこかで失敗した場合はそのまま例外を投げる
                throw;
            }
        }
    }
}