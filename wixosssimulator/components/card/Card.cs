using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Reflection;

namespace wixosssimulator.components.card
{
    /// <summary> それぞれのカードに含まれるすべての情報を表します。 </summary>
    public class Card
    {
        public string Id { get; set; }
        public string Kana { get; set; } // 全角カナのみのチェックを行う?
        public string Illust { get; set; }
        public Rarity Rarity { get; set; }
        public string FlevorText { get; set; }

        public string Name { get; set; }

        public Type Type { get; set; }

        public Color Color { get; set; }
        public string Text { get; set; }
        private byte level;
        public byte Level
        {
            get 
            { 
                if (this.Type == card.Type.Arts
                    || this.Type == card.Type.Spell)
                {
                    throw new MissingMemberException("このTypeではLevelを取得できません。"); 
                }
                return this.level;
            }
            set
            {

            }
        }
        public Cost Cost { get; set; }
        public Cost GrowCost { get; set; }
        public byte Limit { get; set; }
        public LrigType LrigType { get; set; }
        public LrigType Condition { get; set; }
        public Class Class { get; set; }
        public int Power { get; set; }

        public Ability[] Ability { get; set; }

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

    //public class CardAbstract
    //{
    //    private string Id { get; set; }
    //    private string Kana { get; set; } // 全角カナのみのチェックを行う?
    //    private string Illust { get; set; }
    //    private Rarity Rarity { get; set; }
    //    private string FlevorText { get; set; }

    //    private string Name { get; set; }

    //    private Type Type { get; set; }

    //    private Color Color { get; set; }
    //    private string Text { get; set; }
    //    private byte Level { get; set; }
    //    private Cost Cost { get; set; }
    //    private Cost GrowCost { get; set; }
    //    private byte Limit { get; set; }
    //    private LrigType LrigType { get; set; }
    //    private LrigType Condition { get; set; }
    //    private Class Class { get; set; }
    //    private int Power { get; set; }

    //    private Ability[] Ability { get; set; }

    //    public CardAbstract()
    //    {
    //        foreach (PropertyInfo p in this.GetType().GetProperties())
    //        {
    //            p.SetValue(this, null);
    //        }
    //    }

    //    /// <summary> このオブジェクトがカードとして有効なものかどうか </summary>
    //    public bool IsValid()
    //    {

    //        return false;
    //    }
    //}

    public class Lrig : Card
    {
        public string Id { get; set; }
        public string Kana { get; set; } // 全角カナのみのチェックを行う?
        public string Illust { get; set; }
        public Rarity Rarity { get; set; }
        public string FlevorText { get; set; }

        public string Name { get; set; }

        public Type Type { get; set; }

        public Color Color { get; set; }
        public string Text { get; set; }
        public byte Level { get; set; }
        private Cost Cost { get; set; }
        public Cost GrowCost { get; set; }
        public byte Limit { get; set; }
        public LrigType LrigType { get; set; }
        private LrigType Condition { get; set; }
        private Class Class { get; set; }
        private int Power { get; set; }

        public Ability[] Ability { get; set; }

        public Lrig()
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

    public interface ICard
    {
        string Id { get; set; }
        string Kana { get; set; } // 全角カナのみのチェックを行う?
        string Illust { get; set; }
        Rarity Rarity { get; set; }
        string FlevorText { get; set; }

        string Name { get; set; }

        Type Type { get; set; }

        Color Color { get; set; }
        string Text { get; set; }
        byte Level { get; set; }
        Cost Cost { get; set; }
        Cost GrowCost { get; set; }
        byte Limit { get; set; }
        LrigType LrigType { get; set; }
        LrigType Condition { get; set; }
        Class Class { get; set; }
        int Power { get; set; }

        Ability[] Ability { get; set; }

        bool IsValid();
    }
}
