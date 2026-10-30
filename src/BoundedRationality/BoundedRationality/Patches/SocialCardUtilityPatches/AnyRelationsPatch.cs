#if !(V1_0 || V1_1 || V1_2 || V1_3)
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.SocialCardUtilityPatches {
	[HarmonyPatch(typeof(SocialCardUtility), nameof(SocialCardUtility.AnyRelations))]
	public static class AnyRelationsPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707
		private static void Postfix(Pawn selPawnForSocialInfo, ref bool __result) =>
#pragma warning restore CA1707
			__result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, selPawnForSocialInfo);
	}
}
#endif
