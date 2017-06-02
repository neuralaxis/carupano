using System;

namespace CarupanoAir.Notification.Commands
{
    public class SendNotification
    {
        public string ToPassengerId { get; }
        public string Template { get; }
        public object MergeData { get; }
        public SendNotification(string to, string template, object data)
        {
            ToPassengerId = to;
            Template = template;
            MergeData = data;
        }
    }
}
