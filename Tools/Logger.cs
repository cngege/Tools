using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    namespace Logger
    {
        public class OldLogger
        {
            private static ConsoleColor infoTextColor = ConsoleColor.White;
            private static ConsoleColor warnTextColor = ConsoleColor.Yellow;
            private static ConsoleColor errorTextColor = ConsoleColor.Red;
            private static ConsoleColor subTitleTextColor = ConsoleColor.Gray;


            private static bool displayColor = true;
            private static string subTitle = string.Empty;

            // Logger.SubTitle("我是副标题").Info("你好世界");
            // Logger.SetSubTitle("我是副标题");

            //private static int colorCount = 0;
            //private static ConsoleColor nextTextColor = ConsoleColor.Gray;

            //private static int subTitleCount = 0;
            //private static string nextSubTitle = subTitle;

            static string getForMatTime(DateTime t)
            {
                return string.Format("{0}:{1}:{2}:{3}",
                    t.Hour.ToString().PadLeft(2, '0'),
                    t.Minute.ToString().PadLeft(2, '0'),
                    t.Second.ToString().PadLeft(2, '0'),
                    t.Millisecond.ToString().PadLeft(3, '0'));
            }

            /// <summary>
            /// Info的输出文字颜色会被环境颜色覆盖,只有环境颜色是默认(Gray)时,该项才有效
            /// </summary>
            /// <param name="infoColor"></param>
            public static void SetInfoTextColor(ConsoleColor infoColor)
            {
                infoTextColor = infoColor;
            }

            public static void SetWarnTextColor(ConsoleColor warnColor)
            {
                warnTextColor = warnColor;
            }

            public static void SetErrorTextColor(ConsoleColor errorColor)
            {
                errorTextColor = errorColor;
            }

            public static void SetSubTitleTextColor(ConsoleColor subTitleColor)
            {
                subTitleTextColor = subTitleColor;
            }

            public static void EnableColor(bool enable = true)
            {
                displayColor = enable;
            }

            /// <summary>
            /// 设置一个统一的副标题
            /// </summary>
            /// <param name="subtitle"></param>
            public static void SetSubTitle(string subtitle)
            {
                subTitle = subtitle;
            }


            //public static Type SubTitle(string subtitle)
            //{
            //    return;
            //}



            public static void NewLine()
            {
                Console.WriteLine();
            }

            public static void Info(String argn, params Object[] p)
            {
                ConsoleColor currentColor = Console.ForegroundColor;
                Console.ResetColor();
                if (!displayColor)
                {
                    Console.Write("{0} INFO ", getForMatTime(DateTime.Now));
                    if (!String.IsNullOrEmpty(subTitle))
                    {
                        Console.Write("[{0}] ", subTitle);
                    }
                    Console.WriteLine(argn, p);
                    Console.ForegroundColor = currentColor;
                    return;
                }
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("{0} ", getForMatTime(DateTime.Now));
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("INFO ");
                Console.ResetColor();
                if (!String.IsNullOrEmpty(subTitle))
                {
                    Console.Write("[");
                    Console.ForegroundColor = subTitleTextColor;
                    Console.Write(subTitle);
                    Console.ResetColor();
                    Console.Write("] ");
                }
                Console.ForegroundColor = (currentColor == ConsoleColor.Gray) ? infoTextColor : currentColor;
                Console.WriteLine(argn, p);
                Console.ForegroundColor = currentColor;
            }

            public static void Warn(String argn, params Object[] p)
            {
                ConsoleColor currentColor = Console.ForegroundColor;
                Console.ResetColor();
                if (!displayColor)
                {
                    Console.Write("{0} WARN ", getForMatTime(DateTime.Now));
                    if (!String.IsNullOrEmpty(subTitle))
                    {
                        Console.Write("[{0}] ", subTitle);
                    }
                    Console.WriteLine(argn, p);
                    Console.ForegroundColor = currentColor;
                    return;
                }
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("{0} ", getForMatTime(DateTime.Now));
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("WARN ");
                Console.ResetColor();
                if (!String.IsNullOrEmpty(subTitle))
                {
                    Console.Write("[");
                    Console.ForegroundColor = subTitleTextColor;
                    Console.Write(subTitle);
                    Console.ResetColor();
                    Console.Write("] ");
                }
                Console.ForegroundColor = warnTextColor;
                Console.WriteLine(argn, p);
                Console.ForegroundColor = currentColor;
            }
            public static void Error(String argn, params Object[] p)
            {
                ConsoleColor currentColor = Console.ForegroundColor;
                Console.ResetColor();
                if (!displayColor)
                {
                    Console.Write("{0} EROR ", getForMatTime(DateTime.Now));
                    if (!String.IsNullOrEmpty(subTitle))
                    {
                        Console.Write("[{0}] ", subTitle);
                    }
                    Console.WriteLine(argn, p);
                    Console.ForegroundColor = currentColor;
                    return;
                }
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("{0} ", getForMatTime(DateTime.Now));
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("EROR ");
                Console.ResetColor();
                if (!String.IsNullOrEmpty(subTitle))
                {
                    Console.Write("[");
                    Console.ForegroundColor = subTitleTextColor;
                    Console.Write(subTitle);
                    Console.ResetColor();
                    Console.Write("] ");
                }
                Console.ForegroundColor = errorTextColor;
                Console.WriteLine(argn, p);
                Console.ForegroundColor = currentColor;
            }
        }

        public class Logger
        {
            private ConsoleColor infoTextColor = ConsoleColor.White;
            private ConsoleColor warnTextColor = ConsoleColor.Yellow;
            private ConsoleColor errorTextColor = ConsoleColor.Red;
            private ConsoleColor subTitleTextColor = ConsoleColor.Gray;

            private ConsoleColor defaultTimestampColor = ConsoleColor.DarkBlue;

            private bool displayColor = true;
            private string subTitle = string.Empty;

            // Logger.SubTitle("我是副标题").Info("你好世界");
            // Logger.SetSubTitle("我是副标题");

            private int textColorCount = 0;
            private ConsoleColor nextTextColor = ConsoleColor.Gray;
            private int subTitleCount = 0;
            private string nextSubTitle = string.Empty;

            public Logger() { }

            public Logger(string subtitle)
            {
                subTitle = subtitle;
            }


            public Logger Copy()
            {
                return (Logger)this.MemberwiseClone();
            }

            string getForMatTime(DateTime t)
            {
                return string.Format("{0}:{1}:{2}:{3}",
                    t.Hour.ToString().PadLeft(2, '0'),
                    t.Minute.ToString().PadLeft(2, '0'),
                    t.Second.ToString().PadLeft(2, '0'),
                    t.Millisecond.ToString().PadLeft(3, '0'));
            }

            /// <summary>
            /// Info的输出文字颜色会被环境颜色覆盖,只有环境颜色是默认(Gray)时,该项才有效
            /// </summary>
            /// <param name="infoColor"></param>
            public void SetInfoTextColor(ConsoleColor infoColor)
            {
                infoTextColor = infoColor;
            }

            public void SetWarnTextColor(ConsoleColor warnColor)
            {
                warnTextColor = warnColor;
            }

            public void SetErrorTextColor(ConsoleColor errorColor)
            {
                errorTextColor = errorColor;
            }

            public void SetSubTitleTextColor(ConsoleColor subTitleColor)
            {
                subTitleTextColor = subTitleColor;
            }

            public void SetTimestampColor(ConsoleColor timestampColor)
            {
                defaultTimestampColor = timestampColor;
            }

            public void EnableColor(bool enable = true)
            {
                displayColor = enable;
            }

            /// <summary>
            /// 设置一个统一的副标题
            /// </summary>
            /// <param name="subtitle"></param>
            public void SetSubTitle(string subtitle)
            {
                subTitle = subtitle;
            }

            public Logger Color(ConsoleColor color)
            {
                //var t = this.Copy();
                this.textColorCount++;
                this.nextTextColor = color;
                return this;
            }

            public Logger SubTitle(string subtitle)
            {
                //var t = this.Copy();
                this.subTitleCount++;
                this.nextSubTitle = subtitle;
                return this;
            }


            public void NewLine()
            {
                Console.WriteLine();
            }

            public void Info(String argn, params Object[] p)
            {
                WriteLogFormat("INFO", defaultTimestampColor, ConsoleColor.Green, subTitleTextColor, infoTextColor, argn, p);
                return;
            }

            public void Warn(String argn, params Object[] p)
            {
                WriteLogFormat("WARN", defaultTimestampColor, ConsoleColor.Yellow, subTitleTextColor, warnTextColor, argn, p);
                return;
            }
            public void Error(String argn, params Object[] p)
            {
                WriteLogFormat("EROR", defaultTimestampColor, ConsoleColor.Red, subTitleTextColor, errorTextColor, argn, p);
                return;
            }


            private void WriteLogFormat(string logtype, ConsoleColor timestampColor, ConsoleColor logtypeColor, ConsoleColor subTitleColor, ConsoleColor textColor, string text, params Object[] p)
            {
                ConsoleColor currentColor = Console.ForegroundColor;
                Console.ResetColor();
                if (!displayColor)
                {
                    Console.Write("{0} {1} ", getForMatTime(DateTime.Now), logtype);
                    if (!String.IsNullOrEmpty(subTitle))
                    {
                        if (subTitleCount > 0)
                        {
                            subTitleCount--;
                            Console.Write("[{0}] ", nextSubTitle);
                        }
                        else
                        {
                            Console.Write("[{0}] ", subTitle);
                        }
                    }
                    Console.WriteLine(text, p);
                    Console.ForegroundColor = currentColor;
                    return;
                }
                Console.ForegroundColor = timestampColor;                    // 时间戳颜色
                Console.Write("{0} ", getForMatTime(DateTime.Now));
                Console.ForegroundColor = logtypeColor;                      // 日志类型标识颜色
                Console.Write("{0} ", logtype);
                Console.ResetColor();
                // 判断是否有副标题
                if (!String.IsNullOrEmpty(subTitle) || subTitleCount > 0)
                {
                    Console.Write("[");
                    Console.ForegroundColor = subTitleColor;
                    if (subTitleCount > 0)
                    {
                        subTitleCount--;
                        Console.Write(nextSubTitle);
                    }
                    else
                    {
                        Console.Write(subTitle);
                    }
                    Console.ResetColor();
                    Console.Write("] ");
                }
                // 单独设置文本颜色
                if (textColorCount > 0)
                {
                    textColorCount--;
                    Console.ForegroundColor = nextTextColor;
                }
                else
                {
                    Console.ForegroundColor = textColor;
                }
                Console.WriteLine(text, p);
                Console.ForegroundColor = currentColor;
            }
        }
    }
}
