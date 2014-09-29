<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Searching.aspx.cs" Inherits="WixossSimulator.test.Searching" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link rel="stylesheet" href="http://yui.yahooapis.com/pure/0.5.0/pure-min.css">
    <title>カード情報の探索(ここtitleタグ)</title>
</head>
<body>
    <span data-bind="visible: !started">読み込み中です……</span>
    <div data-bind="visible: started">

        <h2>カード情報の探索</h2>

        <p>
            SignalR/knockout.jsの使い方テスト中
        </p>

        <p>
            <ul data-bind="foreach: items">
                <li data-bind="text: $data"></li>
            </ul>
            <button data-bind="click: hello, enable: started">hello!</button><br />
            名前:<input data-bind="value: form" /><br />
            <button data-bind="click: helloworld, enable: started">名前にhello!</button><br />
        </p>

        <p>カード情報の探索をします。
            スクレイピングを行うサイトのドメインを指定して探索開始ボタンを押しましょう。<br />
            (今のところ、 Official …公式サイトからのスクレイピングのみ対応)<br />
            <select data-bind="options: domainList, value: domain, event: {change: changeDomain}"></select>
            <button data-bind="click: startSearch">探索開始</button>
        </p>

        <p>
            結果：<br />
            <span data-bind="text: urls, visible: urls().length > 0"></span> 件ヒットしています。<br />
            <span data-bind="visible: isSearch">探索中。しばらくお待ちください……(数十秒ほどかかるだろうか)</span><br />

            <ul data-bind="foreach: urls, visible: urls().length > 0">
                <li data-bind="text: $data"></li>
            </ul>
        </p>

    </div>

    <script type="text/javascript" src="../Scripts/jquery-2.1.1.min.js"></script>
    <script type="text/javascript" src="../Scripts/jquery.signalR-2.1.2.js"></script>
    <script type="text/javascript" src="../Scripts/knockout-3.2.0.js"></script>
    <script type="text/javascript" src="searching.js"></script>

    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
