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
		private static readonly FieldInfo ParentField = AccessTools.Field(typeof(HediffComp), nameof(HediffComp.parent));

		private static readonly FieldInfo PawnField = AccessTools.Field(typeof(Hediff), nameof(Hediff.pawn));

		private static readonly MethodInfo MessageMethod = AccessTools.Method(typeof(Messages), nameof(Messages.Message), new Type[] { typeof(string), typeof(LookTargets), typeof(MessageTypeDef), typeof(bool) });

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldfld, ParentField), new CodeInstruction(OpCodes.Ldfld, PawnField) };

			foreach (CodeInstruction instruction in instructions) {
				if (PatchUtility.Calls(instruction, MessageMethod)) {
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
