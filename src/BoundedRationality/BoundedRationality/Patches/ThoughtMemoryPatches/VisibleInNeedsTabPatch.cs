#if !V1_0
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

namespace Lakuna.BoundedRationality.Patches.ThoughtMemoryPatches {
	[HarmonyPatch(typeof(Thought_Memory), nameof(Thought_Memory.VisibleInNeedsTab), MethodType.Getter)]
	public static class VisibleInNeedsTabPatch {
		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(Thought_Memory __instance, ref bool __result) {
#pragma warning restore IDE1006
#if !(V1_1 || V1_2)
			if (__instance is Thought_RelicAtRitual
				|| __instance is Thought_TameVeneratedAnimalDied
				|| __instance is Thought_Counselled
				|| __instance is Thought_AttendedRitual
				|| __instance is Thought_IdeoRoleLost
				|| __instance is Thought_KilledInnocentAnimal) {
				__result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, __instance.pawn);
				return;
			}
#endif

#if !V1_1
			if (__instance is Thought_WeaponTrait) {
				__result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Gear, __instance.pawn);
				return;
			}
#endif

			if (__instance is Thought_MemoryRoyalTitle) {
				__result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Personal, __instance.pawn);
			}
		}
	}
}
#endif
