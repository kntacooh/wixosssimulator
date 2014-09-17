using WixossSimulator.Card;

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

namespace wixosssimulatorTest.components.card
{
    [TestClass]
    public class ColorTest
    {
        #region 変数
        Dictionary<ColorKind, bool> 連想配列_なし = new Dictionary<ColorKind, bool>();
        Dictionary<ColorKind, bool> 連想配列Full_false = new Dictionary<ColorKind, bool>()
        {
            {ColorKind.Colorless, false},
            {ColorKind.White, false},
            {ColorKind.Red, false},
            {ColorKind.Blue, false},
            {ColorKind.Green, false},
            {ColorKind.Black, false}
        };
        bool[] 配列_false = { false, false, false, false, false, false };
        Dictionary<ColorKind, bool> 連想配列_青 = new Dictionary<ColorKind, bool>()
        {
            {ColorKind.Blue, true}
        };
        Dictionary<ColorKind, bool> 連想配列Full_青 = new Dictionary<ColorKind, bool>()
        {
            {ColorKind.Colorless, false},
            {ColorKind.White, false},
            {ColorKind.Red, false},
            {ColorKind.Blue, true},
            {ColorKind.Green, false},
            {ColorKind.Black, false}
        };
        bool[] 配列_青 = { false, false, false, true, false, false };
        Dictionary<ColorKind, bool> 連想配列_赤黒 = new Dictionary<ColorKind, bool>()
        {
            {ColorKind.Black, true},
            {ColorKind.Red, true}
        };
        Dictionary<ColorKind, bool> 連想配列Full_赤黒 = new Dictionary<ColorKind, bool>()
        {
            {ColorKind.Colorless, false},
            {ColorKind.White, false},
            {ColorKind.Red, true},
            {ColorKind.Blue, false},
            {ColorKind.Green, false},
            {ColorKind.Black, true}
        };
        bool[] 配列_赤黒 = { false, false, true, false, false, true };

        bool[] 配列_なし = new bool[0];
        bool[] 配列_要素過剰 = { false, false, false, false, false, false, true, false, true, false, true, false, true };
        bool[] 配列_要素不足 = { true, false, true };
        //※下限がfalse以外の1次元配列は作れないのだろうか? (以下のコードはエラー)
        //bool[] 配列_要素過剰 = (bool[])Array.CreateInstance(typeof(bool), new int[] { 15 }, new int[] { 0 });
        //bool[] 配列_要素不足 = (bool[])Array.CreateInstance(typeof(bool), new int[] { 6 }, new int[] { 3 });
        #endregion

        [TestMethod]
        public void Color_nullを表す連想配列で初期化()
        {
            Dictionary<ColorKind, bool> 連想配列_null = null;
            Color cost = new Color(連想配列_null);
            CollectionAssert.AreEqual(連想配列Full_false, cost.Value);
            CollectionAssert.AreEqual(配列_false, cost.GetArray());
        }

        [TestMethod]
        public void Color_連想配列で初期化_なし()
        {
            Color cost = new Color(連想配列_なし);
            CollectionAssert.AreEqual(連想配列Full_false, cost.Value);
            CollectionAssert.AreEqual(配列_false, cost.GetArray());
        }

        [TestMethod]
        public void Color_配列で初期化_全false()
        {
            Color cost = new Color(配列_false);
            CollectionAssert.AreEqual(連想配列Full_false, cost.Value);
            CollectionAssert.AreEqual(配列_false, cost.GetArray());
        }

        [TestMethod]
        public void Color_連想配列で初期化_青()
        {
            Color cost = new Color(連想配列_青);
            CollectionAssert.AreEqual(連想配列Full_青, cost.Value);
            CollectionAssert.AreEqual(配列_青, cost.GetArray());
        }

        [TestMethod]
        public void Color_配列で初期化_青()
        {
            Color cost = new Color(配列_青);
            CollectionAssert.AreEqual(連想配列Full_青, cost.Value);
            CollectionAssert.AreEqual(配列_青, cost.GetArray());
        }

        [TestMethod]
        public void Color_列挙値を指定して初期化_青()
        {
            Color cost = new Color(ColorKind.Blue);
            CollectionAssert.AreEqual(連想配列Full_青, cost.Value);
            CollectionAssert.AreEqual(配列_青, cost.GetArray());
        }

        [TestMethod]
        public void Color_連想配列で初期化_赤黒()
        {
            Color cost = new Color(連想配列_赤黒);
            CollectionAssert.AreEqual(連想配列Full_赤黒, cost.Value);
            CollectionAssert.AreEqual(配列_赤黒, cost.GetArray());
        }

        [TestMethod]
        public void Color_配列で初期化_赤黒()
        {
            Color cost = new Color(配列_赤黒);
            CollectionAssert.AreEqual(連想配列Full_赤黒, cost.Value);
            CollectionAssert.AreEqual(配列_赤黒, cost.GetArray());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Color_nullを表す配列で初期化()
        {
            bool[] 配列_null = null;
            Color cost = new Color(配列_null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Color_要素不足の配列で初期化()
        {
            Color cost = new Color(配列_要素不足);
        }

        [TestMethod]
        public void Color_要素過剰の配列で初期化()
        {
            Color cost = new Color(配列_要素過剰);
            CollectionAssert.AreEqual(連想配列Full_false, cost.Value);
            CollectionAssert.AreEqual(配列_false, cost.GetArray());
        }
    }
}
