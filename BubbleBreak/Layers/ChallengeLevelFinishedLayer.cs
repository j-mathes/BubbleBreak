// -----------------------------------------------------------------------------------------------
//  <copyright file="ChallengeLevelFinishedLayer.cs" company="RetroTek Software Ltd">
//      Copyright (C) 2016 RetroTek Software Ltd. All rights reserved.
//  </copyright>
//  <author>Jared Mathes</author>
// -----------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CocosSharp;

namespace BubbleBreak
{
	//---------------------------------------------------------------------------------------------------------
	// ChallengeLevelFinishedLayer constructor
	//---------------------------------------------------------------------------------------------------------
	public class ChallengeLevelFinishedLayer: CCLayerColor
	{
		const string GOTHIC_30_HD_FNT = "gothic-30-hd.fnt";
		const string GOTHIC_44_HD_FNT = "gothic-44-hd.fnt";

		const string LEVEL_DATA_FOLDER = "./LevelData/";

		List<Branch> branches;
		List<Level> levels;
		XDocument levelInfo = new XDocument ();
		XDocument branchInfo = new XDocument ();
		Level lastLevelPlayed;

		CCSpriteSheet uiSpriteSheet;
		CCSprite frameSprite;
		CCLabel scoreLabel, coinsLabel, menuNextLevelLabel, menuQuitLevelLabel, menuRetryLevelLabel, menuShopLabel, frameTitleLabel;

		string scoreMessage = string.Empty;
		string coinsMessage = string.Empty;

		int playerScore;
		bool levelWasCompleted;
		int levelPassScore;
		int totalCoinsEarned;

		Player activePlayer;

		public ChallengeLevelFinishedLayer (int score, Player currentPlayer, bool wasLevelPassed, int coinsEarned, Level lastLevel)
		{
			activePlayer = currentPlayer;
			lastLevelPlayed = lastLevel;
			playerScore = score;
			totalCoinsEarned = coinsEarned;
			levelPassScore = lastLevelPlayed.LevelPassScore;
			levelWasCompleted = wasLevelPassed;
			Color = new CCColor3B(0,0,0);
			Opacity = 255;

			uiSpriteSheet = new CCSpriteSheet ("ui.plist");
		}

		//---------------------------------------------------------------------------------------------------------
		// AddedToScene
		//---------------------------------------------------------------------------------------------------------
		protected override void AddedToScene ()
		{
			base.AddedToScene ();

			// Use the bounds to layout the positioning of our drawable assets
			CCRect bounds = VisibleBoundsWorldspace;

			branches = ReadBranches (branchInfo);
			levels = ReadLevels (levelInfo);

			//If the level was complete and it was the 10th level or higher, we need to flag the next sequence challenge as being available for play
			UnlockNextChallenge(levelWasCompleted, lastLevelPlayed);

			// Add frame to layer
			frameSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("frame.png")));
			frameSprite.AnchorPoint = CCPoint.AnchorMiddle;
			frameSprite.Position = new CCPoint (bounds.Size.Width / 2, bounds.Size.Height / 2);
			AddChild (frameSprite);

			// Add frame title;
			frameTitleLabel = new CCLabel ((levelWasCompleted) ? "Level Complete!" : "Level Incomplete", GOTHIC_44_HD_FNT);
			frameTitleLabel.AnchorPoint = CCPoint.AnchorMiddle;
			frameTitleLabel.Scale = 2.0f;
			frameTitleLabel.Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MaxY - (frameTitleLabel.BoundingBox.Size.Height * 1.5f));
			AddChild (frameTitleLabel);

			scoreMessage = (levelWasCompleted) ? string.Format ("Your score is {0}", playerScore) : string.Format("You needed {0} more points\nto pass the level", levelPassScore - playerScore);
			coinsMessage = (totalCoinsEarned > 0) ? string.Format ("You earned {0} coins", totalCoinsEarned) : string.Format (" ");

			coinsLabel = new CCLabel (coinsMessage, GOTHIC_44_HD_FNT);
			coinsLabel.AnchorPoint = CCPoint.AnchorMiddle;
			coinsLabel.Scale = 1f;
			coinsLabel.Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MinY + (coinsLabel.BoundingBox.Size.Height * 14f));
			coinsLabel.HorizontalAlignment = CCTextAlignment.Center;
			AddChild (coinsLabel);

			scoreLabel = new CCLabel (scoreMessage, GOTHIC_44_HD_FNT);
			scoreLabel.AnchorPoint = CCPoint.AnchorMiddle;
			scoreLabel.Scale = 1f;
			scoreLabel.Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MinY + (coinsLabel.BoundingBox.Size.Height * 12f));
			scoreLabel.HorizontalAlignment = CCTextAlignment.Center;
			AddChild (scoreLabel);

			menuNextLevelLabel = new CCLabel ("Next Level",GOTHIC_44_HD_FNT);
			menuNextLevelLabel.AnchorPoint = CCPoint.AnchorMiddle;
			menuNextLevelLabel.Scale = 2.0f;
			var menuItemNextLevel = new CCMenuItemLabel (menuNextLevelLabel, NextLevel);

			menuRetryLevelLabel = new CCLabel ("Retry", GOTHIC_44_HD_FNT);
			menuRetryLevelLabel.AnchorPoint = CCPoint.AnchorMiddle;
			menuRetryLevelLabel.Scale = 2.0f;
			var menuItemRetryLevel = new CCMenuItemLabel (menuRetryLevelLabel, RetryLevel);

			menuShopLabel = new CCLabel ("Shop", GOTHIC_44_HD_FNT);
			menuShopLabel.AnchorPoint = CCPoint.AnchorMiddle;
			menuShopLabel.Scale = 2.0f;
			var menuItemShop = new CCMenuItemLabel (menuShopLabel, GoToShop);

			menuQuitLevelLabel = new CCLabel ("Quit", GOTHIC_44_HD_FNT);
			menuQuitLevelLabel.AnchorPoint = CCPoint.AnchorMiddle;
			menuQuitLevelLabel.Scale = 2.0f;
			var menuItemQuit = new CCMenuItemLabel (menuQuitLevelLabel, QuitLevel);

			var menu = new CCMenu ((levelWasCompleted) ? menuItemNextLevel : menuItemRetryLevel, menuItemShop, menuItemQuit) {
				Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MinY + (menuQuitLevelLabel.BoundingBox.Size.Height * 6f)),
				AnchorPoint = CCPoint.AnchorMiddle
			};
			menu.AlignItemsVertically (menuQuitLevelLabel.BoundingBox.Size.Height * 0.75f);

			AddChild (menu);
		}

		//---------------------------------------------------------------------------------------------------------
		// UnlockNextChallenge
		//---------------------------------------------------------------------------------------------------------
		// Decides if we need to flag the next challenge as unlocked based on the last level played.
		//---------------------------------------------------------------------------------------------------------
		void UnlockNextChallenge (bool wasLastLevelCompleted, Level lastLevelUserPlayed)
		{
			if (wasLastLevelCompleted) {
				if (lastLevelUserPlayed.LevelNum >= 9)
					activePlayer.BranchProgression [lastLevelUserPlayed.BranchNum + 1].IsLocked = false;
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// CreateScene
		//---------------------------------------------------------------------------------------------------------
		// 
		//---------------------------------------------------------------------------------------------------------
		public static CCScene CreateScene (CCWindow mainWindow, int score, Player currentPlayer, bool levelPassed, int coinsEarned, Level theLastLevelPlayed)
		{
			var scene = new CCScene (mainWindow);
			var layer = new ChallengeLevelFinishedLayer (score, currentPlayer, levelPassed, coinsEarned, theLastLevelPlayed);

			scene.AddChild (layer);

			return scene;
		} 

		//---------------------------------------------------------------------------------------------------------
		// NextLevel
		//---------------------------------------------------------------------------------------------------------
		// Transitions to the next level if the player passed this level
		//---------------------------------------------------------------------------------------------------------
		void NextLevel (object stuff = null)
		{
			var nextLevelToPlay = GetNextLevelToPlay ();
			var nextLevel = ChallengeLevelLayer.CreateScene (Window, nextLevelToPlay, activePlayer);
			var transitionToGame = new CCTransitionFade (2.0f, nextLevel);
			Director.ReplaceScene (transitionToGame);
		}

		//---------------------------------------------------------------------------------------------------------
		// RetryLevel
		//---------------------------------------------------------------------------------------------------------
		// If the player didn't pass the level, let them play it over
		//---------------------------------------------------------------------------------------------------------
		void RetryLevel (object stuff = null)
		{
			var level = ChallengeLevelLayer.CreateScene (Window, lastLevelPlayed, activePlayer);
			var transitionToGame = new CCTransitionFade (2.0f, level);
			Director.ReplaceScene (transitionToGame);
		}

		//---------------------------------------------------------------------------------------------------------
		// GoToShop
		//---------------------------------------------------------------------------------------------------------
		// Let the player go to the shop to spend their hard earned coins
		//---------------------------------------------------------------------------------------------------------
		void GoToShop (object stuff = null)
		{
			var layer = ShopLayer.CreateScene (Window, activePlayer);
			var transitionToGame = new CCTransitionSlideInR(0.2f, layer);
			Director.ReplaceScene (transitionToGame);
		}

		//---------------------------------------------------------------------------------------------------------
		// QuitLevel
		//---------------------------------------------------------------------------------------------------------
		// If they've had enough for now, let them quit back to the Select Challenge layer
		//---------------------------------------------------------------------------------------------------------
		void QuitLevel (object stuff = null)
		{
			var mainMenu = SelectChallengeLayer.CreateScene (Window, activePlayer);
			var transitionToMainMenu = new CCTransitionFade (2.0f, mainMenu);
			Director.ReplaceScene (transitionToMainMenu);
		}

		//---------------------------------------------------------------------------------------------------------
		// GetNextLevelToPlay
		//---------------------------------------------------------------------------------------------------------
		// Returns the level from the list of levels, taking into account the current branch
		//---------------------------------------------------------------------------------------------------------
		Level GetNextLevelToPlay ()
		{
			var currentBranch = lastLevelPlayed.BranchNum;
			int levelIndex;

			if (currentBranch == 1) {
				levelIndex = (activePlayer.BranchProgression [currentBranch].LastLevelCompleted + 1) + 3;  //TODO: is this correct?  Do we need the +1?
			} else {
				levelIndex = (activePlayer.BranchProgression [currentBranch].LastLevelCompleted) + ((currentBranch - 1) * 20) + 3;  // TODO: do we need to split these out?  Can we combine them?
			}

			return levels[levelIndex]; 
		}

		//---------------------------------------------------------------------------------------------------------
		// ReadBranches
		//---------------------------------------------------------------------------------------------------------
		// Reads the contents of the branches.xml file and builds a list of branch objects
		//---------------------------------------------------------------------------------------------------------
		public List<Branch> ReadBranches(XDocument branchInfo)
		{
			branchInfo = XDocument.Load (LEVEL_DATA_FOLDER + "branches.xml");
			List<Branch> branchList = (from branch in branchInfo.Root.Descendants ("record")
				select new Branch {
					BranchNum = int.Parse (branch.Element ("BranchNum").Value),
					BranchName = branch.Element ("BranchName").Value,
					NumberOfLevels = int.Parse (branch.Element ("NumberOfLevels").Value),
					UnlockNextBranch = int.Parse (branch.Element ("UnlockNextBranch").Value),
					UnlockFreePlay = int.Parse (branch.Element ("UnlockFreePlay").Value),
				}).ToList ();
			return branchList;
		}

		//---------------------------------------------------------------------------------------------------------
		// ReadLevels
		//---------------------------------------------------------------------------------------------------------
		// Reads the level xml data and builds a list of levels 
		//---------------------------------------------------------------------------------------------------------
		public List<Level> ReadLevels(XDocument levelData)
		{
			levelData = XDocument.Load (LEVEL_DATA_FOLDER + "levels.xml");

			List<Level> lvl = (from level in levelData.Root.Descendants ("record")
				select new Level {
					BranchNum = int.Parse (level.Element ("BranchNum").Value), 
					LevelNum = int.Parse (level.Element ("LevelNum").Value),
					LevelName = level.Element ("LevelName").Value,
					MaxBubbles = int.Parse (level.Element ("MaxBubbles").Value),
					MaxVisibleBubbles = int.Parse (level.Element ("MaxVisibleBubbles").Value),
					StartingPointValue = int.Parse (level.Element ("StartingPointValue").Value),
					MaxLevelTime = int.Parse (level.Element ("MaxLevelTime").Value),
					LevelPassScore = int.Parse (level.Element ("LevelPassScore").Value),
					TapsToPopStandard = int.Parse (level.Element ("TapsToPopStandard").Value),
					InitialVisibleBubbles = int.Parse (level.Element ("InitialVisibleBubbles").Value),
					ChanceToRollNextSeq = int.Parse (level.Element ("ChanceToRollNextSeq").Value),
					ChanceToRollBonus = int.Parse (level.Element ("ChanceToRollBonus").Value),
					LevelDescription = level.Element ("LevelDescription").Value,
					SeqLinear = bool.Parse (level.Element ("SeqLinear").Value),
					SeqEven = bool.Parse ( level.Element ("SeqEven").Value),
					SeqOdd = bool.Parse (level.Element ("SeqOdd").Value),
					SeqTriangular = bool.Parse (level.Element ("SeqTriangular").Value),
					SeqSquare = bool.Parse (level.Element ("SeqSquare").Value),
					SeqLazy = bool.Parse (level.Element ("SeqLazy").Value),
					SeqFibonacci = bool.Parse (level.Element ("SeqFibonacci").Value),
					SeqPrime = bool.Parse (level.Element ("SeqPrime").Value),
					SeqDouble = bool.Parse (level.Element ("SeqDouble").Value),
					SeqTriple = bool.Parse (level.Element ("SeqTriple").Value),
					SeqPi = bool.Parse (level.Element ("SeqPi").Value),
					SeqRecaman = bool.Parse (level.Element ("SeqRecaman").Value),
					ConsumablesCheckpoints = int.Parse (level.Element ("ConsumablesCheckpoints").Value),
					ConsumablesAdditions = int.Parse (level.Element ("ConsumablesAdditions").Value),
					ConsumablesSubtractions = int.Parse (level.Element ("ConsumablesSubtractions").Value),
					ConsumablesNexts = int.Parse (level.Element ("ConsumablesNexts").Value),
					BonusDoubleScore = bool.Parse (level.Element ("BonusDoubleScore").Value),
					BonusTripleScore = bool.Parse (level.Element ("BonusTripleScore").Value),
					BonusCheckpoint = bool.Parse (level.Element ("BonusCheckpoint").Value),
					BonusPosTime = bool.Parse (level.Element ("BonusPosTime").Value),
					BonusPosPoints = bool.Parse (level.Element ("BonusPosPoints").Value),
					BonusTapStrength = bool.Parse (level.Element ("BonusTapStrength").Value),
					BonusAddition = bool.Parse (level.Element ("BonusAddition").Value),
					BonusSubtraction = bool.Parse (level.Element ("BonusSubtraction").Value),
					BonusNextMF = bool.Parse (level.Element ("BonusNextMF").Value),
					BonusBonusMF = bool.Parse (level.Element ("BonusBonusMF").Value),
					BonusNextInSeq = bool.Parse (level.Element ("BonusNextInSequence").Value),
					BonusMystery = bool.Parse (level.Element ("BonusMystery").Value),
					BonusLuckyPointChance = bool.Parse (level.Element ("BonusCritPointChance").Value), 
					BonusLuckyPointAmount = bool.Parse (level.Element ("BonusCritPointAmount").Value),
					UnlockSequenceUI = bool.Parse (level.Element ("UnlockSequenceUI").Value),
					UnlockBonusUI = bool.Parse (level.Element ("UnlockBonusUI").Value),
					UnlockConsumableUI = bool.Parse (level.Element ("UnlockConsumableUI").Value),
				}).ToList ();		
			return lvl;
		}
	}
}
