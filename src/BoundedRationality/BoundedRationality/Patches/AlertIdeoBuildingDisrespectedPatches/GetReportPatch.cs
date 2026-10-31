#if !(V1_0 || V1_1 || V1_2)
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

namespace Lakuna.BoundedRationality.Patches.AlertIdeoBuildingDisrespectedPatches {
	[HarmonyPatch(typeof(Alert_IdeoBuildingDisrespected), nameof(Alert_IdeoBuildingDisrespected.GetReport))]
	public static class GetReportPatch {
		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(ref AlertReport __result) =>
#pragma warning restore IDE1006
			__result = __result.active && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, PawnType.Colonist, ControlCategory.Alert);
	}
}
#endif
