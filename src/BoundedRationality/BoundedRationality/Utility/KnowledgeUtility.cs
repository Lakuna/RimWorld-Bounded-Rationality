using System;
using System.Linq;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Utility {
	/// <summary>
	/// A static utility class that contains static utility methods for checking whether information should be known.
	/// </summary>
	public static class KnowledgeUtility {
		/// <summary>
		/// The amount of nutrition that an unmodified human pawn must consume daily.
		/// </summary>
		private const float _humanDailyNutrition = 1.6f;

		/// <summary>
		/// The maximum amount of health that an unmodified human pawn can have.
		/// </summary>
		private const int _humanMaxHealth = 100;

		/// <summary>
		/// The number of ticks per in-game day.
		/// </summary>
		private const int _ticksPerDay = 24 * _ticksPerHour;

		/// <summary>
		/// The number of ticks per in-game hour.
		/// </summary>
		private const int _ticksPerHour = 2500;

		/// <summary>
		/// The number of ticks per in-game quadrum (in-game month/in-game season).
		/// </summary>
		private const int _ticksPerQuadrum = 15 * _ticksPerDay;

		/// <summary>
		/// Get the trait definition of the wimp trait.
		/// </summary>
		/// <returns>The trait definition of the wimp trait.</returns>
#if V1_0 || V1_1 || V1_2
		private static TraitDef _traitDefOfWimp =>
			null;
#elif V1_3 || V1_4 || V1_5 || V1_6
		private static TraitDef _traitDefOfWimp =>
			TraitDefOf.Wimp;
#else
		private static TraitDef? _traitDefOfWimp =>
			TraitDefOf.Wimp;
#endif

		/// <summary>
		/// Determine whether all of the relevant skill types for the given work type are known for the given pawn.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <param name="workType">The work type.</param>
		/// <returns>Whether all of the relevant skill types for the given work type are known for the given pawn.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static bool AreAllSkillsForWorkTypeKnown(Pawn pawn, WorkTypeDef workType) =>
#else
		public static bool AreAllSkillsForWorkTypeKnown(Pawn? pawn, WorkTypeDef? workType) =>
#endif
			workType?.relevantSkills?.All((skill) => IsSkillKnown(pawn, skill)) ?? false;

		/// <summary>
		/// Determine whether the given backstory is known for the given pawn.
		/// </summary>
		/// <param name="backstory">The backstory.</param>
		/// <param name="pawn">The pawn.</param>
		/// <returns>Whether the given backstory is known for the given pawn.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3
		public static bool IsBackstoryKnown(Backstory backstory, Pawn pawn) =>
#elif V1_4 || V1_5 || V1_6
		public static bool IsBackstoryKnown(BackstoryDef backstory, Pawn pawn) =>
#else
		public static bool IsBackstoryKnown(BackstoryDef? backstory, Pawn? pawn) =>
#endif
			backstory is null
			|| IsBackstoryKnown(backstory.slot, pawn);

		/// <summary>
		/// Determine whether the given backstory is known for the given pawn.
		/// </summary>
		/// <param name="backstory">The backstory.</param>
		/// <param name="pawn">The pawn.</param>
		/// <returns>Whether the given backstory is known for the given pawn.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static bool IsBackstoryKnown(BackstorySlot backstory, Pawn pawn) {
#else
		public static bool IsBackstoryKnown(BackstorySlot? backstory, Pawn? pawn) {
#endif
			if (!IsInformationKnownFor(InformationCategory.Backstory, pawn)) {
				return false;
			}

			if (pawn is null || !IsLearningEnabledFor(InformationCategory.Backstory, pawn)) {
				return true;
			}

			int order = backstory == BackstorySlot.Adulthood ? 1 : 2;
			return MiscellaneousUtility.TimeAsColonistOrPrisoner(pawn) > order * _ticksPerQuadrum * (BoundedRationalityMod.Settings?.BackstoryLearningDifficulty ?? 0); // Backstory unlocked after one slot per quadrum per difficulty (more recent slots first).
		}

		/// <summary>
		/// Determine whether all information categories are known for the given pawn type.
		/// </summary>
		/// <param name="type">The pawn type.</param>
		/// <returns>Whether all information categories are known for the given pawn type.</returns>
		public static bool IsAllInformationKnownFor(PawnType type) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Enum.GetValues(typeof(InformationCategory)).OfType<InformationCategory>().All((category) =>
#else
			Enum.GetValues<InformationCategory>().OfType<InformationCategory>().All((category) =>
#endif
				IsInformationKnownFor(category, type));

		/// <summary>
		/// Determine whether all information is known for all pawn types.
		/// </summary>
		/// <returns>Whether all information is known for all pawn types.</returns>
		public static bool IsAllInformationKnownForAll() =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Enum.GetValues(typeof(PawnType)).OfType<PawnType>().All(IsAllInformationKnownFor);
#else
			Enum.GetValues<PawnType>().OfType<PawnType>().All(IsAllInformationKnownFor);
#endif

		/// <summary>
		/// Determine whether learning all applicable information categories is enabled for the given pawn type.
		/// </summary>
		/// <param name="type">The pawn type.</param>
		/// <param name="ignoreDifficulty">Whether the set learning difficulty for the information category should be ignored.</param>
		/// <returns>Whether learning all applicable information categories is enabled for the given pawn type.</returns>
		public static bool IsAllLearningEnabledFor(PawnType type, bool ignoreDifficulty = false) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Enum.GetValues(typeof(InformationCategory)).OfType<InformationCategory>()
#else
			Enum.GetValues<InformationCategory>().OfType<InformationCategory>()
#endif
				.Where((category) => category == InformationCategory.Backstory
					|| category == InformationCategory.Skills
					|| category == InformationCategory.Traits) // Filter to only information categories for which learning has been implemented.
				.All((category) => IsLearningEnabledFor(category, type, ignoreDifficulty));

		/// <summary>
		/// Determine whether any information category is known for the given pawn type.
		/// </summary>
		/// <param name="type">The pawn type.</param>
		/// <returns>Whether any information category is known for the given pawn type.</returns>
		public static bool IsAnyInformationKnownFor(PawnType type) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Enum.GetValues(typeof(InformationCategory)).OfType<InformationCategory>().Any((category) =>
#else
			Enum.GetValues<InformationCategory>().OfType<InformationCategory>().Any((category) =>
#endif
				IsInformationKnownFor(category, type));

		/// <summary>
		/// Determine whether learning any information category is enabled for the given pawn type.
		/// </summary>
		/// <param name="type">The pawn type.</param>
		/// <param name="ignoreDifficulty">Whether the set learning difficulty for the information category should be ignored.</param>
		/// <returns>Whether learning any information category is enabled for the given pawn type.</returns>
		public static bool IsAnyLearningEnabledFor(PawnType type, bool ignoreDifficulty = false) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Enum.GetValues(typeof(InformationCategory)).OfType<InformationCategory>().Any((category) =>
#else
			Enum.GetValues<InformationCategory>().OfType<InformationCategory>().Any((category) =>
#endif
				IsLearningEnabledFor(category, type, ignoreDifficulty));

		/// <summary>
		/// Determine whether learning any information category is enabled for any pawn type.
		/// </summary>
		/// <param name="ignoreDifficulty">Whether the set learning difficulty for the information category should be ignored.</param>
		/// <returns>Whether learning the given information category is enabled for any pawn type.</returns>
		public static bool IsAnyLearningEnabledForAny(bool ignoreDifficulty = false) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Enum.GetValues(typeof(PawnType)).OfType<PawnType>().Any((type) =>
#else
			Enum.GetValues<PawnType>().OfType<PawnType>().Any((type) =>
#endif
				IsAnyLearningEnabledFor(type, ignoreDifficulty));

		/// <summary>
		/// Determine whether the given information category is known for the given thing (pawn).
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="thing">The thing (pawn).</param>
		/// <param name="controlCategory">The type of control or notification that contains the obscured information.</param>
		/// <returns>Whether the given information category is known for the given thing (pawn).</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static bool IsInformationKnownFor(InformationCategory category, Thing thing, ControlCategory controlCategory = ControlCategory.Default) =>
#else
		public static bool IsInformationKnownFor(InformationCategory category, Thing? thing, ControlCategory controlCategory = ControlCategory.Default) =>
#endif
			thing is null
			|| thing is Pawn pawn && IsInformationKnownFor(category, pawn, controlCategory)
			|| thing is Corpse corpse && IsInformationKnownFor(category, corpse.InnerPawn, controlCategory);

		/// <summary>
		/// Determine whether the given information category is known for the given pawn.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="pawn">The pawn.</param>
		/// <param name="controlCategory">The type of control or notification that contains the obscured information.</param>
		/// <returns>Whether the given information category is known for the given pawn.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static bool IsInformationKnownFor(InformationCategory category, Pawn pawn, ControlCategory controlCategory = ControlCategory.Default) =>
#else
		public static bool IsInformationKnownFor(InformationCategory category, Pawn? pawn, ControlCategory controlCategory = ControlCategory.Default) =>
#endif
			pawn is null
			|| ((BoundedRationalityMod.Settings?.AlwaysKnowStartingColonists ?? false)
				&& MiscellaneousUtility.IsStartingColonist(pawn)
				|| IsInformationKnownFor(category, MiscellaneousUtility.TypeOf(pawn), controlCategory, !pawn.Dead)
				|| (BoundedRationalityMod.Settings?.AlwaysKnowMoreAboutColonistRelatives ?? false)
				&& MiscellaneousUtility.IsRelativeOfColonist(pawn) && IsStatic(category))
			&& !(!(BoundedRationalityMod.Settings?.LegacyMode ?? false)
				&& (BoundedRationalityMod.Settings?.HideAncientCorpses ?? false)
				&& MiscellaneousUtility.IsAncientCorpse(pawn));

		/// <summary>
		/// Determine whether the given information category is known for the given faction.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="faction">The faction.</param>
		/// <param name="controlCategory">The type of control or notification that contains the obscured information.</param>
		/// <returns>Whether the given information category is known for the given faction.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static bool IsInformationKnownFor(InformationCategory category, Faction faction, ControlCategory controlCategory = ControlCategory.Default) =>
#else
		public static bool IsInformationKnownFor(InformationCategory category, Faction? faction, ControlCategory controlCategory = ControlCategory.Default) =>
#endif
			faction is null
			|| !(BoundedRationalityMod.Settings?.HideFactionInformation ?? false)
			|| IsInformationKnownFor(category, MiscellaneousUtility.TypeOf(faction), controlCategory);

		/// <summary>
		/// Determine whether the given information category is known for the given pawn type.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="type">The pawn type.</param>
		/// <param name="controlCategory">The type of control or notification that contains the obscured information.</param>
		/// <param name="isAlive">Whether the pawn is alive.</param>
		/// <returns>Whether the given information category is known for the given pawn type.</returns>
		public static bool IsInformationKnownFor(InformationCategory category, PawnType type, ControlCategory controlCategory = ControlCategory.Default, bool isAlive = true) =>
			(BoundedRationalityMod.Settings?.KnownInformation?[(int)type, (int)category] ?? false)
			|| (BoundedRationalityMod.Settings?.LegacyMode ?? false)
			|| controlCategory == ControlCategory.Control && (BoundedRationalityMod.Settings?.NeverHideControls ?? false) && MiscellaneousUtility.IsPlayerControlled(type, isAlive)
			|| controlCategory == ControlCategory.Letter && (BoundedRationalityMod.Settings?.NeverHideLetters ?? false)
			|| controlCategory == ControlCategory.Message && (BoundedRationalityMod.Settings?.NeverHideMessages ?? false)
			|| controlCategory == ControlCategory.TextMote && (BoundedRationalityMod.Settings?.NeverHideTextMotes ?? false)
			|| controlCategory == ControlCategory.Alert && (BoundedRationalityMod.Settings?.NeverHideAlerts ?? false)
			|| category == InformationCategory.Traits && type == PawnType.Colonist && (BoundedRationalityMod.Settings?.AlwaysKnowGrowthMomentTraits ?? false) && MiscellaneousUtility.IsInGrowthMoment();

		/// <summary>
		/// Determine whether the given information category is known for all pawn types.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <returns>Whether the given information category is known for all pawn types.</returns>
		public static bool IsInformationKnownForAll(InformationCategory category) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Enum.GetValues(typeof(PawnType)).OfType<PawnType>().All((type) =>
#else
			Enum.GetValues<PawnType>().OfType<PawnType>().All((type) =>
#endif
				IsInformationKnownFor(category, type));

		/// <summary>
		/// Determine whether the given information category is known for any pawn type.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <returns>Whether the given information category is known for any pawn type.</returns>
		public static bool IsInformationKnownForAny(InformationCategory category) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Enum.GetValues(typeof(PawnType)).OfType<PawnType>().Any((type) =>
#else
			Enum.GetValues<PawnType>().OfType<PawnType>().Any((type) =>
#endif
				IsInformationKnownFor(category, type));

		/// <summary>
		/// Determine whether or not the information known about one pawn type contains all of the information known about another pawn type.
		/// </summary>
		/// <param name="superset">The pawn type to check as a superset of the other.</param>
		/// <param name="subset">The pawn type to check as a subset of the other.</param>
		/// <returns>Whether or not `superset` is a superset of `subset`.</returns>
		public static bool IsInformationSupersetOf(PawnType superset, PawnType subset) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Enum.GetValues(typeof(InformationCategory)).OfType<InformationCategory>().All((category) =>
#else
			Enum.GetValues<InformationCategory>().OfType<InformationCategory>().All((category) =>
#endif
				IsInformationKnownFor(category, superset) || !IsInformationKnownFor(category, subset));

		/// <summary>
		/// Determine whether or not the information known about one pawn type contains all of the information known about any other pawn type.
		/// </summary>
		/// <param name="superset">The pawn type to check as a superset of the other.</param>
		/// <returns>Whether or not `superset` is a superset of every other pawn type.</returns>
		public static bool IsInformationSupersetOfAny(PawnType superset) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Enum.GetValues(typeof(PawnType)).OfType<PawnType>().Any((subset) =>
#else
			Enum.GetValues<PawnType>().OfType<PawnType>().Any((subset) =>
#endif
				IsInformationSupersetOf(superset, subset));

		/// <summary>
		/// Determine whether or not learning is enabled for the given pawn and information category pair.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="pawn">The pawn.</param>
		/// <param name="ignoreDifficulty">Whether the set learning difficulty for the information category should be ignored.</param>
		/// <returns>Whether or not learning is enabled for the given pawn and information category pair.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static bool IsLearningEnabledFor(InformationCategory category, Pawn pawn, bool ignoreDifficulty = false) =>
#else
		public static bool IsLearningEnabledFor(InformationCategory category, Pawn? pawn, bool ignoreDifficulty = false) =>
#endif
			IsInformationKnownFor(category, pawn)
			&& IsLearningEnabledFor(category, MiscellaneousUtility.TypeOf(pawn), ignoreDifficulty)
			&& !((BoundedRationalityMod.Settings?.AlwaysKnowStartingColonists ?? false) && MiscellaneousUtility.IsStartingColonist(pawn))
			&& !(category == InformationCategory.Traits && (BoundedRationalityMod.Settings?.AlwaysKnowGrowthMomentTraits ?? false) && (pawn is null || MiscellaneousUtility.IsInGrowthMoment()))
			&& !(IsStatic(category) && (BoundedRationalityMod.Settings?.AlwaysKnowMoreAboutColonistRelatives ?? false) && MiscellaneousUtility.IsRelativeOfColonist(pawn));

		/// <summary>
		/// Determine whether or not learning is enabled for the given faction and information category pair.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="faction">The faction.</param>
		/// <param name="ignoreDifficulty">Whether the set learning difficulty for the information category should be ignored.</param>
		/// <returns>Whether or not learning is enabled for the given faction and information category pair.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static bool IsLearningEnabledFor(InformationCategory category, Faction faction, bool ignoreDifficulty = false) =>
#else
		public static bool IsLearningEnabledFor(InformationCategory category, Faction? faction, bool ignoreDifficulty = false) =>
#endif
			IsInformationKnownFor(category, faction)
			&& IsLearningEnabledFor(category, MiscellaneousUtility.TypeOf(faction), ignoreDifficulty);

		/// <summary>
		/// Determine whether or not learning is enabled for the given pawn type and information category pair.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="type">The pawn type.</param>
		/// <param name="ignoreDifficulty">Whether the set learning difficulty for the information category should be ignored.</param>
		/// <returns>Whether or not learning is enabled for the given pawn type and information category pair.</returns>
		public static bool IsLearningEnabledFor(InformationCategory category, PawnType type, bool ignoreDifficulty = false) {
			if (!IsInformationKnownFor(category, type)) {
				return false;
			}

			bool learningEnabled = BoundedRationalityMod.Settings?.GetLearningEnabledFor(type) ?? false;
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			switch (category) {
				case InformationCategory.Backstory:
					return !(BoundedRationalityMod.Settings?.LegacyMode ?? false)
						&& ((BoundedRationalityMod.Settings?.BackstoryLearningDifficulty ?? 0) > 0
							|| ignoreDifficulty)
						&& learningEnabled;
				case InformationCategory.Skills:
					return !(BoundedRationalityMod.Settings?.LegacyMode ?? false)
						&& ((BoundedRationalityMod.Settings?.SkillsLearningDifficulty ?? 0) > 0
							|| ignoreDifficulty)
						&& learningEnabled;
				case InformationCategory.Traits:
					return ((BoundedRationalityMod.Settings?.TraitsLearningDifficulty ?? 0) > 0
						|| ignoreDifficulty)
					&& ((BoundedRationalityMod.Settings?.LegacyMode ?? false)
						|| learningEnabled);
				default:
					return false;
			}
#else
			return category switch {
				InformationCategory.Backstory =>
					!(BoundedRationalityMod.Settings?.LegacyMode ?? false)
					&& ((BoundedRationalityMod.Settings?.BackstoryLearningDifficulty ?? 0) > 0
						|| ignoreDifficulty)
					&& learningEnabled,
				InformationCategory.Skills =>
					!(BoundedRationalityMod.Settings?.LegacyMode ?? false)
					&& ((BoundedRationalityMod.Settings?.SkillsLearningDifficulty ?? 0) > 0
						|| ignoreDifficulty)
					&& learningEnabled,
				InformationCategory.Traits =>
					((BoundedRationalityMod.Settings?.TraitsLearningDifficulty ?? 0) > 0
						|| ignoreDifficulty)
					&& ((BoundedRationalityMod.Settings?.LegacyMode ?? false)
						|| learningEnabled),
				_ => false
			};
#endif
		}

		/// <summary>
		/// Determine whether learning the given information category is enabled for all pawn types.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="ignoreDifficulty">Whether the set learning difficulty for the information category should be ignored.</param>
		/// <returns>Whether learning the given information category is enabled for all pawn types.</returns>
		public static bool IsLearningEnabledForAll(InformationCategory category, bool ignoreDifficulty = false) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Enum.GetValues(typeof(PawnType)).OfType<PawnType>().All((type) =>
#else
			Enum.GetValues<PawnType>().OfType<PawnType>().All((type) =>
#endif
				IsLearningEnabledFor(category, type, ignoreDifficulty));

		/// <summary>
		/// Determine whether learning the given information category is enabled for any pawn type.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="ignoreDifficulty">Whether the set learning difficulty for the information category should be ignored.</param>
		/// <returns>Whether learning the given information category is enabled for any pawn type.</returns>
		public static bool IsLearningEnabledForAny(InformationCategory category, bool ignoreDifficulty = false) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Enum.GetValues(typeof(PawnType)).OfType<PawnType>().Any((type) =>
#else
			Enum.GetValues<PawnType>().OfType<PawnType>().Any((type) =>
#endif
				IsLearningEnabledFor(category, type, ignoreDifficulty));

		/// <summary>
		/// Determine whether the given skill is known.
		/// </summary>
		/// <param name="skill">The skill.</param>
		/// <returns>Whether the given skill is known.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static bool IsSkillKnown(SkillRecord skill) =>
#else
		public static bool IsSkillKnown(SkillRecord? skill) =>
#endif
			skill is null
			|| IsSkillKnown(MiscellaneousUtility.PawnOfSkillRecord(skill), skill.def);

		/// <summary>
		/// Determine whether the given skill type is known for the given pawn.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <param name="skill">The skill type.</param>
		/// <returns>Whether the given skill type is known for the given pawn.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static bool IsSkillKnown(Pawn pawn, SkillDef skill) {
#else
		public static bool IsSkillKnown(Pawn? pawn, SkillDef? skill) {
#endif
			if (!IsInformationKnownFor(InformationCategory.Skills, pawn)) {
				return false;
			}

			if (pawn is null || skill is null || !IsLearningEnabledFor(InformationCategory.Skills, pawn)) {
				return true;
			}

			IOrderedEnumerable<SkillRecord> skills = pawn.skills.skills.OrderByDescending((record) => record.Level);
			int order = skills.FirstIndexOf((record) => record.def == skill) + 1; // Learn the pawn's skills in order of their level (highest first).
			return MiscellaneousUtility.TimeAsColonistOrPrisoner(pawn) > order * 5 * _ticksPerDay * (BoundedRationalityMod.Settings?.SkillsLearningDifficulty ?? 0); // One skill per order per five days per difficulty.
		}

		/// <summary>
		/// Determine whether or not the given information category represents unchanging information.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <returns>Whether or not the given information category represents unchanging information.</returns>
		public static bool IsStatic(InformationCategory category) =>
			category == InformationCategory.Backstory
			|| category == InformationCategory.Basic
			|| category == InformationCategory.Traits;

		/// <summary>
		/// Determine whether the given thought is known.
		/// </summary>
		/// <param name="thought">The thought.</param>
		/// <returns>Whether the given thought is known.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static bool IsThoughtKnown(Thought thought) =>
#else
		public static bool IsThoughtKnown(Thought? thought) =>
#endif
			thought is null
			|| IsThoughtKnown(thought.pawn, thought.def);

		/// <summary>
		/// Determine whether the given thought type is known for the given pawn.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <param name="thought">The thought type.</param>
		/// <returns>Whether the given thought type is known for the given pawn.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3
		public static bool IsThoughtKnown(Pawn pawn, ThoughtDef thought) =>
			IsInformationKnownFor(InformationCategory.Needs, pawn)
				&& !(thought?.requiredTraits?.Any((traitDef) => !IsTraitKnown(pawn, traitDef)) ?? false);
#elif V1_4
		public static bool IsThoughtKnown(Pawn pawn, ThoughtDef thought) =>
			IsInformationKnownFor(InformationCategory.Needs, pawn)
				&& !(thought?.requiredGenes?.Count > 0 && !IsInformationKnownFor(InformationCategory.Personal, pawn))
				&& !(thought?.requiredTraits?.Any((traitDef) => !IsTraitKnown(pawn, traitDef)) ?? false);
#elif V1_5 || V1_6
		public static bool IsThoughtKnown(Pawn pawn, ThoughtDef thought) =>
			IsInformationKnownFor(InformationCategory.Needs, pawn)
				&& !(thought?.requiredGenes?.Count > 0 && !IsInformationKnownFor(InformationCategory.Personal, pawn))
				&& !(thought?.requiredHediffs?.Count > 0 && !IsInformationKnownFor(InformationCategory.Health, pawn))
				&& !(thought?.requiredTraits?.Any((traitDef) => !IsTraitKnown(pawn, traitDef)) ?? false);
#else
		public static bool IsThoughtKnown(Pawn? pawn, ThoughtDef? thought) =>
			IsInformationKnownFor(InformationCategory.Needs, pawn)
				&& !(thought?.requiredGenes?.Count > 0 && !IsInformationKnownFor(InformationCategory.Personal, pawn))
				&& !(thought?.requiredHediffs?.Count > 0 && !IsInformationKnownFor(InformationCategory.Health, pawn))
				&& !(thought?.requiredTraits?.Any((traitDef) => !IsTraitKnown(pawn, traitDef)) ?? false);
#endif

		/// <summary>
		/// Determine whether the given trait is known.
		/// </summary>
		/// <param name="trait">The trait.</param>
		/// <returns>Whether the given trait is known.</returns>
#if V1_0 || V1_1
		public static bool IsTraitKnown(Trait trait) =>
			trait is null
			|| IsTraitKnown(null, trait.def);
#elif V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static bool IsTraitKnown(Trait trait) =>
			trait is null
			|| IsTraitKnown(trait.pawn, trait.def);
#else
		public static bool IsTraitKnown(Trait? trait) =>
			trait is null
			|| IsTraitKnown(trait.pawn, trait.def);
#endif

		/// <summary>
		/// Determine whether the given trait type is known for the given pawn.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <param name="trait">The trait type.</param>
		/// <returns>Whether the given trait type is known for the given pawn.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static bool IsTraitKnown(Pawn pawn, TraitDef trait) {
#else
		public static bool IsTraitKnown(Pawn? pawn, TraitDef? trait) {
#endif
			// In vanilla RimWorld, `pawn is null` only during a growth moment.
			if (pawn is null || MiscellaneousUtility.IsInGrowthMoment()) {
				return IsInformationKnownFor(InformationCategory.Traits, PawnType.Colonist) && !IsLearningEnabledFor(InformationCategory.Traits, pawn);
			}

			if (!IsInformationKnownFor(InformationCategory.Traits, pawn)) {
				return false;
			}

			if (trait is null || !IsLearningEnabledFor(InformationCategory.Traits, pawn)) {
				return true;
			}

			// One trait per rarity (multiplicative inverse of commonality) per quadrum per difficulty.
			bool defaultUnlocked = MiscellaneousUtility.TimeAsColonistOrPrisoner(pawn) > 1 / trait.GetGenderSpecificCommonality(pawn.gender) * _ticksPerQuadrum * (BoundedRationalityMod.Settings?.TraitsLearningDifficulty ?? 0);
			return (!(BoundedRationalityMod.Settings?.EnableUniqueTraitUnlockConditions ?? false) || BoundedRationalityMod.Settings.LegacyMode) ? defaultUnlocked
				: trait == TraitDefOf.Bloodlust ? pawn.records.GetValue(RecordDefOf.Kills) >= BoundedRationalityMod.Settings.TraitsLearningDifficulty
				: trait == TraitDefOf.Pyromaniac ? pawn.records.GetValue(RecordDefOf.TimesInMentalState) >= BoundedRationalityMod.Settings.TraitsLearningDifficulty
				: trait == TraitDefOf.Brawler || trait.defName == "ShootingAccuracy" ? pawn.records.GetValue(RecordDefOf.ShotsFired) >= BoundedRationalityMod.Settings.TraitsLearningDifficulty * 10
				: trait == _traitDefOfWimp || trait.defName == "Tough" || trait.defName == "Masochist" ? pawn.records.GetValue(RecordDefOf.DamageTaken) >= BoundedRationalityMod.Settings.TraitsLearningDifficulty * (_humanMaxHealth / 10)
				: trait == TraitDefOf.BodyPurist || trait == TraitDefOf.Transhumanist ? pawn.records.GetValue(RecordDefOf.OperationsReceived) >= BoundedRationalityMod.Settings.TraitsLearningDifficulty
				: trait.defName == "Gourmand" ? pawn.records.GetValue(RecordDefOf.NutritionEaten) >= BoundedRationalityMod.Settings.TraitsLearningDifficulty * _humanDailyNutrition * 10
				: defaultUnlocked;
		}
	}
}
