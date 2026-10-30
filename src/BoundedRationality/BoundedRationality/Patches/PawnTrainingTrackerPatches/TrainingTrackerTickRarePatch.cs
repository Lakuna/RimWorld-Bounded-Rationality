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

namespace Lakuna.BoundedRationality.Patches.PawnTrainingTrackerPatches {
	[HarmonyPatch(typeof(Pawn_TrainingTracker), nameof(Pawn_TrainingTracker.TrainingTrackerTickRare))]
	public static class TrainingTrackerTickRarePatch {
		private static readonly MethodInfo _messageMethod = AccessTools.Method(typeof(Messages), nameof(Messages.Message), new Type[] { typeof(string), typeof(LookTargets), typeof(MessageTypeDef), typeof(bool) });

#if V1_0 || V1_1 || V1_2
		private static readonly FieldInfo _pawnField = AccessTools.Field(typeof(Pawn_TrainingTracker), "pawn");
#else
		private static readonly FieldInfo _pawnField = AccessTools.Field(typeof(Pawn_TrainingTracker), nameof(Pawn_TrainingTracker.pawn));
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
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldfld, _pawnField) };
#else
			CodeInstruction[] getPawnInstructions = [new(OpCodes.Ldarg_0), new(OpCodes.Ldfld, _pawnField)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				if (PatchUtility.Calls(instruction, _messageMethod)) {
					foreach (CodeInstruction i in PatchUtility.SkipIfPawnNotKnown(instruction, InformationCategory.Basic, getPawnInstructions, generator, controlCategory: ControlCategory.Message)) {
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
