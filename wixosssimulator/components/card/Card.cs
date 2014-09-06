using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Reflection;

namespace wixosssimulator.components.card
{
    /// <summary> それぞれのカードに含まれるすべての情報を表すクラス。 </summary>
    public class Card
    {
        public string Id { get; set; }
        public string NameKana { get; set; } // 全角カナのみのチェックを行う?
        public string Illust { get; set; }
        public Rarity Rarity { get; set; }

        public string Name { get; set; }
        public Type Type { get; set; }
        public Color Color { get; set; }
        public string Text { get; set; }
        public byte Level { get; set; }
        public Cost Cost { get; set; }
        public Cost GrowCost { get; set; }
        public byte Limit { get; set; }
        public LrigType LrigType { get; set; }
        public LrigType Condition { get; set; }
        public Class Class { get; set; }
        public int Power { get; set; }

        public Ability Ability { get; set; }

        public Card()
        {
            foreach (PropertyInfo p in this.GetType().GetProperties())
            {
                p.SetValue(this, null);
            }
        }

        /// <summary> このオブジェクトがカードとして有効なものかどうか </summary>
        public bool IsValid()
        {
            return false;
        }
    }
}