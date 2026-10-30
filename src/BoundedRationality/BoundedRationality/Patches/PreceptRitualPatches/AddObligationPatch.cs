#if !(V1_0 || V1_1 || V1_2)
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.PreceptRitualPatches {
	[HarmonyPatch(typeof(Precept_Ritual), nameof(Precept_Ritual.AddObligation))]
	public static class AddObligationPatch {
		private static readonly MethodInfo _ofPlayerSilentFailMethod = PatchUtility.PropertyGetter(typeof(Faction), nameof(Faction.OfPlayerSilentFail));

#if V1_3 || V1_4
		private static readonly MethodInfo _receiveLetterMethod = AccessTools.Method(typeof(LetterStack), nameof(LetterStack.ReceiveLetter), new Type[] { typeof(TaggedString), typeof(TaggedString), typeof(LetterDef), typeof(string) });

		private static readonly MethodInfo _receiveLetterMethod2 = AccessTools.Method(typeof(LetterStack), nameof(LetterStack.ReceiveLetter), new Type[] { typeof(TaggedString), typeof(TaggedString), typeof(LetterDef), typeof(LookTargets), typeof(Faction), typeof(Quest), typeof(List<ThingDef>), typeof(string) });
#elif V1_5 || V1_6
		private static readonly MethodInfo _receiveLetterMethod = AccessTools.Method(typeof(LetterStack), nameof(LetterStack.ReceiveLetter), new Type[] { typeof(TaggedString), typeof(TaggedString), typeof(LetterDef), typeof(string), typeof(int), typeof(bool) });

		private static readonly MethodInfo _receiveLetterMethod2 = AccessTools.Method(typeof(LetterStack), nameof(LetterStack.ReceiveLetter), new Type[] { typeof(TaggedString), typeof(TaggedString), typeof(LetterDef), typeof(LookTargets), typeof(Faction), typeof(Quest), typeof(List<ThingDef>), typeof(string), typeof(int), typeof(bool) });
#else
		private static readonly MethodInfo _receiveLetterMethod = AccessTools.Method(typeof(LetterStack), nameof(LetterStack.ReceiveLetter), [typeof(TaggedString), typeof(TaggedString), typeof(LetterDef), typeof(string), typeof(int), typeof(bool)]);

		private static readonly MethodInfo _receiveLetterMethod2 = AccessTools.Method(typeof(LetterStack), nameof(LetterStack.ReceiveLetter), [typeof(TaggedString), typeof(TaggedString), typeof(LetterDef), typeof(LookTargets), typeof(Faction), typeof(Quest), typeof(List<ThingDef>), typeof(string), typeof(int), typeof(bool)]);
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
			CodeInstruction[] getFactionInstructions = new CodeInstruction[] { PatchUtility.LoadValue(_ofPlayerSilentFailMethod) };
#else
			CodeInstruction[] getFactionInstructions = [PatchUtility.LoadValue(_ofPlayerSilentFailMethod)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				// Don't inform the player about ritual opportunities if they don't know about their own ideoligion. This probably won't ever apply...
				if (PatchUtility.Calls(instruction, _receiveLetterMethod) || PatchUtility.Calls(instruction, _receiveLetterMethod2)) {
					foreach (CodeInstruction i in PatchUtility.SkipIfFactionNotKnown(instruction, InformationCategory.Ideoligion, getFactionInstructions, generator, controlCategory: ControlCategory.Letter)) {
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
