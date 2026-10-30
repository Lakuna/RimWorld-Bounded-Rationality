#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using Verse;

namespace Lakuna.BoundedRationality.Patches.StartingPawnUtilityPatches {
	[HarmonyPatch(typeof(StartingPawnUtility), nameof(StartingPawnUtility.DrawSkillSummaries))]
	public static class DrawSkillSummariesPatch {
		[HarmonyPrefix]
		private static bool Prefix() => BoundedRationalityMod.Settings.AlwaysKnowStartingColonists || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, PawnType.Colonist);
	}
}
#endif
