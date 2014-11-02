using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Xml.Serialization;

namespace WixossSimulator.SugarSync
{
    /// <summary>
    /// A contact resource represents another SugarSync user who has shared a folder or folders with this user.
    /// https://www.sugarsync.com/dev/api/shared-folder-contact-resource.html
    /// </summary>
    [XmlRoot("contact")]
    public class ContactResource
    {
        /// <summary> The primary email address of the contact. </summary>
        [XmlElement("primaryEmailAddress")]
        public string PrimaryEmailAddress { get; set; }

        /// <summary> The first name of the contact. </summary>
        [XmlElement("firstName")]
        public string FirstName { get; set; }

        /// <summary> The last name of the contact. </summary>
        [XmlElement("lastName")]
        public string LastName { get; set; }
    }
}