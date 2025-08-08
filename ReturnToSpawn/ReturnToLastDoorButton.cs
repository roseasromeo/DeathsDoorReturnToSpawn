using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DDoor.ReturnToSpawn;

internal class ReturnToLastDoorButton
{
    private static UIPrompt returnToDoorPrompt;
    private static UIPrompt returnToHallPrompt;
    private static readonly List<string> modifiedStrings = [];

    private static readonly float menuEntryHeight = 50;

    public void CreateReturnToButtons()
    {
        string parentScene = "_PLAYER";
        try
        {
            Util.GetByPath(parentScene, "UI_PauseCanvas/MENU_Pause/Content/Panels/MENU_Options/ItemWindow_9slice/ItemWindow/UI_Respawn");
            return;
        }
        catch (InvalidOperationException)
        {
            // Continue
        }
        GameObject optionsPanel = Util.GetByPath(parentScene, "UI_PauseCanvas/MENU_Pause/Content/Panels/MENU_Options/ItemWindow_9slice/ItemWindow");
        GameObject exitToTitle = Util.GetByPath(parentScene, "UI_PauseCanvas/MENU_Pause/Content/Panels/MENU_Options/ItemWindow_9slice/ItemWindow/UI_ExitSession");
        GameObject returnToLastDoorButtonObject = GameObject.Instantiate(exitToTitle, optionsPanel.transform);
        returnToLastDoorButtonObject.name = "UI_Respawn";
        LocTextTMP lastDoorButtonText = returnToLastDoorButtonObject.GetComponentInChildren<LocTextTMP>();
        lastDoorButtonText.locId = "RETURN TO RESPAWN LOCATION";
        if (!modifiedStrings.Contains(lastDoorButtonText.locId))
        {
            modifiedStrings.Add(lastDoorButtonText.locId);
        }
        returnToLastDoorButtonObject.transform.SetSiblingIndex(18);

        GameObject optionsMenuObject = Util.GetByPath(parentScene, "UI_PauseCanvas/MENU_Pause/Content/Panels/MENU_Options");
        GameObject exitToTitlePopup = Util.GetByPath(parentScene, "UI_PauseCanvas/MENU_Pause/Content/Panels/MENU_Options/Popup_ExitToTitle");
        GameObject returnToLastDoorPopup = GameObject.Instantiate(exitToTitlePopup, optionsMenuObject.transform);
        returnToLastDoorPopup.name = "Popup_Respawn";
        LocTextTMP lastDoorPopupPromptText = returnToLastDoorPopup.transform.Cast<Transform>()
            .First((t) => t.name == "Prompt")
            .gameObject.GetComponentInChildren<LocTextTMP>();
        lastDoorPopupPromptText.locId = "DO YOU WANT TO RETURN TO YOUR RESPAWN LOCATION?";
        if (!modifiedStrings.Contains(lastDoorPopupPromptText.locId))
        {
            modifiedStrings.Add(lastDoorPopupPromptText.locId);
        }
        returnToDoorPrompt = returnToLastDoorPopup.GetComponent<UIPrompt>();
        returnToDoorPrompt.id = "ReturnToDoor";

        returnToLastDoorButtonObject.GetComponent<UIAction>().actionId = "ReturnToDoor";

        GameObject returnToHallButtonObject = GameObject.Instantiate(exitToTitle, optionsPanel.transform);
        returnToHallButtonObject.name = "UI_Respawn";
        LocTextTMP hallButtonText = returnToHallButtonObject.GetComponentInChildren<LocTextTMP>();
        hallButtonText.locId = "RETURN TO HALL OF DOORS";
        if (!modifiedStrings.Contains(hallButtonText.locId))
        {
            modifiedStrings.Add(hallButtonText.locId);
        }
        returnToHallButtonObject.transform.SetSiblingIndex(19);

        GameObject returnToHallPopup = GameObject.Instantiate(exitToTitlePopup, optionsMenuObject.transform);
        returnToHallPopup.name = "Popup_Respawn";
        LocTextTMP hallPopupPromptText = returnToHallPopup.transform.Cast<Transform>()
            .First((t) => t.name == "Prompt")
            .gameObject.GetComponentInChildren<LocTextTMP>();
        hallPopupPromptText.locId = "DO YOU WANT TO RETURN TO THE HALL OF DOORS?";
        if (!modifiedStrings.Contains(hallPopupPromptText.locId))
        {
            modifiedStrings.Add(hallPopupPromptText.locId);
        }
        returnToHallPrompt = returnToHallPopup.GetComponent<UIPrompt>();
        returnToHallPrompt.id = "ReturnToHall";

        returnToHallButtonObject.GetComponent<UIAction>().actionId = "ReturnToHall";

        UIMenuOptions optionsMenu = (UIMenuOptions)returnToDoorPrompt.master;
        UIButton[] newGrid = new UIButton[10];
        for (int i = 0; i < 10; i++)
        {
            if (i == 6)
            {
                newGrid[i] = returnToLastDoorButtonObject.GetComponent<UIAction>();
            }
            else if (i == 7)
            {
                newGrid[i] = returnToHallButtonObject.GetComponent<UIAction>();
            }
            else if (i > 7)
            {
                newGrid[i] = optionsMenu.grid[i - 2];
            }
            else
            {
                newGrid[i] = optionsMenu.grid[i];
            }
        }
        optionsMenu.grid = newGrid;

        string[] newCtxt = new string[10];
        for (int i = 0; i < 10; i++)
        {
            if (i == 6)
            {
                newCtxt[i] = "cts_options_respawn";
            }
            else if (i == 7)
            {
                newCtxt[i] = "cts_options_hall";
            }
            else if (i > 7)
            {
                newCtxt[i] = optionsMenu.ctxt[i - 2];
            }
            else
            {
                newCtxt[i] = optionsMenu.ctxt[i];
            }
        }
        optionsMenu.ctxt = newCtxt;

        RectTransform optionsRectTransform = Util.GetByPath(parentScene, "UI_PauseCanvas/MENU_Pause/Content/Panels/MENU_Options/ItemWindow_9slice").GetComponent<RectTransform>();
        optionsRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, optionsRectTransform.rect.m_Height + 2 * menuEntryHeight);

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
                // optionsMenu.canMove = false;
                UIMenuPauseController.instance.UnPause();
                ReturnToLastDoor();
            }
        }
    }

    private static void ClosePrompt_ReturnToHall(bool value)
    {
        UIMenuOptions optionsMenu = (UIMenuOptions)returnToHallPrompt.master;
        if (optionsMenu.currentPrompt == returnToHallPrompt)
        {
            optionsMenu.currentPrompt = null;
            returnToHallPrompt.LoseFocus(true);
            optionsMenu.canMove = true;
            if (value)
            {
                // optionsMenu.canMove = false;
                UIMenuPauseController.instance.UnPause();
                ReturnToHall();
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
            else if (__instance.actionId == "ReturnToHall")
            {
                __instance.master.currentPrompt = returnToHallPrompt;
                returnToHallPrompt.GainFocus(true);
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
            else if (id == "ReturnToHall")
            {
                ClosePrompt_ReturnToHall(value);
                return false;
            }

            return true;
        }
    }
}