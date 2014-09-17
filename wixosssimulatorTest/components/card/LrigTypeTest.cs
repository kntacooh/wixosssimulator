using WixossSimulator.Card;

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace wixosssimulatorTest.components.card
{
    [TestClass]
    public class LrigTypeTest
    {
        #region 変数
        string 文字列_なし = "";
        string[] 配列_なし = { };
        string 文字列_空白のみ = "  　  ";
        string[] 配列_空白とnull要素のみ = { "  ", "　　", " 　　 　 ", null, "" };
        string 文字列_タマ = "タマ";
        string 文字列_タマ_前後空白含む = " 　タマ  　 　　 ";
        string[] 配列_タマ = { "タマ" };
        string 文字列_花代とユヅキ = "花代/ユヅキ";
        string 文字列_花代とユヅキ_中空白含む = "  花代 　 /　　ユヅキ　　";
        string[] 配列_花代とユヅキ = { "花代", "ユヅキ" };
        string[] 配列_花代とユヅキ_空白とnull要素含む
            = { "  ", "", "花代", "", "     ", "　　　　　　", " ", "ユヅキ", "     ", null, "　　　　", "　　 　  　", null };
        string[] 配列_花代とユヅキ_前後空白含む = { " 花代　　", "　 　ユヅキ 　" };
        #endregion

        [TestMethod]
        public void LrigType_文字列で初期化_なし()
        {
            LrigType lrigType = new LrigType(文字列_なし);
            Assert.AreEqual(文字列_なし, lrigType.Text);
            CollectionAssert.AreEqual(配列_なし, lrigType.TextList);
        }

        [TestMethod]
        public void LrigType_配列で初期化_なし()
        {
            LrigType lrigType = new LrigType(配列_なし);
            Assert.AreEqual(文字列_なし, lrigType.Text);
            CollectionAssert.AreEqual(配列_なし, lrigType.TextList);
        }

        [TestMethod]
        public void LrigType_文字列で初期化_タマ()
        {
            LrigType lrigType = new LrigType(文字列_タマ);
            Assert.AreEqual(文字列_タマ, lrigType.Text);
            CollectionAssert.AreEqual(配列_タマ, lrigType.TextList);
        }

        [TestMethod]
        public void LrigType_配列で初期化_タマ()
        {
            LrigType lrigType = new LrigType(配列_タマ);
            Assert.AreEqual(文字列_タマ, lrigType.Text);
            CollectionAssert.AreEqual(配列_タマ, lrigType.TextList);
        }

        [TestMethod]
        public void LrigType_文字列で初期化_花代とユヅキ()
        {
            LrigType lrigType = new LrigType(文字列_花代とユヅキ);
            Assert.AreEqual(文字列_花代とユヅキ, lrigType.Text);
            CollectionAssert.AreEqual(配列_花代とユヅキ, lrigType.TextList);
        }

        [TestMethod]
        public void LrigType_配列で初期化_花代とユヅキ()
        {
            LrigType lrigType = new LrigType(配列_花代とユヅキ);
            Assert.AreEqual(文字列_花代とユヅキ, lrigType.Text);
            CollectionAssert.AreEqual(配列_花代とユヅキ, lrigType.TextList);
        }

        [TestMethod]
        public void LrigType_文字列で初期化_前後や中の空白を削除させる()
        {
            LrigType lrigType = new LrigType(文字列_空白のみ);
            Assert.AreEqual(文字列_なし, lrigType.Text);
            CollectionAssert.AreEqual(配列_なし, lrigType.TextList);

            LrigType lrigType2 = new LrigType(文字列_タマ_前後空白含む);
            Assert.AreEqual(文字列_タマ, lrigType2.Text);
            CollectionAssert.AreEqual(配列_タマ, lrigType2.TextList);

            LrigType lrigType3 = new LrigType(文字列_花代とユヅキ_中空白含む);
            Assert.AreEqual(文字列_花代とユヅキ, lrigType3.Text);
            CollectionAssert.AreEqual(配列_花代とユヅキ, lrigType3.TextList);
        }

        [TestMethod]
        public void LrigType_配列で初期化_空白やnull要素を削除させる()
        {
            LrigType lrigType = new LrigType(配列_空白とnull要素のみ);
            Assert.AreEqual(文字列_なし, lrigType.Text);
            CollectionAssert.AreEqual(配列_なし, lrigType.TextList);

            LrigType lrigType2 = new LrigType(配列_花代とユヅキ_空白とnull要素含む);
            Assert.AreEqual(文字列_花代とユヅキ, lrigType2.Text);
            CollectionAssert.AreEqual(配列_花代とユヅキ, lrigType2.TextList);

            LrigType lrigType3 = new LrigType(配列_花代とユヅキ_前後空白含む);
            Assert.AreEqual(文字列_花代とユヅキ, lrigType3.Text);
            CollectionAssert.AreEqual(配列_花代とユヅキ, lrigType3.TextList);
        }

        [TestMethod]
        public void LrigType_文字列と配列が連動して変更されているかどうか()
        {
            LrigType lrigType = new LrigType(文字列_なし);

            lrigType.Text = 文字列_タマ;
            Assert.AreEqual(文字列_タマ, lrigType.Text);
            CollectionAssert.AreEqual(配列_タマ, lrigType.TextList);

            lrigType.TextList = 配列_花代とユヅキ;
            Assert.AreEqual(文字列_花代とユヅキ, lrigType.Text);
            CollectionAssert.AreEqual(配列_花代とユヅキ, lrigType.TextList);
        }

        [TestMethod]
        public void LrigType_nullを指定しても空の文字列or配列が代入されるかどうか()
        {
            LrigType lrigType = new LrigType(文字列_タマ);
            lrigType.Text = null;
            Assert.AreEqual(文字列_なし, lrigType.Text);
            CollectionAssert.AreEqual(配列_なし, lrigType.TextList);

            LrigType lrigType2 = new LrigType(文字列_タマ);
            lrigType2.TextList = null;
            Assert.AreEqual(文字列_なし, lrigType2.Text);
            CollectionAssert.AreEqual(配列_なし, lrigType2.TextList);
        }

        [TestMethod]
        public void LrigType_nullを表す文字列で初期化()
        {
            string 文字列_null = null;
            LrigType lrigType = new LrigType(文字列_null);
            Assert.AreEqual(文字列_なし, lrigType.Text);
            CollectionAssert.AreEqual(配列_なし, lrigType.TextList);
        }

        [TestMethod]
        public void LrigType_nullを表す配列で初期化()
        {
            string[] 配列_null = null;
            LrigType lrigType = new LrigType(配列_null);
            Assert.AreEqual(文字列_なし, lrigType.Text);
            CollectionAssert.AreEqual(配列_なし, lrigType.TextList);
        }
    }
}
