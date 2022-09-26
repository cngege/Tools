using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    namespace Logger
    {
        public class Logger
        {
            private static ConsoleColor infoTextColor = ConsoleColor.White;
            private static ConsoleColor warnTextColor = ConsoleColor.Yellow;
            private static ConsoleColor errorTextColor = ConsoleColor.Red;

            private static bool displayColor = true;

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

            public static void EnableColor(bool enable = true)
            {
                displayColor = enable;
            }

            public static void Info(String argn, params Object[] p)
            {
                ConsoleColor currentColor = Console.ForegroundColor;
                Console.ResetColor();
                if (!displayColor)
                {
                    Console.Write("[{0} INFO] ", getForMatTime(DateTime.Now));
                    Console.WriteLine(argn, p);
                    Console.ForegroundColor = currentColor;
                    return;
                }
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("{0} ", getForMatTime(DateTime.Now));
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("INFO");
                Console.ResetColor();
                Console.Write("] ");
                Console.ForegroundColor = (currentColor == ConsoleColor.Gray) ? infoTextColor : currentColor;
                Console.WriteLine(argn, p);
            }

            public static void Warn(String argn, params Object[] p)
            {
                ConsoleColor currentColor = Console.ForegroundColor;
                Console.ResetColor();
                if (!displayColor)
                {
                    Console.Write("[{0} WARN] ", getForMatTime(DateTime.Now));
                    Console.WriteLine(argn, p);
                    Console.ForegroundColor = currentColor;
                    return;
                }
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("{0} ", getForMatTime(DateTime.Now));
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("WARN");
                Console.ResetColor();
                Console.Write("] ");
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
                    Console.Write("[{0} EROR] ", getForMatTime(DateTime.Now));
                    Console.WriteLine(argn, p);
                    Console.ForegroundColor = currentColor;
                    return;
                }
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("{0} ", getForMatTime(DateTime.Now));
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("EROR");
                Console.ResetColor();
                Console.Write("] ");
                Console.ForegroundColor = errorTextColor;
                Console.WriteLine(argn, p);
                Console.ForegroundColor = currentColor;
            }
        }
    }
}
