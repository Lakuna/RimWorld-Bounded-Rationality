using System;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using Verse;
using Verse.AI;

namespace Lakuna.BoundedRationality.Patches.MentalStatePatches {
	[HarmonyPatch(typeof(MentalState), nameof(MentalState.InspectLine), MethodType.Getter)]
	public static class InspectLinePatch {
		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(MentalState __instance, ref string __result) {
#pragma warning restore IDE1006
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (__instance is null) {
				throw new ArgumentNullException(nameof(__instance));
			}
#else
			ArgumentNullException.ThrowIfNull(__instance);
#endif

			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, __instance.pawn)) {
				return;
			}

			__result = "BR.Unknown".Translate();
		}
	}
}
