using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Helper
    {
        public static DateTime ConvertToLocalDate(string timeInMilliseconds)
        {
            double timeInTicks = double.Parse(timeInMilliseconds);
            TimeSpan dateTimeSpan = TimeSpan.FromMilliseconds(timeInTicks);
            DateTime dateAfterEpoch = new DateTime(1970, 1, 1) + dateTimeSpan;
            DateTime dateInLocalTimeFormat = dateAfterEpoch.ToLocalTime();
            return dateInLocalTimeFormat;
        }

        public static ConnectionFactory CreateFactory()
        {
            return new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "root",
                Password = "password",
                VirtualHost = "/"
            };
        }
    }
}
