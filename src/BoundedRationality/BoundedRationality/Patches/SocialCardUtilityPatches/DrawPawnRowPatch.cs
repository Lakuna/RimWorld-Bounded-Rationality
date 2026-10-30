#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.SocialCardUtilityPatches {
	[HarmonyPatch(typeof(SocialCardUtility), "DrawPawnRow")]
	public static class DrawPawnRowPatch {
		[HarmonyPrefix]
		private static bool Prefix(Pawn selPawnForSocialInfo) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, selPawnForSocialInfo);
	}
}
