using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

#if !(V1_0 || V1_1 || V1_2 || V1_3)
using RimWorld;
#endif

using Verse;
#if V1_0 || V1_1 || V1_2 || V1_3
using Verse.AI;
#endif
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
using Verse.AI.Group;
#endif

namespace Lakuna.BoundedRationality.Patches.PawnPatches {
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.GetInspectString))]
	public static class GetInspectStringPatch {
#if !V1_0
		private static readonly FieldInfo _abilitiesField = AccessTools.Field(typeof(Pawn), nameof(Pawn.abilities));
#endif

		private static readonly FieldInfo _carryTrackerField = AccessTools.Field(typeof(Pawn), nameof(Pawn.carryTracker));

#if V1_0 || V1_1 || V1_2 || V1_3
		private static readonly FieldInfo _curJobField = AccessTools.Field(typeof(Pawn_JobTracker), nameof(Pawn_JobTracker.curJob));
#endif

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		private static readonly FieldInfo _energyField = AccessTools.Field(typeof(Pawn_NeedsTracker), nameof(Pawn_NeedsTracker.energy));
#endif

		private static readonly FieldInfo _equipmentField = AccessTools.Field(typeof(Pawn), nameof(Pawn.equipment));

#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5)
		private static readonly FieldInfo _flightField = AccessTools.Field(typeof(Pawn), nameof(Pawn.flight));
#endif

#if !V1_0
		private static readonly MethodInfo _getInspectStringMethod = AccessTools.Method(typeof(ThingWithComps), nameof(ThingWithComps.GetInspectString)); // Used for stuff like egg progress.
#endif

#if V1_0
		private static readonly MethodInfo _getLordMethod = AccessTools.Method(typeof(LordUtility), nameof(LordUtility.GetLord));
#elif V1_1 || V1_2 || V1_3 || V1_4
		private static readonly MethodInfo _getLordMethod = AccessTools.Method(typeof(LordUtility), nameof(LordUtility.GetLord), new Type[] { typeof(Pawn) });
#endif

#if !V1_0
		private static readonly FieldInfo _guestField = AccessTools.Field(typeof(Pawn), nameof(Pawn.guest));
#endif

		private static readonly FieldInfo _healthField = AccessTools.Field(typeof(Pawn), nameof(Pawn.health));

#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
		private static readonly FieldInfo _hideMainDescField = AccessTools.Field(typeof(ThingDef), nameof(ThingDef.hideMainDesc));
#endif

		private static readonly MethodInfo _inMentalStateMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.InMentalState));

#if V1_0
		private static readonly MethodInfo _inspectStringPartsFromCompsMethod = AccessTools.Method(typeof(ThingWithComps), "InspectStringPartsFromComps");
#endif

		private static readonly MethodInfo _inspiredMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.Inspired));

#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
		private static readonly MethodInfo _isMutantMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.IsMutant));
#endif

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		private static readonly FieldInfo _jobsField = AccessTools.Field(typeof(Pawn), nameof(Pawn.jobs));
#endif

#if !(V1_0 || V1_1 || V1_2)
		private static readonly FieldInfo _ropingField = AccessTools.Field(typeof(Pawn), nameof(Pawn.roping));
#endif

#if !V1_0
		private static readonly FieldInfo _royaltyField = AccessTools.Field(typeof(Pawn), nameof(Pawn.royalty));
#endif

		private static readonly FieldInfo _stancesField = AccessTools.Field(typeof(Pawn), nameof(Pawn.stances));

		private static readonly MethodInfo _traderKindMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.TraderKind));

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

#if V1_0
				if (PatchUtility.Calls(instruction, _inspectStringPartsFromCompsMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator, string.Empty)) {
						yield return i;
					}

					continue;
				}
#else
				if (PatchUtility.Calls(instruction, _getInspectStringMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
				if (PatchUtility.Calls(instruction, _getLordMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#else
				if (PatchUtility.LoadsField(instruction, _hideMainDescField)) {
					foreach (CodeInstruction i in PatchUtility.OrPawnNotKnown(InformationCategory.Basic, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, _isMutantMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Health, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}
#endif

				if (PatchUtility.Calls(instruction, _traderKindMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, _inMentalStateMethod) || PatchUtility.Calls(instruction, _inspiredMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Needs, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, _healthField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Health, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, _stancesField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, _equipmentField) || PatchUtility.LoadsField(instruction, _carryTrackerField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Gear, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

#if V1_0 || V1_1 || V1_2 || V1_3
				if (PatchUtility.LoadsField(instruction, _curJobField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#else

				if (PatchUtility.LoadsField(instruction, _jobsField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, _energyField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Needs, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#endif

#if !(V1_0 || V1_1 || V1_2)
				if (PatchUtility.LoadsField(instruction, _ropingField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#endif

#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5)
				if (PatchUtility.LoadsField(instruction, _flightField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#endif

#if !V1_0
				if (PatchUtility.LoadsField(instruction, _royaltyField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Personal, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				// `guest` is used here only for unwaveringly loyal.
				if (PatchUtility.LoadsField(instruction, _guestField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, _abilitiesField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Abilities, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#endif
			}
		}
	}
}
