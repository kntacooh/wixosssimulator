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

        /// <summary> クラスの第１分類と第２分類の間の文字を表す配列を取得します。 delimiter[0] が標準の文字です。 </summary>
        private static char[] delimiter = { '：', ':' }; // public static readonly にしても配列の内容は変更できるので public 指定は無理。

        /// <summary> クラスを示すテキストを取得します。 </summary>
        public string Text
        {
            get { return this.text; }
            set { SetText(value, true); }
        }
        /// <summary> クラスの第１分類を示すテキストを取得します。 </summary>
        public string Primary
        {
            get { return this.primary; }
            set { SetPrimary(value, true); }
        }
        /// <summary> クラスの第２分類を示すテキストを取得します。 </summary>
        public string Secondary
        {
            get { return this.secondary; }
            set { SetSecondary(value, true); }
        }

        /// <summary> クラスを示すテキストを指定して新しいインスタンスを初期化します。 </summary>
        /// <param name="text"> クラスを示すテキスト。 </param>
        public Class(string text) { this.Text = text; }
        /// <summary> クラスの第１分類と第２分類を指定して新しいインスタンスを初期化します。 </summary>
        /// <param name="primary"> クラスの第１分類を示すテキスト。  </param>
        /// <param name="secondary"> ククラスの第２分類を示すテキスト。 </param>
        public Class(string primary, string secondary)
        {
            SetOnlyPrimary(primary);
            SetOnlySecondary(secondary);
            SetOnlyText(ConvertToText(this.Primary, this.Secondary));
        }

        /// <summary> クラスを示すテキストから第１分類を抽出します。 </summary>
        /// <param name="text"> クラスを示すテキスト。 </param>
        /// <returns> クラスの第１分類を示すテキスト。 </returns>
        public static string ExtractTextToPrimary(string text) 
        {
            int i = text.IndexOfAny(delimiter);
            if (i == -1) { return text; }
            else { return text.Substring(0, i); }
        }
        /// <summary> クラスを示すテキストから第２分類を抽出します。 </summary>
        /// <param name="text"> クラスを示すテキスト。 </param>
        /// <returns> クラスの第２分類を示すテキスト。 </returns>
        public static string ExtractTextToSecondary(string text)
        {
            int i = text.IndexOfAny(delimiter);
            if (i == -1) { return ""; }
            else { return text.Substring(i + 1); }
        }
        /// <summary> クラスの第１分類と第２分類からクラスを示すテキストに変換します。 </summary>
        /// <param name="primary"> クラスの第１分類を示すテキスト。 </param>
        /// <param name="secondary"> クラスの第２分類を示すテキスト。 </param>
        /// <returns> クラスを示すテキスト。 </returns>
        public static string ConvertToText(string primary, string secondary)
        {
            if (string.IsNullOrEmpty(secondary)) { return primary; }
            else { return primary + delimiter[0] + secondary; }
        }

        /// <summary> クラスを示すテキストのみを設定します。 </summary>
        /// <param name="text"> クラスを示すテキスト。 </param>
        public void SetOnlyText(string text) { SetText(text, false); }
        /// <summary> クラスの第１分類のみを設定します。 </summary>
        /// <param name="primary"> クラスの第１分類を示すテキスト。 </param>
        public void SetOnlyPrimary(string primary) { SetPrimary(primary, false); }
        /// <summary> クラスの第２分類のみを設定します。 </summary>
        /// <param name="secondary"> クラスの第２分類を示すテキスト。 </param>
        public void SetOnlySecondary(string secondary) { SetSecondary(secondary, false); }

        /// <summary> クラスを示すテキストを整形してプライベートメンバに代入します。 </summary>
        /// <param name="text"> クラスを示すテキスト。 </param>
        /// <param name="isConbine"> プロパティを連動して変更させるかどうか。 </param>
        private void SetText(string text, bool isConbine)
        {
            this.text = text ?? "";
            if (isConbine)
            {
                SetPrimary(ExtractTextToPrimary(this.Text), false);
                SetSecondary(ExtractTextToSecondary(this.Text), true);
            }
        }
        /// <summary> クラスの第１分類を整形してプライベートメンバに代入します。 </summary>
        /// <param name="primary"> クラスの第１分類を示すテキスト。 </param>
        /// <param name="isConbine"> プロパティを連動して変更させるかどうか。 </param>
        private void SetPrimary(string primary, bool isConbine)
        {
            primary = primary ?? "";
            this.primary = primary.Trim();
            if (isConbine) { SetText(ConvertToText(this.Primary, this.Secondary), false); }
        }
        /// <summary> クラスの第２分類を整形してプライベートメンバに代入します。 </summary>
        /// <param name="secondary"> クラスの第２分類を示すテキスト。 </param>
        /// <param name="isConbine"> プロパティを連動して変更させるかどうか。 </param>
        private void SetSecondary(string secondary, bool isConbine)
        {
            secondary = secondary ?? "";
            this.secondary = secondary.Trim();
            if (isConbine) { SetText(ConvertToText(this.Primary, this.Secondary), false); }
        }
    }
    // クラスの階層が2段階以上になる可能性があるなら、クラスの中身を再帰的に定義するかも
    // 同階層で2つ以上のクラスを持つ可能性があるなら、プロパティをクラスにしてToStringメソッドをオーバーライド
}