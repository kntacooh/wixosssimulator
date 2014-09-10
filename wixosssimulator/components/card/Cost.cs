using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WixossSimulator.Card
{
    /// <summary> 効果の使用に必要なコストを色ごとに表します。 </summary>
    public class Cost : ValueForEachColor<byte>
    {
        /// <summary> 色ごとのコストを示す連想配列を指定して、新しいインスタンスを初期化します。 </summary>
        /// <param name="value"> 色ごとのコストを示す連想配列。 </param>
        public Cost(Dictionary<ColorKind, byte> value) : base(value) { }
        /// <summary> 色ごとのコストを示す配列を指定して、新しいインスタンスを初期化します。 </summary>
        /// <param name="value"> 色ごとのコストを示す配列。 </param>
        public Cost(byte[] value) : base(value) { }
    }
}