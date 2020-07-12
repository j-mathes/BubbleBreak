// -----------------------------------------------------------------------------------------------
//  <copyright file="StartLayer.cs" company="RetroTek Software Ltd">
//      Copyright (C) 2016 RetroTek Software Ltd. All rights reserved.
//  </copyright>
//  <author>Jared Mathes</author>
// -----------------------------------------------------------------------------------------------
using System;
using System.IO;
using System.Collections.Generic;
using CocosSharp;
using Foundation;


namespace BubbleBreak
{
	public class StartLayer : CCLayerColor
	{
		const string GOTHIC_30_HD_FNT = "gothic-30-hd.fnt";
		const string GOTHIC_44_HD_FNT = "gothic-44-hd.fnt";
		const string GOTHIC_56_WHITE_HD_FNT = "gothic-56-white-hd.fnt";
		const string GOTHIC_56_WHITE_FNT = "gothic-56-white.fnt";

		CCSpriteSheet uiSpriteSheet;

		CCLabel backLabel;

		CCSprite slotStdSprite, slotSelSprite;
		CCSprite playerDeleteStdSprite,playerDeleteSelSprite, playerInfoStdSprite, playerInfoSelSprite;

		CCMenuItemImage slot1MenuItem, slot2MenuItem, slot3MenuItem;
		CCMenuItemImage player1DeleteMenuItem, player2DeleteMenuItem, player3DeleteMenuItem;
		CCMenuItemImage player1InfoMenuItem, player2InfoMenuItem, player3InfoMenuItem;

		List<CCMenuItemImage> playerSlotItems;
		CCMenu playerSlotsMenu;
		CCMenu playerSlot1SubMenu, playerSlot2SubMenu, playerSlot3SubMenu;

		CCLabel slot1NewGameLabel, slot2NewGameLabel, slot3NewGameLabel;
		CCLabel slot1NameLabel, slot2NameLabel, slot3NameLabel;
		CCLabel slot1CoinsLabel, slot2CoinsLabel, slot3CoinsLabel;

		Player player1, player2, player3;

		CCRect bounds;

		NSUrl docDir;
		string Player1DataFile, Player2DataFile, Player3DataFile;

		//---------------------------------------------------------------------------------------------------------
		// StartLayer Constructor
		//---------------------------------------------------------------------------------------------------------
		public StartLayer () : base (new CCColor4B(0,0,0))
		{
			Color = new CCColor3B(0,0,0);
			Opacity = 255;

			// define spritesheets
			uiSpriteSheet = new CCSpriteSheet("ui.plist");

			docDir = NSFileManager.DefaultManager.GetUrls (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User) [0];

			Player1DataFile = docDir.AbsoluteUrl.RelativePath + "/player1data.xml";
			Player2DataFile = docDir.AbsoluteUrl.RelativePath + "/player2data.xml";
			Player3DataFile = docDir.AbsoluteUrl.RelativePath + "/player3data.xml";
		}

		//---------------------------------------------------------------------------------------------------------
		// AddedToScene
		//--------------------------------------------------------------------------------------------------------- 
		protected override void AddedToScene ()
		{
			base.AddedToScene ();

			// Use the bounds to layout the positioning of our drawable assets
			bounds = VisibleBoundsWorldspace;

			// read player data from file if it exists, if not, create file

			// player 1
			if (File.Exists (Player1DataFile)) {
				player1 = Player.ReadData (Player1DataFile); // to prevent list double entries when reading from file, in the constructor with no parameters, don't include the Add statements to the list items.  Just initialize them
			} else {
				player1 = new Player (PlayerSlot.slot1);
				player1.WriteData (player1);
			}

			// player 2
			if (File.Exists (Player2DataFile)) {
				player2 = Player.ReadData (Player2DataFile);
			} else {
				player2 = new Player (PlayerSlot.slot2);
				player2.WriteData (player2);
			}

			// player 3
			if (File.Exists (Player3DataFile)) {
				player3 = Player.ReadData (Player3DataFile);
			} else {
				player3 = new Player (PlayerSlot.slot3);
				player3.WriteData (player3);
			}

			SetupUI (bounds);
		}

		//---------------------------------------------------------------------------------------------------------
		// CreateScene
		//---------------------------------------------------------------------------------------------------------
		public static CCScene CreateScene (CCWindow mainWindow)
		{
			var scene = new CCScene (mainWindow);
			var layer = new StartLayer ();

			scene.AddChild (layer);

			return scene;
		}

		//---------------------------------------------------------------------------------------------------------
		// SetupUI
		//---------------------------------------------------------------------------------------------------------
		// Sets up the game UI
		//---------------------------------------------------------------------------------------------------------
		void SetupUI (CCRect screenBounds)
		{

			slot1NewGameLabel = new CCLabel ("New Game", GOTHIC_56_WHITE_HD_FNT);
			slot2NewGameLabel = new CCLabel ("New Game", GOTHIC_56_WHITE_HD_FNT);
			slot3NewGameLabel = new CCLabel ("New Game", GOTHIC_56_WHITE_HD_FNT);

			slot1NameLabel = new CCLabel (player1.Name, GOTHIC_56_WHITE_FNT) { HorizontalAlignment = CCTextAlignment.Left};
			slot2NameLabel = new CCLabel (player2.Name, GOTHIC_56_WHITE_FNT) { HorizontalAlignment = CCTextAlignment.Left};
			slot3NameLabel = new CCLabel (player3.Name, GOTHIC_56_WHITE_FNT) { HorizontalAlignment = CCTextAlignment.Left};

			slot1CoinsLabel = new CCLabel ("Coins: " + player1.Coins, GOTHIC_56_WHITE_FNT);
			slot2CoinsLabel = new CCLabel ("Coins: " + player2.Coins, GOTHIC_56_WHITE_FNT);
			slot3CoinsLabel = new CCLabel ("Coins: " + player3.Coins, GOTHIC_56_WHITE_FNT);

			slotStdSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("slot_std.png")));
			slotSelSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("slot_sel.png")));

			// Setup Slot 1
			slot1MenuItem = new CCMenuItemImage (slotStdSprite, slotSelSprite, PlayerSelect) {
				UserData = "Slot1",
				ZoomBehaviorOnTouch = false,
				AnchorPoint = CCPoint.AnchorMiddle,
			};

			if (player1.HasStarted) {
				slot1MenuItem.AddChild (slot1NameLabel);
				slot1MenuItem.AddChild (slot1CoinsLabel);
				slot1NameLabel.PositionX = slot1NameLabel.Parent.AnchorPointInPoints.X - 200;
				slot1NameLabel.PositionY = slot1NameLabel.Parent.AnchorPointInPoints.Y;
				slot1CoinsLabel.PositionX = slot1CoinsLabel.Parent.AnchorPointInPoints.X + 200;
				slot1CoinsLabel.PositionY = slot1CoinsLabel.Parent.AnchorPointInPoints.Y;
			} else {
				slot1MenuItem.AddChild (slot1NewGameLabel);
				slot1NewGameLabel.Position = slot1NewGameLabel.Parent.AnchorPointInPoints;
			}

			// Setup Slot 2
			slot2MenuItem = new CCMenuItemImage (slotStdSprite, slotSelSprite, PlayerSelect) {
				UserData = "Slot2",
				ZoomBehaviorOnTouch = false,
			};

			if (player2.HasStarted) {
				slot2MenuItem.AddChild (slot2NameLabel);
				slot2MenuItem.AddChild (slot2CoinsLabel);
				slot2NameLabel.PositionX = slot2NameLabel.Parent.AnchorPointInPoints.X - 200;
				slot2NameLabel.PositionY = slot2NameLabel.Parent.AnchorPointInPoints.Y;
				slot2CoinsLabel.PositionX = slot2CoinsLabel.Parent.AnchorPointInPoints.X + 200;
				slot2CoinsLabel.PositionY = slot2CoinsLabel.Parent.AnchorPointInPoints.Y;
			} else {
				slot2MenuItem.AddChild (slot2NewGameLabel);
				slot2NewGameLabel.Position = slot2NewGameLabel.Parent.AnchorPointInPoints;
			}

			// Setup Slot 3
			slot3MenuItem = new CCMenuItemImage (slotStdSprite, slotSelSprite, PlayerSelect) {
				UserData = "Slot3",
				ZoomBehaviorOnTouch = false,
			};

			if (player3.HasStarted) {
				slot3MenuItem.AddChild (slot3NameLabel);
				slot3MenuItem.AddChild (slot3CoinsLabel);
				slot3NameLabel.PositionX = slot3NameLabel.Parent.AnchorPointInPoints.X - 200;
				slot3NameLabel.PositionY = slot3NameLabel.Parent.AnchorPointInPoints.Y;
				slot3CoinsLabel.PositionX = slot3CoinsLabel.Parent.AnchorPointInPoints.X + 200;
				slot3CoinsLabel.PositionY = slot3CoinsLabel.Parent.AnchorPointInPoints.Y;
			} else {
				slot3MenuItem.AddChild (slot3NewGameLabel);
				slot3NewGameLabel.Position = slot3NewGameLabel.Parent.AnchorPointInPoints;
			}

			playerSlotItems = new List<CCMenuItemImage> ();
			playerSlotItems.Add (slot1MenuItem);
			playerSlotItems.Add (slot2MenuItem);
			playerSlotItems.Add (slot3MenuItem);

			playerSlotsMenu = new CCMenu (slot1MenuItem, slot2MenuItem, slot3MenuItem) {
				AnchorPoint = CCPoint.AnchorMiddleTop,
				PositionX = screenBounds.Size.Width / 2,
				PositionY = (screenBounds.Size.Height / 5) * 2.7f,
			};
			playerSlotsMenu.AlignItemsVertically (150);
			AddChild (playerSlotsMenu);

			playerDeleteStdSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("red_x_std.png")));
			playerDeleteSelSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("red_x_sel.png")));
			playerInfoStdSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("info_std.png")));
			playerInfoSelSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("info_sel.png")));

			player1DeleteMenuItem = new CCMenuItemImage (playerDeleteStdSprite, playerDeleteSelSprite, PlayerDelete) {
				UserData = "Slot1",
				ZoomBehaviorOnTouch = false,
			};
			player2DeleteMenuItem = new CCMenuItemImage (playerDeleteStdSprite, playerDeleteSelSprite, PlayerDelete) {
				UserData = "Slot2",
				ZoomBehaviorOnTouch = false,
			};
			player3DeleteMenuItem = new CCMenuItemImage (playerDeleteStdSprite, playerDeleteSelSprite, PlayerDelete) {
				UserData = "Slot3",
				ZoomBehaviorOnTouch = false,
			};

			player1InfoMenuItem = new CCMenuItemImage (playerInfoStdSprite, playerInfoSelSprite, PlayerInfo) {
				UserData = "Slot1",
				ZoomBehaviorOnTouch = false,
			};
			player2InfoMenuItem = new CCMenuItemImage (playerInfoStdSprite, playerInfoSelSprite, PlayerInfo) {
				UserData = "Slot2",
				ZoomBehaviorOnTouch = false,
			};
			player3InfoMenuItem = new CCMenuItemImage (playerInfoStdSprite, playerInfoSelSprite, PlayerInfo) {
				UserData = "Slot3",
				ZoomBehaviorOnTouch = false,
			};

			if (player1.HasStarted) {
				playerSlot1SubMenu = new CCMenu (player1InfoMenuItem, player1DeleteMenuItem) {
					AnchorPoint = CCPoint.AnchorMiddleTop,
					PositionX = playerSlotsMenu.Children [0].PositionWorldspace.X,
					PositionY = playerSlotsMenu.Children [0].PositionWorldspace.Y - 205,
				};
				playerSlot1SubMenu.AlignItemsHorizontally (400);
				AddChild (playerSlot1SubMenu);
			}

			if (player2.HasStarted) {
				playerSlot2SubMenu = new CCMenu (player2InfoMenuItem, player2DeleteMenuItem) {
					AnchorPoint = CCPoint.AnchorMiddleTop,
					PositionX = playerSlotsMenu.Children [1].PositionWorldspace.X,
					PositionY = playerSlotsMenu.Children [1].PositionWorldspace.Y - 205,
				};
				playerSlot2SubMenu.AlignItemsHorizontally (400);
				AddChild (playerSlot2SubMenu);
			}
			if (player3.HasStarted) {
				playerSlot3SubMenu = new CCMenu (player3InfoMenuItem, player3DeleteMenuItem) {
					AnchorPoint = CCPoint.AnchorMiddleTop,
					PositionX = playerSlotsMenu.Children [2].PositionWorldspace.X,
					PositionY = playerSlotsMenu.Children [2].PositionWorldspace.Y - 205,
				};
				playerSlot3SubMenu.AlignItemsHorizontally (400);
				AddChild (playerSlot3SubMenu);
			}

			backLabel = new CCLabel ("Back", GOTHIC_44_HD_FNT) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 1.5f,
			};

			var backItem = new CCMenuItemLabel (backLabel, BackToMain);

			var backMenu = new CCMenu (backItem);
			backMenu.AnchorPoint = CCPoint.AnchorMiddleBottom;
			backMenu.Position = new CCPoint (screenBounds.Size.Width / 2, 220f);

			AddChild (backMenu);
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
		// PlayerSelect
		//---------------------------------------------------------------------------------------------------------
		// Selects a player and transitions to the GameSelect layer
		//---------------------------------------------------------------------------------------------------------
		void PlayerSelect (object stuff)
		{
			var layer = new CCScene(Window);
			var sentMenuItem = (CCMenuItemImage)stuff;
			var playerSlotString = sentMenuItem.UserData.ToString ();

			if (playerSlotString == "Slot1") {
				if (!player1.HasStarted) player1.HasStarted = true;
				player1.WriteData (player1);
				layer = GameSelectLayer.CreateScene (Window, player1);
			}

			if (playerSlotString == "Slot2") {
				if (!player2.HasStarted) player2.HasStarted = true;
				player2.WriteData (player2);
				layer = GameSelectLayer.CreateScene (Window, player2);
			}

			if (playerSlotString == "Slot3") {
				if (!player3.HasStarted) player3.HasStarted = true;
				player3.WriteData (player3);
				layer = GameSelectLayer.CreateScene (Window, player3);
			}

			var transitionToLayer = new CCTransitionSlideInR(0.2f, layer);
			Director.ReplaceScene (transitionToLayer);
		}

		//---------------------------------------------------------------------------------------------------------
		// PlayerInfo
		//---------------------------------------------------------------------------------------------------------
		// Displays the player information popup
		// TODO: modify the information shown to match player class attributes
		//---------------------------------------------------------------------------------------------------------
		void PlayerInfo (object stuff)
		{
			Player currentPlayer;
			var sentMenuItem = (CCMenuItemImage)stuff;
			var playerSlotString = sentMenuItem.UserData.ToString ();

			if (playerSlotString == "Slot1") {
				currentPlayer = player1;
			} else if (playerSlotString == "Slot2") {
				currentPlayer = player2;
			} else if (playerSlotString == "Slot3") {
				currentPlayer = player3;
			}

			PauseListeners (true);
			Application.Paused = true;

			var playerStatsLayer = new CCLayerColor (new CCColor4B (0, 0, 0, 230));
			AddChild (playerStatsLayer, 99999);

			// Add frame to layer
			var frameSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("frame.png")));
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
			//var lastLevelLabel = new CCLabel ((currentPlayer.LastLevelCompleted == -1) ?  "Last Level Completed: Not Started Yet" : string.Format ("Last Level Completed: {0}", currentPlayer.LastLevelCompleted), GOTHIC_56_WHITE_FNT) {
			// TODO: instead of last level competed, change to report next level to be played
			var lastLevelLabel = new CCLabel ((currentPlayer.BranchProgression[1].BranchState == CompletionState.notStarted) ?  "Last Level Completed: Not Started Yet" : string.Format ("Last Level Completed: {0}", currentPlayer.BranchProgression[1].LastLevelCompleted), GOTHIC_56_WHITE_FNT) {
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
		// PlayerDelete
		//---------------------------------------------------------------------------------------------------------
		// Deletes player information
		//---------------------------------------------------------------------------------------------------------
		void PlayerDelete (object stuff)
		{
			PauseListeners (true);
			Application.Paused = true;

			var playerResetLayer = new CCLayerColor (new CCColor4B (0, 0, 0, 230));
			AddChild (playerResetLayer, 99999);

			// Add frame to layer
			var frameSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("frame.png")));
			frameSprite.AnchorPoint = CCPoint.AnchorMiddle;
			frameSprite.Position = new CCPoint (bounds.Size.Width / 2, bounds.Size.Height / 2);
			playerResetLayer.AddChild (frameSprite);

			string playerName = string.Empty;
			var sentMenuItem = (CCMenuItemImage)stuff;
			var playerSlotString = sentMenuItem.UserData.ToString ();

			if (playerSlotString == "Slot1") {
				playerName = player1.Name; 
			} else if (playerSlotString == "Slot2") {
				playerName = player1.Name;
			} else if (playerSlotString == "Slot3") {
				playerName = player1.Name;
			}

			var playerNameLabel = new CCLabel (playerName, GOTHIC_56_WHITE_HD_FNT);
			playerNameLabel.AnchorPoint = CCPoint.AnchorMiddle;
			playerNameLabel.Scale = 2;
			playerNameLabel.PositionX = frameSprite.BoundingBox.Center.X;
			playerNameLabel.PositionY = frameSprite.BoundingBox.MaxY - 200;
			playerNameLabel.HorizontalAlignment = CCTextAlignment.Center;
			playerResetLayer.AddChild (playerNameLabel);

			var newGameWarning = new CCLabel ("This will erase your current progress!\n\n\nProceed?", GOTHIC_56_WHITE_FNT);
			newGameWarning.AnchorPoint = CCPoint.AnchorMiddle;
			newGameWarning.Scale = 1.5f;
			newGameWarning.Position = new CCPoint(frameSprite.BoundingBox.Center);
			newGameWarning.HorizontalAlignment = CCTextAlignment.Center;
			playerResetLayer.AddChild (newGameWarning);

			var okLabel = new CCLabel ("OK", GOTHIC_44_HD_FNT);
			okLabel.AnchorPoint = CCPoint.AnchorMiddle;
			okLabel.Scale = 1.5f;

			var cancelLabel = new CCLabel ("Cancel", GOTHIC_44_HD_FNT);
			cancelLabel.AnchorPoint = CCPoint.AnchorMiddle;
			cancelLabel.Scale = 1.5f;

			var okItem = new CCMenuItemLabel (okLabel, okSender => {
				playerResetLayer.RemoveFromParent ();
				ResumeListeners (true);
				Application.Paused = false;

				if (playerSlotString == "Slot1") {
					player1 = new Player (PlayerSlot.slot1);
					player1.WriteData (player1);
				} else if (playerSlotString == "Slot2") {
					player2 = new Player (PlayerSlot.slot2);
					player2.WriteData (player2);
				} else if (playerSlotString == "Slot3") {
					player3 = new Player (PlayerSlot.slot3);
					player3.WriteData (player3);
				}

				UpdateUI ();
			});
			okItem.Position = bounds.Center;

			var cancelItem = new CCMenuItemLabel (cancelLabel, cancelSender => {
				playerResetLayer.RemoveFromParent ();
				ResumeListeners (true);
				Application.Paused = false;
			});
			cancelItem.Position = bounds.Center;

			var closeMenu = new CCMenu (okItem, cancelItem);
			closeMenu.AlignItemsHorizontally (50);
			closeMenu.AnchorPoint = CCPoint.AnchorMiddleBottom;
			closeMenu.Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MinY + (okLabel.BoundingBox.Size.Height * 2.5f));

			playerResetLayer.AddChild (closeMenu);
		}

		//---------------------------------------------------------------------------------------------------------
		// UpdateUI
		//---------------------------------------------------------------------------------------------------------
		// Updates UI elements
		//---------------------------------------------------------------------------------------------------------
		void UpdateUI ()
		{
			RemoveAllChildren ();
			SetupUI (bounds);
		}
	}
}

