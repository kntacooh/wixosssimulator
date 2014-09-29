using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.SqlClient;

namespace WixossSimulator.Sql
{
    public class CrawlingData : TableData
    {
        private string domain = "";
        private string domainId = "";
        private string content = "";

        public int No { get; set; }
        public string Domain 
        { 
            get { return this.domain; }
            set { this.domain = value ?? ""; }
        }
        public string DomainId
        {
            get { return this.domainId; }
            set { this.domainId = value ?? ""; }
        }
        public string Content
        {
            get { return this.content; }
            set { this.content = value ?? ""; }
        }
        public DateTime LastUpdated { get; set; }
        public DateTime LastConfirmed { get; set; }
        public DateTime? Deleted { get; set; }

        public CrawlingData() { }
        public CrawlingData(SqlDataReader reader) : base(reader) { }
    }
}