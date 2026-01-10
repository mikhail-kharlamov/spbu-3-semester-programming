// <copyright file="Client.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

using System.Net.Sockets;
using FTPServer.ServerObjects;

namespace FTPServer;

/// <summary>
/// Represents a TCP client that connects to a server and provides methods
/// to request a directory listing or to download a file asynchronously.
/// </summary>
public class Client : IDisposable
{
    private readonly TcpClient client;

    private readonly Stream stream;

    private bool shutDown = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class.
    /// </summary>
    /// <param name="server">The server address to connect to.</param>
    /// <param name="port">The TCP port number to connect to.</param>
    public Client(string server, int port)
    {
        this.client = new TcpClient(server, port);
        this.stream = this.client.GetStream();
    }

    /// <summary>
    /// Sends a request to the server to retrieve a directory listing at the specified path.
    /// </summary>
    /// <param name="path">The directory path for which to get the listing.</param>
    /// <returns>A task representing the asynchronous operation, with a result of <see cref="ListResponseInfo"/> containing the directory listing information.</returns>
    public async Task<ListResponseInfo> List(string path)
    {
        ObjectDisposedException.ThrowIf(this.shutDown, this);

        var message = $"1 {path}\n";
        var response = await this.Request(message);
        return this.ParseListResponse(response);
    }

    /// <summary>
    /// Sends a request to the server to retrieve the contents of a file at the specified path.
    /// </summary>
    /// <param name="path">The file path to download.</param>
    /// <returns>A task representing the asynchronous operation, with a result of <see cref="GetResponseInfo"/> containing file data and metadata.</returns>
    public async Task<GetResponseInfo> Get(string path)
    {
        ObjectDisposedException.ThrowIf(this.shutDown, this);

        var message = $"2 {path}\n";
        var response = await this.Request(message);
        return await this.ParseGetResponse(response);
    }

    /// <summary>
    /// Object dispose.
    /// </summary>
    public void Dispose()
    {
        this.stream.Dispose();
        this.client.Dispose();
        this.shutDown = true;
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
        var parts = message.Trim().Split();
        var objects = new List<ServerObjectInfo>();
        for (var i = 1; i < parts.Length - 1; i += 2)
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

    private async Task<GetResponseInfo> ParseGetResponse(ResponseInfo response)
    {
        const int maxExpectedSize = 65536;
        var buffer = new byte[maxExpectedSize];
        var totalRead = 0;
        var expectedLength = -1;

        while (true)
        {
            var read = await this.stream.ReadAsync(buffer, totalRead, buffer.Length - totalRead);
            if (read == 0)
            {
                throw new IOException("The connection is closed.");
            }

            totalRead += read;

            if (expectedLength == -1 && totalRead >= 8)
            {
                var sizeBuf = buffer.Take(8).ToArray();
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(sizeBuf);
                }

                expectedLength = (int)BitConverter.ToInt64(sizeBuf, 0);
                if (expectedLength == -1)
                {
                    break;
                }
            }

            if (expectedLength != -1 && totalRead >= expectedLength + 8)
            {
                break;
            }
        }

        if (expectedLength == -1)
        {
           return new GetResponseInfo(expectedLength, (byte[])[]);
        }

        var fileContent = new byte[expectedLength];
        Array.Copy(buffer, 8, fileContent, 0, expectedLength);
        return new GetResponseInfo(expectedLength, fileContent);
    }
}
