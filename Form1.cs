using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace FlyerTrading
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Form1Instance = this;
        }
        private static Form1 _form1Instance;
        public static Form1 Form1Instance
        {
            set { _form1Instance = value; }
            get { return _form1Instance; }
        }

        private void buttonMarketData_Click(object sender, EventArgs e)
        {
            SystemFlg.setMasterFlg(true);
            MarketData.startMarketData();
            FlyerAPI2.startFlyerAPIMonitoring();


        }

        private async void buttonGetINfo_Click(object sender, EventArgs e)
        {
            Form1Instance.initializeListBox();
            var res = await FlyerAPI2.getBalanceAsync();
            //var res2 = await FlyerAPI2.getBoardAsync("FX_BTC_JPY");
            //var res3 = await FlyerAPI2.getCollateralAsync();
            //var res4 = await FlyerAPI2.getParentOrderAsync();
            //var res5 = await FlyerAPI2.getChildOrderAsync("ACTIVE");
            //var res6 = await FlyerAPI2.getPositionsAsync();
            //var res7 = await FlyerAPI2.getExecutionsAsync();
            //var res8 = await FlyerAPI2.cancelAllChildOrdersAsync();
            foreach (var v in res)
                Form1Instance.addListBox(v.currency_code + "- amount=" + v.amount + ", avaialble=" + v.available);
        }

        private async void buttonGetActiveOrders_Click(object sender, EventArgs e)
        {
            Form1Instance.initializeListBox();
            var res5 = await FlyerAPI2.getChildOrderAsync("ACTIVE");
            foreach (var v in res5)
                Form1Instance.addListBox(v.child_order_date + " - " + v.side + " - " + v.price + "*" + v.size + " - " + v.child_order_acceptance_id);
        }

        private async void buttonCancelOrder_Click(object sender, EventArgs e)
        {
            /*
            var res8 = await FlyerAPI2.cancelAllChildOrdersAsync();
            Form1Instance.addListBox(res8);
            */
            
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var r = await FlyerAPI2.cancelChildOrdersAsync(id);
            addListBox2(r);
            sw.Stop();
            addListBox2("time=" + sw.ElapsedMilliseconds);
        }

        public string id = "";
        private async void buttonSendOrder_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var res = await FlyerAPI2.sendChiledOrderAsync("BUY", 600000, 0.01, 1);
            sw.Stop();
            id = res.order_id;
            Form1Instance.addListBox2(res.order_id + ":time="+sw.ElapsedMilliseconds);
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
           // var res = DBManager.getAllBoardData();
           // foreach (var v in res)
           //     addListBox(v.Key.ToLongTimeString()+" : bid=" + v.Value[0]+", ask="+v.Value[1]+", spread="+v.Value[2]);

            var exe = DBManager.getAllExecutions();
            foreach(var v in exe)
                addListBox2("id="+v.id+", side="+v.side+", price="+v.price +", size="+v.size+", date="+v.exec_date+", id="+v.buy_child_order_acceptance_id+" : "+ v.sell_child_order_acceptance_id);
        }

        private async void buttonBoardUpdate_Click(object sender, EventArgs e)
        {
            //await BoardDataUpdate.startBoardUpdate();
        }


        private async void buttonExportFromDB_Click(object sender, EventArgs e)
        {
            await Task.Run(async () =>
            {
                this.Invoke((Action)(() =>
                {
                    setLabel("exporting board data...");
                }));
                var res = DBManager.getAllBoardData();
                using(StreamWriter sw = new StreamWriter("./board data.csv",false, Encoding.Default))
                {
                    sw.WriteLine("datetime,bid,ask,spread");
                    foreach (var v in res)
                        sw.WriteLine(v.Key.ToString("yyyy:MM:dd:HH:mm:ss:fff") + "," + v.Value[0] + "," + v.Value[1] + "," + v.Value[2]);
                }
                this.Invoke((Action)(() =>
                {
                    setLabel("exporting executions data...");
                }));
                using (StreamWriter sw = new StreamWriter("./executions.csv", false, Encoding.Default))
                {
                    sw.WriteLine("id,datetime,side,price,size,buy_child_order_acceptance_id,sell_child_order_acceptance_id");
                    var res2 = DBManager.getAllExecutions();
                    foreach (var v in res2)
                        sw.WriteLine(v.id+","+v.exec_date.ToString("yyyy:MM:dd:HH:mm:ss:fff")+","+v.side+","+v.price+","+v.size+","+v.buy_child_order_acceptance_id+","+v.sell_child_order_acceptance_id);
                 }
                this.Invoke((Action)(() =>
                {
                    setLabel("Completed data exporte");
                }));
            });
            
        }

        private void buttonStartMasterThread_Click(object sender, EventArgs e)
        {
            MasterThread.startMasterThread();
            MarketData.startMarketData();
            FlyerAPI2.startFlyerAPIMonitoring();
        }

        private void buttonStopMasterThread_Click(object sender, EventArgs e)
        {
            MasterThread.finishMasterThread();
        }

        #region Delegate
        private delegate void setLabel1Delegate(string text);
        public void setLabel(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new setLabel1Delegate(setLabel), text);
                return;
            }
            this.label1.Text = text;
        }

        private delegate void setLabel2Delegate(string text);
        public void setLabel2(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new setLabel2Delegate(setLabel2), text);
                return;
            }
            this.label2.Text = text;
        }

        private delegate void setLabel3Delegate(string text);
        public void setLabel3(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new setLabel3Delegate(setLabel3), text);
                return;
            }
            this.label3.Text = text;
        }

        private delegate void setLabel4Delegate(string text);
        public void setLabel4(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new setLabel4Delegate(setLabel4), text);
                return;
            }
            this.label4.Text = text;
        }

        private delegate void setLabel5Delegate(string text);
        public void setLabel5(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new setLabel5Delegate(setLabel5), text);
                return;
            }
            this.label5.Text = text;
        }

        private delegate void setLabel6Delegate(string text);
        public void setLabel6(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new setLabel6Delegate(setLabel6), text);
                return;
            }
            this.label6.Text = text;
        }

        private delegate void setLabel7Delegate(string text);
        public void setLabel7(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new setLabel7Delegate(setLabel7), text);
                return;
            }
            this.label7.Text = text;
        }

        private delegate void setLabel8Delegate(string text);
        public void setLabel8(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new setLabel8Delegate(setLabel8), text);
                return;
            }
            this.label8.Text = text;
        }
        private delegate void setLabel9Delegate(string text);
        public void setLabel9(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new setLabel9Delegate(setLabel9), text);
                return;
            }
            this.label9.Text = text;
        }

        private delegate void setLabel10Delegate(string text);
        public void setLabel10(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new setLabel10Delegate(setLabel10), text);
                return;
            }
            this.label10.Text = text;
        }

        private delegate void setLabel11Delegate(string text);
        public void setLabel11(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new setLabel11Delegate(setLabel11), text);
                return;
            }
            this.label11.Text = text;
        }

        private delegate void setLabel12Delegate(string text);
        public void setLabel12(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new setLabel12Delegate(setLabel12), text);
                return;
            }
            this.label12.Text = text;
        }

        private delegate void addListBoxDelegate(string text);
        public void addListBox(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new addListBoxDelegate(addListBox), text);
                return;
            }
            this.listBox1.Items.Add(text);
            this.listBox1.TopIndex = this.listBox1.Items.Count - 1;
        }

        private delegate void initializeListBoxDelegate();
        public void initializeListBox()
        {
            if (InvokeRequired)
            {
                Invoke(new initializeListBoxDelegate(initializeListBox));
                return;
            }
            this.listBox1.Items.Clear();
        }

        private delegate void addListBox2Delegate(string text);
        public void addListBox2(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new addListBox2Delegate(addListBox2), text);
                return;
            }
            this.listBox2.Items.Add(text);
            this.listBox2.TopIndex = this.listBox2.Items.Count - 1;
        }

        private delegate void initializeListBox2Delegate();
        public void initializeListBox2()
        {
            if (InvokeRequired)
            {
                Invoke(new initializeListBox2Delegate(initializeListBox2));
                return;
            }
            this.listBox2.Items.Clear();
        }

        private delegate void initializeListBox3Delegate();
        public void initializeListBox3()
        {
            if (InvokeRequired)
            {
                Invoke(new initializeListBox3Delegate(initializeListBox3));
                return;
            }
            this.listBox3.Items.Clear();
        }

        private delegate void addListBox3Delegate(string text);
        public void addListBox3(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new addListBox3Delegate(addListBox3), text);
                return;
            }
            this.listBox3.Items.Add(text);
            this.listBox3.TopIndex = this.listBox3.Items.Count - 1;
        }

        private delegate void setTextBox1Delegate(string text);
        public void setTextBox1(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new setTextBox1Delegate(setTextBox1));
                return;
            }
            this.textBox1.Text = text;
        }

        private delegate void setDataGridviewDelegate(double[] bid_p, double[] bid_s, double[] ask_p, double[] ask_s);
        public void setDataGridView(double[] bid_p, double[] bid_s, double[] ask_p, double[] ask_s)
        {
            if(InvokeRequired)
            {
                Invoke(new setDataGridviewDelegate(setDataGridView));
                return;
            }
            this.dataGridView1.Rows.Clear();
            for(int i=0; i<bid_p.Length; i++)
                this.dataGridView1.Rows.Add(bid_p[i], bid_s[i],"","");
            for(int i=0; i<ask_p.Length; i++)
                this.dataGridView1.Rows.Add("", "", ask_p[i], ask_s[i]);
        }





        #endregion

        
    }
}
