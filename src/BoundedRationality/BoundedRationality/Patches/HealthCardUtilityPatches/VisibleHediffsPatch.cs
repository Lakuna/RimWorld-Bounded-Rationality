using System.Collections.Generic;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.HealthCardUtilityPatches {
	[HarmonyPatch(typeof(HealthCardUtility), "VisibleHediffs")]
	public static class VisibleHediffsPatch {
		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(Pawn pawn, ref IEnumerable<Hediff> __result) {
#pragma warning restore IDE1006
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, pawn)) {
				return;
			}

			__result = MiscellaneousUtility.EmptyArray<Hediff>();
		}
	}
}
