using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Xml.Serialization;

namespace WixossSimulator.SugarSync
{
    /// <summary> 
    /// A received share resource represents a shared folder owned by another user and to which this user has been granted access privileges by the owner.
    /// https://www.sugarsync.com/dev/api/received-share-resource.html
    /// 
    /// https://www.sugarsync.com/dev/api/received-share-list-resource.html
    /// </summary>
    [XmlRoot("receivedShare")]
    public class ReceivedShareResource
    {
        /// <summary> The <permissions> element has the following subelements: </summary>
        public class PermissionsElement
        {
            public class ReadAllowedElement
            {
                [XmlAttribute("enabled")]
                public bool? Enabled { get; set; }
            }

            public class WriteAllowedElement
            {
                [XmlAttribute("enabled")]
                public bool? Enabled { get; set; }
            }


            /// <summary> Indicates whether the user has read access to the shared folder (enabled="true") or does not have read access (enabled="false"). </summary>
            [XmlElement("readAllowed")]
            public ReadAllowedElement ReadAllowed { get; set; }

            /// <summary> Indicates whether the user has write access to the shared folder (enabled="true") or does not have write access (enabled="false"). </summary>
            [XmlElement("writeAllowed")]
            public WriteAllowedElement WriteAllowed { get; set; }
        }



        /// <summary> ref attribute points to the shared folder resource. </summary>
        [XmlAttribute("ref")]
        public string Ref { get; set; }

        /// <summary> The user-visible name of the folder. </summary>
        [XmlElement("displayName")]
        public string DisplayName { get; set; }

        [XmlElement("timeRecieved")]
        public string TimeRecievedString
        {
            get
            {
                if (TimeRecieved.HasValue) { return ((DateTime)TimeRecieved).ToString(SugarSyncResource.TimeFormat); }
                else { return null; }
            }
            set { TimeRecieved = DateTime.ParseExact(value, SugarSyncResource.TimeFormat, System.Globalization.DateTimeFormatInfo.InvariantInfo); }
        }
        /// <summary> A dateTime value that specifies the date and time the shared folder was received by the user. </summary>
        [XmlIgnore]
        public DateTime? TimeRecieved { get; set; }

        /// <summary> A link to the shared folder. </summary>
        [XmlElement("sharedFolder")]
        public string SharedFolder { get; set; }

        /// <summary> The level of access the user has to the shared folder. </summary>
        [XmlElement("permissions")]
        public PermissionsElement Permissions { get; set; }

        /// <summary> A link to the contact resource representing the owner of the shared folder. </summary>
        [XmlElement("owner")]
        public string Owner { get; set; }
    }
}