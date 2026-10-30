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
		private static readonly MethodInfo _averageOfRelevantSkillsForMethod = AccessTools.Method(typeof(Pawn_SkillTracker), nameof(Pawn_SkillTracker.AverageOfRelevantSkillsFor));

#if !(V1_0 || V1_1 || V1_2)
		private static readonly MethodInfo _ideoMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.Ideo));
#endif

#if !V1_0
		private static readonly MethodInfo _workTypeIsDisabledMethod = AccessTools.Method(typeof(Pawn), nameof(Pawn.WorkTypeIsDisabled));
#endif

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			if (generator is null) {
				throw new ArgumentNullException(nameof(generator));
			}
#else
			ArgumentNullException.ThrowIfNull(instructions);
			ArgumentNullException.ThrowIfNull(generator);
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };
			CodeInstruction[] getWorkTypeInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_1) };
#else
			CodeInstruction[] getPawnInstructions = [new(OpCodes.Ldarg_0)];
			CodeInstruction[] getWorkTypeInstructions = [new(OpCodes.Ldarg_1)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

#if !V1_0
				if (PatchUtility.Calls(instruction, _workTypeIsDisabledMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfAnySkillsForWorkTypeNotKnown(getPawnInstructions, getWorkTypeInstructions, generator, false)) {
						yield return i;
					}

					continue;
				}
#endif

				if (PatchUtility.Calls(instruction, _averageOfRelevantSkillsForMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfAnySkillsForWorkTypeNotKnown(getPawnInstructions, getWorkTypeInstructions, generator, 0f)) {
						yield return i;
					}

					continue;
				}

#if !(V1_0 || V1_1 || V1_2)
				if (PatchUtility.Calls(instruction, _ideoMethod)) {
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
