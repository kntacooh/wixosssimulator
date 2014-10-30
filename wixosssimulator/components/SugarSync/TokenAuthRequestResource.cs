using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Xml.Serialization;

namespace WixossSimulator.SugarSync
{
    /// <summary>
    /// https://www.sugarsync.com/dev/api/method/create-auth-token.html
    /// https://www.sugarsync.com/dev/getting-started.html#apiauth
    /// </summary>
    [XmlRoot("tokenAuthRequest")]
    public class TokenAuthRequestResource
    {
        /// <summary> The application developer's access key ID. </summary>
        [XmlElement("accessKeyId")]
        public string AccessKeyId { get; set; }

        /// <summary> The application developer's private access key. </summary>
        [XmlElement("privateAccessKey")]
        public string PrivateAccessKey { get; set; }

        /// <summary> The refresh token. </summary>
        [XmlElement("refreshToken")]
        public string RefreshToken { get; set; }
    }
}