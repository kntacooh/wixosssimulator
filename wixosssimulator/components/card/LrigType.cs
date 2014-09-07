using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wixosssimulator.components.card
{
    /// <summary> ルリグタイプを表すクラス。 </summary>
    public class LrigType
    {
        private string text = null;
        private string[] array = null;

        /// <summary> ルリグタイプを表すテキスト </summary>
        public string Text
        {
            get { return this.text; }
            set
            {
                this.text = value;
                this.array = ConvertTextToArray(this.text);
            }
        }
        /// <summary> 各ルリグタイプの文字列を表す配列 </summary>
        public string[] Array
        {
            get { return this.array; }
            set
            {
                this.array = value;
                this.text = ConvertArrayToText(this.array);
            }
        }

        public LrigType()
        {

        }

        public LrigType(string text)
        {
            this.Text = text;
        }

        public LrigType(string[] array)
        {
            this.Array = array;
        }

        /// <summary> テキストのみを代入 </summary>
        public void SetSeparately(string text) { this.text = text; }
        /// <summary> 配列のみを代入 </summary>
        public void SetSeparately(string[] array) { this.array = array; }

        /// <summary> 配列をテキストに変換</summary>
        private static string ConvertArrayToText(string[] array)
        {
            return string.Join("/", array);
        }
        /// <summary> テキストを配列に変換</summary>
        private static string[] ConvertTextToArray(string text)
        {
            if (text == "") { return new string[0]; }
            else { return text.Split('/'); }
        }
    }
}