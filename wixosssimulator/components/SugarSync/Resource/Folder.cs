using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Xml.Serialization;

namespace WixossSimulator.SugarSync
{
    /// <summary>
    /// A folder resource is a collection that represents a folder in a user's file system that has been selected for inclusion in the user's SugarSync account.
    /// A folder can contain files, other folders, or both. One type of folder, called a sync folder, is a top-level folder that the user has selected for syncing by SugarSync.
    /// A sync folder can contain other folders that are in the user's file system.
    /// However, the folders in a sync folder are simply "folders", and not "sync folders" themselves.
    /// https://www.sugarsync.com/dev/api/folder-resource.html 
    /// </summary>
    [XmlRoot("folder")]
    public class FolderResource
    {
        public class SharingElement
        {
            [XmlAttribute("enabled")]
            public bool? Enabled { get; set; }
        }



        /// <summary> The user-visible name of the folder. </summary>
        [XmlElement("displayName")]
        public string DisplayName { get; set; }

        /// <summary> A SugarSync identifier that uniquely identifies the folder resource. </summary>
        [XmlElement("dsid")]
        public string Dsid { get; set; }
        //[XmlIgnore] //プロパティにするなら必要
        public string GetFolderId() { return Dsid.Substring(Dsid.LastIndexOf('/') + 1); }

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

        /// <summary> A link to the parent collection (workspace or folder) that contains the folder. </summary>
        [XmlElement("parent")]
        public string Parent { get; set; }

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
        public SharingElement Sharing { get; set; }
    }

}