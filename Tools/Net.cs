using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Tools
{
    namespace Net
    {
        public class Get
        {
            public String GetUrl;
            public String ContentType = "text/json";
            public String UA = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:74.0) Gecko/20100101 Firefox/74.0";
            public String IP;
            public int Port = 6800;
            public String CookieStr;
            private String ReCookies;   //发生请求之后 获取服务端在流修改设置的Cookie;
            public String Referer;

            /// <summary>
            /// 要和链接一起发送的数据,如:user=123&pass=456
            /// </summary>
            public String data;

            /// <summary>
            /// 发送网络Get请求
            /// </summary>
            /// <param name="getUrl">请求的URL</param>
            public Get(string getUrl = null)
            {
                GetUrl = getUrl;
            }

            public String GetCookies()
            {
                return ReCookies;
            }

            /// <summary>
            /// 发送Get请求并返回数据
            /// </summary>
            /// <returns>返回服务器数据</returns>
            public String Getdata()
            {
                if (GetUrl == String.Empty || GetUrl == null)
                {
                    throw new Error("Tools.NET.Get", "请求连接为空");
                }
                try
                {
                    //整合要传送的数据
                    if (data != null)
                    {
                        GetUrl += "?" + data;
                    }
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(GetUrl);
                    myRequest.Method = "GET";
                    myRequest.UserAgent = UA;
                    myRequest.ContentType = ContentType;

                    //需要时添加代理服务器
                    if (IP != null)
                    {
                        myRequest.Proxy = new WebProxy(IP, Port); //设置代理
                    }

                    //添加自定义的Cookie
                    if (CookieStr != null) myRequest.Headers.Add("Cookie",CookieStr);
                    //设置请求Referer信息
                    if (Referer != null) myRequest.Referer = Referer;
                    

                    //获取远程服务器响应
                    HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                    StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                    string content = reader.ReadToEnd();
                    ReCookies = myResponse.Headers["Set-Cookie"].Replace(", ",";");

                    reader.Close();
                    
                    return content;
                }
                catch (Exception Ex)
                {
                    throw new Error("Tools.NET.Get", Ex.GetType().ToString() + Ex.Message);
                }
            }
        }


        public class Post
        {
            public String GetUrl;
            public String ContentType = "application/x-www-form-urlencoded";
            public String UA = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:74.0) Gecko/20100101 Firefox/74.0";
            public String IP;
            public int Port = 6800;
            public String Referer;
            private String ReCookies;   //发生请求之后 获取服务端在流修改设置的Cookie；

            /// <summary>
            /// 要和链接一起发送的数据,如:user=123&pass=456
            /// </summary>
            public String data;
            public String CookieStr;

            /// <summary>
            /// 发送网络Post请求
            /// </summary>
            /// <param name="getUrl">请求链接地址</param>
            public Post(string getUrl = null)
            {
                GetUrl = getUrl;
            }

            public String GetCookies()
            {
                return ReCookies;
            }

            public String Getdata()
            {
                if (GetUrl == String.Empty || GetUrl == null)
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
                    if (IP != null)
                    {
                        myRequest.Proxy = new WebProxy(IP, Port); //设置代理
                    }

                    //添加自定义的Cookie
                    if (CookieStr != null) myRequest.Headers.Add("Cookie", CookieStr);
                    //设置请求Referer信息
                    if (Referer != null) myRequest.Referer = Referer;

                    //发送数据
                    if (data != null)
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
                    ReCookies = myResponse.Headers["Set-Cookie"].Replace(", ", ";");

                    reader.Close();
                    return content;
                }
                catch (Exception Ex)
                {
                    throw new Error("Tools.NET.Post", Ex.GetType().ToString() + Ex.Message);
                }
            }
        }


        public class Download
        {
            public String Url;
            public String SavePath; //保存文件的路径，不包含文件名
            public String SaveFile; //保存文件的文件名
            public String Suffix = ".tooling";//默认下载后缀，下载过程中显示的后缀,下载完成移除
            public long HttpFileSize;   //要下载的文件的总大小
            public Thread Thread;   //调用的线程句柄
            public Fileoperate.WriteFile FileStream;
            public long SPosition = 0;  //告诉服务器开始下载位置
            public Stream HttpStream;   //文件下载句柄
            public int bt = 1024*10;   //下载字节数?下载速度
            public String Referer = String.Empty;

            /// <summary>
            /// 委托 回调给使用者下载进度
            /// </summary>
            /// <param name="downloadedsize">已下载大小</param>
            /// <param name="filesize">总共大小</param>
            /// <param name="waft">是否下载完成</param>
            public delegate void info(long downloadedsize, long filesize, bool waft);

            //用于回调下载进度
            public event info Downprogress;

            protected virtual void OnDownprogress(long downloadedsize, long filesize, bool waft)
            {
                Downprogress?.Invoke(downloadedsize, filesize, waft);
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
            /// 获取百分比
            /// </summary>
            /// <param name="news">当前已下载大小</param>
            /// <param name="altogether">总大小</param>
            /// <returns>返回100以内的包含一位小数的小数</returns>
            public static double GetdoblePercent(long news, long altogether)
            {
                return Math.Floor(news * 1.000 / altogether * 1000) / 10;
            }

            /// <summary>
            /// 转换大小单位为Mb
            /// </summary>
            /// <param name="b">当前流的数据大小[b]</param>
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
            /// <returns>2:下载完成, 1:开始下载，0:某种原因没有开始下载</returns>
            public int Start()
            {
                try
                {
                    if (SavePath == "" || Url == "" || SaveFile == "")
                    {
                        return 0;
                    }
                    if (!SavePath.EndsWith("\\"))   //如果保存路径的结尾不是反斜杠,则补充
                    {
                        SavePath += "\\";
                    }

                    if (Suffix == "") new Error("Tools.Net.Download", "不允许下载到本地文件的的文件名后缀设为空:Suffix!=\"\"");

                    HttpFileSize = GetHttpLength(Url);  //获取服务器文件的大小

                    FileInfo fileA = new FileInfo(SavePath + SaveFile); //获取要求下载至本地文件的属性
                    FileInfo fileB = new FileInfo(SavePath + SaveFile + Suffix); //获取即将要下载到本地的文件的信息属性


                    if (fileA.Exists)   //判断本地文件是否存在,如果存在，则进一步判断
                    {
                        if (fileA.Length >= HttpFileSize)   // 本地文件和远程文件一样大 下载完成
                        {
                            OnDownprogress(HttpFileSize, HttpFileSize, true);
                            return 2;
                        }

                        if (fileB.Exists)   //包含自定义后缀的文件是否存在,如果存在则进一步判断
                        {
                            if (fileB.Length >= HttpFileSize)    // 本地文件和远程文件一样大 下载完成
                            {
                                fileB.MoveTo(SavePath + SaveFile);
                                OnDownprogress(HttpFileSize, HttpFileSize, true);
                                return 2;
                            }

                            if (fileA.Length > fileB.Length)
                            {
                                fileB.Delete();
                                fileA.MoveTo(SavePath + SaveFile + Suffix);
                            }
                            else
                            {
                                fileA.Delete();
                            }
                        }
                        else
                        {
                            fileA.MoveTo(SavePath + SaveFile + Suffix);
                        }
                    }
                    else
                    {
                        if (fileB.Exists)
                        {
                            if (fileB.Length >= HttpFileSize)    // 本地缓存文件和远程文件一样大 下载完成
                            {
                                fileB.MoveTo(SavePath + SaveFile);
                                OnDownprogress(HttpFileSize, HttpFileSize, true);
                                return 2;
                            }
                        }
                    }

                    FileStream = new Fileoperate.WriteFile(SavePath + SaveFile + Suffix);   //创建本地缓存文件的文件流



                    if (SPosition == 0) SPosition = FileStream.Sposition;   //是否指定了下载位置从头下载，否则从本地文件所在的位置开始下载。


                    info Toinfo = new info(Downprogress);


                    Thread = new Thread(new ThreadStart(Down));
                    Thread.IsBackground = true;
                    Thread.Start();
                    return 1;
                }
                catch (Exception e)
                {
                    new Error("Tools.Net.Download",e.Message);
                }
                return 0;
            }


            private void Down()
            {
                bool flag = false;
                try
                {
                    HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(Url);

                    if (SPosition > 0) myRequest.AddRange(SPosition);             //设置Range值
                    if (Referer != String.Empty) myRequest.Referer = ""; //设置源页面信息 Referer

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
                        File.Move(SavePath + SaveFile + Suffix, SavePath + SaveFile);
                        OnDownprogress(HttpFileSize, HttpFileSize, flag);
                    }
                }
            }

            /// <summary>
            /// 获取远程文件的大小
            /// </summary>
            /// <param name="url"></param>
            /// <param name="Referer"></param>
            /// <returns></returns>
            public static long GetHttpLength(string url, String Referer = null)
            {
                long length = 0;
                HttpWebRequest req = null;
                HttpWebResponse rsp = null;
                try
                {
                    req = (HttpWebRequest)HttpWebRequest.Create(url);
                    if (Referer != null) req.Referer = Referer; //设置请求来源
                    rsp = (HttpWebResponse)req.GetResponse();
                    if (rsp.StatusCode == HttpStatusCode.OK)
                        length = rsp.ContentLength;
                }
                catch
                {
                    //获取远程文件大小失败
                    length = 0;
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

            /// <summary>
            /// 获取远程将下载文件的文件名
            /// </summary>
            /// <param name="url"></param>
            /// <param name="Referer"></param>
            /// <returns></returns>
            public String GetHttpFileName(String url, String Referer = null)
            {
                String FileName = String.Empty;
                HttpWebRequest req = null;
                HttpWebResponse rsp = null;
                try
                {
                    req = (HttpWebRequest)HttpWebRequest.Create(url);
                    if (Referer != null) req.Referer = Referer; //设置请求来源
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


            /// <summary>
            /// 获取URL请求的信息数据,用于较小数据的获取，如文本，图片
            /// </summary>
            /// <param name="Url"></param>
            /// <param name="Headers"></param>
            /// <returns></returns>
            public static String GetHttpData(string Url,WebHeaderCollection Headers = null)
            {
                string strResult = "";
                
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                    //request.ServerCertificateValidationCallback = delegate { return true; };
                    //声明一个HttpWebRequest请求
                    request.Timeout = 30*1000;  //设置连接超时时间
                    if (Headers != null)
                    {
                        request.Headers = Headers;
                    }
                    else
                    {
                        request.Headers.Set("Pragma", "no-cache");
                    }
                    request.Method = "GET";
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    if (response.ToString() != "")
                    {
                        Stream streamReceive = response.GetResponseStream();
                        Encoding encoding = Encoding.GetEncoding("UTF-8");
                        StreamReader streamReader = new StreamReader(streamReceive, encoding);
                        strResult = streamReader.ReadToEnd();
                    }
                }
                catch(Exception ex)
                {
                    strResult = "";
                    //strResult = ex.ToString();
                }
                return strResult;
            }

        }
        
        
    }
}
