#if !(V1_0 || V1_1 || V1_2)
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.SocialCardUtilityPatches {
	[HarmonyPatch(typeof(SocialCardUtility), nameof(SocialCardUtility.DrawPawnRoleSelection))]
	public static class DrawPawnRoleSelectionPatch {
		[HarmonyPrefix]
		public static bool Prefix(Pawn pawn) =>
			KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, pawn, ControlCategory.Control);
	}
}
#endif
