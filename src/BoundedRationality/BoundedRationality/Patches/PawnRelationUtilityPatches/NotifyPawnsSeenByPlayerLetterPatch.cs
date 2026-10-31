using System;
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
		public static bool Prefix(IEnumerable<Pawn> seenPawns) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (seenPawns is null) {
				throw new ArgumentNullException(nameof(seenPawns));
			}
#else
			ArgumentNullException.ThrowIfNull(seenPawns);
#endif

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
