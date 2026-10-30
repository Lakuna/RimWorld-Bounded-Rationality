#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using Verse;

namespace Lakuna.BoundedRationality.Patches.HediffGiverPatches {
	[HarmonyPatch(typeof(HediffGiver), "SendLetter")]
	public static class SendLetterPatch {
		[HarmonyPrefix]
		public static bool Prefix(Pawn pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, pawn, ControlCategory.Letter);
	}
}
