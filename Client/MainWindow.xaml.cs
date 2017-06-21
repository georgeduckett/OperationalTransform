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
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
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

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            throw e.Exception;
        }

        private void SocketClient_ReceivedMessage(object sender, EventArgs e)
        {
            using (var ms = new MemoryStream(SocketClient.ReceiveMessage()))
            {
                Dispatcher.Invoke(() =>
                {
                    var caretIndex = txtDocument.CaretIndex;

                    AppliedOperation appliedOperation = ProtoBuf.Serializer.Deserialize<AppliedOperation>(ms);
                    try
                    {
                        DocumentState.ApplyTransform(appliedOperation, ref caretIndex);
                    }
                    catch (Exception)
                    {
                        System.Diagnostics.Debugger.Break();
                        throw;
                    }

                    txtDocument.TextChanged -= Document_TextChanged;
                    txtDocument.Text = DocumentState.CurrentState;
                    txtDocument.TextChanged += Document_TextChanged;
                    txtDocument.CaretIndex = caretIndex;
                });
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            SocketClient.Close();
            base.OnClosed(e);
        }
        private void UndoCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentState.CanUndo();
            e.Handled = true;
        }
        private void RedoCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentState.CanRedo();
            e.Handled = true;
        }
        private void UndoCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var caretIndex = txtDocument.CaretIndex;
            var undoOp = DocumentState.Undo(ref caretIndex);
            UpdateDocumentTextFromState();
            txtDocument.CaretIndex = caretIndex;
            SendOperation(undoOp);

            e.Handled = true;
        }
        private void RedoCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var caretIndex = txtDocument.CaretIndex;
            var redoOp = DocumentState.Redo(ref caretIndex);
            UpdateDocumentTextFromState();
            txtDocument.CaretIndex = caretIndex;
            SendOperation(redoOp);
            e.Handled = true;
        }
        private void Document_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyAndSendChanges((TextBox)sender, e.Changes);

            UpdateDocumentTextFromState();
        }
        private void UpdateDocumentTextFromState()
        {
            txtDocument.TextChanged -= Document_TextChanged;

            OldText = txtDocument.Text;
            txtDocument.Text = DocumentState.CurrentState; // Apply local changes via the document state

            txtDocument.TextChanged += Document_TextChanged;
        }

        private void ApplyAndSendChanges(TextBox sender, ICollection<TextChange> textChanges)
        {
            var caretIndex = txtDocument.CaretIndex;
            foreach (var change in textChanges)
            {
                AppliedOperation appliedOperation = null;

                if (change.AddedLength != 0)
                {
                    var addedString = (sender).Text.Substring(change.Offset, change.AddedLength);
                    var changeCharOffset = change.Offset;
                    foreach (var c in addedString)
                    {
                        appliedOperation = new AppliedOperation(new InsertOperation(DocumentState, changeCharOffset++, c), DocumentState);
                        DocumentState.ApplyTransform(appliedOperation, ref caretIndex);
                        SendOperation(appliedOperation);
                    }
                }
                else
                {
                    var removedString = (sender).Text.Substring(change.Offset, change.RemovedLength);
                    foreach (var c in removedString)
                    {
                        appliedOperation = new AppliedOperation(new DeleteOperation(DocumentState, change.Offset), DocumentState);
                        DocumentState.ApplyTransform(appliedOperation, ref caretIndex);
                        SendOperation(appliedOperation);
                    }
                }
            }
            txtDocument.CaretIndex = caretIndex;
        }

        private void SendOperation(AppliedOperation appliedOperation)
        {
            using (var ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, appliedOperation);
                Task.Delay(10000).ContinueWith(t => SocketClient.Send(ms.ToArray()));
            }
        }
    }
}
