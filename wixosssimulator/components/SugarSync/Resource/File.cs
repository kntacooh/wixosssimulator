using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Xml.Serialization;

namespace WixossSimulator.SugarSync
{
    //FileResourceは、特定のユーザーのファイルを表します。
    /// <summary> 
    /// A file resource represents a specific user file.
    /// https://www.sugarsync.com/dev/api/file-resource.html 
    /// </summary>
    [XmlRoot("file")]
    public class FileResource
    {
        public class PublicLinkElement
        {
            [XmlAttribute("enabled")]
            public bool? Enabled { get; set; }
        }

        public class ImageElement
        {
            /// <summary> The height in pixels of the image. </summary>
            [XmlElement("height")]
            public long? Height { get; set; }

            /// <summary> The width in pixels of the image. </summary>
            [XmlElement("width")]
            public long? Width { get; set; }

            /// <summary> The angular rotation of the image. </summary>
            [XmlElement("rotation")]
            public string Rotation { get; set; }
        }



        /// <summary> The user-visible name of the file. </summary>
        [XmlElement("displayName")]
        public string DisplayName { get; set; }

        /// <summary> A SugarSync identifier that uniquely identifies the file resource. </summary>
        [XmlElement("dsid")]
        public string Dsid { get; set; }
        //[XmlIgnore] //プロパティにするなら必要
        public string GetFileId() { return Dsid.Substring(Dsid.LastIndexOf('/') + 1); }

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
        /// <summary> A dateTime value that specifies the date and time the file was created. </summary>
        [XmlIgnore]
        public DateTime? TimeCreated { get; set; }

        /// <summary> A link to the parent folder that contains the file. </summary>
        [XmlElement("parent")]
        public string Parent { get; set; }

        /// <summary> The size of the file. </summary>
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

        /// <summary> A link to the data in the file. </summary>
        [XmlElement("fileData")]
        public string FileData { get; set; }

        /// <summary> A pointer to the version history of the file. See Retrieving Version History for further information. </summary>
        [XmlElement("versions")]
        public string Versions { get; set; }

        /// <summary> Indicates whether a public link exists for the file (enabled="true") or no public link exists (enabled="false"). </summary>
        [XmlElement("publicLink")]
        public PublicLinkElement PublicLink { get; set; }

        /// <summary> Indicates that this is an image file. </summary>
        [XmlElement("image")]
        public ImageElement Image { get; set; }
    }
}