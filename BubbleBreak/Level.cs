//---------------------------------------------------------------------------------------------
// <copyright file="Level.cs" company="RetroTek Software Ltd">
//     Copyright (C) 2016 RetroTek Software Ltd. All rights reserved.
// </copyright>
// <author>Jared Mathes</author>
//---------------------------------------------------------------------------------------------

namespace BubbleBreak
{
	public class Level
	{
		public int BranchNum { get; set; }
		public int LevelNum { get; set;}
		public string LevelName { get; set;}
		public int MaxBubbles { get; set;}
		public int MaxVisibleBubbles { get; set;}
		public int StartingPointValue { get; set;}
		public int MaxLevelTime { get; set;}
		public int LevelPassScore { get; set;}
		public int TapsToPopStandard { get; set;}
		public int InitialVisibleBubbles { get; set;}
		public int ChanceToRollNextSeq { get; set; }
		public int ChanceToRollBonus { get; set; }
		public string LevelDescription { get; set;}
		public bool SeqLinear { get; set; }					// sequence types unlocked for the level
		public bool SeqEven { get; set; }
		public bool SeqOdd { get; set; }
		public bool SeqTriangular { get; set; }
		public bool SeqSquare { get; set; }
		public bool SeqLazy { get; set; }
		public bool SeqFibonacci { get; set; }
		public bool SeqPrime { get; set; }
		public bool SeqDouble { get; set; }
		public bool SeqTriple { get; set; }
		public bool SeqPi { get; set; }
		public bool SeqRecaman { get; set; }
		public int ConsumablesCheckpoints { get; set; }		// consumables given for free when the level starts
		public int ConsumablesAdditions { get; set; }
		public int ConsumablesSubtractions { get; set; }
		public int ConsumablesNexts { get; set; }
		public bool BonusDoubleScore { get; set; }			// bonus bubble types unlocked for the level
		public bool BonusTripleScore { get; set; }
		public bool BonusCheckpoint { get; set; }
		public bool BonusPosTime { get; set; }
		public bool BonusPosPoints { get; set; }
		public bool BonusTapStrength { get; set; }
		public bool BonusAddition { get; set; }
		public bool BonusSubtraction { get; set; }
		public bool BonusNextMF { get; set; }			
		public bool BonusBonusMF { get; set; }			
		public bool BonusNextInSeq { get; set; }
		public bool BonusNegTime { get; set; }
		public bool BonusNegPoints { get; set; }
		public bool BonusMystery { get; set; }
		public bool BonusLuckyPointChance { get; set; }
		public bool BonusLuckyPointAmount { get; set; }
		public bool UnlockSequenceUI{ get; set; }			// unlocks UI elements related to sequences
		public bool UnlockBonusUI{ get; set; }				// unlocks UI elements related to bonuses (tap strength, score multipliers)
		public bool UnlockConsumableUI { get; set; }		// unlocks UI elements related to consumables

		public Level ()
		{
		}
	}
}

