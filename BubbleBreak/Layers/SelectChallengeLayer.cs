// -----------------------------------------------------------------------------------------------
//  <copyright file="SelectPlayMode.cs" company="RetroTek Software Ltd">
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
	public class SelectChallengeLayer : CCLayerColor
	{
		const string GOTHIC_30_HD_FNT = "gothic-30-hd.fnt";
		const string GOTHIC_44_HD_FNT = "gothic-44-hd.fnt";
		const string GOTHIC_56_WHITE_HD_FNT = "gothic-56-white-hd.fnt";
		const string GOTHIC_56_WHITE_FNT = "gothic-56-white.fnt";

		const string LEVEL_DATA_FOLDER = "./LevelData/";

		Player currentPlayer;

		List<Branch> branches;
		List<Level> levels;
		XDocument branchInfo = new XDocument ();
		XDocument levelInfo = new XDocument ();

		CCSpriteSheet uiSpriteSheet;

		CCSprite s01StdSprite, s02StdSprite, s03StdSprite, s04StdSprite, s05StdSprite, s06StdSprite, s07StdSprite, s08StdSprite, s09StdSprite, s10StdSprite, s11StdSprite, s12StdSprite;
		CCSprite s01SelSprite, s02SelSprite, s03SelSprite, s04SelSprite, s05SelSprite, s06SelSprite, s07SelSprite, s08SelSprite, s09SelSprite, s10SelSprite, s11SelSprite, s12SelSprite;
		CCSprite s01DisSprite, s02DisSprite, s03DisSprite, s04DisSprite, s05DisSprite, s06DisSprite, s07DisSprite, s08DisSprite, s09DisSprite, s10DisSprite, s11DisSprite, s12DisSprite;
		CCMenu sequenceChallengesLeft, sequenceChallengesRight, challengeInfoLeft, challengeInfoRight;

		List<CCMenuItemImage> challengeMenuItems, infoMenuItems;
		CCLabel backLabel;

		//---------------------------------------------------------------------------------------------------------
		// SelectChallengeLayer constructor
		//---------------------------------------------------------------------------------------------------------
		public SelectChallengeLayer (Player activePlayer)
		{
			currentPlayer = activePlayer;

// 			need a method to properly get these
//			listOfBranches = branches;
//			listOfLevels = levels;

			Color = new CCColor3B(0,0,0);
			Opacity = 255;
		}

		//---------------------------------------------------------------------------------------------------------
		// AddedToScene
		//--------------------------------------------------------------------------------------------------------- 
		protected override void AddedToScene ()
		{
			base.AddedToScene ();

			// Use the bounds to layout the positioning of our drawable assets
			CCRect bounds = VisibleBoundsWorldspace;

			branches = ReadBranches (branchInfo);
			levels = ReadLevels (levelInfo);

			SetupUI (bounds);
		}

		//---------------------------------------------------------------------------------------------------------
		// CreateScene
		//---------------------------------------------------------------------------------------------------------
		public static CCScene CreateScene (CCWindow mainWindow, Player activePlayer)
		{

			var scene = new CCScene (mainWindow);
			var layer = new SelectChallengeLayer (activePlayer);

			scene.AddChild (layer);

			return scene;
		}

		//---------------------------------------------------------------------------------------------------------
		// SetupUI
		//---------------------------------------------------------------------------------------------------------
		// Sets up the game UI
		//---------------------------------------------------------------------------------------------------------
		void SetupUI (CCRect bounds)
		{
			uiSpriteSheet = new CCSpriteSheet ("ui.plist");

			s01StdSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s01-linear.png")));
			s01SelSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s01-linear-g.png")));
			s01DisSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s01-linear.png")));

			s02StdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s02-even.png")));
			s02SelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s02-even-g.png")));
			s02DisSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s02-even.png")));

			s03StdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s03-odd.png")));
			s03SelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s03-odd-g.png")));
			s03DisSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s03-odd.png")));

			s04StdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s04-triangular.png")));
			s04SelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s04-triangular-g.png")));
			s04DisSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s04-triangular.png")));

			s05StdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s05-square.png")));
			s05SelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s05-square-g.png")));
			s05DisSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s05-square.png")));

			s06StdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s06-lazy.png")));
			s06SelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s06-lazy-g.png")));
			s06DisSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s06-lazy.png")));

			s07StdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s07-fibonacci.png")));
			s07SelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s07-fibonacci-g.png")));
			s07DisSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s07-fibonacci.png")));

			s08StdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s08-prime.png")));
			s08SelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s08-prime-g.png")));
			s08DisSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s08-prime.png")));

			s09StdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s09-double.png")));
			s09SelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s09-double-g.png")));
			s09DisSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s09-double.png")));

			s10StdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s10-triple.png")));
			s10SelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s10-triple-g.png")));
			s10DisSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s10-triple.png")));

			s11StdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s11-pi.png")));
			s11SelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s11-pi-g.png")));
			s11DisSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s11-pi.png")));

			s12StdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s12-recaman.png")));
			s12SelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s12-recaman-g.png")));
			s12DisSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s12-recaman.png")));

			var s01MenuItem = new CCMenuItemImage (s01StdSprite, s01SelSprite, s01DisSprite, ChallengeMenuCallback) {
				UserData = "Linear",
				Scale = 2,
				ZoomBehaviorOnTouch = false,
			};
			var s02MenuItem = new CCMenuItemImage (s02StdSprite, s02SelSprite, s02DisSprite, ChallengeMenuCallback) {
				UserData = "Even",
				Scale = 2,
				ZoomBehaviorOnTouch = false,
			};
			var s03MenuItem = new CCMenuItemImage (s03StdSprite, s03SelSprite, s03DisSprite, ChallengeMenuCallback) {
				UserData = "Odd",
				Scale = 2,
				ZoomBehaviorOnTouch = false,
			};
			var s04MenuItem = new CCMenuItemImage (s04StdSprite, s04SelSprite, s04DisSprite, ChallengeMenuCallback) {
				UserData = "Triangular",
				Scale = 2,
				ZoomBehaviorOnTouch = false,
			};
			var s05MenuItem = new CCMenuItemImage (s05StdSprite, s05SelSprite, s05DisSprite, ChallengeMenuCallback) {
				UserData = "Square",
				Scale = 2,
				ZoomBehaviorOnTouch = false,
			};
			var s06MenuItem = new CCMenuItemImage (s06StdSprite, s06SelSprite, s06DisSprite, ChallengeMenuCallback) {
				UserData = "Lazy",
				Scale = 2,
				ZoomBehaviorOnTouch = false,
			};
			var s07MenuItem = new CCMenuItemImage (s07StdSprite ,s07SelSprite, s07DisSprite, ChallengeMenuCallback) {
				UserData = "Fibonacci",
				Scale = 2,
				ZoomBehaviorOnTouch = false,
			};
			var s08MenuItem = new CCMenuItemImage (s08StdSprite, s08SelSprite, s08DisSprite, ChallengeMenuCallback) {
				UserData = "Prime",
				Scale = 2,
				ZoomBehaviorOnTouch = false,
			};
			var s09MenuItem = new CCMenuItemImage (s09StdSprite, s09SelSprite, s09DisSprite, ChallengeMenuCallback) {
				UserData = "Double",
				Scale = 2,
				ZoomBehaviorOnTouch = false,
			};
			var s10MenuItem = new CCMenuItemImage (s10StdSprite, s10SelSprite, s10DisSprite, ChallengeMenuCallback) {
				UserData = "Triple",
				Scale = 2,
				ZoomBehaviorOnTouch = false,
			};
			var s11MenuItem = new CCMenuItemImage (s11StdSprite, s11SelSprite, s11DisSprite, ChallengeMenuCallback) {
				UserData = "Pi",
				Scale = 2,
				ZoomBehaviorOnTouch = false,
			};
			var s12MenuItem = new CCMenuItemImage (s12StdSprite, s12SelSprite, s12DisSprite, ChallengeMenuCallback) {
				UserData = "Recaman",
				Scale = 2,
				ZoomBehaviorOnTouch = false,
			};

			challengeMenuItems = new List<CCMenuItemImage> ();
			challengeMenuItems.Add (s01MenuItem);
			challengeMenuItems.Add (s02MenuItem);
			challengeMenuItems.Add (s03MenuItem);
			challengeMenuItems.Add (s04MenuItem);
			challengeMenuItems.Add (s05MenuItem);
			challengeMenuItems.Add (s06MenuItem);
			challengeMenuItems.Add (s07MenuItem);
			challengeMenuItems.Add (s08MenuItem);
			challengeMenuItems.Add (s09MenuItem);
			challengeMenuItems.Add (s10MenuItem);
			challengeMenuItems.Add (s11MenuItem);
			challengeMenuItems.Add (s12MenuItem);

			sequenceChallengesLeft = new CCMenu (s01MenuItem, s02MenuItem, s03MenuItem, s04MenuItem, s05MenuItem, s06MenuItem);
			sequenceChallengesLeft.AlignItemsVertically (50);
			sequenceChallengesLeft.AnchorPoint = CCPoint.AnchorMiddle;
			sequenceChallengesLeft.PositionX = bounds.MidX - 350;
			sequenceChallengesLeft.PositionY = bounds.MidY + 70;
			AddChild (sequenceChallengesLeft);

			sequenceChallengesRight = new CCMenu (s07MenuItem, s08MenuItem, s09MenuItem, s10MenuItem, s11MenuItem, s12MenuItem);
			sequenceChallengesRight.AlignItemsVertically (50);
			sequenceChallengesRight.AnchorPoint = CCPoint.AnchorMiddle;
			sequenceChallengesRight.PositionX = bounds.MidX + 200;
			sequenceChallengesRight.PositionY = bounds.MidY + 70;
			AddChild (sequenceChallengesRight);

			var infoStdSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("info_std.png")));
			var infoSelSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("info_sel.png")));
			var infoDisSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("info_std.png")));

			var i01MenuItem = new CCMenuItemImage (infoStdSprite, infoSelSprite, infoDisSprite, InfoMenuCallback) {
				UserData = "Linear",
				Scale = 1.3f,
				ZoomBehaviorOnTouch = false,
			};
			var i02MenuItem = new CCMenuItemImage (infoStdSprite, infoSelSprite, infoDisSprite, InfoMenuCallback) {
				UserData = "Even",
				Scale = 1.3f,
				ZoomBehaviorOnTouch = false,
			};
			var i03MenuItem = new CCMenuItemImage (infoStdSprite, infoSelSprite, infoDisSprite, InfoMenuCallback) {
				UserData = "Odd",
				Scale = 1.3f,
				ZoomBehaviorOnTouch = false,
			};
			var i04MenuItem = new CCMenuItemImage (infoStdSprite, infoSelSprite, infoDisSprite, InfoMenuCallback) {
				UserData = "Triangular",
				Scale = 1.3f,
				ZoomBehaviorOnTouch = false,
			};
			var i05MenuItem = new CCMenuItemImage (infoStdSprite, infoSelSprite, infoDisSprite, InfoMenuCallback) {
				UserData = "Square",
				Scale = 1.3f,
				ZoomBehaviorOnTouch = false,
			};
			var i06MenuItem = new CCMenuItemImage (infoStdSprite, infoSelSprite, infoDisSprite, InfoMenuCallback) {
				UserData = "Lazy",
				Scale = 1.3f,
				ZoomBehaviorOnTouch = false,
			};
			var i07MenuItem = new CCMenuItemImage (infoStdSprite, infoSelSprite, infoDisSprite, InfoMenuCallback) {
				UserData = "Fibonacci",
				Scale = 1.3f,
				ZoomBehaviorOnTouch = false,
			};
			var i08MenuItem = new CCMenuItemImage (infoStdSprite, infoSelSprite, infoDisSprite, InfoMenuCallback) {
				UserData = "Prime",
				Scale = 1.3f,
				ZoomBehaviorOnTouch = false,
			};
			var i09MenuItem = new CCMenuItemImage (infoStdSprite, infoSelSprite, infoDisSprite, InfoMenuCallback) {
				UserData = "Double",
				Scale = 1.3f,
				ZoomBehaviorOnTouch = false,
			};
			var i10MenuItem = new CCMenuItemImage (infoStdSprite, infoSelSprite, infoDisSprite, InfoMenuCallback) {
				UserData = "Triple",
				Scale = 1.3f,
				ZoomBehaviorOnTouch = false,
			};
			var i11MenuItem = new CCMenuItemImage (infoStdSprite, infoSelSprite, infoDisSprite, InfoMenuCallback) {
				UserData = "Pi",
				Scale = 1.3f,
				ZoomBehaviorOnTouch = false,
			};
			var i12MenuItem = new CCMenuItemImage (infoStdSprite, infoSelSprite, infoDisSprite, InfoMenuCallback) {
				UserData = "Recaman",
				Scale = 1.3f,
				ZoomBehaviorOnTouch = false,
			};

			infoMenuItems = new List<CCMenuItemImage> ();
			infoMenuItems.Add (i01MenuItem);
			infoMenuItems.Add (i02MenuItem);
			infoMenuItems.Add (i03MenuItem);
			infoMenuItems.Add (i04MenuItem);
			infoMenuItems.Add (i05MenuItem);
			infoMenuItems.Add (i06MenuItem);
			infoMenuItems.Add (i07MenuItem);
			infoMenuItems.Add (i08MenuItem);
			infoMenuItems.Add (i09MenuItem);
			infoMenuItems.Add (i10MenuItem);
			infoMenuItems.Add (i11MenuItem);
			infoMenuItems.Add (i12MenuItem);

			challengeInfoLeft = new CCMenu (i01MenuItem, i02MenuItem, i03MenuItem, i04MenuItem, i05MenuItem, i06MenuItem);
			challengeInfoLeft.AlignItemsVertically (50);
			challengeInfoLeft.AnchorPoint = CCPoint.AnchorMiddle;
			challengeInfoLeft.PositionX = bounds.MidX - 200;
			challengeInfoLeft.PositionY = bounds.MidY + 70;
			AddChild (challengeInfoLeft);

			challengeInfoRight = new CCMenu (i07MenuItem, i08MenuItem, i09MenuItem, i10MenuItem, i11MenuItem, i12MenuItem);
			challengeInfoRight.AlignItemsVertically (50);
			challengeInfoRight.AnchorPoint = CCPoint.AnchorMiddle;
			challengeInfoRight.PositionX = bounds.MidX + 350;
			challengeInfoRight.PositionY = bounds.MidY + 70;
			AddChild (challengeInfoRight);

			// disable any challenge and info menu items for those that aren't unlocked
			for (int i = 1; i < 13; i++) {
				challengeMenuItems [i - 1].Enabled = !currentPlayer.BranchProgression [i].IsLocked;
				infoMenuItems [i-1].Enabled = !currentPlayer.BranchProgression [i].IsLocked;
				if (!challengeMenuItems [i - 1].Enabled) {
					challengeMenuItems [i - 1].Color = CCColor3B.DarkGray;
					infoMenuItems [i - 1].Color = CCColor3B.DarkGray;
				}
			}

			// disable any challenge modes that are marked as completed
			for (int i = 1; i < 13; i++) {
				if (challengeMenuItems [i - 1].Enabled) { 
					if (Equals (currentPlayer.BranchProgression [i].BranchState, CompletionState.completed)) //TODO: When a branch is completed, set the completion state to completed
						challengeMenuItems [i - 1].Enabled = false;
					if (currentPlayer.BranchProgression [i].LastLevelCompleted > 19) {
						challengeMenuItems [i - 1].Enabled = false; //TODO: remove this when the completion state is properly set
						infoMenuItems [i - 1].Enabled = false;
					}
				}
				if (!challengeMenuItems [i - 1].Enabled) {
					challengeMenuItems [i - 1].Color = CCColor3B.DarkGray;
					infoMenuItems [i - 1].Color = CCColor3B.DarkGray;
				}
			}

			var titleLabel = new CCLabel ("Select Challenge", GOTHIC_56_WHITE_HD_FNT) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 1.5f,
				PositionX = bounds.Center.X,
				PositionY = bounds.MaxY - 130,
			};
			AddChild (titleLabel);

			backLabel = new CCLabel ("Back", GOTHIC_44_HD_FNT) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 1.5f,
			};

			var backItem = new CCMenuItemLabel (backLabel, BackToGameSelect);

			var backMenu = new CCMenu (backItem);
			backMenu.AnchorPoint = CCPoint.AnchorMiddleBottom;
			backMenu.Position = new CCPoint (bounds.Size.Width / 2, 220f);

			AddChild (backMenu);
		}

		//---------------------------------------------------------------------------------------------------------
		// BackToMain
		//---------------------------------------------------------------------------------------------------------
		// Returns to Main Menu
		//---------------------------------------------------------------------------------------------------------
		void BackToGameSelect (object stuff = null)
		{
			var layer = GameSelectLayer.CreateScene (Window, currentPlayer);
			var transitionToLayer = new CCTransitionSlideInL(0.2f, layer);
			Director.ReplaceScene (transitionToLayer);
		}

		//---------------------------------------------------------------------------------------------------------
		// SeqMenuCallBack
		//---------------------------------------------------------------------------------------------------------
		// Callback when sequence UI icon is touched
		//---------------------------------------------------------------------------------------------------------
		void ChallengeMenuCallback (object pSender)
		{
			var nextLevelToPlay = new Level ();
			var sentMenuItem = (CCMenuItemImage)pSender;
			var challengeTypeString = sentMenuItem.UserData.ToString ();
			int branchProgressionIndex; // index of current branch progression
			switch (challengeTypeString) {
			case "Linear":
				{
					branchProgressionIndex = 1;
					currentPlayer.ActiveBranch = BranchType.Linear;
					nextLevelToPlay = GetNextLevelToPlay (branchProgressionIndex);
					currentPlayer.BranchProgression [branchProgressionIndex].BranchState = CompletionState.started;
					break;
				}
			case "Even":
				{
					branchProgressionIndex = 2;
					currentPlayer.ActiveBranch = BranchType.Even;
					nextLevelToPlay = GetNextLevelToPlay (branchProgressionIndex);
					currentPlayer.BranchProgression [branchProgressionIndex].BranchState = CompletionState.started;
					break;
				}
			case "Odd":
				{
					branchProgressionIndex = 3;
					currentPlayer.ActiveBranch = BranchType.Odd;
					nextLevelToPlay = GetNextLevelToPlay (branchProgressionIndex);
					currentPlayer.BranchProgression [branchProgressionIndex].BranchState = CompletionState.started;
					break;
				}
			case "Triangular":
				{
					branchProgressionIndex = 4;
					currentPlayer.ActiveBranch = BranchType.Triangular;
					nextLevelToPlay = GetNextLevelToPlay (branchProgressionIndex);
					currentPlayer.BranchProgression [branchProgressionIndex].BranchState = CompletionState.started;
					break;
				}
			case "Square":
				{
					branchProgressionIndex = 5;
					currentPlayer.ActiveBranch = BranchType.Square;
					nextLevelToPlay = GetNextLevelToPlay (branchProgressionIndex);
					currentPlayer.BranchProgression [branchProgressionIndex].BranchState = CompletionState.started;
					break;
				}
			case "Lazy":
				{
					branchProgressionIndex = 6;
					currentPlayer.ActiveBranch = BranchType.Lazy;
					nextLevelToPlay = GetNextLevelToPlay (branchProgressionIndex);
					currentPlayer.BranchProgression [branchProgressionIndex].BranchState = CompletionState.started;
					break;
				}
			case "Fibonacci":
				{
					branchProgressionIndex = 7;
					currentPlayer.ActiveBranch = BranchType.Fibonacci;
					nextLevelToPlay = GetNextLevelToPlay (branchProgressionIndex);
					currentPlayer.BranchProgression [branchProgressionIndex].BranchState = CompletionState.started;
					break;
				}
			case "Prime":
				{
					branchProgressionIndex = 8;
					currentPlayer.ActiveBranch = BranchType.Prime;
					nextLevelToPlay = GetNextLevelToPlay (branchProgressionIndex);
					currentPlayer.BranchProgression [branchProgressionIndex].BranchState = CompletionState.started;
					break;
				}
			case "Double":
				{
					branchProgressionIndex = 9;
					currentPlayer.ActiveBranch = BranchType.Double;
					nextLevelToPlay = GetNextLevelToPlay (branchProgressionIndex);
					currentPlayer.BranchProgression [branchProgressionIndex].BranchState = CompletionState.started;
					break;
				}
			case "Triple":
				{
					branchProgressionIndex = 10;
					currentPlayer.ActiveBranch = BranchType.Triple;
					nextLevelToPlay = GetNextLevelToPlay (branchProgressionIndex);
					currentPlayer.BranchProgression [branchProgressionIndex].BranchState = CompletionState.started;
					break;
				}
			case "Pi":
				{
					branchProgressionIndex = 11;
					currentPlayer.ActiveBranch = BranchType.Pi;
					nextLevelToPlay = GetNextLevelToPlay (branchProgressionIndex);
					currentPlayer.BranchProgression [branchProgressionIndex].BranchState = CompletionState.started;
					break;
				}
			case "Recaman":
				{
					branchProgressionIndex = 12;
					currentPlayer.ActiveBranch = BranchType.Recaman;
					nextLevelToPlay = GetNextLevelToPlay (branchProgressionIndex);
					currentPlayer.BranchProgression [branchProgressionIndex].BranchState = CompletionState.started;
					break;
				}
			}

			// TODO: because we set the branch progression after a challenge is selected, we should write to the player file 

			var layer = ChallengeLevelLayer.CreateScene (Window, nextLevelToPlay, currentPlayer);
			var transitionToLayer = new CCTransitionSlideInR(0.2f, layer);
			Director.ReplaceScene (transitionToLayer);
		}

		//---------------------------------------------------------------------------------------------------------
		// GetNextLevelToPlay
		//---------------------------------------------------------------------------------------------------------
		// Returns the level from the list of levels, taking into account the current branch
		//---------------------------------------------------------------------------------------------------------
		Level GetNextLevelToPlay (int branchProgressionIndex)
		{
			int numberOfLevels = 0;
			for (int i = 0; i < branchProgressionIndex; i++) {
				numberOfLevels += branches [i].NumberOfLevels;
			}

			if (currentPlayer.BranchProgression [branchProgressionIndex].LastLevelCompleted == -1)
				numberOfLevels += 1;
			
			return levels[(currentPlayer.BranchProgression[branchProgressionIndex].LastLevelCompleted) + numberOfLevels];
		}

		//---------------------------------------------------------------------------------------------------------
		// InfoMenuCallBack
		//---------------------------------------------------------------------------------------------------------
		// Callback when info UI icon is touched
		//---------------------------------------------------------------------------------------------------------
		void InfoMenuCallback (object pSender)
		{
		}

		//---------------------------------------------------------------------------------------------------------
		// ReadBranches
		//---------------------------------------------------------------------------------------------------------
		// Reads the contents of the branches.xml file and builds a list of branch objects
		//---------------------------------------------------------------------------------------------------------
		public List<Branch> ReadBranches(XDocument branchInfo)
		{
			branchInfo = XDocument.Load (LEVEL_DATA_FOLDER + "branches.xml");
			List<Branch> branchList = (from branch in branchInfo.Root.Descendants ("record")
				select new Branch {
					BranchNum = int.Parse (branch.Element ("BranchNum").Value),
					BranchName = branch.Element ("BranchName").Value,
					NumberOfLevels = int.Parse (branch.Element ("NumberOfLevels").Value),
					UnlockNextBranch = int.Parse (branch.Element ("UnlockNextBranch").Value),
					UnlockFreePlay = int.Parse (branch.Element ("UnlockFreePlay").Value),
				}).ToList ();
			return branchList;
		}

		//---------------------------------------------------------------------------------------------------------
		// ReadLevels
		//---------------------------------------------------------------------------------------------------------
		// Using the list of branches, reads each individual level xml file and builds a list of level objects 
		//---------------------------------------------------------------------------------------------------------
		public List<Level> ReadLevels(XDocument levelData)
		{
			levelData = XDocument.Load (LEVEL_DATA_FOLDER + "levels.xml");

			List<Level> lvl = (from level in levelData.Root.Descendants ("record")
				select new Level {
					BranchNum = int.Parse (level.Element ("BranchNum").Value), 
					LevelNum = int.Parse (level.Element ("LevelNum").Value),
					LevelName = level.Element ("LevelName").Value,
					MaxBubbles = int.Parse (level.Element ("MaxBubbles").Value),
					MaxVisibleBubbles = int.Parse (level.Element ("MaxVisibleBubbles").Value),
					StartingPointValue = int.Parse (level.Element ("StartingPointValue").Value),
					MaxLevelTime = int.Parse (level.Element ("MaxLevelTime").Value),
					LevelPassScore = int.Parse (level.Element ("LevelPassScore").Value),
					TapsToPopStandard = int.Parse (level.Element ("TapsToPopStandard").Value),
					InitialVisibleBubbles = int.Parse (level.Element ("InitialVisibleBubbles").Value),
					ChanceToRollNextSeq = int.Parse (level.Element ("ChanceToRollNextSeq").Value),
					ChanceToRollBonus = int.Parse (level.Element ("ChanceToRollBonus").Value),
					LevelDescription = level.Element ("LevelDescription").Value,
					SeqLinear = bool.Parse (level.Element ("SeqLinear").Value),
					SeqEven = bool.Parse ( level.Element ("SeqEven").Value),
					SeqOdd = bool.Parse (level.Element ("SeqOdd").Value),
					SeqTriangular = bool.Parse (level.Element ("SeqTriangular").Value),
					SeqSquare = bool.Parse (level.Element ("SeqSquare").Value),
					SeqLazy = bool.Parse (level.Element ("SeqLazy").Value),
					SeqFibonacci = bool.Parse (level.Element ("SeqFibonacci").Value),
					SeqPrime = bool.Parse (level.Element ("SeqPrime").Value),
					SeqDouble = bool.Parse (level.Element ("SeqDouble").Value),
					SeqTriple = bool.Parse (level.Element ("SeqTriple").Value),
					SeqPi = bool.Parse (level.Element ("SeqPi").Value),
					SeqRecaman = bool.Parse (level.Element ("SeqRecaman").Value),
					ConsumablesCheckpoints = int.Parse (level.Element ("ConsumablesCheckpoints").Value),
					ConsumablesAdditions = int.Parse (level.Element ("ConsumablesAdditions").Value),
					ConsumablesSubtractions = int.Parse (level.Element ("ConsumablesSubtractions").Value),
					ConsumablesNexts = int.Parse (level.Element ("ConsumablesNexts").Value),
					BonusDoubleScore = bool.Parse (level.Element ("BonusDoubleScore").Value),
					BonusTripleScore = bool.Parse (level.Element ("BonusTripleScore").Value),
					BonusCheckpoint = bool.Parse (level.Element ("BonusCheckpoint").Value),
					BonusPosTime = bool.Parse (level.Element ("BonusPosTime").Value),
					BonusPosPoints = bool.Parse (level.Element ("BonusPosPoints").Value),
					BonusTapStrength = bool.Parse (level.Element ("BonusTapStrength").Value),
					BonusAddition = bool.Parse (level.Element ("BonusAddition").Value),
					BonusSubtraction = bool.Parse (level.Element ("BonusSubtraction").Value),
					BonusNextMF = bool.Parse (level.Element ("BonusNextMF").Value),
					BonusBonusMF = bool.Parse (level.Element ("BonusBonusMF").Value),
					BonusNextInSeq = bool.Parse (level.Element ("BonusNextInSequence").Value),
					BonusMystery = bool.Parse (level.Element ("BonusMystery").Value),
					BonusLuckyPointChance = bool.Parse (level.Element ("BonusCritPointChance").Value),
					BonusLuckyPointAmount = bool.Parse (level.Element ("BonusCritPointAmount").Value),
					UnlockSequenceUI = bool.Parse (level.Element ("UnlockSequenceUI").Value),
					UnlockBonusUI = bool.Parse (level.Element ("UnlockBonusUI").Value),
					UnlockConsumableUI = bool.Parse (level.Element ("UnlockConsumableUI").Value),
				}).ToList ();		
			return lvl;
		}
	}
}


