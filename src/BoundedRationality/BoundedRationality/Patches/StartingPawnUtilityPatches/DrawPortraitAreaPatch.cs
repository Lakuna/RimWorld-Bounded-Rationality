#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using Verse;

namespace Lakuna.BoundedRationality.Patches.StartingPawnUtilityPatches {
	[HarmonyPatch(typeof(StartingPawnUtility), nameof(StartingPawnUtility.DrawPortraitArea))]
	public static class DrawPortraitAreaPatch {
		private static readonly MethodInfo _anyMethod = SymbolExtensions.GetMethodInfo((List<ThingDefCount> list) => list.Any());

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
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldloc_1) };
#else
			CodeInstruction[] getPawnInstructions = [new(OpCodes.Ldloc_1)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				// Used only to check if the pawn has any possessions in this method.
				if (PatchUtility.Calls(instruction, _anyMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Gear, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
#endif
