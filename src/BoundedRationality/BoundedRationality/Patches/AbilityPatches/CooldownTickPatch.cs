#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.AbilityPatches {
	[HarmonyPatch(typeof(Ability), "CooldownTick")]
	public static class CooldownTickPatch {
		private static readonly MethodInfo _messageShowAllowedMethod = AccessTools.Method(typeof(MessagesRepeatAvoider), nameof(MessagesRepeatAvoider.MessageShowAllowed));

		private static readonly FieldInfo _pawnField = AccessTools.Field(typeof(Ability), nameof(Ability.pawn));

		private static readonly MethodInfo _shouldSendNotificationAboutMethod = AccessTools.Method(typeof(PawnUtility), nameof(PawnUtility.ShouldSendNotificationAbout));

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
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldfld, _pawnField) };
#else
			CodeInstruction[] getPawnInstructions = [new(OpCodes.Ldarg_0), new(OpCodes.Ldfld, _pawnField)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				// Used for both a message and a letter.
				if (PatchUtility.Calls(instruction, _shouldSendNotificationAboutMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Abilities, getPawnInstructions, ControlCategory.Letter)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, _messageShowAllowedMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Abilities, getPawnInstructions, ControlCategory.Message)) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
#endif
