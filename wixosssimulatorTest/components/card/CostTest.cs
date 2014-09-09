using wixosssimulator.components.card;

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

namespace wixosssimulatorTest.components.card
{
    [TestClass]
    public class CostTest
    {
        Dictionary<Color, byte> 連想配列_なし = new Dictionary<Color,byte>();
        Dictionary<Color, byte> 連想配列Full_全0 = new Dictionary<Color, byte>()
        {
            {Color.Colorless, 0},
            {Color.White, 0},
            {Color.Red, 0},
            {Color.Blue, 0},
            {Color.Green, 0},
            {Color.Black, 0}
        };
        byte[] 配列_全0 = { 0, 0, 0, 0, 0, 0 };
        Dictionary<Color, byte> 連想配列_赤1黒2 = new Dictionary<Color, byte>()
        {
            {Color.Black, 2},
            {Color.Red, 1}
        };
        Dictionary<Color, byte> 連想配列Full_赤1黒2 = new Dictionary<Color, byte>()
        {
            {Color.Colorless, 0},
            {Color.White, 0},
            {Color.Red, 1},
            {Color.Blue, 0},
            {Color.Green, 0},
            {Color.Black, 2}
        };
        byte[] 配列_赤1黒2 = { 0, 0, 1, 0, 0, 2 };

        byte[] 配列_なし = new byte[0];
        byte[] 配列_要素過剰 = { 0, 0, 0, 0, 0, 0, 7, 8, 9, 10, 11, 12, 13 };
        byte[] 配列_要素不足 = { 1, 2, 3 };
        //※下限が0以外の1次元配列は作れないのだろうか? (以下のコードはエラー)
        //byte[] 配列_要素過剰 = (byte[])Array.CreateInstance(typeof(byte), new int[] { 15 }, new int[] { 0 });
        //byte[] 配列_要素不足 = (byte[])Array.CreateInstance(typeof(byte), new int[] { 6 }, new int[] { 3 });



        [TestMethod]
        public void Cost_nullを表す連想配列で初期化()
        {
            Dictionary<Color, byte> 連想配列_null = null;
            Cost cost = new Cost(連想配列_null);
            CollectionAssert.AreEqual(連想配列Full_全0, cost.Value);
            CollectionAssert.AreEqual(配列_全0, cost.GetArray());
        }

        [TestMethod]
        public void Cost_連想配列で初期化_なし()
        {
            Cost cost = new Cost(連想配列_なし);
            CollectionAssert.AreEqual(連想配列Full_全0, cost.Value);
            CollectionAssert.AreEqual(配列_全0, cost.GetArray());
        }

        [TestMethod]
        public void Cost_配列で初期化_全0()
        {
            System.Diagnostics.Debugger.Break();
            Cost cost = new Cost(配列_全0);
            CollectionAssert.AreEqual(連想配列Full_全0, cost.Value);
            CollectionAssert.AreEqual(配列_全0, cost.GetArray());
        }

        [TestMethod]
        public void Cost_連想配列で初期化_赤1黒2()
        {
            Cost cost = new Cost(連想配列_赤1黒2);
            CollectionAssert.AreEqual(連想配列Full_赤1黒2, cost.Value);
            CollectionAssert.AreEqual(配列_赤1黒2, cost.GetArray());
        }

        [TestMethod]
        public void Cost_配列で初期化_赤1黒2()
        {
            Cost cost = new Cost(配列_赤1黒2);
            CollectionAssert.AreEqual(連想配列Full_赤1黒2, cost.Value);
            CollectionAssert.AreEqual(配列_赤1黒2, cost.GetArray());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Cost_nullを表す配列で初期化()
        {
            byte[] 配列_null = null;
            Cost cost = new Cost(配列_null);
        }

        [TestMethod]
        public void Cost_要素過剰の配列で初期化()
        {
            Cost cost = new Cost(配列_要素過剰);
            CollectionAssert.AreEqual(連想配列Full_全0, cost.Value);
            CollectionAssert.AreEqual(配列_全0, cost.GetArray());
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Cost_要素不足の配列で初期化()
        {
            Cost cost = new Cost(配列_要素不足);
        }
    }
}
