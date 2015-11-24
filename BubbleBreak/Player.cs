using System;

namespace BubbleBreak
{
	public class Player
	{
		public string Name { get; set; }
		public int LastLevelCompleted { get; set; }
		public int Coins { get; set; }
		public int HighScore { get; set; }

		// Track player boosts that are turned on / off
		// Track player achievements
		// Track purchased power-ups
		// Track level branches unlocked

		public Player ()
		{
			Name = "TestPlayer";
			LastLevelCompleted = -1;
			Coins = 0;
			HighScore = 0;
		}
	}
}

