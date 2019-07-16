using MHttpServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace WCS_phase1.Http
{
    /// <summary>
    /// 提供WMS访问的服务类
    /// </summary>
    public class HttpServerControl
    {
        /// <summary>
        /// 服务监听端口
        /// </summary>
        private int listenerPort = 8080;
        private bool isStart = false;
        

        public void StartServer()
        {
            try
            {
                if (!isStart)
                {
                    HttpServer httpServer = new HttpServer(listenerPort, Routes.GET);

                    Thread thread = new Thread(new ThreadStart(httpServer.Listen));
                    thread.Start();
                    isStart = true;
                }
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}
