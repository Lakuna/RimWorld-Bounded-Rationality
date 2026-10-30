#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.HediffCompReplaceHediffPatches {
	[HarmonyPatch(typeof(HediffComp_ReplaceHediff), nameof(HediffComp_ReplaceHediff.Trigger))]
	public static class TriggerPatch {
		private static readonly FieldInfo ParentField = AccessTools.Field(typeof(HediffComp), nameof(HediffComp.parent));

		private static readonly FieldInfo PawnField = AccessTools.Field(typeof(Hediff), nameof(Hediff.pawn));

		private static readonly MethodInfo ShouldSendNotificationAboutMethod = AccessTools.Method(typeof(PawnUtility), nameof(PawnUtility.ShouldSendNotificationAbout));

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldfld, ParentField), new CodeInstruction(OpCodes.Ldfld, PawnField) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				// Used for both a message and a letter.
				if (PatchUtility.Calls(instruction, ShouldSendNotificationAboutMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Health, getPawnInstructions, ControlCategory.Letter)) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
#endif
