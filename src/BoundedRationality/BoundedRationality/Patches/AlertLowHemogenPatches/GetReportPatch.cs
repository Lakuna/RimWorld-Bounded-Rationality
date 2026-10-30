#if !(V1_0 || V1_1 || V1_2 || V1_3)
using System.Linq;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

namespace Lakuna.BoundedRationality.Patches.AlertLowHemogenPatches {
	[HarmonyPatch(typeof(Alert_LowHemogen), nameof(Alert_LowHemogen.GetReport))]
	public static class GetReportPatch {
		[HarmonyPostfix]
		public static void Postfix(ref AlertReport __result) {
			__result.culpritsPawns = __result.culpritsPawns?.Where((pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, pawn, ControlCategory.Alert)).ToList();
			__result.active = __result.AnyCulpritValid;
		}
	}
}
#endif
