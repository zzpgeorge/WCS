// Copyright (C) 2016 by Barend Erasmus and donated to the public domain


using MHttpServer;
using MHttpServer.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace WCS_phase1.Http
{
    static class Routes
    {

        public static List<Route> GET
        {
            get
            {
                return new List<Route>()
                {
                    /*
                     * urlRegex 正则表达式
                     * 后面带问号+加任何字符  \?(.*)
                     */

                    //空链接.帮助界面
                    new Route()
                    {
                        Callable = HomeIndex,
                        UrlRegex = "^\\/$",
                        Method = "GET"
                    },
                    //处理：WMS向WCS发送的出库任务
                    new Route()
                    {
                        Callable = StockOutHandle,
                        UrlRegex = "^\\/StockOutHandle$",
                        Method = "GET"
                    }
                    //处理：WMS向WCS发送的移库任务
                    ,new Route()
                    {
                        Callable = StockMoveHandle,
                        UrlRegex = "^\\/StockMoveHandle$",
                        Method = "GET"
                    }
                    //处理：WMS向WCS发送的盘点任务
                    ,new Route()
                    {
                        Callable = StockCheckHandle,
                        UrlRegex = "^\\/StockCheckHandle$",
                        Method = "GET"
                    }
                };

            }
        }

        /// <summary>
        /// 出库任务
        /// 处理：WMS向WCS发送的出库任务
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static HttpResponse StockOutHandle(HttpRequest request)
        {
            if (request.Content != null)
            {
                WmsModel model = JsonConvert.DeserializeObject<WmsModel>(request.Content);
                HttpResponse response = CheckWmsModel(model,WmsStatus.StockOutTask,true);
                if (response != null) return response;
                return OkResponse("StockOutHandle");
            }
            else
            {
                return EmptyMssage();
            }
        }

        /// <summary>
        /// 移库任务
        /// 处理：WMS向WCS发送的移库任务
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static HttpResponse StockMoveHandle(HttpRequest request)
        {
            if (request.Content != null)
            {
                WmsModel model = JsonConvert.DeserializeObject<WmsModel>(request.Content);
                HttpResponse response = CheckWmsModel(model,WmsStatus.StockMoveTask,true);
                if (response != null) return response;
                return OkResponse("StockMoveHandle");
            }
            else
            {
                return EmptyMssage();
            }
        }

        /// <summary>
        /// 盘点任务
        /// 处理：WMS向WCS发送的盘点任务
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static HttpResponse StockCheckHandle(HttpRequest request)
        {
            if (request.Content != null)
            {
                WmsModel model = JsonConvert.DeserializeObject<WmsModel>(request.Content);
                HttpResponse response = CheckWmsModel(model,WmsStatus.StockCheckTask,false);
                if (response != null) return response;
                return OkResponse("StockCheckHandle");
            }
            else
            {
                return EmptyMssage();
            }
        }

        /// <summary>
        /// 检查Wms请求过来的信息是否完整
        /// </summary>
        /// <param name="model"></param>
        /// <param name="tasktype"></param>
        /// <param name="checkWdloc"></param>
        /// <returns></returns>
        private static HttpResponse CheckWmsModel(WmsModel model,WmsStatus tasktype,bool checkWdloc)
        {
            string msg = "";

            //是否检查目标货位是否为空
            if (checkWdloc)
            {
                if (model.W_D_Loc == null || model.W_D_Loc.Length == 0)
                {
                    msg = "W_D_Loc can't be empty";
                }
            }

            //任务ID不能为空
            if (model.Task_UID == null || model.Task_UID.Length == 0)
            {
                msg = "Task_UID can't be empty";
            }
            //任务类型不能为空
            else if(model.Task_type == WmsStatus.Empty)
            {
                msg = "Task_type can't be empty";
            }
            //货位条形码不能为空
            else if(model.Barcode == null || model.Barcode.Length == 0)
            {
                msg = "Barcode can't be empty";
            }
            //源货位不能为空
            else if(model.W_S_Loc == null || model.W_S_Loc.Length == 0)
            {
                msg = "W_S_Loc can't be empty";
            }

            if (msg.Length != 0)
            {
                return new HttpResponse()
                {
                    ContentAsUTF8 = msg,
                    ReasonPhrase = "OK",
                    StatusCode = "200"
                };
            }

            return null;
        }

        private static HttpResponse EmptyMssage()
        {
            return new HttpResponse()
            {
                ContentAsUTF8 = "can't get any msg",
                ReasonPhrase = "OK",
                StatusCode = "200"
            };
        }

        private static HttpResponse OkResponse(string msg = "OK")
        {
            return new HttpResponse()
            {
                ContentAsUTF8 = msg,
                ReasonPhrase = "OK",
                StatusCode = "200"
            };
        }

        private static HttpResponse HomeIndex(HttpRequest request)
        {
            return HttpBuilder.NotFound();
            //string value = "";
            //if (request.Headers.ContainsKey("WMS_DATA"))
            //{
            //    value = request.Headers["WMS_DATA"];
            //}

            //return new HttpResponse()
            //{
            //    ContentAsUTF8 = "{\"data\":\""+ value
            //        +"\",\"name\":\"kyle\",\"age\":18,\"friend\":[{\"name\":\"matt\",\"sex\":\"man\"},{\"name\":\"butt\",\"sex\":\"man\"}]}",
            //    ReasonPhrase = "OK",
            //    StatusCode = "200"
            //};
        }
    }
}
