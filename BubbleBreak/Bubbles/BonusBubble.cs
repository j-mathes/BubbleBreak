//---------------------------------------------------------------------------------------------
// <copyright file="BonusBubble.cs" company="RetroTek Software Ltd">
//     Copyright (C) 2016 RetroTek Software Ltd. All rights reserved.
// </copyright>
// <author>Jared Mathes</author>
//---------------------------------------------------------------------------------------------

using System;
using CocosSharp;

namespace BubbleBreak
{
	public class BonusBubble : GameBubble
	{
		new const float MIN_TIME_DELAY = 0.1f;		// minimum amount of time before bubble is born
		new const float MAX_TIME_DELAY = 2.0f;		// maximum amount of time before bubble is born
		new const float MIN_TIME_APPEAR = 0.1f;		// minimum amount of time to fade in a bubble
		new const float MAX_TIME_APPEAR = 1.5f;		// maximum amount of time to fade in a bubble
		new const float MIN_TIME_HOLD = 6.0f;		// minimum amount of time to hold a bubble visible
		new const float MAX_TIME_HOLD = 15.0f;		// maximum amount of time to hold a bubble visible
		new const float MIN_TIME_FADE = 2.0f;		// minimum amount of time to fade out a bubble
		new const float MAX_TIME_FADE = 6.0f;		// maximum amount of time to fade out a bubble

		const int MIN_EASY_TAPS = 1;
		const int MAX_EASY_TAPS = 5;
		const int MIN_NORMAL_TAPS = 3;
		const int MAX_NORMAL_TAPS = 10;
		const int MIN_HARD_TAPS = 5;
		const int MAX_HARD_TAPS = 15;

		const int BWEIGHT_POS_TIME = 250;
		const int BWEIGHT_DOUBLE_SCORE = 225;
		const int BWEIGHT_POS_POINTS = 200;
		const int BWEIGHT_ADDITION = 150;
		const int BWEIGHT_SUBTRACTION = 150;
		const int BWEIGHT_CHECKPOINT = 125;
		const int BWEIGHT_TRIPLE_SCORE = 180;
		const int BWEIGHT_NEXT_IN_SEQ = 70;
		const int BWEIGHT_NEXT_MF = 50;
		const int BWEIGHT_BONUS_MF = 50;
		const int BWEIGHT_TAP_STR = 30;
		const int BWEIGHT_LUCKY_POINT_CHANCE = 15;
		const int BWEIGHT_LUCKY_POINT_AMOUNT = 15;
		const int BWEIGHT_NEG_TIME = 10;
		const int BWEIGHT_NEG_POINTS = 10;
		const int BWEIGHT_MYSTERY = 5;

		int bWeightTotalAddition;
		int bWeightTotalBonusMF;
		int bWeightTotalCheckpoint;
		int bWeightTotalDoubleScore;
		int bWeightTotalMystery;
		int bWeightTotalNegPoints;
		int bWeightTotalNegTime;
		int bWeightTotalNextInSeq;
		int bWeightTotalNextMF;
		int bWeightTotalPosPoints;
		int bWeightTotalPosTime;
		int bWeightTotalSubtraction;
		int bWeightTotalTapStrength;
		int bWeightTotalTripleScore;
		int bWeightTotalLuckyPointChance;
		int bWeightTotalLuckyPointAmount;

		readonly CCSpriteSheet BonusSpriteSheet;
		public CCSprite BonusSprite;

		public CCLabel PosLabel{ get; set; }
		public string LableText { get; set; }

		public Bonuses BonusType { get; set; }
		Level currentLevel;

		//---------------------------------------------------------------------------------------------------------
		// BonusBubble Constructor
		//---------------------------------------------------------------------------------------------------------
		public BonusBubble (int xIndex, int yIndex, int listIndex, int tapsNeeded, Level activeLevel) : 
		base (xIndex, yIndex, listIndex)
		{
			TapCount = 0;
			TapsToPop = tapsNeeded;
			XIndex = xIndex;
			YIndex = yIndex;
			ListIndex = listIndex;
			currentLevel = activeLevel;

			TimeDelay = CCRandom.GetRandomFloat (MIN_TIME_DELAY, MAX_TIME_DELAY);
			TimeAppear = CCRandom.GetRandomFloat (MIN_TIME_APPEAR, MAX_TIME_APPEAR);
			TimeHold = CCRandom.GetRandomFloat (MIN_TIME_HOLD, MAX_TIME_HOLD);
			TimeFade = CCRandom.GetRandomFloat (MIN_TIME_FADE, MAX_TIME_FADE);

			BonusSpriteSheet = new CCSpriteSheet ("bonus.plist");

			BonusType = GetRandomBonusType ();
			GetRandomTapsToPop (BonusType);
			GetBubbleSprite (BonusType);
			GetBonusSprite (BonusType);
		}

		//---------------------------------------------------------------------------------------------------------
		// Pop
		//---------------------------------------------------------------------------------------------------------
		// Processes logic for popping a bubble
		//---------------------------------------------------------------------------------------------------------

		public void Pop()
		{
			BonusSprite.RemoveFromParent ();
		}

		//---------------------------------------------------------------------------------------------------------
		// GetRandomTapsToPop
		//---------------------------------------------------------------------------------------------------------
		// Determines taps to pop based on bonus type and difficulty level
		//---------------------------------------------------------------------------------------------------------

		public int GetRandomTapsToPop (Bonuses bonusType)
		{
			int tapsNeeded = MIN_EASY_TAPS;

			switch (bonusType) {
			case Bonuses.pos_time:
				{
					tapsNeeded = MIN_EASY_TAPS;
					break;
				}
			case Bonuses.double_score:
				{
					tapsNeeded = MIN_EASY_TAPS;
					break;
				}
			case Bonuses.pos_points:
				{
					tapsNeeded = MIN_EASY_TAPS;
					break;
				}
			case Bonuses.addition:
				{
					tapsNeeded = MIN_EASY_TAPS;
					break;
				}
			case Bonuses.subtraction:
				{
					tapsNeeded = MIN_EASY_TAPS;
					break;
				}
			case Bonuses.checkpoint:
				{
					tapsNeeded = MIN_EASY_TAPS;
					break;
				}
			case Bonuses.triple_score:
				{
					tapsNeeded = MIN_EASY_TAPS;
					break;
				}
			case Bonuses.next_in_seq:
				{
					tapsNeeded = MIN_EASY_TAPS;
					break;
				}
			case Bonuses.next_mf:
				{
					tapsNeeded = MIN_EASY_TAPS;
					break;
				}
			case Bonuses.bonus_mf:
				{
					tapsNeeded = MIN_EASY_TAPS;
					break;
				}
			case Bonuses.tap_str:
				{
					tapsNeeded = MIN_EASY_TAPS;
					break;
				}
			case Bonuses.neg_time:
				{
					tapsNeeded = 1;
					break;
				}
			case Bonuses.neg_points:
				{
					tapsNeeded = 1;
					break;
				}
			case Bonuses.mystery:
				{
					tapsNeeded = CCRandom.GetRandomInt (MIN_HARD_TAPS,MAX_HARD_TAPS);
					break;
				}
			case Bonuses.lucky_point_chance:
				{
					tapsNeeded = 1;
					break;
				}
			case Bonuses.lucky_point_amount:
				{
					tapsNeeded = 1;
					break;
				}
			}

			return tapsNeeded;
		}

		//---------------------------------------------------------------------------------------------------------
		// GetRandomBonusType
		//---------------------------------------------------------------------------------------------------------
		// Randomly chooses a bonus bubble type
		//---------------------------------------------------------------------------------------------------------
		// TODO: Bonus bubbles that are the same as consumables need to be more rare so the player is encouraged to
		// buy them with their coins.
		//---------------------------------------------------------------------------------------------------------

		public Bonuses GetRandomBonusType()
		{
			Bonuses bonusType = Bonuses.unused;
			int random;
			int upperRandomRange = 0;

			if (currentLevel.BonusAddition) {
				upperRandomRange += BWEIGHT_ADDITION;
				bWeightTotalAddition = upperRandomRange;
			}
			if (currentLevel.BonusBonusMF) {
				upperRandomRange += BWEIGHT_BONUS_MF;
				bWeightTotalBonusMF = upperRandomRange;
			}
			if (currentLevel.BonusCheckpoint) {
				upperRandomRange += BWEIGHT_CHECKPOINT;
				bWeightTotalCheckpoint = upperRandomRange;
			}
			if (currentLevel.BonusDoubleScore) {
				upperRandomRange += BWEIGHT_DOUBLE_SCORE;
				bWeightTotalDoubleScore = upperRandomRange;
			}
			if (currentLevel.BonusMystery) {
				upperRandomRange += BWEIGHT_MYSTERY;
				bWeightTotalMystery = upperRandomRange;
			}
			if (currentLevel.BonusNegPoints) {
				upperRandomRange += BWEIGHT_NEG_POINTS;
				bWeightTotalNegPoints = upperRandomRange;
			}
			if (currentLevel.BonusNegTime) {
				upperRandomRange += BWEIGHT_NEG_TIME;
				bWeightTotalNegTime = upperRandomRange;
			}
			if (currentLevel.BonusNextInSeq) {
				upperRandomRange += BWEIGHT_NEXT_IN_SEQ;
				bWeightTotalNextInSeq = upperRandomRange;
			}
			if (currentLevel.BonusNextMF) {
				upperRandomRange += BWEIGHT_NEXT_MF;
				bWeightTotalNextMF = upperRandomRange;
			}
			if (currentLevel.BonusPosPoints) {
				upperRandomRange += BWEIGHT_POS_POINTS;
				bWeightTotalPosPoints = upperRandomRange;
			}
			if (currentLevel.BonusPosTime) {
				upperRandomRange += BWEIGHT_POS_TIME;
				bWeightTotalPosTime = upperRandomRange;
			}
			if (currentLevel.BonusSubtraction) {
				upperRandomRange += BWEIGHT_SUBTRACTION;
				bWeightTotalSubtraction = upperRandomRange;
			}
			if (currentLevel.BonusTapStrength) {
				upperRandomRange += BWEIGHT_TAP_STR;
				bWeightTotalTapStrength = upperRandomRange;
			}
			if (currentLevel.BonusTripleScore) {
				upperRandomRange += BWEIGHT_TRIPLE_SCORE;
				bWeightTotalTripleScore = upperRandomRange;
			}
			if (currentLevel.BonusLuckyPointChance) {
				upperRandomRange += BWEIGHT_LUCKY_POINT_CHANCE;
				bWeightTotalLuckyPointChance = upperRandomRange;
			}
			if (currentLevel.BonusLuckyPointAmount) {
				upperRandomRange += BWEIGHT_LUCKY_POINT_AMOUNT;
				bWeightTotalLuckyPointAmount = upperRandomRange;
			}
		
			random = CCRandom.GetRandomInt (1, upperRandomRange); // we make the lower end of the range a 1 because any unused weights will be 0

			if (random <= bWeightTotalAddition) {
				bonusType = Bonuses.addition;
			} else if (random <= bWeightTotalBonusMF) {
				bonusType = Bonuses.bonus_mf;
			} else if (random <= bWeightTotalCheckpoint) {
				bonusType = Bonuses.checkpoint;
			} else if (random <= bWeightTotalDoubleScore) {
				bonusType = Bonuses.double_score;
			} else if (random <= bWeightTotalMystery) {
				bonusType = Bonuses.mystery;
			} else if (random <= bWeightTotalNegPoints) {
				bonusType = Bonuses.neg_points;
			} else if (random <= bWeightTotalNegTime) {
				bonusType = Bonuses.neg_time;
			} else if (random <= bWeightTotalNextInSeq) {
				bonusType = Bonuses.next_in_seq;
			} else if (random <= bWeightTotalNextMF) {
				bonusType = Bonuses.next_mf;
			} else if (random <= bWeightTotalPosPoints) {
				bonusType = Bonuses.pos_points;
			} else if (random <= bWeightTotalPosTime) {
				bonusType = Bonuses.pos_time;
			} else if (random <= bWeightTotalSubtraction) {
				bonusType = Bonuses.subtraction;
			} else if (random <= bWeightTotalTapStrength) {
				bonusType = Bonuses.tap_str;
			} else if (random <= bWeightTotalTripleScore) {
				bonusType = Bonuses.triple_score;
			} else if (random <= bWeightTotalLuckyPointChance) {
				bonusType = Bonuses.lucky_point_chance;
			} else if (random <= bWeightTotalLuckyPointAmount) {
				bonusType = Bonuses.lucky_point_amount;
			}
			//TODO: need to add assignment for Big Pos TIME and POINTS
			return bonusType;
		}

		//---------------------------------------------------------------------------------------------------------
		// GetBubbleSprite
		//---------------------------------------------------------------------------------------------------------
		// Gets the correct bubble sprite for the bonus bubble
		//---------------------------------------------------------------------------------------------------------

		public void GetBubbleSprite(Bonuses bonusType)
		{
			switch (bonusType) {
			case Bonuses.pos_time:
				{
					GetGlowSprite (BubbleColors.iceblue.ToString ());
					break;
				}
			case Bonuses.double_score:
				{
					GetGlowSprite (BubbleColors.iceblue.ToString ());
					break;
				}
			case Bonuses.pos_points:
				{
					GetGlowSprite (BubbleColors.iceblue.ToString ());
					break;
				}
			case Bonuses.addition:
				{
					GetGlowSprite (BubbleColors.iceblue.ToString ());
					break;
				}
			case Bonuses.subtraction:
				{
					GetGlowSprite (BubbleColors.iceblue.ToString ());
					break;
				}
			case Bonuses.checkpoint:
				{
					GetGlowSprite (BubbleColors.iceblue.ToString ());
					break;
				}
			case Bonuses.triple_score:
				{
					GetGlowSprite (BubbleColors.iceblue.ToString ());
					break;
				}
			case Bonuses.next_in_seq:
				{
					GetGlowSprite (BubbleColors.iceblue.ToString ());
					break;
				}
			case Bonuses.next_mf:
				{
					GetGlowSprite (BubbleColors.iceblue.ToString ());
					break;
				}
			case Bonuses.bonus_mf:
				{
					GetGlowSprite (BubbleColors.iceblue.ToString ());
					break;
				}
			case Bonuses.tap_str:
				{
					GetGlowSprite (BubbleColors.iceblue.ToString ());
					break;
				}
			case Bonuses.neg_time:
				{
					GetGlowSprite (BubbleColors.red.ToString ());
					break;
				}
			case Bonuses.neg_points:
				{
					GetGlowSprite (BubbleColors.red.ToString ());
					break;
				}
			case Bonuses.lucky_point_chance:
				{
					GetGlowSprite (BubbleColors.iceblue.ToString ());
					break;
				}
			case Bonuses.lucky_point_amount:
				{
					GetGlowSprite (BubbleColors.iceblue.ToString ());
					break;
				}
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// GetSprite
		//---------------------------------------------------------------------------------------------------------
		// Gets the sprite graphic based on color passed
		//---------------------------------------------------------------------------------------------------------

		void GetGlowSprite ( string colorName)
		{
			BubbleSprite = new CCSprite (BubbleSpriteSheet.Frames.Find (x => x.TextureFilename.Equals (string.Format ("bubble-glow-{0}.png", colorName))));
			BubbleSprite.AnchorPoint = CCPoint.AnchorMiddle;
			BubbleSprite.Opacity = 0;
			AddChild (BubbleSprite);
			PopSprite = new CCSprite (BubbleSpriteSheet.Frames.Find (x => x.TextureFilename.Equals (string.Format ("bubble-pop-{0}.png", colorName))));
			PopSprite.AnchorPoint = CCPoint.AnchorMiddle;
			PopSprite.Scale = 0.0f;
			AddChild (PopSprite);
		}

		//---------------------------------------------------------------------------------------------------------
		// GetBonusSprite
		//---------------------------------------------------------------------------------------------------------
		// Gets the correct bonus sprite identifier for the bonus bubble
		//---------------------------------------------------------------------------------------------------------

		public void GetBonusSprite(Bonuses bonusType)
		{
			var spriteName = string.Format ("bonus_{0}.png", bonusType);
			BonusSprite = new CCSprite (BonusSpriteSheet.Frames.Find (x => x.TextureFilename.Equals (spriteName)));
			BonusSprite.AnchorPoint = CCPoint.AnchorMiddle;
			BonusSprite.Opacity = 0;
			AddChild (BonusSprite,1);
		}
	}
}

