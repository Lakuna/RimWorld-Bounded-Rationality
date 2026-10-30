#if !(V1_0 || V1_1 || V1_2)
using System.Linq;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

namespace Lakuna.BoundedRationality.Patches.AlertHitchedAnimalHungryNoFoodPatches {
	[HarmonyPatch(typeof(Alert_HitchedAnimalHungryNoFood), nameof(Alert_HitchedAnimalHungryNoFood.GetReport))]
	public static class GetReportPatch {
		[HarmonyPostfix]
		private static void Postfix(ref AlertReport __result) {
			__result.culpritsPawns = __result.culpritsPawns?.Where((pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, pawn, ControlCategory.Alert)).ToList();
			__result.active = __result.AnyCulpritValid;
		}
	}
}
#endif
