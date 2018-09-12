namespace FlyerTrading
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonMarketData = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonGetINfo = new System.Windows.Forms.Button();
            this.buttonSendOrder = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonCancelOrder = new System.Windows.Forms.Button();
            this.buttonGetActiveOrders = new System.Windows.Forms.Button();
            this.listBox3 = new System.Windows.Forms.ListBox();
            this.buttonTest = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ColumnBidPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnBidSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnAskPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnAskSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonBoardUpdate = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.buttonExportFromDB = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.buttonStartMasterThread = new System.Windows.Forms.Button();
            this.buttonStopMasterThread = new System.Windows.Forms.Button();
            this.buttonGetExecutions = new System.Windows.Forms.Button();
            this.buttonGetPositions = new System.Windows.Forms.Button();
            this.buttonGetOrders = new System.Windows.Forms.Button();
            this.buttonStartMMBot = new System.Windows.Forms.Button();
            this.buttonStopMMBot = new System.Windows.Forms.Button();
            this.buttonGetExecutionsId = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonMarketData
            // 
            this.buttonMarketData.Location = new System.Drawing.Point(12, 172);
            this.buttonMarketData.Name = "buttonMarketData";
            this.buttonMarketData.Size = new System.Drawing.Size(179, 75);
            this.buttonMarketData.TabIndex = 0;
            this.buttonMarketData.Text = "MarketData";
            this.buttonMarketData.UseVisualStyleBackColor = true;
            this.buttonMarketData.Click += new System.EventHandler(this.buttonMarketData_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 24);
            this.label2.TabIndex = 2;
            this.label2.Text = "label2";
            // 
            // buttonGetINfo
            // 
            this.buttonGetINfo.Location = new System.Drawing.Point(236, 172);
            this.buttonGetINfo.Name = "buttonGetINfo";
            this.buttonGetINfo.Size = new System.Drawing.Size(179, 75);
            this.buttonGetINfo.TabIndex = 3;
            this.buttonGetINfo.Text = "buttonGetInfo";
            this.buttonGetINfo.UseVisualStyleBackColor = true;
            this.buttonGetINfo.Click += new System.EventHandler(this.buttonGetINfo_Click);
            // 
            // buttonSendOrder
            // 
            this.buttonSendOrder.Location = new System.Drawing.Point(470, 172);
            this.buttonSendOrder.Name = "buttonSendOrder";
            this.buttonSendOrder.Size = new System.Drawing.Size(179, 75);
            this.buttonSendOrder.TabIndex = 4;
            this.buttonSendOrder.Text = "send order";
            this.buttonSendOrder.UseVisualStyleBackColor = true;
            this.buttonSendOrder.Click += new System.EventHandler(this.buttonSendOrder_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.HorizontalScrollbar = true;
            this.listBox1.ItemHeight = 24;
            this.listBox1.Location = new System.Drawing.Point(12, 359);
            this.listBox1.Name = "listBox1";
            this.listBox1.ScrollAlwaysVisible = true;
            this.listBox1.Size = new System.Drawing.Size(785, 604);
            this.listBox1.TabIndex = 5;
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.HorizontalScrollbar = true;
            this.listBox2.ItemHeight = 24;
            this.listBox2.Location = new System.Drawing.Point(812, 359);
            this.listBox2.Name = "listBox2";
            this.listBox2.ScrollAlwaysVisible = true;
            this.listBox2.Size = new System.Drawing.Size(916, 604);
            this.listBox2.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 125);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 24);
            this.label3.TabIndex = 7;
            this.label3.Text = "label3";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(2171, 19);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(311, 334);
            this.textBox1.TabIndex = 9;
            // 
            // buttonCancelOrder
            // 
            this.buttonCancelOrder.Location = new System.Drawing.Point(691, 172);
            this.buttonCancelOrder.Name = "buttonCancelOrder";
            this.buttonCancelOrder.Size = new System.Drawing.Size(179, 75);
            this.buttonCancelOrder.TabIndex = 10;
            this.buttonCancelOrder.Text = "cancel order";
            this.buttonCancelOrder.UseVisualStyleBackColor = true;
            this.buttonCancelOrder.Click += new System.EventHandler(this.buttonCancelOrder_Click);
            // 
            // buttonGetActiveOrders
            // 
            this.buttonGetActiveOrders.Location = new System.Drawing.Point(12, 262);
            this.buttonGetActiveOrders.Name = "buttonGetActiveOrders";
            this.buttonGetActiveOrders.Size = new System.Drawing.Size(179, 75);
            this.buttonGetActiveOrders.TabIndex = 11;
            this.buttonGetActiveOrders.Text = "get active orders";
            this.buttonGetActiveOrders.UseVisualStyleBackColor = true;
            this.buttonGetActiveOrders.Click += new System.EventHandler(this.buttonGetActiveOrders_Click);
            // 
            // listBox3
            // 
            this.listBox3.FormattingEnabled = true;
            this.listBox3.HorizontalScrollbar = true;
            this.listBox3.ItemHeight = 24;
            this.listBox3.Location = new System.Drawing.Point(1779, 359);
            this.listBox3.Name = "listBox3";
            this.listBox3.ScrollAlwaysVisible = true;
            this.listBox3.Size = new System.Drawing.Size(916, 604);
            this.listBox3.TabIndex = 12;
            // 
            // buttonTest
            // 
            this.buttonTest.Location = new System.Drawing.Point(2533, 22);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(179, 75);
            this.buttonTest.TabIndex = 13;
            this.buttonTest.Text = "test";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnBidPrice,
            this.ColumnBidSize,
            this.ColumnAskPrice,
            this.ColumnAskSize});
            this.dataGridView1.Location = new System.Drawing.Point(12, 998);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 33;
            this.dataGridView1.Size = new System.Drawing.Size(1088, 524);
            this.dataGridView1.TabIndex = 14;
            // 
            // ColumnBidPrice
            // 
            this.ColumnBidPrice.HeaderText = "BidPrice";
            this.ColumnBidPrice.Name = "ColumnBidPrice";
            // 
            // ColumnBidSize
            // 
            this.ColumnBidSize.HeaderText = "Size";
            this.ColumnBidSize.Name = "ColumnBidSize";
            // 
            // ColumnAskPrice
            // 
            this.ColumnAskPrice.HeaderText = "AskPrice";
            this.ColumnAskPrice.Name = "ColumnAskPrice";
            // 
            // ColumnAskSize
            // 
            this.ColumnAskSize.HeaderText = "Size";
            this.ColumnAskSize.Name = "ColumnAskSize";
            // 
            // buttonBoardUpdate
            // 
            this.buttonBoardUpdate.Location = new System.Drawing.Point(236, 262);
            this.buttonBoardUpdate.Name = "buttonBoardUpdate";
            this.buttonBoardUpdate.Size = new System.Drawing.Size(179, 75);
            this.buttonBoardUpdate.TabIndex = 15;
            this.buttonBoardUpdate.Text = "Board update";
            this.buttonBoardUpdate.UseVisualStyleBackColor = true;
            this.buttonBoardUpdate.Click += new System.EventHandler(this.buttonBoardUpdate_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(615, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 24);
            this.label4.TabIndex = 16;
            this.label4.Text = "label4";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(615, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 24);
            this.label5.TabIndex = 17;
            this.label5.Text = "label5";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(615, 125);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 24);
            this.label6.TabIndex = 18;
            this.label6.Text = "label6";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(1178, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 24);
            this.label7.TabIndex = 19;
            this.label7.Text = "label7";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(1178, 74);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(67, 24);
            this.label8.TabIndex = 20;
            this.label8.Text = "label8";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(1178, 125);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 24);
            this.label9.TabIndex = 21;
            this.label9.Text = "label9";
            // 
            // buttonExportFromDB
            // 
            this.buttonExportFromDB.Location = new System.Drawing.Point(470, 262);
            this.buttonExportFromDB.Name = "buttonExportFromDB";
            this.buttonExportFromDB.Size = new System.Drawing.Size(179, 75);
            this.buttonExportFromDB.TabIndex = 22;
            this.buttonExportFromDB.Text = "export db data";
            this.buttonExportFromDB.UseVisualStyleBackColor = true;
            this.buttonExportFromDB.Click += new System.EventHandler(this.buttonExportFromDB_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(1702, 122);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(79, 24);
            this.label10.TabIndex = 25;
            this.label10.Text = "label10";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(1702, 71);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(79, 24);
            this.label11.TabIndex = 24;
            this.label11.Text = "label11";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(1702, 19);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(79, 24);
            this.label12.TabIndex = 23;
            this.label12.Text = "label12";
            // 
            // buttonStartMasterThread
            // 
            this.buttonStartMasterThread.Location = new System.Drawing.Point(969, 172);
            this.buttonStartMasterThread.Name = "buttonStartMasterThread";
            this.buttonStartMasterThread.Size = new System.Drawing.Size(179, 75);
            this.buttonStartMasterThread.TabIndex = 26;
            this.buttonStartMasterThread.Text = "start master";
            this.buttonStartMasterThread.UseVisualStyleBackColor = true;
            this.buttonStartMasterThread.Click += new System.EventHandler(this.buttonStartMasterThread_Click);
            // 
            // buttonStopMasterThread
            // 
            this.buttonStopMasterThread.Location = new System.Drawing.Point(969, 262);
            this.buttonStopMasterThread.Name = "buttonStopMasterThread";
            this.buttonStopMasterThread.Size = new System.Drawing.Size(179, 75);
            this.buttonStopMasterThread.TabIndex = 27;
            this.buttonStopMasterThread.Text = "stop master";
            this.buttonStopMasterThread.UseVisualStyleBackColor = true;
            this.buttonStopMasterThread.Click += new System.EventHandler(this.buttonStopMasterThread_Click);
            // 
            // buttonGetExecutions
            // 
            this.buttonGetExecutions.Location = new System.Drawing.Point(1168, 998);
            this.buttonGetExecutions.Name = "buttonGetExecutions";
            this.buttonGetExecutions.Size = new System.Drawing.Size(230, 33);
            this.buttonGetExecutions.TabIndex = 28;
            this.buttonGetExecutions.Text = "get executions";
            this.buttonGetExecutions.UseVisualStyleBackColor = true;
            this.buttonGetExecutions.Click += new System.EventHandler(this.buttonGetExecutions_Click);
            // 
            // buttonGetPositions
            // 
            this.buttonGetPositions.Location = new System.Drawing.Point(1168, 1060);
            this.buttonGetPositions.Name = "buttonGetPositions";
            this.buttonGetPositions.Size = new System.Drawing.Size(230, 33);
            this.buttonGetPositions.TabIndex = 29;
            this.buttonGetPositions.Text = "get positions";
            this.buttonGetPositions.UseVisualStyleBackColor = true;
            this.buttonGetPositions.Click += new System.EventHandler(this.buttonGetPositions_Click);
            // 
            // buttonGetOrders
            // 
            this.buttonGetOrders.Location = new System.Drawing.Point(1168, 1117);
            this.buttonGetOrders.Name = "buttonGetOrders";
            this.buttonGetOrders.Size = new System.Drawing.Size(230, 33);
            this.buttonGetOrders.TabIndex = 30;
            this.buttonGetOrders.Text = "get orders";
            this.buttonGetOrders.UseVisualStyleBackColor = true;
            this.buttonGetOrders.Click += new System.EventHandler(this.buttonGetOrders_Click);
            // 
            // buttonStartMMBot
            // 
            this.buttonStartMMBot.Location = new System.Drawing.Point(1377, 179);
            this.buttonStartMMBot.Name = "buttonStartMMBot";
            this.buttonStartMMBot.Size = new System.Drawing.Size(179, 75);
            this.buttonStartMMBot.TabIndex = 31;
            this.buttonStartMMBot.Text = "start MMBot";
            this.buttonStartMMBot.UseVisualStyleBackColor = true;
            this.buttonStartMMBot.Click += new System.EventHandler(this.buttonStartMMBot_Click);
            // 
            // buttonStopMMBot
            // 
            this.buttonStopMMBot.Location = new System.Drawing.Point(1377, 262);
            this.buttonStopMMBot.Name = "buttonStopMMBot";
            this.buttonStopMMBot.Size = new System.Drawing.Size(179, 75);
            this.buttonStopMMBot.TabIndex = 32;
            this.buttonStopMMBot.Text = "stop MMBot";
            this.buttonStopMMBot.UseVisualStyleBackColor = true;
            this.buttonStopMMBot.Click += new System.EventHandler(this.buttonStopMMBot_Click);
            // 
            // buttonGetExecutionsId
            // 
            this.buttonGetExecutionsId.Location = new System.Drawing.Point(1168, 1180);
            this.buttonGetExecutionsId.Name = "buttonGetExecutionsId";
            this.buttonGetExecutionsId.Size = new System.Drawing.Size(230, 33);
            this.buttonGetExecutionsId.TabIndex = 33;
            this.buttonGetExecutionsId.Text = "get id exe";
            this.buttonGetExecutionsId.UseVisualStyleBackColor = true;
            this.buttonGetExecutionsId.Click += new System.EventHandler(this.buttonGetExecutionsId_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(2740, 1534);
            this.Controls.Add(this.buttonGetExecutionsId);
            this.Controls.Add(this.buttonStopMMBot);
            this.Controls.Add(this.buttonStartMMBot);
            this.Controls.Add(this.buttonGetOrders);
            this.Controls.Add(this.buttonGetPositions);
            this.Controls.Add(this.buttonGetExecutions);
            this.Controls.Add(this.buttonStopMasterThread);
            this.Controls.Add(this.buttonStartMasterThread);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.buttonExportFromDB);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonBoardUpdate);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.buttonTest);
            this.Controls.Add(this.listBox3);
            this.Controls.Add(this.buttonGetActiveOrders);
            this.Controls.Add(this.buttonCancelOrder);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.buttonSendOrder);
            this.Controls.Add(this.buttonGetINfo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonMarketData);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonMarketData;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonGetINfo;
        private System.Windows.Forms.Button buttonSendOrder;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonCancelOrder;
        private System.Windows.Forms.Button buttonGetActiveOrders;
        private System.Windows.Forms.ListBox listBox3;
        private System.Windows.Forms.Button buttonTest;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnBidPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnBidSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnAskPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnAskSize;
        private System.Windows.Forms.Button buttonBoardUpdate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button buttonExportFromDB;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button buttonStartMasterThread;
        private System.Windows.Forms.Button buttonStopMasterThread;
        private System.Windows.Forms.Button buttonGetExecutions;
        private System.Windows.Forms.Button buttonGetPositions;
        private System.Windows.Forms.Button buttonGetOrders;
        private System.Windows.Forms.Button buttonStartMMBot;
        private System.Windows.Forms.Button buttonStopMMBot;
        private System.Windows.Forms.Button buttonGetExecutionsId;
    }
}

