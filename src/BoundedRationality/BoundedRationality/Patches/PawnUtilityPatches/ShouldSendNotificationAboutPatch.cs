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
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(Pawn p, ref bool __result) =>
#pragma warning restore IDE1006
			__result = __result && (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, p) || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, p));
	}
}
