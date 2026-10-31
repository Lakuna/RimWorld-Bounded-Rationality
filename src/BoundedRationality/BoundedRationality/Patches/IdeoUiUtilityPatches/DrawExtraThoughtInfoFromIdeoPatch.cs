#if !(V1_0 || V1_1 || V1_2)
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.IdeoUiUtilityPatches {
	[HarmonyPatch(typeof(IdeoUIUtility), nameof(IdeoUIUtility.DrawExtraThoughtInfoFromIdeo))]
	public static class DrawExtraThoughtInfoFromIdeoPatch {
		[HarmonyPrefix]
		public static bool Prefix(Pawn pawn) =>
			KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, pawn);
	}
}
#endif
