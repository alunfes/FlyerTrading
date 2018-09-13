using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace FlyerTrading
{
    class MarketData
    {
        public static void startMarketData()
        {
            SystemFlg.setMarketDataFlg(true);
            MarketDataLog.initialize();
            //BoardDataUpdate.getCurrentBoard();
            BoardDataUpdate.startBoardUpdate();
            marketDataThread();
            DBWriter.startDBWriter();
            //System.Diagnostics.Debug.WriteLine("kita2");
        }

        private async static void marketDataThread()
        {
            var api = new API();
            //var api2 = new API();
            var api3 = new API();

            api.Subscribe<Tick>(api.TickerFxBtcJpy, OnReceive, OnConnect, OnError);
            //api2.Subscribe<BoardDiff>(api.boardFxBtcJpy, OnReceiveBoard, OnConnectBoard, OnError);
            api3.Subscribe<List<Executions>>(api.ExecutionsFxBtcJpy, OnReceiveExecutions, OnConnectExecutions, OnError);

            while (SystemFlg.getMarketDataFlg())
            {
                await Task.Delay(10);
                //Thread.Sleep(10);
                Form1.Form1Instance.Invoke((Action)(() =>
                {
                    Form1.Form1Instance.setLabel("num executions log=" + MarketDataLog.getNumExecutionsLog().ToString());
                    Form1.Form1Instance.setLabel2("num board data log=" + MarketDataLog.getNumBoardData().ToString());
                }));
            }
        }
        
        private static void OnConnect(string message)
        {
            //take log
        }

        private static void OnReceive(Tick data)
        {
            MarketDataLog.addTickData(data);
            //Form1.Form1Instance.addListBox2(data.Timestamp + ", price=" + data.LatestPrice + ", vol=" + data.Volume + ", vol product=" + data.VolumeByProduct);
        }

        private static void OnError(string message, Exception ex)
        {
            Form1.Form1Instance.setLabel3(message);
            if (ex != null)
            {
                Form1.Form1Instance.setLabel2(ex.Message);
            }
        }

        private static void OnConnectBoard(string message)
        {
            //take log
        }

        private static void OnReceiveBoard(BoardDiff data)
        {
            //if (BoardDataUpdate.getCurrentBoard().MidPrice  > 0)
            {
                var current_board = BoardDataUpdate.getCurrentBoard();
                current_board.dt = DateTime.Now;
                var ask_min = current_board.Asks.Select(x => x.Price).ToList().Min();
                var bid_max = current_board.Bids.Select(x => x.Price).ToList().Max();

                foreach (var v in data.Asks)
                {
                    if (v.Price < ask_min)
                        ask_min = v.Price; break;
                }
                foreach (var v in data.Bids)
                {
                    if (v.Price > bid_max)
                        bid_max = v.Price; break;
                }

                current_board.Asks[0].Price = ask_min;
                current_board.Bids[0].Price = bid_max;
                current_board.spread = ask_min - bid_max;
                MarketDataLog.addBoardData(current_board.dt, new double[] { bid_max, ask_min, current_board.spread});
                BoardDataUpdate.setCurrentBoard(current_board);
                Form1.Form1Instance.setLabel3(current_board.spread.ToString());
            }

            /*
            foreach (var v in data.Asks)
                line += v.Price + " x " + v.Size;
            Form1.Form1Instance.addListBox(data.MidPrice+" : " + line);
            */
        }
        

        private static void OnConnectExecutions(string message)
        {
            //take log
            Form1.Form1Instance.setLabel("executions channel connected");
        }

        private static void OnReceiveExecutions(List<Executions> data)
        {
            MarketDataLog.addExecutionsData(data);
            foreach(var v in data)
                Form1.Form1Instance.addListBox3(v.exec_date+", id="+v.id+", side=" + v.side+", price=" +v.price+", size="+v.size);
        }

        private static void OnErrorExecutions(string message, Exception ex)
        {
            Form1.Form1Instance.setLabel3(message);
            if (ex != null)
            {
                Form1.Form1Instance.setLabel3(ex.Message);
            }
        }
    }
}
