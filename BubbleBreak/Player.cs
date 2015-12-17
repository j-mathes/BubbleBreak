using System;
using System.IO;
using System.Collections.Generic;
using Foundation;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace BubbleBreak
{
	public class Player
	{
		public string Name { get; set; }
		public int LastLevelCompleted { get; set; }
		public int Coins { get; set; }
		public int HighScore { get; set; }
		public int TapStrength { get; set; }
		public List<Sequences> UnlockedSequences { get; set; }

		NSUrl docDir;
		public string PlayerDataFile { get; }

		// Track player boosts that are turned on / off
		// Track player achievements
		// Track purchased power-ups
		// Track level branches unlocked

		public Player ()
		{
			Name = "New Player";
			LastLevelCompleted = -1;
			Coins = 0;
			HighScore = 0;
			TapStrength = 1;
			UnlockedSequences = new List<Sequences>();
			UnlockedSequences.Add (Sequences.Linear);

			docDir = NSFileManager.DefaultManager.GetUrls (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User) [0];
			PlayerDataFile = docDir.AbsoluteUrl.RelativePath + "/playerdata.xml";
		}

		public static Player ReadData(Player currentPlayer)
		{
			XmlSerializer mySerializer = new XmlSerializer (typeof(Player));
			using (FileStream fs = new FileStream (currentPlayer.PlayerDataFile, FileMode.OpenOrCreate)) {
				return (Player)mySerializer.Deserialize (fs);
			}
		}

		public void WriteData()
		{
			XmlSerializer mySerializer = new XmlSerializer (typeof(Player));
			using (StreamWriter myWriter = new StreamWriter (PlayerDataFile, false)) {
				mySerializer.Serialize (myWriter, this);
			}
		}
	}
}

