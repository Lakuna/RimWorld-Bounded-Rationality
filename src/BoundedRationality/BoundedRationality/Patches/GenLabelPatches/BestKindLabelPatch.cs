using System;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.GenLabelPatches {
	[HarmonyPatch(typeof(GenLabel), nameof(GenLabel.BestKindLabel), new Type[] { typeof(Pawn), typeof(bool), typeof(bool), typeof(bool), typeof(int) })]
	public static class BestKindLabelPatch {
		[HarmonyPrefix]
		public static void Prefix(Pawn pawn, ref bool mustNoteGender, ref bool mustNoteLifeStage) {
			bool basic = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn);
			mustNoteGender = mustNoteGender && basic;
			mustNoteLifeStage = mustNoteLifeStage && basic;
		}

		[HarmonyPostfix]
#pragma warning disable CA1707
		public static void Postfix(Pawn pawn, ref string __result) {
#pragma warning restore CA1707
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn) || MiscellaneousUtility.IsAnimal(pawn)) { // Animals should still show their kind since it's their species.
				return;
			}

			// If this isn't done, NPC humans with basic information not known will show their kind (i.e. "mercenary gunner").
			__result = "BR.Unknown".Translate();
		}
	}
}
