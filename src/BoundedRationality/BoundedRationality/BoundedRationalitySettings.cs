using System;

using Lakuna.BoundedRationality.Utility;

using Verse;

namespace Lakuna.BoundedRationality {
	public sealed class BoundedRationalitySettings : ModSettings {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly int _infoHeight = Enum.GetValues(typeof(InformationCategory)).Length;
#else
		private static readonly int _infoHeight = Enum.GetValues<InformationCategory>().Length;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly int _infoWidth = Enum.GetValues(typeof(PawnType)).Length;
#else
		private static readonly int _infoWidth = Enum.GetValues<PawnType>().Length;
#endif

		public bool AlwaysKnowGrowthMomentTraits {
			get => this._alwaysKnowGrowthMomentTraits;
			set => this._alwaysKnowGrowthMomentTraits = value;
		}

		public bool AlwaysKnowMoreAboutColonistRelatives {
			get => this._alwaysKnowMoreAboutColonistRelatives;
			set => this._alwaysKnowMoreAboutColonistRelatives = value;
		}

		public bool AlwaysKnowStartingColonists {
			get => this._alwaysKnowStartingColonists;
			set => this._alwaysKnowStartingColonists = value;
		}

		public int BackstoryLearningDifficulty {
			get => this._backstoryDiscoveryDifficulty;
			set => this._backstoryDiscoveryDifficulty = value;
		}

		public bool EnableUniqueTraitUnlockConditions {
			get => this._enableUniqueTraitUnlockConditions;
			set => this._enableUniqueTraitUnlockConditions = value;
		}

		public bool HideAncientCorpses {
			get => this._hideAncientCorpses;
			set => this._hideAncientCorpses = value;
		}

		public bool HideFactionInformation {
			get => this._hideFactionInformation;
			set => this._hideFactionInformation = value;
		}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public BoolGrid KnownInformation {
#else
		public BoolGrid? KnownInformation {
#endif
			get => this._knownInformation;
			set => this._knownInformation = value;
		}

		public bool LegacyMode {
			get => this._legacyMode;
			set => this._legacyMode = value;
		}

		public bool NeverHideAlerts {
			get => this._neverHideAlerts;
			set => this._neverHideAlerts = value;
		}

		public bool NeverHideControls {
			get => this._neverHideControls;
			set => this._neverHideControls = value;
		}

		public bool NeverHideLetters {
			get => this._neverHideLetters;
			set => this._neverHideLetters = value;
		}

		public bool NeverHideMessages {
			get => this._neverHideMessages;
			set => this._neverHideMessages = value;
		}

		public bool NeverHidePawnLabels {
			get => this._neverHidePawnLabels;
			set => this._neverHidePawnLabels = value;
		}

		public bool NeverHideTextMotes {
			get => this._neverHideTextMotes;
			set => this._neverHideTextMotes = value;
		}

		public bool PreventForcedSpeed {
			get => this._preventForcedSpeed;
			set => this._preventForcedSpeed = value;
		}

		public bool RememberFormerColonists {
			get => this._rememberFormerColonists;
			set => this._rememberFormerColonists = value;
		}

		public int SkillsLearningDifficulty {
			get => this._skillsDiscoveryDifficulty;
			set => this._skillsDiscoveryDifficulty = value;
		}

		public int TraitsLearningDifficulty {
			get => this._traitDiscoveryDifficulty;
			set => this._traitDiscoveryDifficulty = value;
		}

		private bool _alwaysKnowGrowthMomentTraits = true;

		private bool _alwaysKnowMoreAboutColonistRelatives = true;

		private bool _alwaysKnowStartingColonists = true;

		private int _backstoryDiscoveryDifficulty = 1;

		private bool _enableUniqueTraitUnlockConditions = true;

		private bool _hideAncientCorpses = true;

		private bool _hideFactionInformation;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
		private BoolGrid _knownInformation = new BoolGrid(new Map() { info = { Size = new IntVec3(_infoWidth, 0, _infoHeight) } });
#elif V1_6
		private BoolGrid _knownInformation = new BoolGrid(_infoWidth, _infoHeight);
#else
		private BoolGrid? _knownInformation = new(_infoWidth, _infoHeight);
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private bool[] _learningEnabled = new bool[_infoWidth];
#else
		private bool[]? _learningEnabled = new bool[_infoWidth];
#endif

		private bool _legacyMode;

		private bool _neverHideAlerts;

		private bool _neverHideControls = true;

		private bool _neverHideLetters;

		private bool _neverHideMessages;

		private bool _neverHidePawnLabels;

		private bool _neverHideTextMotes;

		private bool _preventForcedSpeed;

		private bool _rememberFormerColonists = true;

		private int _skillsDiscoveryDifficulty = 1;

		private int _traitDiscoveryDifficulty = 1;

		public BoundedRationalitySettings() {
			this.InitializeKnownInformation();
			this.InitializeLearningEnabled();
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Deep.Look(ref this._knownInformation, "knownInformation");
			MiscellaneousUtility.LookBoolArray(ref this._learningEnabled, _infoWidth, "learningEnabled");
			Scribe_Values.Look(ref this._traitDiscoveryDifficulty, "traitDiscoveryDifficulty");
			Scribe_Values.Look(ref this._backstoryDiscoveryDifficulty, "backstoryDiscoveryDifficulty");
			Scribe_Values.Look(ref this._skillsDiscoveryDifficulty, "skillsDiscoveryDifficulty");
			Scribe_Values.Look(ref this._alwaysKnowStartingColonists, "alwaysKnowStartingColonists");
			Scribe_Values.Look(ref this._rememberFormerColonists, "rememberFormerColonists");
			Scribe_Values.Look(ref this._alwaysKnowMoreAboutColonistRelatives, "alwaysKnowMoreAboutColonistRelatives");
			Scribe_Values.Look(ref this._alwaysKnowGrowthMomentTraits, "alwaysKnowGrowthMomentTraits");
			Scribe_Values.Look(ref this._hideFactionInformation, "hideFactionInformation");
			Scribe_Values.Look(ref this._neverHideControls, "neverHideControls");
			Scribe_Values.Look(ref this._neverHideTextMotes, "neverHideTextMotes");
			Scribe_Values.Look(ref this._neverHideMessages, "neverHideMessages");
			Scribe_Values.Look(ref this._neverHideLetters, "neverHideLetters");
			Scribe_Values.Look(ref this._neverHideAlerts, "neverHideAlerts");
			Scribe_Values.Look(ref this._neverHidePawnLabels, "neverHidePawnLabels");
			Scribe_Values.Look(ref this._preventForcedSpeed, "preventForcedSpeed");
			Scribe_Values.Look(ref this._hideAncientCorpses, "hideAncientCorpses");
			Scribe_Values.Look(ref this._legacyMode, "legacyMode");
			Scribe_Values.Look(ref this._enableUniqueTraitUnlockConditions, "enableUniqueTraitUnlockConditions");

			// If new information categories have been added since the last load, add them to the settings.
			if (Scribe.mode == LoadSaveMode.PostLoadInit) {
				if (!this.KnownInformationIsValid()) {
					this.InitializeKnownInformation();
				}

				if (!this.LearningEnabledIsValid()) {
					this.InitializeLearningEnabled();
				}
			}
		}

		public bool GetLearningEnabledFor(PawnType type) {
			if (this._learningEnabled is null) {
				BoundedRationalityLogger.LogErrorMessage("Attempted to get learning enabled from null.");
				this._learningEnabled = new bool[_infoWidth];
				this.InitializeLearningEnabled();
			}

			return this._learningEnabled[(int)type];
		}

		public bool SetLearningEnabledFor(PawnType type, bool value) {
			if (this._learningEnabled is null) {
				BoundedRationalityLogger.LogErrorMessage("Attempted to set learning enabled in null.");
				this._learningEnabled = new bool[_infoWidth];
				this.InitializeLearningEnabled();
			}

			return this._learningEnabled[(int)type] = value;
		}

		private void InitializeKnownInformation() {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
			Map map = new Map();
			map.info.Size = new IntVec3(_infoWidth, 0, _infoHeight);
			if (this.KnownInformation is null) {
				this.KnownInformation = new BoolGrid(map);
			} else {
				this.KnownInformation.ClearAndResizeTo(map);
			}
#elif V1_5
			Map map = new Map();
			map.info.Size = new IntVec3(_infoWidth, 0, _infoHeight);
			if (this.KnownInformation is null) {
				this.KnownInformation = new BoolGrid(map);
			} else {
				this.KnownInformation.ClearAndResizeTo(map);
			}

			map.Dispose();
#else
			if (this.KnownInformation is null) {
				this.KnownInformation = new BoolGrid(_infoWidth, _infoHeight);
			} else {
				this.KnownInformation.ClearAndResizeTo(_infoWidth, _infoHeight);
			}
#endif

			this.KnownInformation[(int)PawnType.Colonist, (int)InformationCategory.Basic] = true;
			this.KnownInformation[(int)PawnType.Colonist, (int)InformationCategory.Health] = true;
			this.KnownInformation[(int)PawnType.Colonist, (int)InformationCategory.Needs] = true;
			this.KnownInformation[(int)PawnType.Colonist, (int)InformationCategory.Gear] = true;
			this.KnownInformation[(int)PawnType.Colonist, (int)InformationCategory.Skills] = true;
			this.KnownInformation[(int)PawnType.Colonist, (int)InformationCategory.Traits] = true;
			this.KnownInformation[(int)PawnType.Colonist, (int)InformationCategory.Abilities] = true;
			this.KnownInformation[(int)PawnType.Colonist, (int)InformationCategory.Backstory] = true;
			this.KnownInformation[(int)PawnType.Colonist, (int)InformationCategory.Social] = true;
			this.KnownInformation[(int)PawnType.Colonist, (int)InformationCategory.Ideoligion] = true;
			this.KnownInformation[(int)PawnType.Colonist, (int)InformationCategory.Personal] = true;
			this.KnownInformation[(int)PawnType.Controlled, (int)InformationCategory.Basic] = true;
			this.KnownInformation[(int)PawnType.Controlled, (int)InformationCategory.Health] = true;
			this.KnownInformation[(int)PawnType.Controlled, (int)InformationCategory.Needs] = true;
			this.KnownInformation[(int)PawnType.Controlled, (int)InformationCategory.Gear] = true;
			this.KnownInformation[(int)PawnType.Controlled, (int)InformationCategory.Skills] = true;
			this.KnownInformation[(int)PawnType.Controlled, (int)InformationCategory.Traits] = true;
			this.KnownInformation[(int)PawnType.Controlled, (int)InformationCategory.Abilities] = true;
			this.KnownInformation[(int)PawnType.Slave, (int)InformationCategory.Basic] = true;
			this.KnownInformation[(int)PawnType.Slave, (int)InformationCategory.Health] = true;
			this.KnownInformation[(int)PawnType.Slave, (int)InformationCategory.Needs] = true;
			this.KnownInformation[(int)PawnType.Slave, (int)InformationCategory.Gear] = true;
			this.KnownInformation[(int)PawnType.Slave, (int)InformationCategory.Skills] = true;
			this.KnownInformation[(int)PawnType.Slave, (int)InformationCategory.Traits] = true;
			this.KnownInformation[(int)PawnType.Slave, (int)InformationCategory.Abilities] = true;
			this.KnownInformation[(int)PawnType.Prisoner, (int)InformationCategory.Basic] = true;
			this.KnownInformation[(int)PawnType.Prisoner, (int)InformationCategory.Health] = true;
			this.KnownInformation[(int)PawnType.Prisoner, (int)InformationCategory.Needs] = true;
			this.KnownInformation[(int)PawnType.Prisoner, (int)InformationCategory.Gear] = true;
			this.KnownInformation[(int)PawnType.Prisoner, (int)InformationCategory.Skills] = true;
			this.KnownInformation[(int)PawnType.Prisoner, (int)InformationCategory.Traits] = true;
			this.KnownInformation[(int)PawnType.Neutral, (int)InformationCategory.Basic] = true;
		}

		private void InitializeLearningEnabled() {
			this._learningEnabled = new bool[_infoWidth];
			this.SetLearningEnabledFor(PawnType.Slave, true);
			this.SetLearningEnabledFor(PawnType.Prisoner, true);
		}

		private bool KnownInformationIsValid() {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
			Map map = new Map();
			map.info.Size = new IntVec3(_infoWidth, 0, _infoHeight);
			bool output = this.KnownInformation.MapSizeMatches(map);
			return output;
#elif V1_5
			Map map = new Map();
			map.info.Size = new IntVec3(_infoWidth, 0, _infoHeight);
			bool output = this.KnownInformation.MapSizeMatches(map);
			map.Dispose();
			return output;
#else
			if (this.KnownInformation is null) {
				BoundedRationalityLogger.LogErrorMessage("Attempted to check size of null known information.");
				this.KnownInformation = new BoolGrid();
			}

			return this.KnownInformation.Width == _infoWidth && this.KnownInformation.Height == _infoHeight;
#endif
		}

		private bool LearningEnabledIsValid() =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			!(this._learningEnabled is null)
#else
			this._learningEnabled is not null
#endif
			&& this._learningEnabled.Length == _infoWidth;
	}
}
