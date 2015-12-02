using System;
using System.Collections.Generic;
using CocosSharp;

namespace BubbleBreak
{
	public class LevelIntroLayer : CCLayerColor
	{
		CCSpriteSheet uiSpriteSheet;
		CCSprite frameSprite;

		CCLabel titleLabel, messageLabel;

		Player activePlayer;
		List<Level> levels;
		int levelNumber;
		Level nextLevel;

		string nextLevelText, messageText;

		public LevelIntroLayer (List<Level> gameLevels, Player player) : base (CCColor4B.Black)
		{
			uiSpriteSheet = new CCSpriteSheet ("ui.plist");
			levels = gameLevels;
			activePlayer = player;
		}

		protected override void AddedToScene ()
		{
			base.AddedToScene ();

			// Use the bounds to layout the positioning of our drawable assets
			CCRect bounds = VisibleBoundsWorldspace;

			levelNumber = activePlayer.LastLevelCompleted + 1;
			nextLevel = levels[levelNumber];

			nextLevelText = "Level " + levelNumber;
			messageText = string.Format ("{0}\nTime to finish: {1}\nPoints to pass: {2}", nextLevel.LevelDescription, nextLevel.MaxLevelTime, nextLevel.LevelPassScore);

			// Add frame to layer
			frameSprite = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("frame.png")));
			frameSprite.AnchorPoint = CCPoint.AnchorMiddle;
			frameSprite.Position = new CCPoint (bounds.Size.Width / 2, bounds.Size.Height / 2);
			AddChild (frameSprite);

			// Add title lable
			titleLabel = new CCLabel(nextLevelText, "arial", 30);
			titleLabel.Scale = 3f;
			titleLabel.AnchorPoint = CCPoint.AnchorMiddle;
			titleLabel.Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MaxY - (titleLabel.BoundingBox.Size.Height * 3f));
			AddChild (titleLabel);

			// Add message label
			messageLabel = new CCLabel (messageText, "arial", 30);
			messageLabel.Scale = 1f;
			messageLabel.Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MinY + (messageLabel.BoundingBox.Size.Height / 3 * 8.5f));
			messageLabel.AnchorPoint = CCPoint.AnchorMiddle;
			messageLabel.HorizontalAlignment = CCTextAlignment.Center;
			AddChild (messageLabel);

			// wait to transition to level
			this.RunAction (new CCDelayTime (5f));

			// transition to level
			StartLevel ();
		}

		public static CCScene CreateScene (CCWindow mainWindow, List<Level> gameLevels, Player player)
		{
			var scene = new CCScene (mainWindow);
			var layer = new LevelIntroLayer (gameLevels, player);

			scene.AddChild (layer);

			return scene;
		} 

		void StartLevel ()
		{
			var levelScene = LevelLayer.CreateScene (Window, levels, activePlayer);
			var transitionToLevel = new CCTransitionFade (3.0f, levelScene);
			Director.ReplaceScene (transitionToLevel);
		}
	}
}

