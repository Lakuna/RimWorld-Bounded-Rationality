#if !(V1_0 || V1_1 || V1_2 || V1_3)
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.GizmoGrowthTierPatches {
	[HarmonyPatch(typeof(Gizmo_GrowthTier), nameof(Gizmo_GrowthTier.Visible), MethodType.Getter)]
	public static class VisiblePatch {
		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(ref bool __result, Pawn ___child) =>
#pragma warning restore IDE1006
			__result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, ___child, ControlCategory.Control);
	}
}
#endif
