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

#if !(V1_0 || V1_1 || V1_2)
using Verse;
#endif

namespace Lakuna.BoundedRationality.Patches.WidgetsWorkPatches {
	[HarmonyPatch(typeof(WidgetsWork), "DrawWorkBoxBackground")]
	public static class DrawWorkBoxBackgroundPatch {
		private static readonly MethodInfo _averageOfRelevantSkillsForMethod = AccessTools.Method(typeof(Pawn_SkillTracker), nameof(Pawn_SkillTracker.AverageOfRelevantSkillsFor));

#if !(V1_0 || V1_1 || V1_2)
		private static readonly MethodInfo _ideoMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.Ideo));
#endif

		private static readonly MethodInfo _maxPassionOfRelevantSkillsForMethod = AccessTools.Method(typeof(Pawn_SkillTracker), nameof(Pawn_SkillTracker.MaxPassionOfRelevantSkillsFor));

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
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_1) };
			CodeInstruction[] getWorkTypeInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_2) };
#else
			CodeInstruction[] getPawnInstructions = [new(OpCodes.Ldarg_1)];
			CodeInstruction[] getWorkTypeInstructions = [new(OpCodes.Ldarg_2)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

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

				if (PatchUtility.Calls(instruction, _maxPassionOfRelevantSkillsForMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfAnySkillsForWorkTypeNotKnown(getPawnInstructions, getWorkTypeInstructions, generator, Passion.None)) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
