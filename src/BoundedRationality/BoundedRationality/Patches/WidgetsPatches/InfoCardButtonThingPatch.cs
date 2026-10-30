#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
using System;
#endif

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using Verse;

namespace Lakuna.BoundedRationality.Patches.WidgetsPatches {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
	[HarmonyPatch(typeof(Widgets), nameof(Widgets.InfoCardButton), new Type[] { typeof(float), typeof(float), typeof(Thing) })]
#else
	[HarmonyPatch(typeof(Widgets), nameof(Widgets.InfoCardButton), [typeof(float), typeof(float), typeof(Thing)])]
#endif
	public static class InfoCardButtonThingPatch {
		[HarmonyPrefix]
		public static bool Prefix(Thing thing) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			!(thing is Pawn pawn)
#else
			thing is not Pawn pawn
#endif
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, pawn, ControlCategory.Control);
	}
}
