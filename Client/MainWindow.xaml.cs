using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();
            SocketClient = new SocketClient();
            SocketClient.MessageReceived += SocketClient_MessageReceived;
        }

        private void SocketClient_MessageReceived(object sender, SocketClient.MessageReceivedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                txtDocument.TextChanged -= TextBox_TextChanged;
                txtDocument.Text = txtDocument.Text + Encoding.UTF8.GetString(e.Message);
                txtDocument.TextChanged += TextBox_TextChanged;
            });
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            foreach(var change in e.Changes)
            {
                var addedString = ((TextBox)sender).Text.Substring(change.Offset, change.AddedLength);
                //var removedString = ((TextBox)sender).Text.Substring(change.Offset, change.RemovedLength);
                SocketClient.SendData(Encoding.UTF8.GetBytes(addedString));
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            SocketClient.Dispose();
            base.OnClosed(e);
        }
    }
}
