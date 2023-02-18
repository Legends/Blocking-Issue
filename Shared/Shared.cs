using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;

namespace Shared
{
    public static class Piper
    {
        public static async Task<byte[]> ReadMessageAsync(PipeStream pipe)
        {
            using MemoryStream ms = new MemoryStream();
            byte[] buffer = new byte[0x1000]; // Read in 4 KB blocks

            do
            {
                // pipe reads from stream and writes into buffer
                // ms reads from buffer into own memory stream
                await ms.WriteAsync(buffer, 0, await pipe.ReadAsync(buffer, 0, buffer.Length));
            }
            while (!pipe.IsMessageComplete);

            return ms.ToArray();
        }

        public static async Task<string> ReadMessageAsyncAsString(PipeStream pipe)
        {
            var result = await ReadMessageAsync(pipe);
            return await Task.FromResult(Encoding.UTF8.GetString(result));
        }

        public static async Task WriteMessageAsyncAsString(PipeStream pipe, string msg)
        {
            var bMsg = Encoding.UTF8.GetBytes(msg);
            await pipe.WriteAsync(bMsg);
        }

        public static async Task<T?> ReadMessageAsyncAsObject<T>(PipeStream pipe)
        {
            //var bt = await ReadMessageAsync(pipe);
            //var obj = JsonSerializer.Deserialize<T>(bt);
            //return await Task.FromResult(obj);
            //Console.WriteLine("CALLING  JsonSerializer.DeserializeAsync<T>(pipe) ");
            //Debugger.Launch();
            //Debugger.Break();

            return await JsonSerializer.DeserializeAsync<T>(pipe); // !!! doesn't work !!! application will block
        }

        public static async Task WriteMessageAsyncAsObject<T>(PipeStream pipe, T obj)
        {
            await JsonSerializer.SerializeAsync<T>(pipe, obj);            
        }
    }

    [Serializable]
    public class SomeObject
    {

        int fieldProp2 = 0;

        public string Property1 { get; set; }
        public int Property2
        {
            get { return fieldProp2; }
            set { fieldProp2 = value; }
        }

        public SomeObject()
        {
            Property1 = string.Empty;
        }
    }
}