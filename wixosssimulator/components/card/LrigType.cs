using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wixosssimulator.components.card
{
    /// <summary> ルリグタイプを示すテキストと、各ルリグタイプを示す文字列のリストを表します。 </summary>
    public class LrigType
    {
        private string text;
        private string[] textList;

        /// <summary> ルリグタイプを示すテキストを取得します。 </summary>
        public string Text
        {
            get { return this.text; }
            set { SetText(value, true); }
        }
        /// <summary> 各ルリグタイプを示す文字列のリストを取得します。 </summary>
        public string[] TextList
        {
            get { return this.textList; }
            set { SetTextList(value, true); }
        }

        /// <summary> ルリグタイプを示すテキストを指定して新しいオブジェクトを初期化します。 </summary>
        /// <param name="text"> ルリグタイプを示すテキスト。 </param>
        public LrigType(string text) { Text = text; }
        /// <summary> 各ルリグタイプを示す文字列のリストを指定して、新しいオブジェクトを初期化します。 </summary>
        /// <param name="textList"> 各ルリグタイプを示す文字列のリスト。 </param>
        public LrigType(string[] textList) { TextList = textList; }

        /// <summary> 各ルリグタイプを示す文字列のリストを、ルリグタイプを示すテキストに変換します。</summary>
        public static string ConvertListToText(string[] textList)
        {
            return string.Join("/", textList);
        }
        /// <summary> ルリグタイプを示すテキストを、各ルリグタイプを示す文字列のリストに変換します。</summary>
        public static string[] ConvertTextToList(string text)
        {
            if (text == "") { return new string[0]; }
            else { return text.Split('/'); }
        }

        /// <summary> ルリグタイプを示すテキストのみを変更します。 </summary>
        public void SetSeparately(string text) { SetText(text, false); }
        /// <summary> 各ルリグタイプを示す文字列のリストのみを変更します。 </summary>
        public void SetSeparately(string[] textList) { SetText(text, false); }

        /// <summary> 各ルリグタイプを示す文字列のリストをメンバに代入します。 </summary>
        private void SetText(string text, bool isConbine)
        {
            text = text ?? "";
            this.text = text.Trim();
            if (isConbine) { SetTextList(ConvertTextToList(this.Text), false); }
        }
        /// <summary> ルリグタイプを示すテキストをメンバに代入します。 </summary>
        private void SetTextList(string[] textList, bool isConbine)
        {
            textList = textList ?? new string[0];
            this.textList = textList.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            if (isConbine) { SetText(ConvertListToText(this.TextList), false); }
        }

    }
}