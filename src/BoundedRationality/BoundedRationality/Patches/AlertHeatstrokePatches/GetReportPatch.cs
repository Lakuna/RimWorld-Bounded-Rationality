#if !V1_0
using System.Linq;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

namespace Lakuna.BoundedRationality.Patches.AlertHeatstrokePatches {
	[HarmonyPatch(typeof(Alert_Heatstroke), nameof(Alert_Heatstroke.GetReport))]
	public static class GetReportPatch {
		[HarmonyPostfix]
		public static void Postfix(ref AlertReport __result) {
			__result.culpritsPawns = __result.culpritsPawns?.Where((pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, pawn, ControlCategory.Alert)).ToList();
			__result.active = __result.AnyCulpritValid;
		}
	}
}
#endif
