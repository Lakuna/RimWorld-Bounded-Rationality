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
		private static readonly MethodInfo _pawnForHealthMethod = PatchUtility.PropertyGetter(typeof(ITab_Pawn_Health), "PawnForHealth");

		private static readonly MethodInfo _selPawnForCombatInfoMethod = PatchUtility.PropertyGetter(typeof(ITab_Pawn_Log), "SelPawnForCombatInfo");

		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(InspectTabBase __instance, ref bool __result) {
#pragma warning restore IDE1006
			if (__instance is ITab_Pawn_Health healthTab) {
				__result = __result
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					&& (!(_pawnForHealthMethod.Invoke(healthTab, MiscellaneousUtility.EmptyArray()) is Pawn pawn)
#else
					&& (_pawnForHealthMethod.Invoke(healthTab, MiscellaneousUtility.EmptyArray()) is not Pawn pawn
#endif
						|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, pawn, ControlCategory.Control));
				return;
			}

			if (__instance is ITab_Pawn_Log logTab) {
				__result = __result
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
					&& (!(_selPawnForCombatInfoMethod.Invoke(logTab, MiscellaneousUtility.EmptyArray()) is Pawn pawn)
#else
					&& (_selPawnForCombatInfoMethod.Invoke(logTab, MiscellaneousUtility.EmptyArray()) is not Pawn pawn
#endif
						|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, pawn) // Social log
						|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, pawn)); // Combat log
			}
		}
	}
}
