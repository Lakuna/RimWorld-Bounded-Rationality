#if !(V1_0 || V1_1 || V1_2)
using System;
using System.Reflection;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.ITabPawnVisitorPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Visitor), "DoSlavePriceListing")]
	public static class DoSlavePriceListingPatch {
		private static readonly MethodInfo SelPawnMethod = PatchUtility.PropertyGetter(typeof(ITab), "SelPawn");

		[HarmonyPrefix]
#pragma warning disable CA1707
		public static bool Prefix(ITab_Pawn_Visitor __instance) =>
#pragma warning restore CA1707
			!(SelPawnMethod.Invoke(__instance, MiscellaneousUtility.EmptyArray()) is Pawn pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, pawn);
	}
}
#endif
