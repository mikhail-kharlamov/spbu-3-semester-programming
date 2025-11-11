// <copyright file="ClientServerTests.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

using FTPServer.ServerObjects;

namespace FTPServer;

/// <summary>
/// End-to-end client–server integration tests that validate the basic protocol
/// behavior over TCP and filesystem interaction for directory listing responses.
/// </summary>
public class ClientServerTests
{
    private Server server = null!;

    private Client client = null!;

    private int port;

    private string rootDir = null!;

    private string filePath = null!;

    private byte[] fileBytes = null!;

    /// <summary>
    /// One-time asynchronous setup that prepares the filesystem layout,
    /// starts the server, and initializes the client for subsequent tests.
    /// Precondition: the chosen port must be available; the server start routine
    /// should be suitable for long-running execution within the fixture lifetime.
    /// </summary>
    /// <returns>A task from void.</returns>
    [OneTimeSetUp]
    public async Task Setup()
    {
        this.rootDir = this.CreateTempDir();
        Directory.CreateDirectory(Path.Combine(this.rootDir, "sub"));
        this.filePath = Path.Combine(this.rootDir, "data.txt");
        this.fileBytes = System.Text.Encoding.UTF8.GetBytes("hello-world");
        await File.WriteAllBytesAsync(this.filePath, this.fileBytes);

        this.port = 8888;
        this.server = new Server();
        await this.server.Start(this.port);

        this.client = new Client("127.0.0.1", this.port);
    }

    /// <summary>
    /// One-time teardown that disposes network resources and removes
    /// the temporary filesystem artifacts created during setup.
    /// Ensures test isolation by cleaning up the temporary root directory.
    /// </summary>
    [OneTimeTearDown]
    public void TearDown()
    {
        this.client.Dispose();
        this.server.Dispose();
        Directory.Delete(this.rootDir, true);
    }

    /// <summary>
    /// Verifies that listing the prepared root directory returns exactly two entries
    /// (one file and one directory) and that the reported types match the expected order.
    /// Arrange: a root directory with "data.txt" and a "sub" directory. Act: call List on root.
    /// Assert: Size equals 2; first entry is File, second entry is Directory.
    /// </summary>
    /// <returns>A task from void.</returns>
    [Test]
    public async Task ListRootDirectoryTest()
    {
        var response = await this.client.List(this.rootDir);
        Assert.That(response.Size, Is.EqualTo(2));
        Assert.That(response.Objects[0].Type, Is.EqualTo(SystemObjectType.File));
        Assert.That(response.Objects[1].Type, Is.EqualTo(SystemObjectType.Directory));
    }

    private string CreateTempDir()
    {
        var dir = Path.Combine(Path.GetTempPath(), "ftpserver-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        return dir;
    }
}
