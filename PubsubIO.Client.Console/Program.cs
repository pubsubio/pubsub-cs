using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PubsubIO.Client;

namespace PubsubIO.Client.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = PubsubIoClient.Create("matcctst09", 9999, "dr");
            var subscription = client.Subscribe<SomeDocument>(new
                                                    {
                                                        hello = new Dictionary<string, object>()
                                                        {
                                                            {"$any", new []{"world", "mundo", "verden"}}
                                                        }
                                                    });

            subscription.DocumentPublished += (sender, document) => System.Console.WriteLine(document.PublishedDocument.hello);
            System.Console.ReadLine();
        }
    }

    public class SomeDocument
    {
        public string hello { get; set; }
    }
}
