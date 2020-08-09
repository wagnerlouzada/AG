using System;
using AgProtocol;

namespace AgClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // Ag server address
            string address = AgProtocol.AgProtocol.Address;
            if (args.Length > 0)
                address = args[0];

            // Ag server port
            int port = AgProtocol.AgProtocol.Port;
            if (args.Length > 1)
                port = int.Parse(args[1]);

            Console.WriteLine($"Ag Server Address: {address}");
            Console.WriteLine($"Ag Server Port: {port}");

            Console.WriteLine();

            // Create a new Ag client
            var client = new AgClientEx(address, port);

            Console.WriteLine("Press Enter to stop the client or '!' to reconnect the client...");

            // Perform text input
            for (;;)
            {
                string line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                    break;

                // Reconnect the client
                if (line == "!")
                {
                    Console.Write("Client reconnecting...");
                    if (client.IsConnected)
                        client.ReconnectAsync();
                    else
                        client.ConnectAsync();
                    Console.WriteLine("Done!");
                    continue;
                }

                var commands = line.Split(' ');
                if (commands.Length < 2)
                {
                    Console.WriteLine("Ag method and Content KEY must be entered!");
                    continue;
                }
           
                switch (commands[0].ToUpper()) {
                    case "HEAD":
                        var responseH = client.SendHeadRequest(commands[1]).Result;
                        Console.WriteLine(responseH);
                        break;
                    case "GET":
                        var responseG = client.SendGetRequest(commands[1]).Result;
                        Console.WriteLine(responseG);
                        break;
                    case "DOWNLOAD":
                        var responseDl = client.SendGetRequest(commands[1]).Result;
                        Console.WriteLine(responseDl);
                        break;
                    case "LIST":
                        var responseL = client.SendGetRequest(commands[1]).Result;
                        Console.WriteLine(responseL);
                        break;
                    case "UPDATE":
                        if (commands.Length < 3)
                        {
                            Console.WriteLine("Ag method, Content KEY and body DATA be entered!");
                            continue;
                        }
                        var responseU = client.SendPostRequest(commands[1], commands[2]).Result;
                        Console.WriteLine(responseU);
                        break;
                    case "SAVE":
                        if (commands.Length < 3)
                        {
                            Console.WriteLine("Ag method, Content KEY and body DATA be entered!");
                            continue;
                        }
                        var responseS = client.SendPostRequest(commands[1], commands[2]).Result;
                        Console.WriteLine(responseS);
                        break;
                    case "UPLOAD":
                        if (commands.Length < 3)
                        {
                            Console.WriteLine("Ag method, Content KEY and body DATA be entered!");
                            continue;
                        }
                        var responseUl = client.SendPostRequest(commands[1], commands[2]).Result;
                        Console.WriteLine(responseUl);
                        break;
                    case "DELETE":
                        var responseD = client.SendDeleteRequest(commands[1]).Result;
                        Console.WriteLine(responseD);
                        break;
                    case "OPTIONS":
                        var responseO = client.SendOptionsRequest(commands[1]).Result;
                        Console.WriteLine(responseO);
                        break;
                    case "LOGIN":
                        var responseLg = client.SendOptionsRequest(commands[1]).Result;
                        Console.WriteLine(responseLg);
                        break;
                    case "LOGOUT":
                        var responseLo = client.SendOptionsRequest(commands[1]).Result;
                        Console.WriteLine(responseLo);
                        break;
                    case "SHARE":
                        var responseSh = client.SendOptionsRequest(commands[1]).Result;
                        Console.WriteLine(responseSh);
                        break;
                    case "RIGHTS":
                        var responseRi = client.SendOptionsRequest(commands[1]).Result;
                        Console.WriteLine(responseRi);
                        break;
                    case "TRACE":
                        var responseT = client.SendTraceRequest(commands[1]).Result;
                        Console.WriteLine(responseT);
                        break;
                    default:
                        Console.WriteLine("Unknown Ag method");
                        break;
                }

            }

            // Disconnect the client
            Console.Write("Client disconnecting...");
            client.Disconnect();
            Console.WriteLine("Done!");
        }
    }
}
