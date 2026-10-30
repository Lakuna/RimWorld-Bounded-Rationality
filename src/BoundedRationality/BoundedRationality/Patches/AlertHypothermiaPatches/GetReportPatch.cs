using System.Linq;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

namespace Lakuna.BoundedRationality.Patches.AlertHypothermiaPatches {
	[HarmonyPatch(typeof(Alert_Hypothermia), nameof(Alert_Hypothermia.GetReport))]
	public static class GetReportPatch {
		[HarmonyPostfix]
		public static void Postfix(ref AlertReport __result) {
#if V1_0
			__result.culprits = __result.culprits?.Where((target) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, target.Thing, ControlCategory.Alert)).ToList();
#else
			__result.culpritsPawns = __result.culpritsPawns?.Where((pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, pawn, ControlCategory.Alert)).ToList();
#endif
			__result.active = __result.AnyCulpritValid;
		}
	}
}
