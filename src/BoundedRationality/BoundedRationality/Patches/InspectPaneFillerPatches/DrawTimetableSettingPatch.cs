#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.InspectPaneFillerPatches {
	[HarmonyPatch(typeof(InspectPaneFiller), "DrawTimetableSetting")]
	public static class DrawTimetableSettingPatch {
		[HarmonyPrefix]
		public static bool Prefix(Pawn pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn);
	}
}
