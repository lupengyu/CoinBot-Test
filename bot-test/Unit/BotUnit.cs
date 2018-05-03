using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bot_test.Unit
{
    /// <summary>
    ///  “BotUnit”帮助工具类
    /// </summary>
    class BotUnit
    {
        /// <summary>
        /// 获取当前系统时间
        /// </summary>
        /// <returns></returns>
        public static String getLocalTime()
        {
            return DateTime.Now.ToString();
        }
    }
}