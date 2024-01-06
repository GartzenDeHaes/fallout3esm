using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords.DIAL;

using Portland.Mathmatics;

using System;
using System.Collections.Generic;

namespace Fallout.NET.TES4.SubRecords.INFO
{

	/// <summary>
	/// CTDA fields are widely used in Skyrim records to provide conditions that must be 
	/// met. The format is similar to that used in Oblivion records, although there are more
	/// function indices.
	///	In Skyrim.esm, the following types have CTDA fields: ALCH, COBJ, CPTH, ENCH, FACT, 
	///	IDLE, INFO, LSCR, MESG, MGEF, MUST, PACK, PERK, QUST, SCEN, SCRL, SMBN, SMQN, SNDR, SPEL.
	/// https://github.com/TES5Edit/fopdoc/blob/master/Fallout3/Records/Subrecords/CTDA.md
	/// https://en.uesp.net/wiki/Skyrim_Mod:Mod_File_Format/CTDA_Field
	/// </summary>
	public sealed class CTDASubRecord : SubRecord
	{
		public DialogType Type;
		public IntFloat GlobFormIdOrValue;
		public CondFunctionIndex FunctionIdx;
		public byte[] Parameter1;
		public byte[] Parameter2;
		public RunOnType RunOn;
		public uint ReferenceFormId;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);
			if (Size < 20)
			{
				UnityEngine.Debug.LogError(Size);
				reader.ReadBytes((int)Size);
				return;
			}

			Type = (DialogType)reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();

			GlobFormIdOrValue.UIntValue = reader.ReadUInt32();
			FunctionIdx = (CondFunctionIndex)reader.ReadUInt32();
			Parameter1 = reader.ReadBytes(4);
			Parameter2 = reader.ReadBytes(4);
			if (Size > 20)
			{
				RunOn = (RunOnType)reader.ReadUInt32();
			}
			if (Size > 24)
			{
				ReferenceFormId = reader.ReadUInt32();
			}
		}
	}

	public enum RunOnType
	{
		Subject = 0,
		Target = 1,
		Reference = 2,
		CombatTarget = 3,
		LinkedReference = 4,
	}

	[Flags]
	public enum ConditionType
	{
		None = 0,
		Combine = 0x0001,       //	Combine next condition using OR (default is to use AND)
		RunOnTarget = 0x0002,   //	Run On Target
		UseGlobal = 0x0004,     //	Use Global
		EqualTo = 0x0000,       //	Equal To
		NotEqualTo = 0x2000,    //	Not Equal To
		GT = 0x4000,            //	Greater Than
		GTEQ = 0x6000,          //	Greater Than or Equal To
		LT = 0x8000,            //	Less Than
		LTEQ = 0xA000,          //	Less Than or Equal To
	}

	public enum CondFunctionIndex
	{
		GetDistance = 1,
		GetLocked = 5,
		GetPos = 6,
		GetAngle = 8,
		GetStartingPos = 10,
		GetStartingAngle = 11,
		GetSecondsPassed = 12,
		GetActorValue = 14,
		GetCurrentTime = 18,
		GetScale = 24,
		IsMoving = 25,
		IsTurning = 26,
		GetLineOfSight = 27,
		GetInSameCell = 32,
		GetDisabled = 35,
		MenuMode = 36,
		GetDisease = 39,
		GetVampire = 40,
		GetClothingValue = 41,
		SameFaction = 42,
		SameRace = 43,
		SameSex = 44,
		GetDetected = 45,
		GetDead = 46,
		GetItemCount = 47,
		GetGold = 48,
		GetSleeping = 49,
		GetTalkedToPC = 50,
		GetScriptVariable = 53,
		GetQuestRunning = 56,
		GetStage = 58,
		GetStageDone = 59,
		GetFactionRankDifference = 60,
		GetAlarmed = 61,
		IsRaining = 62,
		GetAttacked = 63,
		GetIsCreature = 64,
		GetLockLevel = 65,
		GetShouldAttack = 66,
		GetInCell = 67,
		GetIsClass = 68,
		GetIsRace = 69,
		GetIsSex = 70,
		GetInFaction = 71,
		GetIsID = 72,
		GetFactionRank = 73,
		GetGlobalValue = 74,
		IsSnowing = 75,
		GetDisposition = 76,
		GetRandomPercent = 77,
		GetQuestVariable = 79,
		GetLevel = 80,
		GetArmorRating = 81,
		GetDeadCount = 84,
		GetIsAlerted = 91,
		GetHeadingAngle = 99,
		IsWeaponOut = 101,
		IsTorchOut = 102,
		IsShieldOut = 103,
		IsFacingUp = 106,
		GetKnockedState = 107,
		GetWeaponAnimType = 108,
		IsWeaponSkillType = 109,
		GetCurrentAIPackage = 110,
		IsWaiting = 111,
		IsIdlePlaying = 112,
		GetMinorCrimeCount = 116,
		GetMajorCrimeCount = 117,
		GetActorAggroRadiusViolated = 118,
		GetCrime = 122,
		IsGreetingPlayer = 123,
		IsGuard = 125,
		HasBeenEaten = 127,
		GetFatiguePercentage = 128,
		GetPCIsClass = 129,
		GetPCIsRace = 130,
		GetPCIsSex = 131,
		GetPCInFaction = 132,
		SameFactionAsPC = 133,
		SameRaceAsPC = 134,
		SameSexAsPC = 135,
		GetIsReference = 136,
		IsTalking = 141,
		GetWalkSpeed = 142,
		GetCurrentAIProcedure = 143,
		GetTrespassWarningLevel = 144,
		IsTrespassing = 145,
		IsInMyOwnedCell = 146,
		GetWindSpeed = 147,
		GetCurrentWeatherPercent = 148,
		GetIsCurrentWeather = 149,
		IsContinuingPackagePCNear = 150,
		CanHaveFlames = 153,
		HasFlames = 154,
		GetOpenState = 157,
		GetSitting = 159,
		GetFurnitureMarkerID = 160,
		GetIsCurrentPackage = 161,
		IsCurrentFurnitureRef = 162,
		IsCurrentFurnitureObj = 163,
		GetDayOfWeek = 170,
		GetTalkedToPCParam = 172,
		IsPCSleeping = 175,
		IsPCAMurderer = 176,
		GetDetectionLevel = 180,
		GetEquipped = 182,
		IsSwimming = 185,
		GetAmountSoldStolen = 190,
		GetIgnoreCrime = 192,
		GetPCExpelled = 193,
		GetPCFactionMurder = 195,
		GetPCEnemyofFaction = 197,
		GetPCFactionAttack = 199,
		GetDestroyed = 203,
		HasMagicEffect = 214,
		GetDefaultOpen = 215,
		GetAnimAction = 219,
		IsSpellTarget = 223,
		GetVATSMode = 224,
		GetPersuasionNumber = 225,
		GetSandman = 226,
		GetCannibal = 227,
		GetIsClassDefault = 228,
		GetClassDefaultMatch = 229,
		GetInCellParam = 230,
		GetVatsTargetHeight = 235,
		GetIsGhost = 237,
		GetUnconscious = 242,
		GetRestrained = 244,
		GetIsUsedItem = 246,
		GetIsUsedItemType = 247,
		GetIsPlayableRace = 254,
		GetOffersServicesNow = 255,
		GetUsedItemLevel = 258,
		GetUsedItemActivate = 259,
		GetBarterGold = 264,
		IsTimePassing = 265,
		IsPleasant = 266,
		IsCloudy = 267,
		GetArmorRatingUpperBody = 274,
		GetBaseActorValue = 277,
		IsOwner = 278,
		IsCellOwner = 280,
		IsHorseStolen = 282,
		IsLeftUp = 285,
		IsSneaking = 286,
		IsRunning = 287,
		GetFriendHit = 288,
		IsInCombat = 289,
		IsInInterior = 300,
		IsWaterObject = 304,
		IsActorUsingATorch = 306,
		IsXBox = 309,
		GetInWorldspace = 310,
		GetPCMiscStat = 312,
		IsActorEvil = 313,
		IsActorAVictim = 314,
		GetTotalPersuasionNumber = 315,
		GetIdleDoneOnce = 318,
		GetNoRumors = 320,
		WhichServiceMenu = 323,
		IsRidingHorse = 327,
		IsInDangerousWater = 332,
		GetIgnoreFriendlyHits = 338,
		IsPlayersLastRiddenHorse = 339,
		IsActor = 353,
		IsEssential = 354,
		IsPlayerMovingIntoNewSpace = 358,
		GetTimeDead = 361,
		GetPlayerHasLastRiddenHorse = 362,
		IsChild = 365,
		GetLastPlayerAction = 367,
		IsPlayerActionActive = 368,
		IsTalkingActivatorActor = 370,
		IsInList = 372,
		GetHasNote = 382,
		GetHitLocation = 391,
		IsPC1stPerson = 392,
		GetCauseofDeath = 397,
		IsLimbGone = 398,
		IsWeaponInList = 399,
		HasFriendDisposition = 403,
		GetVATSValue = 408,
		IsKiller = 409,
		IsKillerObject = 410,
		GetFactionCombatReaction = 411,
		Exists = 415,
		GetGroupMemberCount = 416,
		GetGroupTargetCount = 417,
		GetIsVoiceType = 427,
		GetPlantedExplosive = 428,
		IsActorTalkingThroughActivator = 430,
		GetHealthPercentage = 431,
		GetIsObjectType = 433,
		GetDialogueEmotion = 435,
		GetDialogueEmotionValue = 436,
		GetIsCreatureType = 438,
		GetInZone = 446,
		HasPerk = 449,
		GetFactionRelation = 450,
		IsLastIdlePlayed = 451,
		GetPlayerTeammate = 454,
		GetPlayerTeammateCount = 455,
		GetActorCrimePlayerEnemy = 459,
		GetActorFactionPlayerEnemy = 460,
		IsPlayerGrabbedRef = 464,
		GetDestructionStage = 471,
		GetIsAlignment = 474,
		GetThreatRatio = 478,
		GetIsUsedItemEquipType = 480,
		GetConcussed = 489,
		GetMapMarkerVisible = 492,
		GetPermanentActorValue = 495,
		GetKillingBlowLimb = 496,
		GetWeaponHealthPerc = 500,
		GetRadiationLevel = 503,
		GetLastHitCritical = 510,
		IsCombatTarget = 515,
		GetVATSRightAreaFree = 518,
		GetVATSLeftAreaFree = 519,
		GetVATSBackAreaFree = 520,
		GetVATSFrontAreaFree = 521,
		GetIsLockBroken = 522,
		IsPS3 = 523,
		IsWin32 = 524,
		GetVATSRightTargetVisible = 525,
		GetVATSLeftTargetVisible = 526,
		GetVATSBackTargetVisible = 527,
		GetVATSFrontTargetVisible = 528,
		IsInCriticalStage = 531,
		GetXPForNextLevel = 533,
		GetQuestCompleted = 546,
		IsGoreDisabled = 550,
		GetSpellUsageNum = 555,
		GetActorsInHigh = 557,
		HasLoaded3D = 558,
	}

	public enum GenderArg
	{
		Male = 0,
		Female = 1,
	}

	public enum CrimeTypeArg
	{
		None = -1,
		Steal = 0,
		PickPocket = 1,
		Trespass = 2,
		Attack = 3,
		Murder = 4,
	}

	public enum AxisArg
	{
		Unknown = 0,
		X = 88,
		Y = 89,
		Z = 90
	}

	public enum MiscStatArg
	{
		QuestsCompleted = 0,
		LocationsDiscovered = 1,
		PeopleKilled = 2,
		CreaturesKilled = 3,
		LocksPicked = 4,
		ComputersHacked = 5,
		StimpaksTaken = 6,
		RadXTaken = 7,
		RadAwayTaken = 8,
		ChemsTaken = 9,
		TimesAddicted = 10,
		MinesDisarmed = 11,
		SpeechSuccesses = 12,
		PocketsPicked = 13,
		PantsExploded = 14,
		BooksRead = 15,
		BobbleheadsFound = 16,
		WeaponsCreated = 17,
		PeopleMezzed = 18,
		CaptivesRescued = 19,
		SandmanKills = 20,
		ParalyzingPunches = 21,
		RobotsDisabled = 22,
		ContractsCompleted = 23,
		CorpsesEaten = 24,
		MysteriousStrangerVisits = 25,
	}

	public enum AlignmentArg
	{
		Good = 0,
		Neutral = 1,
		Evil = 2,
		VeryGood = 3,
		VeryEvil = 4,
	}

	public enum FormTypeArg
	{
		TextureSet = 0x04,
		MenuIcon = 0x05,
		Global = 0x06,
		Class = 0x07,
		Faction = 0x08,
		HeadPart = 0x09,
		Hair = 0x0A,
		Eyes = 0x0B,
		Race = 0x0C,
		Sound = 0x0D,
		AcousticSpace = 0x0E,
		Skill = 0x0F,
		BaseEffect = 0x10,
		Script = 0x11,
		LandscapeTexture = 0x12,
		ObjectEffect = 0x13,
		ActorEffect = 0x14,
		Activator = 0x15,
		TalkingActivator = 0x16,
		Terminal = 0x17,
		Armor = 0x18,
		Book = 0x19,
		Clothing = 0x1A,
		Container = 0x1B,
		Door = 0x1C,
		Ingredient = 0x1D,
		Light = 0x1E,
		Misc = 0x1F,
		Static = 0x20,
		StaticCollection = 0x21,
		MovableStatic = 0x22,
		PlaceableWater = 0x23,
		Grass = 0x24,
		Tree = 0x25,
		Flora = 0x26,
		Furniture = 0x27,
		Weapon = 0x28,
		Ammo = 0x29,
		NPC = 0x2A,
		Creature = 0x2B,
		LeveledCreature = 0x2C,
		LeveledNPC = 0x2D,
		Key = 0x2E,
		Ingestible = 0x2F,
		IdleMarker = 0x30,
		Note = 0x31,
		ConstructibleObject = 0x32,
		Projectile = 0x33,
		LeveledItem = 0x34,
		Weather = 0x35,
		Climate = 0x36,
		Region = 0x37,
		Cell = 0x39,
		PlacedObject = 0x3A,
		PlacedCharacter = 0x3B,
		PlacedCreature = 0x3C,
		PlacedGrenade = 0x3E,
		Worldspace = 0x41,
		Landscape = 0x42,
		NavigationMesh = 0x43,
		DialogTopic = 0x45,
		DialogResponse = 0x46,
		Quest = 0x47,
		IdleAnimation = 0x48,
		Package = 0x49,
		CombatStyle = 0x4A,
		LoadScreen = 0x4B,
		LeveledSpell = 0x4C,
		AnimatedObject = 0x4D,
		Water = 0x4E,
		EffectShader = 0x4F,
		Explosion = 0x51,
		Debris = 0x52,
		ImageSpace = 0x53,
		ImageSpaceModifier = 0x54,
		FormIDList = 0x55,
		Perk = 0x56,
		BodyPartData = 0x57,
		AddonNode = 0x58,
		ActorValueInfo = 0x59,
		RadiationStage = 0x5A,
		CameraShot = 0x5B,
		CameraPath = 0x5C,
		VoiceType = 0x5D,
		ImpactData = 0x5E,
		ImpactDataSet = 0x5F,
		ArmorAddon = 0x60,
		EncounterZone = 0x61,
		Message = 0x62,
		Ragdoll = 0x63,
		DefaultObjectManager = 0x64,
		LightingTemplate = 0x65,
		MusicType = 0x66,
	}

	public enum CriticalStageType
	{
		None = 0,
		GooStart = 1,
		GooEnd = 2,
		DisintegrateStart = 3,
		DisintegrateEnd = 4,
	}

	public enum CreatureType
	{
		Animal = 0,
		MutatedAnimal = 1,
		MutatedInsect = 2,
		Abomination = 3,
		SuperMutant = 4,
		FeralGhoul = 5,
		Robot = 6,
		Giant = 7,
	}

	public enum PlayerActionType
	{
		None = 0,
		SwingingMeleeWeapon = 1,
		ThrowingGrenade = 2,
		FireWeapon = 3,
		LayMine = 4,
		ZKeyObject = 5,
		Jumping = 6,
		KnockingOverObjects = 7,
		StandOnTableOrChair = 8,
		IronSights = 9,
		DestroyingObject = 10,
	}

	public enum BodyLocationType
	{
		None = -1,
		Torso = 0,
		Head1 = 1,
		Head2 = 2,
		LeftArm1 = 3,
		LeftArm2 = 4,
		RightArm1 = 5,
		RightArm2 = 6,
		LeftLeg1 = 7,
		LeftLeg2 = 8,
		LeftLeg3 = 9,
		RightLeg1 = 10,
		RightLeg2 = 11,
		RightLeg3 = 12,
		Brain = 14,
	}
}
