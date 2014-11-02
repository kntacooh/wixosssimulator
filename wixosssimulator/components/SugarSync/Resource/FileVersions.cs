using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Xml.Serialization;

namespace WixossSimulator.SugarSync
{
    /// <summary>
    /// A user resource represents information about a SugarSync user,
    /// including: user name, current storage utilization, a list of the user's workspaces (computers), a list of the user's sync folders,
    /// a list of the user's shared folders, the contacts for those shared folders, and a list of the user's photo albums.
    /// https://www.sugarsync.com/dev/api/user-resource.html 
    /// </summary>
    [XmlRoot("fileVersions")]
    public class FileVersionsResource
    {
        public class FileVersionElement
        {
            /// <summary> 
            /// The size of the file in bytes.
            /// This element is displayed only if the value of the presentOnServer element is true, indicating that all of the data for the file has been uploaded. 
            /// </summary>
            [XmlElement("size")]
            public long? Size { get; set; }

            /// <summary>
            /// The number of bytes uploaded to the file.
            /// This element is displayed only if the value of the presentOnServer element is false, indicating that not all of the data for the file has been uploaded.
            /// </summary>
            [XmlElement("storedSize")]
            public long? StoredSize { get; set; }

            [XmlElement("timeCreated")]
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

            /// <summary> Indicates whether all the data for the file was uploaded to the server (true) or not (false). </summary>
            [XmlElement("presentOnServer")]
            public bool? PresentOnServer { get; set; }

            /// <summary> A link to the data in the file. </summary>
            [XmlElement("fileData")]
            public string FileData { get; set; }

            /// <summary> A pointer to the file version. </summary>
            [XmlElement("ref")]
            public string Ref { get; set; }
        }



        [XmlAttribute("start")]
        public long? Start { get; set; }

        [XmlAttribute("hasMore")]
        public bool? HasMore { get; set; }

        [XmlAttribute("end")]
        public long? End { get; set; }

        [XmlElement("fileVersion")]
        public List<FileVersionElement> FileVersion { get; set; }
    }
}