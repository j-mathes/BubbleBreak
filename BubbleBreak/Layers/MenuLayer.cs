// -----------------------------------------------------------------------------------------------
//  <copyright file="MenuLayer.cs" company="RetroTek Software Ltd">
//      Copyright (C) 2016 RetroTek Software Ltd. All rights reserved.
//  </copyright>
//  <author>Jared Mathes</author>
// -----------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CocosSharp;

namespace BubbleBreak
{
	public class MenuLayer : CCLayerColor
	{
		const string GOTHIC_30_HD_FNT = "gothic-30-hd.fnt";
		const string GOTHIC_44_HD_FNT = "gothic-44-hd.fnt";
		const string GOTHIC_56_WHITE_HD_FNT = "gothic-56-white-hd.fnt";
		const string GOTHIC_56_WHITE_FNT = "gothic-56-white.fnt";

		const string LEVEL_DATA_FOLDER = "./LevelData/";

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
		const int BUBBLES_VISIBLE_INCREMENT_DELAY = 10; // delay in 10ths of a second to increase the number of bubbles on the screen

		CCNode Bubbles;

		bool[] BubbleOccupiedArray = new bool[MAX_VISIBLE_BUBBLES];

		CCSprite rtLogo, title;
		CCRepeatForever repeatedAction;
		CCSpriteSheet uiSpriteSheet;

		CCLabel startLabel, creditsLabel;

		int timeIncrement;
		int visibleBubbles = 1;

//		List<Branch> branches;
//		List<Level> levels;
//		XDocument branchInfo = new XDocument ();
//		XDocument levelInfo = new XDocument ();
	
		//---------------------------------------------------------------------------------------------------------
		// MenuLayer Constructor
		//---------------------------------------------------------------------------------------------------------
		public MenuLayer (): base (new CCColor4B(0,0,0))
		{
			Color = new CCColor3B(0,0,0);
			Opacity = 255;

			// define spritesheets
			uiSpriteSheet = new CCSpriteSheet("ui.plist");

			// Define Actions

			var inflate = new CCScaleBy (0.7f, 1.1f);
			var deflate = new CCScaleBy (4.0f, 0.9f);

			//Define Sequence of actions

			var actionSeq = new CCSequence (new CCEaseElasticIn (inflate), new CCEaseExponentialOut(deflate), new CCDelayTime (7.0f));

			repeatedAction = new CCRepeatForever (actionSeq);

			StartScheduling ();
		}

		//---------------------------------------------------------------------------------------------------------
		// AddedToScene
		//--------------------------------------------------------------------------------------------------------- 
		protected override void AddedToScene ()
		{
			base.AddedToScene ();

			// Use the bounds to layout the positioning of our drawable assets
			CCRect bounds = VisibleBoundsWorldspace;

			Bubbles = new CCNode ();
			AddChild (Bubbles);

			//initialize every bool in BubbleArray to false.  True if there is a bubble there.
			for (int i = 0; i < MAX_BUBBLES_X; i++) {
				BubbleOccupiedArray [i] = false;
			}

//			branches = ReadBranches (branchInfo);
//			levels = ReadLevels (levelInfo);

			//---------------------------------------------------------------------------------------------------------
			//Menu Elements
			//---------------------------------------------------------------------------------------------------------

			startLabel = new CCLabel ("Start", GOTHIC_44_HD_FNT);
			startLabel.AnchorPoint = CCPoint.AnchorMiddle;
			startLabel.Scale = 2.0f;
			var menuItemStart = new CCMenuItemLabel (startLabel, Start);

			creditsLabel = new CCLabel ("Credits", GOTHIC_44_HD_FNT);
			creditsLabel.AnchorPoint = CCPoint.AnchorMiddle;
			creditsLabel.Scale = 2.0f;
			var menuItemCredits = new CCMenuItemLabel (creditsLabel, Credits);

			var menu = new CCMenu (menuItemStart, menuItemCredits) {
				Position = new CCPoint (bounds.Size.Width / 2, bounds.Size.Height / 2.3f),
				AnchorPoint = CCPoint.AnchorMiddle
			};

			menu.AlignItemsVertically (250);

			AddChild (menu);

			rtLogo = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("bb_retrotek.png")));
			rtLogo.AnchorPoint = CCPoint.AnchorMiddle;
			rtLogo.Position = new CCPoint (bounds.Size.Width / 2, bounds.Size.Height / 14);
			//rtLogo.RunAction (repeatedAction);
			AddChild (rtLogo);

			title = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("bb_title.png")));
			title.AnchorPoint = CCPoint.AnchorMiddle;
			title.Position = new CCPoint (bounds.Size.Width / 2, (bounds.Size.Height / 7)*6);
			title.RunAction (repeatedAction);
			AddChild (title);

			Schedule (_ => CheckForFadedBubbles());
		}

		//---------------------------------------------------------------------------------------------------------
		// CreateScene
		//---------------------------------------------------------------------------------------------------------
		public static CCScene CreateScene (CCWindow mainWindow)
		{
			var scene = new CCScene (mainWindow);
			var layer = new MenuLayer ();

			scene.AddChild (layer);

			return scene;
		}

		//---------------------------------------------------------------------------------------------------------
		// StartScheduling
		//---------------------------------------------------------------------------------------------------------
		void StartScheduling()
		{
			Schedule (t => {
				timeIncrement++;

				if (timeIncrement > 0) {
					if (visibleBubbles < MAX_SCREEN_VISIBLE_BUBBLES){
						visibleBubbles = ((timeIncrement % BUBBLES_VISIBLE_INCREMENT_DELAY) == 0) ? visibleBubbles + 1 : visibleBubbles;
					}
				}

			},0.1f);
		}

//		//---------------------------------------------------------------------------------------------------------
//		// ReadBranches
//		//---------------------------------------------------------------------------------------------------------
//		// Reads the contents of the branches.xml file and builds a list of branch objects
//		//---------------------------------------------------------------------------------------------------------
//		public List<Branch> ReadBranches(XDocument branchInfo)
//		{
//			branchInfo = XDocument.Load (LEVEL_DATA_FOLDER + "branches.xml");
//			List<Branch> branchList = (from branch in branchInfo.Root.Descendants ("record")
//				select new Branch {
//					BranchNum = int.Parse (branch.Element ("BranchNum").Value),
//					BranchName = branch.Element ("BranchName").Value,
//					NumberOfLevels = int.Parse (branch.Element ("NumberOfLevels").Value),
//					UnlockNextBranch = int.Parse (branch.Element ("UnlockNextBranch").Value),
//					UnlockFreePlay = int.Parse (branch.Element ("UnlockFreePlay").Value),
//				}).ToList ();
//			return branchList;
//		}
//
//		//---------------------------------------------------------------------------------------------------------
//		// ReadLevels
//		//---------------------------------------------------------------------------------------------------------
//		// Using the list of branches, reads each individual level xml file and builds a list of level objects 
//		//---------------------------------------------------------------------------------------------------------
//		public List<Level> ReadLevels(XDocument levelData)
//		{
//			levelData = XDocument.Load (LEVEL_DATA_FOLDER + "levels.xml");
//
//			List<Level> lvl = (from level in levelData.Root.Descendants ("record")
//				select new Level {
//					BranchNum = int.Parse (level.Element ("BranchNum").Value), 
//					LevelNum = int.Parse (level.Element ("LevelNum").Value),
//					LevelName = level.Element ("LevelName").Value,
//					MaxBubbles = int.Parse (level.Element ("MaxBubbles").Value),
//					MaxVisibleBubbles = int.Parse (level.Element ("MaxVisibleBubbles").Value),
//					StartingPointValue = int.Parse (level.Element ("StartingPointValue").Value),
//					MaxLevelTime = int.Parse (level.Element ("MaxLevelTime").Value),
//					LevelPassScore = int.Parse (level.Element ("LevelPassScore").Value),
//					TapsToPopStandard = int.Parse (level.Element ("TapsToPopStandard").Value),
//					InitialVisibleBubbles = int.Parse (level.Element ("InitialVisibleBubbles").Value),
//					ChanceToRollNextSeq = int.Parse (level.Element ("ChanceToRollNextSeq").Value),
//					ChanceToRollBonus = int.Parse (level.Element ("ChanceToRollBonus").Value),
//					LevelDescription = level.Element ("LevelDescription").Value,
//					SeqLinear = bool.Parse (level.Element ("SeqLinear").Value),
//					SeqEven = bool.Parse ( level.Element ("SeqEven").Value),
//					SeqOdd = bool.Parse (level.Element ("SeqOdd").Value),
//					SeqTriangular = bool.Parse (level.Element ("SeqTriangular").Value),
//					SeqSquare = bool.Parse (level.Element ("SeqSquare").Value),
//					SeqLazy = bool.Parse (level.Element ("SeqLazy").Value),
//					SeqFibonacci = bool.Parse (level.Element ("SeqFibonacci").Value),
//					SeqPrime = bool.Parse (level.Element ("SeqPrime").Value),
//					SeqDouble = bool.Parse (level.Element ("SeqDouble").Value),
//					SeqTriple = bool.Parse (level.Element ("SeqTriple").Value),
//					SeqPi = bool.Parse (level.Element ("SeqPi").Value),
//					SeqRecaman = bool.Parse (level.Element ("SeqRecaman").Value),
//					ConsumablesCheckpoints = int.Parse (level.Element ("ConsumablesCheckpoints").Value),
//					ConsumablesAdditions = int.Parse (level.Element ("ConsumablesAdditions").Value),
//					ConsumablesSubtractions = int.Parse (level.Element ("ConsumablesSubtractions").Value),
//					ConsumablesNexts = int.Parse (level.Element ("ConsumablesNexts").Value),
//					BonusDoubleScore = bool.Parse (level.Element ("BonusDoubleScore").Value),
//					BonusTripleScore = bool.Parse (level.Element ("BonusTripleScore").Value),
//					BonusCheckpoint = bool.Parse (level.Element ("BonusCheckpoint").Value),
//					BonusPosTime = bool.Parse (level.Element ("BonusPosTime").Value),
//					BonusPosPoints = bool.Parse (level.Element ("BonusPosPoints").Value),
//					BonusTapStrength = bool.Parse (level.Element ("BonusTapStrength").Value),
//					BonusAddition = bool.Parse (level.Element ("BonusAddition").Value),
//					BonusSubtraction = bool.Parse (level.Element ("BonusSubtraction").Value),
//					BonusNextMF = bool.Parse (level.Element ("BonusNextMF").Value),
//					BonusBonusMF = bool.Parse (level.Element ("BonusBonusMF").Value),
//					BonusNextInSeq = bool.Parse (level.Element ("BonusNextInSequence").Value),
//					BonusMystery = bool.Parse (level.Element ("BonusMystery").Value),
//		UnlockSequenceUI = bool.Parse (level.Element ("UnlockSequenceUI").Value),
//		UnlockBonusUI = bool.Parse (level.Element ("UnlockBonusUI").Value),
//		UnlockConsumableUI = bool.Parse (level.Element ("UnlockConsumableUI").Value),
//				}).ToList ();		
//			return lvl;
//		}

		//---------------------------------------------------------------------------------------------------------
		// CheckForFadedBubbles
		//---------------------------------------------------------------------------------------------------------
		// Check to see if a bubble faded and died.  If so, replace it
		//---------------------------------------------------------------------------------------------------------
		void CheckForFadedBubbles()
		{
			//check if a bubble has popped on its own
			if (Bubbles.ChildrenCount < visibleBubbles) {
				var newBubbleOrder = new ShuffleBag<int> ();

				for (int i = 0; i < BubbleOccupiedArray.Length; i++) {
					if (!BubbleOccupiedArray [i])
						newBubbleOrder.Add (i);
				}

				var newBubbleIndex = newBubbleOrder.Next ();
				var newBubble = new MenuBubble ((newBubbleIndex % MAX_BUBBLES_X), (newBubbleIndex / MAX_BUBBLES_X), newBubbleIndex);
				CCPoint p = GetRandomPosition (newBubble.XIndex, newBubble.YIndex);
				newBubble.Position = new CCPoint (p.X, p.Y);
				newBubble.Scale = BUBBLE_SCALE;
				DisplayBubble (newBubble);
				Bubbles.AddChild (newBubble);
				BubbleOccupiedArray [newBubbleIndex] = true;
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// GetRandomPosition
		//---------------------------------------------------------------------------------------------------------
		// Get a position based on the index and sprite size
		//---------------------------------------------------------------------------------------------------------
		static CCPoint GetRandomPosition (int xIndex, int yIndex)
		{
			double rndX = CCRandom.GetRandomFloat ((xIndex * CELL_DIMS_HALF * 2) + (SCREEN_X_MARGIN + CELL_DIMS_HALF - CELL_CENTER_ZONE_HALF), (xIndex * CELL_DIMS_HALF * 2) + (SCREEN_X_MARGIN + CELL_DIMS_HALF + CELL_CENTER_ZONE_HALF));
			double rndY = CCRandom.GetRandomFloat ((yIndex * CELL_DIMS_HALF * 2) + (SCREEN_Y_MARGIN + CELL_DIMS_HALF - CELL_CENTER_ZONE_HALF), (yIndex * CELL_DIMS_HALF * 2) + (SCREEN_Y_MARGIN + CELL_DIMS_HALF + CELL_CENTER_ZONE_HALF));
			return new CCPoint ((float)rndX, (float)rndY);
		}

		//---------------------------------------------------------------------------------------------------------
		// DisplayBubble
		//---------------------------------------------------------------------------------------------------------
		// Displays a bubble
		//---------------------------------------------------------------------------------------------------------
		async void DisplayBubble(Bubble newBubble)
		{
			await newBubble.BubbleSprite.RunActionsAsync (new CCFadeIn (newBubble.TimeAppear), new CCDelayTime (newBubble.TimeHold), new CCFadeOut (newBubble.TimeFade));
			BubbleOccupiedArray [newBubble.ListIndex] = false;
			newBubble.RemoveFromParent ();
		}

		//---------------------------------------------------------------------------------------------------------
		// Start - Used in menu selection
		//---------------------------------------------------------------------------------------------------------
		// Transitions to Start Menu layer
		//---------------------------------------------------------------------------------------------------------
		void Start (object stuff = null)
		{
			var layer = StartLayer.CreateScene (Window);
			var transitionToLayer = new CCTransitionSlideInR(0.2f, layer);
			Director.ReplaceScene (transitionToLayer);
		}

		//---------------------------------------------------------------------------------------------------------
		// Credits - Used in menu selection
		//---------------------------------------------------------------------------------------------------------
		// Transitions to Credits layer
		//---------------------------------------------------------------------------------------------------------
		void Credits (object stuff = null)
		{
			var layer = CreditsLayer.CreateScene (Window);
			var transitionToLayer = new CCTransitionSlideInL(0.2f, layer);
			Director.ReplaceScene (transitionToLayer);
		}
	}
}

