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
		public static void Postfix(
			Pawn initiator,
			Pawn recipient,
			ref string letterText,
			ref string letterLabel,
			ref LetterDef letterDef
#if !V1_0
			, ref LookTargets lookTargets
#endif
		) {
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
