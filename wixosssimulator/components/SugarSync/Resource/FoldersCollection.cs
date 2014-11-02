using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Xml.Serialization;

namespace WixossSimulator.SugarSync
{
    /// <summary>
    /// A sync folders collection resource represents the list of sync folders in the user's SugarSync account.
    /// An individual sync folder is a top-level folder that the user has selected for syncing by SugarSync. 
    /// A sync folder can contain other folders that are in the user's file system.
    /// However, the folders in a sync folder are simply "folders", and not "sync folders" themselves.
    /// https://www.sugarsync.com/dev/api/syncfolders-list-resource.html

    /// A folder resource is a collection that represents a folder in a user's file system that has been selected for inclusion in the user's SugarSync account.
    /// A folder can contain files, other folders, or both. One type of folder, called a sync folder, is a top-level folder that the user has selected for syncing by SugarSync.
    /// https://www.sugarsync.com/dev/api/folder-resource.html
    /// </summary>
    [XmlRoot("collectionContents")]
    public class FoldersCollectionResource
    {
        public class CollectionElement
        {
            /// <summary> The type of the collection (type="folder" or type="syncFolder"). </summary>
            [XmlAttribute("type")]
            public string collectionType { get; set; }

            /// <summary> The user-visible name of the folder or the sync folder. </summary>
            [XmlElement("displayName")]
            public string DisplayName { get; set; }

            /// <summary> A link to the folder or the sync folder. </summary>
            [XmlElement("ref")]
            public string Ref { get; set; }

            /// <summary> A link to the contents of the folder or the sync folder. </summary>
            [XmlElement("contents")]
            public string ContentsUrl { get; set; }
        }

        public class FileElement
        {
            /// <summary> The user-visible name of the file. </summary>
            [XmlElement("displayName")]
            public string DisplayName { get; set; }

            /// <summary> A link to the file. </summary>
            [XmlElement("ref")]
            public string Ref { get; set; }

            /// <summary> The size in bytes of the file. </summary>
            [XmlElement("size")]
            public long? Size { get; set; }

            [XmlElement("lastModified")]
            public string LastModifiedString
            {
                get
                {
                    if (LastModified.HasValue) { return ((DateTime)LastModified).ToString(SugarSyncResource.TimeFormat); }
                    else { return null; }
                }
                set { LastModified = DateTime.ParseExact(value, SugarSyncResource.TimeFormat, System.Globalization.DateTimeFormatInfo.InvariantInfo); }
            }
            /// <summary> A dateTime value that specifies the date and time the file was last modified. </summary>
            [XmlIgnore]
            public DateTime? LastModified { get; set; }

            /// <summary> The media type of the file, such as image/jpeg. </summary>
            [XmlElement("mediaType")]
            public string MediaType { get; set; }

            /// <summary> Whether the file is on the server (true) or not (false). </summary>
            [XmlElement("presentOnServer")]
            public bool? PresentOnServer { get; set; }

            /// <summary> A link to the data for the file. </summary>
            [XmlElement("fileData")]
            public string FileData { get; set; }
        }



        [XmlAttribute("end")]
        public long? End { get; set; }

        [XmlAttribute("hasMore")]
        public bool? HasMore { get; set; }

        [XmlAttribute("start")]
        public long? Start { get; set; }

        [XmlElement("collection")]
        public List<CollectionElement> Collection { get; set; }

        [XmlElement("file")]
        public List<FileElement> File { get; set; }
    }
}