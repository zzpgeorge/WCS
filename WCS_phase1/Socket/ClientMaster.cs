using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace AsyncTcp
{
    /// <summary>
    /// 设备管理器
    /// 1.通过该类添加连接设备
    /// 2.通过该类获取设备信息
    /// </summary>
    public class ClinetMaster
    {
        private readonly static object _uobj = new object();
        /// <summary>
        /// 设备列表
        /// </summary>
        private static readonly List<Client> clinets = new List<Client>();

        /// <summary>
        /// 设备信息列表
        /// </summary>
        private static readonly List<Device> devices = new List<Device>();
        private static readonly List<Device> _devices = new List<Device>();

        /// <summary>
        /// 添加联网设备
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns>bool</returns>
        public static bool AddClient(string name, string ip, int port)
        {
            if (clinets.Find(c => { return c.Name.Equals(name); }) == null)
            {
                clinets.Add(new Client(ip, port, name));
                return true;
            }
            return false;
        }

        /// <summary>
        /// 向指定设备发送信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="order"></param>
        public static bool SendToClient(string name, string order)
        {
            Client clinet = clinets.Find(c => { return name.Equals(c.Name); });
            if (clinet != null)
            {
                clinet.Send(order);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新设备信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        internal static void UpdateDevceData(string name, byte[] data)
        {

            Device device = devices.Find(c => { return name.Equals(c.Name); });
            if (device != null)
            {
                device.Bdata = data;
            }
            else
            {
                devices.Add(new Device(name, data));
            }
            lock (_uobj)
            {
                _devices.Clear();
                _devices.AddRange(devices);
            }
        }

        /// <summary>
        /// 获取设备信息列表
        /// </summary>
        /// <returns></returns>
        public static List<Device> GetDeviceList()
        {
            lock (_uobj)
            {
                return _devices;
            }
        }

        /// <summary>
        /// 根据设备名称获取信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Device GetDeviceData(string name)
        {
            return devices.Find(c => { return name.Equals(c.Name); });
        }
    }

    /// <summary>
    /// 设备信息
    /// </summary>
    public class Device
    {
        internal string Name;
        internal byte[] Bdata;

        public Device(string name, byte[] data)
        {
            this.Name = name;
            this.Bdata = data;
        }
    }

    /// <summary>
    /// 设备
    /// </summary>
    class Client
    {
        /// <summary>
        /// 服务端IP
        /// </summary>
        private readonly string IP;
        /// <summary>
        /// 服务端端口
        /// </summary>
        private readonly int Port;

        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name;

        /// <summary>
        /// TCP连接
        /// </summary>
        AsyncTcpClient client;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="name"></param>
        public Client(string ip, int port, string name)
        {
            IP = ip;
            Port = port;
            Name = name;
            ConnectToService();
        }

        /// <summary>
        /// 连接到服务端
        /// </summary>
        public void ConnectToService()
        {
            client = new AsyncTcpClient(IPAddress.Parse(IP), Port);
            client.ServerConnected += Client_ServerConnected;//连接成功
            //client.PlaintextReceived += Client_PlaintextReceived; 文本接收
            client.DatagramReceived += Client_DatagramReceived; //字节接收
            client.ServerDisconnected += Client_ServerDisconnected;//断开连接
            client.ServerExceptionOccurred += Client_ServerExceptionOccurred;//
            client.Connect();
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="text"></param>
        internal void Send(string text)
        {
            client.Send(text);
        }

        /// <summary>
        /// 连接异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_ServerExceptionOccurred(object sender, TcpServerExceptionOccurredEventArgs e)
        {
            //throw new NotImplementedException(); 
            Console.WriteLine("Client_ServerExceptionOccurred");

        }

        /// <summary>
        /// 服务断开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_ServerDisconnected(object sender, TcpServerDisconnectedEventArgs e)
        {
            //throw new NotImplementedException();
            Console.WriteLine("服务器断开");
            ConnectToService();
        }

        /// <summary>
        /// 接收字节数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_DatagramReceived(object sender, TcpDatagramReceivedEventArgs<byte[]> e)
        {
            //throw new NotImplementedException();
            Console.WriteLine(System.Text.Encoding.ASCII.GetString(e.Datagram));
            ClinetMaster.UpdateDevceData(Name, e.Datagram);
        }

        /// <summary>
        /// 接收文本数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_PlaintextReceived(object sender, TcpDatagramReceivedEventArgs<string> e)
        {
            // throw new NotImplementedException();
            Console.WriteLine(e.Datagram);
        }

        /// <summary>
        /// 服务连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_ServerConnected(object sender, TcpServerConnectedEventArgs e)
        {
            //throw new NotImplementedException();
            Console.WriteLine("服务器已连接");
        }
    }
}
