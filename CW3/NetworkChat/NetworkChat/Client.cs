// <copyright file="Client.cs" company="MyCompany">
// Copyright (c) MyCompany. All rights reserved.
// </copyright>
using System.Net.Sockets;

namespace NetworkChat;

public class Client(int port, string? host = null) : BaseChat(port, host: host)
{
    public override async Task Run()
    {
        using (var client = new TcpClient(this.host!, this.port))
        {
            var stream = client.GetStream();
            await this.RunSessionAsync(stream);
        }
    }
}
