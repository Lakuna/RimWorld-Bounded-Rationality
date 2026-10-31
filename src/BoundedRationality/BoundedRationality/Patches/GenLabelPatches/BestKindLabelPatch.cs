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
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
	[HarmonyPatch(typeof(GenLabel), nameof(GenLabel.BestKindLabel), new Type[] { typeof(Pawn), typeof(bool), typeof(bool), typeof(bool), typeof(int) })]
#else
	[HarmonyPatch(typeof(GenLabel), nameof(GenLabel.BestKindLabel), [typeof(Pawn), typeof(bool), typeof(bool), typeof(bool), typeof(int)])]
#endif
	public static class BestKindLabelPatch {
		[HarmonyPrefix]
		public static void Prefix(Pawn pawn, ref bool mustNoteGender, ref bool mustNoteLifeStage) {
			bool basic = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn);
			mustNoteGender = mustNoteGender && basic;
			mustNoteLifeStage = mustNoteLifeStage && basic;
		}

		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(Pawn pawn, ref string __result) {
#pragma warning restore IDE1006
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn) || MiscellaneousUtility.IsAnimal(pawn)) { // Animals should still show their kind since it's their species.
				return;
			}

			// If this isn't done, NPC humans with basic information not known will show their kind (i.e. "mercenary gunner").
			__result = "BR.Unknown".Translate();
		}
	}
}
