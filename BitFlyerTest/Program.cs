using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;

class BitFlyerTest
{
    static readonly Uri endpointUri = new Uri("https://api.bitflyer.jp");

    public static void Main()
    {
        AutoResetEvent autoEvent = new AutoResetEvent(false);
        var process = Process.GetCurrentProcess();
        process.ProcessorAffinity = (IntPtr)1;
        var timerDeleg = new TimerCallback(DoGetting);
        Console.WriteLine("{0} Timer start.\n",
            DateTime.Now.ToString("h:mm:ss.fff"));
        Timer stateTimer =
               //最後の２つの値→初動実行までの時間,実行間隔(ミリ秒)b
               new Timer(timerDeleg, autoEvent, 0, 60000);

        // タイマーの待機時間は-1にすると無制限になる(有限にしたい場合はミリ秒で記述)
        autoEvent.WaitOne(-1, false);
        //タイマーの開放
        stateTimer.Dispose();
        

        Console.WriteLine("end");

    }

    public static async Task GetTick()
    {
        var method = "GET";
        var path = "/v1/getticker";
        var query = "";

        using (var client = new HttpClient())
        using (var request = new HttpRequestMessage(new HttpMethod(method), path + query))
        {
            client.BaseAddress = endpointUri;
            var message = await client.SendAsync(request);
            var response = await message.Content.ReadAsStringAsync();
            if (response != null)
            {
                var parsedResponse = JsonConvert.DeserializeObject(response);
                parsedResponse = JsonConvert.SerializeObject(parsedResponse, Formatting.Indented);
                var now = DateTime.Now;
                var time = now.ToString("yyyyMMdd-HHmmss"); //"yyyyMMdd-HHmm-ss-ff"
                var timeForDir = now.ToString("yyyyMMdd");
                string dirName = Path.Combine(@"C:\Users\seigo\Desktop\Programing Test\BTCData", timeForDir);
                var fileName = "test" + time + ".json";
                var sw = new StreamWriter(Path.Combine(dirName, fileName));
                sw.WriteLine(parsedResponse.ToString());
                sw.Close();
                Console.WriteLine("success at " + time + ".");
                //File.WriteAllText(@"C:\Users\seigo\Desktop\Programing Test\BitFlyerTest\test02.json", parsedResponse.ToString());
            }
            else
                Console.WriteLine("error: " + response);
            
        }
        GC.Collect();
    }

    public static void DoGetting(object stateInfo)
    {
        AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;

        //ここに一定時間で実行したい処理を書く
        Console.WriteLine("{0} Start GetTick {0}.",
               DateTime.Now.ToString("h:mm:ss.fff"));
        var task = GetTick();
        Console.WriteLine("{0} End GetTick {0}.",
               DateTime.Now.ToString("h:mm:ss.fff"));
        Thread.Sleep(50000);
        while (true) ;
    }
}

class StatusChecker
{
    public StatusChecker()
    {
    }

    // This method is called by the timer delegate.
    public void CheckStatus(Object stateInfo)
    {
        AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;

        //ここに一定時間で実行したい処理を書く
        Console.WriteLine("{0} Checking status {0}.",
            DateTime.Now.ToString("h:mm:ss.fff"));
    }
}