using UnityEngine;
using HarmonyLib;
using System.Reflection;

public class FishSeated : Mod
{
    public Harmony harmony;

    public void Start()
    {
        harmony = new Harmony("com.dcsobral.FishSeated");
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        Debug.Log("Mod FishSeated has been loaded! Now go fishing!");
	}

    public void OnModUnload()
    {

        harmony.UnpatchAll(harmony.Id);
        Destroy(gameObject);

        Debug.Log("Mod FishSeated has been unloaded! Sit idle all you want, see if I care.");
    }
}

[HarmonyPatch(typeof(PlayerSeat), "TryTakingSeat")]
public class HarmonyPatch_UseItemSeated
{
    [HarmonyPrefix]
	static void SaveCurrentItem(Network_Player player)
	{
		bool isLocalPlayer = player.IsLocalPlayer;
		if (isLocalPlayer)
		{
			currentItem = player.PlayerItemManager.useItemController.GetCurrentItemInHand();
		}
	}

    [HarmonyPostfix]
	static void MakeNotBusyAndUsingItem(ref bool __result, Network_Player player)
	{
        if (__result && player.IsLocalPlayer)
		{
			PlayerItemManager.IsBusy = false;
            if (currentItem != null)
			{
				player.PlayerItemManager.SelectUsable(currentItem);
			}
		}
	}

	private static Item_Base currentItem;
}
