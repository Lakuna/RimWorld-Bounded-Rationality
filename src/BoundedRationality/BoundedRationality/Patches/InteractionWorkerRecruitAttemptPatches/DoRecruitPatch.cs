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

using UnityEngine;

using Verse;

namespace Lakuna.BoundedRationality.Patches.InteractionWorkerRecruitAttemptPatches {
#if V1_0 || V1_1 || V1_2
	[HarmonyPatch(
		typeof(InteractionWorker_RecruitAttempt),
		nameof(InteractionWorker_RecruitAttempt.DoRecruit),
		new Type[] { typeof(Pawn), typeof(Pawn), typeof(float), typeof(string), typeof(string), typeof(bool), typeof(bool) },
		new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Out, ArgumentType.Normal, ArgumentType.Normal }
	)]
#else
	[HarmonyPatch(
		typeof(InteractionWorker_RecruitAttempt),
		nameof(InteractionWorker_RecruitAttempt.DoRecruit),
		new Type[] { typeof(Pawn), typeof(Pawn), typeof(string), typeof(string), typeof(bool), typeof(bool) },
		new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Out, ArgumentType.Normal, ArgumentType.Normal }
	)]
#endif
	public static class DoRecruitPatch {
		private static readonly MethodInfo ThrowTextMethod = AccessTools.Method(typeof(MoteMaker), nameof(MoteMaker.ThrowText), new Type[] { typeof(Vector3), typeof(Map), typeof(string), typeof(float) });

		private static readonly MethodInfo MessageMethod = AccessTools.Method(typeof(Messages), nameof(Messages.Message), new Type[] { typeof(string), typeof(LookTargets), typeof(MessageTypeDef), typeof(bool) });

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_1) };

			foreach (CodeInstruction instruction in instructions) {
				// This text mote is thrown only when the tame attempt succeeds.
				if (PatchUtility.Calls(instruction, ThrowTextMethod)) {
					foreach (CodeInstruction i in PatchUtility.SkipIfPawnNotKnown(instruction, InformationCategory.Basic, getPawnInstructions, generator, controlCategory: ControlCategory.TextMote)) {
						yield return i;
					}

					// Skip the normal instruction (already returned above).
					continue;
				}

				// This message is sent only when the tame attempt succeeds.
				if (PatchUtility.Calls(instruction, MessageMethod)) {
					foreach (CodeInstruction i in PatchUtility.SkipIfPawnNotKnown(instruction, InformationCategory.Basic, getPawnInstructions, generator, controlCategory: ControlCategory.Message)) {
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
