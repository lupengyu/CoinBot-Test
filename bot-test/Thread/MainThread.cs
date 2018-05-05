using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using bot_test.Unit;
using com.okcoin.rest.future;
using com.okcoin.rest.stock;
using bot_test.strategy;
using bot_test.future;

namespace bot_test.thread
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
        private Strategy strategy;
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
        ///  时间梯度
        /// </summary>
        private String type;
        /// <summary>
        ///  主线程
        /// </summary>
        private Thread mainthread;
        /// <summary>
        ///  okexapi-get方法类
        /// </summary>
        private FutureRestApiV1 getRequest;
        /// <summary>
        ///  最新价格时间
        /// </summary>
        private String newTime;
        /// <summary>
        ///  当日开价
        /// </summary>
        private double dayBegin = 0;
        /// <summary>
        ///  日期
        /// </summary>
        private int day;
        /// <summary>
        ///  最短间隔
        /// </summary>
        private int minBetween;
        /// <summary>
        ///  时间计数
        /// </summary>
        private int timecount = 0;
        /// <summary>
        ///  当前订单
        /// </summary>
        private Order order = null;
        /// <summary>
        ///  当前卖出请求
        /// </summary>
        private SellOrder sellorder = null;
        /// <summary>
        ///  当前买一价
        /// </summary>
        private double buy1 = 0;
        /// <summary>
        ///  当前卖一价
        /// </summary>
        private double sell1 = 0;
        /// <summary>
        ///  总币数
        /// </summary>
        private double coinsum = 0;
        /// <summary>
        ///  订单等待时间
        /// </summary>
        private int wait = -1;

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
            Strategy astrategy, String asymbol, String acontractType, String atype, int aminBetween)
        {
            this.page = apage;
            this.initCoin = ainitCoin;
            this.exchangeCoin = aexchangeCoin;
            this.strategy = astrategy;
            this.symbol = asymbol;
            this.contractType = acontractType;
            this.Coin = ainitCoin;
            coinsum = Coin;
            this.type = atype;
            this.minBetween = aminBetween;
            this.timecount = aminBetween;
            startTime = BotUnit.getLocalTime();
            day = BotUnit.getDay();
            String url_prex = "https://www.okex.cn";
            this.getRequest = new FutureRestApiV1(url_prex);
            mainthread = new Thread(new ThreadStart(run));
            mainthread.IsBackground = true;
            mainthread.Start();
            ThreadPool.QueueUserWorkItem(dayrefrash, null);
            page.交易信息_Add("系统启动:" + startTime);
            page.set初始金额(initCoin.ToString());
            page.set币数目(Coin.ToString());
            page.set收益率(getWinrate());
        }

        /// <summary>
        /// 分割json字符串
        /// </summary>
        /// <param name="str">json字符串</param>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        private String jsonGetKey(String str, String key)
        {
            int start = str.IndexOf(key) + key.Length + 1;
            int end = 0;
            for (int i = start; i < str.Length; i++)
            {
                if (str[i] != '"' && str[i] != ',' && str[i] != ':')
                {
                    start = i;
                    break;
                }
            }
            for (int i = start + 1; i < str.Length; i++)
            {
                if (str[i] == '"' || str[i] == ',')
                {
                    end = i - 1;
                    break;
                }
            }
            return str.Substring(start, end - start + 1);
        }

        /// <summary>
        /// 获取最新价格
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /*
        * {"date":"1525446875",
           "ticker":{
               "high":20.944,
               "vol":1.96523876E8,
               "day_high":0,
               "last":19.072,
               "low":18.62,
               "contract_id":201806290200057,
               "buy":19.072,
               "sell":19.077,
               "coin_vol":0,
               "day_low":0,
               "unit_amount":10
           }}
        */
        private void changePrice(object data)
        {
            try
            {
                String result = getRequest.future_ticker(symbol, contractType);
                //result = result.Replace("\"", "'");
                String time = jsonGetKey(result, "date");
                if(String.Compare(time, newTime) == 1)
                {
                    newTime = time;
                    String last = jsonGetKey(result, "last");
                    String buy = jsonGetKey(result, "buy");
                    String sell = jsonGetKey(result, "sell");
                    page.setPrice(last);
                    double price;
                    double buyprice;
                    double sellprice;
                    double.TryParse(last, out price);
                    double.TryParse(buy, out buyprice);
                    double.TryParse(sell, out sellprice);
                    buy1 = buyprice;
                    sell1 = sellprice;
                    if (dayBegin != 0)
                    {
                        double zhangfu = 100 * (price - dayBegin) / dayBegin;
                        page.set日涨幅(zhangfu.ToString() + "%");
                    }
                    coinsum = Coin;
                    if (order != null)
                    {
                        if(order.judge == false)
                        {
                            if(price <= order.getbuyprice())
                            {
                                order.judge = true;
                                page.交易信息_Add("买入订单交易成功");
                                Coin -= order.getcoin() * 1.003;
                                coinsum -= order.getcoin() * 0.003;
                                timecount = 0;
                            } else
                            {
                                if(wait == 60)
                                {
                                    page.交易信息_Add("买入订单超时");
                                    order = null;
                                }
                            }
                        } else
                        {   // 当前有持仓
                            if (-((price - order.getbuyprice()) * 10 / price) > order.getcoin())
                            {
                                page.交易信息_Add("你的仓位已爆仓");
                            }
                            else
                            {
                                if (sellorder != null)
                                {
                                    if (sellorder.judge == false)
                                    {
                                        if (price >= sellorder.getsellprice())
                                        {
                                            sellorder.judge = true;
                                            page.交易信息_Add("卖出订单交易成功");
                                            Coin += ((sellorder.getsellprice() - sellorder.getprice()) * 10 / sellorder.getsellprice()) * 0.995;
                                            coinsum = Coin;
                                            timecount = 0;
                                            order = null;
                                            sellorder = null;
                                        }
                                        else
                                        {
                                            if (wait == 60)
                                            {
                                                page.交易信息_Add("卖出订单超时");
                                                sell = null;
                                            }
                                            coinsum += order.getcoin();
                                            coinsum += (price - order.getbuyprice()) * 10 / price;
                                        }
                                    }
                                }
                                else
                                {
                                    coinsum += order.getcoin();
                                    coinsum += (price - order.getbuyprice()) * 10 / price;
                                }
                            }
                        }
                    }
                    double nowMoney = coinsum * price;
                    page.set币数目(coinsum.ToString());
                    page.set总金额(nowMoney.ToString());
                    page.set收益率(getWinrate());
                }
            } catch(Exception e)
            {
                return;
            }
        }

        /// <summary>
        /// 获取当前相关参数
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private void getP(object data)
        {
            String result = getRequest.future_kline(symbol, type, contractType, "60", "");
            String[] sArray = result.Split(',');//开高低收
            double EMA12 = 0;
            double EMA26 = 0;
            double DIF = 0;
            double DEA = 0;
            double nowPrice = 0;
            double K = 50;
            double D = 50;
            double RSV = 0;
            for (int i = 4; i < sArray.Length; i += 7)
            {
                double price;
                double high;
                double low;
                double.TryParse(sArray[i], out price);
                double.TryParse(sArray[i - 2], out high);
                double.TryParse(sArray[i - 1], out low);
                nowPrice = price;
                EMA12 = (EMA12 * 11) / 13 + price * 2 / 13;
                EMA26 = (EMA26 * 25) / 27 + price * 2 / 27;
                DIF = EMA12 - EMA26;
                DEA = (DEA * 8) / 10 + DIF * 2 / 10;
                RSV = (price - low) * 100 / (high - low);
                K = 2 * K / 3 + RSV / 3;
                D = 2 * D / 3 + K / 3;
            }
            double MACD = (DIF - DEA) * 2;
            double J = 3 * K - 2 * D;
            page.setMACD(MACD.ToString());
            String kdj = "K:" + K.ToString().Substring(0, 2) + " D:" + D.ToString().Substring(0, 2) + " J:" + J.ToString().Substring(0, 2);
            page.setKDJ(kdj);
            if(strategy.strategyname == "MACD")
            {
                if(MACD > strategy.p1)
                {//买入
                    if(order == null)
                    {
                        if(minBetween == timecount)
                        {
                            if(sell1 != 0)
                            {
                                if(Coin >= exchangeCoin)
                                {
                                    order = new Order(exchangeCoin, sell1, page);
                                    wait = 0;
                                }
                            }
                        }
                    }
                } else
                {//卖出
                    if(order != null)
                    {
                        if(order.judge == false)
                        {
                            order = null;
                            page.交易信息_Add("买入订单删除");
                            return;
                        }
                        if (minBetween == timecount)
                        {
                            if(buy1 != 0)
                            {
                                sellorder = new SellOrder(order.getcoin(), order.getbuyprice(), buy1, page);
                                wait = 0;
                            }
                        }
                    }
                }
            } else if(strategy.strategyname == "KDJ")
            {
                if (D - K > strategy.p1)
                {//买入
                    if (order == null)
                    {
                        if (minBetween == timecount)
                        {
                            if (sell1 != 0)
                            {
                                if (Coin >= exchangeCoin)
                                {
                                    order = new Order(exchangeCoin, sell1, page);
                                    wait = 0;
                                }
                            }
                        }
                    }
                }
                else
                {//卖出
                    if (order != null)
                    {
                        if (order.judge == false)
                        {
                            order = null;
                            page.交易信息_Add("买入订单删除");
                            return;
                        }
                        if (minBetween == timecount)
                        {
                            if (buy1 != 0)
                            {
                                sellorder = new SellOrder(order.getcoin(), order.getbuyprice(), buy1, page);
                                wait = 0;
                            }
                        }
                    }
                }
            } else
            {
                int masmall = (int)strategy.p1;
                int masbig = (int)strategy.p2;
                double sum1 = 0, sum2 = 0;
                int index = 0;
                for(int i = 417;index < masmall ;index++)
                {
                    double price;
                    double.TryParse(sArray[i], out price);
                    sum1 += price;
                    i -= 7;
                }
                index = 0;
                for (int i = 417; index < masbig; index++)
                {
                    double price;
                    double.TryParse(sArray[i], out price);
                    sum2 += price;
                    i -= 7;
                }
                double ma1 = sum1 / masmall;
                double ma2 = sum2 / masbig;
                if(ma1 > ma2)
                {
                    //买入
                    if (order == null)
                    {
                        if (minBetween == timecount)
                        {
                            if (sell1 != 0)
                            {
                                if (Coin >= exchangeCoin)
                                {
                                    order = new Order(exchangeCoin, sell1, page);
                                    wait = 0;
                                }
                            }
                        }
                    }
                } else
                {
                    //卖出
                    if (order != null)
                    {
                        if (order.judge == false)
                        {
                            order = null;
                            page.交易信息_Add("买入订单删除");
                            return;
                        }
                        if (minBetween == timecount)
                        {
                            if (buy1 != 0)
                            {
                                sellorder = new SellOrder(order.getcoin(), order.getbuyprice(), buy1, page);
                                wait = 0;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取当前价格函数相关参数
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private void getPrice(object data)
        {
            ThreadPool.QueueUserWorkItem(changePrice, null);
            ThreadPool.QueueUserWorkItem(getP, null);
        }

        /// <summary>
        /// 更新当天数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private void dayrefrash(object data)
        {
            day = BotUnit.getDay();
            addSummary();
            String result = getRequest.future_kline(symbol, "1day", contractType, "1", "");
            String[] sArray = result.Split(',');
            double.TryParse(sArray[1], out dayBegin);
        }

        /// <summary>
        /// 主线程运行函数，每秒执行一次查询
        /// </summary>
        /// <returns></returns>
        private void run()
        {
            while (true)
            {   
                ThreadPool.QueueUserWorkItem(getPrice, null);
                if(day != BotUnit.getDay())
                {
                    ThreadPool.QueueUserWorkItem(dayrefrash, null);
                }
                if(timecount != minBetween)
                {
                    timecount++;
                }
                if(wait != -1 && wait != 60)
                {
                    wait++;
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 关闭所有线程
        /// </summary>
        /// <returns></returns>
        public void close()
        {
            mainthread.Abort();
            page.交易信息_Add("系统中止:" + BotUnit.getLocalTime());
        }

        /// <summary>
        /// 显示总结信息
        /// </summary>
        /// <returns></returns>
        public void addSummary()
        {
            page.交易信息_Add("####################");
            page.交易信息_Add("总结报告:");
            page.交易信息_Add("当前时间:" + BotUnit.getLocalTime());
            page.交易信息_Add("初始币数:" + initCoin.ToString());
            page.交易信息_Add("当前币数:" + coinsum.ToString());
            page.交易信息_Add("总收益率:" + getWinrate());
            page.交易信息_Add("####################");
        }

        /// <summary>
        /// 获得当前利润率
        /// </summary>
        /// <returns></returns>
        private String getWinrate()
        {
            double result = 100 * (coinsum - initCoin) / initCoin;
            return result.ToString() + "%";
        }
    }
}