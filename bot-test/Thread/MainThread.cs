using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using bot_test.Unit;

namespace bot_test.Thread
{
    /// <summary>
    ///  “MainThread”主线程类
    /// </summary>
    class MainThread
    {
        /// <summary>
        ///  主界面类
        /// </summary>
        private MainPage page;
        /// <summary>
        ///  线程开始时间
        /// </summary>
        private String startTime;
        /// <summary>
        ///  初始币数
        /// </summary>
        private double initCoin;
        /// <summary>
        ///  单次交易币数
        /// </summary>
        private double exchangeCoin;
        /// <summary>
        ///  策略选择
        /// </summary>
        private double strategy;
        /// <summary>
        ///  交易策略比率
        /// </summary>
        private double rate;
        /// <summary>
        ///  当前币数
        /// </summary>
        private double Coin;
        /// <summary>
        ///  合约种类
        /// </summary>
        private String symbol;
        /// <summary>
        ///  合约期限
        /// </summary>
        private String contractType;

        /// <summary>
        /// "MainThread"构造函数
        /// </summary>
        /// <param name="apage">控制显示的主页面</param>
        /// <param name="ainitCoin">初始币数</param>
        /// <param name="aexchangeCoin">单次交易币数</param>
        /// <param name="astrategy">策略选择</param>
        /// <param name="arate">交易策略比率</param>
        /// <param name="asymbol">合约种类</param>
        /// <param name="acontractType">合约期限</param>
        /// <returns></returns>
        public MainThread(MainPage apage, double ainitCoin, double aexchangeCoin, 
            int astrategy, double arate,String asymbol, String acontractType)
        {
            this.page = apage;
            this.initCoin = ainitCoin;
            this.exchangeCoin = aexchangeCoin;
            this.strategy = astrategy;
            this.rate = arate;
            this.symbol = asymbol;
            this.contractType = acontractType;
            this.Coin = ainitCoin;
            startTime = BotUnit.getLocalTime();
            page.交易信息_Add("系统启动:" + startTime);
        }

        /// <summary>
        /// 关闭所有线程
        /// </summary>
        /// <returns></returns>
        public void close()
        {
            page.交易信息_Add("系统中止:" + BotUnit.getLocalTime());
        }

        /// <summary>
        /// 显示总结信息
        /// </summary>
        /// <returns></returns>
        public void addSummary()
        {
            page.交易信息_Add("总结报告:");
            page.交易信息_Add("####################");
            page.交易信息_Add("系统启动时间:" + startTime);
            page.交易信息_Add("初始币数:" + initCoin.ToString());
            page.交易信息_Add("当前币数:" + Coin.ToString());
            page.交易信息_Add("总收益率:" + getWinrate());
            page.交易信息_Add("####################");
        }

        /// <summary>
        /// 获得当前利润率
        /// </summary>
        /// <returns></returns>
        private String getWinrate()
        {
            double result = 100 * (Coin - initCoin) / initCoin;
            return result.ToString() + "%";
        }
    }
}