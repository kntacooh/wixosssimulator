<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm2.aspx.cs" Inherits="WixossSimulator.test.WebForm2" %>
<%@ Import Namespace="WixossSimulator.Crawler" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
<%--    <%
        string html = HtmlStream.GetDocument("http://www.takaratomy.co.jp/products/wixoss/card/card_list.php");
        Regex r = new Regex(@"<body>(.*)</body>",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);
        Match m = r.Match(html);
        if (m.Success) { Response.Write(m.Groups[1].Value); }
         %>--%>
    <form id="form1" runat="server">
    <div>
        <ol>
            <%
                WebCrawler crawler = new WebCrawler(CrawledDomain.Official);
                //crawler.Urls.Add("http://example.com/");
                crawler.SearchAllUrls();

                foreach (string url in crawler.Urls)
                {
                    Response.Write(@"<li><a href=""" + url + @""" target=""_blank"">" + url + @"</li><br>");
                }
                 %>
        </ol>
    </div>
    </form>
</body>
</html>
