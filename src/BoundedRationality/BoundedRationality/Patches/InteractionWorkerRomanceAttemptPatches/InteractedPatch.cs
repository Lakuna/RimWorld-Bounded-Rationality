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
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly MethodInfo _messageMethod = AccessTools.Method(typeof(Messages), nameof(Messages.Message), new Type[] { typeof(string), typeof(LookTargets), typeof(MessageTypeDef), typeof(bool) });
#else
		private static readonly MethodInfo _messageMethod = AccessTools.Method(typeof(Messages), nameof(Messages.Message), [typeof(string), typeof(LookTargets), typeof(MessageTypeDef), typeof(bool)]);
#endif

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

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}
#else
			ArgumentNullException.ThrowIfNull(instructions);
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_2) };
#else
			CodeInstruction[] getPawnInstructions = [new(OpCodes.Ldarg_2)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				// The message is received only when the romance attempt fails.
				if (PatchUtility.Calls(instruction, _messageMethod)) {
					foreach (CodeInstruction i in PatchUtility.SkipIfPawnNotKnown(instruction, InformationCategory.Social, getPawnInstructions, generator, controlCategory: ControlCategory.Message)) {
						yield return i;
					}

					// Skip the normal instruction (already returned above).
					continue;
				}

				yield return instruction;
			}
		}
	}
}
