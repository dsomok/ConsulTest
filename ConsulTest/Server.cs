using System;
using System.Net;
using System.Text;

namespace ConsulTest
{
    internal class Server : IDisposable
    {
        private readonly Uri _address;

        private bool _isListening = false;
        private HttpListener _listener = null;

        private byte[] _response = Encoding.UTF8.GetBytes("Hi");


        public Server(Uri address)
        {
            this._address = address;
        }



        public void Start()
        {
            this._listener = new HttpListener();
            this._listener.Prefixes.Add(this._address.ToString());

            this._listener.Start();
            this._isListening = true;

            Console.WriteLine($"Server is listening on address: {this._address}");

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
