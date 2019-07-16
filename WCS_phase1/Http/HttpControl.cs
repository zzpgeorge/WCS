using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WCS_phase1.Http
{
    /// <summary>
    /// 处理请求WMS和返回的结果
    /// </summary>
    class HttpControl
    {
        public string wcsUrl = "";

        public HttpControl()
        {
            wcsUrl = "http://10.9.31.101/wms.php";
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public string DoPost(string url)
        {

            //定义request并设置request的路径
            WebRequest request = WebRequest.Create(url);

            //定义请求的方式
            request.Method = "POST";


            //定义response为前面的request响应
            WebResponse response = request.GetResponse();

            //获取相应的状态代码
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            //定义response字符流
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();//读取所有
            Console.WriteLine(responseFromServer);

            //关闭资源
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }


    }
}
