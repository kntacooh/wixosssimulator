using WixossSimulator.Card;

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace wixosssimulatorTest.components.card
{
    [TestClass]
    public class ClassTest
    {
        string なし = "";
        string 空白のみ = "  　 　  ";
        string 精元 = "精元";
        string 精元_前後空白含む = " 　精元　  　";
        string 精像_天使 = "精像：天使";
        string 精像_天使_中空白含む = "  精像 　　 ：   天使　 ";
        string 精像_天使_半角コロン = "精像:天使";
        string 精像 = "精像";
        string 天使 = "天使";
        string 天使_第１分類なしのテキスト = "：天使";

        [TestMethod]
        public void Class_テキストから初期化_なし()
        {
            Class c = new Class(なし);
            Assert.AreEqual(なし, c.Text);
            Assert.AreEqual(なし, c.Primary);
            Assert.AreEqual(なし, c.Secondary);
        }

        [TestMethod]
        public void Class_分類から初期化_なし()
        {
            Class c = new Class(なし, なし);
            Assert.AreEqual(なし, c.Text);
            Assert.AreEqual(なし, c.Primary);
            Assert.AreEqual(なし, c.Secondary);
        }

        [TestMethod]
        public void Class_テキストから初期化_精元()
        {
            Class c = new Class(精元);
            Assert.AreEqual(精元, c.Text);
            Assert.AreEqual(精元, c.Primary);
            Assert.AreEqual(なし, c.Secondary);
        }

        [TestMethod]
        public void Class_第１分類のみから初期化_精元()
        {
            Class c = new Class(精元, なし);
            Assert.AreEqual(精元, c.Text);
            Assert.AreEqual(精元, c.Primary);
            Assert.AreEqual(なし, c.Secondary);
        }

        [TestMethod]
        public void Class_テキストから初期化_精像_天使()
        {
            Class c = new Class(精像_天使);
            Assert.AreEqual(精像_天使, c.Text);
            Assert.AreEqual(精像, c.Primary);
            Assert.AreEqual(天使, c.Secondary);
        }

        [TestMethod]
        public void Class_第１分類と第２分類から初期化_精像_天使()
        {
            Class c = new Class(精像, 天使);
            Assert.AreEqual(精像_天使, c.Text);
            Assert.AreEqual(精像, c.Primary);
            Assert.AreEqual(天使, c.Secondary);
        }

        [TestMethod]
        public void Class_テキストから初期化_前後や中の空白を削除させる()
        {
            Class c = new Class(空白のみ);
            Assert.AreEqual(なし, c.Text);
            Assert.AreEqual(なし, c.Primary);
            Assert.AreEqual(なし, c.Secondary);

            Class c2 = new Class(精元_前後空白含む);
            Assert.AreEqual(精元, c2.Text);
            Assert.AreEqual(精元, c2.Primary);
            Assert.AreEqual(なし, c2.Secondary);

            Class c3 = new Class(精像_天使_中空白含む);
            Assert.AreEqual(精像_天使, c3.Text);
            Assert.AreEqual(精像, c3.Primary);
            Assert.AreEqual(天使, c3.Secondary);
        }

        [TestMethod]
        public void Class_テキストから初期化_半角コロンを使われた場合()
        {
            Class c = new Class(精像_天使_半角コロン);
            Assert.AreEqual(精像_天使, c.Text);
            Assert.AreEqual(精像, c.Primary);
            Assert.AreEqual(天使, c.Secondary);
        }

        [TestMethod]
        public void Class_第２分類のみ指定される状況になった場合()
        {
            Class c = new Class(精像_天使);
            c.Primary = null;
            Assert.AreEqual(天使_第１分類なしのテキスト, c.Text);
            Assert.AreEqual(なし, c.Primary);
            Assert.AreEqual(天使, c.Secondary);

            Class c2 = new Class(天使_第１分類なしのテキスト);
            Assert.AreEqual(天使_第１分類なしのテキスト, c2.Text);
            Assert.AreEqual(なし, c2.Primary);
            Assert.AreEqual(天使, c2.Secondary);

            Class c3 = new Class(なし, 天使);
            Assert.AreEqual(天使_第１分類なしのテキスト, c3.Text);
            Assert.AreEqual(なし, c3.Primary);
            Assert.AreEqual(天使, c3.Secondary);
        }

        [TestMethod]
        public void Class_nullを指定しても空の文字列が代入されるか()
        {
            Class c = new Class(精像_天使);
            c.Text = null;
            Assert.AreEqual(なし, c.Text);
            Assert.AreEqual(なし, c.Primary);
            Assert.AreEqual(なし, c.Secondary);

            Class c2 = new Class(精像_天使);
            c2.Secondary = null;
            Assert.AreEqual(精像, c2.Text);
            Assert.AreEqual(精像, c2.Primary);
            Assert.AreEqual(なし, c2.Secondary);
        }

        [TestMethod]
        public void Class_nullのテキストから初期化()
        {
            Class c = new Class(null);
            Assert.AreEqual(なし, c.Text);
            Assert.AreEqual(なし, c.Primary);
            Assert.AreEqual(なし, c.Secondary);
        }

        [TestMethod]
        public void Class_nullの分類から初期化()
        {
            Class c = new Class(null, null);
            Assert.AreEqual(なし, c.Text);
            Assert.AreEqual(なし, c.Primary);
            Assert.AreEqual(なし, c.Secondary);
        }
    }
}
