#if !(V1_0 || V1_1)
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using Verse;

namespace Lakuna.BoundedRationality.Patches.TooltipUtilityPatches {
	[HarmonyPatch(typeof(TooltipUtility), nameof(TooltipUtility.ShotCalculationTipString))]
	public static class ShotCalculationTipStringPatch {
		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(ref string __result) {
#pragma warning restore IDE1006
			if (Find.Selector.SingleSelectedThing is Pawn pawn && !KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, pawn)) {
				__result = string.Empty;
			}
		}
	}
}
#endif
