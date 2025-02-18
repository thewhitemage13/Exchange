using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class UDPServer
{
    static Dictionary<string, double> exchangeRates = new Dictionary<string, double>
    {
        {"USD_EURO", 0.92},
        {"EURO_USD", 1.09},
        {"USD_GBP", 0.78},
        {"GBP_USD", 1.28},
        {"EURO_GBP", 0.85},
        {"GBP_EURO", 1.18}
    };

    static void Main()
    {
        TcpListener listener = new TcpListener(IPAddress.Any, 5000);
        listener.Start();
        Console.WriteLine("Server started. Waiting for connections...");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Thread clientThread = new Thread(HandleClient);
            clientThread.Start(client);
        }
    }

    static void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        NetworkStream stream = client.GetStream();
        IPEndPoint endPoint = (IPEndPoint)client.Client.RemoteEndPoint;
        Console.WriteLine($"Client connected: {endPoint.Address}:{endPoint.Port}");

        try
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                string request = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                Console.WriteLine($"Received request: {request}");

                string response = "Invalid request";
                if (exchangeRates.TryGetValue(request.ToUpper(), out double rate))
                {
                    response = rate.ToString();
                }

                byte[] responseData = Encoding.UTF8.GetBytes(response);
                stream.Write(responseData, 0, responseData.Length);
                Console.WriteLine($"Sent response: {response}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            client.Close();
            Console.WriteLine("Client disconnected");
        }
    }
}