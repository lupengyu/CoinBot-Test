using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bot_test.strategy
{
    /// <summary>
    ///  策略类
    /// </summary>
    class Strategy
    {
        /// <summary>
        ///  策略类型
        /// </summary>
        public String strategyname;
        /// <summary>
        ///  参数1
        /// </summary>
        public double p1 = 0;
        /// <summary>
        ///  参数2
        /// </summary>
        public double p2 = 0;
        /// <summary>
        ///  参数3
        /// </summary>
        public double p3 = 0;
        /// <summary>
        ///  参数4
        /// </summary>
        public double p4 = 0;
        /// <summary>
        ///  参数5
        /// </summary>
        public double p5 = 0;

        /// <summary>
        /// "Strategy"构造函数
        /// </summary>
        /// <param name="astrategyname"></param>
        /// <returns></returns>
        public Strategy(String astrategyname)
        {
            strategyname = astrategyname;
        }
    }
}
