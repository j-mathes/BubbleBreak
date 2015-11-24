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
		CCSprite frameSprite, frameTitle, menuNextLevel, menuQuitLevel, menuRetryLevel;
		CCLabel scoreText, coinsText, scoreNeededText;

		string scoreMessage = string.Empty;
		string coinsMessage = string.Empty;
		string scoreNeededMessage = string.Empty;

		int playerScore;
		bool levelWasCompleted;

		Player activePlayer;

		public LevelFinishedLayer (int score, bool levelPassed, Player currentPlayer)
		{
			activePlayer = currentPlayer;
			playerScore = score;
			levelWasCompleted = levelPassed;
			Color = new CCColor3B(0,0,0);
			Opacity = 255;
		}

		protected override void AddedToScene ()
		{
			base.AddedToScene ();

			// Use the bounds to layout the positioning of our drawable assets
			CCRect bounds = VisibleBoundsWorldspace;

			// Add frame to layer
			frameSprite = new CCSprite ("frame.png");
			frameSprite.AnchorPoint = CCPoint.AnchorMiddle;
			frameSprite.Position = new CCPoint (bounds.Size.Width / 2, bounds.Size.Height / 2);
			AddChild (frameSprite);

			// Add frame title;
			frameTitle = new CCSprite ((levelWasCompleted) ? "level_com.png" : "level_inc.png");
			frameTitle.AnchorPoint = CCPoint.AnchorMiddle;
			frameTitle.Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MaxY - (frameTitle.BoundingBox.Size.Height * 1.5f));

			scoreMessage = string.Format ("Game Over\nYour score is {0}", playerScore);

			//var textColor = new CCColor3B(153,255,255);

			menuNextLevel = new CCSprite ("level_next");
			menuNextLevel.AnchorPoint = CCPoint.AnchorMiddle;
			var menuItemNextLevel = new CCMenuItemImage (menuNextLevel, menuNextLevel, NextLevel);

			menuRetryLevel = new CCSprite ("level_retry");
			menuRetryLevel.AnchorPoint = CCPoint.AnchorMiddle;
			var menuItemRetryLevel = new CCMenuItemImage (menuRetryLevel, menuRetryLevel, RetryLevel);

			menuQuitLevel = new CCSprite ("level_quit");
			menuQuitLevel.AnchorPoint = CCPoint.AnchorMiddle;
			var menuItemQuit = new CCMenuItemImage (menuQuitLevel, menuQuitLevel, QuitLevel);

			var menu = new CCMenu ((levelWasCompleted) ? menuItemNextLevel : menuItemRetryLevel, menuItemQuit) {
				Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MinY + (menuQuitLevel.BoundingBox.Size.Height * 4)),
				AnchorPoint = CCPoint.AnchorMiddle
			};
			menu.AlignItemsVertically (menuQuitLevel.BoundingBox.Size.Height);

			AddChild (menu);
		}
			

		public static CCScene CreateScene (CCWindow mainWindow, int score, Player currentPlayer, bool levelPassed)
		{
			var scene = new CCScene (mainWindow);
			var layer = new LevelFinishedLayer (score, levelPassed, currentPlayer);

			scene.AddChild (layer);

			return scene;
		} 

		void NextLevel (object stuff = null)
		{
			//TODO: CreateScene wants a level object passed for the second parameter, not an int index
			var nextLevel = LevelLayer.CreateScene (Window, activePlayer.LastLevelCompleted, activePlayer);
			var transitionToGame = new CCTransitionFade (3.0f, nextLevel);
			Director.ReplaceScene (transitionToGame);
		}

		void RetryLevel (object stuff = null)
		{
			var level = LevelLayer.CreateScene (Window, activePlayer.LastLevelCompleted, activePlayer);
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
