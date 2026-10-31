#if !(V1_0 || V1_1 || V1_2)
using System.Linq;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

namespace Lakuna.BoundedRationality.Patches.AlertSlavesUnsuppressedPatches {
	[HarmonyPatch(typeof(Alert_SlavesUnsuppressed), nameof(Alert_SlavesUnsuppressed.GetReport))]
	public static class GetReportPatch {
		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(ref AlertReport __result) {
#pragma warning restore IDE1006
			__result.culpritsPawns = __result.culpritsPawns?.Where((pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, pawn, ControlCategory.Alert)).ToList();
			__result.active = __result.AnyCulpritValid;
		}
	}
}
#endif
