
using Server;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

var serverProcess = Task.Run(() =>
{

    var ip = IPAddress.Parse("192.168.1.105");
    var port = 27001;
    using var listener = new TcpListener(ip, port);
    listener.Start();
    while (true)
    {
        try
        {
            using var client = listener.AcceptTcpClient();
            using var stream = client.GetStream();
            Console.WriteLine("Client connected...");
            var bytes = new byte[10000];
            var bytesRead = stream.Read(bytes, 0, bytes.Length);
            if (bytesRead == 0)
            {
                Console.WriteLine("No data received.");
                continue; 
            }
            var recievedData = Encoding.UTF8.GetString(bytes).TrimEnd('\0');
            var command = JsonSerializer.Deserialize<Command>(recievedData);
            if (command != null)
            {
                if (command.Type == "Kill" && command.Name != null)
                {
                    try
                    {
                        var process = Process.GetProcessById(command.Id);
                        if (process != null) { process.Kill(); Console.WriteLine($"{process.ProcessName} killed"); }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;

                        Console.WriteLine("error killing "+ex.Message );
                        Console.ForegroundColor = ConsoleColor.White;

                    }
                }

                else if (command.Type == "Start" && command.Name != null)
                {

                    try
                    {
                        var pr = Process.Start(command.Name);
                        var message = $"{command.Name} process started.";
                        Console.WriteLine(message);
                        var bytes1 = Encoding.UTF8.GetBytes(message);
                        stream.Write(bytes1, 0, bytes1.Length);
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                    }
                }
                else if (command.Refresh == true)
                {
                    try
                    {
                        Console.WriteLine("Refresh wanted");
                        var processes = new List<PRocessDTO>();
                        var allProcesses = Process.GetProcesses();
                        foreach (var process in allProcesses)
                        {
                            processes.Add(new PRocessDTO(process.Id, process.ProcessName));
                        }

                        string datas = JsonSerializer.Serialize(processes);
                        var buffer = Encoding.UTF8.GetBytes(datas);
                        stream.Write(buffer, 0, buffer.Length);

                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                    }
                }

            }
        }
        catch (Exception)
        {

            throw;
        }
    }
});


await serverProcess;














