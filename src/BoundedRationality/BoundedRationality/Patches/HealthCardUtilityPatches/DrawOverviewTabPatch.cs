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

namespace Lakuna.BoundedRationality.Patches.HealthCardUtilityPatches {
	[HarmonyPatch(typeof(HealthCardUtility), "DrawOverviewTab")]
	public static class DrawOverviewTabPatch {
		private static readonly MethodInfo DrawLeftRowMethod = AccessTools.Method(typeof(HealthCardUtility), "DrawLeftRow");

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_1) };

			foreach (CodeInstruction instruction in instructions) {
				// Don't call `HealthCardUtility.DrawLeftRow`; just pop its arguments instead.
				if (PatchUtility.Calls(instruction, DrawLeftRowMethod)) {
					foreach (CodeInstruction i in PatchUtility.SkipIfPawnNotKnown(instruction, InformationCategory.Health, getPawnInstructions, generator)) {
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
