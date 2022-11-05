﻿using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppMonomiPark.SlimeRancher.Script.Util;
using System;
using UnityEngine;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization;
using Il2CppMonomiPark.SlimeRancher.UI.Popup;
using MelonSRML.Utils;

namespace MelonSRML.Patches
{
    [HarmonyPatch(typeof(SystemContext), "Start")]
    internal static class SystemContextInitializePatch
    {
        public static void Prefix(SystemContext __instance)
        {
            __instance.SceneLoader.onSceneGroupLoadedDelegate += new Action<SceneGroup, Il2CppSystem.Action<SceneLoadErrorData>>((x,y) =>
            {
                if ((x.name == "MainMenuFromBoot" && EntryPoint.interruptMenuLoad) || (x.isGameplay && EntryPoint.interruptGameLoad))
                {
                    var e = EntryPoint.error;
                    
                    // TODO: make better once a proper translation handler is made.
                    var title = LocalizationUtil.GetTable("UI").AddEntry("m.srml_load_error.title", "mSRML Error");
                    var titleString = new LocalizedString(LocalizationUtil.GetTable("UI").SharedData.TableCollectionNameGuid, title.SharedEntry.Id);

                    var error = LocalizationUtil.GetTable("UI").AddEntry("m.srml_load_error.error", 
                        $"{e.LoadingStep} error from '{e.ModName}': {e.Exception.Message}");
                    var errorString = new LocalizedString(LocalizationUtil.GetTable("UI").SharedData.TableCollectionNameGuid, error.SharedEntry.Id);

                    var exit = LocalizationUtil.GetTable("UI").AddEntry("m.srml_load_error.quit", "EXIT");
                    var exitString = new LocalizedString(LocalizationUtil.GetTable("UI").SharedData.TableCollectionNameGuid, exit.SharedEntry.Id);


                   
                    GameContext.Instance.UITemplates.CreatePositivePopupPrompt( ScriptableObjectUtils.CreateScriptable(new Action<PositivePopupPromptConfig>(config =>
                    {
                        config.activateActionsDelay = 0;
                        config.expirationDuration = 0;
                        config.expires = false;
                        config.message = errorString;
                        config.shouldDimBackground = true;
                        config.title = titleString;
                        config.positiveButtonText = exitString;

                    })), new Action(Application.Quit));
                }
            });

            if (EntryPoint.interruptMenuLoad)
                return;

            foreach (var mod in EntryPoint.registeredMods)
            {
                try
                {
                    mod.OnSystemContext(__instance);
                }
                catch (Exception e)
                {
                    LoadingError.CreateLoadingError(mod, LoadingError.Step.OnSystemContext, e);
                    EntryPoint.interruptMenuLoad = true;

                    break;
                }
            }
        }
    }
}
