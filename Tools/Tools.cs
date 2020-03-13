using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace Tools
{
    
    public class Error : Exception 
    {
        public String Type;
        public String MessageText;

        /// <summary>
        /// 用于提供工具箱抛出特定类型的错误
        /// </summary>
        /// <param name="_Type">是哪一种错误</param>
        /// <param name="_Message">错误的具体内容</param>
        public Error(String _Type, String _Message)
        {
            Type = _Type;
            MessageText = _Message;
        }

        public String Gettype()
        {
            return Type;
        }

        public String GetMessage()
        {
            return MessageText;
        }
    }

    namespace net
    {
        public class Get
        {
            public String GetUrl;
            public String ContentType = "text/json";
            public String UA = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:74.0) Gecko/20100101 Firefox/74.0";
            public String IP = String.Empty;
            public int Port = 6800;
            public String CookieStr = String.Empty;

            /// <summary>
            /// 要和链接一起发送的数据,如:user=123&pass=456
            /// </summary>
            public String data = String.Empty;

            /// <summary>
            /// 发送网络Get请求
            /// </summary>
            /// <param name="getUrl">请求的URL</param>
            public Get(string getUrl = "")
            {
                GetUrl = getUrl;
            }

            /// <summary>
            /// 发送Get请求并返回数据
            /// </summary>
            /// <returns>返回服务器数据</returns>
            public String Getdata()
            {
                if (GetUrl == String.Empty)
                {
                    throw new Error("NET.Get", "请求连接为空");
                }
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(GetUrl);
                    myRequest.Method = "GET";
                    myRequest.UserAgent = UA;
                    myRequest.ContentType = ContentType;

                    //需要时添加代理服务器
                    if (IP != String.Empty)
                    {
                        WebProxy proxyObject = new WebProxy(IP, Port);//代理类
                        myRequest.Proxy = proxyObject; //设置代理
                    }

                    //添加自定义的Cookie
                    if (CookieStr != String.Empty)
                    {
                        myRequest.Headers.Add("Cookie", CookieStr);
                    }

                    //发送数据
                    if (data != String.Empty)
                    {
                        byte[] _data = Encoding.UTF8.GetBytes(this.data);
                        myRequest.ContentLength = _data.Length;
                        Stream stream = myRequest.GetRequestStream();
                        stream.Write(_data, 0, _data.Length);
                        stream.Close();
                    }

                    //获取远程服务器响应
                    HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                    StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                    string content = reader.ReadToEnd();
                    reader.Close();
                    return content;
                }
                catch(Exception Ex)
                {
                    throw new Error(Ex.GetType().ToString(), Ex.Message);
                }
            }
        }


        public class Post
        {
            public String GetUrl;
            public String ContentType = "application/x-www-form-urlencoded";
            public String UA = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:74.0) Gecko/20100101 Firefox/74.0";
            public String IP = String.Empty;
            public int Port = 6800;

            /// <summary>
            /// 要和链接一起发送的数据,如:user=123&pass=456
            /// </summary>
            public String data = String.Empty;
            public String CookieStr = String.Empty;

            /// <summary>
            /// 发送网络Post请求
            /// </summary>
            /// <param name="getUrl">请求链接地址</param>
            public Post(string getUrl = "")
            {
                GetUrl = getUrl;
            }

            public String Getdata()
            {
                if (GetUrl == String.Empty)
                {
                    throw new Error("NET.Post", "请求连接为空");
                }
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(GetUrl);
                    myRequest.Method = "POST";
                    myRequest.UserAgent = UA;
                    myRequest.ContentType = ContentType;

                    //需要时添加代理服务器
                    if (IP != String.Empty)
                    {
                        WebProxy proxyObject = new WebProxy(IP, Port);//代理类
                        myRequest.Proxy = proxyObject; //设置代理
                    }
                    
                    //添加自定义的Cookie
                    if (CookieStr != String.Empty)
                    {
                        myRequest.Headers.Add("Cookie",CookieStr);
                    }

                    //发送数据
                    if (data != String.Empty)
                    {
                        byte[] _data = Encoding.UTF8.GetBytes(this.data);
                        myRequest.ContentLength = _data.Length;
                        Stream stream = myRequest.GetRequestStream();
                        stream.Write(_data, 0, _data.Length);
                        stream.Close();
                    }

                    //获取数据响应
                    HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                    StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                    
                    string content = reader.ReadToEnd();
                    reader.Close();
                    return content;
                }
                catch (Exception Ex)
                {
                    throw new Error(Ex.GetType().ToString(), Ex.Message);
                }
            }
        }









        public class Download
        {

        }
    }

}
