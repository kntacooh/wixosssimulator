using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Xml.Serialization;

namespace WixossSimulator.SugarSync
{
    /// <summary>
    /// https://www.sugarsync.com/dev/api/method/create-refresh-token.html
    /// </summary>
    [XmlRoot("appAuthorization")]
    public class AppAuthorizationResource
    {
        /// <summary> The user's email address. This is the email address that the user enters when accessing his or her SugarSync account. </summary>
        [XmlElement("username")]
        public string Username { get; set; }

        /// <summary> The user's password. This is the password that the user enters when accessing his or her SugarSync account. </summary>
        [XmlElement("password")]
        public string Password { get; set; }

        /// <summary> 
        /// The application ID. This is the ID that was assigned when the application was created using the Developer Console.
        /// For instructions on creating an app, see Creating an Application. 
        /// </summary>
        [XmlElement("application")]
        public string Application { get; set; }

        /// <summary> The application developer's access key ID. </summary>
        [XmlElement("accessKeyId")]
        public string AccessKeyId { get; set; }

        /// <summary> The application developer's private access key. </summary>
        [XmlElement("privateAccessKey")]
        public string PrivateAccessKey { get; set; }
    }
}