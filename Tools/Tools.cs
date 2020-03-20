using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

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

            //用于回调下载进度
            public info Downprogress;
            /// <summary>
            /// 委托 回调给使用者下载进度
            /// </summary>
            /// <param name="filesize">已下载大小</param>
            /// <param name="downsize">总共大小</param>
            /// <param name="waft">是否下载完成</param>
            public delegate void info(long filesize, long downsize,bool waft);

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
                        Downprogress?.Invoke(HttpFileSize, HttpFileSize, flag);//委托如果存在则……
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
                        FileStream.Write(btContent,intSize);
                        intSize = HttpStream.Read(btContent, 0, btContent.Length);
                        Downprogress?.Invoke(FileStream.Handle.Length, HttpFileSize, flag);
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
                        Downprogress?.Invoke(HttpFileSize, HttpFileSize, flag);
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

        }

    }

    //文件操作
    namespace Fileoperate
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
                if (File.Exists(_Path) == false)
                {
                    File.Create(_Path);//如果文件不存在 则创建这个文件
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

        public class WriteFile
        {
            //打开的文件句柄
            public FileStream Handle;
            public String Path; //写入的文件路径
            public long Sposition; //开始写的位置，实例化的时候会自动获取，也可手动指定

            public WriteFile(FileStream _Handle)
            {
                Handle = _Handle;
                //Handle = File.OpenWrite(Path);
                Sposition = Handle.Length;  //获取本地原本文件的文件长度
                Handle.Seek(Sposition, SeekOrigin.Current);

            }
            public WriteFile(String _Path)
            {
                Path = _Path;
                if (File.Exists(Path))
                {
                    Handle = File.OpenWrite(Path);
                    Sposition = Handle.Length;  //获取本地原本文件的文件长度
                    Handle.Seek(Sposition, SeekOrigin.Current);
                }
                else
                {
                    Handle = new FileStream(Path, FileMode.Create);
                    Sposition = 0;
                }
                
            }

            public long GetFileSize()
            {
                if (Handle != null)
                {
                    return Handle.Length;
                }
                else
                {
                    return 0;
                }
            }

            /// <summary>
            /// 写入数据到文件
            /// </summary>
            /// <param name="bt">写入的字节数组数据</param>
            /// <param name="_length">写入长度</param>
            public void Write(byte[] bt,int _length)
            {
                Handle.Write(bt, 0, _length);
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

    //内存操作
    namespace Address
    {
        public class address
        {
            //从指定内存中读取字节集数据
            [DllImportAttribute("kernel32.dll", EntryPoint = "ReadProcessMemory")]
            public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int nSize, IntPtr lpNumberOfBytesRead);

            //从指定内存中写入字节集数据
            [DllImportAttribute("kernel32.dll", EntryPoint = "WriteProcessMemory")]
            public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, int[] lpBuffer, int nSize, IntPtr lpNumberOfBytesWritten);

            //打开一个已存在的进程对象，并返回进程的句柄
            [DllImportAttribute("kernel32.dll", EntryPoint = "OpenProcess")]
            public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

            //关闭一个内核对象。其中包括文件、文件映射、进程、线程、安全和同步对象等。
            [DllImport("kernel32.dll")]
            private static extern void CloseHandle(IntPtr hObject);

            /// <summary>
            /// 进程名获取Pid
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public static int GetPid(String name)
            {
                Process[] arrayProcess = Process.GetProcessesByName(name);
                foreach (Process p in arrayProcess)
                {
                    return p.Id;
                }
                return 0;
            }

            /// <summary>
            /// 读内存
            /// </summary>
            /// <param name="address">内存地址</param>
            /// <param name="pid">进程Pid</param>
            /// <param name="AddrSize">读取的内存大小</param>
            /// <returns>返回的内存值</returns>
            public static int ReadValue(int address, int pid, int AddrSize = 4)
            {
                try
                {
                    byte[] buffer = new byte[4];
                    IntPtr byteAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                    //打开一个已存在的进程对象  0x1F0FFF 最高权限
                    IntPtr hProcess = OpenProcess(0x1F0FFF, false, pid);
                    //将制定内存中的值读入缓冲区
                    ReadProcessMemory(hProcess, (IntPtr)address, byteAddress, AddrSize, IntPtr.Zero);
                    //关闭操作
                    CloseHandle(hProcess);
                    //从非托管内存中读取一个 32 位带符号整数。
                    return Marshal.ReadInt32(byteAddress);
                }
                catch
                {
                    return 0;
                }
            }

            /// <summary>
            /// 写内存
            /// </summary>
            /// <param name="address">内存地址</param>
            /// <param name="value">写入的内存值</param>
            /// <param name="pid">进程pid</param>
            /// <param name="AddrSize">写入的地址大小</param>
            public static void WriteValue(int address , int value, int pid, int AddrSize = 4)
            {
                //打开一个已存在的进程对象  0x1F0FFF 最高权限
                IntPtr hProcess = OpenProcess(0x1F0FFF, false, pid);
                //从指定内存中写入字节集数据
                WriteProcessMemory(hProcess, (IntPtr)address, new int[] { value }, AddrSize, IntPtr.Zero);
                //关闭操作
                CloseHandle(hProcess);
            }
        }
    }

    //数据处理
    namespace Data
    {
        public class JSON : JObject 
        {
            /// <summary>
            /// 将JSON格式的字符串重新反序列化
            /// </summary>
            /// <param name="_json">JSON格式的字符串</param>
            /// <returns></returns>
            public static JSON ToJson(String _json)
            {
                return (JSON)Parse(_json);
            }
        }


    }

    //"简单化线程"
    namespace SimpleThread
    {
        public class proxy
        {

        }

        
        
    }

}