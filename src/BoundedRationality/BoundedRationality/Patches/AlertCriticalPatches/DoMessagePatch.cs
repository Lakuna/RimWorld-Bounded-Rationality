#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
using System.Collections.Generic;
using System.Linq;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.AlertCriticalPatches {
	[HarmonyPatch(typeof(Alert_Critical), "DoMessage", MethodType.Getter)]
	public static class DoMessagePatch {
		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(Alert_Critical __instance, ref bool __result) {
#pragma warning restore IDE1006
			// Never enable an already disabled message.
			if (!__result) {
				return;
			}

			List<Pawn>? pawns = __instance?.GetReport().culpritsPawns;
			if (pawns is null || pawns.NullOrEmpty()) {
				return;
			}

			InformationCategory category =
				(__instance is Alert_MajorOrExtremeBreakRisk
				|| __instance is Alert_MeatHunger)
					? InformationCategory.Needs
					: (__instance is Alert_Hypothermia
					|| __instance is Alert_ColonistNeedsRescuing
					|| __instance is Alert_LifeThreateningHediff
					|| __instance is Alert_ImmobileCaravan
					|| __instance is Alert_GhoulHypothermia
#if !V1_5
					|| __instance is Alert_LowOxygen
#endif
					)
						? InformationCategory.Health
						: InformationCategory.Basic;
			if (pawns.All((pawn) => !KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, pawn))) {
				__result = false;
			}
		}
	}
}
#endif
