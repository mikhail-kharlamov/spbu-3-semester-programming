// <copyright file="ChatEndpointBase.cs" company="MyCompany">
// Copyright (c) MyCompany. All rights reserved.
// </copyright>

using System.Net.Sockets;

namespace NetworkChat;

public abstract class BaseChat(int port, string? host = null)
{
    protected readonly string? host = host;
    protected readonly int port = port;

    public abstract Task Run();

    protected async Task RunSessionAsync(NetworkStream stream, CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);
        using var writer = new StreamWriter(stream, leaveOpen: true) { AutoFlush = true };

        using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var consoleTask = this.ConsoleReadLoopAsync(writer, cancellationTokenSource);
        var networkTask = this.NetworkReadLoopAsync(reader, cancellationTokenSource);

        await Task.WhenAny(consoleTask, networkTask);
        await cancellationTokenSource.CancelAsync();

        try
        {
            await Task.WhenAll(consoleTask, networkTask);
        }
        catch (OperationCanceledException)
        {
            /* ok */
        }
    }

    protected virtual async Task ConsoleReadLoopAsync(StreamWriter writer, CancellationTokenSource token)
    {
        while (!token.IsCancellationRequested)
        {
            var line = await Console.In.ReadLineAsync(token.Token);
            Console.WriteLine($"Console Прочел {line}");

            if (line is null)
            {
                await token.CancelAsync();
                return;
            }

            await writer.WriteLineAsync(line);
            Console.WriteLine("Отправил");

            if (string.Equals(line, "exit", StringComparison.OrdinalIgnoreCase))
            {
                await token.CancelAsync();
                return;
            }
        }
    }

    protected virtual async Task NetworkReadLoopAsync(StreamReader reader, CancellationTokenSource cancellationTokenSource)
    {
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationTokenSource.Token);
            Console.WriteLine($"Network Прочел {line}");

            if (line is null)
            {
                await cancellationTokenSource.CancelAsync();
                return;
            }

            Console.WriteLine(line);

            if (string.Equals(line, "exit", StringComparison.OrdinalIgnoreCase))
            {
                await cancellationTokenSource.CancelAsync();
                return;
            }
        }
    }
}
