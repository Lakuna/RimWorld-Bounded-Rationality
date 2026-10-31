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
	[HarmonyPatch(typeof(ITab_Pawn_Visitor), "DoPrisonerTab")]
	public static class DoPrisonerTabPatch {
		private static readonly MethodInfo _factionMethod = PatchUtility.PropertyGetter(typeof(Thing), nameof(Thing.Faction));

		private static readonly FieldInfo _finalResistanceInteractionDataField = AccessTools.Field(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.finalResistanceInteractionData));

		private static readonly MethodInfo _genePreventsPrisonBreakingMethod = AccessTools.Method(typeof(PrisonBreakUtility), nameof(PrisonBreakUtility.GenePreventsPrisonBreaking));

		private static readonly FieldInfo _ideoForConversionField = AccessTools.Field(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.ideoForConversion));

		private static readonly MethodInfo _initiatePrisonBreakMtbDaysMethod = AccessTools.Method(typeof(PrisonBreakUtility), nameof(PrisonBreakUtility.InitiatePrisonBreakMtbDays));

		private static readonly MethodInfo _isGuiltyMethod = PatchUtility.PropertyGetter(typeof(Pawn_GuiltTracker), nameof(Pawn_GuiltTracker.IsGuilty));

		private static readonly MethodInfo _isPrisonBreakingMethod = AccessTools.Method(typeof(PrisonBreakUtility), nameof(PrisonBreakUtility.IsPrisonBreaking));

		private static readonly FieldInfo _maxField = AccessTools.Field(typeof(FloatRange), nameof(FloatRange.max));

		private static readonly FieldInfo _minField = AccessTools.Field(typeof(FloatRange), nameof(FloatRange.min));

		private static readonly MethodInfo _recruitableMethod = PatchUtility.PropertyGetter(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.Recruitable));

		private static readonly FieldInfo _resistanceField = AccessTools.Field(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.resistance));

		private static readonly FieldInfo _royaltyField = AccessTools.Field(typeof(Pawn), nameof(Pawn.royalty));

		private static readonly MethodInfo _selPawnMethod = PatchUtility.PropertyGetter(typeof(ITab), "SelPawn");

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly MethodInfo _toStringMethod = AccessTools.Method(typeof(float), nameof(float.ToString), new Type[] { typeof(string) });
#else
		private static readonly MethodInfo _toStringMethod = AccessTools.Method(typeof(float), nameof(float.ToString), [typeof(string)]);
#endif

		private static readonly MethodInfo _translateSimpleMethod = AccessTools.Method(typeof(Translator), nameof(Translator.TranslateSimple));

		private static readonly FieldInfo _willField = AccessTools.Field(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.will));

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

				if (PatchUtility.Calls(instruction, _initiatePrisonBreakMtbDaysMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, -1f)) {
						yield return i;
					}

					continue;
				}

				// The time to prison break is either actually never or replaced with never and shouldn't be known either way.
				// The relations gain on release is either actually none or replaced with none and shouldn't be known either way.
				if (instruction.LoadsConstant("Never") || instruction.LoadsConstant("None")) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, "BR.Unknown")) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, _isPrisonBreakingMethod) || PatchUtility.Calls(instruction, _genePreventsPrisonBreakingMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Meta, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, _recruitableMethod)) {
					foreach (CodeInstruction i in PatchUtility.OrPawnNotKnown(InformationCategory.Meta, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, _resistanceField) || PatchUtility.LoadsField(instruction, _minField) || PatchUtility.LoadsField(instruction, _maxField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, 0f)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, _willField, true)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, 0f, localAddress: true)) {
						yield return i;
					}

					continue;
				}

				// `float.ToString` is only called to insert will and resistance into the UI.
				if (PatchUtility.Calls(instruction, _toStringMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, "BR.Unknown")) {
						yield return i;
					}

					yield return new CodeInstruction(OpCodes.Call, _translateSimpleMethod);
					continue;
				}

				if (PatchUtility.LoadsField(instruction, _ideoForConversionField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Ideoligion, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				// Royalty is used here only for royal title resistance offset.
				if (PatchUtility.LoadsField(instruction, _royaltyField) || PatchUtility.Calls(instruction, _factionMethod) || PatchUtility.LoadsField(instruction, _finalResistanceInteractionDataField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, _isGuiltyMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Basic, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
#endif
