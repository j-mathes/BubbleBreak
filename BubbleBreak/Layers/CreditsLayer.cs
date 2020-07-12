// -----------------------------------------------------------------------------------------------
//  <copyright file="CreditsLayer.cs" company="RetroTek Software Ltd">
//      Copyright (C) 2016 RetroTek Software Ltd. All rights reserved.
//  </copyright>
//  <author>Jared Mathes</author>
// -----------------------------------------------------------------------------------------------
using System;
using CocosSharp;

namespace BubbleBreak
{
	public class CreditsLayer : CCLayerColor
	{
		const string GOTHIC_30_HD_FNT = "gothic-30-hd.fnt";
		const string GOTHIC_44_HD_FNT = "gothic-44-hd.fnt";
		const string GOTHIC_56_WHITE_HD_FNT = "gothic-56-white-hd.fnt";
		const string GOTHIC_56_WHITE_FNT = "gothic-56-white.fnt";

		string creditText;
		CCLabel creditsTitleLabel;
		CCLabel creditsLabel;
		CCLabel backLabel;

		//---------------------------------------------------------------------------------------------------------
		// CreditsLayer Constructor
		//---------------------------------------------------------------------------------------------------------
		public CreditsLayer () : base (new CCColor4B(0,0,0))
		{
			Color = new CCColor3B(0,0,0);
			Opacity = 255;

			creditText = "Programming\n" +
			"\n\n" + //this doesnt work for some reason
			"Jared Mathes\n" +
			"\n\n" +
			"Testing\n" +
			"\n\n" +
			"Juanita Mathes\n" +
			"McKinley Funk\n" +
			"Dallin Funk\n";
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
		public static CCScene CreateScene (CCWindow mainWindow)
		{
			var scene = new CCScene (mainWindow);
			var layer = new CreditsLayer ();

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
			creditsTitleLabel = new CCLabel ("Credits", GOTHIC_56_WHITE_HD_FNT) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 1.5f,
				PositionX = bounds.MidX,
				PositionY = 1860,
			};
			AddChild (creditsTitleLabel);

			creditsLabel = new CCLabel (creditText, GOTHIC_56_WHITE_HD_FNT) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 1f,
				PositionX = bounds.MidX,
				PositionY = bounds.MidY,
				HorizontalAlignment = CCTextAlignment.Center
			};
			AddChild (creditsLabel);

			backLabel = new CCLabel ("Back", GOTHIC_44_HD_FNT) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 1.5f,
			};

			var backItem = new CCMenuItemLabel (backLabel, BackToMain);

			var backMenu = new CCMenu (backItem);
			backMenu.AnchorPoint = CCPoint.AnchorMiddleBottom;
			backMenu.Position = new CCPoint (bounds.Size.Width / 2, 220f);

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
			var transitionToLayer = new CCTransitionSlideInR(0.2f, layer);
			Director.ReplaceScene (transitionToLayer);
		}
	}
}

