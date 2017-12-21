using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ConsulTest
{
    internal class Server : IDisposable
    {
        private readonly List<string> _addresses;

        private bool _isListening = false;
        private HttpListener _listener = null;

        private byte[] _response = Encoding.UTF8.GetBytes("Hi");


        public Server(IEnumerable<string> addresses)
        {
            this._addresses = addresses.ToList();
        }



        public void Start()
        {
            this._listener = new HttpListener();
            this._addresses.ForEach(address => 
                this._listener.Prefixes.Add(address)
            );

            this._listener.Start();
            this._isListening = true;

            this._addresses.ForEach(address =>
                Console.WriteLine($"Server is listening on address: {address}")
            );

            while (this._isListening)
            {
                var context = this._listener.GetContext();
                var response = context.Response;

                response.OutputStream.Write(this._response, 0, this._response.Length);
                response.OutputStream.Close();
            }
        }

        public void Stop()
        {
            if (this._listener != null)
            {
                this._isListening = false;
                this._listener.Stop();
            }
        }



        public void Dispose()
        {
            this.Stop();
        }
    }
}
