using System.Net.Sockets;
using System.Net;

namespace FTPServer;

public class Server
{
    private TcpListener server;

    public async Task Start(Int32 port)
    {
        try
        {
            var localAddr = IPAddress.Parse("127.0.0.1");

            this.server = new TcpListener(localAddr, port);

            server.Start();

            var bytes = new byte[256];
            var data = string.Empty;

            while(true)
            {
                Console.Write("Waiting for a connection... ");
                
                using var client = await server.AcceptTcpClientAsync();
                Console.WriteLine("Connected!");
                
                var stream = client.GetStream();

                var i = 0;

                while((i = await stream.ReadAsync(bytes, 0, bytes.Length)) != 0)
                {
                    data = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
                    Console.WriteLine("Received: {0}", data);
                    
                    var message = await this.Handler(data);
                    
                    await stream.WriteAsync(message, 0, message.Length);
                    Console.WriteLine("Sent: {0}", message);
                }
            }
        }
        catch(SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            server.Stop();
        }

        Console.WriteLine("\nHit enter to continue...");
    }

    private async Task<byte[]> Handler(string data)
    {
        if (!this.CheckFormat(data))
        {
            throw new Exception($"Invalid data: {data}");
        }
        
        switch (data[0])
        {
            case '1':
                return this.List(data.Split()[1]);
            case '2':
                return await this.Get(data.Split()[1]);
            default:
                throw new Exception($"Invalid data: {data}");
        }
    }

    private bool CheckFormat(string query)
    {
        var words = query.Split(" ");
        if (words.Length != 2)
        {
            return false;
        }
        
        if (words[0] != "1" && words[0] != "2")
        {
            return false;
        }

        return words[1][^1] == '\n';
    }

    private byte[] List(string path)
    {
        if (this.GetSystemObjectInfo(path) != SystemObjectType.Directory)
        {
            return  "-1"u8.ToArray();
        }
        
        var directories = Directory.GetDirectories(path);
        var files = Directory.GetFiles(path);
        
        var result = (directories.Length + files.Length).ToString();
        foreach (var file in files)
        {
            result += $" {file} false";
        }
        
        foreach (var directory in directories)
        {
            result += $" {directory} true";
        }
        
        result += "\n";
        var message = System.Text.Encoding.UTF8.GetBytes(result);
        return message;
    }
    
    private async Task<byte[]> Get(string path)
    {
        if (this.GetSystemObjectInfo(path) != SystemObjectType.File)
        {
            return "-1"u8.ToArray();
        }
        
        var bytes = await File.ReadAllBytesAsync(path);
        var size = bytes.LongLength;
        var sizeBytes = BitConverter.GetBytes(size);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(sizeBytes);
        }

        if (size == -1)
        {
            return sizeBytes;
        }
        
        var result = new byte[sizeBytes.Length + bytes.Length];
        Array.Copy(sizeBytes, 0, result, 0, sizeBytes.Length);
        Array.Copy(bytes, 0, result, sizeBytes.Length, bytes.Length);
        return result;
    }
    
    private SystemObjectType GetSystemObjectInfo(string path)
    {
        if (Directory.Exists(path))
        {
            return SystemObjectType.Directory;
        }

        if (File.Exists(path))
        {
            return SystemObjectType.File;
        }
        
        return SystemObjectType.NotExists;
    }
}
