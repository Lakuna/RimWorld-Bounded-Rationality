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

namespace Lakuna.BoundedRationality.Patches.RelationsUtilityPatches {
	[HarmonyPatch(typeof(RelationsUtility), nameof(RelationsUtility.LabelWithBondInfo))]
	public static class LabelWithBondInfoPatch {
		private static readonly MethodInfo _directRelationExistsMethod = AccessTools.Method(typeof(Pawn_RelationsTracker), nameof(Pawn_RelationsTracker.DirectRelationExists));

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}
#else
			ArgumentNullException.ThrowIfNull(instructions);
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };
#else
			CodeInstruction[] getPawnInstructions = [new(OpCodes.Ldarg_0)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, _directRelationExistsMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Social, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
