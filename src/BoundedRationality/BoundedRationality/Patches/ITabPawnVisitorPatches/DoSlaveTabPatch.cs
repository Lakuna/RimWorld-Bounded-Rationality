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
		private static readonly MethodInfo _curLevelMethod = PatchUtility.PropertyGetter(typeof(Need), nameof(Need.CurLevel));

		private static readonly MethodInfo _factionMethod = PatchUtility.PropertyGetter(typeof(Thing), nameof(Thing.Faction));

		private static readonly MethodInfo _getStatValueMethod = AccessTools.Method(typeof(StatExtension), nameof(StatExtension.GetStatValue));

		private static readonly MethodInfo _getTerrorThoughtsMethod = AccessTools.Method(typeof(TerrorUtility), nameof(TerrorUtility.GetTerrorThoughts));

		private static readonly MethodInfo _initiateSlaveRebellionMtbDaysMethod = AccessTools.Method(typeof(SlaveRebellionUtility), nameof(SlaveRebellionUtility.InitiateSlaveRebellionMtbDays));

		private static readonly MethodInfo _selPawnMethod = PatchUtility.PropertyGetter(typeof(ITab), "SelPawn");

		private static readonly MethodInfo _slaveFactionMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.SlaveFaction));

		private static readonly ConstructorInfo _thoughtMemoryObservationTerrorListConstructor = AccessTools.Constructor(typeof(List<Thought_MemoryObservationTerror>));

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly MethodInfo _toStringPercentMethod = AccessTools.Method(typeof(GenText), nameof(GenText.ToStringPercent), new Type[] { typeof(float) });
#else
		private static readonly MethodInfo _toStringPercentMethod = AccessTools.Method(typeof(GenText), nameof(GenText.ToStringPercent), [typeof(float)]);
#endif

		private static readonly MethodInfo _translateSimpleMethod = AccessTools.Method(typeof(Translator), nameof(Translator.TranslateSimple));

		private static readonly MethodInfo _valueToStringMethod = AccessTools.Method(typeof(StatDef), nameof(StatDef.ValueToString));

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
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Callvirt, _selPawnMethod) };
#else
			CodeInstruction[] getPawnInstructions = [new(OpCodes.Ldarg_0), new(OpCodes.Callvirt, _selPawnMethod)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, _curLevelMethod) || PatchUtility.Calls(instruction, _getStatValueMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, 0f)) {
						yield return i;
					}

					continue;
				}

				// These methods are only called to stringify suppression and terror, so both can be replaced with "unknown" if meta information isn't known.
				if (PatchUtility.Calls(instruction, _toStringPercentMethod) || PatchUtility.Calls(instruction, _valueToStringMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, "BR.Unknown")) {
						yield return i;
					}

					yield return new CodeInstruction(OpCodes.Call, _translateSimpleMethod);
					continue;
				}

				if (PatchUtility.Calls(instruction, _getTerrorThoughtsMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, _thoughtMemoryObservationTerrorListConstructor)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, _initiateSlaveRebellionMtbDaysMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, -1f)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, _factionMethod) || PatchUtility.Calls(instruction, _slaveFactionMethod)) {
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
