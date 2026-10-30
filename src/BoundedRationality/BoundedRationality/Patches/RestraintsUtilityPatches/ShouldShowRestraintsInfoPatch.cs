#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.RestraintsUtilityPatches {
	[HarmonyPatch(typeof(RestraintsUtility), nameof(RestraintsUtility.ShouldShowRestraintsInfo))]
	public static class ShouldShowRestraintsInfoPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707
		private static void Postfix(Pawn pawn, ref bool __result) =>
#pragma warning restore CA1707
			__result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn);
	}
}
