using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.SqlClient;

using WixossSimulator.Crawling; //DomainAttributeクラス
using Newtonsoft.Json; //JsonProperty

namespace WixossSimulator.Sql
{
    public class CrawlingData : TableData
    {
        //全てのプロパティをnull許容にしておきたい
        [JsonProperty("id")]
        public int? No { get; private set; }
        [JsonProperty("domain")]
        public string Domain { get; set; }
        [JsonProperty("domainId")]
        public string DomainId { get; set; }
        [JsonProperty("lastUpdated")]
        public DateTime? LastUpdated { get; set; }
        [JsonProperty("lastConfirmed")]
        public DateTime? LastConfirmed { get; set; }
        [JsonProperty("deleted")]
        public DateTime? Deleted { get; set; }

        [JsonProperty("url")]
        public string Url
        {
            get
            {
                DomainAttribute domainAttribute = new DomainAttribute(this.Domain);
                return domainAttribute.ToUrl(this.DomainId);
            }
        }

        //[JsonIgnore]
        //private string domain = "";
        //[JsonIgnore]
        //private string domainId = "";
        //[JsonProperty("domain")]
        //public string Domain
        //{
        //    get { return this.domain; }
        //    set { this.domain = value ?? ""; }
        //}
        //[JsonProperty("domainId")]
        //public string DomainId
        //{
        //    get { return this.domainId; }
        //    set { this.domainId = value ?? ""; }
        //}
        
        public CrawlingData() : base() { }
        public CrawlingData(SqlDataReader reader) : base(reader) { }
    }
}