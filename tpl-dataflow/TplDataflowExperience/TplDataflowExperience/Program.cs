using System.Diagnostics;
using TplDataflowExperience;

var stopwatch = Stopwatch.StartNew();

var inputFile = "/Users/mikhailkharlamov/Documents/input.txt";
var outputFile = "/Users/mikhailkharlamov/Documents/output.gz";

var compressor = new Compressor();

using var inputStream = File.OpenRead(inputFile);
using (var outputStream = File.Create(outputFile))
{
    compressor.Compress(inputStream, outputStream, parallel: true);
}

stopwatch.Stop();

Console.WriteLine($"Elapsed time: {stopwatch.Elapsed}");
