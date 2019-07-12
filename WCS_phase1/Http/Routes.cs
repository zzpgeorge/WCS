// Copyright (C) 2016 by Barend Erasmus and donated to the public domain


using MHttpServer.Models;
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
                        UrlRegex = "^\\/StockOutHandle\\?(.*)$",
                        Method = "GET"
                    }
                    //处理：WMS向WCS发送的移库任务
                    ,new Route()
                    {
                        Callable = StockMoveHandle,
                        UrlRegex = "^\\/StockMoveHandle\\?(.*)$",
                        Method = "GET"
                    }
                    //处理：WMS向WCS发送的盘点任务
                    ,new Route()
                    {
                        Callable = StockCheckHandle,
                        UrlRegex = "^\\/StockCheckHandle\\?(.*)$",
                        Method = "GET"
                    },
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
            return new HttpResponse()
            {
                ContentAsUTF8 = "Hello",
                ReasonPhrase = "OK",
                StatusCode = "200"
            };
        }

        /// <summary>
        /// 移库任务
        /// 处理：WMS向WCS发送的移库任务
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static HttpResponse StockMoveHandle(HttpRequest request)
        {
            return new HttpResponse()
            {
                ContentAsUTF8 = "Hello",
                ReasonPhrase = "OK",
                StatusCode = "200"
            };
        }

        /// <summary>
        /// 盘点任务
        /// 处理：WMS向WCS发送的盘点任务
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static HttpResponse StockCheckHandle(HttpRequest request)
        {
            return new HttpResponse()
            {
                ContentAsUTF8 = "Hello",
                ReasonPhrase = "OK",
                StatusCode = "200"
            };
        }


        private static HttpResponse HomeIndex(HttpRequest request)
        {

            string value = "";
            if (request.Headers.ContainsKey("WMS_DATA"))
            {
                value = request.Headers["WMS_DATA"];
            }

            return new HttpResponse()
            {
                ContentAsUTF8 = "{\"data\":\""+ value
                    +"\",\"name\":\"kyle\",\"age\":18,\"friend\":[{\"name\":\"matt\",\"sex\":\"man\"},{\"name\":\"butt\",\"sex\":\"man\"}]}",
                ReasonPhrase = "OK",
                StatusCode = "200"
            };
        }
    }
}
