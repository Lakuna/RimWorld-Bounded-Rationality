#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

namespace Lakuna.BoundedRationality.Patches.AlertNeedJoySourcesPatches {
	[HarmonyPatch(typeof(Alert_NeedJoySources), nameof(Alert_NeedJoySources.GetReport))]
	public static class GetReportPatch {
		[HarmonyPostfix]
		private static void Postfix(ref AlertReport __result) => __result = __result.active && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, PawnType.Colonist, ControlCategory.Alert);
	}
}
