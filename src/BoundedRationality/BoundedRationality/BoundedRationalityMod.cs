using System;
using System.Linq;

using Lakuna.BoundedRationality.Utility;

using UnityEngine;

using Verse;

namespace Lakuna.BoundedRationality {
	public sealed class BoundedRationalityMod : Mod {
		private const float _checkboxSize = 24;

		private const float _extraScrollHeight = 300; // Must be larger than the maximum height that can be added to the UI in one frame.

		private const float _scrollViewMargin = 18; // Reduce the scroll view by this width so that the vertical scroll bar doesn't overflow the window horizontally.

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static BoundedRationalitySettings Settings {
#else
		public static BoundedRationalitySettings? Settings {
#endif
			get; private set;
		}

		private Vector2 _settingsScrollPosition = Vector2.zero;

		private float _totalSettingsHeight = 99999; // Arbitrarily large number.

		public BoundedRationalityMod(ModContentPack content) : base(content) =>
			Settings = this.GetSettings<BoundedRationalitySettings>();

		public override void DoSettingsWindowContents(Rect inRect) {
			if (Settings is null) {
				BoundedRationalityLogger.LogErrorMessage("Tried to draw settings window contents with null settings.");
				return;
			}

			if (Settings.KnownInformation is null) {
				BoundedRationalityLogger.LogErrorMessage("Tried to draw settings window contents with null known information.");
				return;
			}

			Rect scrollViewRect = new Rect(0, 0, inRect.width - _scrollViewMargin, this._totalSettingsHeight + _extraScrollHeight).Rounded();
			Widgets.BeginScrollView(inRect, ref this._settingsScrollPosition, scrollViewRect);

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			PawnType[] pawnTypes = Enum.GetValues(typeof(PawnType)).OfType<PawnType>().ToArray();
			InformationCategory[] informationCategories = Enum.GetValues(typeof(InformationCategory)).OfType<InformationCategory>().ToArray();
#else
			PawnType[] pawnTypes = [.. Enum.GetValues<PawnType>().OfType<PawnType>()];
			InformationCategory[] informationCategories = [.. Enum.GetValues<InformationCategory>().OfType<InformationCategory>()];
#endif

			int rowCount = informationCategories.Length + 1; // Add one blank row for the column labels.
			float labelHeight = Text.LineHeight;
			float rowHeight = Math.Max(labelHeight, _checkboxSize);
			Rect tableRect = new Rect(scrollViewRect.x, scrollViewRect.y, scrollViewRect.width, Settings.LegacyMode ? 0 : rowHeight * rowCount).Rounded();
			Rect learningEnabledLabelRect = new Rect(scrollViewRect.x, tableRect.yMax, scrollViewRect.width, Settings.LegacyMode ? 0 : labelHeight).Rounded();
			Rect learningEnabledRect = new Rect(tableRect.x, learningEnabledLabelRect.yMax, tableRect.width, Settings.LegacyMode ? 0 : rowHeight).Rounded(); // Horizontally align the "learning enabled" rectangle with the table so that the column labels can be reused.

			if (!Settings.LegacyMode) {
				int columnCount = pawnTypes.Length + 1; // Add one blank column for the row labels.
				float columnWidth = tableRect.width / columnCount;

				// Draw column labels.
				for (int i = 0; i < pawnTypes.Length; i++) {
					Rect columnRect = new Rect(tableRect.x + columnWidth * (i + 1), tableRect.y, columnWidth, tableRect.height).Rounded(); // Add one blank column for the row labels.
					Rect labelRect = new Rect(columnRect.x, columnRect.y, columnRect.width, rowHeight).Rounded();
					Widgets.Label(labelRect, $"BR.{pawnTypes[i]}".Translate().CapitalizeFirst());

					// Draw label tooltips.
					if (Mouse.IsOver(labelRect)) {
#if V1_0
						TooltipHandler.TipRegion(labelRect, $"{$"BR.{pawnTypes[i]}.Desc".Translate()?.CapitalizeFirst() ?? "null"}.");
#else
						TooltipHandler.TipRegion(labelRect, $"BR.{pawnTypes[i]}.Desc".Translate().CapitalizeFirst().EndWithPeriod());
#endif
					}
				}

				// Draw rows.
				for (int i = 0; i < informationCategories.Length; i++) {
					// Draw row label.
					Rect rowRect = new Rect(tableRect.x, tableRect.y + rowHeight * (i + 1), tableRect.width, rowHeight).Rounded(); // Add one blank row for the column labels.
					Rect labelRect = new Rect(rowRect.x, rowRect.y, columnWidth, rowRect.height).Rounded();
					Widgets.Label(labelRect, $"BR.{informationCategories[i]}".Translate().CapitalizeFirst());

					// Draw label tooltips.
					if (Mouse.IsOver(labelRect)) {
#if V1_0
						TooltipHandler.TipRegion(labelRect, $"{$"BR.{informationCategories[i]}.Desc".Translate()?.CapitalizeFirst() ?? "null"}.");
#else
						TooltipHandler.TipRegion(labelRect, $"BR.{informationCategories[i]}.Desc".Translate().CapitalizeFirst().EndWithPeriod());
#endif
					}

					// Draw checkboxes.
					for (int j = 0; j < pawnTypes.Length; j++) {
						Rect checkboxRect = new Rect(rowRect.x + columnWidth * (j + 1), rowRect.y, columnWidth, rowRect.height).Rounded(); // Add one blank column for the row labels.

						bool value = Settings.KnownInformation[(int)pawnTypes[j], (int)informationCategories[i]];
						Widgets.Checkbox(checkboxRect.min, ref value, Math.Min(checkboxRect.width, checkboxRect.height));
						Settings.KnownInformation[(int)pawnTypes[j], (int)informationCategories[i]] = value;

						// Draw checkbox tooltips.
						if (Mouse.IsOver(checkboxRect)) {
#if V1_0
							TooltipHandler.TipRegion(checkboxRect, $"{"BR.ToggleInformationAboutFor".Translate($"BR.{informationCategories[i]}.Desc".Translate() ?? "null", $"BR.{pawnTypes[j]}.Desc".Translate() ?? "null")?.CapitalizeFirst() ?? "null"}.");
#else
							TooltipHandler.TipRegion(checkboxRect, "BR.ToggleInformationAboutFor".Translate($"BR.{informationCategories[i]}.Desc".Translate(), $"BR.{pawnTypes[j]}.Desc".Translate()).CapitalizeFirst().EndWithPeriod());
#endif
						}
					}
				}

				// Draw "learning enabled" label.
				Widgets.Label(learningEnabledLabelRect, "BR.LearningEnabled".Translate().CapitalizeFirst());
				if (Mouse.IsOver(learningEnabledLabelRect)) {
#if V1_0
					TooltipHandler.TipRegion(learningEnabledLabelRect, $"{"BR.LearningEnabled.Desc".Translate()?.CapitalizeFirst() ?? "null"}.");
#else
					TooltipHandler.TipRegion(learningEnabledLabelRect, "BR.LearningEnabled.Desc".Translate().CapitalizeFirst().EndWithPeriod());
#endif
				}

				// Draw "learning enabled" row.
				for (int i = 0; i < pawnTypes.Length; i++) {
					Rect checkboxRect = new Rect(learningEnabledRect.x + columnWidth * (i + 1), learningEnabledRect.y, columnWidth, learningEnabledRect.height).Rounded(); // Add one blank column for the row labels.

					bool value = Settings.GetLearningEnabledFor(pawnTypes[i]);
					Widgets.Checkbox(checkboxRect.min, ref value, Math.Min(checkboxRect.width, checkboxRect.height));
					Settings.SetLearningEnabledFor(pawnTypes[i], value);
				}
			}

			Rect listingRect = new Rect(scrollViewRect.x, learningEnabledRect.yMax, scrollViewRect.width, scrollViewRect.height - tableRect.height - learningEnabledLabelRect.height - learningEnabledRect.height).Rounded();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Listing_Standard listing = new Listing_Standard();
#else
			Listing_Standard listing = new();
#endif
			listing.Begin(listingRect);

			if (KnowledgeUtility.IsLearningEnabledForAny(InformationCategory.Traits, true)) {
#if V1_0
				listing.Label($"{"BR.TraitDiscoveryDifficulty".Translate()?.CapitalizeFirst() ?? "null"} ({Settings.TraitsLearningDifficulty})");
#elif V1_1 || V1_2 || V1_3
				listing.Label($"{"BR.TraitDiscoveryDifficulty".Translate().CapitalizeFirst().StringSafe()} ({Settings.TraitsLearningDifficulty})");
				Settings.TraitsLearningDifficulty = (int)listing.Slider(Settings.TraitsLearningDifficulty, 0, 10);
#elif V1_4 || V1_5 || V1_6
				Settings.TraitsLearningDifficulty = (int)listing.SliderLabeled($"{"BR.TraitDiscoveryDifficulty".Translate().CapitalizeFirst().StringSafe()} ({Settings.TraitsLearningDifficulty})", Settings.TraitsLearningDifficulty, 0, 10, tooltip: "BR.TraitDiscoveryDifficulty.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe());
#else
				Settings.TraitsLearningDifficulty = (int)listing.SliderLabeled($"{"BR.TraitDiscoveryDifficulty".Translate().CapitalizeFirst().StringSafe} ({Settings.TraitsLearningDifficulty})", Settings.TraitsLearningDifficulty, 0, 10, tooltip: "BR.TraitDiscoveryDifficulty.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe);
#endif
			}

			if (KnowledgeUtility.IsLearningEnabledForAny(InformationCategory.Backstory, true)) {
#if V1_0
				listing.Label($"{"BR.BackstoryDiscoveryDifficulty".Translate()?.CapitalizeFirst() ?? "null"} ({Settings.BackstoryLearningDifficulty})");
				Settings.BackstoryLearningDifficulty = (int)listing.Slider(Settings.BackstoryLearningDifficulty, 0, 10);
#elif V1_1 || V1_2 || V1_3
				listing.Label($"{"BR.BackstoryDiscoveryDifficulty".Translate().CapitalizeFirst().StringSafe()} ({Settings.BackstoryLearningDifficulty})");
				Settings.BackstoryLearningDifficulty = (int)listing.Slider(Settings.BackstoryLearningDifficulty, 0, 10);
#elif V1_4 || V1_5 || V1_6
				Settings.BackstoryLearningDifficulty = (int)listing.SliderLabeled($"{"BR.BackstoryDiscoveryDifficulty".Translate().CapitalizeFirst().StringSafe()} ({Settings.BackstoryLearningDifficulty})", Settings.BackstoryLearningDifficulty, 0, 10, tooltip: "BR.BackstoryDiscoveryDifficulty.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe());
#else
				Settings.BackstoryLearningDifficulty = (int)listing.SliderLabeled($"{"BR.BackstoryDiscoveryDifficulty".Translate().CapitalizeFirst().StringSafe} ({Settings.BackstoryLearningDifficulty})", Settings.BackstoryLearningDifficulty, 0, 10, tooltip: "BR.BackstoryDiscoveryDifficulty.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe);
#endif
			}

			if (KnowledgeUtility.IsLearningEnabledForAny(InformationCategory.Skills, true)) {
#if V1_0
				listing.Label($"{"BR.SkillsDiscoveryDifficulty".Translate()?.CapitalizeFirst() ?? "null"} ({Settings.SkillsLearningDifficulty})");
				Settings.SkillsLearningDifficulty = (int)listing.Slider(Settings.SkillsLearningDifficulty, 0, 10);
#elif V1_1 || V1_2 || V1_3
				listing.Label($"{"BR.SkillsDiscoveryDifficulty".Translate().CapitalizeFirst().StringSafe()} ({Settings.SkillsLearningDifficulty})");
				Settings.SkillsLearningDifficulty = (int)listing.Slider(Settings.SkillsLearningDifficulty, 0, 10);
#elif V1_4 || V1_5 || V1_6
				Settings.SkillsLearningDifficulty = (int)listing.SliderLabeled($"{"BR.SkillsDiscoveryDifficulty".Translate().CapitalizeFirst().StringSafe()} ({Settings.SkillsLearningDifficulty})", Settings.SkillsLearningDifficulty, 0, 10, tooltip: "BR.SkillsDiscoveryDifficulty.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe());
#else
				Settings.SkillsLearningDifficulty = (int)listing.SliderLabeled($"{"BR.SkillsDiscoveryDifficulty".Translate().CapitalizeFirst().StringSafe} ({Settings.SkillsLearningDifficulty})", Settings.SkillsLearningDifficulty, 0, 10, tooltip: "BR.SkillsDiscoveryDifficulty.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe);
#endif
			}

			if (!Settings.LegacyMode && KnowledgeUtility.IsLearningEnabledForAny(InformationCategory.Traits)) {
				bool enableUniqueTraitUnlockConditions = Settings.EnableUniqueTraitUnlockConditions;
#if V1_0
				listing.CheckboxLabeled("BR.EnableUniqueTraitUnlockConditions".Translate()?.CapitalizeFirst() ?? "null", ref enableUniqueTraitUnlockConditions, $"{"BR.EnableUniqueTraitUnlockConditions.Desc".Translate()?.CapitalizeFirst() ?? "null"}.");
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				listing.CheckboxLabeled("BR.EnableUniqueTraitUnlockConditions".Translate().CapitalizeFirst().StringSafe(), ref enableUniqueTraitUnlockConditions, "BR.EnableUniqueTraitUnlockConditions.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe());
#else
				listing.CheckboxLabeled("BR.EnableUniqueTraitUnlockConditions".Translate().CapitalizeFirst().StringSafe, ref enableUniqueTraitUnlockConditions, "BR.EnableUniqueTraitUnlockConditions.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe);
#endif
				Settings.EnableUniqueTraitUnlockConditions = enableUniqueTraitUnlockConditions;
			}

			bool isAllInformationKnownForColonists = KnowledgeUtility.IsAllInformationKnownFor(PawnType.Colonist);
			if (!isAllInformationKnownForColonists || KnowledgeUtility.IsAnyLearningEnabledFor(PawnType.Colonist)) {
				bool alwaysKnowStartingColonists = Settings.AlwaysKnowStartingColonists;
#if V1_0
				listing.CheckboxLabeled("BR.AlwaysKnowStartingColonists".Translate()?.CapitalizeFirst() ?? "null", ref alwaysKnowStartingColonists, $"{"BR.AlwaysKnowStartingColonists.Desc".Translate()?.CapitalizeFirst() ?? "null"}.");
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				listing.CheckboxLabeled("BR.AlwaysKnowStartingColonists".Translate().CapitalizeFirst().StringSafe(), ref alwaysKnowStartingColonists, "BR.AlwaysKnowStartingColonists.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe());
#else
				listing.CheckboxLabeled("BR.AlwaysKnowStartingColonists".Translate().CapitalizeFirst().StringSafe, ref alwaysKnowStartingColonists, "BR.AlwaysKnowStartingColonists.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe);
#endif
				Settings.AlwaysKnowStartingColonists = alwaysKnowStartingColonists;
			}

			bool isAnyLearningEnabledForAny = KnowledgeUtility.IsAnyLearningEnabledForAny();
			if (!KnowledgeUtility.IsInformationSupersetOfAny(PawnType.Colonist) || isAnyLearningEnabledForAny) {
				bool rememberFormerColonists = Settings.RememberFormerColonists;
#if V1_0
				listing.CheckboxLabeled("BR.RememberFormerColonists".Translate()?.CapitalizeFirst() ?? "null", ref rememberFormerColonists, $"{"BR.RememberFormerColonists.Desc".Translate()?.CapitalizeFirst() ?? "null"}.");
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				listing.CheckboxLabeled("BR.RememberFormerColonists".Translate().CapitalizeFirst().StringSafe(), ref rememberFormerColonists, "BR.RememberFormerColonists.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe());
#else
				listing.CheckboxLabeled("BR.RememberFormerColonists".Translate().CapitalizeFirst().StringSafe, ref rememberFormerColonists, "BR.RememberFormerColonists.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe);
#endif
				Settings.RememberFormerColonists = rememberFormerColonists;
			}

			if (!KnowledgeUtility.IsInformationKnownForAll(InformationCategory.Basic) || !KnowledgeUtility.IsInformationKnownForAll(InformationCategory.Traits) || !KnowledgeUtility.IsInformationKnownForAll(InformationCategory.Backstory)) {
				bool alwaysKnowMoreAboutColonistRelatives = Settings.AlwaysKnowMoreAboutColonistRelatives;
#if V1_0
				listing.CheckboxLabeled("BR.AlwaysKnowMoreAboutColonistRelatives".Translate()?.CapitalizeFirst() ?? "null", ref alwaysKnowMoreAboutColonistRelatives, $"{"BR.AlwaysKnowMoreAboutColonistRelatives.Desc".Translate()?.CapitalizeFirst() ?? "null"}.");
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				listing.CheckboxLabeled("BR.AlwaysKnowMoreAboutColonistRelatives".Translate().CapitalizeFirst().StringSafe(), ref alwaysKnowMoreAboutColonistRelatives, "BR.AlwaysKnowMoreAboutColonistRelatives.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe());
#else
				listing.CheckboxLabeled("BR.AlwaysKnowMoreAboutColonistRelatives".Translate().CapitalizeFirst().StringSafe, ref alwaysKnowMoreAboutColonistRelatives, "BR.AlwaysKnowMoreAboutColonistRelatives.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe);
#endif
				Settings.AlwaysKnowMoreAboutColonistRelatives = alwaysKnowMoreAboutColonistRelatives;
			}

			if (KnowledgeUtility.IsLearningEnabledFor(InformationCategory.Traits, PawnType.Colonist)) {
				bool alwaysKnowGrowthMoments = Settings.AlwaysKnowGrowthMomentTraits;
#if V1_0
				listing.CheckboxLabeled("BR.AlwaysKnowGrowthMomentTraits".Translate()?.CapitalizeFirst() ?? "null", ref alwaysKnowGrowthMoments, $"{"BR.AlwaysKnowGrowthMomentTraits.Desc".Translate()?.CapitalizeFirst() ?? "null"}.");
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				listing.CheckboxLabeled("BR.AlwaysKnowGrowthMomentTraits".Translate().CapitalizeFirst().StringSafe(), ref alwaysKnowGrowthMoments, "BR.AlwaysKnowGrowthMomentTraits.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe());
#else
				listing.CheckboxLabeled("BR.AlwaysKnowGrowthMomentTraits".Translate().CapitalizeFirst().StringSafe, ref alwaysKnowGrowthMoments, "BR.AlwaysKnowGrowthMomentTraits.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe);
#endif
				Settings.AlwaysKnowGrowthMomentTraits = alwaysKnowGrowthMoments;
			}

			if (!isAllInformationKnownForColonists || !KnowledgeUtility.IsAllInformationKnownFor(PawnType.Controlled) || !KnowledgeUtility.IsAllInformationKnownFor(PawnType.Slave)) {
				bool neverHideControls = Settings.NeverHideControls;
#if V1_0
				listing.CheckboxLabeled("BR.NeverHideControls".Translate()?.CapitalizeFirst() ?? "null", ref neverHideControls, $"{"BR.NeverHideControls.Desc".Translate()?.CapitalizeFirst() ?? "null"}.");
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				listing.CheckboxLabeled("BR.NeverHideControls".Translate().CapitalizeFirst().StringSafe(), ref neverHideControls, "BR.NeverHideControls.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe());
#else
				listing.CheckboxLabeled("BR.NeverHideControls".Translate().CapitalizeFirst().StringSafe, ref neverHideControls, "BR.NeverHideControls.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe);
#endif
				Settings.NeverHideControls = neverHideControls;
			}

			bool isAllInformationKnownForAll = KnowledgeUtility.IsAllInformationKnownForAll();
			if (!isAllInformationKnownForAll) {
				bool neverHideMessages = Settings.NeverHideMessages;
#if V1_0
				listing.CheckboxLabeled("BR.NeverHideMessages".Translate()?.CapitalizeFirst() ?? "null", ref neverHideMessages, $"{"BR.NeverHideMessages.Desc".Translate()?.CapitalizeFirst() ?? "null"}.");
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				listing.CheckboxLabeled("BR.NeverHideMessages".Translate().CapitalizeFirst().StringSafe(), ref neverHideMessages, "BR.NeverHideMessages.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe());
#else
				listing.CheckboxLabeled("BR.NeverHideMessages".Translate().CapitalizeFirst().StringSafe, ref neverHideMessages, "BR.NeverHideMessages.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe);
#endif
				Settings.NeverHideMessages = neverHideMessages;

				bool neverHideLetters = Settings.NeverHideLetters;
#if V1_0
				listing.CheckboxLabeled("BR.NeverHideLetters".Translate()?.CapitalizeFirst() ?? "null", ref neverHideLetters, $"{"BR.NeverHideLetters.Desc".Translate()?.CapitalizeFirst() ?? "null"}.");
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				listing.CheckboxLabeled("BR.NeverHideLetters".Translate().CapitalizeFirst().StringSafe(), ref neverHideLetters, "BR.NeverHideLetters.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe());
#else
				listing.CheckboxLabeled("BR.NeverHideLetters".Translate().CapitalizeFirst().StringSafe, ref neverHideLetters, "BR.NeverHideLetters.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe);
#endif
				Settings.NeverHideLetters = neverHideLetters;

				bool neverHideTextMotes = Settings.NeverHideTextMotes;
#if V1_0
				listing.CheckboxLabeled("BR.NeverHideTextMotes".Translate()?.CapitalizeFirst() ?? "null", ref neverHideTextMotes, $"{"BR.NeverHideTextMotes.Desc".Translate()?.CapitalizeFirst() ?? "null"}.");
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				listing.CheckboxLabeled("BR.NeverHideTextMotes".Translate().CapitalizeFirst().StringSafe(), ref neverHideTextMotes, "BR.NeverHideTextMotes.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe());
#else
				listing.CheckboxLabeled("BR.NeverHideTextMotes".Translate().CapitalizeFirst().StringSafe, ref neverHideTextMotes, "BR.NeverHideTextMotes.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe);
#endif
				Settings.NeverHideTextMotes = neverHideTextMotes;

				bool neverHideAlerts = Settings.NeverHideAlerts;
#if V1_0
				listing.CheckboxLabeled("BR.NeverHideAlerts".Translate()?.CapitalizeFirst() ?? "null", ref neverHideAlerts, $"{"BR.NeverHideAlerts.Desc".Translate()?.CapitalizeFirst() ?? "null"}.");
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				listing.CheckboxLabeled("BR.NeverHideAlerts".Translate().CapitalizeFirst().StringSafe(), ref neverHideAlerts, "BR.NeverHideAlerts.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe());
#else
				listing.CheckboxLabeled("BR.NeverHideAlerts".Translate().CapitalizeFirst().StringSafe, ref neverHideAlerts, "BR.NeverHideAlerts.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe);
#endif
				Settings.NeverHideAlerts = neverHideAlerts;

				bool hideFactionInformation = Settings.HideFactionInformation;
#if V1_0
				listing.CheckboxLabeled("BR.HideFactionInformation".Translate()?.CapitalizeFirst() ?? "null", ref hideFactionInformation, $"{"BR.HideFactionInformation.Desc".Translate()?.CapitalizeFirst() ?? "null"}.");
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				listing.CheckboxLabeled("BR.HideFactionInformation".Translate().CapitalizeFirst().StringSafe(), ref hideFactionInformation, "BR.HideFactionInformation.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe());
#else
				listing.CheckboxLabeled("BR.HideFactionInformation".Translate().CapitalizeFirst().StringSafe, ref hideFactionInformation, "BR.HideFactionInformation.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe);
#endif
				Settings.HideFactionInformation = hideFactionInformation;
			}

			if (pawnTypes.Any((type) => !KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, type) && !KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, type))) {
				bool neverHidePawnLabels = Settings.NeverHidePawnLabels;
#if V1_0
				listing.CheckboxLabeled("BR.NeverHidePawnLabels".Translate()?.CapitalizeFirst() ?? "null", ref neverHidePawnLabels, $"{"BR.NeverHidePawnLabels.Desc".Translate()?.CapitalizeFirst() ?? "null"}.");
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				listing.CheckboxLabeled("BR.NeverHidePawnLabels".Translate().CapitalizeFirst().StringSafe(), ref neverHidePawnLabels, "BR.NeverHidePawnLabels.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe());
#else
				listing.CheckboxLabeled("BR.NeverHidePawnLabels".Translate().CapitalizeFirst().StringSafe, ref neverHidePawnLabels, "BR.NeverHidePawnLabels.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe);
#endif
				Settings.NeverHidePawnLabels = neverHidePawnLabels;
			}

			if (!KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, PawnType.Colonist)) {
				bool preventForcedSpeed = Settings.PreventForcedSpeed;
#if V1_0
				listing.CheckboxLabeled("BR.PreventForcedSpeed".Translate()?.CapitalizeFirst() ?? "null", ref preventForcedSpeed, $"{"BR.PreventForcedSpeed.Desc".Translate()?.CapitalizeFirst() ?? "null"}.");
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				listing.CheckboxLabeled("BR.PreventForcedSpeed".Translate().CapitalizeFirst().StringSafe(), ref preventForcedSpeed, "BR.PreventForcedSpeed.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe());
#else
				listing.CheckboxLabeled("BR.PreventForcedSpeed".Translate().CapitalizeFirst().StringSafe, ref preventForcedSpeed, "BR.PreventForcedSpeed.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe);
#endif
				Settings.PreventForcedSpeed = preventForcedSpeed;
			}

			if (!Settings.LegacyMode) {
				bool hideAncientCorpses = Settings.HideAncientCorpses;
#if V1_0
				listing.CheckboxLabeled("BR.HideAncientCorpses".Translate()?.CapitalizeFirst() ?? "null", ref hideAncientCorpses, $"{"BR.HideAncientCorpses.Desc".Translate()?.CapitalizeFirst() ?? "null"}.");
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				listing.CheckboxLabeled("BR.HideAncientCorpses".Translate().CapitalizeFirst().StringSafe(), ref hideAncientCorpses, "BR.HideAncientCorpses.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe());
#else
				listing.CheckboxLabeled("BR.HideAncientCorpses".Translate().CapitalizeFirst().StringSafe, ref hideAncientCorpses, "BR.HideAncientCorpses.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe);
#endif
				Settings.HideAncientCorpses = hideAncientCorpses;
			}

			bool legacyMode = Settings.LegacyMode;
#if V1_0
			listing.CheckboxLabeled("BR.LegacyMode".Translate()?.CapitalizeFirst() ?? "null", ref legacyMode, $"{"BR.LegacyMode.Desc".Translate()?.CapitalizeFirst() ?? "null"}.");
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			listing.CheckboxLabeled("BR.LegacyMode".Translate().CapitalizeFirst().StringSafe(), ref legacyMode, "BR.LegacyMode.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe());
#else
			listing.CheckboxLabeled("BR.LegacyMode".Translate().CapitalizeFirst().StringSafe, ref legacyMode, "BR.LegacyMode.Desc".Translate().CapitalizeFirst().EndWithPeriod().StringSafe);
#endif
			Settings.LegacyMode = legacyMode;

			if (!Settings.AlwaysKnowStartingColonists && !KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, PawnType.Colonist, ControlCategory.Control)) {
#if V1_0
				listing.Label("BR.WarningDisabledBasicForStartingColonists".Translate()?.CapitalizeFirst() ?? "null");
#elif V1_1 || V1_2
				_ = listing.Label("BR.WarningDisabledBasicForStartingColonists".Translate().CapitalizeFirst().EndWithPeriod().StringSafe().Colorize(ColoredText.WarningColor) ?? "null");
#else
				_ = listing.Label("BR.WarningDisabledBasicForStartingColonists".Translate().CapitalizeFirst().EndWithPeriod().Colorize(ColoredText.WarningColor) ?? "null");
#endif
			}

			if (isAllInformationKnownForAll && !isAnyLearningEnabledForAny && (!Settings.HideAncientCorpses || Settings.LegacyMode)) {
#if V1_0
				listing.Label("BR.NoteModHasNoEffect".Translate()?.CapitalizeFirst() ?? "null");
#elif V1_1 || V1_2
				_ = listing.Label("BR.NoteModHasNoEffect".Translate().CapitalizeFirst().StringSafe().Colorize(ColoredText.WarningColor) ?? "null");
#else
				_ = listing.Label("BR.NoteModHasNoEffect".Translate().CapitalizeFirst().Colorize(ColoredText.WarningColor) ?? "null");
#endif
			}

			listing.End();
			this._totalSettingsHeight = listingRect.y + listing.CurHeight;
			Widgets.EndScrollView();
		}

		public override string SettingsCategory() =>
#if V1_0
			"BR.BoundedRationality".Translate()?.CapitalizeAsTitleSafe() ?? "null";
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			"BR.BoundedRationality".Translate().StringSafe().CapitalizeAsTitleSafe();
#else
			"BR.BoundedRationality".Translate().StringSafe.CapitalizeAsTitleSafe;
#endif
	}
}
