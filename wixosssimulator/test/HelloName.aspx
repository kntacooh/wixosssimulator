<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HelloName.aspx.cs" Inherits="WixossSimulator.test.WebForm5" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script type="text/javascript" src="../Scripts/knockout-3.2.0.js"></script>
    <script type="text/javascript" src="HelloName.js"></script>
</head>
<body>
    <p>ファーストネーム: <input data-bind="value: firstName" /></p>
    <p>ラストネーム: <input data-bind="value: lastName" /></p>
    <h2>Hello, <span data-bind="text: fullName"> </span>!</h2>
    <script>
        ko.applyBindings(new ViewModel());
    </script>
</body>
</html>
