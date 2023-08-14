using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace Tools
{
    //数据处理
    namespace Data
    {
        public class JSON
        {
            /// <summary>
            /// 将JSON格式的字符串重新反序列化
            /// </summary>
            /// <param name="jsonstr">JSON格式的字符串</param>
            /// <returns></returns>
            public static T parse<T>(String jsonstr)
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                return js.Deserialize<T>(jsonstr);
            }

            /// <summary>
            /// 将字符串反序列化为只能靠索引取值的对象
            /// </summary>
            /// <param name="jsonstr">JSON格式的字符串</param>
            /// <returns>一个包含JSON值的dynamic对象</returns>
            public static dynamic parse(String jsonstr)
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                return js.Deserialize<dynamic>(jsonstr);
            }

            /// <summary>
            /// JSON对象(类对象)序列化为字符串
            /// </summary>
            /// <param name="jsonobj">对象</param>
            /// <returns>返回序列化后的字符串</returns>
            public static String stringify(object jsonobj)
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                return js.Serialize(jsonobj);
            }
        }

        public class Data
        {
            //复制数据到剪贴板
            public static void CopyData(object _data)
            {
                Clipboard.SetDataObject(_data);
            }

            /// <summary>
            /// 复制到剪贴板指定格式的数据
            /// </summary>
            /// <param name="format"></param>
            /// <param name="data">数据</param>
            public static void CopyData(string format,object data)
            {
                Clipboard.SetData(format, data);
            }
        }

        /// <summary>
        /// 以给定的概率返回 <br>
        /// 比如 1 2 3 4 分别以 10%,20%,30%,40%的概率获取其中的一个
        /// </summary>
        public class Randoms
        {
            Random random;

            public Randoms()
            {
                random = new Random();
            }

            public Randoms(int seed)
            {
                random = new Random(seed);
            }

            /// <summary>
            /// 传入一个数组
            /// </summary>
            /// <param name="probability">每个成员都是一个概率0-100之间,和等于100</param>
            /// <returns>返回选中这个概率的下标</returns>
            /// <exception cref="ArgumentException">概率没有按照要求传入</exception>
            public int Ranges(int[] probability)
            {
                // 数组成员值必须在 0 - 100 之间, 且 和必须为100
                int[] cumulativeData = new int[probability.Length];
                for (int i = 0; i < probability.Length; i++)
                {
                    if (probability[i] < 0 || probability[i] > 100) throw new ArgumentException("参数成员值范围不在0-100之间", "probability[" + i.ToString() + "]");
                    cumulativeData[i] = (i == 0) ? probability[i] : cumulativeData[i - 1] + probability[i];
                }
                if (cumulativeData[cumulativeData.Length - 1] != 100) throw new ArgumentException("参数成员值的和不为100", "probability");

                int v = random.Next(100);
                for (int i = 0; i < cumulativeData.Length; i++)
                {
                    if (v < cumulativeData[i])
                    {
                        if (i == 0 || v >= cumulativeData[i - 1])
                        {
                            return i;
                        }
                    }
                }
                // 理论永远不会触发的返回值
                return 0;
            }
        }

    }
}
