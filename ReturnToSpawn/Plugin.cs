using BepInEx;
using BepInEx.Logging;
using DDoor.AddUIToOptionsMenu;
using HarmonyLib;
using UnityEngine;

namespace DDoor.ReturnToSpawn;

[BepInPlugin("deathsdoor.returntospawn", MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("deathsdoor.adduitooptionsmenu")]
public class Plugin : BaseUnityPlugin
{
	internal static new ManualLogSource Logger;
	private static Plugin instance;

	public static Plugin Instance => instance;
	public int InitStatus { get; internal set; } = 0;

	private void Awake()
	{
		instance = this;

		try
		{
			Logger = base.Logger;
			Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

			new Harmony("deathsdoor.returntospawn").PatchAll();

			AddReturnButtons();

			InitStatus = 1;
		}
		catch (System.Exception err)
		{
			InitStatus = 2;
			throw err;
		}
	}

	private void AddReturnButtons()
	{
		OptionsPrompt returnToDoorPrompt = new(promptText: "DO YOU WANT TO RETURN TO YOUR RESPAWN LOCATION?", gameObjectName: "RETURN_TO_SPAWN_Popup_Respawn", closeAction: ReturnToLastDoor);
		OptionsButton returnToDoorButton = new(itemText: "RETURN TO RESPAWN LOCATION", gameObjectName: "RETURN_TO_SPAWN_UI_Respawn", id: "ReturnToDoor", relevantScenes: [IngameUIManager.RelevantScene.InGame], optionsPrompt: returnToDoorPrompt, contextText: "BUTTON:CONFIRM Return to Respawn Location BUTTON:BACK Back");
		IngameUIManager.AddOptionsMenuItem(returnToDoorButton);

		OptionsPrompt returnToHallPrompt = new(promptText: "DO YOU WANT TO RETURN TO THE HALL OF DOORS?", gameObjectName: "RETURN_TO_SPAWN_Popup_Hall", closeAction: ReturnToHall);
		OptionsButton returnToHallButton = new(itemText: "RETURN TO HALL OF DOORS", gameObjectName: "RETURN_TO_SPAWN_UI_Hall", id: "ReturnToHall", relevantScenes: [IngameUIManager.RelevantScene.InGame], optionsPrompt: returnToHallPrompt, contextText: "BUTTON:CONFIRM Return to Hall of Doors BUTTON:BACK Back");
		IngameUIManager.AddOptionsMenuItem(returnToHallButton);
	}
	
	private static void ReturnToLastDoor()
    {
        GameSceneManager.instance.Respawn(); // Same function called on Death
    }
    private static void ReturnToHall()
    {
        GameSceneManager.LoadSceneFadeOut("lvl_hallofdoors", 0.2f, true);
        DoorTrigger.currentTargetDoor = "_debug";
        ScreenFade.instance.UnLockFade();
		ScreenFade.instance.SetColor(Color.black, false);
		ScreenFade.instance.FadeOut(0.2f, true, null);
		ScreenFade.instance.LockFade();
    }
}