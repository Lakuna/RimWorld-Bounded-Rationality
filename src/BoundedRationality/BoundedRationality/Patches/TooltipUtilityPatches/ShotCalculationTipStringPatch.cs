#if !(V1_0 || V1_1)
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using Verse;

namespace Lakuna.BoundedRationality.Patches.TooltipUtilityPatches {
	[HarmonyPatch(typeof(TooltipUtility), nameof(TooltipUtility.ShotCalculationTipString))]
	public static class ShotCalculationTipStringPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707
		private static void Postfix(ref string __result) {
#pragma warning restore CA1707
			if (Find.Selector.SingleSelectedThing is Pawn pawn && !KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, pawn)) {
				__result = string.Empty;
			}
		}
	}
}
#endif
