#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.InteractionCardUtilityPatches {
	[HarmonyPatch(typeof(InteractionCardUtility), nameof(InteractionCardUtility.DrawInteractionsLog))]
	public static class DrawInteractionsLogPatch {
		[HarmonyPrefix]
		public static bool Prefix(Pawn pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, pawn);
	}
}
