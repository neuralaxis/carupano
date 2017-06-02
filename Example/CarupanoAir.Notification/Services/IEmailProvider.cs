using System;
using System.Collections.Generic;
using System.Text;

namespace CarupanoAir.Notification.Services
{
    public interface IEmailProvider
    {
        void Send(string template, string email, object data);
    }
}
