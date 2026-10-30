using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.InteractionWorkerRomanceAttemptPatches {
	[HarmonyPatch(typeof(InteractionWorker_RomanceAttempt), nameof(InteractionWorker_RomanceAttempt.Interacted))]
	public static class InteractedPatch {
		private static readonly MethodInfo MessageMethod = AccessTools.Method(typeof(Messages), nameof(Messages.Message), new Type[] { typeof(string), typeof(LookTargets), typeof(MessageTypeDef), typeof(bool) });

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_2) };

			foreach (CodeInstruction instruction in instructions) {
				// The message is received only when the romance attempt fails.
				if (PatchUtility.Calls(instruction, MessageMethod)) {
					foreach (CodeInstruction i in PatchUtility.SkipIfPawnNotKnown(instruction, InformationCategory.Social, getPawnInstructions, generator, controlCategory: ControlCategory.Message)) {
						yield return i;
					}

					// Skip the normal instruction (already returned above).
					continue;
				}

				yield return instruction;
			}
		}

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
