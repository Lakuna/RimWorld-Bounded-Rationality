using System.Reflection;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.InspectTabBasePatches {
	[HarmonyPatch(typeof(InspectTabBase), nameof(InspectTabBase.IsVisible), MethodType.Getter)]
	public static class IsVisiblePatch {
		private static readonly MethodInfo PawnForHealthMethod = PatchUtility.PropertyGetter(typeof(ITab_Pawn_Health), "PawnForHealth");

		private static readonly MethodInfo SelPawnForCombatInfoMethod = PatchUtility.PropertyGetter(typeof(ITab_Pawn_Log), "SelPawnForCombatInfo");

		[HarmonyPostfix]
#pragma warning disable CA1707
		private static void Postfix(InspectTabBase __instance, ref bool __result) {
#pragma warning restore CA1707
			if (__instance is ITab_Pawn_Health healthTab) {
				__result = __result
					&& (!(PawnForHealthMethod.Invoke(healthTab, MiscellaneousUtility.EmptyArray()) is Pawn pawn)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, pawn, ControlCategory.Control));
				return;
			}

			if (__instance is ITab_Pawn_Log logTab) {
				__result = __result
					&& (!(SelPawnForCombatInfoMethod.Invoke(logTab, MiscellaneousUtility.EmptyArray()) is Pawn pawn)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, pawn) // Social log
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, pawn)); // Combat log
			}
		}
	}
}
