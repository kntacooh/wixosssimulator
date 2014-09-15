using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Collections.ObjectModel; //ReadOnlyDictionary

namespace WixossSimulator.Card
{
    /// <summary> レアリティについての情報を表します。 </summary>
    public class Rarity
    {
        /// <summary> レアリティを示す列挙値を取得します。 </summary>
        public RarityKind Value { get; set; }
        #region RarityKindに対応する各種連想配列
        /// <summary> キーをレアリティを示す文字列として値を対応する列挙値とする、読み取り専用の連想配列を取得します。 </summary>
        public static ReadOnlyDictionary<string, RarityKind> Text = new ReadOnlyDictionary<string, RarityKind>(
            new Dictionary<string, RarityKind>(StringComparer.CurrentCultureIgnoreCase)
            {
                {"コモン",RarityKind.C},
                {"C",RarityKind.C},
                {"Common",RarityKind.C},

                {"レア",RarityKind.R},
                {"R",RarityKind.R},
                {"Rare",RarityKind.R},

                {"ルリグコモン",RarityKind.LC},
                {"LC",RarityKind.LC},
                {"LrigCommon",RarityKind.LC},

                {"スーパーレア",RarityKind.SR},
                {"SR",RarityKind.SR},
                {"SuperRare",RarityKind.SR},

                {"ルリグレア",RarityKind.LR},
                {"LR",RarityKind.LR},
                {"LrigRare",RarityKind.LR},

                {"シークレット",RarityKind.Secret},
                {"Secret",RarityKind.Secret},

                {"プロモーションカード",RarityKind.PR},
                {"PR",RarityKind.PR},
                {"PromotionCard",RarityKind.PR},

                {"構築済みデッキ",RarityKind.ST},
                {"ST",RarityKind.ST},
                {"StructureDeck",RarityKind.ST},

                {"SP",RarityKind.SP},

                {"？？？",RarityKind.Question},
                {"???",RarityKind.Question},
            });
        /// <summary> キーをレアリティを示す列挙値として値を対応する記号とする、読み取り専用の連想配列を取得します。 </summary>
        public static ReadOnlyDictionary<RarityKind, string> Sign = new ReadOnlyDictionary<RarityKind, string>(
            new Dictionary<RarityKind, string>()
            {
                {RarityKind.C, "●C"},
                {RarityKind.R, "●●R"},
                {RarityKind.LC, "●●●LC"},
                {RarityKind.SR, "●●●●SR"},
                {RarityKind.LR, "●●●●●LR"},
                {RarityKind.Secret, "●SECRET●"},
                {RarityKind.PR, "●●●PR"},
                {RarityKind.ST, "●●●ST"},
                {RarityKind.SP, "●●●SP"},
                {RarityKind.Question, "●???●"},
            });
        #endregion

        /// <summary> 新しいインスタンスを初期化します。 </summary>
        /// <param name="value"> レアリティを示す列挙値。 </param>
        public Rarity(RarityKind value)
        {
            this.Value = value;
        }

        /// <summary> レアリティを示す文字列を、対応する列挙値に変換します。 </summary>
        /// <param name="text"> レアリティを示す文字列。 </param>
        /// <returns> レアリティを示す列挙値。 </returns>
        public static RarityKind ConvertRarityKindToText(string text);

        /// <summary> レアリティを示す列挙値に対応する記号を取得します。 </summary>
        /// <returns> レアリティを示す記号。 </returns>
        public string GetSign();
    }

    /// <summary> カードのレアリティを表す列挙値を提供します。 </summary>
    public enum RarityKind
    {
        /// <summary> コモン (COMMON) </summary>
        C,
        /// <summary> レア (RARE) </summary>
        R,
        /// <summary> ルリグコモン (LRIG COMMON) </summary>
        LC,
        /// <summary> スーパーレア (SUPER RARE) </summary>
        SR,
        /// <summary> ルリグレア (LRIG RARE) </summary>
        LR,
        /// <summary> シークレット (SECRET) </summary>
        Secret = 9,
        /// <summary> [未確定] プロモーションカード (PROMOTION CARD) </summary>
        PR = 13,
        /// <summary> [未確定] 構築済みデッキ (STRUCTURE DECK) </summary>
        ST = 23,
        /// <summary> [未確定] (SPEC, SPECIAL)　((SPECIFICATION)) [spec selector?] </summary>
        SP = 53,
        /// <summary> ？？？ (???) </summary>
        Question = 99,
    }
}
