using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace FlyerTrading
{
    class BoardDataUpdate
    {
        private static BoardData current_board;
        private static object lockobj = new object();

        public static void setCurrentBoard(BoardData d)
        {
            lock (lockobj)
                current_board = d;
        }
        public static BoardData getCurrentBoard()
        {
            lock (lockobj)
                return current_board;
        }


        public static async void startBoardUpdate()
        {
            initialize();

            await Task.Run(async () =>
            {
                while (SystemFlg.getMarketDataFlg())
                {
                    if (FlyerAPI2.getApiAccessProhibition() == false)
                    {
                        var board = await FlyerAPI2.getBoardAsync("FX_BTC_JPY");

                        if (board.MidPrice != 0)
                        {
                            board.dt = DateTime.Now;
                            var ask_p = board.Asks.Select(c => c.Price).ToArray();
                            var bid_p = board.Bids.Select(c => c.Price).ToArray();

                            var ask_min = ask_p.Min();
                            var bid_max = bid_p.Max();

                            board.spread = ask_min - bid_max;
                            setCurrentBoard(board);
                            MarketDataLog.addBoardData(board.dt, new double[] { bid_max, ask_min, board.spread });
                            Form1.Form1Instance.Invoke((Action)(() =>
                            {
                                Form1.Form1Instance.setLabel3(board.spread.ToString());
                            }));
                        }
                    }
                    //await Task.Delay(0);
                }
            });
        }

        private static void initialize()
        {
            current_board = new BoardData();
        }


        private static double calcBidAskDiff(double[] ask_p, double[] bid_p)
        {
            return ask_p.Min() - bid_p.Max();
        }
    }
}
