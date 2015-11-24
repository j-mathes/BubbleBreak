using System;
using System.Collections.Generic;
using CocosSharp;
//using System.Xml;
//using System.Xml.Serialization;
//using System.Xml.Linq;
//using System.Linq;

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

		const float MIN_TIME_APPEAR = 0.1f;			// minimum amount of time to fade in a bubble
		const float MAX_TIME_APPEAR = 2.0f;			// maximum amount of time to fade in a bubble
		const float MIN_TIME_HOLD = 1.0f;			// minimum amount of time to hold a bubble visible
		const float MAX_TIME_HOLD = 8.0f;			// maximum amount of time to hold a bubble visible
		const float MIN_TIME_FADE = 0.5f;			// minimum amount of time to fade out a bubble
		const float MAX_TIME_FADE = 5.0f;			// maximum amount of time to fade out a bubble

		const float COIN_MULTIPLIER = 0.01f;		// multiplier to determin how many bonus coins the player gets - 100 extra points = 1 coin

		CCNode bubbles;

		bool[] bubbleOccupiedArray;
		bool levelPassed = false;

		Random scoreRandom = new Random();			// randomizer used for getting a new bubble score

		CCPoint tapLocation;
		CCLabel scoreLabel;

		CCSprite timeLabelSprite, scoreLabelSprite, timeSpriteProgressBarEmpty, scoreSpriteProgressBarEmpty;
		CCProgressTimer timeBar, scoreBar;

		Player activePlayer;
		List<Level> levels;
		Level activeLevel;

//		int levelNumber;						// level index number
//		string levelName; 						// level's name
//		string levelDescription;				// level's description
//		int sequenceLevel;						// integer representing which sequences are active on the level
		float levelTimeLimit;		 			// how many second we have to get the levelPassScore
		float levelTimeLeft;					// decreasing time left to complete the level

		int levelTimeIncrement = 0;				// measures the increasing progress of level time.  Used to increment the maximum point value of a new bubble
		int pointIncrementDelay = 40; 			// delay in 10ths of a second to increase the highest random point value generated
		int bubblesVisibleIncrementDelay = 10; 	// delay in 10ths of a second to increase the number of bubbles on the screen

		int levelScore = 0;						// level score
		int levelPassScore;						// how many point we need to pass the level
		int tapsRequired;						// how many taps to pop a standard bubble on this level
		int maxLevelPoints;						// maximum point value for new bubbles.  Initially assigned by the currentLevel property but increased as the level progresses
		int levelVisibleBubbles;				// how many bubbles can currently be visible at a time.  This number increases as the level progresses
		int lastEarnedPoint = 0;				// The value of the last point earned.  Used to determine if we need to increase the maximum point for a new bubble
		int maxBubbles; 						// the maximum amount of bubbles that will fit on the screen at one time.
		int maxVisibleBubbles;					// the limit for how many bubbles we actually want visible on the screen

		//---------------------------------------------------------------------------------------------------------
		// GameLayer
		//---------------------------------------------------------------------------------------------------------
		public LevelLayer (List<Level> gameLevels, Player currentPlayer)
		{
			levels = gameLevels;
			activePlayer = currentPlayer;
			activeLevel = levels [currentPlayer.LastLevelCompleted + 1];

			// assign values from activeLevel properties to the GameLayer properties
			//levelNumber = activeLevel.LevelNum;
			//levelName = activeLevel.LevelName;
			maxBubbles = activeLevel.MaxBubbles;
			maxVisibleBubbles = activeLevel.MaxVisibleBubbles;
			maxLevelPoints = activeLevel.StartingPointValue;
			levelTimeLimit = activeLevel.MaxLevelTime;
			levelPassScore = activeLevel.LevelPassScore;
			tapsRequired = activeLevel.TapsToPopStandard;
			levelVisibleBubbles = activeLevel.InitialVisibleBubbles;
			//levelDescription = activeLevel.LevelDescription;
			//sequenceLevel = activeLevel.SequenceLevel;

			levelTimeLeft = levelTimeLimit;

			bubbleOccupiedArray = new bool[maxBubbles];

			scoreLabel = new CCLabel ("Score:", "arial", 30);
			scoreLabel.Text = "Score: " + levelScore + "/" + levelPassScore;
			scoreLabel.Scale = 1.0f;
			scoreLabel.PositionX = 0;
			scoreLabel.PositionY = 1920;
			scoreLabel.AnchorPoint = CCPoint.AnchorUpperLeft;
			AddChild (scoreLabel);

			timeLabelSprite = new CCSprite ("lbl_time");
			timeLabelSprite.AnchorPoint = CCPoint.AnchorUpperLeft;
			timeLabelSprite.PositionX = 860;
			timeLabelSprite.PositionY = 1870;
			AddChild (timeLabelSprite);

			scoreLabelSprite = new CCSprite ("lbl_score");
			scoreLabelSprite.AnchorPoint = CCPoint.AnchorUpperLeft;
			scoreLabelSprite.PositionX = 860;
			scoreLabelSprite.PositionY = 1810;
			AddChild (scoreLabelSprite);

			timeSpriteProgressBarEmpty = new CCSprite ("prgbar_time_empty");
			timeSpriteProgressBarEmpty.AnchorPoint = CCPoint.AnchorUpperLeft;
			timeSpriteProgressBarEmpty.PositionX = 200;
			timeSpriteProgressBarEmpty.PositionY = 1870;
			AddChild (timeSpriteProgressBarEmpty);

			scoreSpriteProgressBarEmpty = new CCSprite ("prgbar_score_empty");
			scoreSpriteProgressBarEmpty.AnchorPoint = CCPoint.AnchorUpperLeft;
			scoreSpriteProgressBarEmpty.PositionX = 200;
			scoreSpriteProgressBarEmpty.PositionY = 1810;
			AddChild (scoreSpriteProgressBarEmpty);

			timeBar = new CCProgressTimer ("prgbar_time_full");
			timeBar.AnchorPoint = CCPoint.AnchorUpperLeft;
			timeBar.PositionX = 200;
			timeBar.PositionY = 1870;
			timeBar.Percentage = 100;
			timeBar.Midpoint = new CCPoint (0, 0);
			timeBar.BarChangeRate = new CCPoint (1, 0);
			timeBar.Type = CCProgressTimerType.Bar;
			AddChild (timeBar);

			scoreBar = new CCProgressTimer ("prgbar_score_full");
			scoreBar.AnchorPoint = CCPoint.AnchorUpperLeft;
			scoreBar.PositionX = 200;
			scoreBar.PositionY = 1810;
			scoreBar.Midpoint = new CCPoint (0, 0);
			scoreBar.BarChangeRate = new CCPoint (1, 0);
			scoreBar.Type = CCProgressTimerType.Bar;
			AddChild (scoreBar);
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
						activePlayer.Coins += ConvertScoreToCoins(levelScore);
						ContinueToNextLevel();
					} 

					else
					{
						ReplayLevel();
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

			// The initial bubble creation should be refactored into a separate method
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
				CCPoint p = GetRandomPosition ( standardBubble.BubbleSprite.ContentSize * BUBBLE_SCALE, standardBubble.XIndex, standardBubble.YIndex);
				standardBubble.Position = new CCPoint (p.X, p.Y);
				standardBubble.Scale = BUBBLE_SCALE;
				DisplayBubble (standardBubble);
				DisplayLabel (standardBubble);
				bubbles.AddChild (standardBubble);
				bubbleOccupiedArray [bubbleIndex] = true;
			}
				
			// Register for touch events
			var touchListener = new CCEventListenerTouchAllAtOnce ();
			touchListener.OnTouchesEnded = OnTouchesEnded;
			AddEventListener (touchListener, this); 

			Schedule (_ => CheckForFadedBubbles());
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
						standardBubble.TapCount += 1;
						if (standardBubble.CheckPopped ()) {
							bubbleOccupiedArray [standardBubble.ListIndex] = false;
							levelScore += standardBubble.PointValue;
							lastEarnedPoint = standardBubble.PointValue;
							PopBubble (standardBubble);
							scoreLabel.Text = "Score: " + levelScore + "/" + levelPassScore;
							bubblePopped = true;
						}
					}
				}
			}

			// if we found a popped bubble create a new one
			if (bubblePopped) {

				// cycle through BubbleOccupiedArray and make a grab bag of empty indicies
				// randomly select one of the unoccupied indicies
				// add a new bubble to that location
				ShuffleBag<int> newBubbleOrder = new ShuffleBag<int> ();

				for (int i = 0; i < bubbleOccupiedArray.Length; i++) {
					if (!bubbleOccupiedArray [i])
						newBubbleOrder.Add (i);
				}
					
				var newBubbleIndex = newBubbleOrder.Next ();
				var newBubble = new PointBubble (GetRandomScoreValue (scoreRandom, maxLevelPoints), tapsRequired, (newBubbleIndex % MAX_BUBBLES_X), (newBubbleIndex / MAX_BUBBLES_X), newBubbleIndex);
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
				var newBubble = new PointBubble (GetRandomScoreValue (scoreRandom, maxLevelPoints), tapsRequired, (newBubbleIndex % MAX_BUBBLES_X), (newBubbleIndex / MAX_BUBBLES_X), newBubbleIndex);
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
		// GetRandomPointValue
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
			await newBubble.BubbleSprite.RunActionsAsync (new CCFadeIn (newBubble.TimeAppear), new CCDelayTime (newBubble.TimeHold), new CCFadeOut (newBubble.TimeFade));
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
			await newBubble.PointLabel.RunActionsAsync (new CCFadeIn (newBubble.TimeAppear), new CCDelayTime (newBubble.TimeHold), new CCFadeOut (newBubble.TimeFade));
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
		// EndLevelCallBack
		//---------------------------------------------------------------------------------------------------------
		// for testing the delay after showing the game over layer
		//---------------------------------------------------------------------------------------------------------
		void EndLevelCallBack()
		{
			UnscheduleAll();

			var levelEndScene = LevelFinishedLayer.CreateScene (Window, levelScore, levels, activePlayer, levelPassed);
			var transitionToLevelOver = new CCTransitionFade (3.0f, levelEndScene);

			Director.ReplaceScene (transitionToLevelOver);
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

			EndLevelCallBack ();
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
		// ContinueToNextLevel
		//---------------------------------------------------------------------------------------------------------
		// Asks player if they want to continue or go back to the main menu
		//---------------------------------------------------------------------------------------------------------
		public void ContinueToNextLevel()
		{
			EndLevel ();
		}

		//---------------------------------------------------------------------------------------------------------
		// ReplayLevel
		//---------------------------------------------------------------------------------------------------------
		// Asks the player if they want to replay the level  or go back to the main menu
		//---------------------------------------------------------------------------------------------------------
		public void ReplayLevel()
		{
			EndLevel ();
		}
	}
}
