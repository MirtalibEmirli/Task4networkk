using Server;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

var a = Task.Run(() =>
{

    var ip = IPAddress.Parse("10.1.18.4");
    var port = 27001;
    using var listener = new TcpListener(ip, port);
    listener.Start();
    while (true)
    {
        var processDTOs = Process.GetProcesses().Select(p => new PRocessDTO( p.Id, p.ProcessName)).ToList();

        var datas = JsonSerializer.Serialize(processDTOs);

        var client = listener.AcceptTcpClient();
        var stream = client.GetStream();
        var bytes = Encoding.UTF8.GetBytes(datas);
        stream.Write(bytes);
    }

});


await a;














