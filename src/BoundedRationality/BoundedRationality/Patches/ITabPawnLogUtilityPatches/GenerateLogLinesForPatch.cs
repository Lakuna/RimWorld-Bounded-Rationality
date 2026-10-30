#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.ITabPawnLogUtilityPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Log_Utility), nameof(ITab_Pawn_Log_Utility.GenerateLogLinesFor))]
	public static class GenerateLogLinesForPatch {
		[HarmonyPrefix]
		public static void Prefix(Pawn pawn, ref bool showCombat, ref bool showSocial) {
			showCombat = showCombat && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, pawn);
			showSocial = showSocial && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, pawn);
		}
	}
}
