using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using bot_test.Unit;
using bot_test.thread;
using bot_test.strategy;

namespace bot_test
{
    /// <summary>
    ///  “MainPage”主页面类
    /// </summary>
    public partial class MainPage : Form
    {
        /// <summary>
        ///  换行符
        /// </summary>
        private String CSRF = "\r\n";
        /// <summary>
        ///  运行主线程
        /// </summary>
        private MainThread thread;
        /// <summary>
        ///  日志文件写入器
        /// </summary>
        private StreamWriter fw = null;

        /// <summary>
        /// “MainPage”类构造函数
        /// </summary>
        /// <returns></returns>
        public MainPage()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        /// <summary>
        /// “开始”按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private void 开始_Click(object sender, EventArgs e)
        {
            String time = BotUnit.getLocalTime();
            int day = BotUnit.getDay();
            String filename = "log" + time + ".txt";
            filename = filename.Replace(" ", "");
            filename = filename.Replace(":", "");
            filename = filename.Replace("\\", "");
            filename = filename.Replace("/", "");
            FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fw = new StreamWriter(fs);
            try
            {
                double initCoin;
                double exchangeCoin;
                int minBetween;
                String symbol;
                String contractType;
                String type;
                Strategy strategy;
                {
                    if (!double.TryParse(初始币数.Text, out initCoin)
                    || !double.TryParse(单次交易.Text, out exchangeCoin))
                        throw new Exception("初始币数与单次交易必须为数字");
                    if(!int.TryParse(最短间隔.Text, out minBetween))
                        throw new Exception("最短间隔必须为数字");
                    if (MACD_choice.Checked)
                    {
                        double rate;
                        if (!double.TryParse(MACD_rate.Text, out rate))
                            throw new Exception("交易策略比率为必填");
                        strategy = new Strategy("MACD");
                        strategy.p1 = rate;
                    } else if(KDJ_choice.Checked)
                    {
                        double rate;
                        if (!double.TryParse(KDJ_rate.Text, out rate))
                            throw new Exception("交易策略比率为必填");
                        strategy = new Strategy("KDJ");
                        strategy.p1 = rate;
                    } else if(MA_choice.Checked)
                    {
                        int rate1, rate2;
                        if (!int.TryParse(MAsmall.Text, out rate1))
                            throw new Exception("交易策略比率为必填");
                        if (!int.TryParse(MAbig.Text, out rate2))
                            throw new Exception("交易策略比率为必填");
                        if(rate1 > 60 || rate2 > 60)
                            throw new Exception("MA策略参数过大，最大60");
                        strategy = new Strategy("MA");
                        strategy.p1 = rate1;
                        strategy.p1 = rate2;
                    } else
                    {
                        throw new Exception("交易策略必须选择");
                    }
                    if (BTC.Checked)
                    {
                        symbol = "btc_usd";
                    }
                    else if (LTC.Checked)
                    {
                        symbol = "ltc_usd";
                    }
                    else if (ETH.Checked)
                    {
                        symbol = "eth_usd";
                    }
                    else if (ETC.Checked)
                    {
                        symbol = "etc_usd";
                    }
                    else if (BCH.Checked)
                    {
                        symbol = "bch_usd";
                    }
                    else if (BTG.Checked)
                    {
                        symbol = "btg_usd";
                    }
                    else if (XRP.Checked)
                    {
                        symbol = "xrp_usd";
                    }
                    else if (EOS.Checked)
                    {
                        symbol = "eos_usd";
                    }
                    else
                    {
                        throw new Exception("请选择一个合约");
                    }
                    if(本周.Checked)
                    {
                        contractType = "this_week";
                    } else if(下周.Checked)
                    {
                        contractType = "next_week";
                    } else if(季度.Checked)
                    {
                        contractType = "quarter";
                    } else
                    {
                        throw new Exception("请选择一个合约期限");
                    }
                    if(一分钟.Checked)
                    {
                        type = "1min";
                    } else if(三分钟.Checked)
                    {
                        type = "3min";
                    } else if(五分钟.Checked)
                    {
                        type = "5min";
                    } else if(十五分钟.Checked)
                    {
                        type = "15min";
                    } else if(三十分钟.Checked)
                    {
                        type = "30min";
                    } else if(一小时.Checked)
                    {
                        type = "1hour";
                    } else if(两小时.Checked)
                    {
                        type = "2hour";
                    } else if(四小时.Checked)
                    {
                        type = "4hour";
                    } else if(六小时.Checked)
                    {
                        type = "6hour";
                    } else if(十二小时.Checked)
                    {
                        type = "12hour";
                    } else if(日.Checked)
                    {
                        type = "1day";
                    } else if(周.Checked)
                    {
                        type = "1week";
                    } else
                    {
                        throw new Exception("请选择时间梯度");
                    }
                }
                thread = new MainThread(this, initCoin, exchangeCoin, strategy, symbol, contractType, type, minBetween, time, day);
            } catch(Exception err)
            {
                交易信息_Add("系统启动失败");
                交易信息_Add(err.Message);
                return;
            }
            set策略选择Unable();
        }

        /// <summary>
        /// “结束”按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private void 结束_Click(object sender, EventArgs e)
        {
            thread.addSummary();
            thread.close();
            set策略选择Enable();
            fw.Close();
            fw = null;
        }

        /// <summary>
        /// 添加交易信息
        /// </summary>
        /// <param name="str">添加的信息内容</param>
        /// <returns></returns>
        public void 交易信息_Add(String str)
        {
            String add = BotUnit.getLocalTime() + ": " + str;
            交易信息.Text += add + CSRF;
            this.交易信息.Focus();
            this.交易信息.Select(this.交易信息.TextLength, 0);
            this.交易信息.ScrollToCaret();
            fw.WriteLine(add);
            fw.Flush();
        }

        /// <summary>
        /// 设置价格
        /// </summary>
        /// <param name="str">价格信息</param>
        /// <returns></returns>
        public void setPrice(String str)
        {
            价格.Text = str;
        }

        /// <summary>
        /// 设置日涨幅
        /// </summary>
        /// <param name="str">价格信息</param>
        /// <returns></returns>
        public void set日涨幅(String str)
        {
            日涨幅.Text = str;
        }

        /// <summary>
        /// 设置总金额
        /// </summary>
        /// <param name="str">价格信息</param>
        /// <returns></returns>
        public void set总金额(String str)
        {
            总金额.Text = str;
        }

        /// <summary>
        /// 设置初始金额
        /// </summary>
        /// <param name="str">价格信息</param>
        /// <returns></returns>
        public void set初始金额(String str)
        {
            初始金额.Text = str;
        }

        /// <summary>
        /// 设置币数目
        /// </summary>
        /// <param name="str">价格信息</param>
        /// <returns></returns>
        public void set币数目(String str)
        {
            币数目.Text = str;
        }

        /// <summary>
        /// 设置收益率
        /// </summary>
        /// <param name="str">价格信息</param>
        /// <returns></returns>
        public void set收益率(String str)
        {
            收益率.Text = str;
        }

        /// <summary>
        /// 设置MACD
        /// </summary>
        /// <param name="str">价格信息</param>
        /// <returns></returns>
        public void setMACD(String str)
        {
            MACD.Text = str;
        }

        /// <summary>
        /// 设置KDJ
        /// </summary>
        /// <param name="str">价格信息</param>
        /// <returns></returns>
        public void setKDJ(String str)
        {
            KDJ.Text = str;
        }

        /// <summary>
        /// 设置“策略选择”内所有组件不可修改
        /// </summary>
        /// <returns></returns>
        public void set策略选择Unable()
        {
            结束.Enabled = true;
            开始.Enabled = false;
            MACD_choice.Enabled = false;
            MACD_rate.Enabled = false;
            KDJ_choice.Enabled = false;
            KDJ_rate.Enabled = false;
            MA_choice.Enabled = false;
            MAsmall.Enabled = false;
            MAbig.Enabled = false;
            初始币数.Enabled = false;
            单次交易.Enabled = false;
            一分钟.Enabled = false;
            三分钟.Enabled = false;
            五分钟.Enabled = false;
            十五分钟.Enabled = false;
            三十分钟.Enabled = false;
            一小时.Enabled = false;
            两小时.Enabled = false;
            四小时.Enabled = false;
            六小时.Enabled = false;
            十二小时.Enabled = false;
            日.Enabled = false;
            本周.Enabled = false;
            季度.Enabled = false;
            下周.Enabled = false;
            BTC.Enabled = false;
            LTC.Enabled = false;
            ETH.Enabled = false;
            ETC.Enabled = false;
            BCH.Enabled = false;
            BTG.Enabled = false;
            XRP.Enabled = false;
            EOS.Enabled = false;
            最短间隔.Enabled = false;
        }

        /// <summary>
        /// 设置“策略选择”内所有组件可修改
        /// </summary>
        /// <returns></returns>
        public void set策略选择Enable()
        {
            结束.Enabled = false;
            开始.Enabled = true;
            MACD_choice.Enabled = true;
            MACD_rate.Enabled = true;
            KDJ_choice.Enabled = true;
            KDJ_rate.Enabled = true;
            MA_choice.Enabled = true;
            MAsmall.Enabled = true;
            MAbig.Enabled = true;
            初始币数.Enabled = true;
            单次交易.Enabled = true;
            一分钟.Enabled = true;
            三分钟.Enabled = true;
            五分钟.Enabled = true;
            十五分钟.Enabled = true;
            三十分钟.Enabled = true;
            一小时.Enabled = true;
            两小时.Enabled = true;
            四小时.Enabled = true;
            六小时.Enabled = true;
            十二小时.Enabled = true;
            日.Enabled = true;
            价格.Text = "";
            日涨幅.Text = "";
            MACD.Text = "";
            KDJ.Text = "";
            本周.Enabled = true;
            季度.Enabled = true;
            下周.Enabled = true;
            BTC.Enabled = true;
            LTC.Enabled = true;
            ETH.Enabled = true;
            ETC.Enabled = true;
            BCH.Enabled = true;
            BTG.Enabled = true;
            XRP.Enabled = true;
            EOS.Enabled = true;
            最短间隔.Enabled = true;
        }

        /// <summary>
        /// “MACD_choice”按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private void MACD_choice_CheckedChanged(object sender, EventArgs e)
        {
            MACD_rate.ReadOnly = false;
            KDJ_rate.ReadOnly = true;
            MAsmall.ReadOnly = true;
            MAbig.ReadOnly = true;
        }

        /// <summary>
        /// “KDJ_choice”按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private void KDJ_choice_CheckedChanged(object sender, EventArgs e)
        {
            MACD_rate.ReadOnly = true;
            KDJ_rate.ReadOnly = false;
            MAsmall.ReadOnly = true;
            MAbig.ReadOnly = true;
        }

        /// <summary>
        /// “MA_choice”按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private void MA_choice_CheckedChanged(object sender, EventArgs e)
        {
            MACD_rate.ReadOnly = true;
            KDJ_rate.ReadOnly = true;
            MAsmall.ReadOnly = false;
            MAbig.ReadOnly = false;
        }
    }
}