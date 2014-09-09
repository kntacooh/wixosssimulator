using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wixosssimulator.components.card
{
    /// <summary> シグニの持つクラスついての情報を表します。 </summary>
    public class Class
    {
        private string text;
        private string primary;
        private string secondary;

        /// <summary> クラスを示すテキストを取得します。 </summary>
        public string Text
        {
            get { return this.text; }
            set { SetText(value, true); }
        }
        /// <summary> クラスの第１を取得します。 </summary>
        public string Primary
        {
            get { return this.primary; }
            set { SetPrimary(value, true); }
        }
        /// <summary> クラスの……2を取得します。 </summary>
        public string Secondary
        {
            get { return this.secondary; }
            set { SetSecondary(value, true); }
        }

        /// <summary> クラスを示すテキストを指定して新しいインスタンスを初期化します。 </summary>
        /// <param name="text"> クラスを示すテキスト。 </param>
        public Class(string text) { Text = text; }
        /// <summary> クラスの……1と……2を指定して新しいインスタンスを初期化します。 </summary>
        /// <param name="primary"> クラスの……1。  </param>
        /// <param name="secondary"> クラスの……2。 </param>
        public Class(string primary, string secondary)
        {
            SetOnlyText(text);
            SetOnlyPrimary(primary);
            SetOnlySecondary(secondary);
        }

        /// <summary> クラスを示すテキストから……1を抽出します。 </summary>
        /// <param name="text"> クラスを示すテキスト。 </param>
        /// <returns> クラスの……1。 </returns>
        public static string ExtractTextToPrimary(string text) 
        {
            //未実装
            return null;
        }
        /// <summary> クラスを示すテキストから……2を抽出します。 </summary>
        /// <param name="text"> クラスを示すテキスト。 </param>
        /// <returns> クラスの……2。 </returns>
        public static string ExtractTextToSecondary(string text)
        {
            //未実装
            return null;
        }
        /// <summary> クラスの……1と……2からクラスを示すテキストに変換します。 </summary>
        /// <param name="primary"> クラスの……1。 </param>
        /// <param name="secondary"> クラスの……2。 </param>
        /// <returns> クラスを示すテキスト。 </returns>
        public static string Convert___ToText(string primary, string secondary)
        {
            //未実装
            return null;
        }

        /// <summary> クラスを示すテキストのみを設定します。 </summary>
        /// <param name="text"> クラスを示すテキスト。 </param>
        public void SetOnlyText(string text) { SetText(text, false); }
        /// <summary> クラスの……1のみを設定します。 </summary>
        /// <param name="primary"> クラスの……1。 </param>
        public void SetOnlyPrimary(string primary) { SetPrimary(primary, false); }
        /// <summary> クラスの……2のみを設定します。 </summary>
        /// <param name="secondary"> クラスの……2。 </param>
        public void SetOnlySecondary(string secondary) { SetSecondary(secondary, false); }

        /// <summary> クラスを示すテキストを整形してプライベートメンバに代入します。 </summary>
        /// <param name="text"> クラスを示すテキスト。 </param>
        /// <param name="isConbine"> プロパティを連動して変更させるかどうか。 </param>
        private void SetText(string text, bool isConbine)
        {
            //未完成
            text = text ?? "";
            this.text = text.Trim();
            if (isConbine) { }
        }
        /// <summary> クラスの……1を整形してプライベートメンバに代入します。 </summary>
        /// <param name="primary"> クラスの……1。 </param>
        /// <param name="isConbine"> プロパティを連動して変更させるかどうか。 </param>
        private void SetPrimary(string primary, bool isConbine)
        {
            //未完成
            primary = primary ?? "";
            this.primary = primary.Trim();
            if (isConbine) { }
        }
        /// <summary> クラスの……2を整形してプライベートメンバに代入します。 </summary>
        /// <param name="secondary"> クラスの……2。 </param>
        /// <param name="isConbine"> プロパティを連動して変更させるかどうか。 </param>
        private void SetSecondary(string secondary, bool isConbine)
        {
            //未完成
            secondary = secondary ?? "";
            this.secondary = secondary.Trim();
            if (isConbine) { }
        }
    }
    // クラスの階層が2段階以上になる可能性があるなら、クラスの中身を再帰的に定義するかも
    // 同階層で2つ以上のクラスを持つ可能性があるなら、プロパティをクラスにしてToStringメソッドをオーバーライド
}