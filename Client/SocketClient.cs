using AwesomeSockets.Domain.Sockets;
using AwesomeSockets.Sockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ASBuffer = AwesomeSockets.Buffers.Buffer;

namespace Client
{
    public class SocketClient : IDisposable
    {
        public class MessageReceivedEventArgs : EventArgs { public byte[] Message; }

        private ISocket Socket;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        private CancellationTokenSource CancelTokenSource;
        private CancellationToken CancelToken;
        private Task ListenTask;
        public SocketClient()
        {
            ListenTask = ListenAsync();
        }
        private Task ListenAsync()
        {
            CancelTokenSource = new CancellationTokenSource();
            CancelToken = CancelTokenSource.Token;
            var listenTask = Task.Run(ListenInternalAsync);

            return listenTask;
        }
        private async Task ListenInternalAsync()
        {
            try
            {
                Socket = AweSock.TcpConnect("127.0.0.1", 8888);
                ListenForMessages(Socket);
            }
            catch (SocketException)
            {
                await Task.Delay(1000);
                if (!CancelToken.IsCancellationRequested)
                {
                    // TODO: Handle socket exception (try and re-connect after a while?)
                }
            }
        }
        private void ListenForMessages(ISocket socket)
        {
            var callbackFired = new ManualResetEventSlim(false);
            var receiveBuffer = ASBuffer.New();
            while (!CancelToken.IsCancellationRequested)
            {
                ASBuffer.ClearBuffer(receiveBuffer);
                Tuple<int, EndPoint> result = null;
                try
                {
                    AweSock.ReceiveMessage(socket, receiveBuffer, AwesomeSockets.Domain.SocketCommunicationTypes.NonBlocking, (b, endPoint) =>
                    {
                        result = new Tuple<int, EndPoint>(b, endPoint); callbackFired.Set();
                    });
                }
                catch (ArgumentOutOfRangeException)
                { // Swallow the exception caused by AweSock's construction of an invalid endpoint

                }
                try
                {
                    callbackFired.Wait(CancelToken);
                }
                catch (OperationCanceledException)
                {

                }
                if (!CancelToken.IsCancellationRequested)
                {
                    callbackFired.Reset();
                    ASBuffer.FinalizeBuffer(receiveBuffer);
                    if (result.Item1 == 0) return;

                    var length = ASBuffer.Get<short>(receiveBuffer);
                    var bytes = new byte[length];
                    ASBuffer.BlockCopy(ASBuffer.GetBuffer(receiveBuffer), sizeof(short), bytes, 0, length);
                    MessageReceived?.Invoke(this, new MessageReceivedEventArgs() { Message = bytes });
                }
            }
        }
        public void SendData(byte[] bytes)
        {
            if (Socket != null)
            {
                using (var ms = new MemoryStream(bytes))
                {
                    var buffer = ASBuffer.New((int)ms.Length + sizeof(short));
                    ASBuffer.ClearBuffer(buffer);

                    ASBuffer.Add(buffer, BitConverter.GetBytes((short)ms.Length).Concat(ms.ToArray()).ToArray());
                    ASBuffer.FinalizeBuffer(buffer);

                    Socket.SendMessage(buffer);
                }
            }
        }
        private void Disconnect()
        {
            Socket?.Close();
            CancelTokenSource.Cancel();
            ListenTask?.Wait();
        }
        public void Dispose()
        {
            Disconnect();
        }
    }
}
