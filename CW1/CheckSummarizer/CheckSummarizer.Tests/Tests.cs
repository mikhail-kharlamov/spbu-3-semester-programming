// <copyright file="Tests.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace CheckSummarizer;


public class CheckSummarizerTests
{
    private CheckSummarizer summarizer;

    [SetUp]
    public void Setup()
    {
        summarizer = new CheckSummarizer();
    }

    [Test]
    public async Task SummarizeSystemObject_File_ReturnsHash()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            await File.WriteAllTextAsync(tempFile, "test content");
            var result = await summarizer.SummarizeSystemObject(tempFile);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Test]
    public async Task SummarizeSystemObject_Directory_ReturnsHash()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);
        var tempFile = Path.Combine(tempDir, "file.txt");
        try
        {
            await File.WriteAllTextAsync(tempFile, "test content");
            var result = await this.summarizer.SummarizeSystemObject(tempDir);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
        }
        finally
        {
            File.Delete(tempFile);
            Directory.Delete(tempDir);
        }
    }

    [Test]
    public void SummarizeSystemObject_NonExistingPath_ThrowsFileNotFoundException()
    {
        var nonExistingPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Assert.ThrowsAsync<FileNotFoundException>(async () =>
            await this.summarizer.SummarizeSystemObject(nonExistingPath));
    }

    [Test]
    public async Task SummarizeSystemObjectParallel_File_ReturnsHash()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            await File.WriteAllTextAsync(tempFile, "test content");
            var result = await this.summarizer.SummarizeSystemObjectParallel(tempFile);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Test]
    public async Task SummarizeSystemObjectParallel_Directory_ReturnsHash()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);
        var tempFile = Path.Combine(tempDir, "file.txt");
        try
        {
            await File.WriteAllTextAsync(tempFile, "test content");
            var result = await this.summarizer.SummarizeSystemObjectParallel(tempDir);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
        }
        finally
        {
            File.Delete(tempFile);
            Directory.Delete(tempDir);
        }
    }
}
