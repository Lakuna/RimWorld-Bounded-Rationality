#if !(V1_0 || V1_1 || V1_2)
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

namespace Lakuna.BoundedRationality.Patches.AlertIdeoBuildingMissingPatches {
	[HarmonyPatch(typeof(Alert_IdeoBuildingMissing), nameof(Alert_IdeoBuildingMissing.GetReport))]
	public static class GetReportPatch {
		[HarmonyPostfix]
		private static void Postfix(ref AlertReport __result) => __result = __result.active && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, PawnType.Colonist, ControlCategory.Alert);
	}
}
#endif
