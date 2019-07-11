using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TelegramServer.Entity
{
   public class ClientEntity
    {
        public int Id { get; set; }
        public TcpClient Client { get; set; }
    }
}
