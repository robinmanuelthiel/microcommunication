using System;

namespace MicroCommunication.Random.Models
{
    public class ChatMessage
    {
        public string Text { get; private set; }
        public string Sender { get; private set; }
        public string Server { get; private set; }
        public DateTime Date { get; private set; }

        public ChatMessage(string text, string sender, string server)
        {
            Text = text;
            Sender = sender;
            Server = server;
            Date = DateTime.Now;
        }
    }
}
