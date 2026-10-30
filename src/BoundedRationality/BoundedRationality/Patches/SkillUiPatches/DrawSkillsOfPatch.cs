#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.SkillUiPatches {
	[HarmonyPatch(typeof(SkillUI), nameof(SkillUI.DrawSkillsOf))]
	public static class DrawSkillsOfPatch {
		[HarmonyPrefix]
		public static bool Prefix(Pawn p) =>
			KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, p);
	}
}
