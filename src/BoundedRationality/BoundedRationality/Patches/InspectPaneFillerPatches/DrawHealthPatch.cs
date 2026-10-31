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
		public static bool Prefix(Thing t) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			!(t is Pawn pawn)
#else
			t is not Pawn pawn
#endif
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, pawn);
	}
}
