#if !(V1_0 || V1_1 || V1_2)
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.SocialCardUtilityPatches {
	[HarmonyPatch(typeof(SocialCardUtility), nameof(SocialCardUtility.DrawPawnCertainty))]
	public static class DrawPawnCertaintyPatch {
		private static readonly MethodInfo _certaintyChangePerDayMethod = PatchUtility.PropertyGetter(typeof(Pawn_IdeoTracker), nameof(Pawn_IdeoTracker.CertaintyChangePerDay));

		private static readonly MethodInfo _certaintyMethod = PatchUtility.PropertyGetter(typeof(Pawn_IdeoTracker), nameof(Pawn_IdeoTracker.Certainty));

#if V1_3
		private static readonly MethodInfo _getStatValueMethod = SymbolExtensions.GetMethodInfo((Pawn pawn) => pawn.GetStatValue(StatDefOf.CertaintyLossFactor, true));
#else
		private static readonly MethodInfo _getStatValueMethod = SymbolExtensions.GetMethodInfo((Pawn pawn) => pawn.GetStatValue(StatDefOf.CertaintyLossFactor, true, -1));
#endif

		private static readonly FieldInfo _allTraitsField = AccessTools.Field(typeof(TraitSet), nameof(TraitSet.allTraits));

		private static readonly MethodInfo _filterTraitsMethod = AccessTools.Method(typeof(DrawPawnCertaintyPatch), nameof(FilterTraits));

#pragma warning disable CA1002 // Use the same type as the value being patched.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static List<Trait>? FilterTraits(List<Trait> traits, Pawn pawn) {
#else
		private static List<Trait>? FilterTraits(List<Trait>? traits, Pawn? pawn) {
#endif
#pragma warning restore CA1002
			if (traits is null) {
				return null;
			}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			List<Trait> outValue = new List<Trait>();
#else
			List<Trait> outValue = [];
#endif
			foreach (Trait trait in traits) {
				if (!KnowledgeUtility.IsTraitKnown(pawn, trait.def)) {
					continue;
				}

				outValue.Add(trait);
			}

			return outValue;
		}

		[HarmonyPrefix]
		public static bool Prefix(Pawn? pawn) =>
			KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, pawn);

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
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };
#else
			CodeInstruction[] getPawnInstructions = [new(OpCodes.Ldarg_0)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, _certaintyChangePerDayMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, 0f)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, _certaintyMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, 1f)) {
						yield return i;
					}

					continue;
				}

				// `GetStatValue` is called only to get the pawn's certainty loss factor.
				if (PatchUtility.Calls(instruction, _getStatValueMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, 1f)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, _allTraitsField)) {
					foreach (CodeInstruction i in getPawnInstructions) {
						yield return new CodeInstruction(i);
					}

					yield return new CodeInstruction(OpCodes.Call, _filterTraitsMethod);
					continue;
				}
			}
		}
	}
}
#endif
