using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tools
{
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
}
