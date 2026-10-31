using System.Linq;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

namespace Lakuna.BoundedRationality.Patches.AlertMajorOrExtremeBreakRiskPatches {
	[HarmonyPatch(typeof(Alert_MajorOrExtremeBreakRisk), nameof(Alert_MajorOrExtremeBreakRisk.GetReport))]
	public static class GetReportPatch {
		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(ref AlertReport __result) {
#pragma warning restore IDE1006
#if V1_0
			__result.culprits = __result.culprits?.Where((target) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, target.Thing, ControlCategory.Alert)).ToList();
#else
			__result.culpritsPawns = __result.culpritsPawns?.Where((pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, pawn, ControlCategory.Alert)).ToList();
#endif
			__result.active = __result.AnyCulpritValid;
		}
	}
}
