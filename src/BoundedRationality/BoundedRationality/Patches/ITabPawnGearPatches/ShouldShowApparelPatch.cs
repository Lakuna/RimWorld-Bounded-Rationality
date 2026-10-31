#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.ITabPawnGearPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Gear), "ShouldShowApparel")]
	public static class ShouldShowApparelPatch {
		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(Pawn p, ref bool __result) =>
#pragma warning restore IDE1006
			__result = __result
				&& KnowledgeUtility.IsInformationKnownFor(InformationCategory.Gear, p, ControlCategory.Control);
	}
}
