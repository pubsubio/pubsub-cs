using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PubsubIO.Client.Model
{
    public class PublishedDocument<T> where T : class
    {
        public string name { get; set; }
        public string id { get; set; }
        public T doc { get; set; }
    }
}
