#if !V1_0
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using Verse;

namespace Lakuna.BoundedRationality.Patches.HediffCompChangeImplantLevelPatches {
#if V1_1 || V1_2 || V1_3 || V1_4 || V1_5
	[HarmonyPatch(typeof(HediffComp_ChangeImplantLevel), nameof(HediffComp_ChangeImplantLevel.CompPostTick))]
#else
	[HarmonyPatch(typeof(HediffComp_ChangeImplantLevel), nameof(HediffComp_ChangeImplantLevel.CompPostTickInterval))]
#endif
	public static class CompPostTickIntervalPatch {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly MethodInfo _messageMethod = AccessTools.Method(typeof(Messages), nameof(Messages.Message), new Type[] { typeof(string), typeof(LookTargets), typeof(MessageTypeDef), typeof(bool) });
#else
		private static readonly MethodInfo _messageMethod = AccessTools.Method(typeof(Messages), nameof(Messages.Message), [typeof(string), typeof(LookTargets), typeof(MessageTypeDef), typeof(bool)]);
#endif

		private static readonly FieldInfo _parentField = AccessTools.Field(typeof(HediffComp), nameof(HediffComp.parent));

		private static readonly FieldInfo _pawnField = AccessTools.Field(typeof(Hediff), nameof(Hediff.pawn));

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
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldfld, _parentField), new CodeInstruction(OpCodes.Ldfld, _pawnField) };
#else
			CodeInstruction[] getPawnInstructions = [new(OpCodes.Ldarg_0), new(OpCodes.Ldfld, _parentField), new(OpCodes.Ldfld, _pawnField)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				if (PatchUtility.Calls(instruction, _messageMethod)) {
					foreach (CodeInstruction i in PatchUtility.SkipIfPawnNotKnown(instruction, InformationCategory.Health, getPawnInstructions, generator, controlCategory: ControlCategory.Message)) {
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
#endif
