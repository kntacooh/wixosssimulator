<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="helloworld.aspx.cs" Inherits="WixossSimulator.helloworldInherits" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <% 
        
        WixossSimulator.Card.Card card = new WixossSimulator.Card.Card();
        //wixosssimulator.components.card.Lrig lrig = new wixosssimulator.components.card.Lrig();
        //wixosssimulator.components.card.ICard cardInterface;
        //cardInterface = lrig;
        //cardInterface.Condition = new wixosssimulator.components.card.LrigType("タマ");
        //lrig.Condition = new wixosssimulator.components.card.LrigType("タマ");
        //Response.Write(cardInterface.Condition.Text + "<br>");
        Response.Write("Hello, world!<br>");

        //wixosssimulator.components.card.Cost cost;
    %>
    </div>
    </form>
</body>
</html>
