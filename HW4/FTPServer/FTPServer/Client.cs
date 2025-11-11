using System.Net.Sockets;

namespace FTPServer;

public class Client
{
    private readonly TcpClient client;
    
    private readonly Stream stream;
    
    public Client(string server, int port)
    {
        this.client = new TcpClient(server, port);
        this.stream = this.client.GetStream();
    }

    public async Task<ListResponseInfo> List(string path)
    {
        var message = $"1 {path}\n";
        var response = await this.Request(message);
        return this.ParseListResponse(response);
    }

    public async Task<GetResponseInfo> Get(string path)
    {
        var message = $"2 {path}\n";
        var response = await this.Request(message);
        return this.ParseGetResponse(response);
    }

    private async Task<ResponseInfo> Request(string message)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(message);
        await this.stream.WriteAsync(bytes, 0, bytes.Length);
        
        var buffer = new byte[65536];
        var bytesRead = await this.stream.ReadAsync(buffer, 0, buffer.Length);
        return new ResponseInfo(buffer, bytesRead);
    }

    private ListResponseInfo ParseListResponse(ResponseInfo response)
    {
        var message = System.Text.Encoding.UTF8.GetString(response.Buffer, 0, response.Length);
        var parts = message.Split();
        var objects = new List<ServerObjectInfo>();
        for (var i = 1; i < parts.Length - 2; i++)
        {
            var type = SystemObjectType.NotExists;
            switch (parts[i + 1].Trim())
            {
                case "true":
                    type = SystemObjectType.Directory;
                    break;
                case "false":
                    type = SystemObjectType.File;
                    break;
                default:
                    throw new Exception($"Unknown type: {parts[i + 1]}");
            }
            
            var serverObject = new ServerObjectInfo(type, parts[i]);
            objects.Add(serverObject);
        }
        
        return new ListResponseInfo(int.Parse(parts[0]), objects.ToArray());
    }

    private GetResponseInfo ParseGetResponse(ResponseInfo response)
    {
        
    }
}
