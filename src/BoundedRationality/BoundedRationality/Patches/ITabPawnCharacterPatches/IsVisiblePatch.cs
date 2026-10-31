using System.Reflection;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.ITabPawnCharacterPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Character), nameof(ITab_Pawn_Character.IsVisible), MethodType.Getter)]
	public static class IsVisiblePatch {
		private static readonly MethodInfo _pawnToShowInfoAboutMethod = PatchUtility.PropertyGetter(typeof(ITab_Pawn_Character), "PawnToShowInfoAbout");

		[HarmonyPostfix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(ITab_Pawn_Character __instance, ref bool __result) =>
#pragma warning restore IDE1006
			__result = __result
#if V1_0
				&& (!(_pawnToShowInfoAboutMethod.Invoke(__instance, MiscellaneousUtility.EmptyArray()) is Pawn pawn)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn, ControlCategory.Control) // Contains "rename," "banish," and "execute" controls.
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Traits, pawn)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Backstory, pawn)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, pawn));
#elif V1_1 || V1_2
				&& (!(_pawnToShowInfoAboutMethod.Invoke(__instance, MiscellaneousUtility.EmptyArray()) is Pawn pawn)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Personal, pawn, ControlCategory.Control) // Contains "renounce title" control.
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, pawn)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn, ControlCategory.Control) // Contains "rename," "banish," and "execute" controls.
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Traits, pawn)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Backstory, pawn)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, pawn));
#elif V1_3 || V1_4 || V1_5 || V1_6
				&& (!(_pawnToShowInfoAboutMethod.Invoke(__instance, MiscellaneousUtility.EmptyArray()) is Pawn pawn)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Personal, pawn, ControlCategory.Control) // Contains "renounce title" control.
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, pawn)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, pawn)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn, ControlCategory.Control) // Contains "rename," "banish," and "execute" controls.
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Traits, pawn)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Backstory, pawn)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, pawn));
#else
				&& (_pawnToShowInfoAboutMethod.Invoke(__instance, MiscellaneousUtility.EmptyArray()) is not Pawn pawn
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Personal, pawn, ControlCategory.Control) // Contains "renounce title" control.
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, pawn)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, pawn)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn, ControlCategory.Control) // Contains "rename," "banish," and "execute" controls.
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Traits, pawn)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Backstory, pawn)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, pawn));
#endif
	}
}
