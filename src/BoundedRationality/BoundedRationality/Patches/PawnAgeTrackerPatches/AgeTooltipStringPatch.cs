#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using Verse;

namespace Lakuna.BoundedRationality.Patches.PawnAgeTrackerPatches {
	[HarmonyPatch(typeof(Pawn_AgeTracker), nameof(Pawn_AgeTracker.AgeTooltipString), MethodType.Getter)]
	public static class AgeTooltipStringPatch {
		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(ref string __result, Pawn ___pawn) {
#pragma warning restore IDE1006
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, ___pawn)) {
				return;
			}

			__result = "BR.Unknown".Translate().CapitalizeFirst().EndWithPeriod().StringSafe;
		}
	}
}
