using System.Reflection;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.ITabPawnPrisonerPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Prisoner), nameof(ITab_Pawn_Prisoner.IsVisible), MethodType.Getter)]
	public static class IsVisiblePatch {
		private static readonly MethodInfo _selPawnMethod = PatchUtility.PropertyGetter(typeof(ITab), "SelPawn");

		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(ITab_Pawn_Prisoner __instance, ref bool __result) =>
#pragma warning restore IDE1006
			__result = __result
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				&& (!(_selPawnMethod.Invoke(__instance, MiscellaneousUtility.EmptyArray()) is Pawn pawn)
#else
				&& (_selPawnMethod.Invoke(__instance, MiscellaneousUtility.EmptyArray()) is not Pawn pawn
#endif
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn, ControlCategory.Control));
	}
}
