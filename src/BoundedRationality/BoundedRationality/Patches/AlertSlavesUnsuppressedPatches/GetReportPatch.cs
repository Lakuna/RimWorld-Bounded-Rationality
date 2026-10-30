#if !(V1_0 || V1_1 || V1_2)
using System.Linq;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

namespace Lakuna.BoundedRationality.Patches.AlertSlavesUnsuppressedPatches {
	[HarmonyPatch(typeof(Alert_SlavesUnsuppressed), nameof(Alert_SlavesUnsuppressed.GetReport))]
	public static class GetReportPatch {
		[HarmonyPostfix]
		public static void Postfix(ref AlertReport __result) {
			__result.culpritsPawns = __result.culpritsPawns?.Where((pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, pawn, ControlCategory.Alert)).ToList();
			__result.active = __result.AnyCulpritValid;
		}
	}
}
#endif
