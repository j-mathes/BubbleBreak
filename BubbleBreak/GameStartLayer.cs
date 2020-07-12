//---------------------------------------------------------------------------------------------
// <copyright file="GameStartLayer.cs" company="RetroTek Software Ltd">
//     Copyright (C) 2016 RetroTek Software Ltd. All rights reserved.
// </copyright>
// <author>Jared Mathes</author>
//---------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CocosSharp;

namespace BubbleBreak
{
	public class GameStartLayer : CCLayerColor
	{
		const string GOTHIC_30_HD_FNT = "gothic-30-hd.fnt";
		const string GOTHIC_44_HD_FNT = "gothic-44-hd.fnt";
		const string GOTHIC_56_WHITE_HD_FNT = "gothic-56-white-hd.fnt";
		const string GOTHIC_56_WHITE_FNT = "gothic-56-white.fnt";

		const string LEVEL_DATA_FOLDER = "./LevelData/";

		const float BUBBLE_BUFFER = 1.0f;
		const float BUBBLE_SCALE = 1.15f;
		const int MAX_VISIBLE_BUBBLES = 40;
		const int MAX_SCREEN_VISIBLE_BUBBLES = 30;
		const int MAX_BUBBLES_X = 5;
		const int MAX_BUBBLES_Y = 8;
		const float SCREEN_X_MARGIN = 40.0f;
		const float SCREEN_Y_MARGIN = 120f;
		const float CELL_DIMS_HALF = 100f;
		const float CELL_CENTER_ZONE_HALF = 28f;
		const int BUBBLES_VISIBLE_INCREMENT_DELAY = 10; // delay in 10ths of a second to increase the number of bubbles on the screen

		CCNode Bubbles;

		bool[] BubbleOccupiedArray = new bool[MAX_VISIBLE_BUBBLES];

		CCSprite rtLogo, title;
		CCSprite optionsStd, optionsSel, frameSprite;

		CCLabel okLabel, cancelLabel;
		CCLabel newGameWarning;
		CCLabel newGameLabel, continueGameLabel, playerStatsLabel, shopLabel;

		CCMenuItemToggle highlightToggleMenuItem;

		CCRepeatForever repeatedAction;
		CCSpriteSheet uiSpriteSheet;

		int timeIncrement;
		int visibleBubbles = 1;

		List<Branch> branches;
		List<Level> levels;
		XDocument branchInfo = new XDocument ();
		XDocument levelInfo = new XDocument ();

		Player currentPlayer;

		//---------------------------------------------------------------------------------------------------------
		// GameStartLayer Constructor
		//---------------------------------------------------------------------------------------------------------
		//
		public GameStartLayer () : base (new CCColor4B(0,0,0))
		{
			Color = new CCColor3B(0,0,0);
			Opacity = 255;

			// define spritesheets
			uiSpriteSheet = new CCSpriteSheet("ui.plist");

			// Define Actions

			var inflate = new CCScaleBy (0.7f, 1.1f);
			var deflate = new CCScaleBy (4.0f, 0.9f);

			//Define Sequence of actions

			var actionSeq = new CCSequence (new CCEaseElasticIn (inflate), new CCEaseExponentialOut(deflate), new CCDelayTime (7.0f));

			repeatedAction = new CCRepeatForever (actionSeq);

			StartScheduling ();
		}

		//---------------------------------------------------------------------------------------------------------
		// AddedToScene
		//---------------------------------------------------------------------------------------------------------
		// 
		protected override void AddedToScene ()
		{
			base.AddedToScene ();

			// Use the bounds to layout the positioning of our drawable assets
			CCRect bounds = VisibleBoundsWorldspace;

			Bubbles = new CCNode ();
			AddChild (Bubbles);

			//initialize every bool in BubbleArray to false.  True if there is a bubble there.
			for (int i = 0; i < MAX_BUBBLES_X; i++) {
				BubbleOccupiedArray [i] = false;
			}

			branches = ReadBranches (branchInfo);
			levels = ReadLevels (levelInfo);

			currentPlayer = new Player();


			if (File.Exists (currentPlayer.PlayerDataFile)) {
				currentPlayer = Player.ReadData (currentPlayer);
			} else {
				currentPlayer.WriteData (currentPlayer);
			}

			// options popup
			optionsStd = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("gear_std.png")));
			optionsStd.AnchorPoint = CCPoint.AnchorMiddle;
			optionsSel = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("gear_sel.png")));
			optionsSel.AnchorPoint = CCPoint.AnchorMiddle;

			var optionPopup = new CCMenuItemImage(optionsStd, optionsSel, sender => {

				PauseListeners (true);
				Application.Paused = true;

				var optionsLayer = new CCLayerColor (new CCColor4B (0, 0, 0, 200));
				AddChild (optionsLayer, 99999);

				// Add frame to layer
				frameSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("frame.png")));
				frameSprite.AnchorPoint = CCPoint.AnchorMiddle;
				frameSprite.Position = new CCPoint (bounds.Size.Width / 2, bounds.Size.Height / 2);
				optionsLayer.AddChild (frameSprite);

				var highlightOnLabel = new CCLabel ("Highlight Next: On", GOTHIC_56_WHITE_FNT) {
					AnchorPoint = CCPoint.AnchorMiddle,
					Scale = 2.0f
				};
				var highlightOffLabel = new CCLabel ("Highlight Next: Off", GOTHIC_56_WHITE_FNT) {
					AnchorPoint = CCPoint.AnchorMiddle,
					Scale = 2.0f
				};

				var highlightOnMenuItem = new CCMenuItemLabel (highlightOnLabel);  
				var highlightOffMenuItem = new CCMenuItemLabel (highlightOffLabel);

				highlightToggleMenuItem = new CCMenuItemToggle (ToggleHighlight, highlightOnMenuItem, highlightOffMenuItem);
				highlightToggleMenuItem.Enabled = currentPlayer.HighlightNextPurchased;
				highlightToggleMenuItem.PositionX = frameSprite.BoundingBox.MidX;
				highlightToggleMenuItem.PositionY = frameSprite.BoundingBox.MinY + (highlightToggleMenuItem.BoundingBox.Size.Height * 10f);
				highlightToggleMenuItem.SelectedIndex = (currentPlayer.IsHighlightNextActive) ? 0 : 1;

				var optionMenu = new CCMenu (highlightToggleMenuItem);
				optionMenu.AnchorPoint = CCPoint.AnchorMiddleBottom;
				optionMenu.Position = CCPoint.Zero;
				optionsLayer.AddChild (optionMenu);

				okLabel = new CCLabel ("OK", GOTHIC_44_HD_FNT);
				okLabel.AnchorPoint = CCPoint.AnchorMiddle;
				okLabel.Scale = 2.0f;

				var closeItem = new CCMenuItemLabel (okLabel, closeSender => {
					optionsLayer.RemoveFromParent ();
					ResumeListeners (true);
					Application.Paused = false;
				});

				closeItem.PositionX = frameSprite.BoundingBox.MidX;
				closeItem.PositionY = frameSprite.BoundingBox.MinY + (closeItem.BoundingBox.Size.Height * 1.5f);

				var closeMenu = new CCMenu (closeItem);
				closeMenu.AnchorPoint = CCPoint.AnchorMiddleBottom;
				closeMenu.Position = CCPoint.Zero;

				optionsLayer.AddChild (closeMenu);
			});

			optionPopup.AnchorPoint = CCPoint.AnchorMiddle;
			optionPopup.Position = new CCPoint(bounds.Size.Width / 10, bounds.Size.Height / 14);

			var optionItemsMenu = new CCMenu(optionPopup);
			optionItemsMenu.AnchorPoint = CCPoint.AnchorLowerLeft;
			optionItemsMenu.Position = CCPoint.Zero;

			AddChild(optionItemsMenu);

			//---------------------------------------------------------------------------------------------------------
			//Menu Elements
			//---------------------------------------------------------------------------------------------------------

			newGameLabel = new CCLabel ("New Game", GOTHIC_44_HD_FNT);
			newGameLabel.AnchorPoint = CCPoint.AnchorMiddle;
			newGameLabel.Scale = 2.0f;
			var menuItemNewGame = new CCMenuItemLabel (newGameLabel, NewGame);

			continueGameLabel = new CCLabel ("Continue", GOTHIC_44_HD_FNT);
			continueGameLabel.AnchorPoint = CCPoint.AnchorMiddle;
			continueGameLabel.Scale = 2.0f;
			var menuItemContinueGame = new CCMenuItemLabel (continueGameLabel, ContinueGame);

			playerStatsLabel = new CCLabel ("Player Stats", GOTHIC_44_HD_FNT);
			playerStatsLabel.AnchorPoint = CCPoint.AnchorMiddle;
			playerStatsLabel.Scale = 2.0f;
			var menuItemPlayerStats = new CCMenuItemLabel (playerStatsLabel, PlayerStats);

			shopLabel = new CCLabel ("Shop", GOTHIC_44_HD_FNT);
			shopLabel.AnchorPoint = CCPoint.AnchorMiddle;
			shopLabel.Scale = 2.0f;
			var menuItemShop = new CCMenuItemLabel (shopLabel, Shop);

			var menu = new CCMenu (menuItemNewGame, menuItemContinueGame, menuItemPlayerStats, menuItemShop) {
				Position = new CCPoint (bounds.Size.Width / 2, bounds.Size.Height / 2),
				AnchorPoint = CCPoint.AnchorMiddle
			};
			
			menu.AlignItemsVertically (150);
			
			AddChild (menu);

			rtLogo = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("bb_retrotek.png")));
			rtLogo.AnchorPoint = CCPoint.AnchorMiddle;
			rtLogo.Position = new CCPoint (bounds.Size.Width / 2, bounds.Size.Height / 14);
			//rtLogo.RunAction (repeatedAction);
			AddChild (rtLogo);

			title = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("bb_title.png")));
			title.AnchorPoint = CCPoint.AnchorMiddle;
			title.Position = new CCPoint (bounds.Size.Width / 2, (bounds.Size.Height / 7)*6);
			title.RunAction (repeatedAction);
			AddChild (title);

			//if (currentPlayer.LastLevelCompleted < 0) {
			if (currentPlayer.BranchProgression[1].LastLevelCompleted < 0) {
				menuItemNewGame.Enabled = true;
				menuItemContinueGame.Enabled = false;
				menuItemPlayerStats.Enabled = false;
			} else {
				menuItemNewGame.Enabled = true;
				menuItemContinueGame.Enabled = true;
				menuItemPlayerStats.Enabled = true;
			}

			Schedule (_ => CheckForFadedBubbles());
		}

		//---------------------------------------------------------------------------------------------------------
		// ToggleHighlight
		//---------------------------------------------------------------------------------------------------------
		// Used to toggle the "Highlight Next Bubble In Sequence" option to "On" if they have purchased it
		//---------------------------------------------------------------------------------------------------------
		void ToggleHighlight (object obj)
		{
			currentPlayer.IsHighlightNextActive = !currentPlayer.IsHighlightNextActive;
		}

		//---------------------------------------------------------------------------------------------------------
		// NewGame - Used in menu selection
		//---------------------------------------------------------------------------------------------------------
		// Transitions to LevelLayer
		//---------------------------------------------------------------------------------------------------------
		void NewGame (object stuff = null)
		{
			if (currentPlayer.BranchProgression[1].LastLevelCompleted > -1) {
			//if (currentPlayer.LastLevelCompleted > -1) {
				CCRect bounds = VisibleBoundsWorldspace;

				PauseListeners (true);
				Application.Paused = true;

				var newGameLayer = new CCLayerColor (new CCColor4B (0, 0, 0, 230));
				AddChild (newGameLayer, 99999);

				// Add frame to layer
				frameSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("frame.png")));
				frameSprite.AnchorPoint = CCPoint.AnchorMiddle;
				frameSprite.Position = new CCPoint (bounds.Size.Width / 2, bounds.Size.Height / 2);
				newGameLayer.AddChild (frameSprite);

				newGameWarning = new CCLabel ("This will erase your current progress!\n\n\nProceed?", GOTHIC_56_WHITE_FNT);
				newGameWarning.AnchorPoint = CCPoint.AnchorMiddle;
				newGameWarning.Scale = 1.5f;
				newGameWarning.Position = new CCPoint(frameSprite.BoundingBox.Center);
				newGameWarning.HorizontalAlignment = CCTextAlignment.Center;
				newGameLayer.AddChild (newGameWarning);

				okLabel = new CCLabel ("OK", GOTHIC_44_HD_FNT);
				okLabel.AnchorPoint = CCPoint.AnchorMiddle;
				okLabel.Scale = 1.5f;

				cancelLabel = new CCLabel ("Cancel", GOTHIC_44_HD_FNT);
				cancelLabel.AnchorPoint = CCPoint.AnchorMiddle;
				cancelLabel.Scale = 1.5f;

				var okItem = new CCMenuItemLabel (okLabel, okSender => {
					newGameLayer.RemoveFromParent ();
					ResumeListeners (true);
					Application.Paused = false;

					currentPlayer = new Player ();
					currentPlayer.WriteData (currentPlayer);

					var mainGame = LevelLayer.CreateScene (Window, levels, currentPlayer);
					var transitionToGame = new CCTransitionFade (2.0f, mainGame);
					Director.ReplaceScene (transitionToGame);
				});
				okItem.Position = bounds.Center;

				var cancelItem = new CCMenuItemLabel (cancelLabel, cancelSender => {
					newGameLayer.RemoveFromParent ();
					ResumeListeners (true);
					Application.Paused = false;
				});
				cancelItem.Position = bounds.Center;
				
				var closeMenu = new CCMenu (okItem, cancelItem);
				closeMenu.AlignItemsHorizontally (50);
				closeMenu.AnchorPoint = CCPoint.AnchorMiddleBottom;
				closeMenu.Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MinY + (okLabel.BoundingBox.Size.Height * 2.5f));

				newGameLayer.AddChild (closeMenu);
			} else {
				var mainGame = LevelLayer.CreateScene (Window, levels, currentPlayer);
				var transitionToGame = new CCTransitionFade(2.0f, mainGame);
				Director.ReplaceScene (transitionToGame);
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// ContinueGame - Used in menu selection
		//---------------------------------------------------------------------------------------------------------
		// Transitions to GameLayer
		//---------------------------------------------------------------------------------------------------------
		void ContinueGame (object stuff = null)
		{
			var mainGame = LevelLayer.CreateScene (Window, levels, currentPlayer);
			var transitionToGame = new CCTransitionFade(2.0f, mainGame);
			Director.ReplaceScene (transitionToGame);
		}

		//---------------------------------------------------------------------------------------------------------
		// PlayerStats 
		//---------------------------------------------------------------------------------------------------------
		// Show player information summary in a popup
		//---------------------------------------------------------------------------------------------------------
		void PlayerStats (object stuff = null)
		{
			CCRect bounds = VisibleBoundsWorldspace;

			PauseListeners (true);
			Application.Paused = true;

			var playerStatsLayer = new CCLayerColor (new CCColor4B (0, 0, 0, 230));
			AddChild (playerStatsLayer, 99999);

			// Add frame to layer
			frameSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("frame.png")));
			frameSprite.AnchorPoint = CCPoint.AnchorMiddle;
			frameSprite.Position = new CCPoint (bounds.Size.Width / 2, bounds.Size.Height / 2);
			frameSprite.ScaleY = 1.3f;
			playerStatsLayer.AddChild (frameSprite);

			var titleLabel = new CCLabel ("Player Stats", GOTHIC_56_WHITE_HD_FNT) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 1f};
			var titleMenuItem = new CCMenuItemLabel (titleLabel);
			var playerNameLabel = new CCLabel (string.Format ("Player Name: {0}", currentPlayer.Name), GOTHIC_56_WHITE_FNT) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 1.0f
			};
			var playerNameMenuItem = new CCMenuItemLabel (playerNameLabel);
			//var lastLevelLabel = new CCLabel (string.Format ("Last Level Completed: {0}", currentPlayer.LastLevelCompleted), GOTHIC_56_WHITE_FNT) {
			var lastLevelLabel = new CCLabel (string.Format ("Last Level Completed: {0}", currentPlayer.BranchProgression[1].LastLevelCompleted), GOTHIC_56_WHITE_FNT) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 1.0f
			};
			var lastLevelMenuItem = new CCMenuItemLabel (lastLevelLabel);
			var coinsLabel = new CCLabel (string.Format ("Coins: {0}", currentPlayer.Coins), GOTHIC_56_WHITE_FNT) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 1.0f
			};
			var coinsMenuItem = new CCMenuItemLabel (coinsLabel);
			var highScoreLabel = new CCLabel (string.Format ("High Score: {0}", currentPlayer.TotalScores), GOTHIC_56_WHITE_FNT) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 1.0f
			};
			var highScoreMenuItem = new CCMenuItemLabel (highScoreLabel);
			var tapStrengthLabel = new CCLabel (string.Format ("Tap Strength: {0}", currentPlayer.PersistentTapStrength), GOTHIC_56_WHITE_FNT) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 1.0f
			};
			var tapStrengthMenuItem = new CCMenuItemLabel (tapStrengthLabel);
			var percentChanceNextSeqLabel = new CCLabel (string.Format ("Next in Sequence Luck: {0}%", currentPlayer.ChanceToRollNextSeq), GOTHIC_56_WHITE_FNT) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 1.0f
			};
			var percentChanceNextSeqMenuItem = new CCMenuItemLabel (percentChanceNextSeqLabel);
			var percentChanceBonusLabel = new CCLabel (string.Format ("Bonus Bubble Luck: {0}%", currentPlayer.ChanceToRollBonus), GOTHIC_56_WHITE_FNT) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 1.0f
			};
			var percentChanceBonusMenuItem = new CCMenuItemLabel (percentChanceBonusLabel);

			var timeBonusLabel = new CCLabel (string.Format ("Additional Level Time: {0} Seconds", currentPlayer.PersistentTimeBonus), GOTHIC_56_WHITE_FNT) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 1.0f
			};
			var timeBonusMenuItem = new CCMenuItemLabel (timeBonusLabel);

			var time2xBonusLabel = new CCLabel (string.Format ("2x Bonus Timer Extra: {0} Seconds", currentPlayer.Persistent2xTimeBonus), GOTHIC_56_WHITE_FNT) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 1.0f
			};
			var time2xBonusMenuItem = new CCMenuItemLabel (time2xBonusLabel);

			var time3xBonusLabel = new CCLabel (string.Format ("3x Bonus Timer Extra: {0} Seconds", currentPlayer.Persistent3xTimeBonus), GOTHIC_56_WHITE_FNT) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 1.0f
			};
			var time3xBonusMenuItem = new CCMenuItemLabel (time3xBonusLabel);

			var highlightNextAvailableLabel = new CCLabel ("Highlight Next Sequence Bubble: " + ((currentPlayer.HighlightNextPurchased) ? "Yes" : "No"), GOTHIC_56_WHITE_FNT) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = Scale= 1.0f
			};
			var highlightNextMenuItem = new CCMenuItemLabel (highlightNextAvailableLabel);

			var okButtonLabel = new CCLabel ("OK", GOTHIC_30_HD_FNT) {
				AnchorPoint = AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 2.0f
			};
			var okButtonMenuItem = new CCMenuItemLabel (okButtonLabel, okSender => {
				playerStatsLayer.RemoveFromParent ();
				ResumeListeners (true);
				Application.Paused = false;
			});

			var playerStatsMenu = new CCMenu (titleMenuItem, playerNameMenuItem, lastLevelMenuItem, coinsMenuItem, highScoreMenuItem, tapStrengthMenuItem, percentChanceNextSeqMenuItem, percentChanceBonusMenuItem, 
				timeBonusMenuItem, time2xBonusMenuItem, time3xBonusMenuItem, highlightNextMenuItem, okButtonMenuItem);
			
			playerStatsMenu.AnchorPoint = CCPoint.AnchorMiddle;
			playerStatsMenu.Position = new CCPoint (bounds.Size.Width / 2, bounds.Size.Height / 2);
			playerStatsMenu.AlignItemsVertically (40);

			playerStatsLayer.AddChild (playerStatsMenu);
		}

		//---------------------------------------------------------------------------------------------------------
		// Shop
		//---------------------------------------------------------------------------------------------------------
		// Transitions to the Shop layer
		//---------------------------------------------------------------------------------------------------------

		void Shop (object stuff = null)
		{
//			var mainGame = ShopLayer.CreateScene (Window, currentPlayer);
//			var transitionToGame = new CCTransitionSlideInR(0.2f, mainGame);
//			Director.ReplaceScene (transitionToGame);
		}

		//---------------------------------------------------------------------------------------------------------
		// CreateScene
		//---------------------------------------------------------------------------------------------------------
		public static CCScene CreateScene (CCWindow mainWindow)
		{
			var scene = new CCScene (mainWindow);
			var layer = new GameStartLayer ();

			scene.AddChild (layer);

			return scene;
		}

		//---------------------------------------------------------------------------------------------------------
		// GetRandomPosition
		//---------------------------------------------------------------------------------------------------------
		// Get a position based on the index and sprite size
		//---------------------------------------------------------------------------------------------------------
		static CCPoint GetRandomPosition (int xIndex, int yIndex)
		{
			double rndX = CCRandom.GetRandomFloat ((xIndex * CELL_DIMS_HALF * 2) + (SCREEN_X_MARGIN + CELL_DIMS_HALF - CELL_CENTER_ZONE_HALF), (xIndex * CELL_DIMS_HALF * 2) + (SCREEN_X_MARGIN + CELL_DIMS_HALF + CELL_CENTER_ZONE_HALF));
			double rndY = CCRandom.GetRandomFloat ((yIndex * CELL_DIMS_HALF * 2) + (SCREEN_Y_MARGIN + CELL_DIMS_HALF - CELL_CENTER_ZONE_HALF), (yIndex * CELL_DIMS_HALF * 2) + (SCREEN_Y_MARGIN + CELL_DIMS_HALF + CELL_CENTER_ZONE_HALF));
			return new CCPoint ((float)rndX, (float)rndY);
		}

		//---------------------------------------------------------------------------------------------------------
		// DisplayBubble
		//---------------------------------------------------------------------------------------------------------
		// Displays a bubble
		//---------------------------------------------------------------------------------------------------------
		async void DisplayBubble(Bubble newBubble)
		{
			await newBubble.BubbleSprite.RunActionsAsync (new CCFadeIn (newBubble.TimeAppear), new CCDelayTime (newBubble.TimeHold), new CCFadeOut (newBubble.TimeFade));
			BubbleOccupiedArray [newBubble.ListIndex] = false;
			newBubble.RemoveFromParent ();
		}

		//---------------------------------------------------------------------------------------------------------
		// CheckForFadedBubbles
		//---------------------------------------------------------------------------------------------------------
		// Check to see if a bubble faded and died.  If so, replace it
		//---------------------------------------------------------------------------------------------------------
		void CheckForFadedBubbles()
		{
			//check if a bubble has popped on its own
			if (Bubbles.ChildrenCount < visibleBubbles) {
				var newBubbleOrder = new ShuffleBag<int> ();

				for (int i = 0; i < BubbleOccupiedArray.Length; i++) {
					if (!BubbleOccupiedArray [i])
						newBubbleOrder.Add (i);
				}

				var newBubbleIndex = newBubbleOrder.Next ();
				var newBubble = new MenuBubble ((newBubbleIndex % MAX_BUBBLES_X), (newBubbleIndex / MAX_BUBBLES_X), newBubbleIndex);
				CCPoint p = GetRandomPosition (newBubble.XIndex, newBubble.YIndex);
				newBubble.Position = new CCPoint (p.X, p.Y);
				newBubble.Scale = BUBBLE_SCALE;
				DisplayBubble (newBubble);
				Bubbles.AddChild (newBubble);
				BubbleOccupiedArray [newBubbleIndex] = true;
			}
		}

		//--------------------------------------------------
		// StartScheduling
		//--------------------------------------------------
		void StartScheduling()
		{
			Schedule (t => {
				timeIncrement++;

				if (timeIncrement > 0) {
					if (visibleBubbles < MAX_SCREEN_VISIBLE_BUBBLES){
					visibleBubbles = ((timeIncrement % BUBBLES_VISIBLE_INCREMENT_DELAY) == 0) ? visibleBubbles + 1 : visibleBubbles;
					}
				}

			},0.1f);
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
					UnlockSequenceUI = bool.Parse (level.Element ("UnlockSequenceUI").Value),
					UnlockBonusUI = bool.Parse (level.Element ("UnlockBonusUI").Value),
					UnlockConsumableUI = bool.Parse (level.Element ("UnlockConsumableUI").Value),
				}).ToList ();		
			return lvl;
		}
	}
}
