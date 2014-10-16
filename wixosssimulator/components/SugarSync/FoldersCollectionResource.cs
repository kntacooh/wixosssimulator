using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Xml.Serialization;

namespace WixossSimulator.SugarSync
{
    /// <summary> https://www.sugarsync.com/dev/api/syncfolders-list-resource.html </summary>
    [XmlRoot("collectionContents")]
    public class FoldersCollectionResource
    {
        public class FoldersCollection
        {
            /// <summary> The type of the collection (syncFolder). </summary>
            [XmlAttribute("type")]
            public string collectionType { get; set; }

            /// <summary> The user-visible name of the sync folder. </summary>
            [XmlElement("displayName")]
            public string DisplayName { get; set; }

            /// <summary> A link to the sync folder. </summary>
            [XmlElement("ref")]
            public string ReferenceUrl { get; set; }

            /// <summary> A link to the contents of the sync folder. </summary>
            [XmlElement("contents")]
            public string ContentsUrl { get; set; }
        }

        public class FoldersCollectionFile
        {
            /// <summary> The user-visible name of the file. </summary>
            [XmlElement("displayName")]
            public string DisplayName { get; set; }

            /// <summary> A link to the file. </summary>
            [XmlElement("ref")]
            public string ReferenceUrl { get; set; }

            /// <summary> The size in bytes of the file. </summary>
            [XmlElement("size")]
            public long Size { get; set; }

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
            [XmlAttribute("mediaType")]
            public string MediaType { get; set; }

            /// <summary> Whether the file is on the server (true) or not (false). </summary>
            [XmlAttribute("presentOnServer")]
            public bool PresentOnServer { get; set; }

            /// <summary> A link to the data for the file. </summary>
            [XmlAttribute("fileData")]
            public string FileData { get; set; }
        }

        [XmlAttribute("end")]
        public long End { get; set; }

        [XmlAttribute("hasMore")]
        public bool HasMore { get; set; }

        [XmlAttribute("start")]
        public long Start { get; set; }

        [XmlElement("collection")]
        public List<FoldersCollection> Collection { get; set; }

        [XmlElement("file")]
        public List<FoldersCollectionFile> File { get; set; }
    }
}