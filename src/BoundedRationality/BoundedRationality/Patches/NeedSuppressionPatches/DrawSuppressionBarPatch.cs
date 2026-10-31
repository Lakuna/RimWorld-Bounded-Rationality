#if !(V1_0 || V1_1 || V1_2)
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.NeedSuppressionPatches {
	[HarmonyPatch(typeof(Need_Suppression), nameof(Need_Suppression.DrawSuppressionBar))]
	public static class DrawSuppressionBarPatch {
		[HarmonyPrefix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static bool Prefix(Pawn ___pawn) =>
#pragma warning restore IDE1006
			KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, ___pawn);
	}
}
#endif
