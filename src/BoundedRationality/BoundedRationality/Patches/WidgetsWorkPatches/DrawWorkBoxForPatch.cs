#if !(V1_0 || V1_1 || V1_2)
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.WidgetsWorkPatches {
	[HarmonyPatch(typeof(WidgetsWork), nameof(WidgetsWork.DrawWorkBoxFor))]
	public static class DrawWorkBoxForPatch {
		private static readonly MethodInfo _ideoMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.Ideo));

#if !V1_3
		private static readonly MethodInfo _isWorkTypeDisabledByAgeMethod = AccessTools.Method(typeof(PawnUtility), nameof(PawnUtility.IsWorkTypeDisabledByAge));
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
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_2) };
			CodeInstruction[] getWorkTypeInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_3) };
#else
			CodeInstruction[] getPawnInstructions = [new(OpCodes.Ldarg_2)];
			CodeInstruction[] getWorkTypeInstructions = [new(OpCodes.Ldarg_3)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, _ideoMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Ideoligion, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

#if !V1_3
				if (PatchUtility.Calls(instruction, _isWorkTypeDisabledByAgeMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator, false)) {
						yield return i;
					}

					continue;
				}
#endif
			}
		}
	}
}
#endif
