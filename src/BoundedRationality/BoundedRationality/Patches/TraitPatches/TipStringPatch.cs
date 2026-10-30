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

namespace Lakuna.BoundedRationality.Patches.TraitPatches {
	[HarmonyPatch(typeof(Trait), nameof(Trait.TipString))]
	public static class TipStringPatch {
#if !(V1_0 || V1_1 || V1_2)
		private static readonly MethodInfo GetAffectedIssuesMethod = AccessTools.Method(typeof(TraitDegreeData), nameof(TraitDegreeData.GetAffectedIssues));

		private static readonly ConstructorInfo IssueDefListConstructor = AccessTools.Constructor(typeof(List<IssueDef>));

		private static readonly MethodInfo GetAffectedMemesMethod = AccessTools.Method(typeof(TraitDegreeData), nameof(TraitDegreeData.GetAffectedMemes));

		private static readonly ConstructorInfo MemeDefListConstructor = AccessTools.Constructor(typeof(List<MemeDef>));

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		private static readonly FieldInfo SourceGeneField = AccessTools.Field(typeof(Trait), nameof(Trait.sourceGene));

		private static readonly MethodInfo SuppressedMethod = PatchUtility.PropertyGetter(typeof(Trait), nameof(Trait.Suppressed));
#endif

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_1) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, GetAffectedIssuesMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Ideoligion, getPawnInstructions, generator, IssueDefListConstructor)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, GetAffectedMemesMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Ideoligion, getPawnInstructions, generator, MemeDefListConstructor)) {
						yield return i;
					}

					continue;
				}

#if !(V1_0 || V1_1 || V1_2 || V1_3)
				if (PatchUtility.LoadsField(instruction, SourceGeneField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Personal, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, SuppressedMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Personal, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}
#endif
			}
		}
#endif

		[HarmonyPostfix]
		private static void Postfix(
#if V1_0 || V1_1
			Pawn pawn,
#endif
#pragma warning disable CA1707
			Trait __instance,
			ref string __result
#pragma warning restore CA1707
		) {
			if (
#if V1_0 || V1_1
				KnowledgeUtility.IsTraitKnown(pawn, __instance.def)
#else
				KnowledgeUtility.IsTraitKnown(__instance)
#endif
			) {
				return;
			}

			__result = MiscellaneousUtility.EndWithPeriod("BR.Unknown".Translate().CapitalizeFirst());
		}
	}
}
