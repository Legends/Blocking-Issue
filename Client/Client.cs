﻿using Shared;
using System.IO.Pipes;

namespace Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("I AM THE CLIENT");

                var pipeName = args[0];
            
                using var client = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
                await client.ConnectAsync();

                client.ReadMode = PipeTransmissionMode.Message;

                #region PIPE MESSAGE TRANSFER

                #region String transfer
                //// DATA WRITTEN AS STRING
                //await Piper.WriteMessageAsyncAsString(client, "Message from Client: 1st msg!");

                //// DATA READ AS STRING
                //Console.WriteLine(await Piper.ReadMessageAsyncAsString(client));

                //// DATA WRITTEN AS STRING
                //await Piper.WriteMessageAsyncAsString(client, "Message from Client: 2nd msg!");

                //// DATA READ AS STRING
                //Console.WriteLine(await Piper.ReadMessageAsyncAsString(client));
                #endregion

                // DATA READ AS OBJECT
                var obj = await Piper.ReadMessageAsyncAsObject<SomeObject>(client);
                Console.WriteLine($"Response from server: {obj?.Property1} - {obj?.Property2}");

                // DATA SENT AS OBJECT
                var respObj = new SomeObject() { Property1 = "SomeValueFromClient", Property2 = 400 };
                await Piper.WriteMessageAsyncAsObject<SomeObject>(client, respObj);

                #endregion

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception on client: {ex.Message}");
            }

            Console.WriteLine("client is about to exit");
            Console.Read();
        }


    } // CLASS
}// NS