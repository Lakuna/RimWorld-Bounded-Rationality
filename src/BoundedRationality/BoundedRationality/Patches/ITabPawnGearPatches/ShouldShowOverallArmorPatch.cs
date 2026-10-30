#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.ITabPawnGearPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Gear), "ShouldShowOverallArmor")]
	public static class ShouldShowOverallArmorPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707
		public static void Postfix(Pawn p, ref bool __result) =>
#pragma warning restore CA1707
			__result = __result
			&& KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, p);
	}
}
