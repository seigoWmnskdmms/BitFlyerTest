using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Util
{
    public static class CsvUtils
    {
        private static char splitter = ',';
        public static List<List<string>> ReadCsv(string fullPath)
        {
            var res = new List<List<string>>();
            var sr = new StreamReader(fullPath);
            while (!sr.EndOfStream)
            {
                var values = sr.ReadLine().Split(splitter);
                res.Add(values.ToList());
            }
            sr.Close();
            return res;
        }

        public static void WriteCsv(List<List<string>> data, string outPath)
        {
            var sw = new StreamWriter(outPath);
            foreach(var line in data)
            {
                var joinedLine = String.Join(splitter.ToString(), line);
                sw.WriteLine(joinedLine);
            }
            sw.Close();
        }
    }
}
