using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TelegramServer.Entity;

namespace TelegramServer.ViewModel
{
    public class ServerViewModel : BaseViewModel
    {
        static string IpAddress = "10.1.16.27";
        static int TcpPort = 1032;
        // static string IP = "10.1.16.16";
        NetworkStream NetworkStream;

        public ServerViewModel()
        {
            ServerList = new ObservableCollection<ServerEntity>();
            Task task = Task.Run(() => AcceptClient());
            List = new List<ClientEntity>();

        }
        TcpListener server = null;
        int counter = 1;
        NetworkStream stream;
        void Recur()
        {

            StartTcpServer();
        }
        void StartTcpServer()
        {
            try
            {

                string data = null;
                byte[] bytes = new byte[256];
                int j = 0;
                while (true)
                {
                    int i;
                    stream = List[j].Client.GetStream();

                    if ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            data = Encoding.ASCII.GetString(bytes, 0, i);
                            ServerEntity serverEntity = new ServerEntity();
                            string[] arr = data.Split('`');
                            serverEntity.Message = arr[0];
                            // serverEntity.SenderIp = arr[1];
                            var action = new Action(() => { ServerList.Add(serverEntity); });
                            Task.Run(() => App.Current.Dispatcher.BeginInvoke(action)).Wait();
                            byte[] msg = Encoding.ASCII.GetBytes(data);
                           // SentClient(msg, List[j].Id);
                        }
                    j++;
                    if(j==List.Count)
                    {
                        j = 0;
                    }

                }
                client.Close();
            }
            catch (SocketException e)
            {
                MessageBox.Show("SocketException: {0}", e.Message);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }
        }
        TcpClient client = new TcpClient();
        void AcceptClient()
        {
            bool a = true;
            IPAddress localAddr = IPAddress.Parse(IpAddress);
            //    TcpListener server = new TcpListener(port);
            server = new TcpListener(localAddr, TcpPort);
            // Start listening for client requests.
            server.Start();

            while (true)
            {
                client = server.AcceptTcpClient();
                stream = client.GetStream();
                ClientEntity clients = new ClientEntity();
                clients.Client = client;
                clients.Id = counter;
                counter++;
                List.Add(clients);
                Thread thread = new Thread(Recur);
                thread.Start();
            }
        }
        void SentClient(byte[] data1, int Id)
        {
            // IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(IpAddress), TcpPort);
            // client.Connect(endPoint);
            foreach (var item in List)
            {
                if (item.Id != Id)
                {
                    stream = item.Client.GetStream();
                    stream.Write(data1, 0, data1.Length);
                }
            }

        }
        ObservableCollection<ServerEntity> serverlist;
        public ObservableCollection<ServerEntity> ServerList
        {
            get
            {
                return serverlist;
            }
            set
            {
                serverlist = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentServer"));
            }
        }
        List<ClientEntity> List;
    }
}
