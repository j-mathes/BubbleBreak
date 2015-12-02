using System;
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
		CCSpriteSheet uiSpriteSheet;
		CCSprite frameSprite, frameTitle, menuNextLevel, menuQuitLevel, menuRetryLevel;
		CCLabel scoreLabel, coinsLabel;

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
			levelScore = levels [activePlayer.LastLevelCompleted + 1].LevelPassScore;
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
			frameSprite = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("frame.png")));
			frameSprite.AnchorPoint = CCPoint.AnchorMiddle;
			frameSprite.Position = new CCPoint (bounds.Size.Width / 2, bounds.Size.Height / 2);
			AddChild (frameSprite);

			// Add frame title;
			frameTitle = new CCSprite ((levelWasCompleted) ? (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("level_com.png"))) : (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("level_inc.png"))));
			frameTitle.AnchorPoint = CCPoint.AnchorMiddle;
			frameTitle.Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MaxY - (frameTitle.BoundingBox.Size.Height * 1.5f));
			AddChild (frameTitle);

			scoreMessage = (levelWasCompleted) ? string.Format ("Your score is {0}\n", playerScore) : string.Format("You needed {0} more points\nto pass the level", levelScore - playerScore);
			coinsMessage = (totalCoinsEarned > 0) ? string.Format ("You earned {0} coins", totalCoinsEarned) : string.Empty;

			scoreLabel = new CCLabel (scoreMessage, "arial", 30);
			scoreLabel.AnchorPoint = CCPoint.AnchorMiddle;
			scoreLabel.Scale = 2f;
			scoreLabel.Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MinY + (scoreLabel.BoundingBox.Size.Height * 9f));
			scoreLabel.HorizontalAlignment = CCTextAlignment.Center;
			AddChild (scoreLabel);

			coinsLabel = new CCLabel (coinsMessage, "arial", 30);
			coinsLabel.AnchorPoint = CCPoint.AnchorMiddle;
			coinsLabel.Scale = 2f;
			coinsLabel.Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MinY + (scoreLabel.BoundingBox.Size.Height * 7.5f));
			coinsLabel.HorizontalAlignment = CCTextAlignment.Center;
			AddChild (coinsLabel);

			menuNextLevel = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("level_next.png")));
			menuNextLevel.AnchorPoint = CCPoint.AnchorMiddle;
			var menuItemNextLevel = new CCMenuItemImage (menuNextLevel, menuNextLevel, NextLevel);

			menuRetryLevel = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("level_retry.png")));
			menuRetryLevel.AnchorPoint = CCPoint.AnchorMiddle;
			var menuItemRetryLevel = new CCMenuItemImage (menuRetryLevel, menuRetryLevel, RetryLevel);

			menuQuitLevel = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("level_quit.png")));
			menuQuitLevel.AnchorPoint = CCPoint.AnchorMiddle;
			var menuItemQuit = new CCMenuItemImage (menuQuitLevel, menuQuitLevel, QuitLevel);

			var menu = new CCMenu ((levelWasCompleted) ? menuItemNextLevel : menuItemRetryLevel, menuItemQuit) {
				Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MinY + (menuQuitLevel.BoundingBox.Size.Height * 2.5f)),
				AnchorPoint = CCPoint.AnchorMiddle
			};
			menu.AlignItemsVertically (menuQuitLevel.BoundingBox.Size.Height * 0.75f);

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
			var transitionToGame = new CCTransitionFade (3.0f, nextLevel);
			Director.ReplaceScene (transitionToGame);
		}

		void RetryLevel (object stuff = null)
		{
			var level = LevelLayer.CreateScene (Window, levels, activePlayer);
			var transitionToGame = new CCTransitionFade (3.0f, level);
			Director.ReplaceScene (transitionToGame);
		}

		void QuitLevel (object stuff = null)
		{
			var mainMenu = GameStartLayer.CreateScene (Window);
			var transitionToMainMenu = new CCTransitionFade (3.0f, mainMenu);
			Director.ReplaceScene (transitionToMainMenu);
		}
	}
}
