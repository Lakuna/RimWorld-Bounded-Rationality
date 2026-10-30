#if !(V1_0 || V1_1 || V1_2)
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

namespace Lakuna.BoundedRationality.Patches.AlertSlaveRebellionLikelyPatches {
	[HarmonyPatch(typeof(Alert_SlaveRebellionLikely), nameof(Alert_SlaveRebellionLikely.GetReport))]
	public static class GetReportPatch {
		[HarmonyPostfix]
		public static void Postfix(ref AlertReport __result) {
			if (
#if V1_3 || V1_4
				KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, __result.culpritTarget?.Thing, ControlCategory.Alert)
#else
				KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, __result.culpritTarget?.Pawn, ControlCategory.Alert)
#endif
			) {
				return;
			}

			__result.active = false;
			__result.culpritTarget = default;
		}
	}
}
#endif
