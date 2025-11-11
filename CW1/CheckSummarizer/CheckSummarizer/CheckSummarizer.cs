// <copyright file="CheckSummarizer.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace CheckSummarizer;

/// <summary>
/// Provides methods to generate MD5 hash summaries of system objects such as files and directories.
/// This class supports both sequential and parallel summarization for directories.
/// </summary>
public class CheckSummarizer
{
    /// <summary>
    /// Asynchronously summarizes the system object at the specified path by computing an MD5 hash.
    /// If the path is a directory, recursively computes the summary for all contained files and subdirectories sequentially.
    /// If the path is a file, computes the summary for that file.
    /// </summary>
    /// <param name="path">The file system path to the file or directory to summarize.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the MD5 hash summary as a byte array.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the specified path does not exist as either a file or directory.</exception>
    public async Task<byte[]> SummarizeSystemObject(string path)
    {
        var type = this.CheckObjectType(path);
        if (type == SystemObjectType.Directory)
        {
            return await this.SummarizeDirectory(path);
        }

        if (type == SystemObjectType.File)
        {
            return await this.SummarizeFile(path);
        }

        throw new FileNotFoundException("File not found", path);
    }

    /// <summary>
    /// Asynchronously summarizes the system object at the specified path by computing an MD5 hash.
    /// If the path is a directory, recursively computes the summary for all contained files and subdirectories in parallel.
    /// If the path is a file, computes the summary for that file.
    /// </summary>
    /// <param name="path">The file system path to the file or directory to summarize.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the MD5 hash summary as a byte array.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the specified path does not exist as either a file or directory.</exception>
    public async Task<byte[]> SummarizeSystemObjectParallel(string path)
    {
        var type = this.CheckObjectType(path);
        if (type == SystemObjectType.Directory)
        {
            return await this.SummarizeDirectoryParallel(path);
        }

        if (type == SystemObjectType.File)
        {
            return await this.SummarizeFile(path);
        }

        throw new FileNotFoundException("File not found", path);
    }

    private async Task<byte[]> SummarizeDirectoryParallel(string path)
    {
        var subDirectories = Directory.GetDirectories(path);
        var subFiles = Directory.GetFiles(path);
        var result = new List<byte>();
        var resultDirs = new System.Collections.Concurrent.ConcurrentBag<byte>();


        var directoryTasks = new List<Task>();

        Parallel.For(
            0,
            subDirectories.Length,
            i =>
            {
                var subDirectory = subDirectories[i];
                var task = Task.Run(
                    async () =>
                {
                    var bytes = await this.SummarizeDirectory(subDirectory);
                    foreach (var b in bytes)
                    {
                        resultDirs.Add(b);
                    }
                });
                directoryTasks.Add(task);
            });

        await Task.WhenAll(directoryTasks);

        result = result.Concat(resultDirs).ToList();
        foreach (var subFile in subFiles)
        {
            result = result.Concat(await this.SummarizeFile(subFile)).ToList();
        }

        return result.ToArray();
    }


    private async Task<byte[]> SummarizeDirectory(string path)
    {
        var subDirectories = Directory.GetDirectories(path);
        var subFiles = Directory.GetFiles(path);
        var result = new List<byte>();

        foreach (var subDirectory in subDirectories)
        {
            result = result.Concat(await this.SummarizeDirectory(subDirectory)).ToList();
        }

        foreach (var subFile in subFiles)
        {
            result = result.Concat(await this.SummarizeFile(subFile)).ToList();
        }

        return result.ToArray();
    }

    private async Task<byte[]> SummarizeFile(string path)
    {
        var name = Path.GetFileNameWithoutExtension(path);
        var nameBytes = System.Text.Encoding.UTF8.GetBytes(name);
        var bytes = nameBytes.Concat(await this.ComputeMd5HashFromFile(path)).ToArray();
        await using var memoryStream = new MemoryStream(bytes);
        using var md5 = System.Security.Cryptography.MD5.Create();
        var hash = await md5.ComputeHashAsync(memoryStream);
        return hash;
    }

    private async Task<byte[]> ComputeMd5HashFromFile(string path)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        await using var stream = File.OpenRead(path);
        return await md5.ComputeHashAsync(stream);
    }

    private SystemObjectType CheckObjectType(string path)
    {
        if (File.Exists(path))
        {
            return SystemObjectType.File;
        }

        if (Directory.Exists(path))
        {
            return SystemObjectType.Directory;
        }

        return SystemObjectType.NotExists;
    }
}
