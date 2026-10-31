#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using Verse;
using Verse.AI;

namespace Lakuna.BoundedRationality.Patches.MentalBreakWorkerPatches {
	[HarmonyPatch(typeof(MentalBreakWorker), "TrySendLetter")]
	public static class TrySendLetterPatch {
		[HarmonyPrefix]
		public static bool Prefix(Pawn pawn) =>
			KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, pawn, ControlCategory.Letter);
	}
}
