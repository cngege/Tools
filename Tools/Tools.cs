using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Tools
{
    /// <summary>
    /// 错误类
    /// </summary>
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

    namespace File
    {
        /// <summary>
        /// .ini配置文件类，可读写操作.ini配置文件
        /// </summary>
        public class InIFile
        {
            [DllImport("kernel32")]
            private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);
            [DllImport("kernel32")]
            private static extern int WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpString, string lpFileName);

            /// <summary>
            /// 该实例默认的配置文件路径，如果操作时没有指定配置文件，则之前必须要将路径赋值到此变量
            /// </summary>
            public String FilePath = String.Empty;

            public InIFile(String _filepath = "")
            {
                if (_filepath != String.Empty)
                {
                    FilePath = _filepath;
                }
            }

            private static void CheckPath(String _Path)
            {
                if (Directory.Exists(Path.GetDirectoryName(_Path)) == false)
                {
                    new DirectoryInfo(Path.GetDirectoryName(_Path)).Create();//如何这个文件的文件夹不存在 则创建一个文件夹 
                }
                if (System.IO.File.Exists(_Path) == false)
                {
                    System.IO.File.Create(_Path);//如果文件不存在 则创建这个文件
                }
            }


            /// <summary>
            /// 设置你要操作文件的路径，你要操作之前可以用各种方法设置路径，但是必须要有
            /// </summary>
            /// <param name="_FilePath">字符串 路径</param>
            public void SetFilePath(String _FilePath)
            {
                FilePath = _FilePath;
            }

            /// <summary>
            /// 读取节点文件相信键值内容
            /// </summary>
            /// <param name="section">节点名称</param>
            /// <param name="key">键名称</param>
            /// <param name="def">如果读不到则返回该值</param>
            /// <param name="_filePath">节点文件路径 [可空]</param>
            /// <returns></returns>
            public String Read(string section, string key, string def, string _filePath = "")
            {
                if (_filePath == String.Empty)
                {
                    _filePath = FilePath;
                    if (FilePath == String.Empty)
                    {
                        throw new Error("Tools.File.INIFile","错误:要操作的路径为空");
                    }
                }
                StringBuilder sb = new StringBuilder(1024);
                GetPrivateProfileString(section, key, def, sb, 1024, _filePath);
                return sb.ToString();
            }


            /// <summary>
            /// 向配置文件中写入信息
            /// </summary>
            /// <param name="section">节点名称</param>
            /// <param name="key">键名称</param>
            /// <param name="value">键值</param>
            /// <param name="_filePath">要操作的路径 [可空]</param>
            /// <returns>非0表示成功 0表示失败</returns>
            public int Write(string section, string key, string value, string _filePath = "")
            {
                if (_filePath == String.Empty)
                {
                    _filePath = FilePath;
                    if (FilePath == String.Empty)
                    {
                        throw new Error("Tools.File.INIFile", "错误:要操作的路径为空");
                    }
                }
                CheckPath(_filePath);
                return WritePrivateProfileString(section, key, value, _filePath);
            }

            /// <summary>
            /// 删除配置文件节点
            /// </summary>
            /// <param name="section">节点名称</param>
            /// <param name="_filePath">配置文件路径 [可空]</param>
            /// <returns>非0表示成功 0表示失败</returns>
            public int DeleteSection(string section, string _filePath = "")
            {
                if(_filePath == String.Empty)
                {
                    _filePath = FilePath;
                    if (FilePath == String.Empty)
                    {
                        throw new Error("Tools.File.INIFile", "错误:要操作的路径为空");
                    }
                }
                return Write(section, null, null, _filePath);
            }

            /// <summary>
            /// 删除配置文件键所对应的值
            /// </summary>
            /// <param name="section">节点名称</param>
            /// <param name="key">键名称</param>
            /// <param name="_filePath">配置文件路径 [可空]</param>
            /// <returns>非0表示成功 0表示失败</returns>
            public int DeleteKey(string section, string key, string _filePath)
            {
                if (_filePath == String.Empty)
                {
                    _filePath = FilePath;
                    if (FilePath == String.Empty)
                    {
                        throw new Error("Tools.File.INIFile", "错误:要操作的路径为空");
                    }
                }
                return Write(section, key, null, _filePath);
            }
        }
    }

    //窗口操作
    namespace Formoperate
    {
        public class Forms
        {
            /// <summary>
            /// 释放鼠标资源 否则SeedMessage可能会失败
            /// </summary>
            /// <returns></returns>
            [DllImport("user32.dll")]
            public static extern bool ReleaseCapture();

            [DllImport("user32.dll")]
            public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

            /// <summary>
            /// 移动窗口，代码放在Down事件下
            /// </summary>
            /// <param name="handle">窗口句柄[(IntPtr)this.handle]</param>
            /// <returns>消息是否发送成功</returns>
            public static bool MoveFrom(IntPtr handle)
            {
                ReleaseCapture();
                return SendMessage(handle, 0XA1, 2, 0);
            }

            /// <summary>
            /// 发送窗口消息修改窗口的状态 最大化 最小化 关闭 还原
            /// </summary>
            /// <param name="form">窗口句柄</param>
            /// <param name="size">0：关闭，1：最小化，2：还原，3：最大化</param>
            public static void FormMessage(IntPtr form, int size)
            {
                ReleaseCapture();
                switch (size)
                {

                    case 0: SendMessage(form, 0x112, 0xf060, 0); break;//关闭
                    case 1: SendMessage(form, 0x112, 0xf020, 0); break;//最大化
                    case 2: SendMessage(form, 0x112, 0xf120, 0); break;//还原
                    case 3: SendMessage(form, 0x112, 0xf030, 0); break;//最小化
                }
            }

             
        }
    }

}
