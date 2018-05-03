using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using bot_test.Unit;
using bot_test.Thread;

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
        /// “MainPage”类构造函数
        /// </summary>
        /// <returns></returns>
        public MainPage()
        {
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
            try
            {
                double initCoin;
                double exchangeCoin;
                double rate;
                int strategy;
                String symbol;
                String contractType;
                {
                    if (!double.TryParse(初始币数.Text, out initCoin)
                    || !double.TryParse(单次交易.Text, out exchangeCoin))
                        throw new Exception("初始币数与单次交易必须为数字");
                    if (!MACD_choice.Checked && !KDJ_choice.Checked)
                    {
                        throw new Exception("交易策略必须选择");
                    }
                    else if (MACD_choice.Checked)
                    {
                        strategy = 1;
                        if (!double.TryParse(MACD_rate.Text, out rate))
                            throw new Exception("交易策略比率为必填");
                    }
                    else
                    {
                        strategy = 2;
                        if (!double.TryParse(KDJ_rate.Text, out rate))
                            throw new Exception("交易策略比率为必填");
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
                }
                thread = new MainThread(this, initCoin, exchangeCoin, strategy, rate, symbol, contractType);
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
        }

        /// <summary>
        /// 添加交易信息
        /// </summary>
        /// <param name="str">添加的信息内容</param>
        /// <returns></returns>
        public void 交易信息_Add(String str)
        {
            交易信息.Text += str + CSRF;
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
            Check.Enabled = false;
            初始币数.Enabled = false;
            单次交易.Enabled = false;
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
            Check.Enabled = true;
            初始币数.Enabled = true;
            单次交易.Enabled = true;
        }
    }
}