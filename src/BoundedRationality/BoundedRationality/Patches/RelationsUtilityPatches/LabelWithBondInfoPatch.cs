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
		private static readonly MethodInfo DirectRelationExistsMethod = AccessTools.Method(typeof(Pawn_RelationsTracker), nameof(Pawn_RelationsTracker.DirectRelationExists));

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, DirectRelationExistsMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Social, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
