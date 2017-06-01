using System;
using System.Collections.Generic;

namespace EreborPhoenixExtension.Libs.Abilites
{
    public class QueueEx
    {
        public event EventHandler<EventArgs> added;
        public Queue<string> que { get; set; }
        public QueueEx()
        {
            que = new Queue<string>();
        }
        public void Enque(string s)
        {
            que.Enqueue(s);
            added?.Invoke(this, new EventArgs());
        }
        public string[] Deque()
        {
            return que.Dequeue().Split(';');
        }
    }
}
