using System.Linq;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

#if V1_0
using Verse;
#endif

namespace Lakuna.BoundedRationality.Patches.AlertBrawlerHasRangedWeaponPatches {
	[HarmonyPatch(typeof(Alert_BrawlerHasRangedWeapon), nameof(Alert_BrawlerHasRangedWeapon.GetReport))]
	public static class GetReportPatch {
		[HarmonyPostfix]
		private static void Postfix(ref AlertReport __result) {
			if (BoundedRationalityMod.Settings.NeverHideAlerts) {
				return;
			}

#if V1_0
			__result.culprits = __result.culprits?.Where((target) =>
				target.Thing is Pawn pawn
				&& KnowledgeUtility.IsInformationKnownFor(InformationCategory.Gear, pawn, ControlCategory.Alert)
				&& KnowledgeUtility.IsTraitKnown(pawn.story.traits.GetTrait(TraitDefOf.Brawler))).ToList();
#else
			__result.culpritsPawns = __result.culpritsPawns?.Where((pawn) =>
				KnowledgeUtility.IsInformationKnownFor(InformationCategory.Gear, pawn, ControlCategory.Alert)
				&& KnowledgeUtility.IsTraitKnown(pawn.story.traits.GetTrait(TraitDefOf.Brawler))).ToList();
#endif
			__result.active = __result.AnyCulpritValid;
		}
	}
}
