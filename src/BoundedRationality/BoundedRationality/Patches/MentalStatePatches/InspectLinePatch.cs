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
#pragma warning disable CA1707
		public static void Postfix(MentalState __instance, ref string __result) {
#pragma warning restore CA1707
			if (__instance is null) {
				throw new ArgumentNullException(nameof(__instance));
			}

			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, __instance.pawn)) {
				return;
			}

			__result = "BR.Unknown".Translate();
		}
	}
}
