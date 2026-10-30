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
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(Pawn selPawnForSocialInfo, ref float __result) {
#pragma warning restore IDE1006
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, selPawnForSocialInfo)) {
				return;
			}

			__result = 0;
		}
	}
}
