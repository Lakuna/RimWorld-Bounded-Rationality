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
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(Pawn pawn, ref bool __result) =>
#pragma warning restore IDE1006
			__result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn);
	}
}
