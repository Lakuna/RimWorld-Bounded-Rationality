#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.ITabPawnVisitorPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Visitor), "DoSlaveTab")]
	public static class DoSlaveTabPatch {
		private static readonly MethodInfo SelPawnMethod = PatchUtility.PropertyGetter(typeof(ITab), "SelPawn");

		private static readonly MethodInfo CurLevelMethod = PatchUtility.PropertyGetter(typeof(Need), nameof(Need.CurLevel));

		private static readonly MethodInfo GetStatValueMethod = AccessTools.Method(typeof(StatExtension), nameof(StatExtension.GetStatValue));

		private static readonly MethodInfo ToStringPercentMethod = AccessTools.Method(typeof(GenText), nameof(GenText.ToStringPercent), new Type[] { typeof(float) });

		private static readonly MethodInfo ValueToStringMethod = AccessTools.Method(typeof(StatDef), nameof(StatDef.ValueToString));

		private static readonly MethodInfo TranslateSimpleMethod = AccessTools.Method(typeof(Translator), nameof(Translator.TranslateSimple));

		private static readonly MethodInfo GetTerrorThoughtsMethod = AccessTools.Method(typeof(TerrorUtility), nameof(TerrorUtility.GetTerrorThoughts));

		private static readonly ConstructorInfo ThoughtMemoryObservationTerrorListConstructor = AccessTools.Constructor(typeof(List<Thought_MemoryObservationTerror>));

		private static readonly MethodInfo InitiateSlaveRebellionMtbDaysMethod = AccessTools.Method(typeof(SlaveRebellionUtility), nameof(SlaveRebellionUtility.InitiateSlaveRebellionMtbDays));

		private static readonly MethodInfo FactionMethod = PatchUtility.PropertyGetter(typeof(Thing), nameof(Thing.Faction));

		private static readonly MethodInfo SlaveFactionMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.SlaveFaction));

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Callvirt, SelPawnMethod) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, CurLevelMethod) || PatchUtility.Calls(instruction, GetStatValueMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, 0f)) {
						yield return i;
					}

					continue;
				}

				// These methods are only called to stringify suppression and terror, so both can be replaced with "unknown" if meta information isn't known.
				if (PatchUtility.Calls(instruction, ToStringPercentMethod) || PatchUtility.Calls(instruction, ValueToStringMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, "BR.Unknown")) {
						yield return i;
					}

					yield return new CodeInstruction(OpCodes.Call, TranslateSimpleMethod);
					continue;
				}

				if (PatchUtility.Calls(instruction, GetTerrorThoughtsMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, ThoughtMemoryObservationTerrorListConstructor)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, InitiateSlaveRebellionMtbDaysMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, -1f)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, FactionMethod) || PatchUtility.Calls(instruction, SlaveFactionMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				// The time to slave rebellion is either actually never or replaced with never and shouldn't be known either way.
				// The relations gain on release is either actually none or replaced with none and shouldn't be known either way.
				if (instruction.LoadsConstant("Never") || instruction.LoadsConstant("None")) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, "BR.Unknown")) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
#endif
