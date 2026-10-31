#if !V1_0
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.NeedChemicalAnyPatches {
	[HarmonyPatch(typeof(Need_Chemical_Any), nameof(Need_Chemical_Any.ShowOnNeedList), MethodType.Getter)]
	public static class ShowOnNeedListPatch {
		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(ref bool __result, Pawn ___pawn) =>
#pragma warning restore IDE1006
			__result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, ___pawn);
	}
}
#endif
