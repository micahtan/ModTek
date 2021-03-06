using BattleTech.ModSupport;
using BattleTech.Save;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using Harmony;
using ModTek.RuntimeLog;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static ModTek.Util.Logger;

namespace ModTek.Patches
{
    [HarmonyPatch(typeof(BattleTech.ModSupport.ModLoader))]
    [HarmonyPatch("AreModsEnabled")]
    [HarmonyPatch(MethodType.Getter)]
    public static class ModLoader_AreModsEnabled
    {
        public static bool Prepare() { return ModTek.Enabled; }
        public static void Postfix(ref bool __result)
        {
            //Action OnModLoadComplete = (Action)typeof(BattleTech.ModSupport.ModLoader).GetField("OnModLoadComplete",BindingFlags.Static|BindingFlags.NonPublic).GetValue(null);
            //OnModLoadComplete.Invoke();
            //typeof(BattleTech.ModSupport.ModLoader).GetProperty("ModFilePaths", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null,new List<string>().ToArray());
            __result = false;
        }
    }
    [HarmonyPatch(typeof(BattleTech.ModSupport.ModLoader))]
    [HarmonyPatch("LoadSystemModStatus")]
    [HarmonyPatch(MethodType.Normal)]
    public static class ModLoader_LoadSystemMods
    {
        public static bool Prepare() { return !ModTek.Enabled; }
        //public static Dictionary<string, bool> loadedModsCache = new Dictionary<string, bool>();
        public static bool Prefix()
        {
            //loadedModsCache = playerSettings.loadedMods;
            //playerSettings.loadedMods.Clear();
            //foreach (var mod in ModTek.allModDefs)
            //{
            //    if (mod.Value.Hidden == false) { playerSettings.loadedMods.Add(mod.Key, mod.Value.Enabled); }
            //}
            return true;
        }
        public static void Postfix()
        {
            //playerSettings.loadedMods = loadedModsCache;
            //if (ModTek.allModDefs.TryGetValue(ModTek.MODTEK_DEF_NAME, out ModDefEx modtek))
            //{
            //    ModLoader.loadedSystemModStatus = new Dictionary<string, ModStatusItem>();
            //    ModLoader.loadedSystemModStatus.Add(ModTek.MODTEK_DEF_NAME, modtek.ToVanilla());
            //}
        }
    }
    [HarmonyPatch(typeof(BattleTech.ModSupport.ModLoader))]
    [HarmonyPatch("LoadSystemModStatus")]
    [HarmonyPatch(MethodType.Normal)]
    public static class ModLoader_LoadSystemModStatus
    {
        public static bool Prepare() { return ModTek.Enabled; }
        //public static Dictionary<string, bool> loadedModsCache = new Dictionary<string, bool>();
        public static bool Prefix()
        {
            //loadedModsCache = playerSettings.loadedMods;
            //playerSettings.loadedMods.Clear();
            //foreach (var mod in ModTek.allModDefs)
            //{
            //    if (mod.Value.Hidden == false) { playerSettings.loadedMods.Add(mod.Key, mod.Value.Enabled); }
            //}
            ModLoader.loadedSystemModStatus = new Dictionary<string, ModStatusItem>();
            ModLoader.loadedSystemModStatus.Add(ModTek.MODTEK_DEF_NAME, ModTek.SettingsDef.ToVanilla());
            return false;
        }
        public static void Postfix()
        {
            //playerSettings.loadedMods = loadedModsCache;
        }
    }
    [HarmonyPatch(typeof(BattleTech.ModSupport.ModLoader))]
    [HarmonyPatch("LoadGameModStatus")]
    [HarmonyPatch(MethodType.Normal)]
    public static class ModLoader_LoadGameModStatus
    {
        public static bool Prepare() { return ModTek.Enabled; }
        //public static Dictionary<string, bool> loadedModsCache = new Dictionary<string, bool>();
        public static bool Prefix()
        {

            ModLoader.loadedGameModStatus = new Dictionary<string, ModStatusItem>();
            foreach (var mod in ModTek.allModDefs) {
                if (mod.Key == ModTek.MODTEK_DEF_NAME) { continue; }
                if (mod.Value.Hidden) { continue; }
                ModLoader.loadedGameModStatus.Add(mod.Key,mod.Value.ToVanilla());
            }
            return false;
        }
        public static void Postfix()
        {
            //playerSettings.loadedMods = loadedModsCache;
        }
    }
    [HarmonyPatch(typeof(BattleTech.UI.ModManagerScreen))]
    [HarmonyPatch("Init")]
    [HarmonyPatch(MethodType.Normal)]
    public static class ModManagerScreen_InitModResources
    {
        public static bool Prepare() { return ModTek.Enabled; }
        public static bool Prefix(BattleTech.UI.ModManagerScreen __instance)
        {
            PlayerPrefs.SetInt("ModsEnabled",1);
            return true;
        }
    }
    [HarmonyPatch(typeof(BattleTech.UI.ModManagerScreen))]
    [HarmonyPatch("Init")]
    [HarmonyPatch(MethodType.Normal)]
    public static class ModManagerScreen_InitModResourcesDisabled
    {
        public static bool Prepare() { return !ModTek.Enabled; }
        public static void Postfix(BattleTech.UI.ModManagerScreen __instance)
        {
            //if(ModLoader.ModDefs)
            if (__instance.tempLoadedMods.ContainsKey(ModTek.MODTEK_DEF_NAME) == false)
            {
                __instance.tempLoadedMods.Add(ModTek.MODTEK_DEF_NAME, ModTek.SettingsDef.ToVanilla());
            }
            else
            {
                __instance.tempLoadedMods[ModTek.MODTEK_DEF_NAME] = ModTek.SettingsDef.ToVanilla();
            }
        }
    }
    [HarmonyPatch(typeof(BattleTech.ModSupport.ModLoader))]
    [HarmonyPatch("GetCombinedModStatus")]
    [HarmonyPatch(MethodType.Normal)]
    public static class ModManagerInstalledModsPanel_GetCombinedModStatusDisabled
    {
        public static bool Prepare() { return !ModTek.Enabled; }
        public static void Postfix(ref Dictionary<string, ModStatusItem> __result)
        {
            if (__result.ContainsKey(ModTek.MODTEK_DEF_NAME) == false)
            {
                __result.Add(ModTek.MODTEK_DEF_NAME, ModTek.SettingsDef.ToVanilla());
            }
        }
    }
    [HarmonyPatch(typeof(BattleTech.ModSupport.ModLoader))]
    [HarmonyPatch("GetCombinedModStatus")]
    [HarmonyPatch(MethodType.Normal)]
    public static class ModManagerInstalledModsPanel_GetCombinedModStatus
    {
        public static bool Prepare() { return ModTek.Enabled; }
        public static void Postfix(ref Dictionary<string, ModStatusItem> __result)
        {
            __result = new Dictionary<string, ModStatusItem>();
            foreach(var mod in ModTek.allModDefs)
            {
                if (mod.Key == ModTek.MODTEK_DEF_NAME) { continue; }
                if (mod.Value.Hidden) { continue; }
                __result.Add(mod.Key, mod.Value.ToVanilla());
            }
            __result.Add(ModTek.MODTEK_DEF_NAME, ModTek.SettingsDef.ToVanilla());
        }
    }
    [HarmonyPatch(typeof(BattleTech.ModSupport.ModLoader))]
    [HarmonyPatch("SaveModStatusToFile")]
    [HarmonyPatch(MethodType.Normal)]
    public static class ModLoader_SaveSystemModStatusToFile
    {
        public static bool Prepare() { return !ModTek.Enabled; }
        public static bool Prefix(Dictionary<string, ModStatusItem> tempLoadedMods)
        {
            return true;
        }
        public static void Postfix(Dictionary<string, ModStatusItem> tempLoadedMods)
        {
            if(tempLoadedMods.TryGetValue(ModTek.MODTEK_DEF_NAME,out ModStatusItem modtek))
            {
                ModTek.SettingsDef.Enabled = modtek.enabled;
                ModTek.SettingsDef.SaveState();
            }
        }
    }
    [HarmonyPatch(typeof(BattleTech.ModSupport.ModLoader))]
    [HarmonyPatch("SaveModStatusToFile")]
    [HarmonyPatch(MethodType.Normal)]
    public static class ModLoader_SaveModStatusToFile
    {
        //public static bool SaveModsState { get; set; } = false;
        public static bool Prepare() { return ModTek.Enabled; }
        public static bool Prefix(Dictionary<string, ModStatusItem> tempLoadedMods)
        {
            RLog.M.TWL(0, "SaveModStatusToFile");
            bool changed = false;
            foreach (var mod in ModTek.allModDefs)
            {
                RLog.M.W(1, mod.Value.Name+":"+mod.Value.Enabled+":"+mod.Value.PendingEnable+":"+mod.Value.LoadFail);
                if (mod.Value.PendingEnable != mod.Value.Enabled)
                {
                    changed = true;
                    string moddefpath = Path.Combine(mod.Value.Directory, ModTek.MOD_JSON_NAME);
                    try
                    {
                        mod.Value.Enabled = mod.Value.PendingEnable;
                        mod.Value.SaveState();
                        RLog.M.W(" save state:"+mod.Value.Enabled);
                    }
                    catch (Exception e)
                    {
                        RLog.M.TWL(0,e.ToString());
                    }
                }
                RLog.M.WL("");
            }
            if(changed)File.WriteAllText(ModTek.ChangedFlagPath, "changed");
            RLog.M.flush();
            //SaveModsState = false;
            return false;
        }
    }
    [HarmonyPatch(typeof(BattleTech.UI.ModManagerScreen))]
    [HarmonyPatch("UnsavedSettings")]
    [HarmonyPatch(MethodType.Normal)]
    public static class ModManagerScreen_UnsavedSettings
    {
        public static bool Prepare() { return ModTek.Enabled; }
        public static bool Prefix(BattleTech.UI.ModManagerScreen __instance, ref bool __result)
        {
            __result = false;
            foreach(var mod in ModTek.allModDefs){
                if (mod.Value.PendingEnable != mod.Value.Enabled) { __result = true; return false; }
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(BattleTech.UI.ModManagerScreen))]
    [HarmonyPatch("ReceiveButtonPress")]
    [HarmonyPatch(MethodType.Normal)]
    public static class ModManagerScreen_ReceiveButtonPress
    {
        public static bool Prepare() { return ModTek.Enabled; }
        public static bool Prefix(BattleTech.UI.ModManagerScreen __instance, string button)
        {
            if(button == "revert")
            {
                foreach (var mod in ModTek.allModDefs){ mod.Value.PendingEnable = mod.Value.Enabled; }
                __instance.installedModsPanel.RefreshListViewItems();
                return false;
            }
            else
            if (button == "save")
            {
                //ActiveOrDefaultSettings_SaveUserSettings.SaveModsState = true;
                return true;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(BattleTech.UI.ModManagerScreen))]
    [HarmonyPatch("ToggleModsEnabled")]
    [HarmonyPatch(MethodType.Normal)]
    public static class ModManagerScreen_ToggleModsEnabled
    {
        public static bool Prepare() { return ModTek.Enabled; }
        public static bool Prefix(BattleTech.UI.ModManagerScreen __instance, BattleTech.UI.HBSDOTweenToggle ___modsEnabledToggleBox)
        {
            if (___modsEnabledToggleBox.IsToggled() == false) { ___modsEnabledToggleBox.SetToggled(true); }
            return false;
        }
    }
    [HarmonyPatch(typeof(BattleTech.UI.ModManagerListViewItem))]
    [HarmonyPatch("ToggleItemEnabled")]
    [HarmonyPatch(MethodType.Normal)]
    public static class ModManagerListViewItem_ToggleItemEnabled
    {
        public static bool Prepare() { return ModTek.Enabled; }
        public static bool Prefix(BattleTech.UI.ModManagerListViewItem __instance, BattleTech.UI.HBSDOTweenToggle ___toggleBox, ModManagerScreen ____screen)
        {
            if (ModTek.allModDefs.ContainsKey(__instance.ModStatusItem.name) == false)
            {
                //if (ModTek.allModDefs[modDef.Name].Enabled == false) { ___modNameText.color = Color.red; };
                ___toggleBox.SetToggled(false);
            }
            else
            {
                ModDefEx mod = ModTek.allModDefs[__instance.ModStatusItem.name];
                if (mod.Locked)
                {
                    ___toggleBox.SetToggled(mod.PendingEnable);
                    return false;
                }
                if(mod.PendingEnable == true)
                {
                    Dictionary<ModDefEx, bool> deps = mod.GatherDependsOnMe();
                    if (deps.Count > 0)
                    {
                        StringBuilder text = new StringBuilder();
                        text.AppendLine("Some mods relay on this:");
                        List<KeyValuePair<ModDefEx, bool>> listdeps = deps.ToList();
                        for (int t = 0; t < Mathf.Min(7, deps.Count); ++t)
                        {
                            text.AppendLine(listdeps[t].Key.Name + "->" + (listdeps[t].Value ? "Enable" : "Disable"));
                        }
                        if (deps.Count > 7) { text.AppendLine("..........."); }
                        text.AppendLine("They will fail to load, but it will be only your damn problem.");
                        GenericPopupBuilder.Create("Dependency conflict", text.ToString()).AddButton("Return", (Action)(() => { mod.PendingEnable = true; ___toggleBox.SetToggled(mod.PendingEnable); })).AddButton("Resolve", (Action)(() =>
                        {
                            mod.PendingEnable = false;
                            ___toggleBox.SetToggled(mod.PendingEnable);
                            foreach (var dmod in deps)
                            {
                                dmod.Key.PendingEnable = dmod.Value;
                            }
                            ____screen.installedModsPanel.RefreshListViewItems();
                        })).AddButton("Shoot own leg", (Action)(() => { mod.PendingEnable = false; ___toggleBox.SetToggled(mod.PendingEnable); })).IsNestedPopupWithBuiltInFader().SetAlwaysOnTop().Render();
                    }
                    else
                    {
                        mod.PendingEnable = false;
                        ___toggleBox.SetToggled(mod.PendingEnable);
                    }
                }
                else {
                    Dictionary<ModDefEx,bool> conflicts = mod.ConflictsWithMe();
                    if (conflicts.Count > 0)
                    {
                        StringBuilder text = new StringBuilder();
                        text.AppendLine("Some mods conflics with this or this mod have unresolved dependency:");
                        List<KeyValuePair<ModDefEx, bool>> listconflicts = conflicts.ToList();
                        for (int t = 0; t < Mathf.Min(7, listconflicts.Count); ++t)
                        {
                            text.AppendLine(listconflicts[t].Key.Name + "->" + (listconflicts[t].Value ? "Enable":"Disable"));
                        }
                        if (conflicts.Count > 7) { text.AppendLine("..........."); }
                        text.AppendLine("They will fail to load, but it will be only your damn problem.");
                        GenericPopupBuilder.Create("Dependency conflict", text.ToString()).AddButton("Return", (Action)(() => { mod.PendingEnable = false; ___toggleBox.SetToggled(mod.PendingEnable); })).AddButton("Resolve", (Action)(() =>
                        {
                            mod.PendingEnable = true;
                            ___toggleBox.SetToggled(mod.PendingEnable);
                            foreach (var dmod in listconflicts)
                            {
                                dmod.Key.PendingEnable = dmod.Value;
                            }
                            ____screen.installedModsPanel.RefreshListViewItems();
                        })).AddButton("Shoot own leg", (Action)(() => { mod.PendingEnable = true; ___toggleBox.SetToggled(mod.PendingEnable); })).IsNestedPopupWithBuiltInFader().SetAlwaysOnTop().Render();
                    }
                    else
                    {
                        mod.PendingEnable = true;
                        ___toggleBox.SetToggled(mod.PendingEnable);
                    }
                }
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(BattleTech.UI.ModManagerListViewItem))]
    [HarmonyPatch("SetData")]
    [HarmonyPatch(MethodType.Normal)]
    public static class ModManagerListViewItem_SetData
    {
        public static bool Prepare() { return ModTek.Enabled; }
        public static void Postfix(BattleTech.UI.ModManagerListViewItem __instance, ModStatusItem modStatusItem, LocalizableText ___modNameText, BattleTech.UI.HBSDOTweenToggle ___toggleBox)
        {
            if (ModTek.allModDefs.ContainsKey(modStatusItem.name))
            {
                ModDefEx mod = ModTek.allModDefs[modStatusItem.name];
                ___toggleBox.SetToggled(mod.PendingEnable);
                if (mod.LoadFail) {
                    ___modNameText.color = Color.red;
                    ___modNameText.SetText("!" + mod.Name);
                } else {
                    ___modNameText.color = Color.white;
                    ___modNameText.SetText(mod.Name);
                }
            }
        }
    }
    [HarmonyPatch(typeof(BattleTech.UI.ModManagerInstalledModsPanel))]
    [HarmonyPatch("InitializeList")]
    [HarmonyPatch(MethodType.Normal)]
    public static class ModManagerInstalledModsPanel_InitializeList
    {
        public static bool Prepare() { return ModTek.Enabled; }
        public static ModStatusItem ToVanilla(this ModDefEx mod)
        {
            ModStatusItem result = new ModStatusItem();
            result.name = mod.Name;
            result.enabled = mod.Enabled;
            result.version = mod.Version;
            result.website = mod.Website;
            result.failedToLoad = mod.LoadFail;
            result.dependsOn = mod.DependsOn.ToList();
            result.directory = mod.Directory;
            //result.Author = mod.Author;
            //result.Contact = mod.Contact;
            //result.Description = mod.Description;
            return result;
        }
        public static Dictionary<ModDefEx, bool> GatherDependsOnMe(this ModDefEx moddef)
        {
            Dictionary<ModDefEx, bool> result = new Dictionary<ModDefEx, bool>();
            foreach(var mod in moddef.AffectingOffline)
            {
                if (mod.Key.PendingEnable != mod.Value) { result.Add(mod.Key, mod.Value); }
            }
            return result;
        }
        public static Dictionary<ModDefEx,bool> ConflictsWithMe(this ModDefEx moddef)
        {
            Dictionary<ModDefEx, bool> result = new Dictionary<ModDefEx, bool>();
            foreach (var mod in moddef.AffectingOnline)
            {
                if (mod.Key.PendingEnable != mod.Value) { result.Add(mod.Key, mod.Value); }
            }
            return result;
        }
        public static bool Prefix(BattleTech.UI.ModManagerInstalledModsPanel __instance, ref bool __result, BattleTech.UI.ModManagerListView ___modsList)
        {
            typeof(BattleTech.UI.ModManagerInstalledModsPanel).GetMethod("Clear", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(__instance, new object[0] { });
            __instance.SetSort(0);
            if (ModTek.allModDefs.Count == 0) { __result = false; return false; };
            RLog.M.TWL(0, "InitializeList");
            foreach (var mod in ModTek.allModDefs)
            {
                mod.Value.PendingEnable = mod.Value.Enabled;
                if (mod.Value.Hidden == false){ ___modsList.Add(mod.Value.ToVanilla()); }
                RLog.M.WL(1, mod.Value.Name+":"+mod.Value.Enabled+":"+mod.Value.PendingEnable+":"+mod.Value.LoadFail);
            }
            __result = true;
            /*StringBuilder dbg = new StringBuilder();
            try
            {
                IList items = (IList)typeof(BattleTech.UI.ModManagerListView).BaseType.BaseType.BaseType.GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(___modsList);
                foreach (object itm in items)
                {
                    BattleTech.UI.ModManagerListViewItem item = itm as BattleTech.UI.ModManagerListViewItem;
                    if (item != null)
                    {
                        item.SetData(item.modDef);
                    }
                }
            }catch(Exception e)
            {
                dbg.AppendLine(e.ToString());
            }*/
            return false;
        }
    }
}