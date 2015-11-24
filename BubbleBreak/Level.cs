using System;
using System.Xml;
using System.Collections;

namespace BubbleBreak
{
	public class Level // should this be a struct?
	{
		public int LevelNum { get; set; }
		public string LevelName { get; set; }
		public int MaxBubbles { get; set; }
		public int MaxVisibleBubbles { get; set; }
		public int StartingPointValue { get; set; }
		public int MaxLevelTime { get; set; }
		public int LevelPassScore { get; set; }
		public int TapsToPopStandard { get; set; }
		public int InitialVisibleBubbles { get; set; }
		public string LevelDescription { get; set; }
		public int SequenceLevel { get; set; }

		public Level ()
		{
			 
		}
	}
}

