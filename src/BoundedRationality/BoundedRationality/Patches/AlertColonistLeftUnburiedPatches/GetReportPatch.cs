#if !V1_0
using System.Linq;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

namespace Lakuna.BoundedRationality.Patches.AlertColonistLeftUnburiedPatches {
	[HarmonyPatch(typeof(Alert_ColonistLeftUnburied), nameof(Alert_ColonistLeftUnburied.GetReport))]
	public static class GetReportPatch {
		[HarmonyPostfix]
		private static void Postfix(ref AlertReport __result) {
			__result.culpritsThings = __result.culpritsThings?.Where((thing) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, thing, ControlCategory.Alert)).ToList();
			__result.active = __result.AnyCulpritValid;
		}
	}
}
#endif
