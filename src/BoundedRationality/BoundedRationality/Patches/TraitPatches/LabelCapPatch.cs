#if !(V1_0 || V1_1)
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.TraitPatches {
	[HarmonyPatch(typeof(Trait), nameof(Trait.LabelCap), MethodType.Getter)]
	public static class LabelCapPatch {
		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(Trait __instance, ref string __result) {
#pragma warning restore IDE1006
			if (KnowledgeUtility.IsTraitKnown(__instance)) {
				return;
			}

			__result = "BR.Unknown".Translate().CapitalizeFirst();
		}
	}
}
#endif
