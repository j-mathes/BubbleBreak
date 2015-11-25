using System;
using System.Collections.Generic;
using System.Text;
using CocosSharp;
using System.Linq;

namespace BubbleBreak
{
	public class MenuBubble : CCNode
    {
		const float MIN_TIME_APPEAR = 0.1f;
		const float MAX_TIME_APPEAR = 1.5f;
		const float MIN_TIME_HOLD = 4.0f;
		const float MAX_TIME_HOLD = 9.0f;
		const float MIN_TIME_FADE = 1.0f;
		const float MAX_TIME_FADE = 4.0f;

		public float TimeAppear { get; set; }// how long it takes to fully appear on the screen
		public float TimeHold { get; set; }  // how long it will remain on the screen
		public float TimeFade { get; set; }  // how long until it disappears from view

		CCSpriteSheet bubbleSpriteSheet;
		public CCSprite BubbleSprite{ get; set; }

		public int XIndex { get; set; }
		public int YIndex { get; set; }
		public int ListIndex { get; set; }

		public MenuBubble(int xIndex, int yIndex, int listIndex) : base()
		{
			XIndex = xIndex;
			YIndex = yIndex;
			ListIndex = listIndex;

			TimeAppear = CCRandom.GetRandomFloat (MIN_TIME_APPEAR, MAX_TIME_APPEAR);
			TimeHold = CCRandom.GetRandomFloat (MIN_TIME_HOLD, MAX_TIME_HOLD);
			TimeFade = CCRandom.GetRandomFloat (MIN_TIME_FADE, MAX_TIME_FADE);

			bubbleSpriteSheet = new CCSpriteSheet ("bubbles.plist");

			var spriteFileName = GetRandomBubbleColor ();

			BubbleSprite = new CCSprite (bubbleSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals (spriteFileName)));
			BubbleSprite.AnchorPoint = CCPoint.AnchorMiddle;
			BubbleSprite.Opacity = 0;
			this.AddChild(BubbleSprite);
		}

		//---------------------------------------------------------------------------------------------------------
		// GetRandomBubbleColor
		//---------------------------------------------------------------------------------------------------------
		// Gets a random bubble sprite color

		string GetRandomBubbleColor()
		{
			BubbleColors randomColorSprite = (BubbleColors)CCRandom.GetRandomInt ((int)BubbleColors.blue, (int)BubbleColors.yellow);
			return "bubble-std-" + randomColorSprite.ToString () + ".png";
		}
    }
}
