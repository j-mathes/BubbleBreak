// -----------------------------------------------------------------------------------------------
//  <copyright file="FreePlayLayer.cs" company="RetroTek Software Ltd">
//      Copyright (C) 2016 RetroTek Software Ltd. All rights reserved.
//  </copyright>
//  <author>Jared Mathes</author>
// -----------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using CocosSharp;

namespace BubbleBreak
{
	public class FreePlayLayer : CCLayerColor
	{
		const string GOTHIC_30_HD_FNT = "gothic-30-hd.fnt";
		const string GOTHIC_44_HD_FNT = "gothic-44-hd.fnt";
		const string GOTHIC_56_WHITE_HD_FNT = "gothic-56-white-hd.fnt";
		const string GOTHIC_56_WHITE_FNT = "gothic-56-white.fnt";

		Player currentPlayer;

		CCLabel backLabel;

		//---------------------------------------------------------------------------------------------------------
		// FreePlayLayer constructor
		//---------------------------------------------------------------------------------------------------------
		public FreePlayLayer (Player activePlayer)
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

			SetupUI (bounds);
		}

		//---------------------------------------------------------------------------------------------------------
		// CreateScene
		//---------------------------------------------------------------------------------------------------------
		public static CCScene CreateScene (CCWindow mainWindow, Player activePlayer)
		{

			var scene = new CCScene (mainWindow);
			var layer = new FreePlayLayer (activePlayer);

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
			var titleLabel = new CCLabel ("Free Play Placeholder", GOTHIC_44_HD_FNT) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 1.5f,
				Position = bounds.Center
			};
			AddChild (titleLabel);

			backLabel = new CCLabel ("Back", GOTHIC_44_HD_FNT) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 1.5f,
			};

			var backItem = new CCMenuItemLabel (backLabel, BackToGameSelect);

			var backMenu = new CCMenu (backItem);
			backMenu.AnchorPoint = CCPoint.AnchorMiddleBottom;
			backMenu.Position = new CCPoint (bounds.Size.Width / 2, 220f);

			AddChild (backMenu);
		}

		//---------------------------------------------------------------------------------------------------------
		// BackToGameSelect
		//---------------------------------------------------------------------------------------------------------
		// Returns to Game Select
		//---------------------------------------------------------------------------------------------------------
		void BackToGameSelect (object stuff = null)
		{
			var layer = GameSelectLayer.CreateScene (Window, currentPlayer);
			var transitionToLayer = new CCTransitionSlideInL(0.2f, layer);
			Director.ReplaceScene (transitionToLayer);
		}
	}
}