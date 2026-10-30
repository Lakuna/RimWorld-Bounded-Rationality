using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Utility {
	/// <summary>
	/// A static utility class that contains static utility methods for performing common patching operations.
	/// </summary>
	public static class PatchUtility {
		/// <summary>
		/// `KnowledgeUtility.AreAllSkillsForWorkTypeKnown`.
		/// </summary>
		public static readonly MethodInfo AreAllSkillsForWorkTypeKnownMethod = AccessTools.Method(
			typeof(KnowledgeUtility),
			nameof(KnowledgeUtility.AreAllSkillsForWorkTypeKnown));

		/// <summary>
		/// `KnowledgeUtility.IsBackstoryKnown` given a `Backstory` as an argument.
		/// </summary>
		public static readonly MethodInfo IsBackstoryDefKnownMethod = AccessTools.Method(
			typeof(KnowledgeUtility),
			nameof(KnowledgeUtility.IsBackstoryKnown),
#if V1_0 || V1_1 || V1_2 || V1_3
			new Type[] { typeof(Backstory), typeof(Pawn) });
#elif V1_4 || V1_5 || V1_6
			new Type[] { typeof(BackstoryDef), typeof(Pawn) });
#else
			[typeof(BackstoryDef), typeof(Pawn)]);
#endif

		/// <summary>
		/// `KnowledgeUtility.IsInformationKnownFor` given a `Faction` as an argument.
		/// </summary>
		public static readonly MethodInfo IsInformationKnownForFactionMethod = AccessTools.Method(
			typeof(KnowledgeUtility),
			nameof(KnowledgeUtility.IsInformationKnownFor),
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			new Type[] { typeof(InformationCategory), typeof(Faction), typeof(ControlCategory) });
#else
			[typeof(InformationCategory), typeof(Faction), typeof(ControlCategory)]);
#endif

		/// <summary>
		/// `KnowledgeUtility.IsInformationKnownFor` given a `Pawn` as an argument.
		/// </summary>
		public static readonly MethodInfo IsInformationKnownForPawnMethod = AccessTools.Method(
			typeof(KnowledgeUtility),
			nameof(KnowledgeUtility.IsInformationKnownFor),
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			new Type[] { typeof(InformationCategory), typeof(Pawn), typeof(ControlCategory) });
#else
			[typeof(InformationCategory), typeof(Pawn), typeof(ControlCategory)]);
#endif

		/// <summary>
		/// `KnowledgeUtility.IsInformationKnownFor` given a `Thing` as an argument.
		/// </summary>
		public static readonly MethodInfo IsInformationKnownForThingMethod = AccessTools.Method(
			typeof(KnowledgeUtility),
			nameof(KnowledgeUtility.IsInformationKnownFor),
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			new Type[] { typeof(InformationCategory), typeof(Thing), typeof(ControlCategory) });
#else
			[typeof(InformationCategory), typeof(Thing), typeof(ControlCategory)]);
#endif

		/// <summary>
		/// `string.Empty`.
		/// </summary>
		private static readonly FieldInfo _emptyStringField = AccessTools.Field(
			typeof(string),
			nameof(string.Empty));

		/// <summary>
		/// AND the value on top of the stack with whether the given information category is known for the "given" faction.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="getFactionInstructions">The instructions to execute to load the faction onto the stack.</param>
		/// <param name="controlCategory">The type of control or notification that contains the obscured information.</param>
		/// <returns>The instructions that will perform the AND.</returns>
		public static IEnumerable<CodeInstruction> AndFactionKnown(InformationCategory category, IEnumerable<CodeInstruction> getFactionInstructions, ControlCategory controlCategory = ControlCategory.Default) =>
			AndKnown(IsInformationKnownForFactionMethod, category, getFactionInstructions, controlCategory);

		/// <summary>
		/// AND the value on top of the stack with whether the given information category is known for the "given" pawn or faction.
		/// </summary>
		/// <param name="isInformationKnownForMethod">The method to call to check knowledge.</param>
		/// <param name="category">The information category.</param>
		/// <param name="getInstructions">The instructions to execute to load the pawn or faction onto the stack.</param>
		/// <param name="controlCategory">The type of control or notification that contains the obscured information.</param>
		/// <returns>The instructions that will perform the AND.</returns>
		private static IEnumerable<CodeInstruction> AndKnown(MethodInfo isInformationKnownForMethod, InformationCategory category, IEnumerable<CodeInstruction> getInstructions, ControlCategory controlCategory = ControlCategory.Default) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (getInstructions is null) {
				throw new ArgumentNullException(nameof(getInstructions));
			}
#else
			ArgumentNullException.ThrowIfNull(getInstructions);
#endif

			// Load the arguments for `KnowledgeUtility.IsInformationKnownFor` onto the stack.
			yield return LoadValue(category); // `category`.
			foreach (CodeInstruction instruction in getInstructions) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				yield return new CodeInstruction(instruction); // `pawn` or `faction`.
#else
				yield return new(instruction); // `pawn` or `faction`.
#endif
			}
			yield return LoadValue(controlCategory); // `controlCategory`.

			// Call `KnowledgeUtility.IsInformationKnownFor`, leaving the return value on top of the stack.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			yield return new CodeInstruction(OpCodes.Call, isInformationKnownForMethod); // Remove the arguments from the stack and add the return value.
#else
			yield return new(OpCodes.Call, isInformationKnownForMethod); // Remove the arguments from the stack and add the return value.
#endif

			// AND the return value with the value that was originally on top of the stack.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			yield return new CodeInstruction(OpCodes.And);
#else
			yield return new(OpCodes.And);
#endif
		}

		/// <summary>
		/// AND the value on top of the stack with whether the given information category is known for the "given" pawn.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="getPawnInstructions">The instructions to execute to load the pawn onto the stack.</param>
		/// <param name="controlCategory">The type of control or notification that contains the obscured information.</param>
		/// <returns>The instructions that will perform the AND.</returns>
		public static IEnumerable<CodeInstruction> AndPawnKnown(InformationCategory category, IEnumerable<CodeInstruction> getPawnInstructions, ControlCategory controlCategory = ControlCategory.Default) =>
			AndKnown(IsInformationKnownForPawnMethod, category, getPawnInstructions, controlCategory);

		/// <summary>
		/// AND the value on top of the stack with whether the given information category is known for the "given" thing.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="getThingInstructions">The instructions to execute to load the thing onto the stack.</param>
		/// <param name="controlCategory">The type of control or notification that contains the obscured information.</param>
		/// <returns>The instructions that will perform the AND.</returns>
		public static IEnumerable<CodeInstruction> AndThingKnown(InformationCategory category, IEnumerable<CodeInstruction> getThingInstructions, ControlCategory controlCategory = ControlCategory.Default) =>
			AndKnown(IsInformationKnownForThingMethod, category, getThingInstructions, controlCategory);

		/// <summary>
		/// Determine whether the given instruction calls the given method. Only necessary for RimWorld 1.0, since the versions of Harmony for later RimWorld versions have their own built-in method for this.
		/// </summary>
		/// <param name="instruction">The instruction.</param>
		/// <param name="method">The method.</param>
		/// <returns>Whether the given instruction calls the given method.</returns>
		public static bool Calls(CodeInstruction instruction, MethodInfo method) =>
#if V1_0
			(instruction.opcode == OpCodes.Call || instruction.opcode == OpCodes.Callvirt) && instruction.operand == method;
#else
			instruction.Calls(method);
#endif

		/// <summary>
		/// Store the value on top of the stack locally then put its address on the stack in its place.
		/// </summary>
		/// <param name="generator">The code generator.</param>
		/// <param name="value">The value on top of the stack.</param>
		/// <returns></returns>
		private static IEnumerable<CodeInstruction> GetLocalAddress(ILGenerator generator, object? value) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (generator is null) {
				throw new ArgumentNullException(nameof(generator));
			}
#else
			ArgumentNullException.ThrowIfNull(generator);
#endif

			LocalBuilder tempLocal = generator.DeclareLocal(value is null ? typeof(object) : value.GetType());
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			yield return new CodeInstruction(OpCodes.Stloc, tempLocal);
			yield return new CodeInstruction(OpCodes.Ldloca_S, tempLocal);
#else
			yield return new(OpCodes.Stloc, tempLocal);
			yield return new(OpCodes.Ldloca_S, tempLocal);
#endif
		}

		/// <summary>
		/// Determine whether the given instruction loads the given field. Only necessary for RimWorld 1.0, since the versions of Harmony for later RimWorld versions have their own built-in method for this.
		/// </summary>
		/// <param name="instruction">The instruction.</param>
		/// <param name="field">The field.</param>
		/// <param name="byAddress">Whether or not the field is loaded by address.</param>
		/// <returns>Whether the given instruction loads the given field.</returns>
		public static bool LoadsField(CodeInstruction instruction, FieldInfo field, bool byAddress = false) =>
#if V1_0
			(field.IsStatic ?
					(byAddress ? (instruction.opcode == OpCodes.Ldsflda)
					: (instruction.opcode == OpCodes.Ldsfld))
				: (byAddress ? (instruction.opcode == OpCodes.Ldflda)
				: (instruction.opcode == OpCodes.Ldfld)))
			&& instruction.operand == field;
#else
			instruction.LoadsField(field, byAddress);
#endif

		/// <summary>
		/// Load the given value onto the stack.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="byAddress">Whether or not to load the value by address. Only applicable to fields.</param>
		/// <returns>The instruction that will load the given value onto the stack.</returns>
		public static CodeInstruction LoadValue(object? value, bool byAddress = false) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			value is bool boolValue ? LoadValue(boolValue ? 1 : 0, byAddress)
			: value is byte byteValue ? LoadValue(unchecked((int)byteValue), byAddress)
			: value is sbyte sbyteValue ? new CodeInstruction(OpCodes.Ldc_I4_S, sbyteValue)
			: value is char charValue ? LoadValue(unchecked((int)charValue), byAddress)
			: value is double doubleValue ? new CodeInstruction(OpCodes.Ldc_R8, doubleValue)
			: value is float floatValue ? new CodeInstruction(OpCodes.Ldc_R4, floatValue)
			: value is int intValue ?
				intValue == -1 ? new CodeInstruction(OpCodes.Ldc_I4_M1)
				: intValue == 0 ? new CodeInstruction(OpCodes.Ldc_I4_0)
				: intValue == 1 ? new CodeInstruction(OpCodes.Ldc_I4_1)
				: intValue == 2 ? new CodeInstruction(OpCodes.Ldc_I4_2)
				: intValue == 3 ? new CodeInstruction(OpCodes.Ldc_I4_3)
				: intValue == 4 ? new CodeInstruction(OpCodes.Ldc_I4_4)
				: intValue == 5 ? new CodeInstruction(OpCodes.Ldc_I4_5)
				: intValue == 6 ? new CodeInstruction(OpCodes.Ldc_I4_6)
				: intValue == 7 ? new CodeInstruction(OpCodes.Ldc_I4_7)
				: intValue == 8 ? new CodeInstruction(OpCodes.Ldc_I4_8)
				: new CodeInstruction(OpCodes.Ldc_I4, intValue)
			: value is uint uintValue ? LoadValue(unchecked((int)uintValue), byAddress)
			: value is long longValue ? new CodeInstruction(OpCodes.Ldc_I8, longValue)
			: value is ulong ulongValue ? LoadValue(unchecked((long)ulongValue), byAddress)
			: value is short shortValue ? LoadValue(unchecked((int)shortValue), byAddress)
			: value is short ushortValue ? LoadValue(unchecked((int)ushortValue), byAddress)
			: value is null ? new CodeInstruction(OpCodes.Ldnull)
			: value is string stringValue ?
				stringValue.NullOrEmpty() ? new CodeInstruction(OpCodes.Ldsfld, _emptyStringField)
				: new CodeInstruction(OpCodes.Ldstr, stringValue)
			: value is ConstructorInfo constructorInfo ?
				constructorInfo.GetParameters().Length == 0 ? new CodeInstruction(OpCodes.Newobj, constructorInfo)
				: throw new ArgumentException($"A constructor {constructorInfo.DeclaringType?.FullName ?? "null"}.{constructorInfo.Name} with parameters was passed to `{nameof(LoadValue)}`.")
			: value is FieldInfo fieldInfo ?
				fieldInfo.IsStatic ?
					byAddress ? new CodeInstruction(OpCodes.Ldsflda, fieldInfo)
					: new CodeInstruction(OpCodes.Ldsfld, fieldInfo)
				: throw new ArgumentException($"A non-static field {fieldInfo.DeclaringType?.FullName ?? "null"}.{fieldInfo.Name} was passed to `{nameof(LoadValue)}`.")
			: value is MethodInfo methodInfo ?
				methodInfo.IsStatic ?
					methodInfo.GetParameters().Length == 0 ? new CodeInstruction(OpCodes.Call, methodInfo)
					: throw new ArgumentException($"A method {methodInfo.DeclaringType?.FullName ?? "null"}.{methodInfo.Name} with parameters was passed to `{nameof(LoadValue)}`.")
				: throw new ArgumentException($"A non-static method {methodInfo.DeclaringType?.FullName ?? "null"}.{methodInfo.Name} was passed to `{nameof(LoadValue)}`.")
			: value is Enum enumValue ? LoadValue(Convert.ChangeType(enumValue, MiscellaneousUtility.GetEnumUnderlyingType(enumValue), CultureInfo.InvariantCulture), byAddress)
			: throw new ArgumentException($"An invalid value type {value.GetType().FullName} was passed to {nameof(LoadValue)}.");
#else
			value is bool boolValue ? LoadValue(boolValue ? 1 : 0, byAddress)
			: value is byte byteValue ? LoadValue(unchecked((int)byteValue), byAddress)
			: value is sbyte sbyteValue ? new(OpCodes.Ldc_I4_S, sbyteValue)
			: value is char charValue ? LoadValue(unchecked((int)charValue), byAddress)
			: value is double doubleValue ? new(OpCodes.Ldc_R8, doubleValue)
			: value is float floatValue ? new(OpCodes.Ldc_R4, floatValue)
			: value is int intValue ?
				intValue == -1 ? new(OpCodes.Ldc_I4_M1)
				: intValue == 0 ? new(OpCodes.Ldc_I4_0)
				: intValue == 1 ? new(OpCodes.Ldc_I4_1)
				: intValue == 2 ? new(OpCodes.Ldc_I4_2)
				: intValue == 3 ? new(OpCodes.Ldc_I4_3)
				: intValue == 4 ? new(OpCodes.Ldc_I4_4)
				: intValue == 5 ? new(OpCodes.Ldc_I4_5)
				: intValue == 6 ? new(OpCodes.Ldc_I4_6)
				: intValue == 7 ? new(OpCodes.Ldc_I4_7)
				: intValue == 8 ? new(OpCodes.Ldc_I4_8)
				: new(OpCodes.Ldc_I4, intValue)
			: value is uint uintValue ? LoadValue(unchecked((int)uintValue), byAddress)
			: value is long longValue ? new(OpCodes.Ldc_I8, longValue)
			: value is ulong ulongValue ? LoadValue(unchecked((long)ulongValue), byAddress)
			: value is short shortValue ? LoadValue(unchecked((int)shortValue), byAddress)
			: value is short ushortValue ? LoadValue(unchecked((int)ushortValue), byAddress)
			: value is null ? new(OpCodes.Ldnull)
			: value is string stringValue ?
				stringValue.NullOrEmpty() ? new(OpCodes.Ldsfld, _emptyStringField)
				: new(OpCodes.Ldstr, stringValue)
			: value is ConstructorInfo constructorInfo ?
				constructorInfo.GetParameters().Length == 0 ? new(OpCodes.Newobj, constructorInfo)
				: throw new ArgumentException($"A constructor {constructorInfo.DeclaringType?.FullName ?? "null"}.{constructorInfo.Name} with parameters was passed to `{nameof(LoadValue)}`.")
			: value is FieldInfo fieldInfo ?
				fieldInfo.IsStatic ?
					byAddress ? new(OpCodes.Ldsflda, fieldInfo)
					: new(OpCodes.Ldsfld, fieldInfo)
				: throw new ArgumentException($"A non-static field {fieldInfo.DeclaringType?.FullName ?? "null"}.{fieldInfo.Name} was passed to `{nameof(LoadValue)}`.")
			: value is MethodInfo methodInfo ?
				methodInfo.IsStatic ?
					methodInfo.GetParameters().Length == 0 ? new(OpCodes.Call, methodInfo)
					: throw new ArgumentException($"A method {methodInfo.DeclaringType?.FullName ?? "null"}.{methodInfo.Name} with parameters was passed to `{nameof(LoadValue)}`.")
				: throw new ArgumentException($"A non-static method {methodInfo.DeclaringType?.FullName ?? "null"}.{methodInfo.Name} was passed to `{nameof(LoadValue)}`.")
			: value is Enum enumValue ? LoadValue(Convert.ChangeType(enumValue, MiscellaneousUtility.GetEnumUnderlyingType(enumValue), CultureInfo.InvariantCulture), byAddress)
			: throw new ArgumentException($"An invalid value type {value.GetType().FullName} was passed to {nameof(LoadValue)}.");
#endif

		/// <summary>
		/// Load the given value onto the stack.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="byAddress">Whether or not to load the value by address. Only applicable to fields.</param>
		/// <param name="generator">The code generator.</param>
		/// <param name="localAddress">Whether or not to store the value locally and load its address. This probably shouldn't be used with `byAddress`.</param>
		/// <returns>The instructions that will load the given value onto the stack.</returns>
		public static IEnumerable<CodeInstruction> LoadValue(object? value, ILGenerator generator, bool byAddress = false, bool localAddress = false) {
			yield return LoadValue(value, byAddress);
			if (!localAddress) {
				yield break;
			}

			foreach (CodeInstruction instruction in GetLocalAddress(generator, value)) {
				yield return instruction;
			}
		}

		/// <summary>
		/// OR the value on top of the stack with whether the given information category is not known for the "given" faction.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="getFactionInstructions">The instructions to execute to load the faction onto the stack.</param>
		/// <param name="controlCategory">The type of control or notification that contains the obscured information.</param>
		/// <returns>The instructions that will perform the OR.</returns>
		public static IEnumerable<CodeInstruction> OrFactionNotKnown(InformationCategory category, IEnumerable<CodeInstruction> getFactionInstructions, ControlCategory controlCategory = ControlCategory.Default) =>
			OrNotKnown(IsInformationKnownForFactionMethod, category, getFactionInstructions, controlCategory);

		/// <summary>
		/// OR the value on top of the stack with whether the given information category is not known for the "given" pawn or faction.
		/// </summary>
		/// <param name="isInformationKnownForMethod">The method to call to check knowledge.</param>
		/// <param name="category">The information category.</param>
		/// <param name="getInstructions">The instructions to execute to load the pawn or faction onto the stack.</param>
		/// <param name="controlCategory">The type of control or notification that contains the obscured information.</param>
		/// <returns>The instructions that will perform the OR.</returns>
		private static IEnumerable<CodeInstruction> OrNotKnown(MethodInfo isInformationKnownForMethod, InformationCategory category, IEnumerable<CodeInstruction> getInstructions, ControlCategory controlCategory = ControlCategory.Default) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (getInstructions is null) {
				throw new ArgumentNullException(nameof(getInstructions));
			}
#else
			ArgumentNullException.ThrowIfNull(getInstructions);
#endif

			// Load the arguments for `KnowledgeUtility.IsInformationKnownFor` onto the stack.
			yield return LoadValue(category); // `category`.
			foreach (CodeInstruction instruction in getInstructions) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				yield return new CodeInstruction(instruction); // `pawn` or `faction`.
#else
				yield return new(instruction); // `pawn` or `faction`.
#endif
			}
			yield return LoadValue(controlCategory); // `controlCategory`.

			// Call `KnowledgeUtility.IsInformationKnownFor`, leaving the return value on top of the stack.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			yield return new CodeInstruction(OpCodes.Call, isInformationKnownForMethod); // Remove the arguments from the stack and add the return value.
#else
			yield return new(OpCodes.Call, isInformationKnownForMethod); // Remove the arguments from the stack and add the return value.
#endif

			// Load `false` onto the stack, then check if it's equal to the return value (thus negating it).
			yield return LoadValue(false);
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
#else
			yield return new(OpCodes.Ceq);
#endif

			// OR the negated return value with the value that was originally on top of the stack.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			yield return new CodeInstruction(OpCodes.Or);
#else
			yield return new(OpCodes.Or);
#endif
		}

		/// <summary>
		/// OR the value on top of the stack with whether the given information category is not known for the "given" pawn.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="getPawnInstructions">The instructions to execute to load the pawn onto the stack.</param>
		/// <param name="controlCategory">The type of control or notification that contains the obscured information.</param>
		/// <returns>The instructions that will perform the OR.</returns>
		public static IEnumerable<CodeInstruction> OrPawnNotKnown(InformationCategory category, IEnumerable<CodeInstruction> getPawnInstructions, ControlCategory controlCategory = ControlCategory.Default) =>
			OrNotKnown(IsInformationKnownForPawnMethod, category, getPawnInstructions, controlCategory);

		/// <summary>
		/// OR the value on top of the stack with whether the given information category is not known for the "given" thing.
		/// </summary>
		/// <param name="category">The information category.</param>
		/// <param name="getThingInstructions">The instructions to execute to load the thing onto the stack.</param>
		/// <param name="controlCategory">The type of control or notification that contains the obscured information.</param>
		/// <returns>The instructions that will perform the OR.</returns>
		public static IEnumerable<CodeInstruction> OrThingNotKnown(InformationCategory category, IEnumerable<CodeInstruction> getThingInstructions, ControlCategory controlCategory = ControlCategory.Default) =>
			OrNotKnown(IsInformationKnownForThingMethod, category, getThingInstructions, controlCategory);

		/// <summary>
		/// Get the getter method for the given accessor. Only necessary for RimWorld 1.0, since the versions of Harmony for later RimWorld versions have their own built-in method for this.
		/// </summary>
		/// <param name="type">The type on which the accessor exists.</param>
		/// <param name="name">The name of the accessor.</param>
		/// <returns>The getter method.</returns>
		public static MethodInfo PropertyGetter(Type type, string name) =>
#if V1_0
			AccessTools.Method(type, $"get_{name}");
#else
			AccessTools.PropertyGetter(type, name);
#endif

		/// <summary>
		/// Replace the backstory on top of the stack if it isn't known for the "given" pawn.
		/// </summary>
		/// <param name="getPawnInstructions">The instructions to execute to load the pawn onto the stack.</param>
		/// <param name="generator">The code generator.</param>
		/// <param name="value">The value to replace the top of the stack with if the information category isn't known for the "given" pawn.</param>
		/// <param name="byAddress">Whether or not to load the value by address. Only applicable to fields.</param>
		/// <param name="localAddress">Whether or not to store the value locally and load its address. This probably shouldn't be used with `byAddress`.</param>
		/// <returns>The instructions that will perform the conditional replacement.</returns>
		public static IEnumerable<CodeInstruction> ReplaceBackstoryIfNotKnown(IEnumerable<CodeInstruction> getPawnInstructions, ILGenerator generator, object? value = null, bool byAddress = false, bool localAddress = false) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (getPawnInstructions is null) {
				throw new ArgumentNullException(nameof(getPawnInstructions));
			}

			if (generator is null) {
				throw new ArgumentNullException(nameof(generator));
			}
#else
			ArgumentNullException.ThrowIfNull(getPawnInstructions);
			ArgumentNullException.ThrowIfNull(generator);
#endif

			// Load the arguments for `KnowledgeUtility.IsBackstoryKnown` onto the stack.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			yield return new CodeInstruction(OpCodes.Dup); // `backstory`.
#else
			yield return new(OpCodes.Dup); // `backstory`.
#endif
			foreach (CodeInstruction instruction in getPawnInstructions) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				yield return new CodeInstruction(instruction); // `pawn`.
#else
				yield return new(instruction); // `pawn`.
#endif
			}

			// Call `KnowledgeUtility.IsBackstoryKnown`, leaving the return value on top of the stack.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			yield return new CodeInstruction(OpCodes.Call, IsBackstoryDefKnownMethod); // Remove the arguments from the stack and add the return value.
#else
			yield return new(OpCodes.Call, IsBackstoryDefKnownMethod); // Remove the arguments from the stack and add the return value.
#endif

			// If the value on top of the stack is `true` (the given information is known), don't replace the value.
			Label dontReplaceLabel = generator.DefineLabel();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			yield return new CodeInstruction(OpCodes.Brtrue_S, dontReplaceLabel); // Remove the return value of `KnowledgeUtility.IsInformationKnownFor` from the stack, leaving the value that might be replaced on top.
#else
			yield return new(OpCodes.Brtrue_S, dontReplaceLabel); // Remove the return value of `KnowledgeUtility.IsInformationKnownFor` from the stack, leaving the value that might be replaced on top.
#endif

			// This section is skipped unless the given information isn't known.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			yield return new CodeInstruction(OpCodes.Pop); // Remove the value that is being replaced from the stack.
#else
			yield return new(OpCodes.Pop); // Remove the value that is being replaced from the stack.
#endif
			foreach (CodeInstruction i in LoadValue(value, generator, byAddress, localAddress)) {
				yield return i;
			}

			// Jump here when the given information is known, skipping the code that replaces the original value (thus not modifying the stack).
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			CodeInstruction dontReplaceTarget = new CodeInstruction(OpCodes.Nop);
#else
			CodeInstruction dontReplaceTarget = new(OpCodes.Nop);
#endif
			dontReplaceTarget.labels.Add(dontReplaceLabel);
			yield return dontReplaceTarget;
		}

		/// <summary>
		/// Replace the value on top of the stack if any skill isn't known for the given work type for the given pawn.
		/// </summary>
		/// <param name="getPawnInstructions">The instructions to execute to load the pawn onto the stack.</param>
		/// <param name="getWorkTypeInstructions">The instructions to execute to load the work type onto the stack.</param>
		/// <param name="generator">The code generator.</param>
		/// <param name="value">The value to replace the top of the stack with if any skill isn't known for the given work type for the given pawn.</param>
		/// <param name="byAddress">Whether or not to load the value by address. Only applicable to fields.</param>
		/// <param name="localAddress">Whether or not to store the value locally and load its address. This probably shouldn't be used with `byAddress`.</param>
		/// <returns>The instructions that will perform the conditional replacement.</returns>
		public static IEnumerable<CodeInstruction> ReplaceIfAnySkillsForWorkTypeNotKnown(IEnumerable<CodeInstruction> getPawnInstructions, IEnumerable<CodeInstruction> getWorkTypeInstructions, ILGenerator generator, object? value = null, bool byAddress = false, bool localAddress = false) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (getPawnInstructions is null) {
				throw new ArgumentNullException(nameof(getPawnInstructions));
			}

			if (getWorkTypeInstructions is null) {
				throw new ArgumentNullException(nameof(getWorkTypeInstructions));
			}

			if (generator is null) {
				throw new ArgumentNullException(nameof(generator));
			}
#else
			ArgumentNullException.ThrowIfNull(getPawnInstructions);
			ArgumentNullException.ThrowIfNull(getWorkTypeInstructions);
			ArgumentNullException.ThrowIfNull(generator);
#endif

			// Load the arguments for `KnowledgeUtility.AreAllSkillsForWorkTypeKnown` onto the stack.
			foreach (CodeInstruction instruction in getPawnInstructions) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				yield return new CodeInstruction(instruction); // `pawn`.
#else
				yield return new(instruction); // `pawn`.
#endif
			}
			foreach (CodeInstruction instruction in getWorkTypeInstructions) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				yield return new CodeInstruction(instruction); // `workType`.
#else
				yield return new(instruction); // `workType`.
#endif
			}

			// Call `KnowledgeUtility.AreAllSkillsForWorkTypeKnown`, leaving the return value on top of the stack.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			yield return new CodeInstruction(OpCodes.Call, AreAllSkillsForWorkTypeKnownMethod); // Remove the arguments from the stack and add the return value.
#else
			yield return new(OpCodes.Call, AreAllSkillsForWorkTypeKnownMethod); // Remove the arguments from the stack and add the return value.
#endif

			// If the value on top of the stack is `true` (all skills for the work type are known), don't replace the value.
			Label dontReplaceLabel = generator.DefineLabel();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			yield return new CodeInstruction(OpCodes.Brtrue_S, dontReplaceLabel); // Remove the return value of `KnowledgeUtility.AreAllSkillsForWorkTypeKnown` from the stack, leaving the value that might be replaced on top.
#else
			yield return new(OpCodes.Brtrue_S, dontReplaceLabel); // Remove the return value of `KnowledgeUtility.AreAllSkillsForWorkTypeKnown` from the stack, leaving the value that might be replaced on top.
#endif

			// This section is skipped if all skills for the work type are known.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			yield return new CodeInstruction(OpCodes.Pop); // Remove the value that is being replaced from the stack.
#else
			yield return new(OpCodes.Pop); // Remove the value that is being replaced from the stack.
#endif
			foreach (CodeInstruction i in LoadValue(value, generator, byAddress, localAddress)) {
				yield return i;
			}

			// Jump here when all skills for the work type are known, skipping the code that replaces the original value (thus not modifying the stack).
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			CodeInstruction dontReplaceTarget = new CodeInstruction(OpCodes.Nop);
#else
			CodeInstruction dontReplaceTarget = new(OpCodes.Nop);
#endif
			dontReplaceTarget.labels.Add(dontReplaceLabel);
			yield return dontReplaceTarget;
		}

		/// <summary>
		/// Replace the value on top of the stack if the given information category isn't known for the "given" faction.
		/// </summary>
		/// <param name="category">The information category that must be known to skip the replacement.</param>
		/// <param name="getFactionInstructions">The instructions to execute to load the faction onto the stack.</param>
		/// <param name="generator">The code generator.</param>
		/// <param name="value">The value to replace the top of the stack with if the information category isn't known for the "given" faction.</param>
		/// <param name="byAddress">Whether or not to load the value by address. Only applicable to fields.</param>
		/// <param name="localAddress">Whether or not to store the value locally and load its address. This probably shouldn't be used with `byAddress`.</param>
		/// <param name="controlCategory">The type of control or notification that contains the obscured information.</param>
		/// <returns>The instructions that will perform the conditional replacement.</returns>
		public static IEnumerable<CodeInstruction> ReplaceIfFactionNotKnown(InformationCategory category, IEnumerable<CodeInstruction> getFactionInstructions, ILGenerator generator, object? value = null, bool byAddress = false, bool localAddress = false, ControlCategory controlCategory = ControlCategory.Default) =>
			ReplaceIfNotKnown(IsInformationKnownForFactionMethod, category, getFactionInstructions, generator, value, byAddress, localAddress, controlCategory);

		/// <summary>
		/// Replace the value on top of the stack if the given information category isn't known for the "given" pawn or faction.
		/// </summary>
		/// <param name="isInformationKnownForMethod">The method to call to check knowledge.</param>
		/// <param name="category">The information category that must be known to skip the replacement.</param>
		/// <param name="getInstructions">The instructions to execute to load the pawn or faction onto the stack.</param>
		/// <param name="generator">The code generator.</param>
		/// <param name="value">The value to replace the top of the stack with if the information category isn't known for the "given" pawn or faction.</param>
		/// <param name="byAddress">Whether or not to load the value by address. Only applicable to fields.</param>
		/// <param name="localAddress">Whether or not to store the value locally and load its address. This probably shouldn't be used with `byAddress`.</param>
		/// <param name="controlCategory">The type of control or notification that contains the obscured information.</param>
		/// <returns>The instructions that will perform the conditional replacement.</returns>
		private static IEnumerable<CodeInstruction> ReplaceIfNotKnown(MethodInfo isInformationKnownForMethod, InformationCategory category, IEnumerable<CodeInstruction> getInstructions, ILGenerator generator, object? value = null, bool byAddress = false, bool localAddress = false, ControlCategory controlCategory = ControlCategory.Default) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (getInstructions is null) {
				throw new ArgumentNullException(nameof(getInstructions));
			}

			if (generator is null) {
				throw new ArgumentNullException(nameof(generator));
			}
#else
			ArgumentNullException.ThrowIfNull(getInstructions);
			ArgumentNullException.ThrowIfNull(generator);
#endif

			// Load the arguments for `KnowledgeUtility.IsInformationKnownFor` onto the stack.
			yield return LoadValue(category); // `category`.
			foreach (CodeInstruction instruction in getInstructions) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				yield return new CodeInstruction(instruction); // `pawn` or `faction`.
#else
				yield return new(instruction); // `pawn` or `faction`.
#endif
			}
			yield return LoadValue(controlCategory); // `controlCategory`.

			// Call `KnowledgeUtility.IsInformationKnownFor`, leaving the return value on top of the stack.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			yield return new CodeInstruction(OpCodes.Call, isInformationKnownForMethod); // Remove the arguments from the stack and add the return value.
#else
			yield return new(OpCodes.Call, isInformationKnownForMethod); // Remove the arguments from the stack and add the return value.
#endif

			// If the value on top of the stack is `true` (the given information is known), don't replace the value.
			Label dontReplaceLabel = generator.DefineLabel();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			yield return new CodeInstruction(OpCodes.Brtrue_S, dontReplaceLabel); // Remove the return value of `KnowledgeUtility.IsInformationKnownFor` from the stack, leaving the value that might be replaced on top.
#else
			yield return new(OpCodes.Brtrue_S, dontReplaceLabel); // Remove the return value of `KnowledgeUtility.IsInformationKnownFor` from the stack, leaving the value that might be replaced on top.
#endif

			// This section is skipped if the given information is known.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			yield return new CodeInstruction(OpCodes.Pop); // Remove the value that is being replaced from the stack.
#else
			yield return new(OpCodes.Pop); // Remove the value that is being replaced from the stack.
#endif
			foreach (CodeInstruction i in LoadValue(value, generator, byAddress, localAddress)) {
				yield return i;
			}

			// Jump here when the given information is known, skipping the code that replaces the original value (thus not modifying the stack).
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			CodeInstruction dontReplaceTarget = new CodeInstruction(OpCodes.Nop);
#else
			CodeInstruction dontReplaceTarget = new(OpCodes.Nop);
#endif
			dontReplaceTarget.labels.Add(dontReplaceLabel);
			yield return dontReplaceTarget;
		}

		/// <summary>
		/// Replace the value on top of the stack if the given information category isn't known for the "given" pawn.
		/// </summary>
		/// <param name="category">The information category that must be known to skip the replacement.</param>
		/// <param name="getPawnInstructions">The instructions to execute to load the pawn onto the stack.</param>
		/// <param name="generator">The code generator.</param>
		/// <param name="value">The value to replace the top of the stack with if the information category isn't known for the "given" pawn.</param>
		/// <param name="byAddress">Whether or not to load the value by address. Only applicable to fields.</param>
		/// <param name="localAddress">Whether or not to store the value locally and load its address. This probably shouldn't be used with `byAddress`.</param>
		/// <param name="controlCategory">The type of control or notification that contains the obscured information.</param>
		/// <returns>The instructions that will perform the conditional replacement.</returns>
		public static IEnumerable<CodeInstruction> ReplaceIfPawnNotKnown(InformationCategory category, IEnumerable<CodeInstruction> getPawnInstructions, ILGenerator generator, object? value = null, bool byAddress = false, bool localAddress = false, ControlCategory controlCategory = ControlCategory.Default) =>
			ReplaceIfNotKnown(IsInformationKnownForPawnMethod, category, getPawnInstructions, generator, value, byAddress, localAddress, controlCategory);

		/// <summary>
		/// Replace the value on top of the stack if the given information category isn't known for the "given" thing.
		/// </summary>
		/// <param name="category">The information category that must be known to skip the replacement.</param>
		/// <param name="getThingInstructions">The instructions to execute to load the thing onto the stack.</param>
		/// <param name="generator">The code generator.</param>
		/// <param name="value">The value to replace the top of the stack with if the information category isn't known for the "given" thing.</param>
		/// <param name="byAddress">Whether or not to load the value by address. Only applicable to fields.</param>
		/// <param name="localAddress">Whether or not to store the value locally and load its address. This probably shouldn't be used with `byAddress`.</param>
		/// <param name="controlCategory">The type of control or notification that contains the obscured information.</param>
		/// <returns>The instructions that will perform the conditional replacement.</returns>
		public static IEnumerable<CodeInstruction> ReplaceIfThingNotKnown(InformationCategory category, IEnumerable<CodeInstruction> getThingInstructions, ILGenerator generator, object? value = null, bool byAddress = false, bool localAddress = false, ControlCategory controlCategory = ControlCategory.Default) =>
			ReplaceIfNotKnown(IsInformationKnownForThingMethod, category, getThingInstructions, generator, value, byAddress, localAddress, controlCategory);

		/// <summary>
		/// Skip the method call on top of the stack if the given information category isn't known, removing its arguments from the stack and optionally adding a replacement return value to the stack. This function includes the passed `callInstruction` in its returned instructions, so the original instance should not be returned again.
		/// </summary>
		/// <param name="callInstruction">The call instruction to be called or skipped depending on knowledge.</param>
		/// <param name="category">The information category.</param>
		/// <param name="getFactionInstructions">The instructions to execute to load the faction onto the stack.</param>
		/// <param name="generator">The code generator.</param>
		/// <param name="value">The value to replace the return value with if the call is skipped and the call would have returned a value.</param>
		/// <param name="byAddress">Whether or not to load the value by address. Only applicable to fields.</param>
		/// <param name="localAddress">Whether or not to store the value locally and load its address. This probably shouldn't be used with `byAddress`.</param>
		/// <param name="controlCategory">The type of control or notification that contains the obscured information.</param>
		/// <returns>The instructions that will perform the check.</returns>
		/// <exception cref="ArgumentException"></exception>
		public static IEnumerable<CodeInstruction> SkipIfFactionNotKnown(CodeInstruction callInstruction, InformationCategory category, IEnumerable<CodeInstruction> getFactionInstructions, ILGenerator generator, object? value = null, bool byAddress = false, bool localAddress = false, ControlCategory controlCategory = ControlCategory.Default) =>
			SkipIfNotKnown(IsInformationKnownForFactionMethod, callInstruction, category, getFactionInstructions, generator, value, byAddress, localAddress, controlCategory);

		/// <summary>
		/// Skip the method call on top of the stack if the given information category isn't known, removing its arguments from the stack and optionally adding a replacement return value to the stack. This function includes the passed `callInstruction` in its returned instructions, so the original instance should not be returned again.
		/// </summary>
		/// <param name="isInformationKnownForMethod">The method to call to check knowledge.</param>
		/// <param name="callInstruction">The call instruction to be called or skipped depending on knowledge.</param>
		/// <param name="category">The information category.</param>
		/// <param name="getInstructions">The instructions to execute to load the pawn or faction onto the stack.</param>
		/// <param name="generator">The code generator.</param>
		/// <param name="value">The value to replace the return value with if the call is skipped and the call would have returned a value.</param>
		/// <param name="byAddress">Whether or not to load the value by address. Only applicable to fields.</param>
		/// <param name="localAddress">Whether or not to store the value locally and load its address. This probably shouldn't be used with `byAddress`.</param>
		/// <param name="controlCategory">The type of control or notification that contains the obscured information.</param>
		/// <returns>The instructions that will perform the check.</returns>
		/// <exception cref="ArgumentException"></exception>
		public static IEnumerable<CodeInstruction> SkipIfNotKnown(MethodInfo isInformationKnownForMethod, CodeInstruction callInstruction, InformationCategory category, IEnumerable<CodeInstruction> getInstructions, ILGenerator generator, object? value = null, bool byAddress = false, bool localAddress = false, ControlCategory controlCategory = ControlCategory.Default) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (callInstruction is null) {
				throw new ArgumentNullException(nameof(callInstruction));
			}

			if (getInstructions is null) {
				throw new ArgumentNullException(nameof(getInstructions));
			}

			if (generator is null) {
				throw new ArgumentNullException(nameof(generator));
			}
#else
			ArgumentNullException.ThrowIfNull(callInstruction);
			ArgumentNullException.ThrowIfNull(getInstructions);
			ArgumentNullException.ThrowIfNull(generator);
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (!(callInstruction.operand is MethodInfo methodInfo) || !Calls(callInstruction, methodInfo)) {
#else
			if (callInstruction.operand is not MethodInfo methodInfo || !Calls(callInstruction, methodInfo)) {
#endif
				throw new ArgumentException($"Attempted to skip a non-call instruction with `{nameof(SkipIfNotKnown)}`.");
			}

			// Load the arguments for `KnowledgeUtility.IsInformationKnownFor` onto the stack.
			yield return LoadValue(category); // `category`
			foreach (CodeInstruction instruction in getInstructions) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				yield return new CodeInstruction(instruction); // `pawn` or `faction`.
#else
				yield return new(instruction); // `pawn` or `faction`.
#endif
			}
			yield return LoadValue(controlCategory); // `controlCategory`

			// Call `KnowledgeUtility.IsInformationKnownFor`, leaving the return value on top of the stack.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			yield return new CodeInstruction(OpCodes.Call, isInformationKnownForMethod); // Remove the arguments from the stack and add the return value.
#else
			yield return new(OpCodes.Call, isInformationKnownForMethod); // Remove the arguments from the stack and add the return value.
#endif

			// If the value on top of the stack is `false` (the given information is not known), skip `callInstruction` and execute the replacement instructions.
			Label doSkipLabel = generator.DefineLabel();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			yield return new CodeInstruction(OpCodes.Brfalse_S, doSkipLabel); // Remove the return value of `KnowledgeUtility.IsInformationKnownFor` from the stack, go to the replacement instructions.
#else
			yield return new(OpCodes.Brfalse_S, doSkipLabel); // Remove the return value of `KnowledgeUtility.IsInformationKnownFor` from the stack, go to the replacement instructions.
#endif

			// If the value on top of the stack was `true` (the given information is known), execute `callInstruction` and skip the replacement instructions.
			yield return callInstruction;
			Label dontSkipLabel = generator.DefineLabel();
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			yield return new CodeInstruction(OpCodes.Br_S, dontSkipLabel); // Skip the replacement instructions.
#else
			yield return new(OpCodes.Br_S, dontSkipLabel); // Skip the replacement instructions.
#endif

			// If the value on top of the stack was `false` (the given information is not known), execute the replacement instructions (pop the arguments and return a dummy value if appropriate).
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			CodeInstruction doSkipTarget = new CodeInstruction(OpCodes.Nop);
#else
			CodeInstruction doSkipTarget = new(OpCodes.Nop);
#endif
			doSkipTarget.labels.Add(doSkipLabel);
			yield return doSkipTarget;
			if (!methodInfo.IsStatic) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				yield return new CodeInstruction(OpCodes.Pop); // `this` isn't included in the return value of `GetParameters`.
#else
				yield return new(OpCodes.Pop); // `this` isn't included in the return value of `GetParameters`.
#endif
			}
			for (int i = 0; i < methodInfo.GetParameters().Length; i++) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
				yield return new CodeInstruction(OpCodes.Pop);
#else
				yield return new(OpCodes.Pop);
#endif
			}
			if (methodInfo.ReturnType != typeof(void)) {
				foreach (CodeInstruction i in LoadValue(value, generator, byAddress, localAddress)) {
					yield return i;
				}
			}

			// Jump here after executing `callInstruction`, skipping the replacement instructions.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			CodeInstruction dontSkipTarget = new CodeInstruction(OpCodes.Nop);
#else
			CodeInstruction dontSkipTarget = new(OpCodes.Nop);
#endif
			dontSkipTarget.labels.Add(dontSkipLabel);
			yield return dontSkipTarget;
		}

		/// <summary>
		/// Skip the method call on top of the stack if the given information category isn't known, removing its arguments from the stack and optionally adding a replacement return value to the stack. This function includes the passed `callInstruction` in its returned instructions, so the original instance should not be returned again.
		/// </summary>
		/// <param name="callInstruction">The call instruction to be called or skipped depending on knowledge.</param>
		/// <param name="category">The information category.</param>
		/// <param name="getPawnInstructions">The instructions to execute to load the pawn onto the stack.</param>
		/// <param name="generator">The code generator.</param>
		/// <param name="value">The value to replace the return value with if the call is skipped and the call would have returned a value.</param>
		/// <param name="byAddress">Whether or not to load the value by address. Only applicable to fields.</param>
		/// <param name="localAddress">Whether or not to store the value locally and load its address. This probably shouldn't be used with `byAddress`.</param>
		/// <param name="controlCategory">The type of control or notification that contains the obscured information.</param>
		/// <returns>The instructions that will perform the check.</returns>
		/// <exception cref="ArgumentException"></exception>
		public static IEnumerable<CodeInstruction> SkipIfPawnNotKnown(CodeInstruction callInstruction, InformationCategory category, IEnumerable<CodeInstruction> getPawnInstructions, ILGenerator generator, object? value = null, bool byAddress = false, bool localAddress = false, ControlCategory controlCategory = ControlCategory.Default) =>
			SkipIfNotKnown(IsInformationKnownForPawnMethod, callInstruction, category, getPawnInstructions, generator, value, byAddress, localAddress, controlCategory);

		/// <summary>
		/// Skip the method call on top of the stack if the given information category isn't known, removing its arguments from the stack and optionally adding a replacement return value to the stack. This function includes the passed `callInstruction` in its returned instructions, so the original instance should not be returned again.
		/// </summary>
		/// <param name="callInstruction">The call instruction to be called or skipped depending on knowledge.</param>
		/// <param name="category">The information category.</param>
		/// <param name="getThingInstructions">The instructions to execute to load the thing onto the stack.</param>
		/// <param name="generator">The code generator.</param>
		/// <param name="value">The value to replace the return value with if the call is skipped and the call would have returned a value.</param>
		/// <param name="byAddress">Whether or not to load the value by address. Only applicable to fields.</param>
		/// <param name="localAddress">Whether or not to store the value locally and load its address. This probably shouldn't be used with `byAddress`.</param>
		/// <param name="controlCategory">The type of control or notification that contains the obscured information.</param>
		/// <returns>The instructions that will perform the check.</returns>
		/// <exception cref="ArgumentException"></exception>
		public static IEnumerable<CodeInstruction> SkipIfThingNotKnown(CodeInstruction callInstruction, InformationCategory category, IEnumerable<CodeInstruction> getThingInstructions, ILGenerator generator, object? value = null, bool byAddress = false, bool localAddress = false, ControlCategory controlCategory = ControlCategory.Default) =>
			SkipIfNotKnown(IsInformationKnownForThingMethod, callInstruction, category, getThingInstructions, generator, value, byAddress, localAddress, controlCategory);
	}
}
