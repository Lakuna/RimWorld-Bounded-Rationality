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

#if !V1_0
using Verse;
#endif

namespace Lakuna.BoundedRationality.Patches.WidgetsWorkPatches {
	[HarmonyPatch(typeof(WidgetsWork), nameof(WidgetsWork.TipForPawnWorker))]
	public static class TipForPawnWorkerPatch {
#if !V1_0
		private static readonly MethodInfo WorkTypeIsDisabledMethod = AccessTools.Method(typeof(Pawn), nameof(Pawn.WorkTypeIsDisabled));
#endif

		private static readonly MethodInfo AverageOfRelevantSkillsForMethod = AccessTools.Method(typeof(Pawn_SkillTracker), nameof(Pawn_SkillTracker.AverageOfRelevantSkillsFor));

#if !(V1_0 || V1_1 || V1_2)
		private static readonly MethodInfo IdeoMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.Ideo));
#endif

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			if (generator is null) {
				throw new ArgumentNullException(nameof(generator));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };
			CodeInstruction[] getWorkTypeInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_1) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

#if !V1_0
				if (PatchUtility.Calls(instruction, WorkTypeIsDisabledMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfAnySkillsForWorkTypeNotKnown(getPawnInstructions, getWorkTypeInstructions, generator, false)) {
						yield return i;
					}

					continue;
				}
#endif

				if (PatchUtility.Calls(instruction, AverageOfRelevantSkillsForMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfAnySkillsForWorkTypeNotKnown(getPawnInstructions, getWorkTypeInstructions, generator, 0f)) {
						yield return i;
					}

					continue;
				}

#if !(V1_0 || V1_1 || V1_2)
				if (PatchUtility.Calls(instruction, IdeoMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Ideoligion, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#endif
			}
		}
	}
}
