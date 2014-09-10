using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WixossSimulator.Card
{
    /// <summary> カードの種類 (ルリグ, アーツ, シグニ, スペル) を表す列挙値を提供します。 </summary>
    public enum TypeKind
    {
        /// <summary> ルリグ </summary>
        Lrig,
        /// <summary> アーツ </summary>
        Arts,
        /// <summary> シグニ </summary>
        Signi,
        /// <summary> スペル </summary>
        Spell,
    }
}