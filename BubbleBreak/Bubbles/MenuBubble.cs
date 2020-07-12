//---------------------------------------------------------------------------------------------
// <copyright file="MenuBubble.cs" company="RetroTek Software Ltd">
//     Copyright (C) 2016 RetroTek Software Ltd. All rights reserved.
// </copyright>
// <author>Jared Mathes</author>
//---------------------------------------------------------------------------------------------

using System;
using CocosSharp;

namespace BubbleBreak
{
	public class MenuBubble : Bubble
    {
		public MenuBubble(int xIndex, int yIndex, int listIndex) :
		base (xIndex, yIndex, listIndex)
		{
			XIndex = xIndex;
			YIndex = yIndex;
			ListIndex = listIndex;

			TimeAppear = CCRandom.GetRandomFloat (MIN_TIME_APPEAR, MAX_TIME_APPEAR);
			TimeHold = CCRandom.GetRandomFloat (MIN_TIME_HOLD, MAX_TIME_HOLD);
			TimeFade = CCRandom.GetRandomFloat (MIN_TIME_FADE, MAX_TIME_FADE);

			BubbleSpriteSheet = new CCSpriteSheet ("bubbles.plist");

			var spriteFileName = GetRandomBubbleColor ();

			BubbleSprite = new CCSprite (BubbleSpriteSheet.Frames.Find (x => x.TextureFilename.Equals (spriteFileName)));
			BubbleSprite.AnchorPoint = CCPoint.AnchorMiddle;
			BubbleSprite.Opacity = 0;
			AddChild (BubbleSprite);
		}

		//---------------------------------------------------------------------------------------------------------
		// GetRandomBubbleColor
		//---------------------------------------------------------------------------------------------------------
		// Gets a random bubble sprite color

		static string GetRandomBubbleColor()
		{
			var randomColorSprite = (BubbleColors)CCRandom.GetRandomInt ((int)BubbleColors.blue, (int)BubbleColors.yellow);
			return "bubble-std-" + randomColorSprite + ".png";
		}
    }
}
