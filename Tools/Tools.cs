using System;

namespace Tools
{
    public class Error 
    {
        public String Type;
        public String Message;

        public Error(String _type) { }
    }

    namespace net
    {
        public class Get
        {
            /// <summary>
            /// 发生网络Get请求
            /// </summary>
            /// <param name="getUrl">请求的URL</param>
            /// <param name="contentType">链接样式 [可空]默认"text/json"</param>
            public Get(string getUrl, string contentType = "text/json")
            { 
                
            }
        }











        public class Download
        {

        }
    }

}
