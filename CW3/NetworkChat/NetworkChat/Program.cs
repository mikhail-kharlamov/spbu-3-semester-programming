// See https://aka.ms/new-console-template for more information

using NetworkChat;


var client = new Client(8000, "127.0.0.1");
await client.Run();
