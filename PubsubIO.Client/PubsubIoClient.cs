using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Cache;
using System.Threading;
using System.Web.Script.Serialization;
using WebSocketSharp;

namespace PubsubIO.Client
{
    public class PubsubIoClient : IDisposable
    {
        protected static string PublishUrl = "{0}/{1}/publish"; //0 = server, 1 = hub

        public string Server { get; private set; }
        public int Port { get; private set; }
        public string Subhub { get; private set; }
        public string SessionID { get; private set; }

        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();
        private WebSocket _socket;

        private readonly ManualResetEvent _hubSubscribeLock = new ManualResetEvent(false);

        protected PubsubIoClient(string server, int port, string subhub)
        {
            //Set caching policy. We don't want ANY client side caching to occur in this application.
            WebRequest.DefaultCachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

            Server = server;
            Port = port;
            Subhub = subhub;

            Connect();
        }

        private void Connect()
        {
            _socket = new WebSocket("ws://" + Server + ":" + Port + "/json-sockets", "sample");
            _socket.OnOpen += (sender, args) =>
                                  {
                                      var hubSubscribe = _serializer.Serialize(new {sub = Subhub});
                                      _socket.Send(hubSubscribe);

                                      _hubSubscribeLock.Set(); //Signal that we're ready to process stuff
                                  };

            _socket.OnClose += (sender, args) =>
                                   {
                                       Console.WriteLine("Closed");
                                   };

            _socket.Connect();
        }

        public PubsubSubscription<T> Subscribe<T>(object query) where T : class
        {
            _hubSubscribeLock.WaitOne(1000);
            return new PubsubSubscription<T>(_socket, query);
        }
        
        /// <summary>
        /// Creates a new persistent connection that can be used to subscribe.
        /// </summary>
        /// <param name="server">Server to connect to</param>
        /// /// <param name="port">Port number on pubsub io server</param>
        /// <param name="subhub">Hub to bind with</param>
        /// <returns></returns>
        public static PubsubIoClient Create(string server, int port, string subhub)
        {
            return new PubsubIoClient(server, port, subhub);
        }

        /// <summary>
        /// Publishes a document to a pubsub.io hub
        /// </summary>
        /// <param name="server">Server to publish to</param>
        /// <param name="hub">Hub to publish to</param>
        /// <param name="document">Document to publish</param>
        /// <returns>Pubsub.IO result acknowledgement ("ok" or "ack")</returns>
        public static string Publish(string server, string hub, object document)
        {
            var url = string.Format(PublishUrl, server, hub);
            return HttpUtil.PostObjectAsJson(url, new { doc = document });
        }

        public void Dispose()
        {
            if(_socket != null)
                _socket.Close();
        }
    }
}