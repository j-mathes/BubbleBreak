// -----------------------------------------------------------------------------------------------
//  <copyright file="TutorialLayer.cs" company="RetroTek Software Ltd">
//      Copyright (C) 2016 RetroTek Software Ltd. All rights reserved.
//  </copyright>
//  <author>Jared Mathes</author>
// -----------------------------------------------------------------------------------------------
//BUG: check if 2x or 3x multiplier affects point bonus
using System;
using System.Collections.Generic;
using System.Linq;
using CocosSharp;

namespace BubbleBreak
{
	public class TutorialLayer : CCLayerColor
	{
		const float BUBBLE_SCALE = 1.15f;			// amount we want to scale the bubble by.  Should change the sprite graphic so this isn't needed.
		const int MAX_BUBBLES_X = 5;				// bubble grid width
		const int MAX_BUBBLES_Y = 8;				// bubble grid height
		const float SCREEN_X_MARGIN = 80.0f;		// margin from the left reserved for UI controls
		const float SCREEN_Y_MARGIN = 120f; 		// margin from the bottom for UI controls.  320 total  120 for bottom, 200 for top
		const float CELL_DIMS_HALF = 100f;			// half width of a bubble grid cell
		const float CELL_CENTER_ZONE_HALF = 28f;	// half width of the area in the center of the cell that a bubble can randomly be placed

		const float COIN_MULTIPLIER = 0.01f;		// multiplier to determin how many bonus coins the player gets - 100 points gets 1 coin

		const String PI_DIGITS = "31415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679821480865132823066470938446095505822317253594081284811174502841027019385211055596446229489549303819644288109756659334461";

		const string GOTHIC_30_HD_FNT = "gothic-30-hd.fnt";
		const string GOTHIC_44_HD_FNT = "gothic-44-hd.fnt";
		const string GOTHIC_56_WHITE_HD_FNT = "gothic-56-white-hd.fnt";
		const string GOTHIC_56_WHITE_FNT = "gothic-56-white.fnt";

		const int POINT_INCREMEMNT_DELAY = 50; 				// delay in 10ths of a second to increase the highest random point value generated
		const int BUBBLES_VISIBLE_INCREMENT_DELAY = 20; 	// delay in 10ths of a second to increase the number of bubbles on the screen

		const int SCORE_MULTIPLIER_DURATION = 4;			// duration in seconds + 1 since the count starts at zero, not one
		const int DEBUG_CONSUMABLES = 0;

		CCNode bubbles;

		CCSpriteSheet uiSpriteSheet;
		CCSpriteSheet bonusSpriteSheet;

		Player activePlayer;
		Level activeLevel;
		int tapStrength;
		int playerTapStrength;
		int levelTapStrength;							 

		int chanceToRollBonus;
		int playerChanceToRollBonus;
		int levelChanceToRollBonus;	

		int chanceToRollNextSeq;
		int playerChanceToRollNextSeq;
		int levelChanceToRollNextSeq;	

		int chanceForLuckyPoints;
		int maxLuckyPointAmount;

		// level consumables
		int availableCheckpoints;
		int availableAdditions;
		int availableSubtractions;
		int availableNextInSeq;

		int bonusDoubleMultiplier, bonusTripleMultiplier;

		bool playerHighlightNext;

		List<Sequences> availableSequences;				// list of sequences available to this level
		List<int> listOfPrimes;
		List<int> listOfRecamans;

		int levelNumber;								// level index number
		int maxBubbles; 								// the maximum amount of bubbles that will fit on the screen at one time.
		int maxVisibleBubbles;							// the limit for how many bubbles we actually want visible on the screen
		int maxLevelPoints;								// maximum point value for new bubbles.  Initially assigned by the currentLevel property but increased as the level progresses

		bool[] bubbleOccupiedArray;
		bool didPassLevel;

		Random scoreRandom = new Random();			// randomizer used for getting a new bubble score

		CCPoint tapLocation;
		CCLabel tapStrengthLabel; 

		CCLabel sequenceTotalLabel;
		CCLabel nextInSequenceLabel;
		CCLabel timeIDLabel, scoreIDLabel, okLabel;

		CCSprite timeSpriteProgressBarEmpty, scoreSpriteProgressBarEmpty, timeSpriteProgressBarFull, scoreSpriteProgressBarFull, tapStrengthSprite, medKitSprite; 
		CCSprite s01StdSprite, s01SelSprite, s02StdSprite, s02SelSprite, s03StdSprite, s03SelSprite, s04StdSprite, s04SelSprite, s05StdSprite, s05SelSprite, s06StdSprite, s06SelSprite,
		s07StdSprite, s07SelSprite, s08StdSprite, s08SelSprite, s09StdSprite, s09SelSprite, s10StdSprite, s10SelSprite, s11StdSprite, s11SelSprite, s12StdSprite, s12SelSprite;

		CCSprite checkpointStdSprite, checkpointSelSprite, additionStdSprite, additionSelSprite, subtractionStdSprite, subtractionSelSprite, nextInSequenceStdSprite, nextInSequenceSelSprite; 
		CCLabel checkpointsCountLabel, additionsCountLabel, subtractionsCountLabel, nextInSequencesCountLabel, additionsNumbersLabel, subtractionsNumbersLabel; 
		CCMenu seqMenu01, seqMenu02, seqMenu03, seqMenu04, seqMenu05, seqMenu06, seqMenu07, seqMenu08, seqMenu09, seqMenu10, seqMenu11, seqMenu12;
		CCMenu menuCheckpoint, menuAddition, menuSubtraction, menuNextInSeq;
		CCMenuItemToggle highlightToggleMenuItem;
		CCSprite checkmarkSprite;

		CCSprite frameSprite;

		CCProgressTimer timeBar, scoreBar, doubleScoreTimer, tripleScoreTimer;
		CCSprite doubleScoreTimerSprite, tripleScoreTimerSprite, doubleScoreFadedTimerSprite, tripleScoreFadedTimerSprite;

		CCLayerColor hideUILayer;

		bool isDoubleScoreActive, isTripleScoreActive;
		float doubleScoreTimerDuration, tripleScoreTimerDuration;

		Sequences currentSequence; //= Sequences.Linear;	// current sequence the player can build on
		Sequences selectedSequence; //= Sequences.Linear;	// to track the selected sequence so it doesn't reset if you re-select the same sequence

		int nextNumberInSequence = 1;					// the next number in the current sequence
		int seqDepth;									// how deep are we into generating a sequence
		int checkpointSequence;
		int checkpointDepth;
		bool isCheckpointActive;

		PointState pointState = PointState.None;
		int bubble1Value = -1;
		int bubble2Value = -1;

		float levelTimeLimit;		 					// how many second we have to get the levelPassScore
		float levelTimeLeft;							// decreasing time left to complete the level

		string levelTitleText, levelDescriptionText, levelTimeLimitText, levelScoreToPassText;

		int levelTimeIncrement;							// measures the increasing progress of level time.  Used to increment the maximum point value of a new bubble
		int levelScore;									// seqTotal + regularTotal
		int regularTotal;								// total points of popped bubble points (including any bonus bubble points)
		int sequenceTotal;								// total points of bubbles popped in a sequence
		bool isScoreBonusActive;							// to determine if the score progress bar should show bonus mode
		int levelPassScore;								// how many point we need to pass the level
		int tapsRequired;								// how many taps to pop a standard bubble on this level

		int levelVisibleBubbles;						// how many bubbles can currently be visible at a time.  This number increases as the level progresses
		int lastEarnedPoint;							// The value of the last point earned.  Used to determine if we need to increase the maximum point for a new bubble

		int coinsEarned;

		//---------------------------------------------------------------------------------------------------------
		// TutorialLayer
		//---------------------------------------------------------------------------------------------------------
		// Class constructor
		//---------------------------------------------------------------------------------------------------------
		public TutorialLayer (Level gameLevel, Player currentPlayer) : base (new CCColor4B (0,0,0))
		{
			activePlayer = currentPlayer;
			activeLevel = gameLevel;
			CreateListOfSequences (activeLevel);
			playerTapStrength = activePlayer.PersistentTapStrength;
			playerChanceToRollBonus = activePlayer.ChanceToRollBonus;
			playerChanceToRollNextSeq = activePlayer.ChanceToRollNextSeq;
			chanceForLuckyPoints = activePlayer.LuckyPointChance;
			maxLuckyPointAmount = activePlayer.LuckyPointBonus;
			availableCheckpoints = activeLevel.ConsumablesCheckpoints + DEBUG_CONSUMABLES;
			availableAdditions = activeLevel.ConsumablesAdditions + DEBUG_CONSUMABLES;
			availableSubtractions = activeLevel.ConsumablesSubtractions + DEBUG_CONSUMABLES;
			availableNextInSeq = activeLevel.ConsumablesNexts + DEBUG_CONSUMABLES;
			bonusDoubleMultiplier = bonusTripleMultiplier = 1;
			playerHighlightNext = (activePlayer.HighlightNextPurchased && activePlayer.IsHighlightNextActive); 
			currentSequence = availableSequences.First ();
			selectedSequence = currentSequence;

			//---------------------------------------------------------------------------------------------------------
			// Function to generate list of primes
			//---------------------------------------------------------------------------------------------------------
			Func<int, IEnumerable<int>> primeNumbers = max => 
				from i in Enumerable.Range(2, max - 1) 
					where Enumerable.Range(2, i - 2).All(j => i % j != 0) 
				select i; 
			IEnumerable<int> result = primeNumbers(500); 
			listOfPrimes = result.ToList ();
			listOfRecamans = RecamanSeq ();

			bonusSpriteSheet = new CCSpriteSheet ("bonus.plist");

			SetupUI ();
		}

		//---------------------------------------------------------------------------------------------------------
		// AddedToScene
		//--------------------------------------------------------------------------------------------------------- 
		protected override void AddedToScene ()
		{
			base.AddedToScene ();

			// Use the bounds to layout the positioning of our drawable assets
			CCRect bounds = VisibleBoundsWorldspace;

			// object instantiation code here

			InGameMenu (bounds);

			CreateBubbleArray ();

			// Register for touch events
			var touchListener = new CCEventListenerTouchAllAtOnce ();
			touchListener.OnTouchesEnded = OnTouchesEnded;
			AddEventListener (touchListener, this); 

			Schedule (_ => CheckForFadedBubbles ());
			Schedule (_ => CheckForBubbleHighlight ());
			Schedule (_ => StartScheduling ());
		}

		//---------------------------------------------------------------------------------------------------------
		// CreateScene
		//---------------------------------------------------------------------------------------------------------
		public static CCScene CreateScene (CCWindow mainWindow, Level level, Player activePlayer)
		{
			var scene = new CCScene (mainWindow);
			var layer = new TutorialLayer (level, activePlayer);

			scene.AddChild (layer);

			return scene;
		}

		//---------------------------------------------------------------------------------------------------------
		// OnEnterTransitionDidFinish
		//---------------------------------------------------------------------------------------------------------
		// Pops up at the beginning of the level and gives information about level goals.  Pauses the game until
		// the user taps on OK.  Happens after the transition to the level is finished.
		//---------------------------------------------------------------------------------------------------------
		public override void OnEnterTransitionDidFinish()
		{
			base.OnEnterTransitionDidFinish();

			uiSpriteSheet = new CCSpriteSheet ("ui.plist");


			CCRect bounds = VisibleBoundsWorldspace;

			// pause listeners
			PauseListeners (true);

			// create the level info layer over top of the game layer
			var levelInfoLayer = new CCLayerColor (new CCColor4B (0, 0, 0, 200));
			AddChild (levelInfoLayer, 99999);

			// Add the information frame sprite to layer
			frameSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("frame.png")));
			frameSprite.AnchorPoint = CCPoint.AnchorMiddle;
			frameSprite.Position = new CCPoint (bounds.Size.Width / 2, bounds.Size.Height / 2);
			levelInfoLayer.AddChild (frameSprite);

			// Add Level Information to the frame
			levelTitleText = "Level " + levelNumber;
			levelDescriptionText = "\"" + activeLevel.LevelDescription + "\"";
			levelTimeLimitText = string.Format ("Time limit: {0} seconds", activeLevel.MaxLevelTime);
			levelScoreToPassText = string.Format ("Score to pass level: {0}", activeLevel.LevelPassScore);

			// Add title lable
			var titleLabel = new CCLabel(levelTitleText, GOTHIC_44_HD_FNT);
			titleLabel.Scale = 1.5f;
			titleLabel.AnchorPoint = CCPoint.AnchorMiddle;
			titleLabel.Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MaxY - (titleLabel.BoundingBox.Size.Height * 3f));
			levelInfoLayer.AddChild (titleLabel);

			// Add level information labels
			var levelDescriptionLabel = new CCLabel (levelDescriptionText, GOTHIC_44_HD_FNT);
			levelDescriptionLabel.Scale = 1f;
			levelDescriptionLabel.Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MinY + (levelDescriptionLabel.BoundingBox.Size.Height * 14f));
			levelDescriptionLabel.AnchorPoint = CCPoint.AnchorMiddle;
			levelDescriptionLabel.HorizontalAlignment = CCTextAlignment.Center;
			levelInfoLayer.AddChild (levelDescriptionLabel);

			var levelTimeLimitLabel = new CCLabel (levelTimeLimitText, GOTHIC_44_HD_FNT);
			levelTimeLimitLabel.Scale = 1f;
			levelTimeLimitLabel.Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MinY + (levelDescriptionLabel.BoundingBox.Size.Height * 11f));
			levelTimeLimitLabel.AnchorPoint = CCPoint.AnchorMiddle;
			levelTimeLimitLabel.HorizontalAlignment = CCTextAlignment.Center;
			levelInfoLayer.AddChild (levelTimeLimitLabel);

			var levelScoreToPassLabel = new CCLabel (levelScoreToPassText, GOTHIC_44_HD_FNT);
			levelScoreToPassLabel.Scale = 1f;
			levelScoreToPassLabel.Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MinY + (levelDescriptionLabel.BoundingBox.Size.Height * 8f));
			levelScoreToPassLabel.AnchorPoint = CCPoint.AnchorMiddle;
			levelScoreToPassLabel.HorizontalAlignment = CCTextAlignment.Center;
			levelInfoLayer.AddChild (levelScoreToPassLabel);

			// set up the sprites for the OK button 
			okLabel = new CCLabel ("OK", GOTHIC_44_HD_FNT);
			okLabel.AnchorPoint = CCPoint.AnchorMiddle;
			okLabel.Scale = 2.0f;

			// create the ok button menu item
			var okMenuItem = new CCMenuItemLabel (okLabel, closeSender => {
				levelInfoLayer.RemoveFromParent ();
				hideUILayer.RemoveFromParent ();
				ResumeListeners (true);
				Application.Paused = false;
			});

			// position the ok button relative to the information frame
			okMenuItem.AnchorPoint = CCPoint.AnchorMiddle;
			okMenuItem.Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MinY + (okLabel.BoundingBox.Size.Height * 1.5f));

			// create the menu to close the pop up
			var closeMenu = new CCMenu (okMenuItem);
			closeMenu.AnchorPoint = CCPoint.AnchorMiddleBottom;
			closeMenu.Position = CCPoint.Zero;

			// add the menu to the levelInfoLayer
			levelInfoLayer.AddChild(closeMenu);
			Application.Paused = true;
		}

		//---------------------------------------------------------------------------------------------------------
		// RecamanSeq
		//---------------------------------------------------------------------------------------------------------
		// Generate the Recaman's sequence up to x iterations
		//---------------------------------------------------------------------------------------------------------
		public List<int> RecamanSeq()
		{
			var sequenceList = new List<int> (new [] {
				1, 3, 6, 2, 7, 13, 20, 12, 21, 11, 22, 10, 23,
				9, 24, 8, 25, 43, 62, 42, 63, 41, 18, 42, 17, 
				43, 16, 44, 15, 45, 14, 46, 79, 113, 78, 114, 
				77, 39, 78, 38, 79, 37, 80, 36, 81, 35, 82, 34, 
				83, 33, 84, 32, 85, 31, 86, 30, 87, 29, 88, 28, 
				89, 27, 90, 26, 91, 157, 224, 156, 225, 155
			});

			return sequenceList;
		}

		//---------------------------------------------------------------------------------------------------------
		// SetupUI
		//---------------------------------------------------------------------------------------------------------
		// Sets up the game UI
		//---------------------------------------------------------------------------------------------------------
		void SetupUI ()
		{
			// Remember to change Build Action to BundleResource for newly added font files, etc...
			// assign values from activeLevel properties to the GameLayer properties
			levelNumber = activeLevel.LevelNum;
			maxBubbles = activeLevel.MaxBubbles;
			maxVisibleBubbles = activeLevel.MaxVisibleBubbles;
			maxLevelPoints = activeLevel.StartingPointValue;
			levelTimeLimit = activeLevel.MaxLevelTime + activePlayer.PersistentTimeBonus;
			levelPassScore = activeLevel.LevelPassScore;
			tapsRequired = activeLevel.TapsToPopStandard;
			levelVisibleBubbles = activeLevel.InitialVisibleBubbles;
			levelChanceToRollNextSeq = activeLevel.ChanceToRollNextSeq;
			levelChanceToRollBonus = activeLevel.ChanceToRollBonus;

			//sequenceLevel = activeLevel.SequenceLevel;
			levelTimeLeft = levelTimeLimit + 3;
			bubbleOccupiedArray = new bool[maxBubbles];

			// create layer to hide main game ui while level intro is showing
			hideUILayer = new CCLayerColor (new CCColor4B (0, 0, 0, 255));
			AddChild (hideUILayer, 88888);

			uiSpriteSheet = new CCSpriteSheet ("ui.plist");

			checkmarkSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("checkmark-red.png")));
			checkmarkSprite.AnchorPoint = CCPoint.AnchorMiddle;
			checkmarkSprite.Scale = 1;

			tapStrengthSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("tap_strength_std.png")));
			tapStrengthSprite.PositionX = 700;
			tapStrengthSprite.PositionY = 1870;
			tapStrengthSprite.AnchorPoint = CCPoint.AnchorMiddle;
			if (activeLevel.UnlockBonusUI) AddChild (tapStrengthSprite);

			tapStrengthLabel = new CCLabel ("1", GOTHIC_30_HD_FNT);
			tapStrengthLabel.Text = tapStrength.ToString ();
			tapStrengthLabel.Scale = 2.0f;
			tapStrengthLabel.PositionX = 750;
			tapStrengthLabel.PositionY = 1870;
			tapStrengthLabel.AnchorPoint = CCPoint.AnchorMiddleLeft;
			if (activeLevel.UnlockBonusUI) AddChild (tapStrengthLabel);

			medKitSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("sprite_std_med_kit.png")));
			medKitSprite.PositionX = 890;
			medKitSprite.PositionY = 1870;
			medKitSprite.AnchorPoint = CCPoint.AnchorMiddle;
			medKitSprite.Scale = 0.8f;
			if ((activePlayer.IsMedKitActivated) && (activeLevel.UnlockBonusUI)) AddChild (medKitSprite);

			timeIDLabel = new CCLabel ("Time", GOTHIC_30_HD_FNT);
			timeIDLabel.PositionX = 10;
			timeIDLabel.PositionY = 1900;
			timeIDLabel.AnchorPoint = CCPoint.AnchorMiddleLeft;
			AddChild (timeIDLabel);

			scoreIDLabel = new CCLabel ("Score", GOTHIC_30_HD_FNT);
			scoreIDLabel.AnchorPoint = CCPoint.AnchorMiddleLeft;
			scoreIDLabel.PositionX = 10;
			scoreIDLabel.PositionY = 1855;
			AddChild (scoreIDLabel);

			timeSpriteProgressBarEmpty = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("prgbar_time_empty.png")));
			timeSpriteProgressBarEmpty.AnchorPoint = CCPoint.AnchorMiddleLeft;
			timeSpriteProgressBarEmpty.PositionX = 100;
			timeSpriteProgressBarEmpty.PositionY = 1900;
			AddChild (timeSpriteProgressBarEmpty);

			scoreSpriteProgressBarEmpty = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("prgbar_score_empty.png")));
			scoreSpriteProgressBarEmpty.AnchorPoint = CCPoint.AnchorMiddleLeft;
			scoreSpriteProgressBarEmpty.PositionX = 100;
			scoreSpriteProgressBarEmpty.PositionY = 1855;
			AddChild (scoreSpriteProgressBarEmpty);

			timeSpriteProgressBarFull = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("prgbar_time_full.png")));
			timeBar = new CCProgressTimer (timeSpriteProgressBarFull);
			timeBar.AnchorPoint = CCPoint.AnchorMiddleLeft;
			timeBar.PositionX = 100;
			timeBar.PositionY = 1900;
			timeBar.Percentage = 100;
			timeBar.Midpoint = new CCPoint (0, 0);
			timeBar.BarChangeRate = new CCPoint (1, 0);
			timeBar.Type = CCProgressTimerType.Bar;
			AddChild (timeBar);

			scoreSpriteProgressBarFull = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("prgbar_score_full.png")));
			scoreBar = new CCProgressTimer (scoreSpriteProgressBarFull);
			scoreBar.AnchorPoint = CCPoint.AnchorMiddleLeft;
			scoreBar.PositionX = 100;
			scoreBar.PositionY = 1855;
			scoreBar.Midpoint = new CCPoint (0, 0);
			scoreBar.BarChangeRate = new CCPoint (1, 0);
			scoreBar.Type = CCProgressTimerType.Bar;
			AddChild (scoreBar);

			doubleScoreFadedTimerSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("timer-double-faded.png")));
			doubleScoreFadedTimerSprite.AnchorPoint = CCPoint.AnchorMiddleLeft;
			doubleScoreFadedTimerSprite.PositionX = 410;
			doubleScoreFadedTimerSprite.PositionY = 1875;
			if (activeLevel.UnlockBonusUI) AddChild (doubleScoreFadedTimerSprite);

			tripleScoreFadedTimerSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("timer-triple-faded.png")));
			tripleScoreFadedTimerSprite.AnchorPoint = CCPoint.AnchorMiddleLeft;
			tripleScoreFadedTimerSprite.PositionX = 500;
			tripleScoreFadedTimerSprite.PositionY = 1875;
			if (activeLevel.UnlockBonusUI) AddChild (tripleScoreFadedTimerSprite);

			doubleScoreTimerSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("timer-double-brite.png")));
			doubleScoreTimer = new CCProgressTimer (doubleScoreTimerSprite);
			doubleScoreTimer.AnchorPoint = CCPoint.AnchorMiddleLeft;
			doubleScoreTimer.PositionX = 410;
			doubleScoreTimer.PositionY = 1875;
			doubleScoreTimer.Type = CCProgressTimerType.Radial;
			doubleScoreTimer.Percentage = 0;
			if (activeLevel.UnlockBonusUI) AddChild (doubleScoreTimer);

			tripleScoreTimerSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("timer-triple-brite.png")));
			tripleScoreTimer = new CCProgressTimer (tripleScoreTimerSprite);
			tripleScoreTimer.AnchorPoint = CCPoint.AnchorMiddleLeft;
			tripleScoreTimer.PositionX = 500;
			tripleScoreTimer.PositionY = 1875;
			tripleScoreTimer.Type = CCProgressTimerType.Radial;
			tripleScoreTimer.Percentage = 0;
			if (activeLevel.UnlockBonusUI) AddChild (tripleScoreTimer);

			s01StdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s01-linear.png")));
			s01SelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s01-linear-g.png")));

			s02StdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s02-even.png")));
			s02SelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s02-even-g.png")));

			s03StdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s03-odd.png")));
			s03SelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s03-odd-g.png")));

			s04StdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s04-triangular.png")));
			s04SelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s04-triangular-g.png")));

			s05StdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s05-square.png")));
			s05SelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s05-square-g.png")));

			s06StdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s06-lazy.png")));
			s06SelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s06-lazy-g.png")));

			s07StdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s07-fibonacci.png")));
			s07SelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s07-fibonacci-g.png")));

			s08StdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s08-prime.png")));
			s08SelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s08-prime-g.png")));

			s09StdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s09-double.png")));
			s09SelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s09-double-g.png")));

			s10StdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s10-triple.png")));
			s10SelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s10-triple-g.png")));

			s11StdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s11-pi.png")));
			s11SelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s11-pi-g.png")));

			s12StdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s12-recaman.png")));
			s12SelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("s12-recaman-g.png")));

			var s01StdMenuItem = new CCMenuItemImage (s01StdSprite, s01SelSprite, SeqMenuCallback) {
				UserData = "Linear",
			};
			var s02StdMenuItem = new CCMenuItemImage (s02StdSprite, s02SelSprite, SeqMenuCallback) {
				UserData = "Even",
			};
			var s03StdMenuItem = new CCMenuItemImage (s03StdSprite, s03SelSprite, SeqMenuCallback) {
				UserData = "Odd",
			};
			var s04StdMenuItem = new CCMenuItemImage (s04StdSprite, s04SelSprite, SeqMenuCallback) {
				UserData = "Triangular",
			};
			var s05StdMenuItem = new CCMenuItemImage (s05StdSprite, s05SelSprite, SeqMenuCallback) {
				UserData = "Square",
			};
			var s06StdMenuItem = new CCMenuItemImage (s06StdSprite,s06SelSprite, SeqMenuCallback) {
				UserData = "Lazy",
			};
			var s07StdMenuItem = new CCMenuItemImage (s07StdSprite,s07SelSprite, SeqMenuCallback) {
				UserData = "Fibonacci",
			};
			var s08StdMenuItem = new CCMenuItemImage (s08StdSprite, s08SelSprite, SeqMenuCallback) {
				UserData = "Prime",
			};
			var s09StdMenuItem = new CCMenuItemImage (s09StdSprite, s09SelSprite, SeqMenuCallback) {
				UserData = "Double",
			};
			var s10StdMenuItem = new CCMenuItemImage (s10StdSprite, s10SelSprite, SeqMenuCallback) {
				UserData = "Triple",
			};
			var s11StdMenuItem = new CCMenuItemImage (s11StdSprite, s11SelSprite, SeqMenuCallback) {
				UserData = "Pi",
			};
			var s12StdMenuItem = new CCMenuItemImage (s12StdSprite, s12SelSprite, SeqMenuCallback) {
				UserData = "Recaman",
			};

			var seqMenuItems = new List<CCMenuItemImage> ();
			seqMenuItems.Add (s01StdMenuItem);
			seqMenuItems.Add (s02StdMenuItem);
			seqMenuItems.Add (s03StdMenuItem);
			seqMenuItems.Add (s04StdMenuItem);
			seqMenuItems.Add (s05StdMenuItem);
			seqMenuItems.Add (s06StdMenuItem);
			seqMenuItems.Add (s07StdMenuItem);
			seqMenuItems.Add (s08StdMenuItem);
			seqMenuItems.Add (s09StdMenuItem);
			seqMenuItems.Add (s10StdMenuItem);
			seqMenuItems.Add (s11StdMenuItem);
			seqMenuItems.Add (s12StdMenuItem);

			for (int i = 0; i < seqMenuItems.Count; i++) {
				seqMenuItems [i].Visible = false;
			}

			seqMenu01 = new CCMenu (s01StdMenuItem);
			seqMenu02 = new CCMenu (s02StdMenuItem);
			seqMenu03 = new CCMenu (s03StdMenuItem);
			seqMenu04 = new CCMenu (s04StdMenuItem);
			seqMenu05 = new CCMenu (s05StdMenuItem);
			seqMenu06 = new CCMenu (s06StdMenuItem);
			seqMenu07 = new CCMenu (s07StdMenuItem);
			seqMenu08 = new CCMenu (s08StdMenuItem);
			seqMenu09 = new CCMenu (s09StdMenuItem);
			seqMenu10 = new CCMenu (s10StdMenuItem);
			seqMenu11 = new CCMenu (s11StdMenuItem);
			seqMenu12 = new CCMenu (s12StdMenuItem);

			var seqMenus = new List<CCMenu>();
			seqMenus.Add (seqMenu01); 
			seqMenus.Add (seqMenu02); 
			seqMenus.Add (seqMenu03); 
			seqMenus.Add (seqMenu04); 
			seqMenus.Add (seqMenu05); 
			seqMenus.Add (seqMenu06); 
			seqMenus.Add (seqMenu07); 
			seqMenus.Add (seqMenu08); 
			seqMenus.Add (seqMenu09); 
			seqMenus.Add (seqMenu10); 
			seqMenus.Add (seqMenu11); 
			seqMenus.Add (seqMenu12); 

			foreach (var seq in availableSequences) {
				s01StdMenuItem.Visible |= seq == Sequences.Linear; 
				s02StdMenuItem.Visible |= seq == Sequences.Even; 
				s03StdMenuItem.Visible |= seq == Sequences.Odd; 
				s04StdMenuItem.Visible |= seq == Sequences.Triangular; 
				s05StdMenuItem.Visible |= seq == Sequences.Square; 
				s06StdMenuItem.Visible |= seq == Sequences.Lazy; 
				s07StdMenuItem.Visible |= seq == Sequences.Fibonacci; 
				s08StdMenuItem.Visible |= seq == Sequences.Prime; 
				s09StdMenuItem.Visible |= seq == Sequences.Double; 
				s10StdMenuItem.Visible |= seq == Sequences.Triple; 
				s11StdMenuItem.Visible |= seq == Sequences.Pi; 
				s12StdMenuItem.Visible |= seq == Sequences.Recaman;
				s12StdMenuItem.Visible |= seq == Sequences.Recaman;
			}

			for (int i = 0; i < seqMenus.Count; i++) {
				seqMenus [i].AlignItemsVertically (0);
				seqMenus [i].AnchorPoint = CCPoint.AnchorUpperLeft;
				seqMenus [i].IgnoreAnchorPointForPosition = false;
				seqMenus [i].PositionX = 40;
				seqMenus [i].PositionY = 1850 - (i * 150);
				if (activeLevel.UnlockSequenceUI) AddChild (seqMenus[i]);
			}

			checkpointStdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("sprite_std_checkpoint.png")));
			checkpointSelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("sprite_sel_checkpoint.png")));
			additionStdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("sprite_std_addition.png")));
			additionSelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("sprite_sel_addition.png")));
			subtractionStdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("sprite_std_subtraction.png")));
			subtractionSelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("sprite_sel_subtraction.png")));
			nextInSequenceStdSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("sprite_std_nextinseq.png")));
			nextInSequenceSelSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("sprite_sel_nextinseq.png")));

			checkpointsCountLabel = new CCLabel (availableCheckpoints.ToString (), GOTHIC_30_HD_FNT) {
				AnchorPoint = CCPoint.AnchorLowerLeft,
				Scale = 1f,
			};
			var checkpointsLabelMenuItem = new CCMenuItemLabel (checkpointsCountLabel);
			checkpointsLabelMenuItem.AnchorPoint = CCPoint.AnchorUpperLeft;

			additionsCountLabel = new CCLabel (availableAdditions.ToString (), GOTHIC_30_HD_FNT) {
				AnchorPoint = CCPoint.AnchorLowerLeft,
				Scale = 1f
			};
			var additionsLabelMenuItem = new CCMenuItemLabel (additionsCountLabel);
			additionsLabelMenuItem.AnchorPoint = CCPoint.AnchorUpperLeft;

			subtractionsCountLabel = new CCLabel (availableSubtractions.ToString (), GOTHIC_30_HD_FNT) {
				AnchorPoint = CCPoint.AnchorLowerLeft,
				Scale = 1f
			};
			var subtractionsLabelMenuItem = new CCMenuItemLabel (subtractionsCountLabel);
			subtractionsLabelMenuItem.AnchorPoint = CCPoint.AnchorUpperLeft;

			nextInSequencesCountLabel = new CCLabel (availableNextInSeq.ToString (), GOTHIC_30_HD_FNT) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 1f
			};
			var nextInSequencesLabelMenuItem = new CCMenuItemLabel (nextInSequencesCountLabel);
			nextInSequencesLabelMenuItem.AnchorPoint = CCPoint.AnchorUpperLeft;


			var menuItemCheckpoint = new CCMenuItemImage (checkpointStdSprite, checkpointSelSprite, ConsumableMenuCallback) {
				UserData = "checkpoint",
			};
			var menuItemAddition = new CCMenuItemImage (additionStdSprite, additionSelSprite, ConsumableMenuCallback) {
				UserData = "addition",
			};
			var menuItemSubtraction = new CCMenuItemImage (subtractionStdSprite, subtractionSelSprite, ConsumableMenuCallback) {
				UserData = "subtraction",
			};
			var menuItemNextInSeq = new CCMenuItemImage (nextInSequenceStdSprite, nextInSequenceSelSprite, ConsumableMenuCallback) {
				UserData = "next_in_seq",
			};

			var consumableMenuItems = new List<CCMenuItemImage> ();
			consumableMenuItems.Add (menuItemCheckpoint);
			consumableMenuItems.Add (menuItemAddition);
			consumableMenuItems.Add (menuItemSubtraction);
			consumableMenuItems.Add (menuItemNextInSeq);

			menuCheckpoint = new CCMenu (menuItemCheckpoint, checkpointsLabelMenuItem);
			menuAddition = new CCMenu (menuItemAddition, additionsLabelMenuItem);
			menuSubtraction = new CCMenu (menuItemSubtraction, subtractionsLabelMenuItem);
			menuNextInSeq = new CCMenu (menuItemNextInSeq, nextInSequencesLabelMenuItem);

			var consumableMenus = new List<CCMenu>();
			consumableMenus.Add (menuCheckpoint);
			consumableMenus.Add (menuAddition);
			consumableMenus.Add (menuSubtraction);
			consumableMenus.Add (menuNextInSeq);

			for (int i = 0; i < consumableMenus.Count; i++) {
				consumableMenus [i].AlignItemsHorizontally (-10);
				consumableMenus [i].AnchorPoint = CCPoint.AnchorLowerLeft;
				consumableMenus [i].IgnoreAnchorPointForPosition = false;
				consumableMenus [i].PositionX = 160 + (i * 200);
				consumableMenus [i].PositionY = 50;
				if (activeLevel.UnlockConsumableUI) AddChild (consumableMenus[i]);
			}

			additionsNumbersLabel = new CCLabel (string.Empty, GOTHIC_30_HD_FNT);
			additionsNumbersLabel.AnchorPoint = CCPoint.AnchorMiddleBottom;
			additionsNumbersLabel.Scale = 1.0f;
			additionsNumbersLabel.PositionX = menuItemAddition.PositionWorldspace.X;
			additionsNumbersLabel.PositionY = menuItemAddition.PositionWorldspace.Y + menuItemAddition.ContentSize.Center.Y;
			if (activeLevel.UnlockConsumableUI) AddChild (additionsNumbersLabel);

			subtractionsNumbersLabel = new CCLabel (string.Empty, GOTHIC_30_HD_FNT);
			subtractionsNumbersLabel.AnchorPoint = CCPoint.AnchorMiddleBottom;
			subtractionsNumbersLabel.Scale = 1.0f;
			subtractionsNumbersLabel.PositionX = menuItemSubtraction.PositionWorldspace.X;
			subtractionsNumbersLabel.PositionY = menuItemSubtraction.PositionWorldspace.Y + menuItemSubtraction.ContentSize.Center.Y;
			if (activeLevel.UnlockConsumableUI) AddChild (subtractionsNumbersLabel);

			sequenceTotalLabel = new CCLabel ("", "arial", 30);  // TODO: change the font to one of the sprite fonts
			sequenceTotalLabel.Color = CCColor3B.Yellow;
			sequenceTotalLabel.Text = sequenceTotal.ToString ();
			sequenceTotalLabel.Scale = 1f;
			sequenceTotalLabel.Position = GetSequenceTotalPosition (currentSequence);
			sequenceTotalLabel.AnchorPoint = CCPoint.AnchorMiddle;
			if (activeLevel.UnlockSequenceUI) AddChild (sequenceTotalLabel);

			nextInSequenceLabel = new CCLabel ("", "arial", 30); 
			nextInSequenceLabel.Text = (nextNumberInSequence == 0) ? "" : nextNumberInSequence.ToString ();
			nextInSequenceLabel.Color = CCColor3B.Green;
			nextInSequenceLabel.Scale = 1f;
			nextInSequenceLabel.Position = GetNextInSequencePosition (currentSequence);
			nextInSequenceLabel.AnchorPoint = CCPoint.AnchorMiddle;
			if (activeLevel.UnlockSequenceUI) AddChild (nextInSequenceLabel);
		}

		//---------------------------------------------------------------------------------------------------------
		// UpdateLabels
		//---------------------------------------------------------------------------------------------------------
		// Update various UI Labels
		//---------------------------------------------------------------------------------------------------------
		void UpdateLabels ()
		{
			sequenceTotalLabel.Text = sequenceTotal.ToString ();
			nextInSequenceLabel.Text = (nextNumberInSequence == 0) ? "" : nextNumberInSequence.ToString ();
			tapStrength = levelTapStrength + playerTapStrength;
			tapStrengthLabel.Text = tapStrength.ToString ();
		}

		//---------------------------------------------------------------------------------------------------------
		// UpdateConsumablesDisplay
		//---------------------------------------------------------------------------------------------------------
		// Updates the labels depicting the consumables count
		//---------------------------------------------------------------------------------------------------------
		void UpdateConsumablesDisplay()
		{
			checkpointsCountLabel.Text = availableCheckpoints.ToString ();
			additionsCountLabel.Text = availableAdditions.ToString ();
			subtractionsCountLabel.Text = availableSubtractions.ToString ();
			nextInSequencesCountLabel.Text = availableNextInSeq.ToString ();
		}

		//---------------------------------------------------------------------------------------------------------
		// OnTouchesEnded
		//---------------------------------------------------------------------------------------------------------
		void OnTouchesEnded (IList<CCTouch> touches, CCEvent touchEvent)
		{
			bool bubblePopped = false;

			// check if a bubble has been tapped on
			if (touches.Count > 0) {
				var locationOnScreen = touches [0].Location;
				tapLocation = locationOnScreen;

				foreach (var childBubble in bubbles.Children.OfType<PointBubble> ()) { 
					var doesTouchOverlapBubble = childBubble.BubbleSprite.BoundingBoxTransformedToWorld.ContainsPoint (tapLocation);
					if (doesTouchOverlapBubble) {
						childBubble.TapCount += tapStrength;
						if (childBubble.CheckPopped ()) {
							bubbleOccupiedArray [childBubble.ListIndex] = false;
							switch (pointState) {
							case PointState.None:
								{
									int luckyTotal = CheckForLuckyPoints (childBubble.PointValue);
									if (luckyTotal > 0) {
										var labelText = string.Format ("+{0}\nLucky!", luckyTotal);
										ExpandLabel (labelText, childBubble.PositionWorldspace );
									}
									regularTotal += childBubble.PointValue * bonusDoubleMultiplier * bonusTripleMultiplier;
									levelScore = regularTotal + sequenceTotal + luckyTotal;
									lastEarnedPoint = childBubble.PointValue;
									MovePointLabel (childBubble, scoreBar);
									var seqBonusAmount = CheckForSequenceBonus (childBubble.PointValue, nextNumberInSequence, currentSequence);
									if (seqBonusAmount != 0)
										sequenceTotal += seqBonusAmount;
									break;
								}
							case PointState.Addition:
								{
									if (bubble1Value == -1) {
										bubble1Value = childBubble.PointValue;
										additionsNumbersLabel.Text = string.Format ("{0} + ?", bubble1Value);
									} else if (bubble2Value == -1) {
										bubble2Value = childBubble.PointValue;
										var combined = CombineBubbleValues (pointState, bubble1Value, bubble2Value);
										additionsNumbersLabel.Text = combined.ToString ();
										regularTotal += combined * bonusDoubleMultiplier * bonusTripleMultiplier;
										levelScore = regularTotal + sequenceTotal;
										lastEarnedPoint = combined;
										var seqBonusAmount = CheckForSequenceBonus (combined, nextNumberInSequence, currentSequence);
										if (seqBonusAmount != 0)
											sequenceTotal += seqBonusAmount;
										MovePointLabel (additionsNumbersLabel,scoreBar);
										bubble1Value = bubble2Value = -1;
										availableAdditions--;
										pointState = PointState.None;
										additionsNumbersLabel.Text = string.Empty;
										additionsCountLabel.Color = CCColor3B.White;
										additionsCountLabel.Scale = 1.0f;
									}
									break;
								}
							case PointState.Subtraction:
								{
									if (bubble1Value == -1) {
										bubble1Value = childBubble.PointValue;
										subtractionsNumbersLabel.Text = string.Format ("{0} - ?", bubble1Value);
									} else if (bubble2Value == -1) {
										bubble2Value = childBubble.PointValue;
										var combined = CombineBubbleValues (pointState, bubble1Value, bubble2Value);
										subtractionsNumbersLabel.Text = combined.ToString ();
										regularTotal += combined * bonusDoubleMultiplier * bonusTripleMultiplier;
										levelScore = regularTotal + sequenceTotal;
										lastEarnedPoint = combined;
										var seqBonusAmount = CheckForSequenceBonus (combined, nextNumberInSequence, currentSequence);
										if (seqBonusAmount != 0)
											sequenceTotal += seqBonusAmount;
										MovePointLabel (subtractionsNumbersLabel,scoreBar);
										bubble1Value = bubble2Value = -1;
										availableSubtractions--;
										pointState = PointState.None;
										subtractionsNumbersLabel.Text = string.Empty;
										subtractionsCountLabel.Color = CCColor3B.White;
										subtractionsCountLabel.Scale = 1.0f;
									}
									break;
								}
							}
							PopBubble (childBubble);
							UpdateLabels ();
							UpdateConsumablesDisplay ();
							bubblePopped = true;
						}
					}
				}

				foreach (var childBubble in bubbles.Children.OfType<BonusBubble> ()) {
					bool doesTouchOverlapBubble = childBubble.BubbleSprite.BoundingBoxTransformedToWorld.ContainsPoint (tapLocation);
					if (doesTouchOverlapBubble) {
						childBubble.TapCount += tapStrength;
						if (childBubble.CheckPopped ()) {
							ProcessBonusBubble (childBubble);
							bubbleOccupiedArray [childBubble.ListIndex] = false;
							PopBubble (childBubble);
							bubblePopped = true;
						}
					}
				}
			}

			// if we found a popped bubble create a new one
			if (bubblePopped) {

				// cycle through BubbleOccupiedArray and make a shuffle bag of empty indicies
				// randomly select one of the unoccupied indicies
				// add a new bubble to that location
				var newBubbleOrder = new ShuffleBag<int> ();

				for (int i = 0; i < bubbleOccupiedArray.Length; i++) {
					if (!bubbleOccupiedArray [i])
						newBubbleOrder.Add (i);
				}

				var newBubbleIndex = newBubbleOrder.Next ();
				if (BubbleTypeToCreate (chanceToRollBonus) == BubbleType.Point) {
					CreatePointBubble (newBubbleIndex);
				} else {
					CreateBonusBubble (newBubbleIndex);
				}
				bubbleOccupiedArray [newBubbleIndex] = true;
				maxLevelPoints = (lastEarnedPoint >= maxLevelPoints) ? lastEarnedPoint + 1 : maxLevelPoints;  // TODO: this is what we need to change to gradually increase the max level's points.  Can possibly grow too quick
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// StartScheduling
		//---------------------------------------------------------------------------------------------------------
		void StartScheduling()
		{
			Schedule (t => {

				if (ShouldLevelEnd ()){
					if (levelScore >= levelPassScore)
					{
						didPassLevel = true;
						activePlayer.BranchProgression[0].LastLevelCompleted++; 
						if (activeLevel.LevelNum == 2) {
							var coinsEarnedFromPoints = ConvertScoreToCoins (levelScore - levelPassScore);
							var coinsEarnedFromSequence = sequenceTotal;
							coinsEarned = (int)Math.Truncate (coinsEarnedFromPoints) + coinsEarnedFromSequence;
							activePlayer.Coins += coinsEarned;
							activePlayer.CoinCarryOver = coinsEarnedFromPoints - (float)Math.Truncate (coinsEarnedFromPoints);
						}
						activePlayer.ChanceToRollBonus = playerChanceToRollBonus;
						activePlayer.ChanceToRollNextSeq = playerChanceToRollNextSeq;
						activePlayer.LuckyPointChance = chanceForLuckyPoints;
						activePlayer.LuckyPointBonus = maxLuckyPointAmount;
						activePlayer.ConsumableAdd = availableAdditions;
						activePlayer.ConsumableSubtract = availableSubtractions;
						activePlayer.ConsumableCheckPoint = availableCheckpoints;
						activePlayer.ConsumableNextSeq = availableNextInSeq;
						if (activePlayer.BranchProgression[0].LastLevelCompleted == 2) activePlayer.BranchProgression[0].BranchState = CompletionState.completed;
						activePlayer.WriteData (activePlayer);  // after successfully passing the level, save the player's progress
						EndLevel();
					} 
					else
					{
						if (activePlayer.IsMedKitActivated) {
							var randomInt = CCRandom.GetRandomInt (1, (activePlayer.MedKitUpgradeLevel + 3));
							levelTimeLeft += (float)randomInt;
							var labelText = string.Format ("+{0} sec", randomInt);
							ExpandLabel (labelText, medKitSprite.PositionWorldspace, VisibleBoundsWorldspace.Center);
							medKitSprite.RemoveFromParent ();
							activePlayer.IsMedKitActivated = false;
							activePlayer.WriteData (activePlayer);
						} else {
//							coinsEarned = (int)Math.Truncate ((double)sequenceTotal / 2);
//							activePlayer.Coins += coinsEarned;
							activePlayer.ConsumableAdd = availableAdditions;
							activePlayer.ConsumableSubtract = availableSubtractions;
							activePlayer.ConsumableCheckPoint = availableCheckpoints;
							activePlayer.ConsumableNextSeq = availableNextInSeq;
							activePlayer.WriteData (activePlayer);
							EndLevel();
						}
					}
					return;
				}

				levelTimeLeft -= t;
				levelTimeIncrement++;
				if (isDoubleScoreActive) {
					doubleScoreTimerDuration -= t;
				}
				if (isTripleScoreActive) {
					tripleScoreTimerDuration -= t;
				}
				if (levelTimeIncrement > 0) {
					maxLevelPoints = ((levelTimeIncrement % POINT_INCREMEMNT_DELAY) == 0) ? maxLevelPoints + 1 : maxLevelPoints;
				}

				if (levelTimeIncrement > 0) {
					if (levelVisibleBubbles < maxVisibleBubbles){
						levelVisibleBubbles = ((levelTimeIncrement % BUBBLES_VISIBLE_INCREMENT_DELAY) == 0) ? levelVisibleBubbles + 1 : levelVisibleBubbles;
					}
				}

			},0.1f);

			chanceToRollBonus = levelChanceToRollBonus + playerChanceToRollBonus;
			chanceToRollNextSeq = levelChanceToRollNextSeq + playerChanceToRollNextSeq;
			tapStrength = levelTapStrength + playerTapStrength;
			tapStrengthLabel.Text = tapStrength.ToString ();

			timeBar.Percentage = (levelTimeLeft / levelTimeLimit) * 100;
			scoreBar.Percentage = ((float)levelScore / (float)levelPassScore) * 100;
			if (levelScore > levelPassScore) {
				if (!isScoreBonusActive) {
					scoreSpriteProgressBarFull = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("prgbar_score_bonus.png")));
					scoreBar.Sprite = scoreSpriteProgressBarFull;
					isScoreBonusActive = true;
				}
			}
			// score bonus timers
			if (isDoubleScoreActive) { 
				doubleScoreTimer.Percentage = (doubleScoreTimerDuration / SCORE_MULTIPLIER_DURATION) * 100;
				if (doubleScoreTimerDuration < 0) {
					bonusDoubleMultiplier = 1;
					doubleScoreTimerDuration = 0;
					doubleScoreTimer.Percentage = 0;
					isDoubleScoreActive = false;
				}
			}

			if (isTripleScoreActive) { 
				tripleScoreTimer.Percentage = (tripleScoreTimerDuration / SCORE_MULTIPLIER_DURATION) * 100;
				if (tripleScoreTimerDuration < 0) {
					bonusTripleMultiplier = 1;
					tripleScoreTimerDuration = 0;
					tripleScoreTimer.Percentage = 0;
					isTripleScoreActive = false;
				}
			}
		}
	
		//---------------------------------------------------------------------------------------------------------
		// InGameMenu
		//---------------------------------------------------------------------------------------------------------
		// Creates an in-game menu that pauses gameplay and pops up to allow the user to do whatever
		//---------------------------------------------------------------------------------------------------------
		void InGameMenu (CCRect bounds)
		{
			var menuStd = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("gear_std.png")));
			menuStd.AnchorPoint = CCPoint.AnchorMiddle;
			var menuSel = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("gear_sel.png")));
			menuSel.AnchorPoint = CCPoint.AnchorMiddle;
			var optionPopup = new CCMenuItemImage (menuStd, menuSel, sender =>  {
				PauseListeners (true);
				Application.Paused = true;
				var menuLayer = new CCLayerColor (new CCColor4B (0, 0, 0, 200));
				AddChild (menuLayer, 99999);
				// Add frame to layer
				frameSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("frame.png")));
				frameSprite.AnchorPoint = CCPoint.AnchorMiddle;
				frameSprite.Position = new CCPoint (bounds.Size.Width / 2, bounds.Size.Height / 2);
				menuLayer.AddChild (frameSprite);

				var highlightOnLabel = new CCLabel ("Highlight Next: On", GOTHIC_56_WHITE_FNT) {
					AnchorPoint = CCPoint.AnchorMiddle,
					Scale = 2.0f
				};
				var highlightOffLabel = new CCLabel ("Highlight Next: Off", GOTHIC_56_WHITE_FNT) {
					AnchorPoint = CCPoint.AnchorMiddle,
					Scale = 2.0f
				};

				var highlightOnMenuItem = new CCMenuItemLabel (highlightOnLabel);  
				var highlightOffMenuItem = new CCMenuItemLabel (highlightOffLabel);

				highlightToggleMenuItem = new CCMenuItemToggle (ToggleHighlight, highlightOnMenuItem, highlightOffMenuItem);
				highlightToggleMenuItem.Enabled = activePlayer.HighlightNextPurchased;
				highlightToggleMenuItem.PositionX = frameSprite.BoundingBox.MidX;
				highlightToggleMenuItem.PositionY = frameSprite.BoundingBox.MinY + (highlightToggleMenuItem.BoundingBox.Size.Height * 10f);
				highlightToggleMenuItem.SelectedIndex = (playerHighlightNext) ? 0 : 1;

				var optionMenu = new CCMenu (highlightToggleMenuItem);
				optionMenu.AnchorPoint = CCPoint.AnchorMiddleBottom;
				optionMenu.Position = CCPoint.Zero;
				menuLayer.AddChild (optionMenu);

				okLabel = new CCLabel ("OK", GOTHIC_44_HD_FNT);
				okLabel.AnchorPoint = CCPoint.AnchorMiddle;
				okLabel.Scale = 2.0f;

				var closeItem = new CCMenuItemLabel (okLabel, closeSender => {
					menuLayer.RemoveFromParent ();
					ResumeListeners (true);
					Application.Paused = false;
				});

				closeItem.PositionX = frameSprite.BoundingBox.MidX;
				closeItem.PositionY = frameSprite.BoundingBox.MinY + (closeItem.BoundingBox.Size.Height * 1.5f);

				var closeMenu = new CCMenu (closeItem);
				closeMenu.AnchorPoint = CCPoint.AnchorMiddleBottom;
				closeMenu.Position = CCPoint.Zero;

				menuLayer.AddChild (closeMenu);
			});
			optionPopup.AnchorPoint = CCPoint.AnchorUpperRight;
			//optionPopup.Position = new CCPoint(bounds.Size.Width / 10, bounds.Size.Height / 14);
			var optionsMenu = new CCMenu (optionPopup);
			optionsMenu.AnchorPoint = CCPoint.AnchorUpperRight;
			optionsMenu.Position = new CCPoint (1080, 1920);
			AddChild (optionsMenu);
		}

		//---------------------------------------------------------------------------------------------------------
		// ToggleHighlight
		//---------------------------------------------------------------------------------------------------------
		// Used to toggle the "Highlight Next Bubble In Sequence" option to "On" if they have purchased it
		//---------------------------------------------------------------------------------------------------------
		void ToggleHighlight (object obj)
		{
			playerHighlightNext = !playerHighlightNext;
		}

		//---------------------------------------------------------------------------------------------------------
		// ConsumableMenuCallback
		//---------------------------------------------------------------------------------------------------------
		// Callback when consumable UI icon is touched
		//---------------------------------------------------------------------------------------------------------
		void ConsumableMenuCallback (object pSender)
		{
			var sentMenuItem = (CCMenuItemImage)pSender;
			var consumableTypeString = sentMenuItem.UserData.ToString ();

			switch (consumableTypeString) {
			case "checkpoint":
				{
					if (availableCheckpoints > 0) {
						if (checkpointDepth != seqDepth) { 
							availableCheckpoints--;
							isCheckpointActive = true;
							checkpointSequence = nextNumberInSequence;
							checkpointDepth = seqDepth;
							ExpandLabel ("Checkpoint!", menuCheckpoint.PositionWorldspace);
							RemoveChild (checkmarkSprite);
							checkmarkSprite.Position = GetCheckmarkPosition (currentSequence);
							AddChild (checkmarkSprite);
						}
					}  
					break;
				}
			case "addition":
				{
					if ((availableAdditions > 0) && (pointState != PointState.Addition)) {
						additionsCountLabel.Color = CCColor3B.Orange;
						subtractionsCountLabel.Color = CCColor3B.White;
						additionsCountLabel.Scale = 1.2f;
						subtractionsCountLabel.Scale = 1.0f;
						pointState = PointState.Addition;
					} else if ((availableAdditions > 0) && (pointState == PointState.Addition)) {
						additionsCountLabel.Color = CCColor3B.White;
						additionsCountLabel.Scale = 1.0f;
						pointState = PointState.None;
					}
					break;
				}
			case "subtraction":
				{
					if ((availableSubtractions > 0) && (pointState != PointState.Subtraction)) {
						subtractionsCountLabel.Color = CCColor3B.Orange;
						additionsCountLabel.Color = CCColor3B.White;
						subtractionsCountLabel.Scale = 1.2f;
						additionsCountLabel.Scale = 1.0f;
						pointState = PointState.Subtraction;
					} else if ((availableSubtractions > 0) && (pointState == PointState.Subtraction)) {
						subtractionsCountLabel.Color = CCColor3B.White;
						subtractionsCountLabel.Scale = 1.0f;
						pointState = PointState.None;
					}
					break;
				}
			case "next_in_seq":
				{
					if (availableNextInSeq > 0) {
						regularTotal += nextNumberInSequence * bonusDoubleMultiplier * bonusTripleMultiplier;
						levelScore = regularTotal + sequenceTotal;
						lastEarnedPoint = nextNumberInSequence;
						MovePointLabel (menuNextInSeq, scoreBar, lastEarnedPoint);
						var seqBonusAmount = CheckForSequenceBonus (lastEarnedPoint, nextNumberInSequence, currentSequence);
						if (seqBonusAmount != 0)
							sequenceTotal += seqBonusAmount;
						availableNextInSeq--;
						UpdateLabels ();
					}
					break;
				}
			}

			UpdateConsumablesDisplay ();
		}

		//---------------------------------------------------------------------------------------------------------
		// SeqMenuCallBack
		//---------------------------------------------------------------------------------------------------------
		// Callback when sequence UI icon is touched
		//---------------------------------------------------------------------------------------------------------
		void SeqMenuCallback (object pSender)
		{
			var sentMenuItem = (CCMenuItemImage)pSender;
			var seqTypeString = sentMenuItem.UserData.ToString ();
			switch (seqTypeString) {
			case "Linear":
				{
					if (selectedSequence != Sequences.Linear) {
						currentSequence = Sequences.Linear;
						selectedSequence = Sequences.Linear;
						nextNumberInSequence = 1;
						nextInSequenceLabel.Text = nextNumberInSequence.ToString ();
						seqDepth = 0;
						isCheckpointActive = false;
						checkpointDepth = 0;
					}
					break;
				}
			case "Even":
				{
					if (selectedSequence != Sequences.Even) {
						currentSequence = Sequences.Even;
						selectedSequence = Sequences.Even;
						nextNumberInSequence = 2;
						nextInSequenceLabel.Text = nextNumberInSequence.ToString ();
						seqDepth = 0;
						isCheckpointActive = false;
						checkpointDepth = 0;
					}
					break;
				}
			case "Odd":
				{
					if (selectedSequence != Sequences.Odd) {
						currentSequence = Sequences.Odd;
						selectedSequence = Sequences.Odd;
						nextNumberInSequence = 1;
						nextInSequenceLabel.Text = nextNumberInSequence.ToString ();
						seqDepth = 0;
						isCheckpointActive = false;
						checkpointDepth = 0;
					}
					break;
				}
			case "Triangular":
				{
					if (selectedSequence != Sequences.Triangular) {
						currentSequence = Sequences.Triangular;
						selectedSequence = Sequences.Triangular;
						nextNumberInSequence = 1;
						nextInSequenceLabel.Text = nextNumberInSequence.ToString ();
						seqDepth = 0;
						isCheckpointActive = false;
						checkpointDepth = 0;
					}
					break;
				}
			case "Square":
				{
					if (selectedSequence != Sequences.Square) {
						currentSequence = Sequences.Square;
						selectedSequence = Sequences.Square;
						nextNumberInSequence = 1;
						nextInSequenceLabel.Text = nextNumberInSequence.ToString ();
						seqDepth = 0;
						isCheckpointActive = false;
						checkpointDepth = 0;
					}
					break;
				}
			case "Lazy":
				{
					if (selectedSequence != Sequences.Lazy) {
						currentSequence = Sequences.Lazy;
						selectedSequence = Sequences.Lazy;
						nextNumberInSequence = 1;
						nextInSequenceLabel.Text = nextNumberInSequence.ToString ();
						seqDepth = 0;
						isCheckpointActive = false;
						checkpointDepth = 0;
					}
					break;
				}
			case "Fibonacci":
				{
					if (selectedSequence != Sequences.Fibonacci) {
						currentSequence = Sequences.Fibonacci;
						selectedSequence = Sequences.Fibonacci;
						nextNumberInSequence = 1;
						nextInSequenceLabel.Text = nextNumberInSequence.ToString ();
						seqDepth = 0;
						isCheckpointActive = false;
						checkpointDepth = 0;
					}
					break;
				}
			case "Prime":
				{
					if (selectedSequence != Sequences.Prime) {
						currentSequence = Sequences.Prime;
						selectedSequence = Sequences.Prime;
						nextNumberInSequence = 2;
						nextInSequenceLabel.Text = nextNumberInSequence.ToString ();
						seqDepth = 0;
						isCheckpointActive = false;
						checkpointDepth = 0;
					}
					break;
				}
			case "Double":
				{
					if (selectedSequence != Sequences.Double) {
						currentSequence = Sequences.Double;
						selectedSequence = Sequences.Double;
						nextNumberInSequence = 1;
						nextInSequenceLabel.Text = nextNumberInSequence.ToString ();
						seqDepth = 0;
						isCheckpointActive = false;
						checkpointDepth = 0;
					}
					break;
				}
			case "Triple":
				{
					if (selectedSequence != Sequences.Triple) {
						currentSequence = Sequences.Triple;
						selectedSequence = Sequences.Triple;
						nextNumberInSequence = 1;
						nextInSequenceLabel.Text = nextNumberInSequence.ToString ();
						seqDepth = 0;
						isCheckpointActive = false;
						checkpointDepth = 0;
					}
					break;
				}
			case "Pi":
				{
					if (selectedSequence != Sequences.Pi) {
						currentSequence = Sequences.Pi;
						selectedSequence = Sequences.Pi;
						nextNumberInSequence = 3;
						nextInSequenceLabel.Text = nextNumberInSequence.ToString ();
						seqDepth = 0;
						isCheckpointActive = false;
						checkpointDepth = 0;
					}
					break;
				}
			case "Recaman":
				{
					if (selectedSequence != Sequences.Recaman) {
						currentSequence = Sequences.Recaman;
						selectedSequence = Sequences.Recaman;
						nextNumberInSequence = 1;
						nextInSequenceLabel.Text = nextNumberInSequence.ToString ();
						seqDepth = 0;
						isCheckpointActive = false;
						checkpointDepth = 0;
					}
					break;
				}
			}
			sequenceTotalLabel.Position = GetSequenceTotalPosition (currentSequence); 
			nextInSequenceLabel.Position = GetNextInSequencePosition (currentSequence);

			if (!isCheckpointActive)
				RemoveChild (checkmarkSprite);
		}

		//---------------------------------------------------------------------------------------------------------
		// CreateListOfSequences
		//---------------------------------------------------------------------------------------------------------
		// Creates a list of sequences based on what the level data xml file says should be available
		//---------------------------------------------------------------------------------------------------------
		public void CreateListOfSequences(Level thisLevel)
		{
			availableSequences = new List<Sequences>();

			if (thisLevel.SeqLinear)
				availableSequences.Add (Sequences.Linear);
			if (thisLevel.SeqEven)
				availableSequences.Add (Sequences.Even);
			if (thisLevel.SeqOdd)
				availableSequences.Add (Sequences.Odd);
			if (thisLevel.SeqTriangular)
				availableSequences.Add (Sequences.Triangular);
			if (thisLevel.SeqSquare)
				availableSequences.Add (Sequences.Square);
			if (thisLevel.SeqLazy)
				availableSequences.Add (Sequences.Lazy);
			if (thisLevel.SeqFibonacci)
				availableSequences.Add (Sequences.Fibonacci);
			if (thisLevel.SeqPrime)
				availableSequences.Add (Sequences.Prime);
			if (thisLevel.SeqDouble)
				availableSequences.Add (Sequences.Double);
			if (thisLevel.SeqTriple)
				availableSequences.Add (Sequences.Triple);
			if (thisLevel.SeqPi)
				availableSequences.Add (Sequences.Pi);
			if (thisLevel.SeqRecaman)
				availableSequences.Add (Sequences.Recaman);
		}

		//---------------------------------------------------------------------------------------------------------
		// GetSequenceMenu
		//---------------------------------------------------------------------------------------------------------
		// Takes a sequence enum and returns the sequence menu
		//---------------------------------------------------------------------------------------------------------
		CCMenu GetSequenceMenu (Sequences theCurrentSequence)
		{
			var menuToReturn = new CCMenu ();

			switch (theCurrentSequence) {
			case Sequences.Linear:
				{
					menuToReturn = seqMenu01;
					break;
				}
			case Sequences.Even:
				{
					menuToReturn = seqMenu02;
					break;
				}
			case Sequences.Odd:
				{
					menuToReturn = seqMenu03;
					break;
				}
			case Sequences.Triangular:
				{
					menuToReturn = seqMenu04;
					break;
				}
			case Sequences.Square:
				{
					menuToReturn = seqMenu05;
					break;
				}
			case Sequences.Lazy:
				{
					menuToReturn = seqMenu06;
					break;
				}
			case Sequences.Fibonacci:
				{
					menuToReturn = seqMenu07;
					break;
				}
			case Sequences.Prime:
				{
					menuToReturn = seqMenu08;
					break;
				}
			case Sequences.Double:
				{
					menuToReturn = seqMenu09;
					break;
				}
			case Sequences.Triple:
				{
					menuToReturn = seqMenu10;
					break;
				}
			case Sequences.Pi:
				{
					menuToReturn = seqMenu11;
					break;
				}
			case Sequences.Recaman:
				{
					menuToReturn = seqMenu12;
					break;
				}
			}
			return menuToReturn;
		}

		//---------------------------------------------------------------------------------------------------------
		// CheckForSequenceBonus
		//---------------------------------------------------------------------------------------------------------
		// Checks the value of the bubble popped and if that value was the next number in the sequence, grants a 
		// bonus.
		//---------------------------------------------------------------------------------------------------------
		public int CheckForSequenceBonus (int pointValue, int nextNumberInSequence, Sequences currentSequence)
		{
			var pointsAwarded = 0;

			switch (currentSequence) {
			case Sequences.Linear:
				{
					if (pointValue == nextNumberInSequence) {
						if (pointValue == 1) {
							pointsAwarded = 0;
							seqDepth += 1;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, seqDepth);
						} else {
							pointsAwarded = nextNumberInSequence;
							seqDepth += 1;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, seqDepth);
						}
					} else {
						this.nextNumberInSequence = (isCheckpointActive) ? checkpointSequence :((pointValue == 1) ? 2 : 1); 
						seqDepth = (isCheckpointActive) ? checkpointDepth :((pointValue == 1) ? 1 : 0);
					}
					break;
				}
			case Sequences.Even:
				{
					if (pointValue == nextNumberInSequence) {
						if (pointValue == 2) {
							pointsAwarded = 0;
							seqDepth += 1;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, seqDepth);
						} else {
							pointsAwarded = nextNumberInSequence;
							seqDepth += 1;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, seqDepth);
						}
					} else {
						this.nextNumberInSequence = (isCheckpointActive) ? checkpointSequence :((pointValue == 2) ? 4 : 2); 
						seqDepth = (isCheckpointActive) ? checkpointDepth :((pointValue == 2) ? 1 : 0);
					}
					break;
				}
			case Sequences.Odd:
				{
					if (pointValue == nextNumberInSequence) {
						if (pointValue == 1) {
							pointsAwarded = 0;
							seqDepth += 1;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, seqDepth);
						} else {
							pointsAwarded = nextNumberInSequence;
							seqDepth += 1;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, seqDepth);
						}
					} else {
						this.nextNumberInSequence = (isCheckpointActive) ? checkpointSequence :((pointValue == 1) ? 3 : 1); 
						seqDepth = (isCheckpointActive) ? checkpointDepth :((pointValue == 1) ? 1 : 0);
					}
					break;
				}
			case Sequences.Triangular:
				{
					if (pointValue == nextNumberInSequence) {
						if (pointValue == 1) {
							pointsAwarded = 0;
							seqDepth += 1;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, seqDepth);
						} else {
							pointsAwarded = nextNumberInSequence;
							seqDepth += 1;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, seqDepth);
						}
					} else {
						this.nextNumberInSequence = (isCheckpointActive) ? checkpointSequence :((pointValue == 1) ? 3 : 1); 
						seqDepth = (isCheckpointActive) ? checkpointDepth :((pointValue == 1) ? 1 : 0);
					}
					break;
				}
			case Sequences.Square:
				{
					if (pointValue == nextNumberInSequence) {
						if (pointValue == 1) {
							pointsAwarded = 0;
							seqDepth += 1;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, seqDepth);
						} else {
							pointsAwarded = nextNumberInSequence;
							seqDepth += 1;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, seqDepth);
						}
					} else {
						this.nextNumberInSequence = (isCheckpointActive) ? checkpointSequence :((pointValue == 1) ? 4 : 1); 
						seqDepth = (isCheckpointActive) ? checkpointDepth :((pointValue == 1) ? 1 : 0);
					}
					break;
				}
			case Sequences.Lazy:
				{
					if (pointValue == nextNumberInSequence) {
						if (pointValue == 1) {
							pointsAwarded = 0;
							seqDepth += 1;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, seqDepth);
						} else {
							pointsAwarded = nextNumberInSequence;
							seqDepth += 1;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, seqDepth);
						}
					} else {
						this.nextNumberInSequence = (isCheckpointActive) ? checkpointSequence :((pointValue == 1) ? 2 : 1); 
						seqDepth = (isCheckpointActive) ? checkpointDepth :((pointValue == 1) ? 1 : 0);
					}
					break;
				}
			case Sequences.Fibonacci:
				{
					if (pointValue == nextNumberInSequence) {
						if (pointValue == 1 && seqDepth == 0) {
							pointsAwarded = 0;
							seqDepth += 1;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, seqDepth);
						} else if (pointValue == 1 && seqDepth == 1) {
							pointsAwarded = 1;
							seqDepth += 1;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, seqDepth);
						} else {
							pointsAwarded = nextNumberInSequence;
							seqDepth += 1;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, seqDepth);
						}
					} else {
						this.nextNumberInSequence = (isCheckpointActive) ? checkpointSequence : 1;
						seqDepth = (isCheckpointActive) ? checkpointDepth :((pointValue == 1) ? 1 : 0);
					}
					break;
				}
			case Sequences.Prime:
				{
					if (pointValue == nextNumberInSequence) {
						if (pointValue == 2) {
							pointsAwarded = 0;
							seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, seqDepth);
						} else {
							pointsAwarded = nextNumberInSequence;
							seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, seqDepth);
						}
					} else {
						this.nextNumberInSequence = (isCheckpointActive) ? checkpointSequence :((pointValue == 2) ? 3 : 2); 
						seqDepth = (isCheckpointActive) ? checkpointDepth :((pointValue == 2) ? 1 : 0);
					}
					break;
				}
			case Sequences.Double:
				{
					if (pointValue == nextNumberInSequence) {
						if (pointValue == 1) {
							pointsAwarded = 0;
							seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, seqDepth);
						} else {
							pointsAwarded = nextNumberInSequence;
							seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, seqDepth);
						}
					} else {
						this.nextNumberInSequence = (isCheckpointActive) ? checkpointSequence :((pointValue == 1) ? 2 : 1); 
						seqDepth = (isCheckpointActive) ? checkpointDepth :((pointValue == 1) ? 1 : 0);
					}
					break;
				}
			case Sequences.Triple:
				{
					if (pointValue == nextNumberInSequence) {
						if (pointValue == 1) {
							pointsAwarded = 0;
							seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, seqDepth);
						} else {
							pointsAwarded = nextNumberInSequence;
							seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, seqDepth);
						}
					} else {
						this.nextNumberInSequence = (isCheckpointActive) ? checkpointSequence :((pointValue == 1) ? 3 : 1); 
						seqDepth = (isCheckpointActive) ? checkpointDepth :((pointValue == 1) ? 1 : 0);
					}
					break;
				}
			case Sequences.Pi:
				{
					if (pointValue == nextNumberInSequence) {
						if (pointValue == 3 && seqDepth == 0) {
							pointsAwarded = 0;
							seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, seqDepth);
						} else {
							pointsAwarded = nextNumberInSequence;
							seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, seqDepth);
						}
					} else {
						this.nextNumberInSequence = (isCheckpointActive) ? checkpointSequence :((pointValue == 3) ? 1 : 3); 
						seqDepth = (isCheckpointActive) ? checkpointDepth :((pointValue == 3) ? 1 : 0);
					}
					break;
				}
			case Sequences.Recaman:
				{
					if (pointValue == nextNumberInSequence) {
						if (pointValue == 1 && seqDepth == 0) {
							pointsAwarded = 0;
							seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, seqDepth);
						} else {
							pointsAwarded = nextNumberInSequence;
							seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, seqDepth);
						}
					} else {
						this.nextNumberInSequence = (isCheckpointActive) ? checkpointSequence :((pointValue == 1) ? 3 : 1); 
						seqDepth = (isCheckpointActive) ? checkpointDepth :((pointValue == 1) ? 1 : 0);
					}
					break;
				}
			}

			return pointsAwarded;
		}

		//---------------------------------------------------------------------------------------------------------
		// GetNextSequenceNumber
		//---------------------------------------------------------------------------------------------------------
		// Is given the name of the current sequence and the number that was just reached and returns the next 
		// number in the sequence.  Also takes how deep we are in a sequence since some calcs will require that
		//---------------------------------------------------------------------------------------------------------
		public int GetNextSequenceNumber (Sequences currentSequence, int currentNumber, int sequenceDepth)
		{
			int nextNumber = 1;	// if for some reason we fail to find a case, we will return 1, which will start us at the beginning of the sequence

			switch (currentSequence) {
			case Sequences.Linear:
				{
					nextNumber = currentNumber + 1;
					break;
				}
			case Sequences.Even:
			case Sequences.Odd:
				{
					nextNumber = currentNumber + 2;
					break;
				}
			case Sequences.Triangular:
				{
					// we need to add 1 to sequence depth because the depth at this point is the current depth
					// and we need to calculate the next depth
					nextNumber = (sequenceDepth + 1) * (sequenceDepth + 2) / 2;
					break;
				}
			case Sequences.Square:
				{
					nextNumber = (sequenceDepth + 1) * (sequenceDepth + 1);
					break;
				}
			case Sequences.Lazy:
				{
					nextNumber = ((sequenceDepth * sequenceDepth) + sequenceDepth + 2 ) / 2;
					break;
				}
			case Sequences.Fibonacci:
				{
					double sqrt5 = Math.Sqrt (5);
					double p1 = (1 + sqrt5) / 2;
					double p2 = -1 * (p1 - 1);
					double n1 = Math.Pow (p1, sequenceDepth + 1);
					double n2 = Math.Pow (p2, sequenceDepth + 1);
					nextNumber = (int)((n1 - n2) / sqrt5);
					break;
				}
			case Sequences.Prime:
				{
					nextNumber = listOfPrimes [sequenceDepth];
					break;
				}
			case Sequences.Double:
				{
					nextNumber = currentNumber * 2;
					break;
				}
			case Sequences.Triple:
				{
					nextNumber = currentNumber * 3;
					break;
				}
			case Sequences.Pi:
				{
					nextNumber = (int)Char.GetNumericValue (PI_DIGITS[sequenceDepth]);
					if (nextNumber == 0) {
						nextNumber = 10;
					}
					break;
				}
			case Sequences.Recaman:
				{
					nextNumber = listOfRecamans [sequenceDepth];
					break;
				}
			}

			return nextNumber;
		}

		//---------------------------------------------------------------------------------------------------------
		// GetSequenceTotalPosition
		//---------------------------------------------------------------------------------------------------------
		// Find the correct position for SequenceTotal based on selected sequence (should be below)
		//---------------------------------------------------------------------------------------------------------
		CCPoint GetSequenceTotalPosition (Sequences theCurrentSequence)
		{
			var newPoint = new CCPoint ();
			CCMenu seqMenu;

			seqMenu = GetSequenceMenu (theCurrentSequence);
			newPoint.X = seqMenu.PositionWorldspace.X;
			newPoint.Y = seqMenu.PositionWorldspace.Y - ((seqMenu.ContentSize.Height * 1.5f)+5); 

			return newPoint;
		}

		//---------------------------------------------------------------------------------------------------------
		// GetNextInSequencePosition
		//---------------------------------------------------------------------------------------------------------
		// Find correct position for NextInSeqLabel.  Should be on top
		//---------------------------------------------------------------------------------------------------------
		CCPoint GetNextInSequencePosition (Sequences theCurrentSequence)
		{
			var newPoint = new CCPoint ();
			CCMenu seqMenu;

			seqMenu = GetSequenceMenu (theCurrentSequence);
			newPoint.X = seqMenu.PositionWorldspace.X;
			newPoint.Y = seqMenu.PositionWorldspace.Y - ((seqMenu.ContentSize.Height / 2)-5); 

			return newPoint;
		}

		//---------------------------------------------------------------------------------------------------------
		// GetCheckmarkPosition
		//---------------------------------------------------------------------------------------------------------
		// Find the correct position for the green checkmark to indicate a sequence checkpoint
		//---------------------------------------------------------------------------------------------------------
		CCPoint GetCheckmarkPosition (Sequences theCurrentSequence)
		{
			var newPoint = new CCPoint ();
			CCMenu seqMenu;

			seqMenu = GetSequenceMenu (theCurrentSequence);
			newPoint.X = seqMenu.PositionWorldspace.X + (seqMenu.ContentSize.Width / 3);
			newPoint.Y = seqMenu.PositionWorldspace.Y - (seqMenu.ContentSize.Height / 0.8f); 

			return newPoint;
		}

		//---------------------------------------------------------------------------------------------------------
		// BubbleTypeToCreate
		//---------------------------------------------------------------------------------------------------------
		// Determines if we create a point bubble or a bonus bubble
		//---------------------------------------------------------------------------------------------------------
		public BubbleType BubbleTypeToCreate (int bonusModifyer)
		{
			BubbleType createThisTypeOfBubble = BubbleType.Point;
			var randomPick = CCRandom.GetRandomInt (1, 5000); // make this higher to decrease frequency of bonus bubbles

			if (randomPick <= bonusModifyer) {
				createThisTypeOfBubble = BubbleType.Bonus;
			}

			return createThisTypeOfBubble;
		}

		//---------------------------------------------------------------------------------------------------------
		// CheckForLuckyPoints
		//---------------------------------------------------------------------------------------------------------
		// Using the bubble point value as a seed, checks if the pop was a lucky pop, granting additional points
		//---------------------------------------------------------------------------------------------------------
		public int CheckForLuckyPoints (int pointValue)
		{
			int pointsToReturn;

			var randomNumber = CCRandom.GetRandomInt (1, 5000); // make this higher to decrease frequency of lucky points

			pointsToReturn = (randomNumber <= chanceForLuckyPoints) ? CCRandom.GetRandomInt (pointValue, pointValue + maxLuckyPointAmount) : 0;

			return pointsToReturn;
		}
			
		//---------------------------------------------------------------------------------------------------------
		// CreatePointBubble
		//---------------------------------------------------------------------------------------------------------
		// Creates a single point bubble and adds it to the node parent
		//---------------------------------------------------------------------------------------------------------

		void CreatePointBubble (int bubbleIndex, bool initialCreation = false)
		{
			PointBubble pointBubble;
			pointBubble = initialCreation ? new PointBubble ((bubbleIndex % MAX_BUBBLES_X), (bubbleIndex / MAX_BUBBLES_X), bubbleIndex, GetRandomScoreValue (scoreRandom, maxLevelPoints), tapsRequired) 
				: new PointBubble ((bubbleIndex % MAX_BUBBLES_X), (bubbleIndex / MAX_BUBBLES_X), bubbleIndex, GetRandomScoreValue (scoreRandom, maxLevelPoints, nextNumberInSequence, chanceToRollNextSeq), tapsRequired);
			CCPoint p = GetRandomPosition (pointBubble.XIndex, pointBubble.YIndex);
			pointBubble.Position = new CCPoint (p.X, p.Y);
			pointBubble.Scale = BUBBLE_SCALE;
			DisplayBubble (pointBubble);
			DisplayLabel (pointBubble);
			DisplayHighlight (pointBubble);
			bubbles.AddChild (pointBubble);
		}

		//---------------------------------------------------------------------------------------------------------
		// CreateBonusBubble
		//---------------------------------------------------------------------------------------------------------
		// Creates a single bonus bubble and adds it to the node parent
		//---------------------------------------------------------------------------------------------------------

		void CreateBonusBubble (int bubbleIndex)
		{
			var bonusBubble = new BonusBubble ((bubbleIndex % MAX_BUBBLES_X), (bubbleIndex / MAX_BUBBLES_X), bubbleIndex, tapsRequired, activeLevel);
			CCPoint p = GetRandomPosition (bonusBubble.XIndex, bonusBubble.YIndex);
			bonusBubble.Position = new CCPoint (p.X, p.Y);
			bonusBubble.Scale = BUBBLE_SCALE;
			DisplayBubble (bonusBubble);
			DisplayBonus (bonusBubble); 
			bubbles.AddChild (bonusBubble);
		}

		//---------------------------------------------------------------------------------------------------------
		// CreateBubbleArray
		//---------------------------------------------------------------------------------------------------------
		// Creates the initial array of bubbles for the level
		//---------------------------------------------------------------------------------------------------------
		void CreateBubbleArray ()
		{
			bubbles = new CCNode ();
			AddChild (bubbles);
			//initialize every bool in BubbleArray to false.  True if there is a bubble there.
			for (int i = 0; i < MAX_BUBBLES_X; i++) {
				bubbleOccupiedArray [i] = false;
			}
			//---------------------------------------------------------------------------------------------------------
			// Create shuffle bags
			//---------------------------------------------------------------------------------------------------------
			// Create a shuffle bag.  Add random integer to bag, one for every possible location on the screen.  Use
			// the integer for the item in the bag to generate x,y indicies in the bubble occupied array.  Assign the 
			// indicies to the bubbles.  These will be used to generate initial points on the screen for each bubble.
			var bubbleOrder = new ShuffleBag<int> ();
			for (int i = 0; i < maxBubbles; i++) {
				bubbleOrder.Add (i);
			}
			for (int i = 0; i < levelVisibleBubbles; i++) {
				var bubbleIndex = bubbleOrder.Next ();
				if (BubbleTypeToCreate (chanceToRollBonus) == BubbleType.Point) {
					CreatePointBubble (bubbleIndex, true);
				} else {
					CreateBonusBubble (bubbleIndex);
				}
				bubbleOccupiedArray [bubbleIndex] = true;
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// CheckForFadedBubbles
		//---------------------------------------------------------------------------------------------------------
		// We check to see if the number of child nodes that are bubbles is less than the number of visible
		// bubbles allowed.  If so a bubble has either faded or the number allowed has increased.  Either way we 
		// need to add a new bubble.
		//---------------------------------------------------------------------------------------------------------
		void CheckForFadedBubbles()
		{
			//check if a bubble has popped on its own
			if (bubbles.ChildrenCount < levelVisibleBubbles) {
				var newBubbleOrder = new ShuffleBag<int> ();

				for (int i = 0; i < bubbleOccupiedArray.Length; i++) {
					if (!bubbleOccupiedArray [i])
						newBubbleOrder.Add (i);
				}

				var newBubbleIndex = newBubbleOrder.Next ();
				if (BubbleTypeToCreate (chanceToRollBonus) == BubbleType.Point) {
					CreatePointBubble (newBubbleIndex);
				} else {
					CreateBonusBubble (newBubbleIndex);
				}
				bubbleOccupiedArray [newBubbleIndex] = true;
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// CheckForBubbleHighlight
		//---------------------------------------------------------------------------------------------------------
		// We check to see if any of the point bubbles are the next needed number in a sequence.  If so we want to 
		// highlight that bubble with a particle effect.  We only do this if they have purchased that option in the
		// shop.  Every bubble with a point value that matches should be highlighted
		//---------------------------------------------------------------------------------------------------------
		void CheckForBubbleHighlight()
		{
			if (playerHighlightNext) {
				foreach (var childBubble in bubbles.Children.OfType<PointBubble> ()) {
					bool isNextInSequence = (childBubble.PointValue == nextNumberInSequence);
					if (isNextInSequence) {
						if ((seqDepth > 0) && !childBubble.IsHighlighted) {
							childBubble.IsHighlighted = true;
							childBubble.Emitter.TotalParticles = 30;
							childBubble.AddChild (childBubble.Emitter);
						} 
					} else {
						childBubble.IsHighlighted = false;
						childBubble.Emitter.RemoveFromParent ();
					}
				}
			} else {
				foreach (var childBubble in bubbles.Children.OfType<PointBubble> ()) {
					if (childBubble.IsHighlighted) {
						childBubble.IsHighlighted = false;
						childBubble.Emitter.RemoveFromParent ();
					}
				}
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// GetRandomPosition
		//---------------------------------------------------------------------------------------------------------
		// Parameters: 	xIndex - indicies of the grid where the position will be in
		//				yIndex 
		//
		// Returns: CCPoint - random point within the center area of the x,y grid
		//---------------------------------------------------------------------------------------------------------
		static CCPoint GetRandomPosition (int xIndex, int yIndex)
		{
			double rndX = CCRandom.GetRandomFloat ((xIndex * CELL_DIMS_HALF * 2) + (SCREEN_X_MARGIN + CELL_DIMS_HALF - CELL_CENTER_ZONE_HALF), (xIndex * CELL_DIMS_HALF * 2) + (SCREEN_X_MARGIN + CELL_DIMS_HALF + CELL_CENTER_ZONE_HALF));
			double rndY = CCRandom.GetRandomFloat ((yIndex * CELL_DIMS_HALF * 2) + (SCREEN_Y_MARGIN + CELL_DIMS_HALF - CELL_CENTER_ZONE_HALF), (yIndex * CELL_DIMS_HALF * 2) + (SCREEN_Y_MARGIN + CELL_DIMS_HALF + CELL_CENTER_ZONE_HALF));
			return new CCPoint ((float)rndX, (float)rndY);
		}

		//---------------------------------------------------------------------------------------------------------
		// GetRandomScoreValue
		//---------------------------------------------------------------------------------------------------------
		// Parameters:	Random - Random object for seed
		//				int - Maximum point value
		//
		// Returns:		int - Random integer to be used for bubble point value
		//---------------------------------------------------------------------------------------------------------
		// This method returns a random number between 1 and maxPointValue.  The randomization is such that numbers
		// closer to 1 will be more common and numbers closer to maxPointValue will be more rare.
		//---------------------------------------------------------------------------------------------------------
		static int GetRandomScoreValue (Random scoreRandomizer, int maxPointValue)
		{
			int scoreValue = (int)Math.Floor ((scoreRandomizer.NextDouble () * scoreRandomizer.NextDouble ()) * maxPointValue) + 1;

			return scoreValue;
		}

		//---------------------------------------------------------------------------------------------------------
		// GetRandomScoreValue
		//---------------------------------------------------------------------------------------------------------
		// Parameters:	Random - Random object for seed
		//				int - Maximum point value
		//				int - Next sequence value
		//
		// Returns:		int - Random integer to be used for bubble point value
		//---------------------------------------------------------------------------------------------------------
		// This method returns a random number between 1 and maxPointValue.  The randomization is such that numbers
		// closer to 1 will be more common and numbers closer to maxPointValue will be more rare.  There is also a
		// % chance to return the next sequence number
		//---------------------------------------------------------------------------------------------------------
		static int GetRandomScoreValue (Random scoreRandomizer, int maxPointValue, int nextSequenceValue, int percentChanceNext)
		{
			var r = CCRandom.GetRandomInt (1, 3000);  // make this higher to decrease the chance that it will outright give the next sequence number
			int scoreValue;

			if (r <= percentChanceNext) {
				scoreValue = nextSequenceValue;
			} else {
				scoreValue = (int)Math.Floor ((scoreRandomizer.NextDouble () * scoreRandomizer.NextDouble ()) * maxPointValue) + 1;
			}
			return scoreValue;
		}

		//---------------------------------------------------------------------------------------------------------
		// DisplayBubble
		//---------------------------------------------------------------------------------------------------------
		// Async method to display a bubble with the randomized fade in, delay, and fade out paramaters.  Will 
		// mark the grid index as empty after the bubble has faded and remove the node from the parent.
		//---------------------------------------------------------------------------------------------------------
		async void DisplayBubble(Bubble newBubble)
		{
			await newBubble.BubbleSprite.RunActionsAsync (new CCDelayTime(newBubble.TimeDelay), new CCFadeIn (newBubble.TimeAppear), new CCDelayTime (newBubble.TimeHold), new CCFadeOut (newBubble.TimeFade));
			bubbleOccupiedArray [newBubble.ListIndex] = false;
			newBubble.RemoveFromParent ();
		}

		//---------------------------------------------------------------------------------------------------------
		// DisplayLabel
		//---------------------------------------------------------------------------------------------------------
		// Similar to the DisplayBubble method.  Matched fade and hold values of the associated bubble.  Don't need
		// to remove from parent because DisplayBubble will take care of that.
		//---------------------------------------------------------------------------------------------------------
		static async void DisplayLabel(PointBubble newBubble)
		{
			await newBubble.PointLabel.RunActionsAsync (new CCDelayTime(newBubble.TimeDelay), new CCFadeIn (newBubble.TimeAppear), new CCDelayTime (newBubble.TimeHold), new CCFadeOut (newBubble.TimeFade));
		}

		//---------------------------------------------------------------------------------------------------------
		// DisplayHighlight
		//---------------------------------------------------------------------------------------------------------
		// Displays the particle effect for highlighted bubbles
		//---------------------------------------------------------------------------------------------------------
		static async void DisplayHighlight(PointBubble newBubble)
		{
			await newBubble.Emitter.RunActionsAsync (new CCDelayTime (newBubble.TimeDelay), new CCDelayTime (newBubble.TimeAppear), new CCFadeIn (0.01f), new CCDelayTime (newBubble.TimeHold), new CCFadeOut (newBubble.TimeFade));
		}

		//---------------------------------------------------------------------------------------------------------
		// DisplayBonus
		//---------------------------------------------------------------------------------------------------------
		// Similar to the DisplayBubble method.  Matched fade and hold values of the associated bubble.  Don't need
		// to remove from parent because DisplayBubble will take care of that.
		//---------------------------------------------------------------------------------------------------------
		static async void DisplayBonus(BonusBubble newBubble)
		{
			await newBubble.BonusSprite.RunActionsAsync (new CCDelayTime(newBubble.TimeDelay), new CCFadeIn (newBubble.TimeAppear), new CCDelayTime (newBubble.TimeHold), new CCFadeOut (newBubble.TimeFade));
		}

		//---------------------------------------------------------------------------------------------------------
		// PopBubble
		//---------------------------------------------------------------------------------------------------------
		// Parameters:	poppedBubble - PointBubble for the pointBubble object being popped
		//
		// Returns:		void
		//---------------------------------------------------------------------------------------------------------
		// Asynchronus method called to pop bubbles
		//---------------------------------------------------------------------------------------------------------
		static async void PopBubble(PointBubble poppedBubble)
		{
			poppedBubble.BubbleSprite.RemoveFromParent ();
			poppedBubble.PointLabel.RemoveFromParent ();
			poppedBubble.PopSprite.RunAction (new CCScaleTo(0.07f,1));
			poppedBubble.PopSprite.RunAction (new CCDelayTime (.3f));
			await poppedBubble.PopSprite.RunActionAsync (new CCFadeOut (0.3f));
			poppedBubble.RemoveFromParent ();
		}

		static async void PopBubble(BonusBubble poppedBubble)
		{
			poppedBubble.BubbleSprite.RemoveFromParent ();
			poppedBubble.Pop ();
			poppedBubble.PopSprite.RunAction (new CCScaleTo(0.07f,1));
			poppedBubble.PopSprite.RunAction (new CCDelayTime (.3f));
			await poppedBubble.PopSprite.RunActionAsync (new CCFadeOut (0.3f));
			poppedBubble.RemoveFromParent ();
		}

		//---------------------------------------------------------------------------------------------------------
		// MovePointLabel
		//---------------------------------------------------------------------------------------------------------
		// Moves the point label to the score bar.  Overload moves from subtraction or addition button.
		//---------------------------------------------------------------------------------------------------------
		async void MovePointLabel(PointBubble poppedBubble, CCNode target)
		{
			var bubblePointAmount = poppedBubble.PointValue * bonusDoubleMultiplier * bonusTripleMultiplier;
			var targetCenter = new CCPoint ();
			targetCenter.X = target.Position.X + target.ContentSize.Center.X;
			targetCenter.Y = target.Position.Y;

			var actionTo = new CCMoveTo (.6f, targetCenter);

			var newLabel = new CCLabel (bubblePointAmount.ToString (), GOTHIC_44_HD_FNT);
			newLabel.AnchorPoint = poppedBubble.PointLabel.AnchorPoint;
			newLabel.Opacity = poppedBubble.PointLabel.Opacity;
			newLabel.Position = poppedBubble.PointLabel.PositionWorldspace;

			AddChild (newLabel, 500);

			newLabel.RunAction (actionTo); 
			await newLabel.RunActionAsync (new CCFadeOut (1.3f));
			newLabel.RemoveFromParent ();
		}

		async void MovePointLabel(CCLabel pointvalue, CCNode target)
		{
			var targetCenter = new CCPoint ();
			targetCenter.X = target.Position.X + target.ContentSize.Center.X;
			targetCenter.Y = target.Position.Y;

			var actionTo = new CCMoveTo (.6f, targetCenter);

			var newLabel = new CCLabel (pointvalue.Text, GOTHIC_30_HD_FNT);
			newLabel.AnchorPoint = pointvalue.AnchorPoint;
			newLabel.Opacity = pointvalue.Opacity;
			newLabel.Position = pointvalue.PositionWorldspace;

			AddChild (newLabel, 500);

			newLabel.RunAction (actionTo); 
			await newLabel.RunActionAsync (new CCFadeOut (1.3f));
			newLabel.RemoveFromParent ();
		}

		async void MovePointLabel(CCNode origin, CCNode target, int pointAmount)
		{
			var targetCenter = new CCPoint ();
			targetCenter.X = target.Position.X + target.ContentSize.Center.X;
			targetCenter.Y = target.Position.Y;

			var actionTo = new CCMoveTo (.6f, targetCenter);

			var newLabel = new CCLabel (pointAmount.ToString (), GOTHIC_44_HD_FNT);
			newLabel.AnchorPoint = CCPoint.AnchorMiddle;
			newLabel.Opacity = 0;
			newLabel.Position = origin.PositionWorldspace;

			AddChild (newLabel, 500);

			newLabel.RunAction (actionTo); 
			await newLabel.RunActionAsync (new CCFadeOut (1.3f));
			newLabel.RemoveFromParent ();
		}

		//---------------------------------------------------------------------------------------------------------
		// MoveBonusSprite
		//---------------------------------------------------------------------------------------------------------
		// Moves the bonus sprite to the appropriate UI element
		//---------------------------------------------------------------------------------------------------------
		async void MoveBonusSprite(BonusBubble poppedBubble, CCNode target)
		{
			var targetCenter = new CCPoint ();
			targetCenter.X = target.PositionWorldspace.X; 
			targetCenter.Y = target.PositionWorldspace.Y; 

			var actionTo = new CCMoveTo (.6f, targetCenter);

			var spriteName = string.Format ("bonus_{0}.png", poppedBubble.BonusType);
			var newBonusSprite = new CCSprite (bonusSpriteSheet.Frames.Find (x => x.TextureFilename.Equals (spriteName)));
			newBonusSprite.AnchorPoint = poppedBubble.BonusSprite.AnchorPoint;
			newBonusSprite.Opacity = poppedBubble.BonusSprite.Opacity;
			newBonusSprite.Position = poppedBubble.BonusSprite.PositionWorldspace;

			AddChild (newBonusSprite,500); 

			newBonusSprite.RunAction (actionTo);
			newBonusSprite.RunAction (new CCScaleTo(0.6f, 0.3f));
			await newBonusSprite.RunActionAsync (new CCFadeOut (1.4f));
			newBonusSprite.RemoveFromParent ();
		}

		//---------------------------------------------------------------------------------------------------------
		// ExpandLabel
		//---------------------------------------------------------------------------------------------------------
		// Used to make an expanding label for extra points or time
		//---------------------------------------------------------------------------------------------------------
		async void ExpandLabel (string labelText, CCPoint startingPosition )
		{
			var bonusPointsLabel = new CCLabel (labelText, GOTHIC_44_HD_FNT);
			bonusPointsLabel.AnchorPoint = CCPoint.AnchorMiddle;
			bonusPointsLabel.Opacity = 0;
			bonusPointsLabel.Position = startingPosition;
			AddChild (bonusPointsLabel);

			var randomAngle = CCRandom.GetRandomFloat (-50f, 50f);

			bonusPointsLabel.RunAction (new CCScaleTo(2.5f,3f));
			bonusPointsLabel.RunAction (new CCRotateBy (2.5f, randomAngle));
			bonusPointsLabel.RunAction (new CCDelayTime (.5f));
			await bonusPointsLabel.RunActionAsync (new CCFadeOut (2.5f));
			bonusPointsLabel.RemoveFromParent ();
		}

		async void ExpandLabel (string labelText, CCPoint startingPosition, CCPoint endingPosition)
		{
			var actionTo = new CCMoveTo (.6f, endingPosition);

			var labelToExpand = new CCLabel (labelText, GOTHIC_44_HD_FNT);
			labelToExpand.AnchorPoint = CCPoint.AnchorMiddle;
			labelToExpand.Opacity = 0;
			labelToExpand.Position = startingPosition;
			AddChild (labelToExpand);

			var randomAngle = CCRandom.GetRandomFloat (-50f, 50f);

			labelToExpand.RunAction (actionTo);
			labelToExpand.RunAction (new CCScaleTo(2.5f,3f));
			labelToExpand.RunAction (new CCRotateBy (2.5f, randomAngle));
			labelToExpand.RunAction (new CCDelayTime (.5f));
			await labelToExpand.RunActionAsync (new CCFadeOut (2.5f));
			labelToExpand.RemoveFromParent ();
		}

		//---------------------------------------------------------------------------------------------------------
		// ProcessBonusBubble
		//---------------------------------------------------------------------------------------------------------
		// Logic for processing a bonus bubble when popped
		//---------------------------------------------------------------------------------------------------------
		void ProcessBonusBubble (BonusBubble bubbleToProcess)
		{
			string labelText;
			switch (bubbleToProcess.BonusType) {
			case Bonuses.pos_time:
				{
					var randomInt = CCRandom.GetRandomInt (3, 6);
					levelTimeLeft += (float)randomInt;
					labelText = string.Format ("{0} sec", randomInt);
					ExpandLabel (labelText, bubbleToProcess.PositionWorldspace );
					break;
				}
			case Bonuses.double_score:
				{
					doubleScoreTimerDuration += SCORE_MULTIPLIER_DURATION + activePlayer.Persistent2xTimeBonus;
					doubleScoreTimer.Percentage = 100;
					isDoubleScoreActive = true;
					bonusDoubleMultiplier = 2;
					break;
				}
			case Bonuses.pos_points:
				{
					var randomPoints = CCRandom.GetRandomInt (1, 5) * 50;
					regularTotal += randomPoints * bonusDoubleMultiplier * bonusTripleMultiplier;
					levelScore = regularTotal + sequenceTotal;
					labelText = string.Format ("+{0}", randomPoints);
					ExpandLabel (labelText, bubbleToProcess.PositionWorldspace );
					break;
				}
			case Bonuses.addition:
				{
					availableAdditions++;
					MoveBonusSprite (bubbleToProcess, menuAddition);
					break;
				}
			case Bonuses.subtraction:
				{
					availableSubtractions++;
					MoveBonusSprite (bubbleToProcess, menuSubtraction);
					break;
				}
			case Bonuses.checkpoint:
				{
					availableCheckpoints++;
					MoveBonusSprite (bubbleToProcess, menuCheckpoint);
					break;
				}
			case Bonuses.triple_score:
				{
					tripleScoreTimerDuration += SCORE_MULTIPLIER_DURATION + activePlayer.Persistent3xTimeBonus;
					tripleScoreTimer.Percentage = 100;
					isTripleScoreActive = true;
					bonusTripleMultiplier = 3;
					break;
				}
			case Bonuses.next_in_seq:
				{
					availableNextInSeq++;
					MoveBonusSprite (bubbleToProcess, menuNextInSeq);
					break;
				}
			case Bonuses.next_mf:
				{
					// chance to get next in sequence
					ExpandLabel ("+Seq\nLuck!", bubbleToProcess.PositionWorldspace);
					playerChanceToRollNextSeq += 1;
					break;
				}
			case Bonuses.bonus_mf:
				{
					// chance to get bonus bubble	
					// activePlayer.BonusBubbleMagicFind += 1; TODO: need to track level chance to roll bonuses separate from player's magic find
					playerChanceToRollBonus += 1;
					ExpandLabel ("+Bonus\nLuck!", bubbleToProcess.PositionWorldspace);
					break;
				}
			case Bonuses.lucky_point_chance:
				{
					chanceForLuckyPoints += 1;
					ExpandLabel ("+Luck %", bubbleToProcess.PositionWorldspace);
					break;
				}
			case Bonuses.lucky_point_amount:
				{
					maxLuckyPointAmount += 1;
					ExpandLabel ("+Luck\nPoints!", bubbleToProcess.PositionWorldspace);
					break;
				}
			case Bonuses.tap_str:
				{
					levelTapStrength += 1;
					MoveBonusSprite (bubbleToProcess, tapStrengthSprite);
					break;
				}
			case Bonuses.neg_time:
				{

					break;
				}
			case Bonuses.neg_points:
				{

					break;
				}
			case Bonuses.mystery:
				{

					break;
				}
			}
			UpdateConsumablesDisplay ();
			UpdateLabels ();
		}

		//---------------------------------------------------------------------------------------------------------
		// CombinedBubbleValues
		//---------------------------------------------------------------------------------------------------------
		// Depending on the PointState, either adds or subtracts the two values and returns the result.
		//---------------------------------------------------------------------------------------------------------
		static int CombineBubbleValues (PointState currentPointState, int value1, int value2)
		{
			int combinedValues;
			switch (currentPointState) {
			case PointState.Addition:
				{
					combinedValues = value1 + value2;
					break;
				}
			case PointState.Subtraction:
				{
					combinedValues = (value1 > value2) ? value1 - value2 : value2 - value1;
					if (combinedValues < 0)
						combinedValues = 0;
					break;
				}
			default:
				{
					combinedValues = 0;
					break;
				}
			}

			return combinedValues;
		}

		//---------------------------------------------------------------------------------------------------------
		// ShouldLevelEnd
		//---------------------------------------------------------------------------------------------------------
		// Checks for conditions for the game to end.  Should probably change this to reflect level ending or 
		// maybe add a separate method to check for level ending vs game ending.
		//---------------------------------------------------------------------------------------------------------
		public bool ShouldLevelEnd()
		{
			return levelTimeLeft <= 0;
		}

		//---------------------------------------------------------------------------------------------------------
		// EndLevel
		//---------------------------------------------------------------------------------------------------------
		// Unschedules everything that is scheduled.  Creates the GameOver layer and passes the score to that layer 
		// Sets the transition and has the Director transition to the GameOverLayer.
		//
		// End game should only happen if the player didn't reach the required score before the time is done.
		//---------------------------------------------------------------------------------------------------------
		void EndLevel()
		{
			UnscheduleAll();
			bubbles.RemoveAllChildren ();

			var levelEndScene = TutorialLevelFinishedLayer.CreateScene (Window, levelScore, activePlayer, activeLevel, didPassLevel, coinsEarned);
			var transitionToLevelOver = new CCTransitionFade (2.0f, levelEndScene);

			Director.ReplaceScene (transitionToLevelOver);
		}

		//---------------------------------------------------------------------------------------------------------
		// ConvertScoreToCoins
		//---------------------------------------------------------------------------------------------------------
		// Takes the amout of overscore the player earned and converts to 1 coin for every x number of points
		//---------------------------------------------------------------------------------------------------------
		public float ConvertScoreToCoins(int overScore)
		{
			return (overScore * COIN_MULTIPLIER) + activePlayer.CoinCarryOver;

			//TODO: create a layer to display calculation of points to coins
		}

	}
}

