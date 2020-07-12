//---------------------------------------------------------------------------------------------
// <copyright file="PointBubble.cs" company="RetroTek Software Ltd">
//     Copyright (C) 2016 RetroTek Software Ltd. All rights reserved.
// </copyright>
// <author>Jared Mathes</author>
//---------------------------------------------------------------------------------------------

using System;
using CocosSharp;

namespace BubbleBreak
{
	public class PointBubble : GameBubble
    {
		public int PointValue { get; set; }
		public CCLabel PointLabel { get; set; }
		public bool IsHighlighted { get; set; }
		public CCParticleFlower Emitter { get; set; }

		public PointBubble(int xIndex, int yIndex, int listIndex, int pointValue, int tapsNeeded) :
		base (xIndex, yIndex, listIndex)
		{
			TapCount = 0;
			TapsToPop = tapsNeeded;
			PointValue = pointValue;
			XIndex = xIndex;
			YIndex = yIndex;
			ListIndex = listIndex;

			TimeDelay = CCRandom.GetRandomFloat (MIN_TIME_DELAY, MAX_TIME_DELAY);
			TimeAppear = CCRandom.GetRandomFloat (MIN_TIME_APPEAR, MAX_TIME_APPEAR);
			TimeHold = CCRandom.GetRandomFloat (MIN_TIME_HOLD, MAX_TIME_HOLD);
			TimeFade = CCRandom.GetRandomFloat (MIN_TIME_FADE, MAX_TIME_FADE);

			BubbleSpriteSheet = new CCSpriteSheet ("bubbles.plist");

			BubbleSprite = new CCSprite(BubbleSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("bubble-std-iceblue.png")));
			BubbleSprite.AnchorPoint = CCPoint.AnchorMiddle;
			BubbleSprite.Opacity = 0;
			AddChild (BubbleSprite);

			PointLabel = new CCLabel (pointValue.ToString (), "gothic-44-hd.fnt");
			PointLabel.AnchorPoint = CCPoint.AnchorMiddle;
			PointLabel.Opacity = 0;
			AddChild (PointLabel);

			PopSprite = new CCSprite(BubbleSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("bubble-pop-iceblue.png")));
			PopSprite.AnchorPoint = CCPoint.AnchorMiddle;
			PopSprite.Scale = 0.0f;
			AddChild (PopSprite);

			Emitter = new CCParticleFlower (Position) {
				Texture = CCTextureCache.SharedTextureCache.AddImage ("stars"),
				TotalParticles = 0, // start with total particles at zero then have it go to a higher number once the bubble sprite is visible
			};
		}

		public void Pop()
		{
			PointLabel.RemoveFromParent ();
			Emitter.RemoveFromParent ();
		}
    }
}
