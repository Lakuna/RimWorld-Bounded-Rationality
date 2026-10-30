#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.PawnUtilityPatches {
	[HarmonyPatch(typeof(PawnUtility), nameof(PawnUtility.ShouldSendNotificationAbout))]
	public static class ShouldSendNotificationAboutPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707
		private static void Postfix(Pawn p, ref bool __result) =>
#pragma warning restore CA1707
			__result = __result && (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, p) || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, p));
	}
}
