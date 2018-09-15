using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Util;

namespace BitFlyerBuyAndSell
{
    class Program
    {
        // C:\Users\seigo\Desktop\Programing Test\BTCData\20180901\test20180901-114349.json
        static string dataDir = @"C:\Users\seigo\Desktop\Programing Test\BTCData";
        static string dateForm = "yyyyMMdd";

        static string positionDir = @"C:\Users\seigo\Desktop\Programing Test\BTCMyPosition";
        // index = timeStamp, isBuy, btcBuyPrice, btcAmount, isSold
        static string buySellHistoryPath = Path.Combine(positionDir, "BuySellHistory.csv");
        // index = timeStamp, CashAmount, btcAmount
        static string cashHistoryPath = Path.Combine(positionDir, "CashHistory.csv");

        static bool defaultBuying = true;
        static bool defaultSelling = true;
        static int isBuyIndex = 1;
        static int btcBuyPriceIndex = 2;
        static int btcAmountIndex = 3;
        static int isSoldIndex = 4;
        static int cashAmountIndex = 1;
        static int btcAmountForCashDataIndex = 2;
        static double buyUnit = 1000.0; // JPY

        static void Main(string[] args)
        {
            var buyAmountPerUnit = 1;
            var buying = bool.Parse(args[0]);
            var selling = bool.Parse(args[1]);
            var today = DateTime.Now;
            var btcData = GetLatestData(today);

            if (buying)
                RecordBuyAndCashHistory(buyAmountPerUnit, btcData.best_ask, btcData.timestamp);
            if (selling)
                RecordSellHistory(btcData.best_bid, btcData.timestamp);


        }

        static BTCData GetLatestData(DateTime today)
        {
            var latestFilePath = Directory.GetFiles(Path.Combine(dataDir, today.ToString(dateForm)), "*.json", SearchOption.AllDirectories)
                .OrderByDescending(file => (new DirectoryInfo(file)).CreationTime).FirstOrDefault();

            var res = new List<string>();
            var sr = new StreamReader(latestFilePath);
            var jsonStr = sr.ReadToEnd();
            sr.Close();
            var json = JsonConvert.DeserializeObject<BTCData>(jsonStr);
            return json;
        }

        static void RecordBuyAndCashHistory(int buyAmountPerUnit, double bestAsk, string timeStamp)
        {
            var btcAmount = buyAmountPerUnit * buyUnit / bestAsk;
            var paymentAmount = buyAmountPerUnit * buyUnit;

            var buySellData = CsvUtils.ReadCsv(buySellHistoryPath);
            var cashData = CsvUtils.ReadCsv(cashHistoryPath);

            var recordingBuyData = new List<string>() { timeStamp, true.ToString(),
                bestAsk.ToString(), btcAmount.ToString(), false.ToString()};
            buySellData.Add(recordingBuyData);

            var cashAmount = Double.Parse(cashData.Last()[cashAmountIndex]) - paymentAmount;
            var btcAmountForCashData = Double.Parse(cashData.Last()[btcAmountForCashDataIndex]) + btcAmount;
            var recordingCashData = new List<string>() { timeStamp, cashAmount.ToString(), btcAmountForCashData.ToString() };
            cashData.Add(recordingCashData);

            CsvUtils.WriteCsv(buySellData, buySellHistoryPath);
            CsvUtils.WriteCsv(cashData, cashHistoryPath);

        }
        static void RecordSellHistory(double bestBid, string timeStamp)
        {
            var buySellData = CsvUtils.ReadCsv(buySellHistoryPath);
            var cashData = CsvUtils.ReadCsv(cashHistoryPath);

            double totalSellBtcAmount = 0.0;
            foreach (var buyData in buySellData.GetRange(1, buySellData.Count()-1).Where(d =>
             bool.Parse(d[isBuyIndex]) == true && bool.Parse(d[isSoldIndex]) == false))
            {
                if (bestBid > Double.Parse(buyData[btcBuyPriceIndex]) * 1.1)
                {
                    totalSellBtcAmount += Double.Parse(buyData[btcAmountIndex]);
                    var dataIndex = buySellData.IndexOf(buyData);
                    buySellData[dataIndex][isSoldIndex] = true.ToString();
                }
            }
            if (totalSellBtcAmount > 1e-14)
            {

                var cashAmount = Double.Parse(cashData.Last()[cashAmountIndex]) + totalSellBtcAmount * bestBid;
                var btcAmountForCashData = Double.Parse(cashData.Last()[btcAmountForCashDataIndex]) - totalSellBtcAmount;

                var recordingBuyData = new List<string>() { timeStamp, false.ToString(),
                bestBid.ToString(), totalSellBtcAmount.ToString(), true.ToString()};
                buySellData.Add(recordingBuyData);

                var recordingCashData = new List<string>() { timeStamp, cashAmount.ToString(), btcAmountForCashData.ToString() };
                cashData.Add(recordingCashData);
            }
        }


    }
    public class BTCData
    {
        public string product_code { get; set; }
        // timestamp format is yyyy-MM-ddThh:mm:ss.sss
        public string timestamp { get; set; }
        public int tick_id { get; set; }
        public double best_bid { get; set; }
        public double best_ask { get; set; }
        public double best_bid_size { get; set; }
        public double best_ask_size { get; set; }
        public double best_bid_depth { get; set; }
        public double best_ask_depth { get; set; }
        public double ltp { get; set; }
        public double volume { get; set; }
        public double volume_by_product { get; set; }


    }
}
