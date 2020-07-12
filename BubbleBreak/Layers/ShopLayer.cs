// -----------------------------------------------------------------------------------------------
//  <copyright file="ShopLayer.cs" company="RetroTek Software Ltd">
//      Copyright (C) 2016 RetroTek Software Ltd. All rights reserved.
//  </copyright>
//  <author>Jared Mathes</author>
// -----------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using CocosSharp;
using CoreGraphics;
using iAd;
using UIKit;

namespace BubbleBreak
{
	//---------------------------------------------------------------------------------------------------------
	// ShopLayer
	//---------------------------------------------------------------------------------------------------------
	// This class will create the layer that allows a player to purchase items with the coins they earned 
	// while playing the game
	//---------------------------------------------------------------------------------------------------------
	public class ShopLayer : CCLayerColor
	{			
		const string GOTHIC_30_HD_FNT = "gothic-30-hd.fnt";
		const string GOTHIC_44_HD_FNT = "gothic-44-hd.fnt";
		const string GOTHIC_56_WHITE_HD_FNT = "gothic-56-white-hd.fnt";
		const string GOTHIC_56_WHITE_FNT = "gothic-56-white.fnt";

		const string STARS = "stars";

		const int CHECKPOINT_BASE = 250;
		const int ADDITIONS_BASE = 100;
		const int SUBTRACTIONS_BASE = 50;
		const int NEXT_IN_SEQUENCE_BASE = 500;
		const int PERSISTENT_TAP_STRENGTH_BASE = 1000;
		const int PERSISTENT_TIME_BONUS_BASE = 1000;  // this should ramp up quickly the more they buy
		const int PERSISTENT_2X_TIME_BONUS_BASE = 500;
		const int PERSISTENT_3X_TIME_BONUS_BASE = 750;
		const int LUCKY_POINT_CHANCE_BASE = 500;
		const int LUCKY_POINT_BONUS_BASE = 500;
		const int HIGHLIGHT_NEXT_BASE = 25; // 5000;
		const int MED_KIT_ACTIVATE_BASE = 100;
		const int MED_KIT_UPGRADE_BASE = 75;

		CCSpriteSheet uiSpriteSheet;
		CCLabel shopLabel;
		CCLabel backLabel;
		CCLabel availableCoinsAmountLabel;
		CCLabel persistentTapStrengthAmountLabel;
		CCLabel availableCheckpointsAmountLabel;
		CCLabel availableAdditionsAmountLabel;
		CCLabel availableSubtractionsAmountLabel;
		CCLabel availableNextInSequenceAmountLabel;
		CCLabel persistentTimeBonusAmountLabel;
		CCLabel persistent2xTimeBonusAmountLabel;
		CCLabel persistent3xTimeBonusAmountLabel;
		CCLabel persistentLuckyPointChanceAmountLabel;
		CCLabel persistentLuckyPointBonusAmountLabel;
		CCLabel medKitUpgradeAmountLabel;
		CCLabel medKitActivateAmountLabel;
		CCLabel highlightNextAmountLabel;

		//TODO: change all of these to buy and sell sprites
		CCLabel buyCheckpointItemLabel;
		CCLabel buyAdditionItemLabel;
		CCLabel buySubtractionItemLabel;
		CCLabel buyNextInSequenceItemLabel;
		CCLabel buyPersistentTapStrengthItemLabel;
		CCLabel buyPersistentTimeBonusItemLabel;
		CCLabel buyPersistent2xTimeBonusItemLabel;
		CCLabel	buyPersistent3xTimeBonusItemLabel;
		CCLabel buyPersistentLuckyPointChanceItemLabel;
		CCLabel buyPersistentLuckyPointBonusItemLabel;
		CCLabel buyMedKitUpgradeItemLabel;
		CCLabel buyMedKitActivateItemLabel;
		CCLabel buyHighlightNextItemLabel;
		CCLabel sellCheckpointItemLabel;
		CCLabel sellAdditionItemLabel;
		CCLabel sellSubtractionItemLabel;
		CCLabel sellNextInSequenceItemLabel;


		CCSprite coinsSprite;
		CCSprite checkpointsSprite;
		CCSprite additionsSprite;
		CCSprite subtractionsSprite;
		CCSprite nextInSequenceSprite;
		CCSprite persistentTapStrengthSprite;
		CCSprite persistentTimeBonusSprite;
		CCSprite persistent2xTimeBonusSprite;
		CCSprite persistent3xTimeBonusSprite;
		CCSprite persistentLuckyPointChanceSprite;
		CCSprite persistentLuckyPointBonusSprite;
		CCSprite medKitUpgradeSprite;
		CCSprite medKitActivateSprite;
		CCSprite highlightNextSprite;

		CCMenu buyCheckpointsMenu;
		CCMenu buyAdditionsMenu;
		CCMenu buySubtractionsMenu;
		CCMenu buyNextInSequenceMenu;
		CCMenu buyPersistentTapStrengthMenu;
		CCMenu buyPersistentTimeBonusMenu;
		CCMenu buyPersistent2xTimeBonusMenu;
		CCMenu buyPersistent3xTimeBonusMenu;
		CCMenu buyPersistentLuckyPointChanceMenu;
		CCMenu buyPersistentLuckyPointBonusMenu;
		CCMenu buyMedKitUpgradeMenu;
		CCMenu buyMedKitActivateMenu;
		CCMenu buyHighlightNextMenu;
		CCMenu sellCheckpointsMenu;
		CCMenu sellAdditionsMenu;
		CCMenu sellSubtractionsMenu;
		CCMenu sellNextInSequenceMenu;


//		readonly ADBannerView adBannerView;
//		UIViewController topController;

		Player activePlayer;

		int availableCoins;
		int availableCheckpoints;
		int availableAdditions;
		int availableSubtractions;
		int availableNextInSequence;
		int persistentTapStrength;
		int persistentTimeBonus;
		int persistent2xTimeBonus;
		int persistent3xTimeBonus;
		int persistentLuckyPointChance;
		int persistentLuckyPointBonus;
		int medKitUpgradeLevel;
		bool isMedKitActive;
		bool isHighlightNextOn;

		int checkpointBuyPrice;
		int additionsBuyPrice;
		int subtractionsBuyPrice;
		int nextInSequenceBuyPrice;
		int persistentTapStrengthPrice;
		int persistentTimeBonusPrice;
		int persistent2xTimeBonusPrice;
		int persistent3xTimeBonusPrice;
		int persistentLuckyPointChancePrice;
		int persistentLuckyPointBonusPrice;
		int medKitUpgradePrice;
		int medKitActivatePrice;
		int highlightNextPrice;
		int checkpointSellPrice;
		int additionsSellPrice;
		int subtractionsSellPrice;
		int nextInSequenceSellPrice;

		CCLabel checkpointBuyPriceLabel;
		CCLabel additionsBuyPriceLabel;
		CCLabel subtractionsBuyPriceLabel;
		CCLabel nextInSequenceBuyPriceLabel;
		CCLabel persistentTapStrengthPriceLabel;
		CCLabel persistentTimeBonusPriceLabel;
		CCLabel persistent2xTimeBonusPriceLabel;
		CCLabel persistent3xTimeBonusPriceLabel;
		CCLabel persistentLuckyPointChancePriceLabel;
		CCLabel persistentLuckyPointBonusPriceLabel;
		CCLabel medKitUpgradePriceLabel;
		CCLabel medKitActivatePriceLabel;
		CCLabel highlightNextPriceLabel;
		CCLabel checkpointSellPriceLabel;
		CCLabel additionsSellPriceLabel;
		CCLabel subtractionsSellPriceLabel;
		CCLabel nextInSequenceSellPriceLabel;

		CCParticleSystemQuad emitter;

		CCRepeatForever repeatedAction;

		//---------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="BubbleBreak.ShopLayer"/> class.
		/// </summary>
		//---------------------------------------------------------------------------------------------------------
		public ShopLayer (Player currentPlayer)
		{
//			topController = UIApplication.SharedApplication.KeyWindow.RootViewController;
//			adBannerView = new ADBannerView (new CGRect (0, 518, 320, 50));

			activePlayer = currentPlayer;

			GetPlayerConsumableData (activePlayer);
			CalculatePrices ();

			// Define Actions

			var inflate = new CCScaleBy (0.7f, 1.1f);
			var deflate = new CCScaleBy (4.0f, 0.9f);

			//Define Sequence of actions

			var actionSeq = new CCSequence (new CCEaseElasticIn (inflate), new CCEaseExponentialOut(deflate), new CCDelayTime (CCRandom.GetRandomFloat (5.0f, 12.0f)));

			repeatedAction = new CCRepeatForever (actionSeq);
		}

		//---------------------------------------------------------------------------------------------------------
		// GetPlayerConsumableData
		//---------------------------------------------------------------------------------------------------------
		// Reads in the amoutn of consumables and coins the player has
		//---------------------------------------------------------------------------------------------------------
		void GetPlayerConsumableData (Player currentPlayer)
		{
			activePlayer = currentPlayer;
			availableCoins = activePlayer.Coins;
			//availableCoins += 10000;
			availableCheckpoints = activePlayer.ConsumableCheckPoint;
			availableAdditions = activePlayer.ConsumableAdd;
			availableSubtractions = activePlayer.ConsumableSubtract;
			availableNextInSequence = activePlayer.ConsumableNextSeq;
			persistentTapStrength = activePlayer.PersistentTapStrength;
			persistentTimeBonus = activePlayer.PersistentTimeBonus;
			persistent2xTimeBonus = activePlayer.Persistent2xTimeBonus;
			persistent3xTimeBonus = activePlayer.Persistent3xTimeBonus;
			persistentLuckyPointChance = activePlayer.LuckyPointChance;
			persistentLuckyPointBonus = activePlayer.LuckyPointBonus;
			medKitUpgradeLevel = activePlayer.MedKitUpgradeLevel;
			isMedKitActive = activePlayer.IsMedKitActivated;
			isHighlightNextOn = activePlayer.HighlightNextPurchased;
		}

		//---------------------------------------------------------------------------------------------------------
		protected override void AddedToScene ()
		{
			base.AddedToScene ();

			// Use the bounds to layout the positioning of our drawable assets
			CCRect bounds = VisibleBoundsWorldspace;

			// Register for touch events
			var touchListener = new CCEventListenerTouchAllAtOnce ();
			touchListener.OnTouchesEnded = OnTouchesEnded;
			AddEventListener (touchListener, this); 

			SetupUI (bounds);
			ActivateMenuSelections ();

			//topController.View.AddSubview (adBannerView);
		}

		//---------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Creates the scene.
		/// </summary>
		/// <returns>The scene.</returns>
		public static CCScene CreateScene (CCWindow mainWindow, Player activePlayer)
		{
			var scene = new CCScene (mainWindow);
			var layer = new ShopLayer (activePlayer);

			scene.AddChild (layer);

			return scene;
		} 

		//---------------------------------------------------------------------------------------------------------
		// OnTouchesEnded
		//---------------------------------------------------------------------------------------------------------
		void OnTouchesEnded (IList<CCTouch> touches, CCEvent touchEvent)
		{

		}

		//---------------------------------------------------------------------------------------------------------
		// BackToGameSelect
		//---------------------------------------------------------------------------------------------------------
		// Returns to Game Select
		//---------------------------------------------------------------------------------------------------------
		void BackToGameSelect (object stuff = null)
		{
			//adBannerView.Hidden = true;

			var layer = GameSelectLayer.CreateScene (Window, activePlayer);
			var transitionToLayer = new CCTransitionSlideInL(0.2f, layer);
			Director.ReplaceScene (transitionToLayer);
		}

		//---------------------------------------------------------------------------------------------------------
		// SetupUI
		//---------------------------------------------------------------------------------------------------------
		// Sets up the game UI
		//TODO: Use plus and minus sprites rather than text (sprite_std_buy / sprite_std_sell)
		//---------------------------------------------------------------------------------------------------------
		void SetupUI (CCRect bounds)
		{
			uiSpriteSheet = new CCSpriteSheet ("ui.plist");

			shopLabel = new CCLabel ("Shop", GOTHIC_56_WHITE_HD_FNT) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 1.5f,
				PositionX = bounds.MidX,
				PositionY = 1860,
			};
			AddChild (shopLabel);

			var coinsTitle = string.Format ("{0}", availableCoins);

			availableCoinsAmountLabel = new CCLabel (coinsTitle, GOTHIC_56_WHITE_HD_FNT) {
				AnchorPoint = CCPoint.AnchorMiddle,
				//Scale = 1.0f,
				PositionX = bounds.MidX,
				PositionY = 1760,
			};
			AddChild (availableCoinsAmountLabel);

			coinsSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("coins_small.png"))) {
				AnchorPoint = CCPoint.AnchorMiddle,
				Scale = 1.0f,
				PositionX = availableCoinsAmountLabel.BoundingBox.MinX - 150, //TODO: this jumps around because at this point, the bounding box doesn't have anything in it so it's really small
				PositionY = availableCoinsAmountLabel.PositionY
			};
			AddChild (coinsSprite);

			checkpointsSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("sprite_std_checkpoint.png")));
			additionsSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("sprite_std_addition.png")));
			subtractionsSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("sprite_std_subtraction.png")));
			nextInSequenceSprite = new CCSprite(uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("sprite_std_nextinseq.png")));
			persistentTapStrengthSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("tap_strength_std.png")));
			persistentTimeBonusSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("sprite_std_persistent_time_bonus.png")));
			persistent2xTimeBonusSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("sprite_std_persistent_2x_time_bonus.png")));
			persistent3xTimeBonusSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("sprite_std_persistent_3x_time_bonus.png")));
			persistentLuckyPointChanceSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("sprite_std_lucky_chance.png")));
			persistentLuckyPointBonusSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("sprite_std_lucky_bonus.png")));
			medKitUpgradeSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("sprite_std_med_kit.png")));
			medKitActivateSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("sprite_std_med_kit.png")));
			highlightNextSprite = new CCSprite (uiSpriteSheet.Frames.Find (x => x.TextureFilename.Equals ("sprite_std_nextinseq.png")));

			var shopMenuSprites = new List<CCSprite> ();

			shopMenuSprites.Add (checkpointsSprite);
			shopMenuSprites.Add (additionsSprite);
			shopMenuSprites.Add (subtractionsSprite);
			shopMenuSprites.Add (nextInSequenceSprite);
			shopMenuSprites.Add (persistentTapStrengthSprite);
			shopMenuSprites.Add (persistentTimeBonusSprite);
			shopMenuSprites.Add (persistent2xTimeBonusSprite);
			shopMenuSprites.Add (persistent3xTimeBonusSprite); 
			shopMenuSprites.Add (persistentLuckyPointChanceSprite);
			shopMenuSprites.Add (persistentLuckyPointBonusSprite);
			shopMenuSprites.Add (medKitUpgradeSprite);
			shopMenuSprites.Add (medKitActivateSprite);
			shopMenuSprites.Add (highlightNextSprite); 

			for (int i = 0; i < shopMenuSprites.Count; i++) {
				shopMenuSprites [i].AnchorPoint = CCPoint.AnchorLowerLeft;
				shopMenuSprites [i].IgnoreAnchorPointForPosition = false;
				shopMenuSprites [i].PositionX = 180;
				shopMenuSprites [i].PositionY = 1650 - (i * 135);
				AddChild (shopMenuSprites [i], 100);
			}

			emitter = new CCParticleFlower (highlightNextSprite.BoundingBox.Center) {
				Texture = CCTextureCache.SharedTextureCache.AddImage ("stars"),
				TotalParticles = 50,
			};
			AddChild (emitter);

			checkpointBuyPriceLabel = new CCLabel (string.Format ("{0}", checkpointBuyPrice), GOTHIC_56_WHITE_HD_FNT);
			additionsBuyPriceLabel = new CCLabel (string.Format ("{0}", additionsBuyPrice), GOTHIC_56_WHITE_HD_FNT);
			subtractionsBuyPriceLabel = new CCLabel (string.Format ("{0}", subtractionsBuyPrice), GOTHIC_56_WHITE_HD_FNT);
			nextInSequenceBuyPriceLabel = new CCLabel (string.Format ("{0}", nextInSequenceBuyPrice), GOTHIC_56_WHITE_HD_FNT);
			persistentTapStrengthPriceLabel = new CCLabel (string.Format ("{0}", persistentTapStrengthPrice), GOTHIC_56_WHITE_HD_FNT);
			persistentTimeBonusPriceLabel = new CCLabel (string.Format ("{0}", persistentTimeBonusPrice), GOTHIC_56_WHITE_HD_FNT);
			persistent2xTimeBonusPriceLabel = new CCLabel (string.Format ("{0}", persistent2xTimeBonusPrice), GOTHIC_56_WHITE_HD_FNT);
			persistent3xTimeBonusPriceLabel = new CCLabel (string.Format ("{0}", persistent3xTimeBonusPrice), GOTHIC_56_WHITE_HD_FNT);
			persistentLuckyPointChancePriceLabel = new CCLabel (string.Format ("{0}", persistentLuckyPointChancePrice), GOTHIC_56_WHITE_HD_FNT);
			persistentLuckyPointBonusPriceLabel = new CCLabel (string.Format ("{0}", persistentLuckyPointBonusPrice), GOTHIC_56_WHITE_HD_FNT);
			medKitUpgradePriceLabel = new CCLabel (string.Format ("{0}", medKitUpgradePrice), GOTHIC_56_WHITE_HD_FNT); 
			medKitActivatePriceLabel = new CCLabel (string.Format ("{0}", medKitActivatePrice), GOTHIC_56_WHITE_HD_FNT); 
			highlightNextPriceLabel = new CCLabel (string.Format ("{0}", highlightNextPrice), GOTHIC_56_WHITE_HD_FNT);

			var buyPriceLabels = new List<CCLabel> ();

			buyPriceLabels.Add (checkpointBuyPriceLabel);
			buyPriceLabels.Add (additionsBuyPriceLabel);
			buyPriceLabels.Add (subtractionsBuyPriceLabel);
			buyPriceLabels.Add (nextInSequenceBuyPriceLabel);
			buyPriceLabels.Add (persistentTapStrengthPriceLabel);
			buyPriceLabels.Add (persistentTimeBonusPriceLabel);
			buyPriceLabels.Add (persistent2xTimeBonusPriceLabel);
			buyPriceLabels.Add (persistent3xTimeBonusPriceLabel); 
			buyPriceLabels.Add (persistentLuckyPointChancePriceLabel);
			buyPriceLabels.Add (persistentLuckyPointBonusPriceLabel);
			buyPriceLabels.Add (medKitUpgradePriceLabel);
			buyPriceLabels.Add (medKitActivatePriceLabel);
			buyPriceLabels.Add (highlightNextPriceLabel);

			for (int i = 0; i < buyPriceLabels.Count; i++) {
				buyPriceLabels [i].AnchorPoint = CCPoint.AnchorLowerLeft;
				buyPriceLabels [i].IgnoreAnchorPointForPosition = false;
				buyPriceLabels [i].PositionX = shopMenuSprites [i].PositionX + 140;
				buyPriceLabels [i].PositionY = shopMenuSprites [i].PositionY;
				//priceLabels [i].Scale = 1.5f;
				AddChild (buyPriceLabels [i]); 
			}

			checkpointSellPriceLabel = new CCLabel (string.Format ("{0}", checkpointSellPrice), GOTHIC_56_WHITE_HD_FNT);
			additionsSellPriceLabel = new CCLabel (string.Format ("{0}", additionsSellPrice), GOTHIC_56_WHITE_HD_FNT);
			subtractionsSellPriceLabel = new CCLabel (string.Format ("{0}", subtractionsSellPrice), GOTHIC_56_WHITE_HD_FNT);
			nextInSequenceSellPriceLabel = new CCLabel (string.Format ("{0}", nextInSequenceSellPrice), GOTHIC_56_WHITE_HD_FNT);

			var sellPriceLabels = new List<CCLabel> ();

			sellPriceLabels.Add (checkpointSellPriceLabel);
			sellPriceLabels.Add (additionsSellPriceLabel);
			sellPriceLabels.Add (subtractionsSellPriceLabel);
			sellPriceLabels.Add (nextInSequenceSellPriceLabel);

			for (int i = 0; i < sellPriceLabels.Count; i++) {
				sellPriceLabels [i].AnchorPoint = CCPoint.AnchorLowerLeft;
				sellPriceLabels [i].IgnoreAnchorPointForPosition = false;
				sellPriceLabels [i].PositionX = shopMenuSprites [i].PositionX + 650;
				sellPriceLabels [i].PositionY = shopMenuSprites [i].PositionY;
				//priceLabels [i].Scale = 1.5f;
				AddChild (sellPriceLabels [i]); 
			}
				
			availableCheckpointsAmountLabel = new CCLabel (string.Format ("{0}", availableCheckpoints), GOTHIC_56_WHITE_FNT);
			availableAdditionsAmountLabel = new CCLabel (string.Format ("{0}", availableAdditions), GOTHIC_56_WHITE_FNT);
			availableSubtractionsAmountLabel = new CCLabel (string.Format ("{0}", availableSubtractions), GOTHIC_56_WHITE_FNT);
			availableNextInSequenceAmountLabel = new CCLabel (string.Format ("{0}", availableNextInSequence), GOTHIC_56_WHITE_FNT);
			persistentTapStrengthAmountLabel = new CCLabel (string.Format ("{0}", persistentTapStrength), GOTHIC_56_WHITE_FNT);
			persistentTimeBonusAmountLabel = new CCLabel (string.Format ("{0}", persistentTimeBonus), GOTHIC_56_WHITE_FNT);
			persistent2xTimeBonusAmountLabel = new CCLabel (string.Format ("{0}", persistent2xTimeBonus), GOTHIC_56_WHITE_FNT);
			persistent3xTimeBonusAmountLabel = new CCLabel (string.Format ("{0}", persistent3xTimeBonus), GOTHIC_56_WHITE_FNT);
			persistentLuckyPointChanceAmountLabel = new CCLabel (string.Format ("{0}", persistentLuckyPointChance), GOTHIC_56_WHITE_FNT);
			persistentLuckyPointBonusAmountLabel = new CCLabel (string.Format ("{0}", persistentLuckyPointBonus), GOTHIC_56_WHITE_FNT);
			medKitUpgradeAmountLabel = new CCLabel (string.Format ("{0}", medKitUpgradeLevel), GOTHIC_56_WHITE_FNT);
			medKitActivateAmountLabel = new CCLabel ((isMedKitActive) ? "Yes" : "No", GOTHIC_56_WHITE_FNT);
			highlightNextAmountLabel = new CCLabel ((isHighlightNextOn) ? "Yes" : "No", GOTHIC_56_WHITE_FNT);

			var availableAmountLabels = new List<CCLabel> ();

			availableAmountLabels.Add (availableCheckpointsAmountLabel);
			availableAmountLabels.Add (availableAdditionsAmountLabel);
			availableAmountLabels.Add (availableSubtractionsAmountLabel);
			availableAmountLabels.Add (availableNextInSequenceAmountLabel);
			availableAmountLabels.Add (persistentTapStrengthAmountLabel);
			availableAmountLabels.Add (persistentTimeBonusAmountLabel);
			availableAmountLabels.Add (persistent2xTimeBonusAmountLabel);
			availableAmountLabels.Add (persistent3xTimeBonusAmountLabel);
			availableAmountLabels.Add (persistentLuckyPointChanceAmountLabel);
			availableAmountLabels.Add (persistentLuckyPointBonusAmountLabel);
			availableAmountLabels.Add (medKitUpgradeAmountLabel);
			availableAmountLabels.Add (medKitActivateAmountLabel);
			availableAmountLabels.Add (highlightNextAmountLabel);

			for (int i = 0; i < availableAmountLabels.Count; i++) {
				availableAmountLabels [i].AnchorPoint = CCPoint.AnchorLowerRight;
				availableAmountLabels [i].IgnoreAnchorPointForPosition = false;
				availableAmountLabels [i].PositionX = shopMenuSprites [i].PositionX - 30;
				availableAmountLabels [i].PositionY = shopMenuSprites [i].PositionY;
				availableAmountLabels [i].Scale = 1.3f;
				availableAmountLabels [i].Color = CCColor3B.Green;
				AddChild (availableAmountLabels [i]); 
			}
				
			buyCheckpointItemLabel = new CCLabel ("+", GOTHIC_44_HD_FNT); 
			buyAdditionItemLabel = new CCLabel ("+", GOTHIC_44_HD_FNT); 
			buySubtractionItemLabel = new CCLabel ("+", GOTHIC_44_HD_FNT); 
			buyNextInSequenceItemLabel = new CCLabel ("+", GOTHIC_44_HD_FNT); 
			buyPersistentTapStrengthItemLabel = new CCLabel ("+", GOTHIC_44_HD_FNT); 
			buyPersistentTimeBonusItemLabel = new CCLabel ("+", GOTHIC_44_HD_FNT);
			buyPersistent2xTimeBonusItemLabel = new CCLabel ("+", GOTHIC_44_HD_FNT);
			buyPersistent3xTimeBonusItemLabel = new CCLabel ("+", GOTHIC_44_HD_FNT);
			buyPersistentLuckyPointChanceItemLabel = new CCLabel ("+", GOTHIC_44_HD_FNT);
			buyPersistentLuckyPointBonusItemLabel = new CCLabel ("+", GOTHIC_44_HD_FNT);
			buyMedKitUpgradeItemLabel = new CCLabel ("+", GOTHIC_44_HD_FNT);
			buyMedKitActivateItemLabel = new CCLabel  ("+", GOTHIC_44_HD_FNT);
			buyHighlightNextItemLabel = new CCLabel ("+", GOTHIC_44_HD_FNT);

			sellCheckpointItemLabel = new CCLabel ("-", GOTHIC_44_HD_FNT); 
			sellAdditionItemLabel = new CCLabel ("-", GOTHIC_44_HD_FNT); 
			sellSubtractionItemLabel = new CCLabel ("-", GOTHIC_44_HD_FNT); 
			sellNextInSequenceItemLabel = new CCLabel ("-", GOTHIC_44_HD_FNT); 

			var buyCheckpointsMenuItem = new CCMenuItemLabel (buyCheckpointItemLabel, BuyCheckpoint);
			var buyAdditionsMenuItem = new CCMenuItemLabel (buyAdditionItemLabel, BuyAddition);
			var buySubtractionsMenuItem = new CCMenuItemLabel (buySubtractionItemLabel, BuySubtraction);
			var buyNextInSequenceMenuItem = new CCMenuItemLabel (buyNextInSequenceItemLabel, BuyNextInSequence);
			var buyTapStrengthMenuItem = new CCMenuItemLabel (buyPersistentTapStrengthItemLabel, BuyTapStrength);
			var buyPersistentTimeBonusMenuItem = new CCMenuItemLabel (buyPersistentTimeBonusItemLabel, BuyPersistentTimeBonus);
			var buyPersistent2xTimeBonusMenuItem = new CCMenuItemLabel (buyPersistent2xTimeBonusItemLabel, BuyPersistent2xTimeBonus);
			var buyPersistent3xTimeBonusMenuItem = new CCMenuItemLabel (buyPersistent3xTimeBonusItemLabel, BuyPersistent3xTimeBonus);
			var buyPersistentLuckyPointChanceMenuItem = new CCMenuItemLabel (buyPersistentLuckyPointChanceItemLabel, BuyPersistentLuckyPointChance);
			var buyPersistentLuckyPointBonusMenuItem = new CCMenuItemLabel (buyPersistentLuckyPointBonusItemLabel, BuyPersistentLuckyPointBonus);
			var buyMedKitUpgradeMenuItem = new CCMenuItemLabel (buyMedKitUpgradeItemLabel, BuyMedKitUpgrade);
			var buyMedKitActivateMenuItem = new CCMenuItemLabel (buyMedKitActivateItemLabel, BuyMedKitActivate);
			var buyHighlightNextMenuItem = new CCMenuItemLabel (buyHighlightNextItemLabel, BuyHighlightNext);

			var sellCheckpointsMenuItem = new CCMenuItemLabel (sellCheckpointItemLabel, SellCheckpoint);
			var sellAdditionsMenuItem = new CCMenuItemLabel (sellAdditionItemLabel, SellAddition);
			var sellSubtractionsMenuItem = new CCMenuItemLabel (sellSubtractionItemLabel, SellSubtraction);
			var sellNextInSequenceMenuItem = new CCMenuItemLabel (sellNextInSequenceItemLabel, SellNextInSequence);

			buyCheckpointsMenu = new CCMenu (buyCheckpointsMenuItem);
			buyAdditionsMenu = new CCMenu (buyAdditionsMenuItem);
			buySubtractionsMenu = new CCMenu (buySubtractionsMenuItem);
			buyNextInSequenceMenu = new CCMenu (buyNextInSequenceMenuItem);
			buyPersistentTapStrengthMenu = new CCMenu (buyTapStrengthMenuItem);
			buyPersistentTimeBonusMenu = new CCMenu (buyPersistentTimeBonusMenuItem);
			buyPersistent2xTimeBonusMenu = new CCMenu (buyPersistent2xTimeBonusMenuItem);
			buyPersistent3xTimeBonusMenu = new CCMenu (buyPersistent3xTimeBonusMenuItem);
			buyPersistentLuckyPointChanceMenu = new CCMenu (buyPersistentLuckyPointChanceMenuItem);
			buyPersistentLuckyPointBonusMenu = new CCMenu (buyPersistentLuckyPointBonusMenuItem);
			buyMedKitUpgradeMenu = new CCMenu (buyMedKitUpgradeMenuItem);
			buyMedKitActivateMenu = new CCMenu (buyMedKitActivateMenuItem);
			buyHighlightNextMenu = new CCMenu (buyHighlightNextMenuItem);

			sellCheckpointsMenu = new CCMenu (sellCheckpointsMenuItem);
			sellAdditionsMenu = new CCMenu (sellAdditionsMenuItem);
			sellSubtractionsMenu = new CCMenu (sellSubtractionsMenuItem);
			sellNextInSequenceMenu = new CCMenu (sellNextInSequenceMenuItem);

			var buyMenus = new List<CCMenu> ();

			buyMenus.Add (buyCheckpointsMenu);
			buyMenus.Add (buyAdditionsMenu);
			buyMenus.Add (buySubtractionsMenu);
			buyMenus.Add (buyNextInSequenceMenu);
			buyMenus.Add (buyPersistentTapStrengthMenu);
			buyMenus.Add (buyPersistentTimeBonusMenu);
			buyMenus.Add (buyPersistent2xTimeBonusMenu);
			buyMenus.Add (buyPersistent3xTimeBonusMenu);
			buyMenus.Add (buyPersistentLuckyPointChanceMenu);
			buyMenus.Add (buyPersistentLuckyPointBonusMenu);
			buyMenus.Add (buyMedKitUpgradeMenu);
			buyMenus.Add (buyMedKitActivateMenu);
			buyMenus.Add (buyHighlightNextMenu);

			for (int i = 0; i < buyMenus.Count; i++) {
				buyMenus [i].AnchorPoint = CCPoint.AnchorLowerLeft;
				buyMenus [i].IgnoreAnchorPointForPosition = false;
				buyMenus [i].PositionX = shopMenuSprites [i].PositionX + 400;
				buyMenus [i].PositionY = shopMenuSprites [i].PositionY + 35;
				buyMenus [i].Scale = 1.5f;
				AddChild (buyMenus [i]);
			}

			var sellMenus = new List<CCMenu> ();

			sellMenus.Add (sellCheckpointsMenu);
			sellMenus.Add (sellAdditionsMenu);
			sellMenus.Add (sellSubtractionsMenu);
			sellMenus.Add (sellNextInSequenceMenu);

			for (int i = 0; i < sellMenus.Count; i++) {
				sellMenus [i].AnchorPoint = CCPoint.AnchorLowerLeft;
				sellMenus [i].IgnoreAnchorPointForPosition = false;
				sellMenus [i].PositionX = shopMenuSprites [i].PositionX + 550;
				sellMenus [i].PositionY = shopMenuSprites [i].PositionY + 35;
				sellMenus [i].Scale = 1.5f;
				AddChild (sellMenus [i]);
			}

			backLabel = new CCLabel ("Back", GOTHIC_44_HD_FNT) {
			AnchorPoint = CCPoint.AnchorMiddle,
			Scale = 1.5f,
			};

			var backItem = new CCMenuItemLabel (backLabel, BackToGameSelect);
			//returnItem.Position = bounds.Center;  RESEARCH: using this creates an offset.  Maybe this is why we have strange positioning for the sequences menu in the LevelLayer

			var backMenu = new CCMenu (backItem);
			backMenu.AnchorPoint = CCPoint.AnchorMiddleBottom;
			backMenu.Position = new CCPoint ((bounds.Size.Width / 4) * 3, 220f);

			AddChild (backMenu);
		}

		//---------------------------------------------------------------------------------------------------------
		// CalculatePrices
		//---------------------------------------------------------------------------------------------------------
		// Prices can change depending on how many of a consumable the player has in inventory.  This method will 
		// update prices depending on inventory level
		//---------------------------------------------------------------------------------------------------------
		void CalculatePrices()
		{
			checkpointBuyPrice = (availableCheckpoints == 0) ? CHECKPOINT_BASE : CHECKPOINT_BASE + (CHECKPOINT_BASE * availableCheckpoints);
			additionsBuyPrice = (availableAdditions == 0) ? ADDITIONS_BASE : ADDITIONS_BASE + (ADDITIONS_BASE * availableAdditions);
			subtractionsBuyPrice = (availableSubtractions == 0) ? SUBTRACTIONS_BASE : SUBTRACTIONS_BASE + (SUBTRACTIONS_BASE * availableSubtractions);
			nextInSequenceBuyPrice = ((availableNextInSequence == 0) ? NEXT_IN_SEQUENCE_BASE : NEXT_IN_SEQUENCE_BASE * availableNextInSequence * 2);
			persistentTapStrengthPrice = PERSISTENT_TAP_STRENGTH_BASE + (PERSISTENT_TAP_STRENGTH_BASE * persistentTapStrength);
			persistentTimeBonusPrice = PERSISTENT_TIME_BONUS_BASE + (PERSISTENT_TIME_BONUS_BASE * persistentTimeBonus);
			persistent2xTimeBonusPrice = PERSISTENT_2X_TIME_BONUS_BASE + (PERSISTENT_2X_TIME_BONUS_BASE * persistent2xTimeBonus);
			persistent3xTimeBonusPrice = PERSISTENT_3X_TIME_BONUS_BASE + (PERSISTENT_2X_TIME_BONUS_BASE * persistent3xTimeBonus);
			persistentLuckyPointChancePrice = LUCKY_POINT_CHANCE_BASE + (LUCKY_POINT_CHANCE_BASE * persistentLuckyPointChance);
			persistentLuckyPointBonusPrice = LUCKY_POINT_BONUS_BASE + (LUCKY_POINT_BONUS_BASE * persistentLuckyPointBonus);
			medKitUpgradePrice = MED_KIT_UPGRADE_BASE + ((MED_KIT_UPGRADE_BASE * 2) * medKitUpgradeLevel);
			medKitActivatePrice = MED_KIT_ACTIVATE_BASE;
			highlightNextPrice = HIGHLIGHT_NEXT_BASE;
			checkpointSellPrice = (int)(CHECKPOINT_BASE * 0.5);
			additionsSellPrice = (int)(ADDITIONS_BASE * 0.5);
			subtractionsSellPrice = (int)(SUBTRACTIONS_BASE * 0.5);
			nextInSequenceSellPrice = (int)(NEXT_IN_SEQUENCE_BASE * 0.5);
		}

		//---------------------------------------------------------------------------------------------------------
		// ActivateMenuSelections
		//---------------------------------------------------------------------------------------------------------
		// Determin if the menu selections should be active or not
		//---------------------------------------------------------------------------------------------------------
		void ActivateMenuSelections ()
		{

			buyCheckpointsMenu.Enabled = (availableCoins >= checkpointBuyPrice);
			buyAdditionsMenu.Enabled = (availableCoins >= additionsBuyPrice);
			buySubtractionsMenu.Enabled = (availableCoins >= subtractionsBuyPrice);
			buyNextInSequenceMenu.Enabled = (availableCoins >= nextInSequenceBuyPrice);
			buyPersistentTapStrengthMenu.Enabled = (availableCoins >= persistentTapStrengthPrice) && (persistentTapStrength < 5);
			buyPersistentTimeBonusMenu.Enabled = (availableCoins >= persistentTimeBonusPrice);
			buyPersistent2xTimeBonusMenu.Enabled = (availableCoins >= persistent2xTimeBonusPrice);
			buyPersistent3xTimeBonusMenu.Enabled = (availableCoins >= persistent3xTimeBonusPrice);
			buyPersistentLuckyPointChanceMenu.Enabled = (availableCoins >= persistentLuckyPointChancePrice);
			buyPersistentLuckyPointBonusMenu.Enabled = (availableCoins >= persistentLuckyPointBonusPrice);
			buyMedKitUpgradeMenu.Enabled = (availableCoins >= medKitUpgradePrice);
			buyMedKitActivateMenu.Enabled = (availableCoins >= medKitActivatePrice);
			buyHighlightNextMenu.Enabled = (availableCoins >= highlightNextPrice);

			sellCheckpointsMenu.Enabled = (availableCheckpoints > 0);
			sellAdditionsMenu.Enabled = (availableAdditions > 0);
			sellSubtractionsMenu.Enabled = (availableSubtractions > 0);
			sellNextInSequenceMenu.Enabled = (availableNextInSequence > 0);

			buyCheckpointsMenu.Color = buyCheckpointsMenu.Enabled ? CCColor3B.White : CCColor3B.DarkGray;
			buyAdditionsMenu.Color = buyAdditionsMenu.Enabled ? CCColor3B.White : CCColor3B.DarkGray;
			buySubtractionsMenu.Color = buySubtractionsMenu.Enabled ? CCColor3B.White : CCColor3B.DarkGray;
			buyNextInSequenceMenu.Color = buyNextInSequenceMenu.Enabled ? CCColor3B.White : CCColor3B.DarkGray;
			buyPersistentTapStrengthMenu.Color = buyPersistentTapStrengthMenu.Enabled ? CCColor3B.White : CCColor3B.DarkGray;
			buyPersistentTimeBonusMenu.Color = buyPersistentTimeBonusMenu.Enabled ? CCColor3B.White : CCColor3B.DarkGray;
			buyPersistent2xTimeBonusMenu.Color = buyPersistent2xTimeBonusMenu.Enabled ? CCColor3B.White : CCColor3B.DarkGray;
			buyPersistent3xTimeBonusMenu.Color = buyPersistent3xTimeBonusMenu.Enabled ? CCColor3B.White : CCColor3B.DarkGray;
			buyPersistentLuckyPointChanceMenu.Color = buyPersistentLuckyPointChanceMenu.Enabled ? CCColor3B.White : CCColor3B.DarkGray;
			buyPersistentLuckyPointBonusMenu.Color = buyPersistentLuckyPointBonusMenu.Enabled ? CCColor3B.White : CCColor3B.DarkGray;
			buyMedKitUpgradeMenu.Color = buyMedKitUpgradeMenu.Enabled ? CCColor3B.White : CCColor3B.DarkGray;
			buyMedKitActivateMenu.Color = buyMedKitActivateMenu.Enabled ? CCColor3B.White : CCColor3B.DarkGray;
			buyHighlightNextMenu.Color = buyHighlightNextMenu.Enabled ? CCColor3B.White : CCColor3B.DarkGray;

			sellCheckpointsMenu.Color = sellCheckpointsMenu.Enabled ? CCColor3B.White : CCColor3B.DarkGray;
			sellAdditionsMenu.Color = sellAdditionsMenu.Enabled ? CCColor3B.White : CCColor3B.DarkGray;
			sellSubtractionsMenu.Color = sellSubtractionsMenu.Enabled ? CCColor3B.White : CCColor3B.DarkGray;
			sellNextInSequenceMenu.Color = sellNextInSequenceMenu.Enabled ? CCColor3B.White : CCColor3B.DarkGray;

			if (isHighlightNextOn) {
				buyHighlightNextMenu.Enabled = false;
				buyHighlightNextMenu.Color = CCColor3B.DarkGray;
				buyHighlightNextItemLabel.Text = "Purchased";
			}

			if (isMedKitActive) {
				buyMedKitActivateMenu.Enabled = false;
				buyMedKitActivateMenu.Color = CCColor3B.DarkGray;
				buyMedKitActivateItemLabel.Text = "Active";
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// UpdateUI
		//---------------------------------------------------------------------------------------------------------
		// Updates UI elements
		//---------------------------------------------------------------------------------------------------------
		void UpdateUI ()
		{
			CalculatePrices ();
			ActivateMenuSelections ();

			availableCoinsAmountLabel.Text = availableCoins.ToString ();
			coinsSprite.PositionX = availableCoinsAmountLabel.BoundingBox.MinX - 200;

			checkpointBuyPriceLabel.Text = string.Format ("{0}", checkpointBuyPrice);
			additionsBuyPriceLabel.Text = string.Format ("{0}", additionsBuyPrice);
			subtractionsBuyPriceLabel.Text = string.Format ("{0}", subtractionsBuyPrice);
			nextInSequenceBuyPriceLabel.Text = string.Format ("{0}", nextInSequenceBuyPrice);
			persistentTapStrengthPriceLabel.Text = string.Format ("{0}", persistentTapStrengthPrice);
			persistentTimeBonusPriceLabel.Text = string.Format ("{0}", persistentTimeBonusPrice);
			persistent2xTimeBonusPriceLabel.Text = string.Format ("{0}", persistent2xTimeBonusPrice);
			persistent3xTimeBonusPriceLabel.Text = string.Format ("{0}", persistent3xTimeBonusPrice);
			persistentLuckyPointChancePriceLabel.Text = string.Format ("{0}", persistentLuckyPointChancePrice);
			persistentLuckyPointBonusPriceLabel.Text = string.Format ("{0}", persistentLuckyPointBonusPrice);
			medKitUpgradePriceLabel.Text = string.Format ("{0}", medKitUpgradePrice);
			medKitActivatePriceLabel.Text = string.Format ("{0}", medKitActivatePrice);
			highlightNextPriceLabel.Text = string.Format ("{0}", highlightNextPrice);

			availableCheckpointsAmountLabel.Text = string.Format ("{0}", availableCheckpoints);
			availableAdditionsAmountLabel.Text = string.Format ("{0}", availableAdditions);
			availableSubtractionsAmountLabel.Text = string.Format ("{0}", availableSubtractions);
			availableNextInSequenceAmountLabel.Text = string.Format ("{0}", availableNextInSequence);
			persistentTapStrengthAmountLabel.Text = string.Format ("{0}", persistentTapStrength);
			persistentTimeBonusAmountLabel.Text = string.Format ("{0}", persistentTimeBonus);
			persistent2xTimeBonusAmountLabel.Text = string.Format ("{0}", persistent2xTimeBonus);
			persistent2xTimeBonusAmountLabel.Text = string.Format ("{0}", persistent2xTimeBonus);
			persistentLuckyPointChanceAmountLabel.Text = string.Format ("{0}", persistentLuckyPointChance);
			persistentLuckyPointBonusAmountLabel.Text = string.Format ("{0}", persistentLuckyPointBonus);
			medKitUpgradeAmountLabel.Text = string.Format ("{0}", medKitUpgradeLevel);
			medKitActivateAmountLabel.Text = string.Format ("{0}", isMedKitActive);
			highlightNextAmountLabel.Text = string.Format ("{0}", isHighlightNextOn);
		}

		//---------------------------------------------------------------------------------------------------------
		// BuyTapStrength
		//---------------------------------------------------------------------------------------------------------
		// Allows the player to buy an increase in tap strength if they have enough coins
		//---------------------------------------------------------------------------------------------------------
		void BuyTapStrength(object obj)
		{
			if (availableCoins >=persistentTapStrengthPrice) {
				availableCoins = availableCoins - persistentTapStrengthPrice;
				persistentTapStrength++;
				UpdateUI ();
				UpdatePlayerData ();
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// BuyCheckpoint
		//---------------------------------------------------------------------------------------------------------
		// Allows the player to buy a checkpoint consumable if they have enough coins
		//---------------------------------------------------------------------------------------------------------
		void BuyCheckpoint (object obj)
		{
			if (availableCoins >= checkpointBuyPrice) {
				availableCoins = availableCoins - checkpointBuyPrice;
				availableCheckpoints++;
				UpdateUI ();
				UpdatePlayerData ();
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// BuyAddition
		//---------------------------------------------------------------------------------------------------------
		// Allows the player to buy an addition consumable if they have enough coins
		//---------------------------------------------------------------------------------------------------------
		void BuyAddition (object obj)
		{
			if (availableCoins >= additionsBuyPrice) {
				availableCoins = availableCoins - additionsBuyPrice;
				availableAdditions++;
				UpdateUI ();
				UpdatePlayerData ();
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// BuySubtraction
		//---------------------------------------------------------------------------------------------------------
		// Allows the player to buy a subtraction consumable if they have enough coins
		//---------------------------------------------------------------------------------------------------------
		void BuySubtraction (object obj)
		{
			if (availableCoins >= subtractionsBuyPrice) {
				availableCoins = availableCoins - subtractionsBuyPrice;
				availableSubtractions++;
				UpdateUI ();
				UpdatePlayerData ();
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// BuyNextInSequence
		//---------------------------------------------------------------------------------------------------------
		// Allows the player to buy a Next In Sequence consumable if they have enough coins
		//---------------------------------------------------------------------------------------------------------
		void BuyNextInSequence (object obj)
		{
			if (availableCoins >= nextInSequenceBuyPrice) {
				availableCoins = availableCoins - nextInSequenceBuyPrice;
				availableNextInSequence++;
				UpdateUI ();
				UpdatePlayerData ();
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// BuyPersistentTimeBonus
		//---------------------------------------------------------------------------------------------------------
		// Allows the player to buy extra time to finish the level
		//---------------------------------------------------------------------------------------------------------
		void BuyPersistentTimeBonus (object obj)
		{
			if (availableCoins >= persistentTimeBonusPrice) {
				availableCoins = availableCoins - persistentTimeBonusPrice;
				persistentTimeBonus++;
				UpdateUI ();
				UpdatePlayerData ();
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// BuyPersistent2xTimeBonus
		//---------------------------------------------------------------------------------------------------------
		// Allows the player to buy extra time for their 2x point bonus timer
		//---------------------------------------------------------------------------------------------------------
		void BuyPersistent2xTimeBonus (object obj)
		{
			if (availableCoins >= persistent2xTimeBonusPrice) {
				availableCoins = availableCoins - persistent2xTimeBonusPrice;
				persistent2xTimeBonus++;
				UpdateUI ();
				UpdatePlayerData ();
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// BuyPersistent3xTimeBonus
		//---------------------------------------------------------------------------------------------------------
		// Allows the player to buy extra time for their 3x point bonus timer
		//---------------------------------------------------------------------------------------------------------
		void BuyPersistent3xTimeBonus (object obj)
		{
			if (availableCoins >= persistent3xTimeBonusPrice) {
				availableCoins = availableCoins - persistent3xTimeBonusPrice;
				persistent3xTimeBonus++;
				UpdateUI ();
				UpdatePlayerData ();
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// BuyPersistentLuckyPointChance
		//---------------------------------------------------------------------------------------------------------
		// Allows the player to buy an increased level of chance to get a lucky point when popping a bubble
		//---------------------------------------------------------------------------------------------------------
		void BuyPersistentLuckyPointChance (object obj)
		{
			if (availableCoins >= persistentLuckyPointChancePrice) {
				availableCoins = availableCoins - persistentLuckyPointChancePrice;
				persistentLuckyPointChance++;
				UpdateUI ();
				UpdatePlayerData ();
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// BuyPersistentLuckyPointBonus
		//---------------------------------------------------------------------------------------------------------
		// Allows the player to buy an increased level of lucky bonus points to get when popping a bubble
		//---------------------------------------------------------------------------------------------------------
		void BuyPersistentLuckyPointBonus (object obj)
		{
			if (availableCoins >= persistentLuckyPointBonusPrice) {
				availableCoins = availableCoins - persistentLuckyPointBonusPrice;
				persistentLuckyPointBonus++;
				UpdateUI ();
				UpdatePlayerData ();
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// BuyMedKitUpgrade
		//---------------------------------------------------------------------------------------------------------
		// Allows the player to upgrade their Med Kit level if they have enough coins
		//---------------------------------------------------------------------------------------------------------
		void BuyMedKitUpgrade (object obj)
		{
			if (availableCoins >= medKitUpgradePrice) {
				availableCoins = availableCoins - medKitUpgradePrice;
				medKitUpgradeLevel++;
				UpdateUI ();
				UpdatePlayerData ();
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// BuyMedKitActive
		//---------------------------------------------------------------------------------------------------------
		// Allows the player to activate their med kit if they have enough coins
		//---------------------------------------------------------------------------------------------------------
		void BuyMedKitActivate (object obj)
		{
			if (availableCoins >= medKitActivatePrice) {
				availableCoins = availableCoins - medKitActivatePrice;
				isMedKitActive = true;
				UpdateUI ();
				UpdatePlayerData ();
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// BuyHighlightNext
		//---------------------------------------------------------------------------------------------------------
		// Allows the player to buy the option to highlight the next in sequence bubble
		//---------------------------------------------------------------------------------------------------------
		void BuyHighlightNext (object obj)
		{
			if (availableCoins >= highlightNextPrice) {
				availableCoins = availableCoins - highlightNextPrice;
				activePlayer.IsHighlightNextActive = true;
				isHighlightNextOn = true;
				UpdateUI ();
				UpdatePlayerData ();
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// SellCheckpoint
		//---------------------------------------------------------------------------------------------------------
		// Allows the player to sell a checkpoint consumable if they have any
		//---------------------------------------------------------------------------------------------------------
		void SellCheckpoint (object obj)
		{
			if (availableCheckpoints > 0) {
				availableCoins = availableCoins + checkpointSellPrice;
				availableCheckpoints--;
				UpdateUI ();
				UpdatePlayerData ();
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// SellAddition
		//---------------------------------------------------------------------------------------------------------
		// Allows the player to sell an addition consumable if they have any
		//---------------------------------------------------------------------------------------------------------
		void SellAddition (object obj)
		{
			if (availableAdditions > 0) {
				availableCoins = availableCoins + additionsSellPrice;
				availableAdditions--;
				UpdateUI ();
				UpdatePlayerData ();
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// SellSubtraction
		//---------------------------------------------------------------------------------------------------------
		// Allows the player to sell a subtraction consumable if they have any
		//---------------------------------------------------------------------------------------------------------
		void SellSubtraction (object obj)
		{
			if (availableSubtractions > 0) {
				availableCoins = availableCoins + subtractionsSellPrice;
				availableSubtractions--;
				UpdateUI ();
				UpdatePlayerData ();
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// SellNextInSequence
		//---------------------------------------------------------------------------------------------------------
		// Allows the player to sell a Next In Sequence consumable if they have any
		//---------------------------------------------------------------------------------------------------------
		void SellNextInSequence (object obj)
		{
			if (availableNextInSequence > 0) {
				availableCoins = availableCoins + nextInSequenceSellPrice;
				availableNextInSequence--;
				UpdateUI ();
				UpdatePlayerData ();
			}
		}

		//---------------------------------------------------------------------------------------------------------
		// UpdatePlayerData
		//---------------------------------------------------------------------------------------------------------
		// Updates player data based on shopping
		//---------------------------------------------------------------------------------------------------------
		void UpdatePlayerData ()
		{
			activePlayer.Coins = availableCoins;
			activePlayer.PersistentTapStrength = persistentTapStrength;
			activePlayer.ConsumableCheckPoint = availableCheckpoints;
			activePlayer.ConsumableAdd = availableAdditions;
			activePlayer.ConsumableSubtract = availableSubtractions;
			activePlayer.ConsumableNextSeq = availableNextInSequence;
			activePlayer.PersistentTimeBonus = persistentTimeBonus;
			activePlayer.Persistent2xTimeBonus = persistent2xTimeBonus;
			activePlayer.Persistent3xTimeBonus = persistent3xTimeBonus;
			activePlayer.LuckyPointChance = persistentLuckyPointChance;
			activePlayer.LuckyPointBonus = persistentLuckyPointBonus;
			activePlayer.MedKitUpgradeLevel = medKitUpgradeLevel;
			activePlayer.IsMedKitActivated = isMedKitActive;
			activePlayer.HighlightNextPurchased = isHighlightNextOn;
			activePlayer.WriteData (activePlayer);
		}
	}
}