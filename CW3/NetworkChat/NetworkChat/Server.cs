// <copyright file="Server.cs" company="MyCompany">
// Copyright (c) MyCompany. All rights reserved.
// </copyright>
using System.Net;
using System.Net.Sockets;

namespace NetworkChat;

public class Server(int port, string? host = null) : BaseChat(port, host: host)
{
    public override async Task Run()
    {
        IPAddress bindAddress;

        if (string.IsNullOrWhiteSpace(this.host))
        {
            bindAddress = IPAddress.Any;
        }
        else if (!IPAddress.TryParse(this.host, out bindAddress))
        {
            throw new ArgumentException($"Invalid IP address: {this.host}", nameof(this.host));
        }

        var listener = new TcpListener(bindAddress, this.port);
        listener.Start();
        Console.WriteLine($"Listening on port {this.port}...");
        while (true)
        {
            var socket = await listener.AcceptSocketAsync();
            await Task.Run(
                async () =>
                {
                    var stream = new NetworkStream(socket);
                    await this.RunSessionAsync(stream);
                    socket.Close();
                });
        }
    }
}
