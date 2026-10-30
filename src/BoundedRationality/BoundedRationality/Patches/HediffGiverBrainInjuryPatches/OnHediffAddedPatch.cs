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

namespace Lakuna.BoundedRationality.Patches.HediffGiverBrainInjuryPatches {
	[HarmonyPatch(typeof(HediffGiver_BrainInjury), nameof(HediffGiver_BrainInjury.OnHediffAdded))]
	public static class OnHediffAddedPatch {
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

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };

			foreach (CodeInstruction instruction in instructions) {
				if (PatchUtility.Calls(instruction, ReceiveLetterMethod)) {
					foreach (CodeInstruction i in PatchUtility.SkipIfPawnNotKnown(instruction, InformationCategory.Health, getPawnInstructions, generator, controlCategory: ControlCategory.Letter)) {
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
