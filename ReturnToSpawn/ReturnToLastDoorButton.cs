using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace DDoor.ReturnToSpawn;

internal class ReturnToLastDoorButton
{
    private static UIPrompt returnToDoorPrompt;
    private static readonly List<string> modifiedStrings = [];

    private static readonly float menuEntryHeight = 50;

    public void CreateReturnToLastDoorButton()
    {
        string parentScene = "_PLAYER";
        GameObject optionsPanel = Util.GetByPath(parentScene, "UI_PauseCanvas/MENU_Pause/Content/Panels/MENU_Options/ItemWindow_9slice/ItemWindow");
        GameObject exitToTitle = Util.GetByPath(parentScene, "UI_PauseCanvas/MENU_Pause/Content/Panels/MENU_Options/ItemWindow_9slice/ItemWindow/UI_ExitSession");
        GameObject returnToLastDoorButtonObject = GameObject.Instantiate(exitToTitle, optionsPanel.transform);
        LocTextTMP buttonText = returnToLastDoorButtonObject.GetComponentInChildren<LocTextTMP>();
        buttonText.locId = "RETURN TO RESPAWN LOCATION";
        if (!modifiedStrings.Contains(buttonText.locId))
        {
            modifiedStrings.Add(buttonText.locId);
        }
        returnToLastDoorButtonObject.transform.SetSiblingIndex(18);

        GameObject optionsMenuObject = Util.GetByPath(parentScene, "UI_PauseCanvas/MENU_Pause/Content/Panels/MENU_Options");
        GameObject exitToTitlePopup = Util.GetByPath(parentScene, "UI_PauseCanvas/MENU_Pause/Content/Panels/MENU_Options/Popup_ExitToTitle");
        GameObject returnToLastDoorPopup = GameObject.Instantiate(exitToTitlePopup, optionsMenuObject.transform);
        LocTextTMP popupPromptText = returnToLastDoorPopup.transform.Cast<Transform>()
            .First((t) => t.name == "Prompt")
            .gameObject.GetComponentInChildren<LocTextTMP>();
        popupPromptText.locId = "DO YOU WANT TO RETURN TO YOUR RESPAWN LOCATION?";
        if (!modifiedStrings.Contains(popupPromptText.locId))
        {
            modifiedStrings.Add(popupPromptText.locId);
        }
        returnToDoorPrompt = returnToLastDoorPopup.GetComponent<UIPrompt>();
        returnToDoorPrompt.id = "ReturnToDoor";

        returnToLastDoorButtonObject.GetComponent<UIAction>().actionId = "ReturnToDoor";

        UIMenuOptions optionsMenu = (UIMenuOptions)returnToDoorPrompt.master;
        UIButton[] newGrid = new UIButton[9];
        for (int i = 0; i < 9; i++)
        {
            if (i == 6)
            {
                newGrid[i] = returnToLastDoorButtonObject.GetComponent<UIAction>();
            }
            else if (i > 6)
            {
                newGrid[i] = optionsMenu.grid[i - 1];
            }
            else
            {
                newGrid[i] = optionsMenu.grid[i];
            }
        }
        optionsMenu.grid = newGrid;

        string[] newCtxt = new string[9];
        for (int i = 0; i < 9; i++)
        {
            if (i == 6)
            {
                newCtxt[i] = "cts_options_respawn";
            }
            else if (i > 6)
            {
                newCtxt[i] = optionsMenu.ctxt[i - 1];
            }
            else
            {
                newCtxt[i] = optionsMenu.ctxt[i];
            }
        }
        optionsMenu.ctxt = newCtxt;

        RectTransform optionsRectTransform = Util.GetByPath(parentScene, "UI_PauseCanvas/MENU_Pause/Content/Panels/MENU_Options/ItemWindow_9slice").GetComponent<RectTransform>();
        optionsRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, optionsRectTransform.rect.m_Height + menuEntryHeight);

    }
    private static void ReturnToLastDoor()
    {
        GameSceneManager.instance.Respawn(); // Same function called on Death
    }

    private static void ClosePrompt_ReturnToDoor(bool value)
    {
        UIMenuOptions optionsMenu = (UIMenuOptions)returnToDoorPrompt.master;
        if (optionsMenu.currentPrompt == returnToDoorPrompt)
        {
            optionsMenu.currentPrompt = null;
            returnToDoorPrompt.LoseFocus(true);
            optionsMenu.canMove = true;
            if (value)
            {
                optionsMenu.canMove = false;
                UIMenuPauseController.instance.UnPause();
                ReturnToLastDoor();
            }
        }
    }

    [HarmonyPatch]
    private class Patches()
    {
        /// <summary>
        /// Displays custom strings instead of original text
        /// </summary>
        [HarmonyPrefix, HarmonyPatch(typeof(LocTextTMP), nameof(LocTextTMP.check))]
        private static bool PreCheck(LocTextTMP __instance)
        {
            if (modifiedStrings.Contains(__instance.locId))
            {
                __instance.text.text = __instance.locId;
                return false;
            }

            return true;
        }


        /// <summary>
        /// Executes custom UIAction actionIDs
        /// </summary>
        [HarmonyPrefix, HarmonyPatch(typeof(UIAction), nameof(UIAction.Action))]
        private static bool PreAction(UIAction __instance)
        {
            if (__instance.actionId == "ReturnToDoor")
            {
                __instance.master.currentPrompt = returnToDoorPrompt;
                returnToDoorPrompt.GainFocus(true);
                __instance.master.canMove = false;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Closes custom prompt
        /// </summary>
        [HarmonyPrefix, HarmonyPatch(typeof(UIMenuOptions), nameof(UIMenuOptions.Close))]
        private static bool PreClose(string id, bool value)
        {
            if (id == "ReturnToDoor")
            {
                ClosePrompt_ReturnToDoor(value);
                return false;
            }

            return true;
        }
    }
}