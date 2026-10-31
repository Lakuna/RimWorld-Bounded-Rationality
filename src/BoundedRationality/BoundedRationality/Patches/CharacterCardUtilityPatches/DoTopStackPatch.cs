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
		private static readonly FieldInfo _factionField = AccessTools.Field(typeof(ExtraFaction), nameof(ExtraFaction.faction));

		private static readonly MethodInfo _factionMethod = PatchUtility.PropertyGetter(typeof(Thing), nameof(Thing.Faction));

		private static readonly FieldInfo _genesField = AccessTools.Field(typeof(Pawn), nameof(Pawn.genes));

		private static readonly FieldInfo _guestField = AccessTools.Field(typeof(Pawn), nameof(Pawn.guest));

		private static readonly MethodInfo _ideoMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.Ideo));

		private static readonly FieldInfo _royaltyField = AccessTools.Field(typeof(Pawn), nameof(Pawn.royalty));

		private static readonly FieldInfo _storyField = AccessTools.Field(typeof(Pawn), nameof(Pawn.story));

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

				// `story` is used only for favorite color in this method.
				if (PatchUtility.LoadsField(instruction, _genesField) || PatchUtility.LoadsField(instruction, _royaltyField) || PatchUtility.LoadsField(instruction, _storyField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Personal, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				// `guest` is used only for unwaveringly loyal status in this method.
				if (PatchUtility.LoadsField(instruction, _guestField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				// `pawn.Faction == tmpExtraFaction.faction` will always be `true` since both sides will be `null`, causing extra factions to be skipped.
				if (PatchUtility.LoadsField(instruction, _factionField) || PatchUtility.Calls(instruction, _factionMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, _ideoMethod)) {
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
