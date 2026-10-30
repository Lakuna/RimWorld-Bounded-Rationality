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
#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5)
		private static readonly MethodInfo _getTrainabilityMethod = AccessTools.Method(typeof(TrainableUtility), nameof(TrainableUtility.GetTrainability));
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
		private static readonly MethodInfo _getWildnessExplanationMethod = AccessTools.Method(typeof(TrainableUtility), nameof(TrainableUtility.GetWildnessExplanation));
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
		private static readonly MethodInfo _labelCapMethod = PatchUtility.PropertyGetter(typeof(Def), nameof(Def.LabelCap)); // Used only for creature trainability in this method.
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly MethodInfo _toStringPercentMethod = AccessTools.Method(typeof(GenText), nameof(GenText.ToStringPercent), new Type[] { typeof(float) }); // Used only for creature wildness in this method.
#else
		private static readonly MethodInfo _toStringPercentMethod = AccessTools.Method(typeof(GenText), nameof(GenText.ToStringPercent), [typeof(float)]); // Used only for creature wildness in this method.
#endif

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
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_1) };
#else
			CodeInstruction[] getPawnInstructions = [new(OpCodes.Ldarg_1)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
				if (PatchUtility.Calls(instruction, _labelCapMethod) || PatchUtility.Calls(instruction, _getWildnessExplanationMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, string.Empty)) {
						yield return i;
					}

					continue;
				}
#else
				if (PatchUtility.Calls(instruction, _getTrainabilityMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#endif

				if (PatchUtility.Calls(instruction, _toStringPercentMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, string.Empty)) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
