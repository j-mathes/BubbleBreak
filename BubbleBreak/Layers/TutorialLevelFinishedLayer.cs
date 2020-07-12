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
	public class TutorialLevelFinishedLayer: CCLayerColor
	{
		const string GOTHIC_30_HD_FNT = "gothic-30-hd.fnt";
		const string GOTHIC_44_HD_FNT = "gothic-44-hd.fnt";
		const string GOTHIC_56_WHITE_HD_FNT = "gothic-56-white-hd.fnt";
		const string GOTHIC_56_WHITE_FNT = "gothic-56-white.fnt";

		const string LEVEL_DATA_FOLDER = "./LevelData/";

		CCSpriteSheet uiSpriteSheet;
		CCSprite frameSprite;
		CCLabel scoreLabel, coinsLabel, menuNextLevelLabel, menuQuitLevelLabel, menuRetryLevelLabel, frameTitleLabel; //menuShopLabel, 

		string scoreMessage = string.Empty;
		string coinsMessage = string.Empty;

		int playerScore;
		bool levelWasCompleted;
		int levelScore;
		int totalCoinsEarned;

		Player activePlayer;
		Level activeLevel;
		List<Level> levels;
		XDocument levelInfo = new XDocument ();

		public TutorialLevelFinishedLayer (int score, Player currentPlayer, Level gameLevel, bool wasLevelPassed, int coinsEarned)
		{
			activeLevel = gameLevel;
			activePlayer = currentPlayer;
			playerScore = score;
			totalCoinsEarned = (activeLevel.LevelNum == 2) ? coinsEarned : 0;
			levelScore = activeLevel.LevelPassScore;
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

			levels = ReadLevels (levelInfo);

			// Use the bounds to layout the positioning of our drawable assets
			CCRect bounds = VisibleBoundsWorldspace;

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

			scoreMessage = (levelWasCompleted) ? string.Format ("Your score is {0}", playerScore) : string.Format("You needed {0} more points\nto pass the level", levelScore - playerScore);
			coinsMessage = (totalCoinsEarned > 0) ? string.Format ("You earned {0} coins", totalCoinsEarned) : string.Format (" ");

			coinsLabel = new CCLabel (coinsMessage, GOTHIC_44_HD_FNT);
			coinsLabel.AnchorPoint = CCPoint.AnchorMiddle;
			coinsLabel.Scale = 1f;
			coinsLabel.Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MinY + (coinsLabel.BoundingBox.Size.Height * 14f));
			coinsLabel.HorizontalAlignment = CCTextAlignment.Center;
			if (activeLevel.LevelNum == 2) AddChild (coinsLabel);

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

//			menuShopLabel = new CCLabel ("Shop", GOTHIC_44_HD_FNT);
//			menuShopLabel.AnchorPoint = CCPoint.AnchorMiddle;
//			menuShopLabel.Scale = 2.0f;
//			var menuItemShop = new CCMenuItemLabel (menuShopLabel, GoToShop);

			menuQuitLevelLabel = new CCLabel ("Return to Main", GOTHIC_44_HD_FNT);
			menuQuitLevelLabel.AnchorPoint = CCPoint.AnchorMiddle;
			menuQuitLevelLabel.Scale = 2.0f;
			var menuItemQuit = new CCMenuItemLabel (menuQuitLevelLabel, QuitLevel);

			//var menu = new CCMenu ((levelWasCompleted) ? menuItemNextLevel : menuItemRetryLevel, menuItemShop, menuItemQuit) {
			var menu = new CCMenu ((levelWasCompleted) ? menuItemNextLevel : menuItemRetryLevel, menuItemQuit) {
				Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MinY + (menuQuitLevelLabel.BoundingBox.Size.Height * 6f)),
				AnchorPoint = CCPoint.AnchorMiddle
			};
			menu.AlignItemsVertically (menuQuitLevelLabel.BoundingBox.Size.Height * 0.75f);

			AddChild (menu);
		}

		//---------------------------------------------------------------------------------------------------------
		// CreateScene
		//---------------------------------------------------------------------------------------------------------
		public static CCScene CreateScene (CCWindow mainWindow, int score, Player currentPlayer, Level gameLevel, bool levelPassed, int coinsEarned)
		{
			var scene = new CCScene (mainWindow);
			var layer = new TutorialLevelFinishedLayer (score, currentPlayer, gameLevel, levelPassed, coinsEarned);

			scene.AddChild (layer);

			return scene;
		} 

		//---------------------------------------------------------------------------------------------------------
		// NextLevel
		//---------------------------------------------------------------------------------------------------------
		// Transitions to the next level for play.  If the next level is not in the Tutorial branch, then
		// transition to the SelectChallengeLayer
		//---------------------------------------------------------------------------------------------------------
		void NextLevel (object stuff = null)
		{
			var nextTutorialLevel = GetNextTutorialLevelToPlay ();
			if (nextTutorialLevel.BranchNum < 1) {
				var layer = TutorialLayer.CreateScene (Window, nextTutorialLevel, activePlayer);
				var transitionToLayer = new CCTransitionSlideInR (0.2f, layer);
				Director.ReplaceScene (transitionToLayer);
			} else {
				activePlayer.BranchProgression [1].IsLocked = false;
				activePlayer.BranchProgression [0].BranchState = CompletionState.completed;
				activePlayer.ConsumableAdd = activePlayer.ConsumableCheckPoint = activePlayer.ConsumableNextSeq = activePlayer.ConsumableSubtract = 0;
				activePlayer.WriteData (activePlayer);
				var layer = SelectChallengeLayer.CreateScene (Window, activePlayer);
				var transitionToLayer = new CCTransitionSlideInR(0.2f, layer);
				Director.ReplaceScene (transitionToLayer);
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// RetryLevel
		//---------------------------------------------------------------------------------------------------------
		// Transitions to the same level the player just completed so they can try again
		//---------------------------------------------------------------------------------------------------------
		void RetryLevel (object stuff = null)
		{
			var layer = TutorialLayer.CreateScene (Window, activeLevel, activePlayer);
			var transitionToLayer = new CCTransitionSlideInR(0.2f, layer);
			Director.ReplaceScene (transitionToLayer);
		}

		//---------------------------------------------------------------------------------------------------------
		// GoToShop
		//---------------------------------------------------------------------------------------------------------
		// Transitions to the ShopLayer
		//---------------------------------------------------------------------------------------------------------
		void GoToShop (object stuff = null)
		{
//			var layer = ShopLayer.CreateScene (Window, activePlayer);
//			var transitionToGame = new CCTransitionSlideInR(0.2f, layer);
//			Director.ReplaceScene (transitionToGame);
		}

		//---------------------------------------------------------------------------------------------------------
		// QuitLevel
		//---------------------------------------------------------------------------------------------------------
		// Transitions to the main MenuLayer
		//---------------------------------------------------------------------------------------------------------
		void QuitLevel (object stuff = null)
		{
			var nextTutorialLevel = GetNextTutorialLevelToPlay ();
			if (nextTutorialLevel.BranchNum > 0) {
				activePlayer.BranchProgression [1].IsLocked = false;
				activePlayer.BranchProgression [0].BranchState = CompletionState.completed;
				activePlayer.ConsumableAdd = activePlayer.ConsumableCheckPoint = activePlayer.ConsumableNextSeq = activePlayer.ConsumableSubtract = 0;
				activePlayer.WriteData (activePlayer);
			}
			var mainMenu = MenuLayer.CreateScene (Window);
			var transitionToMainMenu = new CCTransitionFade (2.0f, mainMenu);
			Director.ReplaceScene (transitionToMainMenu);
		}

		//---------------------------------------------------------------------------------------------------------
		// GetNextLevelToPlay
		//---------------------------------------------------------------------------------------------------------
		// Returns the level from the list of levels, taking into account the current branch
		//---------------------------------------------------------------------------------------------------------
		Level GetNextTutorialLevelToPlay ()
		{
			return levels[(activePlayer.BranchProgression[0].LastLevelCompleted + 1)];
		}

		//---------------------------------------------------------------------------------------------------------
		// ReadLevels
		//---------------------------------------------------------------------------------------------------------
		// Using the list of branches, reads each individual level xml file and builds a list of level objects 
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
