<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EchoAndFirstSignalR.aspx.cs" Inherits="WixossSimulator.test.WebForm3" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
  <title></title>
  <script type="text/javascript" src="../Scripts/jquery-2.1.1.min.js"></script>
  <script type="text/javascript" src="../Scripts/jquery.signalR-2.1.2.js"></script>
  <script>
    $(function() {
      var connection = $.hubConnection();

      var echo = connection.createHubProxy("echo");

      echo.on("Receive", function (text) {
        $("#list").append("<li>" + text + "</li>");
      });

      $("#send").click(function() {
        var message = $("#message").val();

        echo.invoke("Send", message).done(function() {
          $("#message").val("");
        });
      });

      connection.start(function() {
        $("#send").prop("disabled", false);
      });
    })
  </script>
</head>
<body>
  <form id="form1" runat="server">
  <div>
    <input type="text" id="message" />
    <input type="button" id="send" value="送信" disabled="disabled" />

    <ul id="list"></ul>
  </div>
  </form>
</body></html>
