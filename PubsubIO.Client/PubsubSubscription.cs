using System;
using System.Web.Script.Serialization;
using PubsubIO.Client.Model;
using WebSocketSharp;

namespace PubsubIO.Client
{
    public class PubsubSubscription<T> where T : class
    {
        private static int _runningCounter;
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();

        public PubsubSubscription(WebSocket webSocket, object query)
        {
            SubscriptionID = "s" + _runningCounter++;

            webSocket.OnMessage += (sender, data) =>
                                       {
                                           var document = typeof (T) == typeof (object)
                                                                  ? DynamicJson.Parse(data)
                                                                  : _serializer.Deserialize<PublishedDocument<T>>(data);

                                           if (document.name == "publish" && document.id == SubscriptionID)
                                           {
                                               OnDocumentPublished(document.doc);
                                           }
                                       };

            var message = _serializer.Serialize(new {name = "subscribe", query, id = SubscriptionID});
            webSocket.Send(message);
        }

        public string SubscriptionID { get; private set; }

        public event EventHandler<SubscriptionEventArgs<T>> DocumentPublished;

        protected void OnDocumentPublished(T publishedDocument)
        {
            if (DocumentPublished != null)
            {
                DocumentPublished(this, new SubscriptionEventArgs<T>(publishedDocument));
            }
        }
    }

    public class SubscriptionEventArgs<T> : EventArgs
    {
        public SubscriptionEventArgs(T publishedDocument)
        {
            PublishedDocument = publishedDocument;
        }

        public T PublishedDocument { get; private set; }
    }
}