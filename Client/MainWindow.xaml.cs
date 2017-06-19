using OperationalTransform.Operations;
using OperationalTransform.StateManagement;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string OldText = string.Empty;
        SocketMessaging.TcpClient SocketClient = null;
        DocumentState DocumentState;
        public MainWindow()
        {
            InitializeComponent();
            SocketClient = SocketMessaging.TcpClient.Connect(System.Net.IPAddress.Parse("127.0.0.1"), 8888);
            SocketClient.SetMode(SocketMessaging.MessageMode.PrefixedLength);
            SocketClient.ReceivedMessage += SocketClient_ReceivedMessage;
            

            var proccesList = System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).OrderBy(p => p.Id).ToArray();

            int currentProcesId = System.Diagnostics.Process.GetCurrentProcess().Id;
            var userId = Array.FindIndex(proccesList, p => p.Id == currentProcesId);

            Title += " " + userId.ToString();
            DocumentState = new DocumentState((uint)userId, string.Empty); // TODO: Handle getting the initial state of the document rather than assuming empty

            RuntimeTypeModel.Default.Add(typeof(AppliedOperationSurrogate), true);
            RuntimeTypeModel.Default.Add(typeof(AppliedOperation), false).SetSurrogate(typeof(AppliedOperationSurrogate));
            RuntimeTypeModel.Default.Add<OperationBase>()
                                    .AddSubType(101, RuntimeTypeModel.Default.Add<InsertOperation>().Type)
                                    .AddSubType(102, RuntimeTypeModel.Default.Add<DeleteOperation>().Type)
                                    .AddSubType(103, RuntimeTypeModel.Default.Add<IdentityOperation>().Type);
        }

        private void SocketClient_ReceivedMessage(object sender, EventArgs e)
        {
            using (var ms = new MemoryStream(SocketClient.ReceiveMessage()))
            {
                Dispatcher.Invoke(() =>
                {
                    txtDocument.TextChanged -= txtDocument_TextChanged;

                    AppliedOperation appliedOperation = ProtoBuf.Serializer.Deserialize<AppliedOperation>(ms);
                    DocumentState.ApplyTransform(appliedOperation);

                    txtDocument.Text = DocumentState.CurrentState; // TODO: Handle translating the cursor if needed
                    txtDocument.TextChanged += txtDocument_TextChanged;
                });
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            SocketClient.Close();
            base.OnClosed(e);
        }
        private void txtDocument_TextChanged(object sender, TextChangedEventArgs e)
        {
            foreach(var change in e.Changes)
            {
                AppliedOperation appliedOperation = null;

                if (change.AddedLength != 0)
                {
                    var addedString = ((TextBox)sender).Text.Substring(change.Offset, change.AddedLength);
                    var changeCharOffset = change.Offset;
                    foreach (var c in addedString)
                    {
                        appliedOperation = new AppliedOperation(new InsertOperation(DocumentState, changeCharOffset++, c), DocumentState);
                        DocumentState.ApplyTransform(appliedOperation);
                        using (var ms = new MemoryStream())
                        {
                            ProtoBuf.Serializer.Serialize(ms, appliedOperation);
                            Task.Delay(10000).ContinueWith(t => SocketClient.Send(ms.ToArray()));
                        }
                    }
                }
                else
                {
                    var removedString = ((TextBox)sender).Text.Substring(change.Offset, change.RemovedLength);
                    foreach (var c in removedString)
                    {
                        appliedOperation = new AppliedOperation(new DeleteOperation(DocumentState, change.Offset), DocumentState);
                        DocumentState.ApplyTransform(appliedOperation);
                        using (var ms = new MemoryStream())
                        {
                            ProtoBuf.Serializer.Serialize(ms, appliedOperation);
                            Task.Delay(10000).ContinueWith(t => SocketClient.Send(ms.ToArray()));
                        }
                    }
                }

            }

            txtDocument.TextChanged -= txtDocument_TextChanged;

            OldText = txtDocument.Text;
            txtDocument.Text = DocumentState.CurrentState; // Apply local changes via the document state

            txtDocument.TextChanged += txtDocument_TextChanged;
        }
    }
}
