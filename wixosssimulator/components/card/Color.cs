using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WixossSimulator.Card
{
    /// <summary> カードの色を表します。連想配列で色ごとの値がbool型で表されます。 </summary>
    public class Color : ValueForEachColor<bool>
    {
        /// <summary> 連想配列を指定して、新しいインスタンスを初期化します。 </summary>
        /// <param name="value"> 色ごとのbool値を示す連想配列。 </param>
        public Color(Dictionary<ColorKind, bool> value) : base(value) { }
        /// <summary> 配列を指定して、新しいインスタンスを初期化します。 </summary>
        /// <param name="value"> 色ごとのbool値を示す配列。 </param>
        public Color(bool[] value) : base(value) { }
        /// <summary> 列挙値を１つだけを指定して、新しいインスタンスを初期化します。 </summary>
        /// <param name="color"> 色を表す列挙値。 </param>
        public Color(ColorKind color) : base() { this.Value[color] = true; }
    }

    /// <summary> 色ごとに値を取得する必要があるクラスの基本形を提供します。 </summary>
    public class ValueForEachColor<T>
    {
        private Dictionary<ColorKind, T> value;
        /// <summary> 連想配列を初期化するときに色ごとの値を指定しない場合の既定値を取得します。 </summary>
        private T defaultValue;

        /// <summary> 色ごとの値を示す連想配列を取得します。 </summary>
        public Dictionary<ColorKind, T> Value
        {
            get { return this.value; }
            set
            {
                value = value ?? new Dictionary<ColorKind, T>();
                InitializeValue();
                foreach (KeyValuePair<ColorKind, T> v in value) { Value[v.Key] = v.Value; }
            }
        }

        /// <summary> 新しいインスタンスを初期化します。このコンストラクタは派生クラスに継承するときのみ使用されます。 </summary>
        protected ValueForEachColor() : this(default(T)) { }
        /// <summary> 既定値を設定して、新しいインスタンスを初期化します。このコンストラクタは派生クラスに継承するときのみ使用されます。 </summary>
        /// <param name="defaultValue"> 色ごとの値を指定しない場合の既定値。 </param>
        protected ValueForEachColor(T defaultValue)
        {
            this.defaultValue = defaultValue;
            InitializeValue();
        }
        /// <summary> 色ごとの値を示す連想配列を指定して新しいインスタンスを初期化します。 </summary>
        /// <param name="value"> 色ごとの値を示す連想配列。 </param>
        protected ValueForEachColor(Dictionary<ColorKind, T> value) : this(value, default(T)) { }
        /// <summary> 既定値を設定して、色ごとの値を示す連想配列を指定して新しいインスタンスを初期化します。 </summary>
        /// <param name="value"> 色ごとの値を示す連想配列。 </param>
        /// <param name="defaultValue"> 色ごとの値を指定しない場合の既定値。 </param>
        protected ValueForEachColor(Dictionary<ColorKind, T> value, T defaultValue) : this(defaultValue) { this.Value = value; }
        /// <summary> 色ごとの値を示す配列を指定して新しいインスタンスを初期化します。 </summary>
        /// <param name="value"> 色ごとの値を表す配列。 </param>
        protected ValueForEachColor(T[] value) : this(value, default(T)) { }
        /// <summary> 既定値を設定して、色ごとの値を示す配列を指定して新しいインスタンスを初期化します。 </summary>
        /// <param name="value"> 色ごとの値を表す配列。 </param>
        /// <param name="defaultValue"> 色ごとの値を指定しない場合の既定値。 </param>
        protected ValueForEachColor(T[] value, T defaultValue) : this(defaultValue) { SetArray(value); }

        /// <summary> 色ごとの値を配列として取得します。 </summary>
        /// <returns> 色ごとの値を示す配列。 </returns>
        public T[] GetArray()
        {
            int[] enumValues = new int[Enum.GetValues(typeof(ColorKind)).Length];
            Enum.GetValues(typeof(ColorKind)).CopyTo(enumValues, 0);
            T[] value = new T[enumValues.Max() + 1];
            foreach (KeyValuePair<ColorKind, T> v in this.Value) { value[(int)v.Key] = v.Value; }
            return value;
        }
        /// <summary> 配列から色ごとの値を設定します。配列の添字は、Color列挙値に対応するすべての数値を含まなければなりません。
        /// ※範囲外の要素は無視されます。また、適さない配列が指定された場合はエラーが返ります。 </summary>
        /// <param name="value"> 色ごとの値を示す配列。 </param>
        public void SetArray(T[] value)
        {
            if (value == null) { throw new ArgumentNullException("value"); }
            int[] enumValues = new int[Enum.GetValues(typeof(ColorKind)).Length];
            Enum.GetValues(typeof(ColorKind)).CopyTo(enumValues, 0);
            //1次元配列は常に value.GetLowerBound(0) = 0 なのだろうか?
            if (!(value.GetLowerBound(0) <= enumValues.Min() && enumValues.Max() <= value.GetUpperBound(0))) { throw new ArgumentException("Color列挙値に対応するすべての数値を添字に含んでいません。", "value"); }
            foreach (ColorKind c in Enum.GetValues(typeof(ColorKind))) { this.Value[c] = value[(int)c]; }
        }

        /// <summary> 色ごとの値を示す連想配列を初期化します。 </summary>
        private void InitializeValue()
        {
            value = new Dictionary<ColorKind, T>();
            foreach (ColorKind c in Enum.GetValues(typeof(ColorKind))) { this.Value[c] = this.defaultValue; }
        }
    }

    /// <summary> カードの色 (白, 赤, 青, 緑, 黒, 無色) を表す列挙値を提供します。 </summary>
    public enum ColorKind
    {
        /// <summary> 無色 </summary>
        Colorless,
        /// <summary> 白 </summary>
        White,
        /// <summary> 赤 </summary>
        Red,
        /// <summary> 青 </summary>
        Blue,
        /// <summary> 緑 </summary>
        Green,
        /// <summary> 黒 </summary>
        Black,
    }

}