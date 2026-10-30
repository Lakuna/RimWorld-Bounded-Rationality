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

namespace Lakuna.BoundedRationality.Patches.TrainingCardUtilityPatches {
	[HarmonyPatch(typeof(TrainingCardUtility), nameof(TrainingCardUtility.DrawTrainingCard))]
	public static class DrawTrainingCardPatch {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
		private static readonly MethodInfo LabelCapMethod = PatchUtility.PropertyGetter(typeof(Def), nameof(Def.LabelCap)); // Used only for creature trainability in this method.

		private static readonly MethodInfo GetWildnessExplanationMethod = AccessTools.Method(typeof(TrainableUtility), nameof(TrainableUtility.GetWildnessExplanation));
#else
		private static readonly MethodInfo GetTrainabilityMethod = AccessTools.Method(typeof(TrainableUtility), nameof(TrainableUtility.GetTrainability));
#endif

		private static readonly MethodInfo ToStringPercentMethod = AccessTools.Method(typeof(GenText), nameof(GenText.ToStringPercent), new Type[] { typeof(float) }); // Used only for creature wildness in this method.

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_1) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
				if (PatchUtility.Calls(instruction, LabelCapMethod) || PatchUtility.Calls(instruction, GetWildnessExplanationMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, string.Empty)) {
						yield return i;
					}

					continue;
				}
#else
				if (PatchUtility.Calls(instruction, GetTrainabilityMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#endif

				if (PatchUtility.Calls(instruction, ToStringPercentMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, string.Empty)) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
