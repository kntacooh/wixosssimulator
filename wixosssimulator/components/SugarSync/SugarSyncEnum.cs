using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Collections.ObjectModel; //ReadOnlyDictionary

namespace WixossSimulator.SugarSync
{
    public static class SugarSyncEnumExtension
    {
        #region RetrievingFolderType
        /// <summary> SugarSyncのプラットフォームAPIで使う文字列に変換? </summary>
        /// <param name="type">  </param>
        /// <returns>  </returns>
        public static string ToApiString(this RetrievingFolderType type) { return typeApiString[type]; }

        private static ReadOnlyDictionary<RetrievingFolderType, string> typeApiString = new ReadOnlyDictionary<RetrievingFolderType, string>(
            new Dictionary<RetrievingFolderType, string>()
            {
                {RetrievingFolderType.None, ""},
                {RetrievingFolderType.Folder, "folder"},
                {RetrievingFolderType.File, "file"},
            });
        #endregion

        #region RetrievingFolderOrder
        /// <summary> SugarSyncのプラットフォームAPIで使う文字列に変換? </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static string ToApiString(this RetrievingFolderOrder order) { return orderApiString[order]; }

        private static ReadOnlyDictionary<RetrievingFolderOrder, string> orderApiString = new ReadOnlyDictionary<RetrievingFolderOrder, string>(
            new Dictionary<RetrievingFolderOrder, string>()
            {
                {RetrievingFolderOrder.None, ""},
                {RetrievingFolderOrder.Name, "name"},
                {RetrievingFolderOrder.LastModified, "last_modified"},
                {RetrievingFolderOrder.Size, "size"},
                {RetrievingFolderOrder.Extension, "extension"},
            });
        #endregion
    }

    public enum RetrievingFolderType
    {
        /// <summary>  </summary>
        None,
        /// <summary>  </summary>
        Folder,
        /// <summary>  </summary>
        File,
    }

    public enum RetrievingFolderOrder
    {
        /// <summary>  </summary>
        None,
        /// <summary>  </summary>
        Name,
        /// <summary>  </summary>
        LastModified,
        /// <summary>  </summary>
        Size,
        /// <summary>  </summary>
        Extension,
    }
}