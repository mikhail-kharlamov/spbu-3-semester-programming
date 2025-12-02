using System.IO.Compression;
using System.Threading.Tasks.Dataflow;

namespace TplDataflowExperience;

public class Compressor
{
    private const int BufferSize = 16384;

    private BufferBlock<byte[]> buffer;

    private TransformBlock<byte[], byte[]> compressor;

    private ActionBlock<byte[]> writer;

    public void Compress(Stream inputStream, Stream outputStream, bool parallel = false)
    {
        if (parallel)
        {
            this.InitParallelBlocks(outputStream);
        }
        else
        {
            this.InitOrdinaryBlocks(outputStream);
        }

        this.buffer.LinkTo(this.compressor);
        this.buffer.Completion.ContinueWith(_ => this.compressor.Complete());
        this.compressor.LinkTo(this.writer);
        this.compressor.Completion.ContinueWith(_ => this.writer.Complete());

        var readBuffer = new byte[BufferSize];
        while (true)
        {
            var readCount = inputStream.Read(readBuffer, 0, BufferSize);
            if (readCount > 0)
            {
                var postData = new byte[readCount];
                Buffer.BlockCopy(readBuffer, 0, postData, 0, readCount);
                while (!this.buffer.Post(postData))
                {
                }
            }

            if (readCount != BufferSize)
            {
                this.buffer.Complete();
                break;
            }
        }

        this.writer.Completion.Wait();
    }

    private void InitOrdinaryBlocks(Stream outputStream)
    {
        this.buffer = new BufferBlock<byte[]>();
        this.compressor = new TransformBlock<byte[], byte[]>(bytes => CompressBytes(bytes));
        this.writer = new ActionBlock<byte[]>(bytes => outputStream.Write(bytes));
    }

    private void InitParallelBlocks(Stream outputStream)
    {
        this.buffer = new BufferBlock<byte[]>(new DataflowBlockOptions { BoundedCapacity = 100 });
        var compressorOptions = new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = 4,
            BoundedCapacity = 100,
        };
        this.compressor = new TransformBlock<byte[], byte[]>(bytes => CompressBytes(bytes), compressorOptions);
        var writerOptions = new ExecutionDataflowBlockOptions
        {
            BoundedCapacity = 100,
            SingleProducerConstrained = true,
        };
        this.writer = new ActionBlock<byte[]>(bytes => outputStream.Write(bytes, 0, bytes.Length), writerOptions);
    }

    private static byte[] CompressBytes(byte[] bytes)
    {
        using var resultStream = new MemoryStream();
        using var gZipStream = new GZipStream(resultStream, CompressionMode.Compress);
        using var writer = new BinaryWriter(gZipStream);
        writer.Write(bytes);
        return resultStream.ToArray();
    }
}
