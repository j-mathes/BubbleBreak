//---------------------------------------------------------------------------------------------
// <copyright file="GameAppDelegate.cs" company="RetroTek Software Ltd">
//     Copyright (C) 2016 RetroTek Software Ltd. All rights reserved.
// </copyright>
// <author>Jared Mathes</author>
//---------------------------------------------------------------------------------------------

using System;
using CocosSharp;

namespace BubbleBreak
{
	public class GameAppDelegate : CCApplicationDelegate
	{
		static CCWindow sharedWindow;

		public static CCWindow SharedWindow
		{
			get { return sharedWindow; }
		}

		public override void ApplicationDidFinishLaunching(CCApplication application, CCWindow mainWindow)
		{
			application.PreferMultiSampling = false;
			application.ContentRootDirectory = "Content";
			application.ContentSearchPaths.Add("animations");
			application.ContentSearchPaths.Add("fonts");
			application.ContentSearchPaths.Add("sounds");

			sharedWindow = mainWindow;

			CCSize windowSize = mainWindow.WindowSizeInPixels;

			float desiredWidth = 1080.0f;	// 960
			float desiredHeight = 1920.0f;	// 1704
            
			// This will set the world bounds to be (0,0, w, h)
			// CCSceneResolutionPolicy.ShowAll will ensure that the aspect ratio is preserved
			CCScene.SetDefaultDesignResolution(desiredWidth, desiredHeight, CCSceneResolutionPolicy.ShowAll);
            
			// Determine whether to use the high or low def versions of our images
			// Make sure the default texel to content size ratio is set correctly
			// Of course you're free to have a finer set of image resolutions e.g (ld, hd, super-hd)
			if (desiredWidth < windowSize.Width) 
			{
				application.ContentSearchPaths.Add("images/hd");
				CCSprite.DefaultTexelToContentSizeRatio = 2.0f;
			} 
			else 
			{
				application.ContentSearchPaths.Add("images/ld");
				CCSprite.DefaultTexelToContentSizeRatio = 1.0f;
			}

			//var scene = GameStartLayer.CreateScene(mainWindow);
			var scene = MenuLayer.CreateScene(mainWindow);

			mainWindow.RunWithScene(scene);
		}

		public override void ApplicationDidEnterBackground(CCApplication application)
		{
			application.Paused = true;
		}

		public override void ApplicationWillEnterForeground(CCApplication application)
		{
			application.Paused = false;
		}
	}
}
