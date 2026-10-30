#if !(V1_0 || V1_1)
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

namespace Lakuna.BoundedRationality.Patches.FactionPatches {
	[HarmonyPatch(typeof(Faction), nameof(Faction.Hidden), MethodType.Getter)]
	public static class HiddenPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707
		public static void Postfix(Faction __instance, ref bool __result) =>
#pragma warning restore CA1707
			__result = __result
			|| !KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, __instance);
	}
}
#endif
