using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCS_phase1.Http;
using WCS_phase1.Socket;

namespace WCS_phase1.Action
{
    class DataControl
    {
        /// <summary>
        /// 获取设备通信信息
        /// </summary>
        internal static SocketControl _mSocket;

        /// <summary>
        /// 控制提供给WMS的服务
        /// </summary>
        internal static HttpServerControl _mHttpServer;

        /// <summary>
        /// 请求WMS
        /// </summary>
        internal static HttpControl _mHttp;

        private static bool init = false;//是否已经初始化

        public DataControl()
        {
            if (!init)
            {
                _mSocket = new SocketControl();

                _mHttpServer = new HttpServerControl();

                _mHttp = new HttpControl();

                init = true;
            }
        }
    }
}
