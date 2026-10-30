#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.HediffDeathRefusalPatches {
	[HarmonyPatch(typeof(Hediff_DeathRefusal), nameof(Hediff_DeathRefusal.Notify_PawnDied))]
	public static class NotifyPawnDiedPatch {
		private static readonly FieldInfo PawnField = AccessTools.Field(typeof(Hediff), nameof(Hediff.pawn));

		private static readonly MethodInfo ShouldSendNotificationAboutMethod = AccessTools.Method(typeof(PawnUtility), nameof(PawnUtility.ShouldSendNotificationAbout));

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldfld, PawnField) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, ShouldSendNotificationAboutMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Health, getPawnInstructions, ControlCategory.Message)) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
#endif
