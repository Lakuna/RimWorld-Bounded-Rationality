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
		private static readonly MethodInfo PawnToShowInfoAboutMethod = PatchUtility.PropertyGetter(typeof(ITab_Pawn_Character), "PawnToShowInfoAbout");

		[HarmonyPostfix]
#pragma warning disable CA1707
		public static void Postfix(ITab_Pawn_Character __instance, ref bool __result) =>
#pragma warning restore CA1707
			__result = __result
			&& (!(PawnToShowInfoAboutMethod.Invoke(__instance, MiscellaneousUtility.EmptyArray()) is Pawn pawn)
#if !V1_0
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Personal, pawn, ControlCategory.Control) // Contains "renounce title" control.
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, pawn)
#endif
#if !(V1_0 || V1_1 || V1_2)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, pawn)
#endif
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn, ControlCategory.Control) // Contains "rename," "banish," and "execute" controls.
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Traits, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Backstory, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, pawn));
	}
}
