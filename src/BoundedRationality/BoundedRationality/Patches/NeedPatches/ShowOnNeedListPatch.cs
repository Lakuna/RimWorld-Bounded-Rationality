#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.NeedPatches {
	[HarmonyPatch(typeof(Need), nameof(Need.ShowOnNeedList), MethodType.Getter)]
	public static class ShowOnNeedListPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707
		private static void Postfix(Need __instance, ref bool __result, Pawn ___pawn) {
#pragma warning restore CA1707
			if (__instance is Need_Chemical) {
				__result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, ___pawn);
				return;
			}

#if !(V1_0 || V1_1 || V1_2 || V1_3)
			if (__instance is Need_Deathrest) {
				__result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Personal, ___pawn);
			}
#endif
		}
	}
}
