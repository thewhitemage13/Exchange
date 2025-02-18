using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class UDPClient
{
    static void Main()
    {
        Socket clientSocket = null;
        try
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(new IPEndPoint(IPAddress.Loopback, 5000));

            //while (true)
            //{
                Console.Write("Enter text: ");
                string message = Console.ReadLine();
                if (string.IsNullOrEmpty(message))
                {
                    Console.WriteLine("Empty message detected. Stopping communication.");
                    //break;
                }

                byte[] messageData = Encoding.UTF8.GetBytes(message);
                clientSocket.Send(messageData);
            //}

            byte[] buffer = new byte[1024];
            int receivedBytes = clientSocket.Receive(buffer);
            string response = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
            Console.WriteLine($"Server response: {response}");
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"Socket error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            if (clientSocket != null)
            {
                try
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error closing socket: {ex.Message}");
                }
            }
        }
    }
}