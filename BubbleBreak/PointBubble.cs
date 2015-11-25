using System;
using System.Collections.Generic;
using System.Text;
using CocosSharp;
using System.Linq;

namespace BubbleBreak
{
	public class PointBubble : CCNode
    {
		const float MIN_TIME_DELAY = 0.1f;		// minimum amount of time before bubble is born
		const float MAX_TIME_DELAY = 2.0f;		// maximum amount of time before bubble is born
		const float MIN_TIME_APPEAR = 0.1f;		// minimum amount of time to fade in a bubble
		const float MAX_TIME_APPEAR = 1.5f;		// maximum amount of time to fade in a bubble
		const float MIN_TIME_HOLD = 4.0f;		// minimum amount of time to hold a bubble visible
		const float MAX_TIME_HOLD = 9.0f;		// maximum amount of time to hold a bubble visible
		const float MIN_TIME_FADE = 1.0f;		// minimum amount of time to fade out a bubble
		const float MAX_TIME_FADE = 4.0f;		// maximum amount of time to fade out a bubble

		public float TimeDelay { get; set; }	// how long before the bubble's birth (fade in) starts
		public float TimeAppear { get; set; }	// how long it takes to fully appear on the screen
		public float TimeHold { get; set; }  	// how long it will remain on the screen
		public float TimeFade { get; set; }  	// how long until it disappears from view

		public int TapsToPop { get; set; } 		// how many taps the bubble takes to pop
		public int TapCount {get; set;} 		// how many times the bubble has been tapped
        public int PointValue { get; set; }

		CCSpriteSheet bubbleSpriteSheet;

		public CCLabel PointLabel;
		public CCSprite BubbleSprite{ get; set; }
		public CCSprite PopSprite{ get; set; }
		public CCLabel PosLabel{ get; set; }
		public string LableText { get; set; }

		public int XIndex { get; set; }
		public int YIndex { get; set; }
		public int ListIndex { get; set; }

		public PointBubble(int pointValue, int tapsNeeded, int xIndex, int yIndex, int listIndex) : base()
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

			bubbleSpriteSheet = new CCSpriteSheet ("bubbles.plist");

			BubbleSprite = new CCSprite(bubbleSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("bubble-std-iceblue.png")));
			BubbleSprite.AnchorPoint = CCPoint.AnchorMiddle;
			BubbleSprite.Opacity = 0;
			this.AddChild(BubbleSprite);

			PointLabel = new CCLabel (pointValue.ToString (), "arial", 44);
			PointLabel.AnchorPoint = CCPoint.AnchorMiddle;
			PointLabel.Opacity = 0;
			this.AddChild (PointLabel);

			PopSprite = new CCSprite(bubbleSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("bubble-pop-iceblue.png")));
			PopSprite.AnchorPoint = CCPoint.AnchorMiddle;
			PopSprite.Scale = 0.0f;
			this.AddChild(PopSprite);
		}

		//---------------------------------------------------------------------------------------------------------
		// CheckPopped
		//---------------------------------------------------------------------------------------------------------
		// Check to see if the tap count is greater than the number of taps needed to have the bubble popped
		//---------------------------------------------------------------------------------------------------------
		public bool CheckPopped()
		{
			if (TapCount >= TapsToPop) {
				return true;
			} else
				return false;
		}
    }
}
