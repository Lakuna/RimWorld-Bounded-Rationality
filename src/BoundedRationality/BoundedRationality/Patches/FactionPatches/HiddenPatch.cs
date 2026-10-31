#if !(V1_0 || V1_1)
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

namespace Lakuna.BoundedRationality.Patches.FactionPatches {
	[HarmonyPatch(typeof(Faction), nameof(Faction.Hidden), MethodType.Getter)]
	public static class HiddenPatch {
		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(Faction __instance, ref bool __result) =>
#pragma warning restore IDE1006
			__result = __result || !KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, __instance);
	}
}
#endif
