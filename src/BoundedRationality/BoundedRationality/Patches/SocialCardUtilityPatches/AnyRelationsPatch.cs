#if !(V1_0 || V1_1 || V1_2 || V1_3)
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.SocialCardUtilityPatches {
	[HarmonyPatch(typeof(SocialCardUtility), nameof(SocialCardUtility.AnyRelations))]
	public static class AnyRelationsPatch {
		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(Pawn selPawnForSocialInfo, ref bool __result) =>
#pragma warning restore IDE1006
			__result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, selPawnForSocialInfo);
	}
}
#endif
