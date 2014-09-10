using WixossSimulator.Card;

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

namespace wixosssimulatorTest.components.card
{
    [TestClass]
    public class CostTest
    {
        Dictionary<ColorKind, byte> 連想配列_なし = new Dictionary<ColorKind,byte>();
        Dictionary<ColorKind, byte> 連想配列Full_全0 = new Dictionary<ColorKind, byte>()
        {
            {ColorKind.Colorless, 0},
            {ColorKind.White, 0},
            {ColorKind.Red, 0},
            {ColorKind.Blue, 0},
            {ColorKind.Green, 0},
            {ColorKind.Black, 0}
        };
        byte[] 配列_全0 = { 0, 0, 0, 0, 0, 0 };
        Dictionary<ColorKind, byte> 連想配列_赤1黒2 = new Dictionary<ColorKind, byte>()
        {
            {ColorKind.Black, 2},
            {ColorKind.Red, 1}
        };
        Dictionary<ColorKind, byte> 連想配列Full_赤1黒2 = new Dictionary<ColorKind, byte>()
        {
            {ColorKind.Colorless, 0},
            {ColorKind.White, 0},
            {ColorKind.Red, 1},
            {ColorKind.Blue, 0},
            {ColorKind.Green, 0},
            {ColorKind.Black, 2}
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
            Dictionary<ColorKind, byte> 連想配列_null = null;
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
        [ExpectedException(typeof(ArgumentException))]
        public void Cost_要素不足の配列で初期化()
        {
            Cost cost = new Cost(配列_要素不足);
        }

        [TestMethod]
        public void Cost_要素過剰の配列で初期化()
        {
            Cost cost = new Cost(配列_要素過剰);
            CollectionAssert.AreEqual(連想配列Full_全0, cost.Value);
            CollectionAssert.AreEqual(配列_全0, cost.GetArray());
        }
    }
}
