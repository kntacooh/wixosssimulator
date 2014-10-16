using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Xml.Serialization;

namespace WixossSimulator.SugarSync
{
    /// <summary> https://www.sugarsync.com/dev/api/auth-resource.html </summary>
    [XmlRoot("authorization")]
    public class AccessTokenResource
    {
        [XmlElement("expiration")]
        public string ExpirationString
        {
            get { return Expiration.ToString(SugarSyncResource.TimeFormat); }
            set { Expiration = DateTime.ParseExact(value, SugarSyncResource.TimeFormat, System.Globalization.DateTimeFormatInfo.InvariantInfo); }
        }
        /// <summary> A dateTime value that specifies the expiration date and time of the access token. The access token will no longer be accepted by the system after that date and time. </summary>
        [XmlIgnore]
        public DateTime Expiration { get; set; }

        /// <summary> A link to the associated user resource. </summary>
        [XmlElement("user")]
        public string User { get; set; }

        /// <summary> A link to the associated user resource. </summary>
        [XmlIgnore]
        public long UserId
        {
            get { return long.Parse(User.Replace("https://api.sugarsync.com/user/", "")); }
        }
    }
}