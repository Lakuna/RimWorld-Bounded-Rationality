#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5)
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using Verse;

namespace Lakuna.BoundedRationality.Patches.HediffCompGiveHediffPatches {
	[HarmonyPatch(typeof(HediffComp_GiveHediff), "SendLetter")]
	public static class SendLetterPatch {
		[HarmonyPrefix]
		public static bool Prefix(HediffComp_CauseMentalState __instance) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, __instance.Pawn, ControlCategory.Letter);
	}
}
#endif
