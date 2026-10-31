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
	[HarmonyPatch(typeof(ITab_Pawn_Gear), "TryDrawComfyTemperatureRange")]
	public static class TryDrawComfyTemperatureRangePatch {
		private static readonly MethodInfo _selPawnForGearMethod = PatchUtility.PropertyGetter(typeof(ITab_Pawn_Gear), "SelPawnForGear");

		[HarmonyPrefix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static bool Prefix(ITab_Pawn_Gear __instance) =>
#pragma warning restore IDE1006
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			!(_selPawnForGearMethod.Invoke(__instance, MiscellaneousUtility.EmptyArray()) is Pawn pawn)
#else
			_selPawnForGearMethod.Invoke(__instance, MiscellaneousUtility.EmptyArray()) is not Pawn pawn
#endif
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, pawn);
	}
}
