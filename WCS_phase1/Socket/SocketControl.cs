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

        private readonly object _uobj = new object();
        /// <summary>
        /// 设备列表
        /// </summary>
        private readonly List<SocketClient> clinets = new List<SocketClient>();

        /// <summary>
        /// 构造函数
        /// TODO 可以加载读取数据库设备信息
        /// </summary>
        public SocketControl()
        {
            //测试
            AddClient("AGV01", "127.0.0.1", 2000);

        }

        public bool IsAlive(string name)
        {
            SocketClient client = clinets.Find(c => { return c.Name.Equals(name); });
            if (client != null)
            {
                return client.IsAlive;
            }

            return false;
        }

        public byte[] GetByteArr(string name)
        {
            SocketClient client = clinets.Find(c => { return c.Name.Equals(name); });
            if (client != null)
            {
                return client.Bdata;
            }

            return new byte[0];
        }


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
                clinets.Add(new SocketClient(ip, port, name));
                return true;
            }
            return false;
        }

        public void CloseClient(string name)
        {
            SocketClient client = clinets.Find(c => { return c.Name.Equals(name); });
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
            SocketClient clinet = clinets.Find(c => { return name.Equals(c.Name); });
            if (clinet != null && clinet.IsConnect())
            {
                byte[] b = new byte[msg.Length + 2];
                msg.CopyTo(b, 0);
                CRCMethod.ToModbusCRC16Byte(msg).CopyTo(b, msg.Length);
                clinet.Send(b);
                return true;
            }
            return false;
        }
    }


}
