using System.Reflection;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.ITabPawnSocialPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Social), nameof(ITab_Pawn_Social.IsVisible), MethodType.Getter)]
	public static class IsVisiblePatch {
		private static readonly MethodInfo SelPawnForSocialInfoMethod = PatchUtility.PropertyGetter(typeof(ITab_Pawn_Social), "SelPawnForSocialInfo");

		[HarmonyPostfix]
#pragma warning disable CA1707
		public static void Postfix(ITab_Pawn_Social __instance, ref bool __result) =>
#pragma warning restore CA1707
			__result = __result
			&& (!(SelPawnForSocialInfoMethod.Invoke(__instance, MiscellaneousUtility.EmptyArray()) is Pawn pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, pawn, ControlCategory.Control) // "Romance" button.
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, pawn, ControlCategory.Control)); // "Assign role" button.
	}
}
