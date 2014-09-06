using wixosssimulator.components.card;

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Diagnostics;

namespace wixosssimulatorTest.components.card
{
    [TestClass]
    public class LrigTypeTests
    {
        string textNone = "";
        string[] arrayNone = { };
        string textTama = "タマ";
        string[] arrayTama = { "タマ" };
        string textHanayoAndYuduki = "花代/ユヅキ";
        string[] arrayHanayoAndYuduki = { "花代", "ユヅキ" };

        [TestMethod]
        public void LrigType_文字列で初期化_タマ()
        {
            LrigType lrigType = new LrigType(textTama);
            Assert.AreEqual(textTama, lrigType.Text);
            CollectionAssert.AreEqual(arrayTama, lrigType.Array);
        }

        [TestMethod]
        public void LrigType_配列で初期化_タマ()
        {
            LrigType lrigType = new LrigType(arrayTama);
            Assert.AreEqual(textTama, lrigType.Text);
            CollectionAssert.AreEqual(arrayTama, lrigType.Array);
        }

        [TestMethod]
        public void LrigType_文字列で初期化_花代とユヅキ()
        {
            LrigType lrigType = new LrigType(textHanayoAndYuduki);
            Assert.AreEqual(textHanayoAndYuduki, lrigType.Text);
            CollectionAssert.AreEqual(arrayHanayoAndYuduki, lrigType.Array);
        }

        [TestMethod]
        public void LrigType_配列で初期化_花代とユヅキ()
        {
            LrigType lrigType = new LrigType(arrayHanayoAndYuduki);
            Assert.AreEqual(textHanayoAndYuduki, lrigType.Text);
            CollectionAssert.AreEqual(arrayHanayoAndYuduki, lrigType.Array);
        }

        [TestMethod]
        public void LrigTypeなしの場合()
        {
            string text = "";
            string[] array = { "" };
            LrigType lrigType = new LrigType(text);
            Assert.AreEqual(text, lrigType.Text);
            CollectionAssert.AreEqual(array, lrigType.Array);
            Assert.AreEqual(lrigType.Array.Length, 1);
            //Assert.IsNull(lrigType.Array);
            //CollectionAssert.AreEqual(array, lrigType.Array);
        }
    }
}
