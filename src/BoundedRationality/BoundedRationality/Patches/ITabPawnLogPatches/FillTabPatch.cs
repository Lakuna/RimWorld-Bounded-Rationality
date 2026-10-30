using System.Reflection;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.ITabPawnLogPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Log), "FillTab")]
	public static class FillTabPatch {
		private static readonly MethodInfo SelPawnForCombatInfoMethod = PatchUtility.PropertyGetter(typeof(ITab_Pawn_Log), "SelPawnForCombatInfo");

		[HarmonyPrefix]
#pragma warning disable CA1707
		private static void Prefix(ITab_Pawn_Log __instance, ref bool ___showCombat, ref bool ___showSocial) {
#pragma warning restore CA1707
			if (!(SelPawnForCombatInfoMethod.Invoke(__instance, MiscellaneousUtility.EmptyArray()) is Pawn pawn)) {
				return;
			}

			___showCombat = ___showCombat && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, pawn);
			___showSocial = ___showSocial && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, pawn);
		}
	}
}
