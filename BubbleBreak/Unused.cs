using System;
using CocosSharp;

namespace BubbleBreak
{
	public class Unused
	{
		// A place to put useful code that isn't currently needed
		public Unused ()
		{
		}

		//---------------------------------------------------------------------------------------------------------
		// GetRandomPosition - Old method - don't use
		//---------------------------------------------------------------------------------------------------------
		// Parameters: CCsprite - size of the sprite to position
		//
		// Returns: CCPoint - random point within the display field
		//---------------------------------------------------------------------------------------------------------
//		CCPoint GetRandomPosition (CCSize spriteSize)
//		{
//			double rndX = CCRandom.NextDouble ();
//			double rndY = CCRandom.NextDouble ();
//			double randomX = (rndX > 0) 
//				? rndX * VisibleBoundsWorldspace.Size.Width - spriteSize.Width / 2
//				: spriteSize.Width / 2;
//			if (randomX < (spriteSize.Width / 2))
//				randomX += spriteSize.Width;
//
//			double randomY = (rndY > 0) 
//				? rndY * VisibleBoundsWorldspace.Size.Height - spriteSize.Height / 2
//				: spriteSize.Height / 2;
//			if (randomY < (spriteSize.Height / 2))
//				randomY += spriteSize.Height;
//			return new CCPoint ((float)randomX, (float)randomY);
//		}

		//---------------------------------------------------------------------------------------------------------
		// GetSeparatingVector - Not being used
		//---------------------------------------------------------------------------------------------------------
		// Parameters:	first - CCRect of the first object being compared
		//				second - CCRect of the second object being compared
		//
		// Returns:		CCVector2 - vector to direction to move second object so it won't collide with first
		//---------------------------------------------------------------------------------------------------------
		// Used to compare overlaping rectangles of two objects to determine if they collide or not.  If they do
		// return a vector to move the object so it won't overlap
		//---------------------------------------------------------------------------------------------------------
		CCVector2 GetSeparatingVector(CCRect first, CCRect second)
		{
			CCVector2 separation = CCVector2.Zero;

			if (first.IntersectsRect (second))
			{
				var intersecionRect = first.Intersection (second);
				bool separateHorizontally = intersecionRect.Size.Width < intersecionRect.Size.Height;

				if (separateHorizontally) {
					separation.X = intersecionRect.Size.Width;
					if (first.Center.X < second.Center.X) {
						separation.X *= -1;
					}
					separation.Y = 0;
				} else {
					separation.X = 0;
					separation.Y = intersecionRect.Size.Height;
					if (first.Center.Y < second.Center.Y) {
						separation.Y *= -1;
					}
				}

			}
			return separation;
		}
	}
}

