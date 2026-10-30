using System.Reflection;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.ITabPawnGearPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Gear), "TryDrawMassInfo")]
	public static class TryDrawMassInfoPatch {
		private static readonly MethodInfo SelPawnForGearMethod = PatchUtility.PropertyGetter(typeof(ITab_Pawn_Gear), "SelPawnForGear");

		[HarmonyPrefix]
#pragma warning disable CA1707
		public static bool Prefix(ITab_Pawn_Gear __instance) =>
#pragma warning restore CA1707
			!(SelPawnForGearMethod.Invoke(__instance, MiscellaneousUtility.EmptyArray()) is Pawn pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, pawn);
	}
}
