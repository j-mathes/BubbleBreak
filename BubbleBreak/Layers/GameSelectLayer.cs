// -----------------------------------------------------------------------------------------------
//  <copyright file="GameSelectLayer.cs" company="RetroTek Software Ltd">
//      Copyright (C) 2016 RetroTek Software Ltd. All rights reserved.
//  </copyright>
//  <author>Jared Mathes</author>
// -----------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CocosSharp;

namespace BubbleBreak
{
	public class GameSelectLayer : CCLayerColor
	{
		const string GOTHIC_30_HD_FNT = "gothic-30-hd.fnt";
		const string GOTHIC_44_HD_FNT = "gothic-44-hd.fnt";
		const string GOTHIC_56_WHITE_HD_FNT = "gothic-56-white-hd.fnt";
		const string GOTHIC_56_WHITE_FNT = "gothic-56-white.fnt";

		const string LEVEL_DATA_FOLDER = "./LevelData/";

		Player currentPlayer;

		List<Level> levels;
		XDocument levelInfo = new XDocument ();

		CCLabel shopLabel, tutorialLabel, challengesLabel, freePlayLabel, backLabel;

		//---------------------------------------------------------------------------------------------------------
		// GameSelectLayer constructor
		//---------------------------------------------------------------------------------------------------------
		public GameSelectLayer (Player activePlayer)
		{
			currentPlayer = activePlayer;

			Color = new CCColor3B(0,0,0);
			Opacity = 255;
		}

		//---------------------------------------------------------------------------------------------------------
		// AddedToScene
		//--------------------------------------------------------------------------------------------------------- 
		protected override void AddedToScene ()
		{
			base.AddedToScene ();

			// Use the bounds to layout the positioning of our drawable assets
			CCRect bounds = VisibleBoundsWorldspace;

			levels = ReadLevels (levelInfo);

			SetupUI (bounds);
		}

		//---------------------------------------------------------------------------------------------------------
		// CreateScene
		//---------------------------------------------------------------------------------------------------------
		public static CCScene CreateScene (CCWindow mainWindow, Player activePlayer)
		{

			var scene = new CCScene (mainWindow);
			var layer = new GameSelectLayer (activePlayer);

			scene.AddChild (layer);

			return scene;
		}

		//---------------------------------------------------------------------------------------------------------
		// SetupUI
		//---------------------------------------------------------------------------------------------------------
		// Sets up the game UI
		//---------------------------------------------------------------------------------------------------------
		void SetupUI (CCRect bounds)
		{
			shopLabel = new CCLabel ("Shop", GOTHIC_44_HD_FNT);
			shopLabel.AnchorPoint = CCPoint.AnchorMiddle;
			shopLabel.Scale = 2.0f;
			var shopItem = new CCMenuItemLabel (shopLabel, Shop);

			tutorialLabel = new CCLabel ("Tutorial", GOTHIC_44_HD_FNT);
			tutorialLabel.AnchorPoint = CCPoint.AnchorMiddle;
			tutorialLabel.Scale = 2.0f;
			var tutorialItem = new CCMenuItemLabel (tutorialLabel, Tutorial);

			challengesLabel = new CCLabel ("Challenges", GOTHIC_44_HD_FNT);
			challengesLabel.AnchorPoint = CCPoint.AnchorMiddle;
			challengesLabel.Scale = 2.0f;
			var challengesItem = new CCMenuItemLabel (challengesLabel, Challenges);

			freePlayLabel = new CCLabel ("Free Play", GOTHIC_44_HD_FNT);
			freePlayLabel.AnchorPoint = CCPoint.AnchorMiddle;
			freePlayLabel.Scale = 2.0f;
			var freePlayItem = new CCMenuItemLabel (freePlayLabel, FreePlay);

			backLabel = new CCLabel ("Back", GOTHIC_44_HD_FNT) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 2.0f,
			};
			var backItem = new CCMenuItemLabel (backLabel, BackToMain);

			var menu = new CCMenu (tutorialItem, challengesItem, freePlayItem, shopItem, backItem);
			menu.AnchorPoint = CCPoint.AnchorMiddleBottom;
			menu.Position = new CCPoint (bounds.Center);
			menu.AlignItemsVertically (250);

			AddChild (menu);

			//TODO: these will have to change to properly calculate when they should unlock
			challengesItem.Enabled = currentPlayer.BranchProgression [0].BranchState.Equals (CompletionState.completed);
			freePlayItem.Enabled = !currentPlayer.BranchProgression [13].IsLocked;  // this needs to calculate per challenge level
			shopItem.Enabled = currentPlayer.BranchProgression [0].BranchState.Equals (CompletionState.completed);
			tutorialItem.Enabled = (currentPlayer.BranchProgression [0].BranchState != CompletionState.completed);
		}

		//---------------------------------------------------------------------------------------------------------
		// GetNextLevelToPlay
		//---------------------------------------------------------------------------------------------------------
		// Returns the level from the list of levels, taking into account the current branch
		//---------------------------------------------------------------------------------------------------------
		Level GetNextTutorialLevelToPlay ()
		{
			return levels[(currentPlayer.BranchProgression[0].LastLevelCompleted + 1)];
		}

		//---------------------------------------------------------------------------------------------------------
		// Shop
		//---------------------------------------------------------------------------------------------------------
		// Transitions to the Shop layer
		//---------------------------------------------------------------------------------------------------------
		void Shop (object stuff = null)
		{
			var layer = ShopLayer.CreateScene (Window, currentPlayer);
			var transitionToLayer = new CCTransitionSlideInR(0.2f, layer);
			Director.ReplaceScene (transitionToLayer);
		}

		//---------------------------------------------------------------------------------------------------------
		// Tutorial
		//---------------------------------------------------------------------------------------------------------
		// Transitions to the Tutorial
		//---------------------------------------------------------------------------------------------------------
		void Tutorial (object stuff = null)
		{
			currentPlayer.BranchProgression [0].BranchState = CompletionState.started;
			currentPlayer.WriteData (currentPlayer);
			var nextTutorialLevel = GetNextTutorialLevelToPlay ();
			var layer = TutorialLayer.CreateScene (Window, nextTutorialLevel, currentPlayer);
			var transitionToLayer = new CCTransitionSlideInR(0.2f, layer);
			Director.ReplaceScene (transitionToLayer);
		}

		//---------------------------------------------------------------------------------------------------------
		// Challenges
		//---------------------------------------------------------------------------------------------------------
		// Transitions to the Select Challenge layer
		//---------------------------------------------------------------------------------------------------------
		void Challenges (object stuff = null)
		{
			var layer = SelectChallengeLayer.CreateScene (Window, currentPlayer);
			var transitionToLayer = new CCTransitionSlideInR(0.2f, layer);
			Director.ReplaceScene (transitionToLayer);
		}

		//---------------------------------------------------------------------------------------------------------
		// FreePlay
		//---------------------------------------------------------------------------------------------------------
		// Transitions to the Free Play layer
		//---------------------------------------------------------------------------------------------------------
		void FreePlay (object stuff = null)
		{
			var layer = FreePlayLayer.CreateScene (Window, currentPlayer);
			var transitionToLayer = new CCTransitionSlideInR(0.2f, layer);
			Director.ReplaceScene (transitionToLayer);
		}

		//---------------------------------------------------------------------------------------------------------
		// BackToMain
		//---------------------------------------------------------------------------------------------------------
		// Returns to Main Menu
		//---------------------------------------------------------------------------------------------------------
		void BackToMain (object stuff = null)
		{
			var layer = MenuLayer.CreateScene (Window);
			var transitionToLayer = new CCTransitionSlideInL(0.2f, layer);
			Director.ReplaceScene (transitionToLayer);
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

