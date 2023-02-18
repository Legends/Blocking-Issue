using Shared;
using System.Diagnostics;
using System.IO.Pipes;
using System.Reflection;
using System.Text;

namespace Server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("I AM THE SERVER");

                var t = new Thread(CallClient);
                t.Start();

                using var server = new NamedPipeServerStream(
                                                            "hostpipe",
                                                            PipeDirection.InOut,
                                                            NamedPipeServerStream.MaxAllowedServerInstances,
                                                            PipeTransmissionMode.Message,
                                                            PipeOptions.Asynchronous);

                await server.WaitForConnectionAsync();

                #region PIPE MESSAGE TRANSFER

                // DATA READ AS STRING
                Console.WriteLine(await Piper.ReadMessageAsyncAsString(server));

                // DATA WRITTEN AS STRING
                await Piper.WriteMessageAsyncAsString(server, "Message from Server: 1st msg");

                // DATA READ AS STRING
                Console.WriteLine(await Piper.ReadMessageAsyncAsString(server));

                // DATA WRITTEN AS STRING
                await Piper.WriteMessageAsyncAsString(server, "Message from Server: 2nd msg");

                // DATA SENT AS OBJECT
                var obj = new SomeObject() { Property1 = "SomeValueFromServer", Property2 = 20 };
                await Piper.WriteMessageAsyncAsObject<SomeObject>(server, obj);

                // DATA READ AS OBJECT
                var respObj = await Piper.ReadMessageAsyncAsObject<SomeObject>(server);
                Console.WriteLine($"Response from Client: {respObj?.Property2} - {respObj?.Property1}");

                #endregion

                Console.WriteLine("Client thread is about to join!");
                t.Join();

                Console.WriteLine("Client thread joined");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception on server: {ex.Message}");
            }
        }


        static void CallClient()
        {

            var psi = new ProcessStartInfo()
            {
                Arguments = "hostpipe",
                UseShellExecute = true,
                FileName = @"..\..\..\..\Client\bin\Debug\net7.0\Client.exe"
            };

            using Process? p = Process.Start(psi);
            p?.WaitForExit();

            Console.WriteLine(" p.WaitForExit() called");
        }
    }
}