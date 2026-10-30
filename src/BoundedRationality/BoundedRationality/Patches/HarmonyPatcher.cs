#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Verse;

namespace Lakuna.BoundedRationality.Patches {
	[StaticConstructorOnStartup]
	public static class HarmonyPatcher {
#if V1_0
		public static readonly HarmonyInstance Instance = HarmonyInstance.Create(nameof(BoundedRationality));
#elif V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		public static readonly Harmony Instance = new Harmony(nameof(BoundedRationality));
#else
		public static readonly Harmony Instance = new(nameof(BoundedRationality));
#endif

		static HarmonyPatcher() =>
			Instance.PatchAll();
	}
}
