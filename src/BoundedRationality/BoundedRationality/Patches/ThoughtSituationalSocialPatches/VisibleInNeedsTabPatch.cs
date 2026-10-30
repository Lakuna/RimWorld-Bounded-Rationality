using System;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

namespace Lakuna.BoundedRationality.Patches.ThoughtSituationalSocialPatches {
	[HarmonyPatch(typeof(Thought_SituationalSocial), nameof(Thought_SituationalSocial.VisibleInNeedsTab), MethodType.Getter)]
	public static class VisibleInNeedsTabPatch {
		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(Thought_SituationalSocial __instance, ref bool __result) =>
#pragma warning restore IDE1006
			__result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, __instance?.pawn ?? throw new ArgumentNullException(nameof(__instance)));
	}
}
