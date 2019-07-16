using Newtonsoft.Json;
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
        public string serverName = "WMS";
        public string CommandEnd = ",END";

        public HttpControl()
        {
            wcsUrl = "http://10.9.31.101/wms.php";
        }

        #region 入库任务

        /// <summary>
        /// 进仓扫码
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public WmsModel DoBarcodeScanTask(string from,string barcode)
        {
            StringBuilder url = new StringBuilder();
            url.Append(wcsUrl + "/" + serverName + "/");
            url.Append(WmsParam.Status + "=" + WmsStatus.StockInTask+",");
            url.Append(WmsParam.From + "=" + from + ",");
            url.Append(WmsParam.Barcode + "=" + barcode + ",");
            url.Append(WmsParam.DateTime + "=" + DateTime.Now.ToString("yyyyMMddHHmmss") + ",");
            url.Append(CommandEnd);

            string result = DoPost(url.ToString());
            return JsonConvert.DeserializeObject<WmsModel>(result);
        }

        /// <summary>
        /// 进仓到达位置
        /// </summary>
        /// <returns></returns>
        public WmsModel DoReachStockinPosTask(string from,string taskuid)
        {
            StringBuilder url = new StringBuilder();
            url.Append(wcsUrl + "/" + serverName + "/");
            url.Append(WmsParam.Status + "=" + WmsStatus.SiteArrived + ",");
            url.Append(WmsParam.From + "=" + from + ",");
            url.Append(WmsParam.TaskUID + "=" + taskuid + ",");
            url.Append(WmsParam.DateTime + "=" + DateTime.Now.ToString("yyyyMMddHHmmss") + ",");
            url.Append(CommandEnd);

            string result = DoPost(url.ToString());
            return JsonConvert.DeserializeObject<WmsModel>(result);
        }


        /// <summary>
        /// 进仓完成状态
        /// </summary>
        /// <param name="from"></param>
        /// <param name="taskuid"></param>
        /// <returns></returns>
        public string DoStockInFinishTask(string from,string taskuid)
        {
            StringBuilder url = new StringBuilder();
            url.Append(wcsUrl + "/" + serverName + "/");
            url.Append(WmsParam.Status + "=" + WmsStatus.TaskFinish + ",");
            url.Append(WmsParam.From + "=" + from + ",");
            url.Append(WmsParam.TaskUID + "=" + taskuid + ",");
            url.Append(WmsParam.DateTime + "=" + DateTime.Now.ToString("yyyyMMddHHmmss") + ",");
            url.Append(CommandEnd);

            string result = DoPost(url.ToString());
            return result;
        }

        #endregion

        #region 出仓任务

        /// <summary>
        /// 出仓完成
        /// </summary>
        /// <param name="from"></param>
        /// <param name="taskuid"></param>
        /// <returns></returns>
        public string DoStockOutFinishTask(string from, string taskuid)
        {
            StringBuilder url = new StringBuilder();
            url.Append(wcsUrl + "/" + serverName + "/");
            url.Append(WmsParam.Status + "=" + WmsStatus.TaskFinish + ",");
            url.Append(WmsParam.From + "=" + from + ",");
            url.Append(WmsParam.TaskUID + "=" + taskuid + ",");
            url.Append(WmsParam.DateTime + "=" + DateTime.Now.ToString("yyyyMMddHHmmss") + ",");
            url.Append(CommandEnd);

            string result = DoPost(url.ToString());
            return result;
        }

        /// <summary>
        /// 出仓异常状态/暂停
        /// </summary>
        /// <param name="from"></param>
        /// <param name="taskuid"></param>
        /// <returns></returns>
        public WmsModel DoStockOutErrorTask(string from, string taskuid)
        {
            StringBuilder url = new StringBuilder();
            url.Append(wcsUrl + "/" + serverName + "/");
            url.Append(WmsParam.Status + "=" + WmsStatus.TaskSuspend + ",");
            url.Append(WmsParam.From + "=" + from + ",");
            url.Append(WmsParam.TaskUID + "=" + taskuid + ",");
            url.Append(WmsParam.DateTime + "=" + DateTime.Now.ToString("yyyyMMddHHmmss") + ",");
            url.Append(CommandEnd);

            string result = DoPost(url.ToString());
            return JsonConvert.DeserializeObject<WmsModel>(result);
        }

        #endregion

        #region 移仓任务

        /// <summary>
        /// 移仓完成
        /// </summary>
        /// <param name="from"></param>
        /// <param name="taskuid"></param>
        /// <returns></returns>
        public string DoStockMoveFinishTask(string from, string taskuid)
        {
            StringBuilder url = new StringBuilder();
            url.Append(wcsUrl + "/" + serverName + "/");
            url.Append(WmsParam.Status + "=" + WmsStatus.TaskFinish + ",");
            url.Append(WmsParam.From + "=" + from + ",");
            url.Append(WmsParam.TaskUID + "=" + taskuid + ",");
            url.Append(WmsParam.DateTime + "=" + DateTime.Now.ToString("yyyyMMddHHmmss") + ",");
            url.Append(CommandEnd);

            string result = DoPost(url.ToString());
            return result;
        }

        #endregion

        #region 盘点任务

        /// <summary>
        /// 请求对应位置的盘点任务
        /// </summary>
        /// <param name="from"></param>
        /// <param name="taskuid"></param>
        /// <returns></returns>
        public string DoRequestStockCheckTask(string from)
        {
            StringBuilder url = new StringBuilder();
            url.Append(wcsUrl + "/" + serverName + "/");
            url.Append(WmsParam.Status + "=" + WmsStatus.StockCheckTask + ",");
            url.Append(WmsParam.From + "=" + from + ",");
            url.Append(WmsParam.DateTime + "=" + DateTime.Now.ToString("yyyyMMddHHmmss") + ",");
            url.Append(CommandEnd);

            string result = DoPost(url.ToString());
            return result;
        }

        /// <summary>
        /// 盘点任务完成
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public string DoStockCheckFinishTask(string from,string taskuid)
        {
            StringBuilder url = new StringBuilder();
            url.Append(wcsUrl + "/" + serverName + "/");
            url.Append(WmsParam.Status + "=" + WmsStatus.TaskFinish + ",");
            url.Append(WmsParam.From + "=" + from + ",");
            url.Append(WmsParam.TaskUID + "=" + taskuid + ",");
            url.Append(WmsParam.DateTime + "=" + DateTime.Now.ToString("yyyyMMddHHmmss") + ",");
            url.Append(CommandEnd);

            string result = DoPost(url.ToString());
            return result;
        }


        /// <summary>
        /// 盘点任务完成
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public string DoStockCheckErrorTask(string from)
        {
            StringBuilder url = new StringBuilder();
            url.Append(wcsUrl + "/" + serverName + "/");
            url.Append(WmsParam.Status + "=" + WmsStatus.TaskFinish + ",");
            url.Append(WmsParam.From + "=" + from + ",");
            url.Append(WmsParam.DateTime + "=" + DateTime.Now.ToString("yyyyMMddHHmmss") + ",");
            url.Append(CommandEnd);

            string result = DoPost(url.ToString());
            return result;
        }

        #endregion

        #region 网络请求逻辑

        /// <summary>
        /// 网络请求并返回结果
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


        #endregion

    }
}
