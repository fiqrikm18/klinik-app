using System;
using System.Net;
using System.Net.Sockets;

namespace Antrian.SckServer
{
    internal class Listener
    {
        public delegate void SocketAcceptedHandler(Socket e);

        private Socket s;

        public Listener(int port)
        {
            Port = port;
            s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public bool Listening { get; private set; }

        public int Port { get; }

        public void Start()
        {
            if (Listening)
                return;

            s.Bind(new IPEndPoint(0, Port));
            s.Listen(0);

            s.BeginAccept(callback, null);
            Listening = true;
        }

        private void callback(IAsyncResult ar)
        {
            try
            {
                var s = this.s.EndAccept(ar);

                SocketAccepted?.Invoke(s);

                this.s.BeginAccept(callback, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Stop()
        {
            if (!Listening) return;

            s.Close();
            s.Dispose();
            s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public event SocketAcceptedHandler SocketAccepted;
    }
}