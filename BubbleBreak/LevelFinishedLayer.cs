//---------------------------------------------------------------------------------------------
// <copyright file="LevelFinished.cs" company="RetroTek Software Ltd">
//     Copyright (C) 2016 RetroTek Software Ltd. All rights reserved.
// </copyright>
// <author>Jared Mathes</author>
//---------------------------------------------------------------------------------------------

using System.Collections.Generic;
using CocosSharp;

namespace BubbleBreak
{

	//---------------------------------------------------------------------------------------------------------
	// LevelFinishedLayer
	//---------------------------------------------------------------------------------------------------------
	// This class will create either a Level Incomplete layer or Level Complete layer depending on how the
	// level was finished
	//---------------------------------------------------------------------------------------------------------
	public class LevelFinishedLayer : CCLayerColor
	{
		const string GOTHIC_30_HD_FNT = "gothic-30-hd.fnt";
		const string GOTHIC_44_HD_FNT = "gothic-44-hd.fnt";

		CCSpriteSheet uiSpriteSheet;
		CCSprite frameSprite;
		CCLabel scoreLabel, coinsLabel, menuNextLevelLabel, menuQuitLevelLabel, menuRetryLevelLabel, menuShopLabel, frameTitleLabel;

		string scoreMessage = string.Empty;
		string coinsMessage = string.Empty;

		int playerScore;
		bool levelWasCompleted;
		int levelScore;
		int totalCoinsEarned;

		Player activePlayer;
		List<Level> levels;

		public LevelFinishedLayer (int score, List<Level> gameLevels, Player currentPlayer, bool wasLevelPassed, int coinsEarned)
		{
			levels = gameLevels;
			activePlayer = currentPlayer;
			playerScore = score;
			totalCoinsEarned = coinsEarned;
			//levelScore = levels [activePlayer.LastLevelCompleted + 1].LevelPassScore;
			levelScore = levels [activePlayer.BranchProgression[1].LastLevelCompleted + 1].LevelPassScore;
			levelWasCompleted = wasLevelPassed;
			Color = new CCColor3B(0,0,0);
			Opacity = 255;

			uiSpriteSheet = new CCSpriteSheet ("ui.plist");
		}

		protected override void AddedToScene ()
		{
			base.AddedToScene ();

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
			

		public static CCScene CreateScene (CCWindow mainWindow, int score, List<Level> gameLevels, Player currentPlayer, bool levelPassed, int coinsEarned)
		{
			var scene = new CCScene (mainWindow);
			var layer = new LevelFinishedLayer (score, gameLevels, currentPlayer, levelPassed, coinsEarned);

			scene.AddChild (layer);

			return scene;
		} 

		void NextLevel (object stuff = null)
		{
			var nextLevel = LevelLayer.CreateScene (Window, levels, activePlayer);
			var transitionToGame = new CCTransitionFade (2.0f, nextLevel);
			Director.ReplaceScene (transitionToGame);
		}

		void RetryLevel (object stuff = null)
		{
			var level = LevelLayer.CreateScene (Window, levels, activePlayer);
			var transitionToGame = new CCTransitionFade (2.0f, level);
			Director.ReplaceScene (transitionToGame);
		}

		void GoToShop (object stuff = null)
		{
//			var layer = ShopLayer.CreateScene (Window, activePlayer);
//			var transitionToGame = new CCTransitionSlideInR(0.2f, layer);
//			Director.ReplaceScene (transitionToGame);
		}

		void QuitLevel (object stuff = null)
		{
			var mainMenu = GameStartLayer.CreateScene (Window);
			var transitionToMainMenu = new CCTransitionFade (2.0f, mainMenu);
			Director.ReplaceScene (transitionToMainMenu);
		}
	}
}
