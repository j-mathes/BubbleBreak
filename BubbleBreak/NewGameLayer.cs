using System;
using System.Collections.Generic;
using CocosSharp;

namespace BubbleBreak
{
	public class NewGameLayer : CCLayerColor
	{
		CCSpriteSheet uiSpriteSheet;
		CCSprite frameSprite, frameTitle, menuOkSpriteStd, menuOkSpriteSel, menuCancelSpriteStd, menuCancelSpriteSel;

		CCLabel messageLabel;

		Player activePlayer;
		List<Level> levels;

		public NewGameLayer (List<Level> gameLevels, Player player) : base (CCColor4B.Black)
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

			// Add frame to layer
			frameSprite = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("frame.png")));
			frameSprite.AnchorPoint = CCPoint.AnchorMiddle;
			frameSprite.Position = new CCPoint (bounds.Size.Width / 2, bounds.Size.Height / 2);
			AddChild (frameSprite);

			// Add frame title;
			frameTitle = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("newgame_title.png")));
			frameTitle.AnchorPoint = CCPoint.AnchorMiddle;
			frameTitle.Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MaxY - (frameTitle.BoundingBox.Size.Height * 1.5f));
			AddChild (frameTitle);

			// Add message label
			messageLabel = new CCLabel ("Message", "arial", 30);
			messageLabel.Text = "Erase player data and\nstart a new game?";
			messageLabel.Scale = 2f;
			messageLabel.Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MinY + (messageLabel.BoundingBox.Size.Height * 8.5f));
			messageLabel.AnchorPoint = CCPoint.AnchorMiddle;
			messageLabel.HorizontalAlignment = CCTextAlignment.Center;
			AddChild (messageLabel);

			// Add Menu Items
			menuOkSpriteStd = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("ok_std.png")));
			menuOkSpriteStd.AnchorPoint = CCPoint.AnchorMiddle;
			menuOkSpriteSel = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("ok_sel.png")));
			menuOkSpriteSel.AnchorPoint = CCPoint.AnchorMiddle;
			var menuItemOk = new CCMenuItemImage (menuOkSpriteStd, menuOkSpriteStd, StartNewGame);

			menuCancelSpriteStd = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("cancel_std.png")));
			menuCancelSpriteStd.AnchorPoint = CCPoint.AnchorMiddle;
			menuCancelSpriteSel = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("cancel_sel.png")));
			menuCancelSpriteSel.AnchorPoint = CCPoint.AnchorMiddle;
			var menuItemCancel = new CCMenuItemImage (menuCancelSpriteStd, menuCancelSpriteStd, CancelNewGame);

			var menu = new CCMenu (menuItemOk, menuItemCancel) {
				Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MinY + (menuOkSpriteStd.BoundingBox.Size.Height * 3)),
				AnchorPoint = CCPoint.AnchorMiddle
			};
			menu.AlignItemsVertically (menuOkSpriteStd.BoundingBox.Size.Height / 1.5f);

			AddChild (menu);
		}

		public static CCScene CreateScene (CCWindow mainWindow, List<Level> gameLevels, Player player)
		{
			var scene = new CCScene (mainWindow);
			var layer = new NewGameLayer (gameLevels, player);

			scene.AddChild (layer);

			return scene;
		} 

		void StartNewGame (object stuff = null)
		{
			var level = LevelIntroLayer.CreateScene (Window, levels, activePlayer);
			var transitionToGame = new CCTransitionFade (3.0f, level);
			Director.ReplaceScene (transitionToGame);
		}

		void CancelNewGame (object stuff = null)
		{
			var mainMenu = GameStartLayer.CreateScene (Window);
			var transitionToMainMenu = new CCTransitionFade (3.0f, mainMenu);
			Director.ReplaceScene (transitionToMainMenu);
		}
	}
}
