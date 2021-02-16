using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace MyServiceBus.MessagesCompressor.Tests
{
    
    public class TestMessage
    {
        public DateTime Created { get; set; }
        
        public string Data { get; set; }
        
        public long MessageId { get; set; }
    }
    
    public class TestCompressDecompressOperations
    {
        [Test]
        public void TestCompressDecompress()
        {


            var source = new List<TestMessage>();

            for (var i=0; i<100; i++)
            {

                var newEl = new TestMessage
                {
                    Created = DateTime.UtcNow,
                    Data = Convert.ToBase64String(new byte[1024]),
                    MessageId = i
                };
                
                source.Add(newEl);
            }

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(source);

            var sourceBytes = Encoding.UTF8.GetBytes(json);

            var compressedPage = new ReadOnlyMemory<byte>(sourceBytes).Compress();

            var decompressedBytes = compressedPage.Decompress();
            
            var destJson = Encoding.UTF8.GetString(decompressedBytes.ToArray());
            
            var destResult = Newtonsoft.Json.JsonConvert.DeserializeObject<TestMessage[]>(destJson);
            

            Assert.AreEqual(sourceBytes.Length, decompressedBytes.Length);
            
            Assert.AreEqual(source.Count, destResult.Length);

        }
    }



}