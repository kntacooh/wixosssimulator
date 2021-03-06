﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Xml.Serialization;

namespace WixossSimulator.SugarSync
{
    /// <summary>
    /// A workspace resource is a collection that represents a device, such as a laptop or desktop computer, in the user's SugarSync account.
    /// A workspace can only contain sync folders. It cannot contain files or other types of collections.
    /// https://www.sugarsync.com/dev/api/ws-resource.html 
    /// </summary>
    [XmlRoot("workspace")]
    public class WorkspaceResource
    {
        /// <summary> The user-visible name of the workspace. </summary>
        [XmlElement("displayName")]
        public string DisplayName { get; set; }

        /// <summary> A SugarSync identifier that uniquely identifies the workspace resource. </summary>
        [XmlElement("dsid")]
        public string Dsid { get; set; }
        //[XmlIgnore] //プロパティにするなら必要
        public string GetWorkspaceId() { return Dsid.Substring(Dsid.LastIndexOf('/') + 1); }

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
        /// <summary> A dateTime value that specifies the date and time the workspace was created. </summary>
        [XmlIgnore]
        public DateTime? TimeCreated { get; set; }

        /// <summary> A link to the collections, that is, to the sync folders contained in the workspace, if any. </summary>
        [XmlElement("collections")]
        public string Collections { get; set; }

        /// <summary>A link to the contents of the workspace, that is, to the sync folders contained in the workspace, if any.  </summary>
        [XmlElement("contents")]
        public string Contents { get; set; }

        /// <summary> The identifier of the icon associated with the workspace resource. Note that the icons that represent resources are not accessible through the Platform API. </summary>
        [XmlElement("iconID")]
        public string IconId { get; set; }
    }
}