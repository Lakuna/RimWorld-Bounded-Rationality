#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.PawnUtilityPatches {
	[HarmonyPatch(typeof(PawnUtility), nameof(PawnUtility.ShouldDisplayLordReport))]
	public static class ShouldDisplayLordReportPatch {
		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(Pawn pawn, ref bool __result) =>
#pragma warning restore IDE1006
			__result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn);
	}
}
#endif
