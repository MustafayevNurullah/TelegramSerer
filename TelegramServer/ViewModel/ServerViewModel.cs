using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
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
        static string IpAddress = "10.1.16.13";
        static int TcpPort = 1032;
        public ServerViewModel()
        {
            ServerList = new ObservableCollection<ServerEntity>();
            Task task = Task.Run(() => AcceptClient());
            List = new List<ClientEntity>();
        }
        TcpListener server = null;
        int counter = 1;
        NetworkStream stream;

        public string GetImagePath(byte[] buffer, int counter)
        {
            ImageConverter ic = new ImageConverter();
            Image img = (Image)ic.ConvertFrom(buffer);
            Bitmap bitmap1 = new Bitmap(img);
            Random random = new Random();
            counter += random.Next(1, 100);
            bitmap1.Save($@"C:\Users\User\Desktop\image{counter}.png");
            var imagepath = $@"C:\Users\User\Desktop\image{counter}.png";
            return imagepath;
        }
        void StartTcpServer()
        {
            try
            {
                while (true)
                {

                    string data = null;
                    int j = 0;
                    while (true)
                    {
                          byte[] bytes = new byte[11000000];
                            int i;
                            stream = List[j].Client.GetStream();
                            if (stream.DataAvailable)
                            {
                                i = stream.Read(bytes, 0, bytes.Length);
                                data = Encoding.ASCII.GetString(bytes, 0, i);
                                if (data.Contains("PNG"))
                                {
                                    SentClient(bytes, List[j].Id);
                                    ServerEntity serverEntity = new ServerEntity();
                                    serverEntity.Message = data;
                                    var action = new Action(() => { ServerList.Add(serverEntity); });
                                    Task.Run(() => App.Current.Dispatcher.BeginInvoke(action)).Wait();
                                    data = null;
                                    bytes = null;
                                }
                                else
                                {
                                    ServerEntity serverEntity = new ServerEntity();
                                    string[] arr = data.Split('`');
                                    serverEntity.Message = arr[0];
                                    var action = new Action(() => { ServerList.Add(serverEntity); });
                                    Task.Run(() => App.Current.Dispatcher.BeginInvoke(action)).Wait();
                                    byte[] msg = Encoding.ASCII.GetBytes(arr[0]);
                                    SentClient(msg, List[j].Id);
                                }
                            }
                        j++;
                        if (j == List.Count)
                        {
                            j = 0;
                        }

                    }
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                MessageBox.Show("SocketException: {0}", e.Message);
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception" + e.Message);
            }
            finally
            {
                server.Stop();
            }
        }
        TcpClient client = new TcpClient();
        void AcceptClient()
        {
            IPAddress localAddr = IPAddress.Parse(IpAddress);
            server = new TcpListener(localAddr, TcpPort);
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
                Thread thread = new Thread(StartTcpServer);
                thread.Start();
            }
        }
        void SentClient(byte[] data1, int Id)
        {
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
