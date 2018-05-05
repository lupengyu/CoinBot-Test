using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bot_test.future
{
    /// <summary>
    ///  卖出订单类
    /// </summary>
    class SellOrder
    {
        /// <summary>
        ///  保证金
        /// </summary>
        private double coin;
        /// <summary>
        ///  买入价格
        /// </summary>
        private double price;
        /// <summary>
        ///  卖出价格
        /// </summary>
        private double sellprice;
        /// <summary>
        ///  成交判断
        /// </summary>
        public bool judge = false;

        /// <summary>
        /// "SellOrder"构造函数
        /// </summary>
        /// <param name="acoin"></param>
        /// <param name="aprice"></param>
        /// <param name="asellprice"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public SellOrder(double acoin, double aprice, double asellprice, MainPage page)
        {
            this.coin = acoin;
            this.price = aprice;
            this.sellprice = asellprice;
            page.交易信息_Add("新的卖出请求, 价格:" + sellprice.ToString());
        }

        /// <summary>
        /// 获得抵押金数目
        /// </summary>
        /// <returns></returns>
        public double getcoin()
        {
            return coin;
        }

        /// <summary>
        /// 获得买入价格
        /// </summary>
        /// <returns></returns>
        public double getprice()
        {
            return price;
        }

        /// <summary>
        /// 获得卖出价格
        /// </summary>
        /// <returns></returns>
        public double getsellprice()
        {
            return sellprice;
        }
    }
}
