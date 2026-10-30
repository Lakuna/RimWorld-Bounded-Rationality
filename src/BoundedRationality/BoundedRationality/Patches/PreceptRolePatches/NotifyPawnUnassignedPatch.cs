#if !(V1_0 || V1_1 || V1_2)
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.PreceptRolePatches {
	[HarmonyPatch(typeof(Precept_Role), nameof(Precept_Role.Notify_PawnUnassigned))]
	public static class NotifyPawnUnassignedPatch {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly MethodInfo _messageMethod = AccessTools.Method(typeof(Messages), nameof(Messages.Message), new Type[] { typeof(string), typeof(LookTargets), typeof(MessageTypeDef), typeof(bool) });
#else
		private static readonly MethodInfo _messageMethod = AccessTools.Method(typeof(Messages), nameof(Messages.Message), [typeof(string), typeof(LookTargets), typeof(MessageTypeDef), typeof(bool)]);
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
				if (PatchUtility.Calls(instruction, _messageMethod)) {
					foreach (CodeInstruction i in PatchUtility.SkipIfPawnNotKnown(instruction, InformationCategory.Ideoligion, getPawnInstructions, generator, controlCategory: ControlCategory.Message)) {
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
