using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace WixossSimulator.test
{
    [HubName("echo")]
    public class EchoHub : Hub
    {
        public void Send(string text)
        {
            Clients.All.Receive(text);
        }
    }
}