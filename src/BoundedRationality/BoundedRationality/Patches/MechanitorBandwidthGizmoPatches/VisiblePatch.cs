#if !(V1_0 || V1_1 || V1_2 || V1_3)
using System;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

namespace Lakuna.BoundedRationality.Patches.MechanitorBandwidthGizmoPatches {
	[HarmonyPatch(typeof(MechanitorBandwidthGizmo), nameof(MechanitorBandwidthGizmo.Visible), MethodType.Getter)]
	public static class VisiblePatch {
		[HarmonyPostfix]
#pragma warning disable CA1707
		private static void Postfix(ref bool __result, Pawn_MechanitorTracker ___tracker) =>
#pragma warning restore CA1707
			__result = __result
			&& KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, ___tracker?.Pawn ?? throw new ArgumentNullException(nameof(___tracker)), ControlCategory.Control);
	}
}
#endif
