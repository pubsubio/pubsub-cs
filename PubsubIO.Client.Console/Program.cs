using System.Collections.Generic;

namespace PubsubIO.Client.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            PubsubIoClient client = PubsubIoClient.Create("matcctst09", 9999, "dr");
            PubsubSubscription<SomeDocument> subscription = client.Subscribe<SomeDocument>(new
                                                                                               {
                                                                                                   hello =
                                                                                               new Dictionary
                                                                                               <string, object>
                                                                                                   {
                                                                                                       {
                                                                                                           "$any",
                                                                                                           new[]
                                                                                                               {
                                                                                                                   "world",
                                                                                                                   "mundo",
                                                                                                                   "verden"
                                                                                                               }
                                                                                                           }
                                                                                                   }
                                                                                               });

            subscription.DocumentPublished +=
                (sender, document) => System.Console.WriteLine(document.PublishedDocument.hello);
            System.Console.ReadLine();
        }
    }

    public class SomeDocument
    {
        public string hello { get; set; }
    }
}