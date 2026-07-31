using MonitorKeyboard;
using System;

public class AutoSolver
{

	readonly KeyBoardManager keyBoard = KeyBoardManager.GetInstance();
	private int SleepBetweenClicks = 100; // milliseconds
	public CancellationTokenSource tokenCancel;

    async public void ClickFromString(string input)
	{
        //Split by input
        List<string> keys = new List<string>();
		keys.AddRange(input.Split(","));
		foreach (string key in keys)
		{
			try
			{
                if (key == "" || !KeyBoardManager.KeyBoardMap.ContainsKey(key))
                {
                    return;
                }

                keyBoard.ClickKey(KeyBoardManager.KeyBoardMap[key]);
                await Task.Delay(SleepBetweenClicks, tokenCancel.Token);
            }
			catch(OperationCanceledException) {
				return;
			}
            //Check if key exists in the dictionary

        }

	}
    public AutoSolver(CancellationTokenSource tokenCancel, int SleepBetweenClicks=100)
	{
        this.SleepBetweenClicks = SleepBetweenClicks;
        this.tokenCancel = tokenCancel ?? new CancellationTokenSource();
	}
}
