using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text;

namespace wixosssimulator.components
{
    public class Stream
    {
        /// <summary> HTMLファイルの内容を文字コードを合わせて取得 </summary>
        /// <param name="path"> ローカルまたはインターネット上のアドレス </param>
        /// <returns></returns>
        public static string GetHtmlDocument(string path)
        {
            string loadingUri;
            Uri targetUri = new Uri(path);
            if (targetUri.IsFile ||
                targetUri.IsAbsoluteUri)
            {
                loadingUri = targetUri.AbsoluteUri;
            }
            else
            {
                return null;
            }

            // HTMLテキスト…？を仮に取得して文字コードをセット
            try
            {
                System.Net.WebClient client = new System.Net.WebClient();
                string tempDocument = client.DownloadString(loadingUri);
                System.Text.RegularExpressions.Regex AbstCharSet =
                    new System.Text.RegularExpressions.Regex(@"<head>.*?charset=(?<charset>.*?)[\s"";].*?</head>",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase |
                    System.Text.RegularExpressions.RegexOptions.Singleline);
                System.Text.RegularExpressions.Match MatchCharSet =
                    AbstCharSet.Match(tempDocument);
                if (MatchCharSet.Success)
                {
                    try
                    {
                        client.Encoding = Encoding.GetEncoding(MatchCharSet.Groups["charset"].Value);
                    }
                    catch
                    {
                        client.Encoding = Encoding.UTF8;
                    }
                }
                else
                {
                    client.Encoding = Encoding.UTF8;
                }
                // HTMLテキスト…？を取得
                return client.DownloadString(loadingUri);
            }
            catch
            {
                //どこかで失敗した場合はそのまま例外を投げる
                throw;
            }
        }
    }
}