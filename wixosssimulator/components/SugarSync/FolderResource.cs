using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Xml.Serialization;

namespace WixossSimulator.SugarSync
{
    /// <summary> https://www.sugarsync.com/dev/api/folder-resource.html </summary>
    [XmlRoot("folder")]
    public class FolderResource
    {
        public class SharingEnabled
        {
            [XmlAttribute("enabled")]
            public bool Enabled { get; set; }
        }

        /// <summary> The user-visible name of the folder. </summary>
        [XmlElement("displayName")]
        public string DisplayName { get; set; }

        /// <summary> A SugarSync identifier that uniquely identifies the folder resource. </summary>
        [XmlElement("dsid")]
        public string Dsid { get; set; }

        [XmlElement("timeCreated")]
        public string TimeCreatedString
        {
            get
            {
                if (TimeCreated.HasValue) { return ((DateTime)TimeCreated).ToString(SugarSyncResource.TimeFormat); }
                else { return null; }
            }
            set { TimeCreated = DateTime.ParseExact(value, SugarSyncResource.TimeFormat, System.Globalization.DateTimeFormatInfo.InvariantInfo); }
        }
        /// <summary> A dateTime value that specifies the date and time the folder was created. </summary>
        [XmlIgnore]
        public DateTime? TimeCreated { get; set; }

        /// <summary> A link to the collections contained in the folder, if any. </summary>
        [XmlElement("collections")]
        public string Collections { get; set; }

        /// <summary>A link to the files contained in the folder, if any.  </summary>
        [XmlElement("files")]
        public string Files { get; set; }

        /// <summary>A link to the contents of the folder. </summary>
        [XmlElement("contents")]
        public string Contents { get; set; }

        /// <summary> Whether this is a shared folder (true) or not a shared folder (false). </summary>
        [XmlElement("sharing")]
        public SharingEnabled Sharing { get; set; }
    }

}