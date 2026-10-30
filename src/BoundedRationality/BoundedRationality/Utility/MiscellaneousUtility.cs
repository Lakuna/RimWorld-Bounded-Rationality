using System;
using System.Linq;
#if V1_0 || V1_1 || V1_2 || V1_3
using System.Reflection;
#endif
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
using System.Text;
#endif

#if V1_0
using Harmony;
#elif V1_1 || V1_2 || V1_3
using HarmonyLib;
#endif

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Utility {
	/// <summary>
	/// Miscellaneous utility methods.
	/// </summary>
	public static class MiscellaneousUtility {
#if V1_0 || V1_1 || V1_2 || V1_3
		private static readonly FieldInfo _pawnField = AccessTools.Field(typeof(SkillRecord), "pawn");
#endif

		/// <summary>
		/// Split up a long string into lines. Almost equivalent to `str.AddLineBreaksToLongString`.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <returns>The split-up string.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
		public static string AddLineBreaksToLongString(string str) {
			StringBuilder stringBuilder = new StringBuilder(str.Length + (str.Length / 100 + 3) * 2 + 1);
			stringBuilder = stringBuilder.AppendLine();
			for (int i = 0; i < str.Length; i++) {
				if (i % 100 == 0 && i != 0) {
					stringBuilder = stringBuilder.AppendLine();
				}

				stringBuilder = stringBuilder.Append(str[i]);
			}

			stringBuilder = stringBuilder.AppendLine();
			return stringBuilder.ToString();
		}
#else
		public static string AddLineBreaksToLongString(string str) =>
			str.AddLineBreaksToLongString() ?? "null";
#endif

		/// <summary>
		/// Determine whether the given pawn is dead or downed. Equivalent to `pawn.DeadOrDowned` for all versions of RimWorld.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <returns>Whether or not the pawn is dead or downed.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
		public static bool DeadOrDowned(Pawn pawn) =>
			pawn != null && (pawn.Dead || pawn.Downed);
#elif V1_5
		public static bool DeadOrDowned(Pawn pawn) =>
			pawn?.DeadOrDowned ?? false;
#else
		public static bool DeadOrDowned(Pawn? pawn) =>
			pawn?.DeadOrDowned ?? false;
#endif

		/// <summary>
		/// Make an empty array of objects.
		/// </summary>
		/// <returns>The array.</returns>
		public static object[] EmptyArray() =>
			EmptyArray<object>();

		/// <summary>
		/// Make an empty array of the given type.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <returns>The array.</returns>
		public static T[] EmptyArray<T>() =>
#if V1_0
			new T[] { };
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			Array.Empty<T>();
#else
			[];
#endif

		/// <summary>
		/// Equivalent to `enumType.GetEnumUnderlyingType` for all runtimes.
		/// </summary>
		/// <param name="enumType">The enumeration type.</param>
		/// <returns>The enumeration underlying type.</returns>
#if V1_0
		public static Type GetEnumUnderlyingType(Type enumType) {
			if (enumType is null) {
				throw new ArgumentNullException(nameof(enumType));
			}

			if (!enumType.IsEnum) {
				throw new ArgumentException("Type provided must be an Enum.", nameof(enumType));
			}

			FieldInfo[] fields = enumType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (fields is null || fields.Length != 1) {
				throw new ArgumentException("The Enum type should contain one and only one instance field.", nameof(enumType));
			}

			return fields[0].FieldType;
		}
#else
		public static Type GetEnumUnderlyingType(Type enumType) =>
			enumType is null ? throw new ArgumentNullException(nameof(enumType))
			: enumType.GetEnumUnderlyingType();
#endif

		/// <summary>
		/// Get the underlying type for the given enumeration.
		/// </summary>
		/// <param name="value">The enumeration.</param>
		/// <returns>The underlying type.</returns>
		public static Type GetEnumUnderlyingType(Enum value) =>
			value is null ? throw new ArgumentNullException(nameof(value))
			: GetEnumUnderlyingType(value.GetType());

		/// <summary>
		/// Check whether the given pawn is hostile to the player's faction.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <returns>Whether the given pawn is hostile to the player's faction.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static bool HostileToPlayer(Pawn pawn) =>
			pawn != null
			&& Faction.OfPlayerSilentFail != null
			&& ((pawn.Faction is null) ? pawn.HostileTo(Faction.OfPlayerSilentFail)
				: (pawn.Faction != Faction.OfPlayerSilentFail
					&& pawn.Faction.RelationWith(Faction.OfPlayerSilentFail, true) != null
					&& (pawn.HostileTo(Faction.OfPlayerSilentFail)
						|| DeadOrDowned(pawn)
						&& HostileToPlayer(pawn.Faction))));
#else
		public static bool HostileToPlayer(Pawn? pawn) =>
			pawn is not null
			&& Faction.OfPlayerSilentFail is not null
			&& ((pawn.Faction is null) ? pawn.HostileTo(Faction.OfPlayerSilentFail)
				: (pawn.Faction != Faction.OfPlayerSilentFail
					&& pawn.Faction.RelationWith(Faction.OfPlayerSilentFail, true) is not null
					&& (pawn.HostileTo(Faction.OfPlayerSilentFail)
						|| DeadOrDowned(pawn)
						&& HostileToPlayer(pawn.Faction))));
#endif

		/// <summary>
		/// Check whether the given faction is hostile to the player's faction.
		/// </summary>
		/// <param name="faction">The faction.</param>
		/// <returns>Whether the given faction is hostile to the player's faction.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static bool HostileToPlayer(Faction faction) =>
			faction != null
			&& Faction.OfPlayerSilentFail != null
			&& faction != Faction.OfPlayerSilentFail
			&& faction.RelationWith(Faction.OfPlayerSilentFail, true) != null
			&& faction.HostileTo(Faction.OfPlayerSilentFail);
#else
		public static bool HostileToPlayer(Faction? faction) =>
			faction is not null
			&& Faction.OfPlayerSilentFail is not null
			&& faction != Faction.OfPlayerSilentFail
			&& faction.RelationWith(Faction.OfPlayerSilentFail, true) is not null
			&& faction.HostileTo(Faction.OfPlayerSilentFail);
#endif

		/// <summary>
		/// Equivalent to `Pawn.IsAnimal` for all versions of RimWorld.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <returns>`Pawn.IsAnimal`.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
		public static bool IsAnimal(Pawn pawn) =>
			pawn?.RaceProps?.Animal ?? false;
#elif V1_6
		public static bool IsAnimal(Pawn pawn) =>
			pawn?.IsAnimal ?? false;
#else
		public static bool IsAnimal(Pawn? pawn) =>
			pawn?.IsAnimal ?? false;
#endif

		/// <summary>
		/// Equivalent to `Pawn.IsFreeNonSlaveColonist` for all versions of RimWorld.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <returns>`Pawn.IsFreeNonSlaveColonist`.</returns>
#if V1_0 || V1_1 || V1_2
		public static bool IsFreeNonSlaveColonist(Pawn pawn) =>
			pawn?.IsFreeColonist ?? false;
#elif V1_3 || V1_4 || V1_5 || V1_6
		public static bool IsFreeNonSlaveColonist(Pawn pawn) =>
			pawn?.IsFreeNonSlaveColonist ?? false;
#else
		public static bool IsFreeNonSlaveColonist(Pawn? pawn) =>
			pawn?.IsFreeNonSlaveColonist ?? false;
#endif

		/// <summary>
		/// Determine whether or not a growth moment dialog is currently open.
		/// </summary>
		/// <returns>Whether or not a growth moment dialog is currently open.</returns>
		public static bool IsInGrowthMoment() =>
#if V1_0 || V1_1 || V1_2 || V1_3
			false;
#elif V1_4 || V1_5 || V1_6
			Find.WindowStack?.WindowOfType<Dialog_GrowthMomentChoices>() != null;
#else

			Find.WindowStack?.WindowOfType<Dialog_GrowthMomentChoices>() is not null;
#endif

		/// <summary>
		/// Save a Boolean array. Almost equivalent to `DataExposeUtility.LookBoolArray`.
		/// </summary>
		/// <param name="arr">The array to save.</param>
		/// <param name="elements">The number of elements in the array.</param>
		/// <param name="label">The name of the array in the save data.</param>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
		public static void LookBoolArray(ref bool[] arr, int elements, string label) {
			if (Scribe.mode == LoadSaveMode.Saving && arr != null) {
				if (arr.Length != elements) {
					Log.ErrorOnce($"Bool array length mismatch for {label}", 74135877);
				}

				elements = arr.Length;
			}

			int numBytes = (elements + 7) / 8;

			// Equivalent to `byte[] arr2 = DataExposeUtility.BytesToBits(arr, elements, numBytes);`.
			byte[] arr2 = null;
			if (Scribe.mode == LoadSaveMode.Saving && arr != null) {
				arr2 = new byte[numBytes];
				int num = 0;
				byte b = 1;
				for (int i = 0; i < elements; i++) {
					if (arr[i]) {
						arr2[num] |= b;
					}

					b *= 2;
					if (b == 0) {
						b = 1;
						num++;
					}
				}
			}

			// Equivalent to `DataExposeUtility.LookByteArray(ref arr2, label);`.
			if (Scribe.mode == LoadSaveMode.Saving && arr2 != null) {
				byte[] array = CompressUtility.Compress(arr2);
				if (array.Length < arr2.Length) {
					string value = AddLineBreaksToLongString(Convert.ToBase64String(array));
					Scribe_Values.Look(ref value, $"{label}Deflate");
				} else {
					string value = AddLineBreaksToLongString(Convert.ToBase64String(arr2));
					Scribe_Values.Look(ref value, label);
				}
			} else if (Scribe.mode == LoadSaveMode.LoadingVars) {
				string value = null;
				Scribe_Values.Look(ref value, $"{label}Deflate");
				if (value is null) {
					Scribe_Values.Look(ref value, label);
					arr2 = value is null ? null : Convert.FromBase64String(RemoveLineBreaks(value));
				} else {
					arr2 = CompressUtility.Decompress(Convert.FromBase64String(RemoveLineBreaks(value)));
				}
			}

			// Equivalent to `DataExposeUtility.BitsToBytes(ref arr, elements, arr2, numBytes);`.
			if (Scribe.mode != LoadSaveMode.LoadingVars || arr2 is null || arr2.Length == 0) {
				return;
			}
			if (arr is null) {
				arr = new bool[elements];
			}
			if (arr2.Length != numBytes) {
				int num = 0;
				byte b = 1;
				for (int i = 0; i < elements; i++) {
					arr[i] = (arr2[num] & b) != 0;
					b *= 2;
					if (b > 32) {
						b = 1;
						num++;
					}
				}

				return;
			}
			int num2 = 0;
			byte b2 = 1;
			for (int i = 0; i < elements; i++) {
				arr[i] = (arr2[num2] & b2) != 0;
				b2 *= 2;
				if (b2 == 0) {
					b2 = 1;
					num2++;
				}
			}
		}
#else
		public static void LookBoolArray(ref bool[]? arr, int elements, string label) =>
			DataExposeUtility.LookBoolArray(ref arr, elements, label);
#endif

		/// <summary>
		/// Equivalent to `Pawn.IsPlayerControlled` for all versions of RimWorld. For checking whether or not a pawn belongs to a player-controlled pawn type, use `MiscellaneousUtility.IsPlayerControlled` instead.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <returns>`Pawn.IsPlayerControlled`.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3
		public static bool PawnIsPlayerControlled(Pawn pawn) =>
			pawn?.IsColonistPlayerControlled ?? false;
#elif V1_4
		public static bool PawnIsPlayerControlled(Pawn pawn) =>
			(pawn?.IsColonistPlayerControlled ?? false)
			|| (pawn?.IsColonyMechPlayerControlled ?? false);
#elif V1_5
		public static bool PawnIsPlayerControlled(Pawn pawn) =>
			pawn?.IsPlayerControlled ?? false;
#else
		public static bool PawnIsPlayerControlled(Pawn? pawn) =>
			pawn?.IsPlayerControlled ?? false;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3
		/// <summary>
		/// Get the pawn that is associated with the given skill record.
		/// </summary>
		/// <param name="skill">The skill record.</param>
		/// <returns>The pawn that is associated with the given skill record.</returns>
		public static Pawn PawnOfSkillRecord(SkillRecord skill) =>
			_pawnField.GetValue(skill) as Pawn;
#elif V1_4 || V1_5 || V1_6
		/// <summary>
		/// Get the pawn that is associated with the given skill record.
		/// </summary>
		/// <param name="skill">The skill record.</param>
		/// <returns>The pawn that is associated with the given skill record.</returns>
		public static Pawn PawnOfSkillRecord(SkillRecord skill) =>
			skill?.Pawn;
#else
		/// <summary>
		/// Get the pawn that is associated with the given skill record.
		/// </summary>
		/// <param name="skill">The skill record.</param>
		/// <returns>The pawn that is associated with the given skill record.</returns>
		public static Pawn? PawnOfSkillRecord(SkillRecord? skill) =>
			skill?.Pawn;
#endif

		/// <summary>
		/// Compress a long string into one line. Equivalent to `str.RemoveLineBreaks`.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <returns>The compressed string.</returns>
		public static string RemoveLineBreaks(string str) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
			new StringBuilder(str).Replace("\n", string.Empty).Replace("\r", string.Empty).ToString();
#else
			str.RemoveLineBreaks() ?? "null";
#endif

		/// <summary>
		/// Get the type of the given pawn.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <returns>The type of the pawn.</returns>
#if V1_0 || V1_1 || V1_2
		public static PawnType TypeOf(Pawn pawn) =>
			(pawn is null) ? PawnType.Neutral
			: ((IsFreeNonSlaveColonist(pawn)
					|| IsAnimal(pawn)
					&& Faction.OfPlayerSilentFail != null
					&& pawn.Faction == Faction.OfPlayerSilentFail)
				&& pawn.Faction == Faction.OfPlayerSilentFail
				|| (BoundedRationalityMod.Settings?.RememberFormerColonists ?? false)
				&& WasColonist(pawn)) ? PawnType.Colonist
			: PawnIsPlayerControlled(pawn) ? PawnType.Controlled
			: pawn.IsPrisonerOfColony ? PawnType.Prisoner
			: IsAnimal(pawn) ? PawnType.WildAnimal
			: HostileToPlayer(pawn) ? PawnType.Hostile
			: PawnType.Neutral;
#elif V1_3 || V1_4 || V1_5 || V1_6
		public static PawnType TypeOf(Pawn pawn) =>
			(pawn is null) ? PawnType.Neutral
			: ((IsFreeNonSlaveColonist(pawn)
					|| IsAnimal(pawn)
					&& Faction.OfPlayerSilentFail != null
					&& pawn.Faction == Faction.OfPlayerSilentFail)
				&& pawn.HomeFaction == Faction.OfPlayerSilentFail
				|| (BoundedRationalityMod.Settings?.RememberFormerColonists ?? false)
				&& WasColonist(pawn)) ? PawnType.Colonist
			: pawn.IsSlaveOfColony ? PawnType.Slave
			: PawnIsPlayerControlled(pawn) ? PawnType.Controlled
			: pawn.IsPrisonerOfColony ? PawnType.Prisoner
			: IsAnimal(pawn) ? PawnType.WildAnimal
			: HostileToPlayer(pawn) ? PawnType.Hostile
			: PawnType.Neutral;
#else
		public static PawnType TypeOf(Pawn? pawn) =>
			(pawn is null) ? PawnType.Neutral
			: ((IsFreeNonSlaveColonist(pawn)
					|| IsAnimal(pawn)
					&& Faction.OfPlayerSilentFail is not null
					&& pawn.Faction == Faction.OfPlayerSilentFail)
				&& pawn.HomeFaction == Faction.OfPlayerSilentFail
				|| (BoundedRationalityMod.Settings?.RememberFormerColonists ?? false)
				&& WasColonist(pawn)) ? PawnType.Colonist
			: pawn.IsSlaveOfColony ? PawnType.Slave
			: PawnIsPlayerControlled(pawn) ? PawnType.Controlled
			: pawn.IsPrisonerOfColony ? PawnType.Prisoner
			: IsAnimal(pawn) ? PawnType.WildAnimal
			: HostileToPlayer(pawn) ? PawnType.Hostile
			: PawnType.Neutral;
#endif

		/// <summary>
		/// Get the type of the given faction.
		/// </summary>
		/// <param name="faction">The faction.</param>
		/// <returns>The type of the faction.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static PawnType TypeOf(Faction faction) =>
#else
		public static PawnType TypeOf(Faction? faction) =>
#endif
			(faction is null) ? PawnType.Neutral
			: (faction == Faction.OfPlayerSilentFail) ? PawnType.Colonist
			: HostileToPlayer(faction) ? PawnType.Hostile
			: PawnType.Neutral;

		/// <summary>
		/// Determines whether the given pawn is one that can be controlled by the player.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <returns>Whether the pawn is player-controlled.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static bool IsPlayerControlled(Pawn pawn) =>
			pawn != null
			&& IsPlayerControlled(TypeOf(pawn), !pawn.Dead);
#else
		public static bool IsPlayerControlled(Pawn? pawn) =>
			pawn is not null
			&& IsPlayerControlled(TypeOf(pawn), !pawn.Dead);
#endif

		/// <summary>
		/// Determines whether the given faction is one that can be controlled by the player.
		/// </summary>
		/// <param name="faction">The faction.</param>
		/// <returns>Whether the faction is player-controlled.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static bool IsPlayerControlled(Faction faction) =>
			faction != null
			&& IsPlayerControlled(TypeOf(faction));
#else
		public static bool IsPlayerControlled(Faction? faction) =>
			faction is not null
			&& IsPlayerControlled(TypeOf(faction));
#endif

		/// <summary>
		/// Determines whether the given pawn type is one that can be controlled by the player.
		/// </summary>
		/// <param name="type">The pawn type.</param>
		/// <param name="isAlive">Whether the pawn is alive.</param>
		/// <returns>Whether the pawn type is player-controlled.</returns>
		public static bool IsPlayerControlled(PawnType type, bool isAlive = true) =>
			(type == PawnType.Colonist
				|| type == PawnType.Controlled
				|| type == PawnType.Slave)
			&& isAlive;

		/// <summary>
		/// Determine whether or not the given pawn is a starting colonist. This can only be true while selecting starting colonists.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <returns>Whether or not the given pawn is a starting colonist.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static bool IsStartingColonist(Pawn pawn) =>
			pawn != null
			&& (Find.GameInitData?.startingAndOptionalPawns?.Contains(pawn) ?? false);
#else
		public static bool IsStartingColonist(Pawn? pawn) =>
			pawn is not null
			&& (Find.GameInitData?.startingAndOptionalPawns?.Contains(pawn) ?? false);
#endif

		/// <summary>
		/// Determine whether or not the given pawn is related to any colonist.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <returns>Whether or not the given pawn is related to any colonist.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static bool IsRelativeOfColonist(Pawn pawn) =>
#else
		public static bool IsRelativeOfColonist(Pawn? pawn) =>
#endif
			pawn?.relations?.RelatedPawns?.Any((other) => TypeOf(other) == PawnType.Colonist) ?? false;

		/// <summary>
		/// Determine whether or not the given pawn's corpse was dead when the player discovered it.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <returns>Whether or not the given pawn's corpse was dead when the player discovered it.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static bool IsAncientCorpse(Pawn pawn) =>
			pawn != null
			&& pawn.Dead
			&& IsAncientCorpse(pawn.Corpse);
#else
		public static bool IsAncientCorpse(Pawn? pawn) =>
			pawn is not null
			&& pawn.Dead
			&& IsAncientCorpse(pawn.Corpse);
#endif

		/// <summary>
		/// Determine whether or not the given corpse was dead when the player discovered it.
		/// </summary>
		/// <param name="corpse">The corpse.</param>
		/// <returns>Whether or not the given corpse was dead when the player discovered it.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static bool IsAncientCorpse(Corpse corpse) =>
			corpse != null
			&& corpse.timeOfDeath <= 0;
#else
		public static bool IsAncientCorpse(Corpse? corpse) =>
			corpse is not null
			&& corpse.timeOfDeath <= 0;
#endif

		/// <summary>
		/// Determine the number of ticks that the given pawn has spent as a colonist or colony animal.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <returns>The number of ticks that the given pawn has spent as a colonist or colony animal.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static float TimeAsColonist(Pawn pawn) =>
#else
		public static float TimeAsColonist(Pawn? pawn) =>
#endif
			pawn?.records?.GetValue(RecordDefOf.TimeAsColonistOrColonyAnimal) ?? 0;

		/// <summary>
		/// Determine the number of ticks that the given pawn has spent as either a colonist, colony animal, or prisoner.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <returns>The number of ticks that the given pawn has spent as either a colonist, colony animal, or prisoner.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static float TimeAsColonistOrPrisoner(Pawn pawn) =>
#else
		public static float TimeAsColonistOrPrisoner(Pawn? pawn) =>
#endif
			TimeAsColonist(pawn) + TimeAsPrisoner(pawn);

		/// <summary>
		/// Determine the number of ticks that the given pawn has spent as a prisoner.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <returns>The number of ticks that the given pawn has spent as a prisoner.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static float TimeAsPrisoner(Pawn pawn) =>
#else
		public static float TimeAsPrisoner(Pawn? pawn) =>
#endif
			pawn?.records?.GetValue(RecordDefOf.TimeAsPrisoner) ?? 0;

		/// <summary>
		/// Determine whether or not the given pawn is or has ever been a colonist.
		/// </summary>
		/// <param name="pawn">The pawn to check.</param>
		/// <returns>Whether or not the given pawn is or has ever been a colonist.</returns>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static bool WasColonist(Pawn pawn) =>
#else
		public static bool WasColonist(Pawn? pawn) =>
#endif
			TimeAsColonist(pawn) > 0;
	}
}
