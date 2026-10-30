#if !(V1_0 || V1_1 || V1_2)
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
#endif

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.SocialCardUtilityPatches {
	[HarmonyPatch(typeof(SocialCardUtility), nameof(SocialCardUtility.DrawSocialCard))]
	public static class DrawSocialCardPatch {
#if !(V1_0 || V1_1 || V1_2)
		private static readonly MethodInfo _ideoMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.Ideo));
#endif

		[HarmonyPrefix]
		public static bool Prefix(Pawn pawn) =>
			KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, pawn, ControlCategory.Control) // Role selection dropdown.
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, pawn, ControlCategory.Control); // Romance button.

#if !(V1_0 || V1_1 || V1_2)
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
				yield return instruction;

				if (PatchUtility.Calls(instruction, _ideoMethod)) {
					// Ideoligion contains role selection dropdown.
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Ideoligion, getPawnInstructions, generator, controlCategory: ControlCategory.Control)) {
						yield return i;
					}

					continue;
				}
			}
		}
#endif
	}
}
