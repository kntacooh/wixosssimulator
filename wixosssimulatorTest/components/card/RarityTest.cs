using WixossSimulator.Card;

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

namespace wixosssimulatorTest.components.card
{
    [TestClass]
    public class RarityTest
    {
        #region 変数
        string[] LR_文字列リスト =
        { 
            "LR", "lr", "ＬＲ", "ｌｒ", "  　 Ｌ　  　r   ",
            "ルリグレア", "　　 ルり ぐ　　レ　ア ",
            "LrigRare", "LRIG RARE", "　lr i g　　Ｒ ａ  r E　　"
        };
        #endregion

        [TestMethod]
        public void Rarity_列挙値から初期化_全て()
        {
            Rarity r;
            foreach (RarityKind rk in Enum.GetValues(typeof(RarityKind)))
            {
                r = new Rarity(rk);
                Assert.AreEqual(rk, r.Value);
            }
        }

        [TestMethod]
        public void Rarity_文字列から初期化_LR_文字列のタイプの違いを無視するかどうか()
        {
            //以下の違いを無視……大文字と小文字、ひらがなとカタカナ、半角と全角、および空白の有無
            Rarity r;
            foreach (string s in LR_文字列リスト)
            {
                r = new Rarity(s);
                Assert.AreEqual(RarityKind.LR, r.Value);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void Rarity_あり得ない文字列から初期化()
        {
            Rarity r = new Rarity("");
        }
    }
}
