#if !(V1_0 || V1_1 || V1_2)
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.SlaveRebellionUtilityPatches {
	[HarmonyPatch(typeof(SlaveRebellionUtility), nameof(SlaveRebellionUtility.GetAnySlaveRebellionExplanation))]
	public static class GetAnySlaveRebellionExplanationPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707
		private static void Postfix(Pawn pawn, ref string __result) {
#pragma warning restore CA1707
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, pawn)) {
				return;
			}

			__result = string.Empty;
		}
	}
}
#endif
