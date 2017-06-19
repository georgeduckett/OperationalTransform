using SocketMessaging.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static TcpServer Server;
        static void Main(string[] args)
        {
            Server = new TcpServer();
            Server.Start(8888);
            Server.Connected += Server_Connected;

            Console.ReadKey();

            Server.Stop();
        }

        private static void Server_Connected(object sender, SocketMessaging.ConnectionEventArgs e)
        {
            e.Connection.SetMode(SocketMessaging.MessageMode.PrefixedLength);
            e.Connection.ReceivedMessage += Connection_ReceivedMessage;
        }

        private static void Connection_ReceivedMessage(object sender, EventArgs e)
        {
            var senderConnection = (SocketMessaging.Connection)sender;

            var message = senderConnection.ReceiveMessage();

            foreach(var conn in Server.Connections)
            {
                if(conn != senderConnection)
                {
                    conn.Send(message);
                }
            }
        }
    }
}
