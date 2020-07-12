//---------------------------------------------------------------------------------------------
// <copyright file="GameBubble.cs" company="RetroTek Software Ltd">
//     Copyright (C) 2016 RetroTek Software Ltd. All rights reserved.
// </copyright>
// <author>Jared Mathes</author>
//---------------------------------------------------------------------------------------------

using System;
using CocosSharp;

namespace BubbleBreak
{
	public abstract class GameBubble: Bubble
	{
		protected const float MIN_TIME_DELAY = 0.1f;		// minimum amount of time before bubble is born
		protected const float MAX_TIME_DELAY = 2.0f;		// maximum amount of time before bubble is born


		public int TapsToPop { get; set; } 		// how many taps the bubble takes to pop
		public int TapCount {get; set;} 		// how many times the bubble has been tapped

		public CCSprite PopSprite{ get; set; }

		protected GameBubble (int xIndex, int yIndex, int listIndex) :
		base (xIndex, yIndex, listIndex)
		{
			XIndex = xIndex;
			YIndex = yIndex;
			ListIndex = listIndex;
		}

		//---------------------------------------------------------------------------------------------------------
		// CheckPopped
		//---------------------------------------------------------------------------------------------------------
		// Check to see if the tap count is greater than the number of taps needed to have the bubble popped
		//---------------------------------------------------------------------------------------------------------
		public bool CheckPopped()
		{
			return TapCount >= TapsToPop;
		}
	}
}

