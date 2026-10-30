#if !(V1_0 || V1_1 || V1_2)
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.InteractionWorkerConvertIdeoAttemptPatches {
	[HarmonyPatch(typeof(InteractionWorker_ConvertIdeoAttempt), nameof(InteractionWorker_ConvertIdeoAttempt.Interacted))]
	public static class InteractedPatch {
		private static readonly MethodInfo ShouldSendNotificationAboutMethod = AccessTools.Method(typeof(PawnUtility), nameof(PawnUtility.ShouldSendNotificationAbout));

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_1) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				// Message contains certainty information, so is categorized as "meta."
				if (PatchUtility.Calls(instruction, ShouldSendNotificationAboutMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Meta, getPawnInstructions, ControlCategory.Message)) {
						yield return i;
					}

					continue;
				}
			}
		}

		[HarmonyPostfix]
		public static void Postfix(Pawn initiator, Pawn recipient, ref string letterText, ref string letterLabel, ref LetterDef letterDef, ref LookTargets lookTargets) {
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, initiator) || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, recipient)) {
				return;
			}

			letterText = null;
			letterLabel = null;
			letterDef = null;
			lookTargets = null;
		}
	}
}
#endif
