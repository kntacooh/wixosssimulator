using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Text.RegularExpressions;

namespace wixosssimulator.test
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        public string Html { get; set; }
        public string Div { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Html = wixosssimulator.components.Stream.GetHtmlDocument("http://www.takaratomy.co.jp/products/wixoss/card/card_detail.php?id=251");
            Regex r = new Regex(@"<div class=""card_detail"">(.*)</div>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = r.Match(Html);
            if (m.Success) { this.Div = m.Groups[1].Value; }
        }
    }
}