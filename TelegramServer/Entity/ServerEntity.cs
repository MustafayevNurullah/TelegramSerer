using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramServer.Entity
{
   public class ServerEntity
    {
        public int Id { get; set; }
        public string SenderIp { get; set; }
        public string Message { get; set; }
        public string SentIp { get; set; }
    }
}
