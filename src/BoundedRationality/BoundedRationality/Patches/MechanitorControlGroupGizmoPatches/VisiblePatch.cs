#if !(V1_0 || V1_1 || V1_2 || V1_3)
using System.Reflection;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

namespace Lakuna.BoundedRationality.Patches.MechanitorControlGroupGizmoPatches {
	[HarmonyPatch(typeof(MechanitorControlGroupGizmo), nameof(MechanitorControlGroupGizmo.Visible), MethodType.Getter)]
	public static class VisiblePatch {
		private static readonly FieldInfo TrackerField = AccessTools.Field(typeof(MechanitorControlGroup), "tracker");

		[HarmonyPostfix]
#pragma warning disable CA1707
		public static void Postfix(ref bool __result, MechanitorControlGroup ___controlGroup) =>
#pragma warning restore CA1707
			__result = __result
			&& (!(TrackerField.GetValue(___controlGroup) is Pawn_MechanitorTracker pawnMechanitorTracker)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, pawnMechanitorTracker.Pawn, ControlCategory.Control));
	}
}
#endif
