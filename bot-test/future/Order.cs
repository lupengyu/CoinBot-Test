using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bot_test.future
{
    /// <summary>
    ///  买入订单类
    /// </summary>
    class Order
    {
        /// <summary>
        ///  买入价格
        /// </summary>
        private double buynum;
        /// <summary>
        ///  抵押金
        /// </summary>
        private double cost;
        /// <summary>
        ///  成交判断
        /// </summary>
        public bool judge = false;

        /// <summary>
        /// "Order"构造函数
        /// </summary>
        /// <param name="acost"></param>
        /// <param name="abuynum"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public Order(double acost, double abuynum, MainPage page)
        {
            this.cost = acost;
            this.buynum = abuynum;
            page.交易信息_Add("新的买入请求, 价格:" + abuynum.ToString());
        }

        /// <summary>
        /// 获得买入价格
        /// </summary>
        /// <returns></returns>
        public double getbuyprice()
        {
            return buynum;
        }

        /// <summary>
        /// 获得抵押金数目
        /// </summary>
        /// <returns></returns>
        public double getcoin()
        {
            return cost;
        }
    }
}
