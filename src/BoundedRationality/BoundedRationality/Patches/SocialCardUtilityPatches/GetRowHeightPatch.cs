#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.SocialCardUtilityPatches {
	[HarmonyPatch(typeof(SocialCardUtility), "GetRowHeight")]
	public static class GetRowHeightPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707
		private static void Postfix(Pawn selPawnForSocialInfo, ref float __result) {
#pragma warning restore CA1707
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, selPawnForSocialInfo)) {
				return;
			}

			__result = 0;
		}
	}
}
