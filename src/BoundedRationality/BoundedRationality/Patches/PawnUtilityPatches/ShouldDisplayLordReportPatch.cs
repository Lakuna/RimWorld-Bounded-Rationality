#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.PawnUtilityPatches {
	[HarmonyPatch(typeof(PawnUtility), nameof(PawnUtility.ShouldDisplayLordReport))]
	public static class ShouldDisplayLordReportPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707
		private static void Postfix(Pawn pawn, ref bool __result) =>
#pragma warning restore CA1707
			__result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn);
	}
}
#endif
