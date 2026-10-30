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
#pragma warning disable CA1707
		private static void Postfix(ref string __result, Pawn ___pawn) {
#pragma warning restore CA1707
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, ___pawn)) {
				return;
			}

			__result = MiscellaneousUtility.EndWithPeriod("BR.Unknown".Translate().CapitalizeFirst());
		}
	}
}
