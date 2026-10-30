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
		private static readonly MethodInfo CertaintyChangePerDayMethod = PatchUtility.PropertyGetter(typeof(Pawn_IdeoTracker), nameof(Pawn_IdeoTracker.CertaintyChangePerDay));

		private static readonly MethodInfo CertaintyMethod = PatchUtility.PropertyGetter(typeof(Pawn_IdeoTracker), nameof(Pawn_IdeoTracker.Certainty));

#if V1_3
		private static readonly MethodInfo GetStatValueMethod = SymbolExtensions.GetMethodInfo((Pawn pawn) => pawn.GetStatValue(StatDefOf.CertaintyLossFactor, true));
#else
		private static readonly MethodInfo GetStatValueMethod = SymbolExtensions.GetMethodInfo((Pawn pawn) => pawn.GetStatValue(StatDefOf.CertaintyLossFactor, true, -1));
#endif

		private static readonly FieldInfo AllTraitsField = AccessTools.Field(typeof(TraitSet), nameof(TraitSet.allTraits));

		private static readonly MethodInfo FilterTraitsMethod = AccessTools.Method(typeof(DrawPawnCertaintyPatch), nameof(FilterTraits));

		private static List<Trait> FilterTraits(List<Trait> traits, Pawn pawn) {
			List<Trait> outValue = new List<Trait>();
			foreach (Trait trait in traits) {
				if (!KnowledgeUtility.IsTraitKnown(pawn, trait.def)) {
					continue;
				}

				outValue.Add(trait);
			}

			return outValue;
		}

		[HarmonyPrefix]
		private static bool Prefix(Pawn pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, pawn);

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, CertaintyChangePerDayMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, 0f)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, CertaintyMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, 1f)) {
						yield return i;
					}

					continue;
				}

				// `GetStatValue` is called only to get the pawn's certainty loss factor.
				if (PatchUtility.Calls(instruction, GetStatValueMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, 1f)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, AllTraitsField)) {
					foreach (CodeInstruction i in getPawnInstructions) {
						yield return new CodeInstruction(i);
					}

					yield return new CodeInstruction(OpCodes.Call, FilterTraitsMethod);
					continue;
				}
			}
		}
	}
}
#endif
