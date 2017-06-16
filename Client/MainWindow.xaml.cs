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
        SocketClient SocketClient = null;
        DocumentState DocumentState;
        public MainWindow()
        {
            InitializeComponent();
            SocketClient = new SocketClient();
            SocketClient.MessageReceived += SocketClient_MessageReceived;

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

        private void SocketClient_MessageReceived(object sender, SocketClient.MessageReceivedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                txtDocument.TextChanged -= txtDocument_TextChanged;

                try
                {
                    using (var ms = new MemoryStream(e.Message))
                    {
                        AppliedOperation appliedOperation = ProtoBuf.Serializer.Deserialize<AppliedOperation>(ms);
                        DocumentState.ApplyTransform(appliedOperation);
                    }
                }
                catch (Exception) { }
                txtDocument.Text = DocumentState.CurrentState; // TODO: Handle translating the cursor if needed
                txtDocument.TextChanged += txtDocument_TextChanged;
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            SocketClient.Dispose();
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
                            SocketClient.SendData(ms.ToArray());
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException("No easy way to get deleted text!");

                }

            }


            txtDocument.TextChanged -= txtDocument_TextChanged;

            txtDocument.Text = DocumentState.CurrentState; // Apply local changes via the document state

            txtDocument.TextChanged += txtDocument_TextChanged;
        }
    }
}
