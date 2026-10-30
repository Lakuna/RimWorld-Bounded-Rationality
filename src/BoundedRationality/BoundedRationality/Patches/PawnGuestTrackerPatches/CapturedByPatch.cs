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

namespace Lakuna.BoundedRationality.Patches.PawnGuestTrackerPatches {
	[HarmonyPatch(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.CapturedBy))]
	public static class CapturedByPatch {
		private static readonly FieldInfo PawnField = AccessTools.Field(typeof(Pawn_GuestTracker), "pawn");

#if V1_0
		private static readonly MethodInfo ReceiveLetterMethod = AccessTools.Method(typeof(LetterStack), nameof(LetterStack.ReceiveLetter), new Type[] { typeof(string), typeof(string), typeof(LetterDef), typeof(LookTargets), typeof(Faction), typeof(string) });
#elif V1_1 || V1_2 || V1_3 || V1_4
		private static readonly MethodInfo ReceiveLetterMethod = AccessTools.Method(typeof(LetterStack), nameof(LetterStack.ReceiveLetter), new Type[] { typeof(TaggedString), typeof(TaggedString), typeof(LetterDef), typeof(LookTargets), typeof(Faction), typeof(Quest), typeof(List<ThingDef>), typeof(string) });
#else
		private static readonly MethodInfo ReceiveLetterMethod = AccessTools.Method(typeof(LetterStack), nameof(LetterStack.ReceiveLetter), new Type[] { typeof(TaggedString), typeof(TaggedString), typeof(LetterDef), typeof(LookTargets), typeof(Faction), typeof(Quest), typeof(List<ThingDef>), typeof(string), typeof(int), typeof(bool) });
#endif

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldfld, PawnField) };

			foreach (CodeInstruction instruction in instructions) {
				// Letter for when an unwaveringly loyal pawn is captured.
				if (PatchUtility.Calls(instruction, ReceiveLetterMethod)) {
					foreach (CodeInstruction i in PatchUtility.SkipIfPawnNotKnown(instruction, InformationCategory.Meta, getPawnInstructions, generator, controlCategory: ControlCategory.Letter)) {
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
