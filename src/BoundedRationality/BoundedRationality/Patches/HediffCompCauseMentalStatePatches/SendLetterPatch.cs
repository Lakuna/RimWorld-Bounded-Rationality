#if !V1_0
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using Verse;

namespace Lakuna.BoundedRationality.Patches.HediffCompCauseMentalStatePatches {
	[HarmonyPatch(typeof(HediffComp_CauseMentalState), "SendLetter")]
	public static class SendLetterPatch {
		[HarmonyPrefix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static bool Prefix(HediffComp_CauseMentalState __instance) =>
#pragma warning restore IDE1006
			KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, __instance?.Pawn, ControlCategory.Letter);
	}
}
#endif
