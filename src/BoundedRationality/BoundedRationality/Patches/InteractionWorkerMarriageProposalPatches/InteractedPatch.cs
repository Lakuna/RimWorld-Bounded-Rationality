#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.InteractionWorkerMarriageProposalPatches {
	[HarmonyPatch(typeof(InteractionWorker_MarriageProposal), nameof(InteractionWorker_MarriageProposal.Interacted))]
	public static class InteractedPatch {
		[HarmonyPostfix]
#if V1_0
		public static void Postfix(Pawn initiator, Pawn recipient, ref string letterText, ref string letterLabel, ref LetterDef letterDef) {
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static void Postfix(Pawn initiator, Pawn recipient, ref string letterText, ref string letterLabel, ref LetterDef letterDef, ref LookTargets lookTargets) {
#else
		public static void Postfix(Pawn initiator, Pawn recipient, ref string? letterText, ref string? letterLabel, ref LetterDef? letterDef, ref LookTargets? lookTargets) {
#endif
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, initiator) || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, recipient)) {
				return;
			}

			letterText = null;
			letterLabel = null;
			letterDef = null;
#if !V1_0
			lookTargets = null;
#endif
		}
	}
}
