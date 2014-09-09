using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wixosssimulator.components.card
{
    /// <summary> ルリグタイプについての情報を表します。 </summary>
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

        /// <summary> ルリグタイプを示すテキストを指定して新しいインスタンスを初期化します。 </summary>
        /// <param name="text"> ルリグタイプを示すテキスト。 </param>
        public LrigType(string text) { Text = text; }
        /// <summary> 各ルリグタイプを示す文字列のリストを指定して、新しいインスタンスを初期化します。 </summary>
        /// <param name="textList"> 各ルリグタイプを示す文字列のリスト。 </param>
        public LrigType(string[] textList) { TextList = textList; }

        /// <summary> 各ルリグタイプを示す文字列のリストを、ルリグタイプを示すテキストに変換します。</summary>
        /// <param name="textList"> 各ルリグタイプを示す文字列のリスト。 </param>
        /// <returns> ルリグタイプを示すテキスト。 </returns>
        public static string ConvertListToText(string[] textList)
        {
            return string.Join("/", textList);
        }
        /// <summary> ルリグタイプを示すテキストを、各ルリグタイプを示す文字列のリストに変換します。</summary>
        /// <param name="text"> ルリグタイプを示すテキスト。 </param>
        /// <returns> 各ルリグタイプを示す文字列のリスト。 </returns>
        public static string[] ConvertTextToList(string text)
        {
            if (text == "") { return new string[0]; }
            return text.Split('/');
        }

        /// <summary> ルリグタイプを示すテキストのみを設定します。 </summary>
        /// <param name="text"> ルリグタイプを示すテキスト。 </param>
        public void SetOnlyText(string text) { SetText(text, false); }
        /// <summary> 各ルリグタイプを示す文字列のリストのみを設定します。 </summary>
        /// <param name="textList"> 各ルリグタイプを示す文字列のリスト。 </param>
        public void SetonlyTextList(string[] textList) { SetTextList(textList, false); }

        /// <summary> 各ルリグタイプを示す文字列のリストを整形してプライベートメンバに代入します。 </summary>
        /// <param name="text"> ルリグタイプを示すテキスト。 </param>
        /// <param name="isConbine"> プロパティを連動して変更させるかどうか。 </param>
        private void SetText(string text, bool isConbine)
        {
            text = text ?? "";
            this.text = text.Trim();
            if (isConbine) { SetTextList(ConvertTextToList(this.Text), false); }
        }
        /// <summary> ルリグタイプを示すテキストを整形してプライベートメンバに代入します。 </summary>
        /// <param name="textList"> 各ルリグタイプを示す文字列のリスト。 </param>
        /// <param name="isConbine"> プロパティを連動して変更させるかどうか。 </param>
        private void SetTextList(string[] textList, bool isConbine)
        {
            textList = textList ?? new string[0];
            this.textList = textList.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            if (isConbine) { SetText(ConvertListToText(this.TextList), false); }
        }

    }
}