#if !(V1_0 || V1_1 || V1_2)
using System.Reflection;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.ITabPawnSlavePatches {
	[HarmonyPatch(typeof(ITab_Pawn_Slave), nameof(ITab_Pawn_Slave.IsVisible), MethodType.Getter)]
	public static class IsVisiblePatch {
		private static readonly MethodInfo SelPawnMethod = PatchUtility.PropertyGetter(typeof(ITab), "SelPawn");

		[HarmonyPostfix]
#pragma warning disable CA1707
		public static void Postfix(ITab_Pawn_Slave __instance, ref bool __result) =>
#pragma warning restore CA1707
			__result = __result
			&& (!(SelPawnMethod.Invoke(__instance, MiscellaneousUtility.EmptyArray()) is Pawn pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn, ControlCategory.Control));
	}
}
#endif
