#if !(V1_0 || V1_1 || V1_2)
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.SlaveRebellionUtilityPatches {
	[HarmonyPatch(typeof(SlaveRebellionUtility), nameof(SlaveRebellionUtility.GetAnySlaveRebellionExplanation))]
	public static class GetAnySlaveRebellionExplanationPatch {
		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(Pawn pawn, ref string __result) {
#pragma warning restore IDE1006
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, pawn)) {
				return;
			}

			__result = string.Empty;
		}
	}
}
#endif
