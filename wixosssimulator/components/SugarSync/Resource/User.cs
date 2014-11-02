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
    [XmlRoot("user")]
    public class UserResource
    {
        public class QuotaElement
        {
            /// <summary> The total storage available to the user, in bytes. </summary>
            [XmlElement("limit")]
            public long? Limit { get; set; }
            /// <summary> The total storage in use by the user, in bytes. </summary>
            [XmlElement("usage")]
            public long? Usage { get; set; }
        }



        /// <summary> The user's name or email address. </summary>
        [XmlElement("username")]
        public string Username { get; set; }

        /// <summary> The user's nickname. </summary>
        [XmlElement("nickname")]
        public string Nickname { get; set; }

        /// <summary>  </summary>
        [XmlElement("salt")]
        public string Salt { get; set; }

        /// <summary> The user's current storage usage. The element has the following subelements: </summary>
        [XmlElement("quota")]
        public QuotaElement Quota { get; set; }

        /// <summary> A link to a collection listing a user's computers and devices. </summary>
        [XmlElement("workspaces")]
        public string Workspaces { get; set; }

        /// <summary> A link to a collection listing a user's top-level folders synced to SugarSync. </summary>
        [XmlElement("syncfolders")]
        public string Syncfolders { get; set; }

        /// <summary> 
        /// A link to a collection listing the user's Deleted Files folder. 
        /// The Deleted Files folder contains synced files that have been deleted by the user. Files in the Deleted Files folder can be restored. 
        /// </summary>
        [XmlElement("deleted")]
        public string Deleted { get; set; }

        /// <summary>
        /// A link to a collection listing the user's Magic Briefcase folder. 
        /// Anything the user puts into the Magic Briefcase folder is automatically backed up and synced to other devices.
        /// </summary>
        [XmlElement("magicBriefcase")]
        public string MagicBriefcase { get; set; }

        /// <summary> 
        /// A link to a collection listing the user's Web Archive folder. 
        /// The Web Archive is a special synch folder for backing up and storing files online. Files in the Web Archive are not synced to any devices.
        /// </summary>
        [XmlElement("webArchive")]
        public string WebArchive { get; set; }

        /// <summary>
        /// A link to a collection listing the user's Mobile Photos folder.
        /// The Mobile Photos folder is used to store photos (or videos) that the user uploads from a mobile device.
        /// The folder can also contain folders and other types of files if the user chooses to put them there.
        /// </summary>
        [XmlElement("mobilePhotos")]
        public string MobilePhotos { get; set; }

        /// <summary> A link to a collection listing a user's photo albums. Albums correspond to folders that contain images. </summary>
        [XmlElement("albums")]
        public string Albums { get; set; }

        /// <summary> A link to a collection listing a user's recent activities. </summary>
        [XmlElement("recentActivities")]
        public string RecentActivities { get; set; }

        /// <summary> A list that describes the shared folders that are owned by other users and to which this user has been granted access, if any. </summary>
        [XmlElement("receivedShares")]
        public string ReceivedShares { get; set; }

        /// <summary> A link to a collection listing a user's files with associated public URLs, if any. </summary>
        [XmlElement("publicLinks")]
        public string PublicLinks { get; set; }

        /// <summary> The maximum size of a file for which the user can create a public link. </summary>
        [XmlElement("maximumPublicLinkSize")]
        public long? MaximumPublicLinkSize { get; set; }
    }
}