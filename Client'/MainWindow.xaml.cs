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

        public ObservableCollection<Process> processList { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                try
                {
                    var ip = "10.1.18.4";
                    var port = 27001;
                    var client = new TcpClient(ip, port);
                    var stream = client.GetStream();

                    //listwievden secilen haldi
                    if (prListView.SelectedItem != null && prName.Text == string.Empty)
                    {


                        if (Kill.IsChecked == true)
                        {
                            var pr = prListView.SelectedItem as Process;
                            Command command = new()
                            {
                                Name = pr.ProcessName,
                                Id = pr.Id,
                                Type = "Kill"
                            };

                            var prstring = JsonSerializer.Serialize(command);
                            var bytes = Encoding.UTF8.GetBytes(prstring);
                            stream.Write(bytes);

                        }
                        else
                        {
                            var pr = prListView.SelectedItem as Process;
                            Command command = new()
                            {
                                Name = pr.ProcessName,
                                Id = pr.Id,
                                Type = "Start"
                            };

                            var prstring = JsonSerializer.Serialize(command);
                            var bytes = Encoding.UTF8.GetBytes(prstring);
                            stream.Write(bytes);


                        }





                    }
                    else    
                    {
                        Command command = new()
                        {
                            Name =prName.Text
                                                };

                        if (Kill.IsChecked==true)
                        {
                            command.Type = "Kill";

                        }
                        else
                        {
                            command.Type = "Start";
                        }

                        var prstring = JsonSerializer.Serialize(command);
                        var buffer = Encoding.UTF8.GetBytes(prstring);
                        stream.Write(buffer);


                    }
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }
            });
            //burda run duymesi sixilir ,biz birinci yoxlamaliyiqki eyer listviewden secibse onu get edib run a gonderek yeni stream e adini yaz gonder 
            //serverde qebul ele 
           
            


        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            var ip = "10.1.18.4";
            var port = 27001;
            var client = new TcpClient(ip, port);
            var stream = client.GetStream();
            var bytes = new byte[1024];
            stream.Read(bytes,0,bytes.Length);
            var data = Encoding.UTF8.GetString(bytes);
            var processes = JsonSerializer.Deserialize<List<PRocessDTO>>(data);//json u tam oxu


        }
    }
}