#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using Verse;

namespace Lakuna.BoundedRationality.Patches.HealthUtilityPatches {
	[HarmonyPatch(typeof(HealthUtility), nameof(HealthUtility.GetGeneralConditionLabel))]
	public static class GetGeneralConditionLabelPatch {
		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(Pawn pawn, ref string __result) {
#pragma warning restore IDE1006
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, pawn)) {
				return;
			}

			__result = "BR.Unknown".Translate().CapitalizeFirst();
		}
	}
}
