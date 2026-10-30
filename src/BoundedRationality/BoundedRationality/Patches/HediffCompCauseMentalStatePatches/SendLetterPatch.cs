#if !V1_0
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using Verse;

namespace Lakuna.BoundedRationality.Patches.HediffCompCauseMentalStatePatches {
	[HarmonyPatch(typeof(HediffComp_CauseMentalState), "SendLetter")]
	public static class SendLetterPatch {
		[HarmonyPrefix]
		private static bool Prefix(HediffComp_CauseMentalState __instance) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, __instance.Pawn, ControlCategory.Letter);
	}
}
#endif
