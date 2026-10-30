using System;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

namespace Lakuna.BoundedRationality.Patches.ThoughtPatches {
	[HarmonyPatch(typeof(Thought), nameof(Thought.VisibleInNeedsTab), MethodType.Getter)]
	public static class VisibleInNeedsTabPatch {
		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(Thought __instance, ref bool __result) {
#pragma warning restore IDE1006
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (__instance is null) {
				throw new ArgumentNullException(nameof(__instance));
			}
#else
			ArgumentNullException.ThrowIfNull(__instance);
#endif

			bool personal = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Personal, __instance.pawn);
			bool health = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, __instance.pawn);
			bool social = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, __instance.pawn);
#if !(V1_0 || V1_1 || V1_2)
			bool ideoligion = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, __instance.pawn);
			bool gear = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Gear, __instance.pawn);
#endif

#if V1_0 || V1_1 || V1_2
			__result = __result
				&& (__instance.def.requiredTraits?.TrueForAll((traitDef) => KnowledgeUtility.IsTraitKnown(__instance.pawn, traitDef)) ?? true)
				&& (__instance.def.workerClass != typeof(ThoughtWorker_Pain) && __instance.def.workerClass != typeof(ThoughtWorker_Sick) || health);
#elif V1_3
			__result = __result
				&& (!(__instance.def.workerClass?.IsSubclassOf(typeof(ThoughtWorker_Precept)) ?? false) || ideoligion)
				&& (!(__instance.def.workerClass?.IsSubclassOf(typeof(ThoughtWorker_Precept_Social)) ?? false) || ideoligion && social)
				&& (__instance.def.requiredTraits?.TrueForAll((traitDef) => KnowledgeUtility.IsTraitKnown(__instance.pawn, traitDef)) ?? true)
				&& (__instance.def.workerClass != typeof(ThoughtWorker_Pain) && __instance.def.workerClass != typeof(ThoughtWorker_Sick) || health);
#elif V1_4
			__result = __result
				&& (!(__instance.def.workerClass?.IsSubclassOf(typeof(ThoughtWorker_Precept)) ?? false) || ideoligion)
				&& (!(__instance.def.workerClass?.IsSubclassOf(typeof(ThoughtWorker_Precept_Social)) ?? false) || ideoligion && social)
				&& ((__instance.def.requiredGenes?.Count ?? 0) == 0 || personal)
				&& (__instance.def.requiredTraits?.TrueForAll((traitDef) => KnowledgeUtility.IsTraitKnown(__instance.pawn, traitDef)) ?? true)
				&& (__instance.def.workerClass != typeof(ThoughtWorker_Pain) && __instance.def.workerClass != typeof(ThoughtWorker_Sick) || health);
#else
			__result = __result
				&& (!(__instance.def.workerClass?.IsSubclassOf(typeof(ThoughtWorker_Precept)) ?? false) || ideoligion)
				&& (!(__instance.def.workerClass?.IsSubclassOf(typeof(ThoughtWorker_Precept_Social)) ?? false) || ideoligion && social)
				&& ((__instance.def.requiredGenes?.Count ?? 0) == 0 || personal)
				&& ((__instance.def.requiredHediffs?.Count ?? 0) == 0 || health)
				&& (__instance.def.requiredTraits?.TrueForAll((traitDef) => KnowledgeUtility.IsTraitKnown(__instance.pawn, traitDef)) ?? true)
				&& (__instance.def.workerClass != typeof(ThoughtWorker_Pain) && __instance.def.workerClass != typeof(ThoughtWorker_Sick) || health);
#endif

#if !V1_0
			if (__instance is Thought_DecreeUnmet) {
				__result = __result && personal;
				return;
			}
#endif

#if !(V1_0 || V1_1 || V1_2)
			if (__instance is Thought_Situational_Precept_SlavesInColony
				|| __instance is Thought_Situational_Precept_HighLife
				|| __instance is Thought_IdeoMissingBuilding
				|| __instance is Thought_IdeoDisrespectedBuilding
				|| __instance is Thought_IdeoRoleEmpty) {
				__result = __result && ideoligion;
				return;
			}

			if (__instance is Thought_Situational_WearingDesiredApparel
				|| __instance is Thought_IdeoRoleApparelRequirementNotMet) {
				__result = __result && ideoligion && gear;
				return;
			}

			if (__instance is Thought_IdeoLeaderResentment) {
				__result = __result && ideoligion && social;
				return;
			}
#endif

#if !(V1_0 || V1_1 || V1_2 || V1_3)
			if (__instance is Thought_Situational_KillThirst
				|| __instance is Thought_Situational_GeneticChemicalDependency) {
				__result = __result && personal;
				return;
			}
#endif

			if (__instance is Thought_OpinionOfMyLover
				|| __instance is Thought_BondedAnimalMaster
				|| __instance is Thought_NotBondedAnimalMaster
				|| __instance is Thought_SharedBed) {
				__result = __result && social;
			}
		}
	}
}
