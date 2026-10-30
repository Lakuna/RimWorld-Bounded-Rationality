#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.InspectPaneFillerPatches {
	[HarmonyPatch(typeof(InspectPaneFiller), nameof(InspectPaneFiller.DrawHealth))]
	public static class DrawHealthPatch {
		[HarmonyPrefix]
		private static bool Prefix(Thing t) => !(t is Pawn pawn) || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, pawn);
	}
}
