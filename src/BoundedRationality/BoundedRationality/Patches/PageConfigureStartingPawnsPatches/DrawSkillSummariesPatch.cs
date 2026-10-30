#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

namespace Lakuna.BoundedRationality.Patches.PageConfigureStartingPawnsPatches {
	[HarmonyPatch(typeof(Page_ConfigureStartingPawns), "DrawSkillSummaries")]
	public static class DrawSkillSummariesPatch {
		[HarmonyPrefix]
		public static bool Prefix() => BoundedRationalityMod.Settings.AlwaysKnowStartingColonists || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, PawnType.Colonist);
	}
}
#endif
