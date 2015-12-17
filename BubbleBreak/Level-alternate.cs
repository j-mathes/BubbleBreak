using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using Foundation;
using XmlLib;

namespace BubbleBreak
{
	public class LevelAlternate
	{
		XElement self;  // I dont understand how this way would work. Use the level(copy) original way

		// maybe 'self' holds the element value that I need to read, in this case it's probably 'record'
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

		public IList<bool> Flags { 
			get
			{
				try
				{
					return self.Element("Flags") // is flags what we are now looking for in the XML file to read the sequences?  I think so
						.Elements()
						.Select(x => (bool)x)
						.ToList();
				}
				catch
				{
					return new bool[] { }.ToList();
				}
			}
			set
			{
				XElement flags = self.GetElement("Flags");
				flags.RemoveAll();
				flags.Add(value.Select(b => new XElement("flag", b)));
			} }

		public LevelAlternate (XElement e)
		{
			self = e;
		}
	}
}

