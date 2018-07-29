using System;
using System.Threading;

class TimerExample
{
    static void Main()
    {
        AutoResetEvent autoEvent = new AutoResetEvent(false);
        StatusChecker statusChecker = new StatusChecker();

        // Create the delegate that invokes methods for the timer.
        TimerCallback timerDelegate =
            new TimerCallback(statusChecker.CheckStatus);

        // タイマー起動
        Console.WriteLine("{0} タイマーを起動します.\n",
            DateTime.Now.ToString("h:mm:ss.fff"));

        Timer stateTimer =
        //最後の２つの値→初動実行までの時間,実行間隔(ミリ秒)b
                new Timer(timerDelegate, autoEvent, 0, 5000);

        // タイマーの待機時間は-1にすると無制限になる(有限にしたい場合はミリ秒で記述)
        autoEvent.WaitOne(60000, false);
        //タイマーの開放
        stateTimer.Dispose();
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