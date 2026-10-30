#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.CharacterCardUtilityPatches {
	[HarmonyPatch(typeof(CharacterCardUtility), "DoTopStack")]
	public static class DoTopStackPatch {
		private static readonly FieldInfo GenesField = AccessTools.Field(typeof(Pawn), nameof(Pawn.genes));

		private static readonly FieldInfo FactionField = AccessTools.Field(typeof(ExtraFaction), nameof(ExtraFaction.faction));

		private static readonly FieldInfo RoyaltyField = AccessTools.Field(typeof(Pawn), nameof(Pawn.royalty));

		private static readonly FieldInfo StoryField = AccessTools.Field(typeof(Pawn), nameof(Pawn.story));

		private static readonly FieldInfo GuestField = AccessTools.Field(typeof(Pawn), nameof(Pawn.guest));

		private static readonly MethodInfo FactionMethod = PatchUtility.PropertyGetter(typeof(Thing), nameof(Thing.Faction));

		private static readonly MethodInfo IdeoMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.Ideo));

		[HarmonyPrefix]
		public static bool Prefix(Pawn pawn, ref bool creationMode) {
			bool basic = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn);
			creationMode = creationMode && basic;
			return basic
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Personal, pawn)
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, pawn);
		}

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				// `story` is used only for favorite color in this method.
				if (PatchUtility.LoadsField(instruction, GenesField) || PatchUtility.LoadsField(instruction, RoyaltyField) || PatchUtility.LoadsField(instruction, StoryField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Personal, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				// `guest` is used only for unwaveringly loyal status in this method.
				if (PatchUtility.LoadsField(instruction, GuestField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				// `pawn.Faction == tmpExtraFaction.faction` will always be `true` since both sides will be `null`, causing extra factions to be skipped.
				if (PatchUtility.LoadsField(instruction, FactionField) || PatchUtility.Calls(instruction, FactionMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, IdeoMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Ideoligion, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
#endif
