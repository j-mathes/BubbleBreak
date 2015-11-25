using System;
using System.IO;
using System.Collections.Generic;
using CocosSharp;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using Foundation;

namespace BubbleBreak
{
	public class GameStartLayer : CCLayerColor
	{
		const float BUBBLE_BUFFER = 1.0f;
		const float BUBBLE_SCALE = 1.15f;
		const int MAX_VISIBLE_BUBBLES = 40;
		const int MAX_SCREEN_VISIBLE_BUBBLES = 30;
		const int MAX_BUBBLES_X = 5;
		const int MAX_BUBBLES_Y = 8;
		const float SCREEN_X_MARGIN = 40.0f;
		const float SCREEN_Y_MARGIN = 120f;
		const float CELL_DIMS_HALF = 100f;
		const float CELL_CENTER_ZONE_HALF = 28f;

		CCNode bubbles;

		bool[] bubbleOccupiedArray = new bool[MAX_VISIBLE_BUBBLES];

		CCSprite rtLogo, title, menuStart;
		CCRepeatForever repeatedAction;

		int timeIncrement = 0;
		int visibleBubbles = 1;
		int bubblesVisibleIncrementDelay = 10; // delay in 10ths of a second to increase the number of bubbles on the screen

		List<Level> levels;
		XDocument levelInfo = new XDocument ();

		Player currentPlayer;

		//---------------------------------------------------------------------------------------------------------
		// GameStartLayer Constructor
		//---------------------------------------------------------------------------------------------------------
		//
		public GameStartLayer () : base (new CCColor4B(0,0,0))
		{
			Color = new CCColor3B(0,0,0);
			Opacity = 255;

			// Define Actions
			var moveUp = new CCMoveBy (1.0f, new CCPoint(0.0f, 50.0f));
			var moveDown = moveUp.Reverse ();

			//Define Sequence of actions
			var moveSeq = new CCSequence (new CCEaseBackInOut (moveUp), new CCEaseBackInOut (moveDown));

			repeatedAction = new CCRepeatForever (moveSeq);

			StartScheduling ();
		}

		//---------------------------------------------------------------------------------------------------------
		// AddedToScene
		//---------------------------------------------------------------------------------------------------------
		// 
		protected override void AddedToScene ()
		{
			base.AddedToScene ();

			// Use the bounds to layout the positioning of our drawable assets
			CCRect bounds = VisibleBoundsWorldspace;

			bubbles = new CCNode ();
			AddChild (bubbles);

			//initialize every bool in BubbleArray to false.  True if there is a bubble there.
			for (int i = 0; i < MAX_BUBBLES_X; i++) {
				bubbleOccupiedArray [i] = false;
			}

			levels = ReadLevels (levelInfo);

			currentPlayer = new Player();

			if (File.Exists (currentPlayer.PlayerDataFile)) {
				currentPlayer.ReadData ();
			} else {
				currentPlayer.WriteData ();
			}


			//---------------------------------------------------------------------------------------------------------
			//Menu Elements
			//---------------------------------------------------------------------------------------------------------

			menuStart = new CCSprite ("bb_startgame");
			menuStart.AnchorPoint = CCPoint.AnchorMiddle;
			
			var menuItemStart = new CCMenuItemImage (menuStart, menuStart, StartGame);
			var menu = new CCMenu (menuItemStart) {
				Position = new CCPoint (bounds.Size.Width / 2, bounds.Size.Height / 2),
				AnchorPoint = CCPoint.AnchorMiddle
			};
			
			menu.AlignItemsVertically (50);
			
			AddChild (menu);

			rtLogo = new CCSprite ("bb_retrotek");
			rtLogo.AnchorPoint = CCPoint.AnchorMiddle;
			rtLogo.Position = new CCPoint (bounds.Size.Width / 2, bounds.Size.Height / 14);
			//rtLogo.RunAction (repeatedAction);
			AddChild (rtLogo);

			title = new CCSprite ("bb_title");
			title.AnchorPoint = CCPoint.AnchorMiddle;
			title.Position = new CCPoint (bounds.Size.Width / 2, (bounds.Size.Height / 4)*3);
			title.RunAction (repeatedAction);
			AddChild (title);

			Schedule (_ => CheckForFadedBubbles());
		}

		//---------------------------------------------------------------------------------------------------------
		// StartGame - Used in menu selection
		//---------------------------------------------------------------------------------------------------------
		// Transitions to GameLayer
		void StartGame (object stuff = null)
		{
			var mainGame = LevelLayer.CreateScene (Window, levels, currentPlayer);
			var transitionToGame = new CCTransitionFade(3.0f, mainGame);
			Director.ReplaceScene (transitionToGame);
		}

		//---------------------------------------------------------------------------------------------------------
		// CreateScene
		//---------------------------------------------------------------------------------------------------------
		// 
		public static CCScene CreateScene (CCWindow mainWindow)
		{
			var scene = new CCScene (mainWindow);
			var layer = new GameStartLayer ();

			scene.AddChild (layer);

			return scene;
		}

		//---------------------------------------------------------------------------------------------------------
		// GetRandomPosition
		//---------------------------------------------------------------------------------------------------------
		// Get a position based on the index and sprite size

		CCPoint GetRandomPosition (CCSize spriteSize, int xIndex, int yIndex)
		{
			double rndX = CCRandom.GetRandomFloat ((xIndex * CELL_DIMS_HALF * 2) + (SCREEN_X_MARGIN + CELL_DIMS_HALF - CELL_CENTER_ZONE_HALF), (xIndex * CELL_DIMS_HALF * 2) + (SCREEN_X_MARGIN + CELL_DIMS_HALF + CELL_CENTER_ZONE_HALF));
			double rndY = CCRandom.GetRandomFloat ((yIndex * CELL_DIMS_HALF * 2) + (SCREEN_Y_MARGIN + CELL_DIMS_HALF - CELL_CENTER_ZONE_HALF), (yIndex * CELL_DIMS_HALF * 2) + (SCREEN_Y_MARGIN + CELL_DIMS_HALF + CELL_CENTER_ZONE_HALF));
			return new CCPoint ((float)rndX, (float)rndY);
		}

		//---------------------------------------------------------------------------------------------------------
		// DisplayBubble
		//---------------------------------------------------------------------------------------------------------
		// Displays a bubble

		async void DisplayBubble(MenuBubble newBubble)
		{
			await newBubble.BubbleSprite.RunActionsAsync (new CCFadeIn (newBubble.TimeAppear), new CCDelayTime (newBubble.TimeHold), new CCFadeOut (newBubble.TimeFade));
			bubbleOccupiedArray [newBubble.ListIndex] = false;
			newBubble.RemoveFromParent ();
		}

		void CheckForFadedBubbles()
		{
			//check if a bubble has popped on its own
			if (bubbles.ChildrenCount < visibleBubbles) {
				ShuffleBag<int> newBubbleOrder = new ShuffleBag<int> ();

				for (int i = 0; i < bubbleOccupiedArray.Length; i++) {
					if (!bubbleOccupiedArray [i])
						newBubbleOrder.Add (i);
				}

				var newBubbleIndex = newBubbleOrder.Next ();
				var newBubble = new MenuBubble ((newBubbleIndex % MAX_BUBBLES_X), (newBubbleIndex / MAX_BUBBLES_X), newBubbleIndex);
				CCPoint p = GetRandomPosition ( newBubble.BubbleSprite.ContentSize * BUBBLE_SCALE, newBubble.XIndex, newBubble.YIndex);
				newBubble.Position = new CCPoint (p.X, p.Y);
				newBubble.Scale = BUBBLE_SCALE;
				DisplayBubble (newBubble);
				bubbles.AddChild (newBubble);
				bubbleOccupiedArray [newBubbleIndex] = true;
			}
		}

		//--------------------------------------------------
		// StartScheduling
		//--------------------------------------------------
		void StartScheduling()
		{
			Schedule (t => {
				timeIncrement++;

				if (timeIncrement > 0) {
					if (visibleBubbles < MAX_SCREEN_VISIBLE_BUBBLES){
					visibleBubbles = ((timeIncrement % bubblesVisibleIncrementDelay) == 0) ? visibleBubbles + 1 : visibleBubbles;
					}
				}

			},0.1f);
		}

		//---------------------------------------------------------------------------------------------------------
		// ReadLevels
		//---------------------------------------------------------------------------------------------------------
		// Reads the contents of the levelInfo.xml file and builds a list of level objects with the data in the 
		// xml file.  Allows for easy changing of level setup and addint new levels
		//---------------------------------------------------------------------------------------------------------
		public List<Level> ReadLevels(XDocument levelInfo)
		{
			levelInfo = XDocument.Load ("./levelinfo.xml");

			List<Level> lvl = (from level in levelInfo.Root.Descendants ("record") // "record" has to match the record level identifier in the xml file
				select new Level {
					LevelNum = int.Parse (level.Element ("LevelNum").Value),
					LevelName = level.Element ("LevelName").Value,
					MaxBubbles = int.Parse (level.Element ("MaxBubbles").Value),
					MaxVisibleBubbles = int.Parse (level.Element ("MaxVisibleBubbles").Value),
					StartingPointValue = int.Parse (level.Element ("StartingPointValue").Value),
					MaxLevelTime = int.Parse (level.Element ("MaxLevelTime").Value),
					LevelPassScore = int.Parse (level.Element ("LevelPassScore").Value),
					TapsToPopStandard = int.Parse (level.Element ("TapsToPopStandard").Value),
					InitialVisibleBubbles = int.Parse (level.Element ("InitialVisibleBubbles").Value),
					LevelDescription = level.Element ("LevelDescription").Value,
					SequenceLevel = int.Parse (level.Element ("SequenceLevel").Value)
				}).ToList ();
			return lvl;
		}
	}
}
