using System;
using System.IO;
using System.Collections.Generic;
using CocosSharp;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using Foundation;

namespace BubbleBreak
{
	public class GameStartLayer : CCLayerColor
	{
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

		CCNode bubbles;

		bool[] bubbleOccupiedArray = new bool[MAX_VISIBLE_BUBBLES];

		CCSprite rtLogo, title;
		CCSprite newGameStd, continueGameStd, playerStatsStd;
		CCSprite newGameSel, continueGameSel, playerStatsSel;
		CCSprite newGameDis, continueGameDis, playerStatsDis;
		CCSprite optionsStd, optionsSel, okStd, okSel, cancelStd, cancelSel, frameSprite;

		CCLabel newGameWarning;

		CCRepeatForever repeatedAction;
		CCSpriteSheet uiSpriteSheet;

		int timeIncrement = 0;
		int visibleBubbles = 1;
		int bubblesVisibleIncrementDelay = 10; // delay in 10ths of a second to increase the number of bubbles on the screen

		List<Level> levels;
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

			bubbles = new CCNode ();
			AddChild (bubbles);

			//initialize every bool in BubbleArray to false.  True if there is a bubble there.
			for (int i = 0; i < MAX_BUBBLES_X; i++) {
				bubbleOccupiedArray [i] = false;
			}

			levels = ReadLevels (levelInfo);

			currentPlayer = new Player();

			if (File.Exists (currentPlayer.PlayerDataFile)) {
				currentPlayer = Player.ReadData (currentPlayer);
			} else {
				currentPlayer.WriteData ();
			}

			// options popup
			optionsStd = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("gear_std.png")));
			optionsStd.AnchorPoint = CCPoint.AnchorMiddle;
			optionsSel = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("gear_sel.png")));
			optionsSel.AnchorPoint = CCPoint.AnchorMiddle;

			var optionPopup = new CCMenuItemImage(optionsStd, optionsSel, (sender) =>
				{

					this.PauseListeners(true);
					Application.Paused = true;

					var optionsLayer = new CCLayerColor(new CCColor4B(0, 0, 0, 200));
					AddChild(optionsLayer, 99999);

					// Add frame to layer
					frameSprite = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("frame.png")));
					frameSprite.AnchorPoint = CCPoint.AnchorMiddle;
					frameSprite.Position = new CCPoint (bounds.Size.Width / 2, bounds.Size.Height / 2);
					optionsLayer.AddChild (frameSprite);

					okStd = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("ok_std.png")));
					okStd.AnchorPoint = CCPoint.AnchorMiddle;
					okSel = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("ok_sel.png")));
					okSel.AnchorPoint = CCPoint.AnchorMiddle;

					var closeItem = new CCMenuItemImage(okStd, okSel, (closeSender) =>
						{
							optionsLayer.RemoveFromParent();
							this.ResumeListeners(true);
							Application.Paused = false;

						});

					closeItem.Position = bounds.Center;

					var closeMenu = new CCMenu(closeItem);
					closeMenu.AnchorPoint = CCPoint.AnchorMiddleBottom;
					closeMenu.Position = CCPoint.Zero;

					optionsLayer.AddChild(closeMenu);
				});

			optionPopup.AnchorPoint = CCPoint.AnchorMiddle;
			optionPopup.Position = new CCPoint(bounds.Size.Width / 10, bounds.Size.Height / 14);

			var optionMenu = new CCMenu(optionPopup);
			optionMenu.AnchorPoint = CCPoint.AnchorLowerLeft;
			optionMenu.Position = CCPoint.Zero;

			AddChild(optionMenu);

			//---------------------------------------------------------------------------------------------------------
			//Menu Elements
			//---------------------------------------------------------------------------------------------------------

			newGameStd = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("newgame_std.png")));
			newGameStd.AnchorPoint = CCPoint.AnchorMiddle;
			newGameSel = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("newgame_sel.png")));
			newGameSel.AnchorPoint = CCPoint.AnchorMiddle;
			newGameDis = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("newgame_dis.png")));
			newGameDis.AnchorPoint = CCPoint.AnchorMiddle;
			var menuItemNewGame = new CCMenuItemImage (newGameStd, newGameSel, newGameDis, NewGame);

			continueGameStd = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("continue_std.png")));
			continueGameStd.AnchorPoint = CCPoint.AnchorMiddle;
			continueGameSel = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("continue_sel.png")));
			continueGameSel.AnchorPoint = CCPoint.AnchorMiddle;
			continueGameDis = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("continue_dis.png")));
			continueGameDis.AnchorPoint = CCPoint.AnchorMiddle;
			var menuItemContinueGame = new CCMenuItemImage (continueGameStd, continueGameSel, continueGameDis, ContinueGame);

			playerStatsStd = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("playerstats_std.png")));
			playerStatsStd.AnchorPoint = CCPoint.AnchorMiddle;
			playerStatsSel = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("playerstats_sel.png")));
			playerStatsSel.AnchorPoint = CCPoint.AnchorMiddle;
			playerStatsDis = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("playerstats_dis.png")));
			playerStatsDis.AnchorPoint = CCPoint.AnchorMiddle;
			var menuItemPlayerStats = new CCMenuItemImage (playerStatsStd, playerStatsSel, playerStatsDis, PlayerStats);

			var menu = new CCMenu (menuItemNewGame, menuItemContinueGame, menuItemPlayerStats) {
				Position = new CCPoint (bounds.Size.Width / 2, bounds.Size.Height / 2),
				AnchorPoint = CCPoint.AnchorMiddle
			};
			
			menu.AlignItemsVertically (60);
			
			AddChild (menu);

			rtLogo = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("bb_retrotek.png")));
			rtLogo.AnchorPoint = CCPoint.AnchorMiddle;
			rtLogo.Position = new CCPoint (bounds.Size.Width / 2, bounds.Size.Height / 14);
			//rtLogo.RunAction (repeatedAction);
			AddChild (rtLogo);

			title = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("bb_title.png")));
			title.AnchorPoint = CCPoint.AnchorMiddle;
			title.Position = new CCPoint (bounds.Size.Width / 2, (bounds.Size.Height / 7)*6);
			title.RunAction (repeatedAction);
			AddChild (title);

			if (currentPlayer.LastLevelCompleted < 0) {
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
		// NewGame - Used in menu selection
		//---------------------------------------------------------------------------------------------------------
		// Transitions to LevelLayer
		//---------------------------------------------------------------------------------------------------------
		void NewGame (object stuff = null)
		{
			if (currentPlayer.LastLevelCompleted > -1) {
				CCRect bounds = VisibleBoundsWorldspace;

				this.PauseListeners (true);
				Application.Paused = true;

				var newGameLayer = new CCLayerColor (new CCColor4B (0, 0, 0, 230));
				AddChild (newGameLayer, 99999);

				// Add frame to layer
				frameSprite = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("frame.png")));
				frameSprite.AnchorPoint = CCPoint.AnchorMiddle;
				frameSprite.Position = new CCPoint (bounds.Size.Width / 2, bounds.Size.Height / 2);
				newGameLayer.AddChild (frameSprite);

				newGameWarning = new CCLabel ("This will erase your current progress!\n\nProceed?", "arial", 30);
				newGameWarning.AnchorPoint = CCPoint.AnchorMiddle;
				newGameWarning.Scale = 1.5f;
				newGameWarning.Position = new CCPoint(frameSprite.BoundingBox.Center);
				newGameWarning.HorizontalAlignment = CCTextAlignment.Center;
				newGameLayer.AddChild (newGameWarning);

				okStd = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("ok_std.png")));
				okStd.AnchorPoint = CCPoint.AnchorMiddle;
				okSel = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("ok_sel.png")));
				okSel.AnchorPoint = CCPoint.AnchorMiddle;
				cancelStd = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("cancel_std.png")));
				cancelStd.AnchorPoint = CCPoint.AnchorMiddle;
				cancelSel = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("cancel_sel.png")));
				cancelSel.AnchorPoint = CCPoint.AnchorMiddle;

				var okItem = new CCMenuItemImage (okStd, okSel, (okSender) => {
					newGameLayer.RemoveFromParent ();
					this.ResumeListeners (true);
					Application.Paused = false;

					currentPlayer = new Player ();
					currentPlayer.WriteData ();

					var mainGame = LevelLayer.CreateScene (Window, levels, currentPlayer);
					var transitionToGame = new CCTransitionFade (3.0f, mainGame);
					Director.ReplaceScene (transitionToGame);
				});
				okItem.Position = bounds.Center;

				var cancelItem = new CCMenuItemImage (cancelStd, cancelSel, (cancelSender) => {
					newGameLayer.RemoveFromParent ();
					this.ResumeListeners (true);
					Application.Paused = false;
				});
				cancelItem.Position = bounds.Center;
				
				var closeMenu = new CCMenu (okItem, cancelItem);
				closeMenu.AlignItemsHorizontally (50);
				closeMenu.AnchorPoint = CCPoint.AnchorMiddleBottom;
				closeMenu.Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MinY + (okStd.BoundingBox.Size.Height * 1.5f));

				newGameLayer.AddChild (closeMenu);
			} else {
				var mainGame = LevelLayer.CreateScene (Window, levels, currentPlayer);
				var transitionToGame = new CCTransitionFade(3.0f, mainGame);
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
			var transitionToGame = new CCTransitionFade(3.0f, mainGame);
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

			this.PauseListeners (true);
			Application.Paused = true;

			var playerStatsLayer = new CCLayerColor (new CCColor4B (0, 0, 0, 230));
			AddChild (playerStatsLayer, 99999);

			// Add frame to layer
			frameSprite = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("frame.png")));
			frameSprite.AnchorPoint = CCPoint.AnchorMiddle;
			frameSprite.Position = new CCPoint (bounds.Size.Width / 2, bounds.Size.Height / 2);
			playerStatsLayer.AddChild (frameSprite);

			var titleLabel = new CCLabel ("Player Stats", "arial", 30) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 2.5f};
			var titleMenuItem = new CCMenuItemLabel (titleLabel);
			var playerNameLabel = new CCLabel ("Player Name: " + currentPlayer.Name, "arial", 30) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 2.0f};
			var playerNameMenuItem = new CCMenuItemLabel (playerNameLabel);
			var lastLevelLabel = new CCLabel ("Last Level Completed: " + currentPlayer.LastLevelCompleted, "arial", 30) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 2.0f};
			var lastLevelMenuItem = new CCMenuItemLabel (lastLevelLabel);
			var coinsLabel = new CCLabel ("Coins: " + currentPlayer.Coins, "arial", 30) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 2.0f};
			var coinsMenuItem = new CCMenuItemLabel (coinsLabel);
			var highScoreLabel = new CCLabel ("High Score: " + currentPlayer.HighScore, "arial", 30) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 2.0f};
			var highScoreMenuItem = new CCMenuItemLabel (highScoreLabel);
			var tapStrengthLabel = new CCLabel ("Tap Strength: " + currentPlayer.TapStrength, "arial", 30) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 2.0f};
			var tapStrengthMenuItem = new CCMenuItemLabel (tapStrengthLabel);

			okStd = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("ok_std.png")));
			okStd.AnchorPoint = CCPoint.AnchorMiddle;
			okSel = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("ok_sel.png")));
			okSel.AnchorPoint = CCPoint.AnchorMiddle;

			var okItem = new CCMenuItemImage (okStd, okSel, (okSender) => {
				playerStatsLayer.RemoveFromParent ();
				this.ResumeListeners (true);
				Application.Paused = false;
			});

			var playerStatsMenu = new CCMenu (titleMenuItem, playerNameMenuItem, lastLevelMenuItem, coinsMenuItem, highScoreMenuItem, tapStrengthMenuItem, okItem);
			playerStatsMenu.AnchorPoint = CCPoint.AnchorMiddle;
			playerStatsMenu.Position = new CCPoint (bounds.Size.Width / 2, bounds.Size.Height / 2);
			playerStatsMenu.AlignItemsVertically (40);

			playerStatsLayer.AddChild (playerStatsMenu);
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
		CCPoint GetRandomPosition (CCSize spriteSize, int xIndex, int yIndex)
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
		async void DisplayBubble(MenuBubble newBubble)
		{
			await newBubble.BubbleSprite.RunActionsAsync (new CCFadeIn (newBubble.TimeAppear), new CCDelayTime (newBubble.TimeHold), new CCFadeOut (newBubble.TimeFade));
			bubbleOccupiedArray [newBubble.ListIndex] = false;
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
			if (bubbles.ChildrenCount < visibleBubbles) {
				ShuffleBag<int> newBubbleOrder = new ShuffleBag<int> ();

				for (int i = 0; i < bubbleOccupiedArray.Length; i++) {
					if (!bubbleOccupiedArray [i])
						newBubbleOrder.Add (i);
				}

				var newBubbleIndex = newBubbleOrder.Next ();
				var newBubble = new MenuBubble ((newBubbleIndex % MAX_BUBBLES_X), (newBubbleIndex / MAX_BUBBLES_X), newBubbleIndex);
				CCPoint p = GetRandomPosition ( newBubble.BubbleSprite.ContentSize * BUBBLE_SCALE, newBubble.XIndex, newBubble.YIndex);
				newBubble.Position = new CCPoint (p.X, p.Y);
				newBubble.Scale = BUBBLE_SCALE;
				DisplayBubble (newBubble);
				bubbles.AddChild (newBubble);
				bubbleOccupiedArray [newBubbleIndex] = true;
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
					visibleBubbles = ((timeIncrement % bubblesVisibleIncrementDelay) == 0) ? visibleBubbles + 1 : visibleBubbles;
					}
				}

			},0.1f);
		}

		//---------------------------------------------------------------------------------------------------------
		// ReadLevels
		//---------------------------------------------------------------------------------------------------------
		// Reads the contents of the levelInfo.xml file and builds a list of level objects with the data in the 
		// xml file.  Allows for easy changing of level setup and addint new levels
		//---------------------------------------------------------------------------------------------------------
		public List<Level> ReadLevels(XDocument levelInfo)
		{
			levelInfo = XDocument.Load ("./levelinfo.xml");

			List<Level> lvl = (from level in levelInfo.Root.Descendants ("record") // "record" has to match the record level identifier in the xml file
				select new Level {
					LevelNum = int.Parse (level.Element ("LevelNum").Value),
					LevelName = level.Element ("LevelName").Value,
					MaxBubbles = int.Parse (level.Element ("MaxBubbles").Value),
					MaxVisibleBubbles = int.Parse (level.Element ("MaxVisibleBubbles").Value),
					StartingPointValue = int.Parse (level.Element ("StartingPointValue").Value),
					MaxLevelTime = int.Parse (level.Element ("MaxLevelTime").Value),
					LevelPassScore = int.Parse (level.Element ("LevelPassScore").Value),
					TapsToPopStandard = int.Parse (level.Element ("TapsToPopStandard").Value),
					InitialVisibleBubbles = int.Parse (level.Element ("InitialVisibleBubbles").Value),
					LevelDescription = level.Element ("LevelDescription").Value,
					SequenceLevel = int.Parse (level.Element ("SequenceLevel").Value)
				}).ToList ();
			return lvl;
		}
	}
}
