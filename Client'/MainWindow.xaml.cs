using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client_
{

    public partial class MainWindow : Window
    {

        public ObservableCollection<PRocessDTO> processList { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            processList = new ObservableCollection<PRocessDTO>();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
                { 
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("Run started");
                    });

                    var ip = "192.168.1.105";
                    var port = 27001;
                    using var client = new TcpClient(ip, port);
                    using var stream = client.GetStream();

                    if (prListView.SelectedItem != null && Kill.IsChecked == true)
                    {
                        var pr = prListView.SelectedItem as Process;///null gelir 
                        Command command = new()
                        {
                            Name = pr.ProcessName,
                            Id = pr.Id,
                            Type =   "Kill"  
                        };

                        var prstring = JsonSerializer.Serialize(command);
                        var buffer = Encoding.UTF8.GetBytes(prstring);
                        stream.Write(buffer, 0, buffer.Length);
                    }
                    else 
                    {
                        Command command = new()
                        {
                            Name = prName.Text,
                            Type =   "Start"
                        };

                        var prstring = JsonSerializer.Serialize(command);
                        var buffer = Encoding.UTF8.GetBytes(prstring);
                        stream.Write(buffer, 0, buffer.Length);
                    }

                   /* var bytes = new byte[1024];
                    var bytesRead = stream.Read(bytes, 0, bytes.Length);
                    var serverResponse = Encoding.UTF8.GetString(bytes, 0, bytesRead);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(serverResponse);
                    });*/
                 }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(ex.Message);
                    });
                } 
            
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            try
            {
                var ip = "192.168.1.105";
                var port = 27001;
                var client = new TcpClient(ip, port);
                var stream = client.GetStream();
                var bytes = new byte[1024];
                Command c = new Command() { Refresh = true };
                var prstring = JsonSerializer.Serialize(c);
                var buffer = Encoding.UTF8.GetBytes(prstring);
                stream.Write(buffer);


                //recieving
                var buffer2 = new byte[10000];
                stream.Read(buffer2, 0, buffer2.Length);
                var data = Encoding.UTF8.GetString(buffer2).TrimEnd('\0');
                var processes = JsonSerializer.Deserialize<List<PRocessDTO>>(data);// 
               
                Application.Current.Dispatcher.Invoke(() =>
                {
                    processList.Clear();  
                    foreach (var process in processes)
                    {
                        processList.Add(process);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
    }
}