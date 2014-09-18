using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Reflection; //リフレクション

namespace WixossSimulator.Card
{
    /// <summary> それぞれのカードに含まれるすべての情報を表します。 </summary>
    public class Card
    {
        private TypeKind type;
        /// <summary> カードの種類に応じて呼び出されるメソッドを取得します。 </summary>
        private ITypeStrategy TypeStrategy;

        public string Id { get; set; }
        public string Kana { get; set; } // 全角カナのみのチェックを行う?
        public string Illust { get; set; }
        public Rarity Rarity { get; set; }
        public string FlevorText { get; set; }

        public string Name { get; set; }

        public TypeKind Type 
        {
            get { return this.type; }
            set
            {
                this.type = value;
                switch (this.Type)
                {
                    case TypeKind.Lrig:
                        TypeStrategy = new LrigStrategy();
                        break;
                    case TypeKind.Arts:
                        TypeStrategy = new ArtsStrategy();
                        break;
                    case TypeKind.Signi:
                        TypeStrategy = new SigniStrategy();
                        break;
                    case TypeKind.Spell:
                        TypeStrategy = new SpellStrategy();
                        break;
                    default:
                        TypeStrategy = new CardStrategy();
                        break;
                }
            }
        }

        public Color Color { get; set; }
        public string Text { get; set; }
        public byte? Level { get; set; }
        public Cost Cost { get; set; }
        public Cost GrowCost { get; set; }
        public byte? Limit { get; set; }
        public LrigType LrigType { get; set; }
        public LrigType Condition { get; set; }
        public Class Class { get; set; }
        public int? Power { get; set; }

        public Ability[] Ability { get; set; }

        public Card()
        {
            foreach (PropertyInfo p in this.GetType().GetProperties())
            {
                p.SetValue(this, null);
            }
        }

        /// <summary> このオブジェクトがカードとして有効なものかどうか </summary>
        public bool IsValid() { return TypeStrategy.IsValid(this); }
    }

    interface ITypeStrategy
    {
        bool IsValid(Card card);
    }

    class CardStrategy : ITypeStrategy
    {
        public bool IsValid(Card card)
        {
            if (card.Id == null) { return false; }
            return true;
        }
    }

    class LrigStrategy : ITypeStrategy
    {
        public bool IsValid(Card card) 
        {

            return false;
        }
    }

    class ArtsStrategy : ITypeStrategy
    {
        public bool IsValid(Card card)
        {
            return false;
        }
    }

    class SigniStrategy : ITypeStrategy
    {
        public bool IsValid(Card card)
        {
            return false;
        }
    }

    class SpellStrategy : ITypeStrategy
    {
        public bool IsValid(Card card)
        {
            return false;
        }
    }
}
