using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;

namespace DataEditor
{
    class Program
    {
        static List<List<string>> allResult;
        static List<List<string>> dataList;
        static string separator = ",";
        static void Main(string[] args)
        {
            #region ReadAndWriteCsv
            var dates = new List<string>() { "20180728", "20180729" };
            //var dates = new List<string>() { "20180728", "20180729" };
            var dirPath = @"C:\Users\seigo\Desktop\Programing Test\BTCData";
            foreach (var date in dates)
            {
                var files = Directory.GetFiles(Path.Combine(dirPath, date), "*.json", SearchOption.AllDirectories);
                allResult = new List<List<string>>();
                var index = new List<string> { "date", "time", "best_bid", "best_ask" };
                //allResult.Add(index);
                foreach (var file in files)
                {
                    ReadAndWriteData(Path.Combine(dirPath, file));
                }
                var outFileName = "BTC" + ".csv";
                var sw = new StreamWriter(Path.Combine(dirPath, outFileName));
                sw.WriteLine(String.Join(separator, index));
                allResult.ForEach(res => sw.WriteLine(String.Join(separator, res)));
                sw.Close();
            }
            #endregion

            #region analyze
            var timeIndex = 1;
            var bidIndex = 2;
            var askIndex = 3;
            var time = allResult.Select(res => res[timeIndex]).ToArray();
            var bid = allResult.Select(res => int.Parse(res[bidIndex])).ToArray();
            var ask = allResult.Select(res => int.Parse(res[askIndex])).ToArray();

            var pair = new List<Tuple<string, int,string, int,int>>();
            for(int i = 0; i< ask.Count(); i++)
            {
                var bidLevel = ask[i] * 1.004;
                for(int j = i + 1; j < bid.Count(); j++)
                {
                    if(bid[j] > bidLevel)
                    {
                        pair.Add(new Tuple<string, int, string, int, int>(time[i], ask[i], time[j], bid[j], bid[j] - ask[i]));
                    }
                }
            }
            var a = 0;
            #endregion

        }

        static void ReadAndWriteData(string fullFilePath)
        {
            var res = new List<string>();
            var sr = new StreamReader(fullFilePath);
            var jsonStr = sr.ReadToEnd();
            sr.Close();
            var json = JsonConvert.DeserializeObject<BTCData>(jsonStr);
            var dateTime = TimeStampConverter(json.timestamp);
            res.Add(dateTime.ToShortDateString());
            res.Add(dateTime.ToString("HH:mm:ss"));
            res.Add(json.best_bid.ToString());
            res.Add(json.best_ask.ToString());
            allResult.Add(res);
        }

        static DateTime TimeStampConverter(string timeStamp)
        {
            var pattarn = @"(?<year>\d{4})-(?<month>\d{2})-(?<day>\d{2})T(?<hour>\d{2}):(?<min>\d{2}):(?<sec>\d{2})";
            var regex = new Regex(pattarn);
            var match = regex.Match(timeStamp);
            var day = int.Parse(match.Groups["day"].Value);
            var hour = int.Parse(match.Groups["hour"].Value);
            if (hour + 9 > 23)
            {
                hour = hour + 9 - 23;
                day += 1;
            }
            else
                hour += 9;
            return new DateTime(int.Parse(match.Groups["year"].Value), int.Parse(match.Groups["month"].Value),
                day, hour,
                int.Parse(match.Groups["min"].Value), int.Parse(match.Groups["sec"].Value));
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
