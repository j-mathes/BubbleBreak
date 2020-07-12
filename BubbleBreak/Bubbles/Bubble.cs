//---------------------------------------------------------------------------------------------
// <copyright file="Bubble.cs" company="RetroTek Software Ltd">
//     Copyright (C) 2016 RetroTek Software Ltd. All rights reserved.
// </copyright>
// <author>Jared Mathes</author>
//---------------------------------------------------------------------------------------------

using System;
using CocosSharp;

namespace BubbleBreak
{
	public abstract class Bubble : CCNode
	{
		protected const float MIN_TIME_APPEAR = 0.1f;		// minimum amount of time to fade in a bubble
		protected const float MAX_TIME_APPEAR = 1.5f;		// maximum amount of time to fade in a bubble
		protected const float MIN_TIME_HOLD = 4.0f;		// minimum amount of time to hold a bubble visible
		protected const float MAX_TIME_HOLD = 9.0f;		// maximum amount of time to hold a bubble visible
		protected const float MIN_TIME_FADE = 1.0f;		// minimum amount of time to fade out a bubble
		protected const float MAX_TIME_FADE = 4.0f;		// maximum amount of time to fade out a bubble

		public float TimeDelay { get; set; }	// how long before the bubble's birth (fade in) starts
		public float TimeAppear { get; set; }	// how long it takes to fully appear on the screen
		public float TimeHold { get; set; }  	// how long it will remain on the screen
		public float TimeFade { get; set; }  	// how long until it disappears from view

		public CCSpriteSheet BubbleSpriteSheet;

		public CCSprite BubbleSprite{ get; set; }


		public int XIndex { get; set; }
		public int YIndex { get; set; }
		public int ListIndex { get; set; }

		protected Bubble (int xIndex, int yIndex, int listIndex)
		{
			XIndex = xIndex;
			YIndex = yIndex;
			ListIndex = listIndex;

			BubbleSpriteSheet = new CCSpriteSheet ("bubbles.plist");
		}
	}
}

