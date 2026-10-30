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
		private static readonly FieldInfo HealthField = AccessTools.Field(typeof(Pawn), nameof(Pawn.health));

		private static readonly FieldInfo StancesField = AccessTools.Field(typeof(Pawn), nameof(Pawn.stances));

		private static readonly FieldInfo EquipmentField = AccessTools.Field(typeof(Pawn), nameof(Pawn.equipment));

		private static readonly FieldInfo CarryTrackerField = AccessTools.Field(typeof(Pawn), nameof(Pawn.carryTracker));

#if V1_0 || V1_1 || V1_2 || V1_3
		private static readonly FieldInfo CurJobField = AccessTools.Field(typeof(Pawn_JobTracker), nameof(Pawn_JobTracker.curJob));
#else
		private static readonly FieldInfo JobsField = AccessTools.Field(typeof(Pawn), nameof(Pawn.jobs));

		private static readonly FieldInfo EnergyField = AccessTools.Field(typeof(Pawn_NeedsTracker), nameof(Pawn_NeedsTracker.energy));
#endif

#if !(V1_0 || V1_1 || V1_2)
		private static readonly FieldInfo RopingField = AccessTools.Field(typeof(Pawn), nameof(Pawn.roping));
#endif

#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5)
		private static readonly FieldInfo FlightField = AccessTools.Field(typeof(Pawn), nameof(Pawn.flight));
#endif

#if !V1_0
		private static readonly FieldInfo RoyaltyField = AccessTools.Field(typeof(Pawn), nameof(Pawn.royalty));

		private static readonly FieldInfo AbilitiesField = AccessTools.Field(typeof(Pawn), nameof(Pawn.abilities));

		private static readonly FieldInfo GuestField = AccessTools.Field(typeof(Pawn), nameof(Pawn.guest));
#endif

		private static readonly MethodInfo TraderKindMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.TraderKind));

		private static readonly MethodInfo InMentalStateMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.InMentalState));

		private static readonly MethodInfo InspiredMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.Inspired));

#if V1_0
		private static readonly MethodInfo InspectStringPartsFromCompsMethod = AccessTools.Method(typeof(ThingWithComps), "InspectStringPartsFromComps");
#else
		private static readonly MethodInfo GetInspectStringMethod = AccessTools.Method(typeof(ThingWithComps), nameof(ThingWithComps.GetInspectString)); // Used for stuff like egg progress.
#endif

#if V1_0
		private static readonly MethodInfo GetLordMethod = AccessTools.Method(typeof(LordUtility), nameof(LordUtility.GetLord));
#elif V1_1 || V1_2 || V1_3 || V1_4
		private static readonly MethodInfo GetLordMethod = AccessTools.Method(typeof(LordUtility), nameof(LordUtility.GetLord), new Type[] { typeof(Pawn) });
#else
		private static readonly FieldInfo HideMainDescField = AccessTools.Field(typeof(ThingDef), nameof(ThingDef.hideMainDesc));

		private static readonly MethodInfo IsMutantMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.IsMutant));
#endif

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

#if V1_0
				if (PatchUtility.Calls(instruction, InspectStringPartsFromCompsMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator, string.Empty)) {
						yield return i;
					}

					continue;
				}
#else
				if (PatchUtility.Calls(instruction, GetInspectStringMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
				if (PatchUtility.Calls(instruction, GetLordMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#else
				if (PatchUtility.LoadsField(instruction, HideMainDescField)) {
					foreach (CodeInstruction i in PatchUtility.OrPawnNotKnown(InformationCategory.Basic, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, IsMutantMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Health, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}
#endif

				if (PatchUtility.Calls(instruction, TraderKindMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, InMentalStateMethod) || PatchUtility.Calls(instruction, InspiredMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Needs, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, HealthField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Health, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, StancesField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, EquipmentField) || PatchUtility.LoadsField(instruction, CarryTrackerField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Gear, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

#if V1_0 || V1_1 || V1_2 || V1_3
				if (PatchUtility.LoadsField(instruction, CurJobField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#else

				if (PatchUtility.LoadsField(instruction, JobsField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, EnergyField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Needs, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#endif

#if !(V1_0 || V1_1 || V1_2)
				if (PatchUtility.LoadsField(instruction, RopingField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#endif

#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5)
				if (PatchUtility.LoadsField(instruction, FlightField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#endif

#if !V1_0
				if (PatchUtility.LoadsField(instruction, RoyaltyField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Personal, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				// `guest` is used here only for unwaveringly loyal.
				if (PatchUtility.LoadsField(instruction, GuestField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, AbilitiesField)) {
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
