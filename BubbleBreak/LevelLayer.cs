using System;
using System.Collections.Generic;
using CocosSharp;
//using System.Xml;
//using System.Xml.Serialization;
//using System.Xml.Linq;
using System.Linq;

namespace BubbleBreak
{ 
	public class LevelLayer : CCLayerColor
	{
		const float BUBBLE_SCALE = 1.15f;			// amount we want to scale the bubble by.  Should change the sprite graphic so this isn't needed.
		const int MAX_BUBBLES_X = 5;				// bubble grid width
		const int MAX_BUBBLES_Y = 8;				// bubble grid height
		const float SCREEN_X_MARGIN = 80.0f;		// margin from the left reserved for UI controls
		const float SCREEN_Y_MARGIN = 120f; 		// margin from the bottom for UI controls.  320 total  120 for bottom, 200 for top
		const float CELL_DIMS_HALF = 100f;			// half width of a bubble grid cell
		const float CELL_CENTER_ZONE_HALF = 28f;	// half width of the area in the center of the cell that a bubble can randomly be placed

		const float COIN_MULTIPLIER = 0.1f;		// multiplier to determin how many bonus coins the player gets - 10 extra points = 1 coin

		const String PI_DIGITS = "31415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679821480865132823066470938446095505822317253594081284811174502841027019385211055596446229489549303819644288109756659334461";

		CCNode bubbles;

		bool[] bubbleOccupiedArray;
		bool levelPassed = false;

		Random scoreRandom = new Random();			// randomizer used for getting a new bubble score

		CCPoint tapLocation;
		CCLabel levelNameLabel, scoreLabel, tapStrengthLabel;
		//CCLabel s01Label, s02Label,s03Label, s04Label, s05Label, s06Label, s07Label, s08Label, s09Label, s10Label, s11Label, s12Label;
		CCLabel sequenceTallyLabel;
		CCLabel nextInSequence;

		CCSpriteSheet uiSpriteSheet;
		CCSprite timeLabelSprite, scoreLabelSprite, timeSpriteProgressBarEmpty, scoreSpriteProgressBarEmpty, timeSpriteProgressBarFull, scoreSpriteProgressBarFull, tapStrengthSprite;
		CCSprite s01StdSprite, s01SelSprite, s02StdSprite, s02SelSprite, s03StdSprite, s03SelSprite, s04StdSprite, s04SelSprite, s05StdSprite, s05SelSprite, s06StdSprite, s06SelSprite,
				s07StdSprite, s07SelSprite, s08StdSprite, s08SelSprite, s09StdSprite, s09SelSprite, s10StdSprite, s10SelSprite, s11StdSprite, s11SelSprite, s12StdSprite, s12SelSprite;
		CCSprite sequenceIndicatorSprite;

		CCSprite okStd, okSel, frameSprite;

		CCProgressTimer timeBar, scoreBar;

		Player activePlayer;
		List<Level> levels;
		Level activeLevel;

		CCLayerColor hideUILayer;

		List<Sequences> availableSequences;		// list of sequences available to this level
		//List<Sequences> activeSequences;		// list of sequences that are active to the player
		//List<Sequences> playerUnlockedSequences;		// list of the sequences the player has unlocked
		Sequences currentSequence = Sequences.Linear;	// current sequence the player can build on
		int nextNumberInSequence = 1;				// the next number in the current sequence
		int seqTally = 0;						// bonus points added to score for popping a bubble with a value that was next in the sequence
		int seqDepth = 0;						// how deep are we into generating a sequence
		Sequences selectedSequence = Sequences.Linear;	// to track the selected sequence so it doesn't reset if you re-select the same sequence

		int levelNumber;						// level index number

		float levelTimeLimit;		 			// how many second we have to get the levelPassScore
		float levelTimeLeft;					// decreasing time left to complete the level

		string levelTitleText, levelDescriptionText, levelTimeLimitText, levelScoreToPassText;

		int levelTimeIncrement = 0;				// measures the increasing progress of level time.  Used to increment the maximum point value of a new bubble
		int pointIncrementDelay = 40; 			// delay in 10ths of a second to increase the highest random point value generated
		int bubblesVisibleIncrementDelay = 10; 	// delay in 10ths of a second to increase the number of bubbles on the screen

		int levelScore = 0;						// level score
		bool scoreBonusFlag = false;			// to determine if the score progress bar should show bonus mode
		int levelPassScore;						// how many point we need to pass the level
		int tapsRequired;						// how many taps to pop a standard bubble on this level
		int maxLevelPoints;						// maximum point value for new bubbles.  Initially assigned by the currentLevel property but increased as the level progresses
		int levelVisibleBubbles;				// how many bubbles can currently be visible at a time.  This number increases as the level progresses
		int lastEarnedPoint = 0;				// The value of the last point earned.  Used to determine if we need to increase the maximum point for a new bubble
		int maxBubbles; 						// the maximum amount of bubbles that will fit on the screen at one time.
		int maxVisibleBubbles;					// the limit for how many bubbles we actually want visible on the screen

		int percentToRollNextSeq;				// the percentage chance that the next number generated will be the next sequence value the player is looking for

		int coinsEarned = 0;

		List<int> listOfPrimes;
		List<int> listOfRecamans;

		//---------------------------------------------------------------------------------------------------------
		// LevelLayer
		//---------------------------------------------------------------------------------------------------------
		public LevelLayer (List<Level> gameLevels, Player currentPlayer) : base (new CCColor4B(0,0,0))
		{
			levels = gameLevels;
			activePlayer = currentPlayer;
			activeLevel = levels [currentPlayer.LastLevelCompleted + 1];
			CreateListOfSequences (activeLevel);
			percentToRollNextSeq = activeLevel.ChanceToRollNextSeq;

			//---------------------------------------------------------------------------------------------------------
			// Function to generate list of primes
			//---------------------------------------------------------------------------------------------------------
			Func<int, IEnumerable<int>> primeNumbers = max => 
				from i in Enumerable.Range(2, max - 1) 
				where Enumerable.Range(2, i - 2).All(j => i % j != 0) 
				select i; 
			IEnumerable<int> result = primeNumbers(500); 
			listOfPrimes = result.ToList ();
			listOfRecamans = recamanSeq (200);
			SetupUI ();
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
						levelPassed = true;
						activePlayer.LastLevelCompleted++;
						coinsEarned = ConvertScoreToCoins(levelScore - levelPassScore);
						activePlayer.Coins += coinsEarned;
						activePlayer.WriteData ();  // after successfully passing the level, save the player's progress
						EndLevel();
					} 

					else
					{
						activePlayer.WriteData ();
						EndLevel();
					}
						
					return;
				}

				levelTimeLeft -= t;
				levelTimeIncrement++;
				if (levelTimeIncrement > 0) {
					maxLevelPoints = ((levelTimeIncrement % pointIncrementDelay) == 0) ? maxLevelPoints + 1 : maxLevelPoints;
				}

				if (levelTimeIncrement > 0) {
					if (levelVisibleBubbles < maxVisibleBubbles){
					levelVisibleBubbles = ((levelTimeIncrement % bubblesVisibleIncrementDelay) == 0) ? levelVisibleBubbles + 1 : levelVisibleBubbles;
					}
				}

			},0.1f);

			timeBar.Percentage = (levelTimeLeft / levelTimeLimit) * 100;
			scoreBar.Percentage = ((float)levelScore / (float)levelPassScore) * 100;
			if (levelScore > levelPassScore) {
				if (!scoreBonusFlag) {
					scoreSpriteProgressBarFull = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("prgbar_score_bonus.png")));
					scoreBar.Sprite = scoreSpriteProgressBarFull;
					scoreBonusFlag = true;
				}
			}
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
			Schedule (_ => StartScheduling ());
		}

		//---------------------------------------------------------------------------------------------------------
		// OnTouchesEnded
		//---------------------------------------------------------------------------------------------------------
		void OnTouchesEnded (List<CCTouch> touches, CCEvent touchEvent)
		{
			bool bubblePopped = false;

			// check if a bubble has been tapped on
			if (touches.Count > 0) {
				var locationOnScreen = touches [0].Location;
				tapLocation = locationOnScreen;

				foreach (PointBubble standardBubble in bubbles.Children) { 
					bool doesTouchOverlapBubble = standardBubble.BubbleSprite.BoundingBoxTransformedToWorld.ContainsPoint (tapLocation);
					if (doesTouchOverlapBubble) {
						standardBubble.TapCount += activePlayer.TapStrength;
						if (standardBubble.CheckPopped ()) {
							bubbleOccupiedArray [standardBubble.ListIndex] = false;
							var seqBonusAmount = CheckForSequenceBonus (standardBubble.PointValue, nextNumberInSequence, currentSequence);
							seqTally = (seqBonusAmount != 0) ? seqTally + seqBonusAmount : seqBonusAmount;
							levelScore += standardBubble.PointValue + seqTally;
							lastEarnedPoint = standardBubble.PointValue;
							PopBubble (standardBubble);
							scoreLabel.Text = levelScore + "/" + levelPassScore;
							sequenceTallyLabel.Text = seqTally.ToString ();
							nextInSequence.Text = (nextNumberInSequence == 0) ? "" : nextNumberInSequence.ToString ();
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
				ShuffleBag<int> newBubbleOrder = new ShuffleBag<int> ();

				for (int i = 0; i < bubbleOccupiedArray.Length; i++) {
					if (!bubbleOccupiedArray [i])
						newBubbleOrder.Add (i);
				}
					
				var newBubbleIndex = newBubbleOrder.Next ();
				var newBubble = new PointBubble (GetRandomScoreValue (scoreRandom, maxLevelPoints, nextNumberInSequence, percentToRollNextSeq), tapsRequired, (newBubbleIndex % MAX_BUBBLES_X), (newBubbleIndex / MAX_BUBBLES_X), newBubbleIndex);
				CCPoint p = GetRandomPosition ( newBubble.BubbleSprite.ContentSize * BUBBLE_SCALE, newBubble.XIndex, newBubble.YIndex);
				newBubble.Position = new CCPoint (p.X, p.Y);
				newBubble.Scale = BUBBLE_SCALE;
				bubbles.AddChild (newBubble);
				DisplayLabel (newBubble);
				DisplayBubble (newBubble);
				bubbleOccupiedArray [newBubbleIndex] = true;
				maxLevelPoints = (lastEarnedPoint >= maxLevelPoints) ? lastEarnedPoint + 1 : maxLevelPoints;
			}
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
			ShuffleBag<int> bubbleOrder = new ShuffleBag<int> ();
			for (int i = 0; i < maxBubbles; i++) {
				bubbleOrder.Add (i);
			}
			for (int i = 0; i < levelVisibleBubbles; i++) {
				var bubbleIndex = bubbleOrder.Next ();
				var standardBubble = new PointBubble (GetRandomScoreValue (scoreRandom, maxLevelPoints), tapsRequired, (bubbleIndex % MAX_BUBBLES_X), (bubbleIndex / MAX_BUBBLES_X), bubbleIndex);
				CCPoint p = GetRandomPosition (standardBubble.BubbleSprite.ContentSize * BUBBLE_SCALE, standardBubble.XIndex, standardBubble.YIndex);
				standardBubble.Position = new CCPoint (p.X, p.Y);
				standardBubble.Scale = BUBBLE_SCALE;
				DisplayBubble (standardBubble);
				DisplayLabel (standardBubble);
				bubbles.AddChild (standardBubble);
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
				ShuffleBag<int> newBubbleOrder = new ShuffleBag<int> ();

				for (int i = 0; i < bubbleOccupiedArray.Length; i++) {
					if (!bubbleOccupiedArray [i])
						newBubbleOrder.Add (i);
				}
					
				var newBubbleIndex = newBubbleOrder.Next ();
				var newBubble = new PointBubble (GetRandomScoreValue (scoreRandom, maxLevelPoints, nextNumberInSequence, percentToRollNextSeq), tapsRequired, (newBubbleIndex % MAX_BUBBLES_X), (newBubbleIndex / MAX_BUBBLES_X), newBubbleIndex);
				CCPoint p = GetRandomPosition ( newBubble.BubbleSprite.ContentSize * BUBBLE_SCALE, newBubble.XIndex, newBubble.YIndex);
				newBubble.Position = new CCPoint (p.X, p.Y);
				newBubble.Scale = BUBBLE_SCALE;
				DisplayLabel (newBubble);
				DisplayBubble (newBubble);
				bubbles.AddChild (newBubble);
				bubbleOccupiedArray [newBubbleIndex] = true;
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// GetRandomPosition
		//---------------------------------------------------------------------------------------------------------
		// Parameters: 	CCSize - size of the sprite to position
		//				xIndex - indicies of the grid where the position will be in
		//				yIndex 
		//
		// Returns: CCPoint - random point within the center area of the x,y grid
		//---------------------------------------------------------------------------------------------------------
		CCPoint GetRandomPosition (CCSize spriteSize, int xIndex, int yIndex)
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
		int GetRandomScoreValue (Random scoreRandom, int maxPointValue)
		{
			int scoreValue = (int)Math.Floor ((scoreRandom.NextDouble () * scoreRandom.NextDouble ()) * maxPointValue) + 1;

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
		int GetRandomScoreValue (Random scoreRandom, int maxPointValue, int nextSequenceValue, int percentChanceNext)
		{
			var r = CCRandom.GetRandomInt (1, 100);
			int scoreValue;

			if (r <= percentChanceNext) {
				scoreValue = nextSequenceValue;
			} else {
				scoreValue = (int)Math.Floor ((scoreRandom.NextDouble () * scoreRandom.NextDouble ()) * maxPointValue) + 1;
			}
			return scoreValue;
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
		async void PopBubble(PointBubble poppedBubble)
		{
			poppedBubble.BubbleSprite.RemoveFromParent ();
			poppedBubble.PointLabel.RemoveFromParent ();
			poppedBubble.PopSprite.RunAction (new CCScaleTo(0.07f,1));
			poppedBubble.PopSprite.RunAction (new CCDelayTime (.3f));
			await poppedBubble.PopSprite.RunActionAsync (new CCFadeOut (0.3f));
			poppedBubble.RemoveFromParent ();
		}

		//---------------------------------------------------------------------------------------------------------
		// DisplayBubble
		//---------------------------------------------------------------------------------------------------------
		// Async method to display a bubble with the randomized fade in, delay, and fade out paramaters.  Will 
		// mark the grid index as empty after the bubble has faded and remove the node from the parent.
		//---------------------------------------------------------------------------------------------------------
		async void DisplayBubble(PointBubble newBubble)
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
		async void DisplayLabel(PointBubble newBubble)
		{
			await newBubble.PointLabel.RunActionsAsync (new CCDelayTime(newBubble.TimeDelay), new CCFadeIn (newBubble.TimeAppear), new CCDelayTime (newBubble.TimeHold), new CCFadeOut (newBubble.TimeFade));
		}

		//---------------------------------------------------------------------------------------------------------
		// CreateScene
		//---------------------------------------------------------------------------------------------------------
		// Creates the GameLayer scene
		//---------------------------------------------------------------------------------------------------------
		public static CCScene CreateScene (CCWindow mainWindow, List<Level> levels, Player currentPlayer)
		{
			var scene = new CCScene (mainWindow);
			var layer = new LevelLayer (levels, currentPlayer);

			scene.AddChild (layer);

			return scene;
		}

		//---------------------------------------------------------------------------------------------------------
		// EndLevel
		//---------------------------------------------------------------------------------------------------------
		// Unschedules everything that is scheduled.  Creates the GameOver layer and passes the score to that layer
		// Sets the transition and has the Director transition to the GameOverLayer.
		//
		// End game should only happen if the player didn't reach the required score befoe the time is done.
		//---------------------------------------------------------------------------------------------------------
		void EndLevel()
		{
			UnscheduleAll();
			bubbles.RemoveAllChildren ();

			var levelEndScene = LevelFinishedLayer.CreateScene (Window, levelScore, levels, activePlayer, levelPassed, coinsEarned);
			var transitionToLevelOver = new CCTransitionFade (3.0f, levelEndScene);

			Director.ReplaceScene (transitionToLevelOver);
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
		// ConvertScoreToCoins
		//---------------------------------------------------------------------------------------------------------
		// Takes the amout of overscore the player earned and converts to 1 coin for every x number of points
		//---------------------------------------------------------------------------------------------------------
		public int ConvertScoreToCoins(int overScore)
		{
			int bonusCoins = (int)(overScore * COIN_MULTIPLIER);
			return bonusCoins;

			//TODO: create a layer to display calculation of points to coins
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
			this.PauseListeners (true);

			// create the level info layer over top of the game layer
			var levelInfoLayer = new CCLayerColor (new CCColor4B (0, 0, 0, 200));
			AddChild (levelInfoLayer, 99999);

			// Add the information frame sprite to layer
			frameSprite = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("frame.png")));
			frameSprite.AnchorPoint = CCPoint.AnchorMiddle;
			frameSprite.Position = new CCPoint (bounds.Size.Width / 2, bounds.Size.Height / 2);
			levelInfoLayer.AddChild (frameSprite);

			// Add Level Information to the frame
			levelTitleText = "Level " + levelNumber;
			levelDescriptionText = "\"" + activeLevel.LevelDescription + "\"";
			levelTimeLimitText = "Time limit: " + activeLevel.MaxLevelTime.ToString () + " seconds";
			levelScoreToPassText = "Score to pass level: " + activeLevel.LevelPassScore.ToString ();

			// Add title lable
			var titleLabel = new CCLabel(levelTitleText, "arial", 30);
			titleLabel.Scale = 3f;
			titleLabel.AnchorPoint = CCPoint.AnchorMiddle;
			titleLabel.Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MaxY - (titleLabel.BoundingBox.Size.Height * 3f));
			levelInfoLayer.AddChild (titleLabel);

			// Add level information labels
			var levelDescriptionLabel = new CCLabel (levelDescriptionText, "arial", 30);
			levelDescriptionLabel.Scale = 1.5f;
			levelDescriptionLabel.Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MinY + (levelDescriptionLabel.BoundingBox.Size.Height * 20f));
			levelDescriptionLabel.AnchorPoint = CCPoint.AnchorMiddle;
			levelDescriptionLabel.HorizontalAlignment = CCTextAlignment.Center;
			levelInfoLayer.AddChild (levelDescriptionLabel);

			var levelTimeLimitLabel = new CCLabel (levelTimeLimitText, "arial", 30);
			levelTimeLimitLabel.Scale = 1.5f;
			levelTimeLimitLabel.Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MinY + (levelDescriptionLabel.BoundingBox.Size.Height * 17f));
			levelTimeLimitLabel.AnchorPoint = CCPoint.AnchorMiddle;
			levelTimeLimitLabel.HorizontalAlignment = CCTextAlignment.Center;
			levelInfoLayer.AddChild (levelTimeLimitLabel);

			var levelScoreToPassLabel = new CCLabel (levelScoreToPassText, "arial", 30);
			levelScoreToPassLabel.Scale = 1.5f;
			levelScoreToPassLabel.Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MinY + (levelDescriptionLabel.BoundingBox.Size.Height * 14f));
			levelScoreToPassLabel.AnchorPoint = CCPoint.AnchorMiddle;
			levelScoreToPassLabel.HorizontalAlignment = CCTextAlignment.Center;
			levelInfoLayer.AddChild (levelScoreToPassLabel);

			// set up the sprites for the OK button 
			okStd = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("ok_std.png")));
			okStd.AnchorPoint = CCPoint.AnchorMiddle;
			okSel = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("ok_sel.png")));
			okSel.AnchorPoint = CCPoint.AnchorMiddle;

			// create the ok button menu item
			var okMenuItem = new CCMenuItemImage (okStd, okSel, (closeSender) => {
				levelInfoLayer.RemoveFromParent ();
				hideUILayer.RemoveFromParent();
				this.ResumeListeners (true);
				Application.Paused = false;
			});

			// position the ok button relative to the information frame
			okMenuItem.AnchorPoint = CCPoint.AnchorMiddle;
			okMenuItem.Position = new CCPoint (bounds.Size.Width / 2, frameSprite.BoundingBox.MinY + (okStd.BoundingBox.Size.Height * 1.5f));

			// create the menu to close the pop up
			var closeMenu = new CCMenu (okMenuItem);
			closeMenu.AnchorPoint = CCPoint.AnchorMiddleBottom;
			closeMenu.Position = CCPoint.Zero;

			// add the menu to the levelInfoLayer
			levelInfoLayer.AddChild(closeMenu);
			Application.Paused = true;
		}

		//---------------------------------------------------------------------------------------------------------
		// SetupUI
		//---------------------------------------------------------------------------------------------------------
		// Sets up the game UI
		//---------------------------------------------------------------------------------------------------------
		void SetupUI ()
		{
			// assign values from activeLevel properties to the GameLayer properties
			levelNumber = activeLevel.LevelNum;
			maxBubbles = activeLevel.MaxBubbles;
			maxVisibleBubbles = activeLevel.MaxVisibleBubbles;
			maxLevelPoints = activeLevel.StartingPointValue;
			levelTimeLimit = activeLevel.MaxLevelTime;
			levelPassScore = activeLevel.LevelPassScore;
			tapsRequired = activeLevel.TapsToPopStandard;
			levelVisibleBubbles = activeLevel.InitialVisibleBubbles;

			//sequenceLevel = activeLevel.SequenceLevel;
			levelTimeLeft = levelTimeLimit + 3;
			bubbleOccupiedArray = new bool[maxBubbles];

			// create layer to hide main game ui while level intro is showing
			hideUILayer = new CCLayerColor (new CCColor4B (0, 0, 0, 255));
			AddChild (hideUILayer, 88888);

			uiSpriteSheet = new CCSpriteSheet ("ui.plist");

			levelNameLabel = new CCLabel ("Level", "arial", 30);
			levelNameLabel.Text = "Level " + levelNumber;
			levelNameLabel.Scale = 1.0f;
			levelNameLabel.PositionX = 160;
			levelNameLabel.PositionY = 1900;
			levelNameLabel.AnchorPoint = CCPoint.AnchorMiddleRight;
			AddChild (levelNameLabel);

			scoreLabel = new CCLabel ("Score:", "arial", 30);
			scoreLabel.Text = levelScore + "/" + levelPassScore;
			scoreLabel.Scale = 1.0f;
			scoreLabel.PositionX = 160;
			scoreLabel.PositionY = 1855;
			scoreLabel.AnchorPoint = CCPoint.AnchorMiddleRight;
			AddChild (scoreLabel);

			tapStrengthSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("tap_strength_std.png")));
			tapStrengthSprite.PositionX = 760;
			tapStrengthSprite.PositionY = 1870;
			tapStrengthSprite.AnchorPoint = CCPoint.AnchorMiddle;
			AddChild (tapStrengthSprite);

			sequenceIndicatorSprite = new CCSprite (uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("s01-linear.png")));
			sequenceIndicatorSprite.PositionX = 880;
			sequenceIndicatorSprite.PositionY = 1870;
			sequenceIndicatorSprite.AnchorPoint = CCPoint.AnchorMiddle;
			AddChild (sequenceIndicatorSprite);

			sequenceTallyLabel = new CCLabel ("", "arial", 30);
			sequenceTallyLabel.Text = seqTally.ToString ();
			sequenceTallyLabel.Scale = 1.2f;
			sequenceTallyLabel.PositionX = 925;
			sequenceTallyLabel.PositionY = 1910;
			sequenceTallyLabel.AnchorPoint = CCPoint.AnchorUpperLeft;
			AddChild (sequenceTallyLabel);

			nextInSequence = new CCLabel ("", "arial", 30);
			nextInSequence.Text = (nextNumberInSequence == 0) ? "" : nextNumberInSequence.ToString ();
			nextInSequence.Color = CCColor3B.Green;
			nextInSequence.Scale = 1.2f;
			nextInSequence.PositionX = 925;
			nextInSequence.PositionY = 1870;
			nextInSequence.AnchorPoint = CCPoint.AnchorUpperLeft;
			AddChild (nextInSequence);

			tapStrengthLabel = new CCLabel ("1", "arial", 30);
			tapStrengthLabel.Text = activePlayer.TapStrength.ToString ();
			tapStrengthLabel.Scale = 2.0f;
			tapStrengthLabel.PositionX = 810;
			tapStrengthLabel.PositionY = 1870;
			tapStrengthLabel.AnchorPoint = CCPoint.AnchorMiddleLeft;
			AddChild (tapStrengthLabel);

			timeLabelSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("lbl_time.png")));
			timeLabelSprite.AnchorPoint = CCPoint.AnchorMiddleLeft;
			timeLabelSprite.PositionX = 590;
			timeLabelSprite.PositionY = 1900;
			AddChild (timeLabelSprite);

			scoreLabelSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("lbl_score.png")));
			scoreLabelSprite.AnchorPoint = CCPoint.AnchorMiddleLeft;
			scoreLabelSprite.PositionX = 590;
			scoreLabelSprite.PositionY = 1855;
			AddChild (scoreLabelSprite);

			timeSpriteProgressBarEmpty = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("prgbar_time_empty.png")));
			timeSpriteProgressBarEmpty.AnchorPoint = CCPoint.AnchorMiddleLeft;
			timeSpriteProgressBarEmpty.PositionX = 170;
			timeSpriteProgressBarEmpty.PositionY = 1900;
			AddChild (timeSpriteProgressBarEmpty);

			scoreSpriteProgressBarEmpty = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("prgbar_score_empty.png")));
			scoreSpriteProgressBarEmpty.AnchorPoint = CCPoint.AnchorMiddleLeft;
			scoreSpriteProgressBarEmpty.PositionX = 170;
			scoreSpriteProgressBarEmpty.PositionY = 1855;
			AddChild (scoreSpriteProgressBarEmpty);

			timeSpriteProgressBarFull = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("prgbar_time_full.png")));
			timeBar = new CCProgressTimer (timeSpriteProgressBarFull);
			timeBar.AnchorPoint = CCPoint.AnchorMiddleLeft;
			timeBar.PositionX = 170;
			timeBar.PositionY = 1900;
			timeBar.Percentage = 100;
			timeBar.Midpoint = new CCPoint (0, 0);
			timeBar.BarChangeRate = new CCPoint (1, 0);
			timeBar.Type = CCProgressTimerType.Bar;
			AddChild (timeBar);

			scoreSpriteProgressBarFull = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("prgbar_score_full.png")));
			scoreBar = new CCProgressTimer (scoreSpriteProgressBarFull);
			scoreBar.AnchorPoint = CCPoint.AnchorMiddleLeft;
			scoreBar.PositionX = 170;
			scoreBar.PositionY = 1855;
			scoreBar.Midpoint = new CCPoint (0, 0);
			scoreBar.BarChangeRate = new CCPoint (1, 0);
			scoreBar.Type = CCProgressTimerType.Bar;
			AddChild (scoreBar);

			s01StdSprite = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("s01-linear.png")));
			s01SelSprite = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("s01-linear-g.png")));

			s02StdSprite = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("s02-even.png")));
			s02SelSprite = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("s02-even-g.png")));

			s03StdSprite = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("s03-odd.png")));
			s03SelSprite = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("s03-odd-g.png")));

			s04StdSprite = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("s04-triangular.png")));
			s04SelSprite = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("s04-triangular-g.png")));

			s05StdSprite = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("s05-square.png")));
			s05SelSprite = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("s05-square-g.png")));

			s06StdSprite = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("s06-lazy.png")));
			s06SelSprite = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("s06-lazy-g.png")));

			s07StdSprite = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("s07-fibonacci.png")));
			s07SelSprite = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("s07-fibonacci-g.png")));

			s08StdSprite = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("s08-prime.png")));
			s08SelSprite = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("s08-prime-g.png")));

			s09StdSprite = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("s09-double.png")));
			s09SelSprite = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("s09-double-g.png")));

			s10StdSprite = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("s10-triple.png")));
			s10SelSprite = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("s10-triple-g.png")));

			s11StdSprite = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("s11-pi.png")));
			s11SelSprite = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("s11-pi-g.png")));

			s12StdSprite = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("s12-recaman.png")));
			s12SelSprite = new CCSprite(uiSpriteSheet.Frames.Find ((x) => x.TextureFilename.Equals ("s12-recaman-g.png")));

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

			List<CCMenuItemImage> seqMenuItems = new List<CCMenuItemImage> ();
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

			var seqMenu01 = new CCMenu (s01StdMenuItem);
			var seqMenu02 = new CCMenu (s02StdMenuItem);
			var seqMenu03 = new CCMenu (s03StdMenuItem);
			var seqMenu04 = new CCMenu (s04StdMenuItem);
			var seqMenu05 = new CCMenu (s05StdMenuItem);
			var seqMenu06 = new CCMenu (s06StdMenuItem);
			var seqMenu07 = new CCMenu (s07StdMenuItem);
			var seqMenu08 = new CCMenu (s08StdMenuItem);
			var seqMenu09 = new CCMenu (s09StdMenuItem);
			var seqMenu10 = new CCMenu (s10StdMenuItem);
			var seqMenu11 = new CCMenu (s11StdMenuItem);
			var seqMenu12 = new CCMenu (s12StdMenuItem);

			List<CCMenu> seqMenus = new List<CCMenu>();
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
				if (seq == Sequences.Linear) {
					s01StdMenuItem.Visible = true;
				} 
				if (seq == Sequences.Even) {
					s02StdMenuItem.Visible = true;
				} 
				if (seq == Sequences.Odd) {
					s03StdMenuItem.Visible = true;
				} 
				if (seq == Sequences.Triangular) {
					s04StdMenuItem.Visible = true;
				} 
				if (seq == Sequences.Square) {
					s05StdMenuItem.Visible = true;
				} 
				if (seq == Sequences.Lazy) {
					s06StdMenuItem.Visible = true;
				} 
				if (seq == Sequences.Fibonacci) {
					s07StdMenuItem.Visible = true;
				} 
				if (seq == Sequences.Prime) {
					s08StdMenuItem.Visible = true;
				} 
				if (seq == Sequences.Double) {
					s09StdMenuItem.Visible = true;
				} 
				if (seq == Sequences.Triple) {
					s10StdMenuItem.Visible = true;
				} 
				if (seq == Sequences.Pi) {
					s11StdMenuItem.Visible = true;
				} 
				if (seq == Sequences.Recaman) {
					s12StdMenuItem.Visible = true;
				} 
			}

			for (int i = 0; i < seqMenus.Count; i++) {
				seqMenus [i].AlignItemsVertically (0);
				seqMenus [i].AnchorPoint = CCPoint.AnchorUpperLeft;
				seqMenus [i].IgnoreAnchorPointForPosition = false;
				seqMenus [i].PositionX = 40;
				seqMenus [i].PositionY = 1870 - (i * 150);
				AddChild (seqMenus[i]);
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// SeqMenuCallBack
		//---------------------------------------------------------------------------------------------------------
		// Callback when sequence UI icon is touched
		//---------------------------------------------------------------------------------------------------------
		void SeqMenuCallback (object pSender)
		{
			CCMenuItemImage sentMenuItem = (CCMenuItemImage)pSender;
			var seqTypeString = sentMenuItem.UserData.ToString ();
			var spritePos = sequenceIndicatorSprite.Position;
			switch (seqTypeString) {
			case "Linear":
				{
					if (selectedSequence != Sequences.Linear) {
						sequenceIndicatorSprite.RemoveFromParent ();
						currentSequence = Sequences.Linear;
						sequenceIndicatorSprite = s01StdSprite;
						sequenceIndicatorSprite.Position = spritePos;
						AddChild (sequenceIndicatorSprite);
						selectedSequence = Sequences.Linear;
						nextNumberInSequence = 1;
						nextInSequence.Text = nextNumberInSequence.ToString ();
						seqTally = 0;
						sequenceTallyLabel.Text = seqTally.ToString ();
						this.seqDepth = 0;
					}
					break;
				}
			case "Even":
				{
					if (selectedSequence != Sequences.Even) {
						sequenceIndicatorSprite.RemoveFromParent ();
						currentSequence = Sequences.Even;
						sequenceIndicatorSprite = s02StdSprite;
						sequenceIndicatorSprite.Position = spritePos;
						AddChild (sequenceIndicatorSprite);
						selectedSequence = Sequences.Even;
						nextNumberInSequence = 2;
						nextInSequence.Text = nextNumberInSequence.ToString ();
						seqTally = 0;
						sequenceTallyLabel.Text = seqTally.ToString ();
						this.seqDepth = 0;
					}
					break;
				}
			case "Odd":
					{
					if (selectedSequence != Sequences.Odd) {
						sequenceIndicatorSprite.RemoveFromParent ();
						currentSequence = Sequences.Odd;
						sequenceIndicatorSprite = s03StdSprite;
						sequenceIndicatorSprite.Position = spritePos;
						AddChild (sequenceIndicatorSprite);
						selectedSequence = Sequences.Odd;
						nextNumberInSequence = 1;
						nextInSequence.Text = nextNumberInSequence.ToString ();
						seqTally = 0;
						sequenceTallyLabel.Text = seqTally.ToString ();
						this.seqDepth = 0;
					}
					break;
				}
			case "Triangular":
				{
					if (selectedSequence != Sequences.Triangular) {
						sequenceIndicatorSprite.RemoveFromParent ();
						currentSequence = Sequences.Triangular;
						sequenceIndicatorSprite = s04StdSprite;
						sequenceIndicatorSprite.Position = spritePos;
						AddChild (sequenceIndicatorSprite);
						selectedSequence = Sequences.Triangular;
						nextNumberInSequence = 1;
						nextInSequence.Text = nextNumberInSequence.ToString ();
						seqTally = 0;
						sequenceTallyLabel.Text = seqTally.ToString ();
						this.seqDepth = 0;
					}
					break;
				}
			case "Square":
				{
					if (selectedSequence != Sequences.Square) {
						sequenceIndicatorSprite.RemoveFromParent ();
						currentSequence = Sequences.Square;
						sequenceIndicatorSprite = s05StdSprite;
						sequenceIndicatorSprite.Position = spritePos;
						AddChild (sequenceIndicatorSprite);
						selectedSequence = Sequences.Square;
						nextNumberInSequence = 1;
						nextInSequence.Text = nextNumberInSequence.ToString ();
						seqTally = 0;
						sequenceTallyLabel.Text = seqTally.ToString ();
						this.seqDepth = 0;
					}
					break;
				}
			case "Lazy":
				{
					if (selectedSequence != Sequences.Lazy) {
						sequenceIndicatorSprite.RemoveFromParent ();
						currentSequence = Sequences.Lazy;
						sequenceIndicatorSprite = s06StdSprite;
						sequenceIndicatorSprite.Position = spritePos;
						AddChild (sequenceIndicatorSprite);
						selectedSequence = Sequences.Lazy;
						nextNumberInSequence = 1;
						nextInSequence.Text = nextNumberInSequence.ToString ();
						seqTally = 0;
						sequenceTallyLabel.Text = seqTally.ToString ();
						this.seqDepth = 0;
					}
					break;
				}
			case "Fibonacci":
				{
					if (selectedSequence != Sequences.Fibonacci) {
						sequenceIndicatorSprite.RemoveFromParent ();
						currentSequence = Sequences.Fibonacci;
						sequenceIndicatorSprite = s07StdSprite;
						sequenceIndicatorSprite.Position = spritePos;
						AddChild (sequenceIndicatorSprite);
						selectedSequence = Sequences.Fibonacci;
						nextNumberInSequence = 1;
						nextInSequence.Text = nextNumberInSequence.ToString ();
						seqTally = 0;
						sequenceTallyLabel.Text = seqTally.ToString ();
						this.seqDepth = 0;
					}
					break;
				}
			case "Prime":
				{
					if (selectedSequence != Sequences.Prime) {
						sequenceIndicatorSprite.RemoveFromParent ();
						currentSequence = Sequences.Prime;
						sequenceIndicatorSprite = s08StdSprite;
						sequenceIndicatorSprite.Position = spritePos;
						AddChild (sequenceIndicatorSprite);
						selectedSequence = Sequences.Prime;
						nextNumberInSequence = 2;
						nextInSequence.Text = nextNumberInSequence.ToString ();
						seqTally = 0;
						sequenceTallyLabel.Text = seqTally.ToString ();
						this.seqDepth = 0;
					}
					break;
				}
			case "Double":
				{
					if (selectedSequence != Sequences.Double) {
						sequenceIndicatorSprite.RemoveFromParent ();
						currentSequence = Sequences.Double;
						sequenceIndicatorSprite = s09StdSprite;
						sequenceIndicatorSprite.Position = spritePos;
						AddChild (sequenceIndicatorSprite);
						selectedSequence = Sequences.Double;
						nextNumberInSequence = 1;
						nextInSequence.Text = nextNumberInSequence.ToString ();
						seqTally = 0;
						sequenceTallyLabel.Text = seqTally.ToString ();
						this.seqDepth = 0;
					}
					break;
				}
			case "Triple":
				{
					if (selectedSequence != Sequences.Triple) {
						sequenceIndicatorSprite.RemoveFromParent ();
						currentSequence = Sequences.Triple;
						sequenceIndicatorSprite = s10StdSprite;
						sequenceIndicatorSprite.Position = spritePos;
						AddChild (sequenceIndicatorSprite);
						selectedSequence = Sequences.Triple;
						nextNumberInSequence = 1;
						nextInSequence.Text = nextNumberInSequence.ToString ();
						seqTally = 0;
						sequenceTallyLabel.Text = seqTally.ToString ();
						this.seqDepth = 0;
					}
					break;
				}
			case "Pi":
				{
					if (selectedSequence != Sequences.Pi) {
						sequenceIndicatorSprite.RemoveFromParent ();
						currentSequence = Sequences.Pi;
						sequenceIndicatorSprite = s11StdSprite;
						sequenceIndicatorSprite.Position = spritePos;
						AddChild (sequenceIndicatorSprite);
						selectedSequence = Sequences.Pi;
						nextNumberInSequence = 3;
						nextInSequence.Text = nextNumberInSequence.ToString ();
						seqTally = 0;
						sequenceTallyLabel.Text = seqTally.ToString ();
						this.seqDepth = 0;
					}
					break;
				}
			case "Recaman":
				{
					if (selectedSequence != Sequences.Recaman) {
						sequenceIndicatorSprite.RemoveFromParent ();
						currentSequence = Sequences.Recaman;
						sequenceIndicatorSprite = s12StdSprite;
						sequenceIndicatorSprite.Position = spritePos;
						AddChild (sequenceIndicatorSprite);
						selectedSequence = Sequences.Recaman;
						nextNumberInSequence = 1;
						nextInSequence.Text = nextNumberInSequence.ToString ();
						seqTally = 0;
						sequenceTallyLabel.Text = seqTally.ToString ();
						this.seqDepth = 0;
					}
					break;
				}
				
			default:
				break;
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
				this.PauseListeners (true);
				Application.Paused = true;
				var menuLayer = new CCLayerColor (new CCColor4B (0, 0, 0, 200));
				AddChild (menuLayer, 99999);
				// Add frame to layer
				frameSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("frame.png")));
				frameSprite.AnchorPoint = CCPoint.AnchorMiddle;
				frameSprite.Position = new CCPoint (bounds.Size.Width / 2, bounds.Size.Height / 2);
				menuLayer.AddChild (frameSprite);
				okStd = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("ok_std.png")));
				okStd.AnchorPoint = CCPoint.AnchorMiddle;
				okSel = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("ok_sel.png")));
				okSel.AnchorPoint = CCPoint.AnchorMiddle;
				var closeItem = new CCMenuItemImage (okStd, okSel, closeSender =>  {
					menuLayer.RemoveFromParent ();
					this.ResumeListeners (true);
					Application.Paused = false;
				});
				closeItem.Position = bounds.Center;
				var closeMenu = new CCMenu (closeItem);
				closeMenu.AnchorPoint = CCPoint.AnchorMiddleBottom;
				closeMenu.Position = CCPoint.Zero;
				menuLayer.AddChild (closeMenu);
			});
			optionPopup.AnchorPoint = CCPoint.AnchorUpperRight;
			//optionPopup.Position = new CCPoint(bounds.Size.Width / 10, bounds.Size.Height / 14);
			var optionMenu = new CCMenu (optionPopup);
			optionMenu.AnchorPoint = CCPoint.AnchorUpperRight;
			optionMenu.Position = new CCPoint (1080, 1920);
			AddChild (optionMenu);
		}

		//---------------------------------------------------------------------------------------------------------
		// CreateListOfSequences
		//---------------------------------------------------------------------------------------------------------
		// Creates a list of sequences based on what the level data xml file says should be available
		//---------------------------------------------------------------------------------------------------------
		public void CreateListOfSequences(Level thisLevel)
		{
			availableSequences = new List<Sequences>();

			if (activeLevel.SeqLinear)
				availableSequences.Add (Sequences.Linear);
			if (activeLevel.SeqEven)
				availableSequences.Add (Sequences.Even);
			if (activeLevel.SeqOdd)
				availableSequences.Add (Sequences.Odd);
			if (activeLevel.SeqTriangular)
				availableSequences.Add (Sequences.Triangular);
			if (activeLevel.SeqSquare)
				availableSequences.Add (Sequences.Square);
			if (activeLevel.SeqLazy)
				availableSequences.Add (Sequences.Lazy);
			if (activeLevel.SeqFibonacci)
				availableSequences.Add (Sequences.Fibonacci);
			if (activeLevel.SeqPrime)
				availableSequences.Add (Sequences.Prime);
			if (activeLevel.SeqDouble)
				availableSequences.Add (Sequences.Double);
			if (activeLevel.SeqTriple)
				availableSequences.Add (Sequences.Triple);
			if (activeLevel.SeqPi)
				availableSequences.Add (Sequences.Pi);
			if (activeLevel.SeqRecaman)
				availableSequences.Add (Sequences.Recaman);
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
							this.seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, this.seqDepth);
						} else {
							pointsAwarded = nextNumberInSequence;
							this.seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, this.seqDepth);
						}
					} else {
						this.nextNumberInSequence = (pointValue == 1) ? 2 : 1; 
						this.seqDepth = (pointValue == 1) ? 1 : 0;
					}
					break;
				}
			case Sequences.Even:
				{
					if (pointValue == nextNumberInSequence) {
						if (pointValue == 2) {
							pointsAwarded = 0;
							this.seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, this.seqDepth);
						} else {
							pointsAwarded = nextNumberInSequence;
							this.seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, this.seqDepth);
						}
					} else {
						this.nextNumberInSequence = (pointValue == 2) ? 4 : 2; 
						this.seqDepth = (pointValue == 2) ? 1 : 0;
					}
					break;
				}
			case Sequences.Odd:
				{
					if (pointValue == nextNumberInSequence) {
						if (pointValue == 1) {
							pointsAwarded = 0;
							this.seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, this.seqDepth);
						} else {
							pointsAwarded = nextNumberInSequence;
							this.seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, this.seqDepth);
						}
					} else {
						this.nextNumberInSequence = (pointValue == 1) ? 3 : 1; 
						this.seqDepth = (pointValue == 1) ? 1 : 0;
					}
					break;
				}
			case Sequences.Triangular:
				{
					if (pointValue == nextNumberInSequence) {
						if (pointValue == 1) {
							pointsAwarded = 0;
							this.seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, this.seqDepth);
						} else {
							pointsAwarded = nextNumberInSequence;
							this.seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, this.seqDepth);
						}
					} else {
							this.nextNumberInSequence = (pointValue == 1) ? 3 : 1; 
						this.seqDepth = (pointValue == 1) ? 1 : 0;
					}
					break;
				}
			case Sequences.Square:
				{
					if (pointValue == nextNumberInSequence) {
						if (pointValue == 1) {
							pointsAwarded = 0;
							this.seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, this.seqDepth);
						} else {
							pointsAwarded = nextNumberInSequence;
							this.seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, this.seqDepth);
						}
					} else {
						this.nextNumberInSequence = (pointValue == 1) ? 4 : 1; 
						this.seqDepth = (pointValue == 1) ? 1 : 0;
					}
					break;
				}
			case Sequences.Lazy:
				{
					if (pointValue == nextNumberInSequence) {
						if (pointValue == 1) {
							pointsAwarded = 0;
							this.seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, this.seqDepth);
						} else {
							pointsAwarded = nextNumberInSequence;
							this.seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, this.seqDepth);
						}
					} else {
						this.nextNumberInSequence = (pointValue == 1) ? 2 : 1; 
						this.seqDepth = (pointValue == 1) ? 1 : 0;
					}
					break;
				}
			case Sequences.Fibonacci:
				{
					if (pointValue == nextNumberInSequence) {
						if (pointValue == 1 && this.seqDepth == 0) {
							pointsAwarded = 0;
							this.seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, this.seqDepth);
						} else {
							pointsAwarded = nextNumberInSequence;
							this.seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, this.seqDepth);
						}
					} else {
						if (pointValue == 1 && this.seqDepth == 2) {
							this.nextNumberInSequence = 1; 
							this.seqDepth = 1;
						} else {
							this.nextNumberInSequence = 1;
							this.seqDepth = 0;
						}
					}
					break;
				}
			case Sequences.Prime:
				{
					if (pointValue == nextNumberInSequence) {
						if (pointValue == 2) {
							pointsAwarded = 0;
							this.seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, this.seqDepth);
						} else {
							pointsAwarded = nextNumberInSequence;
							this.seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, this.seqDepth);
						}
					} else {
						this.nextNumberInSequence = (pointValue == 2) ? 3 : 2; 
						this.seqDepth = (pointValue == 2) ? 1 : 0;
					}
					break;
				}
			case Sequences.Double:
				{
					if (pointValue == nextNumberInSequence) {
						if (pointValue == 1) {
							pointsAwarded = 0;
							this.seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, this.seqDepth);
						} else {
							pointsAwarded = nextNumberInSequence;
							this.seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, this.seqDepth);
						}
					} else {
						this.nextNumberInSequence = (pointValue == 1) ? 2 : 1; 
						this.seqDepth = (pointValue == 1) ? 1 : 0;
					}
					break;
				}
			case Sequences.Triple:
				{
					if (pointValue == nextNumberInSequence) {
						if (pointValue == 1) {
							pointsAwarded = 0;
							this.seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, this.seqDepth);
						} else {
							pointsAwarded = nextNumberInSequence;
							this.seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, this.seqDepth);
						}
					} else {
						this.nextNumberInSequence = (pointValue == 1) ? 3 : 1; 
						this.seqDepth = (pointValue == 1) ? 1 : 0;
					}
					break;
				}
			case Sequences.Pi:
				{
					if (pointValue == nextNumberInSequence) {
						if (pointValue == 3 && this.seqDepth == 0) {
							pointsAwarded = 0;
							this.seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, this.seqDepth);
						} else {
							pointsAwarded = nextNumberInSequence;
							this.seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, this.seqDepth);
						}
					} else {
						this.nextNumberInSequence = (pointValue == 3) ? 1 : 3; 
						this.seqDepth = (pointValue == 3) ? 1 : 0;
					}
					break;
				}
			case Sequences.Recaman:
				{
					if (pointValue == nextNumberInSequence) {
						if (pointValue == 1 && this.seqDepth == 0) {
							pointsAwarded = 0;
							this.seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, this.seqDepth);
						} else {
							pointsAwarded = nextNumberInSequence;
							this.seqDepth++;
							this.nextNumberInSequence = GetNextSequenceNumber (currentSequence, nextNumberInSequence, this.seqDepth);
						}
					} else {
						this.nextNumberInSequence = (pointValue == 1) ? 3 : 1; 
						this.seqDepth = (pointValue == 1) ? 1 : 0;
					}
					break;
				}
			default:
				break;
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
					//TODO debug when we tap 1 after sequence has started
					nextNumber = listOfRecamans [sequenceDepth];
					break;
				}
			default:
				break;
			}

			return nextNumber;
		}

		//---------------------------------------------------------------------------------------------------------
		// recamanSeq
		//---------------------------------------------------------------------------------------------------------
		// Generate the Recaman's sequence up to x iterations
		//---------------------------------------------------------------------------------------------------------
		public List<int> recamanSeq(int maxValues)
		{
			List<int> sequenceList = new List<int> (new int[] {
				1, 3, 6, 2, 7, 13, 20, 12, 21, 11, 22, 10, 23,
				9, 24, 8, 25, 43, 62, 42, 63, 41, 18, 42, 17, 
				43, 16, 44, 15, 45, 14, 46, 79, 113, 78, 114, 
				77, 39, 78, 38, 79, 37, 80, 36, 81, 35, 82, 34, 
				83, 33, 84, 32, 85, 31, 86, 30, 87, 29, 88, 28, 
				89, 27, 90, 26, 91, 157, 224, 156, 225, 155
			});

			return sequenceList;
		}
	}
}
