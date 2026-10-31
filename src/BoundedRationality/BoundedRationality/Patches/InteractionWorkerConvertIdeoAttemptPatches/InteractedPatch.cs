#if !(V1_0 || V1_1 || V1_2)
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.InteractionWorkerConvertIdeoAttemptPatches {
	[HarmonyPatch(typeof(InteractionWorker_ConvertIdeoAttempt), nameof(InteractionWorker_ConvertIdeoAttempt.Interacted))]
	public static class InteractedPatch {
		private static readonly MethodInfo _shouldSendNotificationAboutMethod = AccessTools.Method(typeof(PawnUtility), nameof(PawnUtility.ShouldSendNotificationAbout));

		[HarmonyPostfix]
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static void Postfix(Pawn initiator, Pawn recipient, ref string letterText, ref string letterLabel, ref LetterDef letterDef, ref LookTargets lookTargets) {
#else
		public static void Postfix(Pawn initiator, Pawn recipient, ref string? letterText, ref string? letterLabel, ref LetterDef? letterDef, ref LookTargets? lookTargets) {
#endif
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, initiator) || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, recipient)) {
				return;
			}

			letterText = null;
			letterLabel = null;
			letterDef = null;
			lookTargets = null;
		}

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
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_1) };
#else
			CodeInstruction[] getPawnInstructions = [new(OpCodes.Ldarg_1)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				// Message contains certainty information, so is categorized as "meta."
				if (PatchUtility.Calls(instruction, _shouldSendNotificationAboutMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Meta, getPawnInstructions, ControlCategory.Message)) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
#endif
