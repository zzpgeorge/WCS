using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AsyncTcp;

namespace WCS_phase1.Socket
{
    /// <summary>
    /// 设备管理器
    /// 1.通过该类添加连接设备
    /// 2.通过该类获取设备信息
    /// </summary>
    public class SocketControl
    {
        private readonly static object _uobj = new object();
        /// <summary>
        /// 设备列表
        /// </summary>
        private readonly List<Client> clinets = new List<Client>();

        /// <summary>
        /// 设备信息列表
        /// </summary>
        private readonly List<Device> devices = new List<Device>();
        private readonly List<Device> _devices = new List<Device>();


        /// <summary>
        /// 添加联网设备
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns>bool</returns>
        public bool AddClient(string name, string ip, int port)
        {
            if (clinets.Find(c => { return c.Name.Equals(name); }) == null)
            {
                clinets.Add(new Client(this,ip, port, name));
                return true;
            }
            return false;
        }

        public void CloseClient(string name)
        {
            Client client = clinets.Find(c => { return c.Name.Equals(name); });
            if (client != null)
            {
                client.Close();
                clinets.Remove(client);
            }
        }

        /// <summary>
        /// 向指定设备发送信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="order"></param>
        public bool SendToClient(string name, string order)
        {
            return SendToClient(name, CRCMethod.StringToHexByte(order));
        }        
        
        /// <summary>
        /// 向指定设备发送信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="order"></param>
        public bool SendToClient(string name, byte[] msg)
        {
            Client clinet = clinets.Find(c => { return name.Equals(c.Name); });
            if (clinet != null && clinet.IsConnect())
            {
                byte[] b = new byte[msg.Length + 2];
                msg.CopyTo(b, 0);
                CRCMethod.ToModbusCRC16Byte(msg).CopyTo(b, msg.Length);
                clinet.Send(b);

                //ThreadPool.QueueUserWorkItem(delegate
                //{
                //    System.Threading.SynchronizationContext.SetSynchronizationContext(new
                //      System.Windows.Threading.DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));
                //    System.Threading.SynchronizationContext.Current.Post(pl =>
                //    {
                //        //里面写真正的业务内容
                //        mainWindow.addReciveMsg(name+" S",CRCMethod.AllByteToString(b));

                //    }, null);
                //});

                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新设备信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        internal void UpdateDevceBData(string name, byte[] data)
        {

            Device device = devices.Find(c => { return name.Equals(c.Name); });
            if (device != null)
            {
                device.Bdata = data;
                device.UpDateTime = DateTime.Now;
            }
            else
            {
                devices.Add(new Device(name, data,DateTime.Now));
            }
            lock (_uobj)
            {
                _devices.Clear();
                _devices.AddRange(devices);
            }
            //ThreadPool.QueueUserWorkItem(delegate
            //{
            //    System.Threading.SynchronizationContext.SetSynchronizationContext(new
            //      System.Windows.Threading.DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));
            //    System.Threading.SynchronizationContext.Current.Post(pl =>
            //    {
            //        //里面写真正的业务内容
            //        mainWindow.addReciveMsg(name+" R" , CRCMethod.AllByteToString(data));

            //    }, null);
            //});
            
        }       
        
        /// <summary>
        /// 更新设备信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        internal void UpdateDevceSData(string name, string data)
        {

            Device device = devices.Find(c => { return name.Equals(c.Name); });
            if (device != null)
            {
                device.Sdata = data;
                device.UpDateTime = DateTime.Now;
            }
            else
            {
                devices.Add(new Device(name, data,DateTime.Now));
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
        public List<Device> GetDeviceList()
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
        public Device GetDeviceData(string name)
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
        internal byte[] Bdata;//字节数据
        internal string Sdata;//字符串数据
        internal DateTime UpDateTime;

        internal string GetHexString()
        {
            if(Bdata==null | Bdata.Count() == 0)
            {
                return "";
            }
            else
            {
                return CRCMethod.ByteToString(Bdata);
            }
        }

        public Device(string name, string sdata,DateTime datetime)
        {
            this.Name = name;
            this.Sdata = sdata;
            this.UpDateTime = datetime;
        }

        public Device(string name, byte[] data,DateTime datetime)
        {
            this.Name = name;
            this.Bdata = data;
            this.UpDateTime = datetime;
        }
    }

    /// <summary>
    /// 设备
    /// </summary>
    class Client
    {
        private SocketControl _master;

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


        private bool doCloseSocket = false;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="name"></param>
        public Client(SocketControl m,string ip, int port, string name)
        {
            _master = m;

            IP = ip;
            Port = port;
            Name = name;
            ConnectToService();
        }

        /// <summary>
        /// 是否连接服务
        /// </summary>
        /// <returns></returns>
        public bool IsConnect()
        {
            if(client == null || !client.Connected)
            {
                return false;
            }

            return true;

        }

        public void Close()
        {
            if (client != null)
            {
                doCloseSocket = true;
                client.Close();
            }
        }


        /// <summary>
        /// 连接到服务端
        /// </summary>
        public void ConnectToService()
        {
            client = new AsyncTcpClient(IPAddress.Parse(IP), Port);
            client.ServerConnected += Client_ServerConnected;//连接成功
            client.PlaintextReceived += Client_PlaintextReceived; //文本接收
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
        /// 发送数据
        /// </summary>
        /// <param name="text"></param>
        internal void Send(byte[] msg)
        {
            client.Send(msg);
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
            Console.WriteLine(Name+"：服务器断开");
            if (!doCloseSocket) ConnectToService();
        }

        /// <summary>
        /// 接收字节数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_DatagramReceived(object sender, TcpDatagramReceivedEventArgs<byte[]> e)
        {
            //throw new NotImplementedException();
            //Console.WriteLine(System.Text.Encoding.ASCII.GetString(e.Datagram));
            _master.UpdateDevceBData(Name, e.Datagram);
        }

        /// <summary>
        /// 接收文本数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_PlaintextReceived(object sender, TcpDatagramReceivedEventArgs<string> e)
        {
            // throw new NotImplementedException();
            //Console.WriteLine(e.Datagram);
            _master.UpdateDevceSData(Name, e.Datagram);
        }

        /// <summary>
        /// 服务连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_ServerConnected(object sender, TcpServerConnectedEventArgs e)
        {
            //throw new NotImplementedException();
            Console.WriteLine(Name+"：服务器已连接");
        }
    }
}
