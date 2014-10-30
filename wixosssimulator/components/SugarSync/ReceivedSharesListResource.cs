using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Xml.Serialization;

namespace WixossSimulator.SugarSync
{
    /// <summary>
    /// A received shares list resource represents the list of received shares that have been granted to the user. 
    /// A received share represents a shared folder owned by another user and to which this user has been granted access privileges by the owner.
    /// https://www.sugarsync.com/dev/api/received-share-list-resource.html
    /// 
    /// https://www.sugarsync.com/dev/api/received-share-resource.html
    /// </summary>
    [XmlRoot("receivedShares")]
    public class ReceivedSharesListResource
    {
        /// <summary> A link to the contact resource representing the owner of the shared folder. </summary>
        [XmlElement("receivedShare")]
        public List<ReceivedShareResource> ReceivedShare { get; set; }
    }
}