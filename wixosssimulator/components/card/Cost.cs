using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wixosssimulator.components.card
{
    /// <summary> 効果の使用に必要なコストを色ごとに表します。 </summary>
    public class Cost
    {
        private Dictionary<Color, byte> value;

        /// <summary> 色ごとのコストを示す連想配列を取得します。 </summary>
        public Dictionary<Color, byte> Value
        {
            get { return this.value; }
            set 
            {
                value = value ?? new Dictionary<Color, byte>();
                InitializeValue();
                foreach (KeyValuePair<Color, byte> v in value) { Value[v.Key] = v.Value; } 
            }
        }

        /// <summary> 色ごとのコストを示す連想配列を指定して新しいインスタンスを初期化します。 </summary>
        /// <param name="value"> 色ごとのコストを示す連想配列。 </param>
        public Cost(Dictionary<Color, byte> value)
        {
            InitializeValue();
            Value = value;
        }
        /// <summary> 色ごとのコストを示す配列を指定して新しいインスタンスを初期化します。 </summary>
        /// <param name="value"> 色ごとのコストを表す配列。 </param>
        public Cost(byte[] value)
        {
            InitializeValue();
            SetArray(value);
        }

        /// <summary> 色ごとのコストを配列として取得します。 </summary>
        /// <returns> 色ごとのコストを示す配列。 </returns>
        public byte[] GetArray()
        {
            int[] i = new int[Enum.GetValues(typeof(Color)).Length];
            Enum.GetValues(typeof(Color)).CopyTo(i, 0);
            byte[] value = new byte[i.Max() + 1];
            foreach (KeyValuePair<Color, byte> v in Value) { value[(int)v.Key] = v.Value; }
            return value;
        }
        /// <summary> 配列から色ごとのコストを設定します。配列の添字は、Color列挙値に対応するすべての数値を含まなければなりません。
        /// ※範囲外の要素は無視されます。また、適さない配列が指定された場合はエラーが返ります。 </summary>
        /// <param name="value"> 色ごとのコストを示す配列。 </param>
        public void SetArray(byte[] value)
        {
            if (value == null) { throw new ArgumentNullException("value"); }
            int[] i = new int[Enum.GetValues(typeof(Color)).Length];
            Enum.GetValues(typeof(Color)).CopyTo(i, 0);
            //1次元配列は常に value.GetLowerBound(0) = 0 なのだろうか?
            if (!(value.GetLowerBound(0) <= i.Min() && i.Max() <= value.GetUpperBound(0))) { throw new ArgumentException("Color列挙値に対応するすべての数値を添字に含んでいません。", "value"); }
            foreach (Color c in Enum.GetValues(typeof(Color))) { Value[c] = value[(int)c]; }
        }

        /// <summary> 色ごとのコストを示す連想配列を初期化します。 </summary>
        private void InitializeValue()
        {
            value = new Dictionary<Color, byte>();
            foreach (Color c in Enum.GetValues(typeof(Color))) { Value[c] = 0; }
        }
    }
}