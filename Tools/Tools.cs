using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
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

    }

}