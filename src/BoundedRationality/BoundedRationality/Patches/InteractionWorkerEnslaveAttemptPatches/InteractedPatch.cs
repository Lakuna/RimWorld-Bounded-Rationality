#if !(V1_0 || V1_1 || V1_2)
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.BoundedRationality.Patches.InteractionWorkerEnslaveAttemptPatches {
	[HarmonyPatch(typeof(InteractionWorker_EnslaveAttempt), nameof(InteractionWorker_EnslaveAttempt.Interacted))]
	public static class InteractedPatch {
#if V1_3 || V1_4 || V1_5 || V1_6
		private static readonly MethodInfo _messageMethod = AccessTools.Method(typeof(Messages), nameof(Messages.Message), new Type[] { typeof(string), typeof(LookTargets), typeof(MessageTypeDef), typeof(bool) });
#else
		private static readonly MethodInfo _messageMethod = AccessTools.Method(typeof(Messages), nameof(Messages.Message), [typeof(string), typeof(LookTargets), typeof(MessageTypeDef), typeof(bool)]);
#endif

#if V1_3 || V1_4 || V1_5 || V1_6
		private static readonly MethodInfo _throwTextMethod = AccessTools.Method(typeof(MoteMaker), nameof(MoteMaker.ThrowText), new Type[] { typeof(Vector3), typeof(Map), typeof(string), typeof(float) });
#else
		private static readonly MethodInfo _throwTextMethod = AccessTools.Method(typeof(MoteMaker), nameof(MoteMaker.ThrowText), [typeof(Vector3), typeof(Map), typeof(string), typeof(float)]);
#endif

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
#if V1_3 || V1_4 || V1_5 || V1_6
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}
#else
			ArgumentNullException.ThrowIfNull(instructions);
#endif

#if V1_3 || V1_4 || V1_5 || V1_6
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_2) };
#else
			CodeInstruction[] getPawnInstructions = [new(OpCodes.Ldarg_2)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				// This text mote is thrown only when the enslavement attempt fails. Categorized as "meta" because the mote contains will information.
				if (PatchUtility.Calls(instruction, _throwTextMethod)) {
					foreach (CodeInstruction i in PatchUtility.SkipIfPawnNotKnown(instruction, InformationCategory.Meta, getPawnInstructions, generator, controlCategory: ControlCategory.TextMote)) {
						yield return i;
					}

					// Skip the normal instruction (already returned above).
					continue;
				}

				// The message is received only when the prisoner's will is broken. Categorized as "meta" because the message contains will information.
				if (PatchUtility.Calls(instruction, _messageMethod)) {
					foreach (CodeInstruction i in PatchUtility.SkipIfPawnNotKnown(instruction, InformationCategory.Meta, getPawnInstructions, generator, controlCategory: ControlCategory.Message)) {
						yield return i;
					}

					// Skip the normal instruction (already returned above).
					continue;
				}

				yield return instruction;
			}
		}

		[HarmonyPostfix]
#if V1_3 || V1_4 || V1_5 || V1_6
		public static void Postfix(Pawn initiator, Pawn recipient, ref string letterText, ref string letterLabel, ref LetterDef letterDef, ref LookTargets lookTargets) {
#else
		public static void Postfix(Pawn initiator, Pawn recipient, ref string? letterText, ref string? letterLabel, ref LetterDef? letterDef, ref LookTargets? lookTargets) {
#endif
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, initiator) || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, recipient)) {
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
