using System.Reflection;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.ITabPawnTrainingPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Training), nameof(ITab_Pawn_Training.IsVisible), MethodType.Getter)]
	public static class IsVisiblePatch {
		private static readonly MethodInfo SelPawnMethod = PatchUtility.PropertyGetter(typeof(ITab), "SelPawn");

		[HarmonyPostfix]
#pragma warning disable CA1707
		private static void Postfix(ITab_Pawn_Training __instance, ref bool __result) =>
#pragma warning restore CA1707
			__result = __result
			&& (!(SelPawnMethod.Invoke(__instance, MiscellaneousUtility.EmptyArray()) is Pawn pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn, ControlCategory.Control));
	}
}
