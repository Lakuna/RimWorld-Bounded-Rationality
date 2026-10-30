using System.Collections.Generic;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.PawnRelationUtilityPatches {
	[HarmonyPatch(typeof(PawnRelationUtility), nameof(PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter))]
	public static class NotifyPawnsSeenByPlayerLetterPatch {
		[HarmonyPrefix]
		private static bool Prefix(IEnumerable<Pawn> seenPawns) {
			foreach (Pawn pawn in seenPawns) {
				// Ignore seen pawns that don't contribute to the letter.
				Pawn relative = PawnRelationUtility.GetMostImportantColonyRelative(pawn);
				if (relative is null) {
					continue;
				}

				// If any seen pawn or that pawn's colonist relative is known, modify the letter as usual.
				if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, pawn, ControlCategory.Letter)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, relative, ControlCategory.Letter)) {
					return false;
				}
			}

			// If none of the seen pawns or their relatives are known, don't modify the letter.
			return false;
		}
	}
}
