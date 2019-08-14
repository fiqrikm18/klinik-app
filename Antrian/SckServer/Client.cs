using System;
using System.Net;
using System.Net.Sockets;

namespace Antrian.SckServer
{
    public class Client
    {
        public delegate void ClientDisconnectedHandler(Client sender);

        public delegate void ClientReceivedHandler(Client sender, byte[] data);

        private readonly Socket sck;

        public Client(Socket accepted)
        {
            sck = accepted;
            ID = Guid.NewGuid().ToString();
            EndPoint = (IPEndPoint) sck.RemoteEndPoint;
            sck.BeginReceive(new byte[] {0}, 0, 0, 0, callback, null);
        }

        public string ID { get; }

        public IPEndPoint EndPoint { get; }

        private void callback(IAsyncResult ar)
        {
            try
            {
                sck.EndReceive(ar);
                var buf = new byte[8192];
                var rec = sck.Receive(buf, buf.Length, 0);

                if (rec < buf.Length) Array.Resize(ref buf, rec);

                Received?.Invoke(this, buf);

                sck.BeginReceive(new byte[] {0}, 0, 0, 0, callback, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Close();

                Disconnected?.Invoke(this);
            }
        }

        public void Close()
        {
            sck.Close();
            sck.Dispose();
        }

        public event ClientReceivedHandler Received;
        public event ClientDisconnectedHandler Disconnected;
    }
}