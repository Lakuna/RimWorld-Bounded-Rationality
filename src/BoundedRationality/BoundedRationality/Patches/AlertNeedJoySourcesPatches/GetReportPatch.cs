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
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(ref AlertReport __result) =>
#pragma warning restore IDE1006
			__result = __result.active && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, PawnType.Colonist, ControlCategory.Alert);
	}
}
