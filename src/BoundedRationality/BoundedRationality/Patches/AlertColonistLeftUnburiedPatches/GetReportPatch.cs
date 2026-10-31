#if !V1_0
using System.Linq;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

namespace Lakuna.BoundedRationality.Patches.AlertColonistLeftUnburiedPatches {
	[HarmonyPatch(typeof(Alert_ColonistLeftUnburied), nameof(Alert_ColonistLeftUnburied.GetReport))]
	public static class GetReportPatch {
		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(ref AlertReport __result) {
#pragma warning restore IDE1006
			__result.culpritsThings = __result.culpritsThings?.Where((thing) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, thing, ControlCategory.Alert)).ToList();
			__result.active = __result.AnyCulpritValid;
		}
	}
}
#endif
