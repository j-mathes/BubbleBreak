//---------------------------------------------------------------------------------------------
// <copyright file="Player.cs" company="RetroTek Software Ltd">
//     Copyright (C) 2016 RetroTek Software Ltd. All rights reserved.
// </copyright>
// <author>Jared Mathes</author>
//---------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Foundation;

namespace BubbleBreak
{
	public class Player
	{
		public string Name { get; set; }
		public BranchType ActiveBranch { get; set; } // added
		//public int LastLevelCompleted { get; set; } // existing but needs to change?
		//public int NextLevel { get; set; } // added
		public int Coins { get; set; }
		public float CoinCarryOver { get; set; }
		public int PersistentTapStrength { get; set; }
		public int TempTapStrength { get; set; }
		public List<Sequences> UnlockedSequences { get; set; }
		public int ChanceToRollNextSeq { get; set; }
		public int ChanceToRollBonus { get; set; }
		public int LuckyPointChance { get; set; } // chance popping a bubble will result in a critical point pop
		public int LuckyPointBonus { get; set; } // maximum additional point multiplier  during a critical point pop
		public int ConsumableAdd { get; set; }
		public int ConsumableSubtract { get; set; }
		public int ConsumableCheckPoint { get; set; }
		public int ConsumableNextSeq { get; set; }
		public int PersistentTimeBonus { get; set; }
		public int Persistent2xTimeBonus { get; set; }
		public int Persistent3xTimeBonus { get; set; }
		public int MedKitUpgradeLevel { get; set; }
		public bool IsMedKitActivated { get; set; }
		public bool HighlightNextPurchased { get; set; }
		public bool IsHighlightNextActive { get; set; }
		public List<ShopUnlocks> UnlockedShopItems { get; set; } // added
		public List<BranchType> UnlockedBranches { get; set; } // added
		public List<Bonuses> UnlockedBonusBubbles { get; set; } // added
		public List<BranchProgress> BranchProgression { get; set; }  // track player's progression throught the various branches
		public PlayerSlot SlotPosition { get; set; }
		public bool HasStarted { get; set; } // used to determine if player info and delete player icons should show on StartLayer
		public int TotalBubblesPopped { get; set; } // for the sake of stats
		public int TotalCoinsCollected { get; set; } // for the sake of stats
		public int TotalGamesPlayed { get; set; }
		public int TotalScores { get; set; } // total of all scores ever awarded

		NSUrl docDir;
		public string PlayerDataFile { get; set; }

		// Track player boosts that are turned on / off
		// Track player achievements
		// Track level branches unlocked

		public Player (PlayerSlot slotPosition)
		{
			SlotPosition = slotPosition;
			//LastLevelCompleted = -1;
			Coins = 0;
			CoinCarryOver = 0;
			TotalScores = 0;
			PersistentTapStrength = 1;
			UnlockedSequences = new List<Sequences>();
			UnlockedSequences.Add (Sequences.Linear);
			ChanceToRollNextSeq = 1;
			ChanceToRollBonus = 0;
			LuckyPointChance = 0;
			LuckyPointBonus = 1;
			UnlockedShopItems = new List<ShopUnlocks> ();
			UnlockedBranches = new List<BranchType> ();
			UnlockedBranches.Add (BranchType.Tutorial); 
			ActiveBranch = BranchType.Tutorial;
			MedKitUpgradeLevel = 0;
			IsMedKitActivated = false;

			BranchProgression = new List<BranchProgress> ();

			var values = Enum.GetValues(typeof(BranchType));
			foreach (BranchType branch in values) {
				BranchProgression.Add (new BranchProgress {
					BranchIsType = branch, 
					IsLocked = true,
					BranchState = CompletionState.notStarted,
					LastLevelCompleted = -1,
				});
			}

			BranchProgression.Find (x => x.BranchIsType == BranchType.Tutorial).IsLocked = false;

			docDir = NSFileManager.DefaultManager.GetUrls (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User) [0];

			switch (SlotPosition) {
			case PlayerSlot.slot1:
				PlayerDataFile = docDir.AbsoluteUrl.RelativePath + "/player1data.xml";
				Name = "Player 1";
				break;

			case PlayerSlot.slot2:
				PlayerDataFile = docDir.AbsoluteUrl.RelativePath + "/player2data.xml";
				Name = "Player 2";
				break;

			case PlayerSlot.slot3:
				PlayerDataFile = docDir.AbsoluteUrl.RelativePath + "/player3data.xml";
				Name = "Player 3";
				break;
			}


		}

		// Serializing needs a parameterless constructor.

		public Player ()
		{
			Name = "New Player";
			//LastLevelCompleted = -1;
			Coins = 0;
			CoinCarryOver = 0;
			TotalScores = 0;
			PersistentTapStrength = 1;
			UnlockedSequences = new List<Sequences>();
			//UnlockedSequences.Add (Sequences.Linear);  // to prevent doubling up of list entries
			ChanceToRollNextSeq = 1;
			ChanceToRollBonus = 1;
			LuckyPointChance = 0;
			LuckyPointBonus = 1;
			UnlockedShopItems = new List<ShopUnlocks> ();
			UnlockedBranches = new List<BranchType> ();
			//UnlockedBranches.Add (BranchType.Tutorial); 
			ActiveBranch = BranchType.Tutorial;
			MedKitUpgradeLevel = 0;
			IsMedKitActivated = false;

			BranchProgression = new List<BranchProgress> ();

//			var values = Enum.GetValues(typeof(BranchType));
//			foreach (BranchType branch in values) {
//				BranchProgression.Add (new BranchProgress {
//					BranchIsType = branch, 
//					IsLocked = true,
//					BranchState = CompletionState.notStarted,
//					LastLevelCompleted = -1,
//				});
//			}
//
//			BranchProgression.Find (x => x.BranchIsType == BranchType.Tutorial).IsLocked = false;

			docDir = NSFileManager.DefaultManager.GetUrls (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User) [0];
			PlayerDataFile = docDir.AbsoluteUrl.RelativePath + "/playerdata.xml";
		}

		public static Player ReadData(Player currentPlayer)
		{
			var mySerializer = new XmlSerializer (typeof(Player));
			using (var fs = new FileStream (currentPlayer.PlayerDataFile, FileMode.OpenOrCreate)) {
				return (Player)mySerializer.Deserialize (fs);
			}
		}

		public static Player ReadData(string datafile)
		{
			var mySerializer = new XmlSerializer (typeof(Player));
			using (var fs = new FileStream (datafile, FileMode.OpenOrCreate)) {
				return (Player)mySerializer.Deserialize (fs);
			}
		}

		public void WriteData(Player currentPlayer)
		{
			var mySerializer = new XmlSerializer (typeof(Player));

			switch (SlotPosition) {
			case PlayerSlot.slot1:
				PlayerDataFile = docDir.AbsoluteUrl.RelativePath + "/player1data.xml";
				break;

			case PlayerSlot.slot2:
				PlayerDataFile = docDir.AbsoluteUrl.RelativePath + "/player2data.xml";
				break;

			case PlayerSlot.slot3:
				PlayerDataFile = docDir.AbsoluteUrl.RelativePath + "/player3data.xml";
				break;
			}
				
			using (var myWriter = new StreamWriter (currentPlayer.PlayerDataFile, false)) {
				mySerializer.Serialize (myWriter, this);
			}
		}
	}
}

