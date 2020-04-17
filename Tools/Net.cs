using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Tools
{
    namespace Net
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
                    throw new Error("Tools.NET.Get", "请求连接为空");
                }
                try
                {
                    //整合要传送的数据
                    if (data != String.Empty)
                    {
                        GetUrl += "?" + data;
                    }
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

                    //获取远程服务器响应
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
                    throw new Error("Tools.NET.Post", "请求连接为空");
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
            public String Url;
            public String SavePath; //保存文件的路径，不包含文件名
            public String SaveFile; //保存文件的文件名
            public long HttpFileSize;   //要下载的文件的总大小
            public Thread Thread;   //调用的线程句柄
            public Fileoperate.WriteFile FileStream;
            public long SPosition = 0;  //告诉服务器开始下载位置
            public Stream HttpStream;   //文件下载句柄
            public int bt = 1024;   //下载字节数?下载速度

            /// <summary>
            /// 委托 回调给使用者下载进度
            /// </summary>
            /// <param name="filesize">已下载大小</param>
            /// <param name="downsize">总共大小</param>
            /// <param name="waft">是否下载完成</param>
            public delegate void info(long filesize, long downsize, bool waft);

            //用于回调下载进度
            public event info Downprogress;

            protected virtual void OnDownprogress(long filesize, long downsize, bool waft)
            {
                Downprogress?.Invoke(filesize, downsize, waft);
            }



            /// <summary>
            /// 获取百分比
            /// </summary>
            /// <param name="news">当前已下载大小</param>
            /// <param name="altogether">总大小</param>
            /// <returns>返回1以内的包含两位小数的小数</returns>
            public static double GetPercent(long news, long altogether)
            {
                return Math.Floor(news *1.00 / altogether * 100) / 100;
            }

            /// <summary>
            /// 获取百分比
            /// </summary>
            /// <param name="news">当前已下载大小</param>
            /// <param name="altogether">总大小</param>
            /// <returns>返回100以内的整数</returns>
            public static int GetintPercent(long news, long altogether)
            {
                return (int)Math.Floor(news * 1.00 / altogether * 100);
            }

            /// <summary>
            /// 转换大小单位为Mb
            /// </summary>
            /// <param name="b">当前流的数据大小</param>
            /// <returns>返回两位小数单位Mb</returns>
            public static double GetSize(long b)
            {
                return Math.Floor(b * 1.0 / (1024 * 1024) * 100) / 100;
            }

            public Download(String url = "", String savepath = "", String filename = "")
            {
                Url = url;
                SavePath = savepath;
                SaveFile = filename;
            }

            public Download() { }

            /// <summary>
            /// 开始下载
            /// </summary>
            /// <returns>true:开始下载，False:某种原因没有开始下载</returns>
            public bool Start()
            {
                if (SavePath == "" || Url == "" || SaveFile == "")
                {
                    return false;
                }
                if (!SavePath.EndsWith("\\"))
                {
                    SavePath += "\\";
                }

                HttpFileSize = GetHttpLength(Url);

                FileStream = new Fileoperate.WriteFile(SavePath + SaveFile);


                if (SPosition == 0)
                {
                    SPosition = FileStream.Sposition;
                }

                info Toinfo = new info(Downprogress);


                Thread = new Thread(new ThreadStart(Down));
                Thread.IsBackground = true;
                Thread.Start();
                return true;
            }


            private void Down()
            {
                bool flag = false;
                try
                {
                    if (FileStream.Handle.Length == HttpFileSize)
                    {
                        flag = true;
                        //关闭流
                        if (HttpStream != null)
                        {
                            HttpStream.Close();
                            HttpStream.Dispose();
                        }
                        if (FileStream.Handle != null)
                        {
                            FileStream.Handle.Close();
                            FileStream.Handle.Dispose();
                        }
                        OnDownprogress(HttpFileSize, HttpFileSize, flag);
                        return;
                    }
                    HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(Url);
                    if (SPosition > 0)
                    {
                        myRequest.AddRange(SPosition);             //设置Range值
                    }
                    HttpStream = myRequest.GetResponse().GetResponseStream();
                    byte[] btContent = new byte[bt];
                    int intSize = 0;
                    intSize = HttpStream.Read(btContent, 0, btContent.Length);
                    while (intSize > 0)
                    {
                        FileStream.Write(btContent, intSize);
                        intSize = HttpStream.Read(btContent, 0, btContent.Length);
                        OnDownprogress(FileStream.Handle.Length, HttpFileSize, flag);
                    }

                    flag = true;
                }
                catch
                {

                }
                finally
                {

                    //关闭流
                    if (HttpStream != null)
                    {
                        HttpStream.Close();
                        HttpStream.Dispose();
                    }
                    if (FileStream.Handle != null)
                    {
                        FileStream.Handle.Close();
                        FileStream.Handle.Dispose();
                    }
                    if (flag)
                    {
                        //下载完成 传递信号
                        OnDownprogress(HttpFileSize, HttpFileSize, flag);
                    }
                }
            }

            /// <summary>
            /// 获取远程文件的大小
            /// </summary>
            /// <param name="url"></param>
            /// <returns></returns>
            public long GetHttpLength(string url)
            {
                long length = 0;
                HttpWebRequest req = null;
                HttpWebResponse rsp = null;
                try
                {
                    req = (HttpWebRequest)HttpWebRequest.Create(url);
                    rsp = (HttpWebResponse)req.GetResponse();
                    if (rsp.StatusCode == HttpStatusCode.OK)
                        length = rsp.ContentLength;
                }
                catch
                {
                    //获取远程文件大小失败
                }
                finally
                {
                    if (rsp != null)
                        rsp.Close();
                    if (req != null)
                        req.Abort();
                }

                return length;
            }

            public String GetHttpFileName(String url)
            {
                String FileName = String.Empty;
                HttpWebRequest req = null;
                HttpWebResponse rsp = null;
                try
                {
                    req = (HttpWebRequest)HttpWebRequest.Create(url);
                    rsp = (HttpWebResponse)req.GetResponse();
                    if (rsp.StatusCode == HttpStatusCode.OK)
                        FileName = rsp.Headers["Content-Disposition"];
                    if (string.IsNullOrEmpty(FileName))
                        FileName = rsp.ResponseUri.Segments[rsp.ResponseUri.Segments.Length - 1];
                    else
                        FileName = FileName.Remove(0, FileName.IndexOf("filename=") + 9);
                }
                catch
                {
                    //获取远程文件大小失败
                    FileName = "";
                }
                finally
                {
                    if (rsp != null)
                        rsp.Close();
                    if (req != null)
                        req.Abort();
                }

                return FileName;
            }

            public static String GetHttpData(string Url)
            {
                string strResult = "";
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                    //声明一个HttpWebRequest请求
                    request.Timeout = 3000000;
                    //设置连接超时时间
                    request.Headers.Set("Pragma", "no-cache");
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    if (response.ToString() != "")
                    {
                        Stream streamReceive = response.GetResponseStream();
                        Encoding encoding = Encoding.GetEncoding("UTF-8");
                        StreamReader streamReader = new StreamReader(streamReceive, encoding);
                        strResult = streamReader.ReadToEnd();
                    }
                }
                catch
                {
                    strResult = "";
                }
                return strResult;
            }

        }


    }
}
