using System;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Readers.Zip;
using SharpCompress.Writers;

namespace MyServiceBus.MessagesCompressor
{
    public static class MessagesCompressor
    {

        public static ReadOnlyMemory<byte> ToReadOnlyMemory(this MemoryStream stream)
        {
            return new ReadOnlyMemory<byte>(stream.GetBuffer(), 0, (int)stream.Length);
        }

        private const string ZipEntryName = "d";

        public static ReadOnlyMemory<byte> Compress(this ReadOnlyMemory<byte> source)
        {
            using var memoryStream = new MemoryStream(source.Length);
            memoryStream.Write(source.Span);
            memoryStream.Position = 0;

            return Compress(memoryStream);
        }

        public static ReadOnlyMemory<byte> Compress(this MemoryStream sourceStream)
        {
            var zipResultStream = new MemoryStream();

            using var zipWriter = WriterFactory.Open(zipResultStream, ArchiveType.Zip, new WriterOptions(CompressionType.Deflate));

            zipWriter.Write(ZipEntryName, sourceStream);

            return zipResultStream.ToArray();
        }


        private static readonly ReaderOptions ReaderOptions = new ReaderOptions();

        public static ReadOnlyMemory<byte> Decompress(this ReadOnlyMemory<byte> src)
        {
            
            var srcStream = new MemoryStream(src.Length);
            srcStream.Write(src.Span);
            srcStream.Position = 0;

            var reader = (ZipReader)ReaderFactory.Open(srcStream, ReaderOptions);

            reader.MoveToNextEntry();
            
            var resultStream = new MemoryStream();
            
            reader.WriteEntryTo(resultStream);

            return resultStream.ToReadOnlyMemory();
        }



    }
}