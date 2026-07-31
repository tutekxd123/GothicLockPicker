using MonitorKeyboard;
using System;

public class AutoSolver
{

	readonly KeyBoardManager keyBoard = KeyBoardManager.GetInstance();
	private int SleepBetweenClicks = 100; // milliseconds
	public CancellationTokenSource tokenCancel;
    bool isRunning = false;
    async public void ClickFromString(string input)
	{
        //Split by input
        if (this.isRunning)
        {
            return;
        }
        this.isRunning = true;
        List<string> keys = new List<string>();
		keys.AddRange(input.Split(","));
		foreach (string key in keys)
		{
			try
			{
                if (key == "" || !KeyBoardManager.KeyBoardMap.ContainsKey(key))
                {
                    this.isRunning = false;
                    return;
                }

                keyBoard.ClickKey(KeyBoardManager.KeyBoardMap[key]);
                await Task.Delay(SleepBetweenClicks, tokenCancel.Token);
            }
			catch(OperationCanceledException) {
                this.isRunning = false;
				return;
			}
            //Check if key exists in the dictionary

        }
        this.isRunning = false;

    }
    public AutoSolver(CancellationTokenSource tokenCancel, int SleepBetweenClicks=100)
	{
        this.SleepBetweenClicks = SleepBetweenClicks;
        this.tokenCancel = tokenCancel ?? new CancellationTokenSource();
	}
}
