#if !(V1_0 || V1_1 || V1_2 || V1_3)
using System.Reflection;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

namespace Lakuna.BoundedRationality.Patches.MechanitorControlGroupGizmoPatches {
	[HarmonyPatch(typeof(MechanitorControlGroupGizmo), nameof(MechanitorControlGroupGizmo.Visible), MethodType.Getter)]
	public static class VisiblePatch {
		private static readonly FieldInfo _trackerField = AccessTools.Field(typeof(MechanitorControlGroup), "tracker");

		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(ref bool __result, MechanitorControlGroup ___controlGroup) =>
#pragma warning restore IDE1006
			__result = __result
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				&& (!(_trackerField.GetValue(___controlGroup) is Pawn_MechanitorTracker pawnMechanitorTracker)
#else
				&& (_trackerField.GetValue(___controlGroup) is not Pawn_MechanitorTracker pawnMechanitorTracker
#endif
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, pawnMechanitorTracker.Pawn, ControlCategory.Control));
	}
}
#endif
