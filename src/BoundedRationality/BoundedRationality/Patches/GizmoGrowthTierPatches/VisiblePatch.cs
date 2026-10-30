#if !(V1_0 || V1_1 || V1_2 || V1_3)
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.GizmoGrowthTierPatches {
	[HarmonyPatch(typeof(Gizmo_GrowthTier), nameof(Gizmo_GrowthTier.Visible), MethodType.Getter)]
	public static class VisiblePatch {
		[HarmonyPostfix]
#pragma warning disable CA1707
		private static void Postfix(ref bool __result, Pawn ___child) =>
#pragma warning restore CA1707
			__result = __result
			&& KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, ___child, ControlCategory.Control);
	}
}
#endif
