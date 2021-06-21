﻿using System;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Photon.Pun;
using UnboundLib;
using UnityEngine;

namespace BossSlothsTweaks
{
    [BepInDependency("com.willis.rounds.unbound")]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class BossSlothsTweaks : BaseUnityPlugin
    {
        
        private const string ModId = "com.BossSloth.Rounds.Tweaks";
        private const string ModName = "BossSlothsTweaks";
        public const string Version = "0.1.1";

        private static ConfigEntry<bool> PHOENIX;
        private static ConfigEntry<bool> GROW;
        private static ConfigEntry<bool> EMP;
        private static ConfigEntry<bool> SCAVENGER;
        private static ConfigEntry<bool> SAW;

        internal static BossSlothsTweaks Instance;

        private void Start()
        {
            Unbound.RegisterGUI("BossSlothTweaks", DrawGUI);
            Unbound.RegisterHandshake("com.willis.rounds.unbound", OnHandShakeCompleted);

            Instance = this;
            
            PHOENIX = Config.Bind("Cards", "Phoenix", false, "Added -50% damage, Reduced health to -50% (from -35%)");
            GROW = Config.Bind("Cards", "Grow", false, "Only one per game");
            EMP = Config.Bind("Cards", "Emp", false, "Only one per game");
            SCAVENGER = Config.Bind("Cards", "Scavenger", false, "Only one per game");
            SAW = Config.Bind("Cards", "Saw", false, "Reduced range to 4 (from 4.5), Only one per game");

            ChangeCards();

            var harmony = new Harmony("com.BossSloth.Rounds.Tweaks.Harmony");
            harmony.PatchAll();
        }

        private static void ChangeCards()
        {
            //Balancing cards
            foreach (var info in CardChoice.instance.cards)
            {
                switch (info.cardName)
                {
                    case "PHOENIX":
                    {
                        if (PHOENIX.Value)
                        {
                            var infoList = info.cardStats.ToList();
                            if (infoList.Count > 2)
                            {
                                break;
                            }
                            var damage = new CardInfoStat {stat = "Damage", amount = "-50%", positive = false};
                            infoList.Add(damage);
                            infoList[0].amount = "-50%";
                            info.cardStats = infoList.ToArray();
                        
                            var gun = info.GetComponent<Gun>();
                            gun.damage = 0.5f;
                            var charstat = info.GetComponent<CharacterStatModifiers>();
                            charstat.health = 0.5f;
                        }
                        else
                        {
                            info.cardStats = new[]
                            {
                                new CardInfoStat {stat = "Health", amount = "-35%", positive = false}
                            };
                            info.GetComponent<Gun>().damage = 1;
                            info.GetComponent<CharacterStatModifiers>().health = 0.65f;
                            
                        }
                        break;
                    }
                    case "GROW":
                    {
                        if (GROW.Value)
                        {
                            info.allowMultiple = false;
                        }
                        else
                        {
                            info.allowMultiple = true;
                        }
                        break;
                    }
                    case "EMP":
                    {
                        if (EMP.Value)
                        {
                            info.allowMultiple = false;
                        }
                        else
                        {
                            info.allowMultiple = true;
                        }
                        break;
                    }
                    case "SCAVENGER":
                    {
                        if (SCAVENGER.Value)
                        {
                            info.allowMultiple = false;
                        }
                        else
                        {
                            info.allowMultiple = true;
                        }
                        break;
                    }
                    case "SAW":
                    {
                        if (!SAW.Value)
                        {
                            info.allowMultiple = false;
                            var saw = info.gameObject.GetComponent<CharacterStatModifiers>().AddObjectToPlayer.GetComponent<SpawnObjects>().objectToSpawn[0].GetComponent<Saw>();
                            saw.range = 4;
                        }
                        else
                        {
                            info.allowMultiple = true;
                            info.gameObject.GetComponent<CharacterStatModifiers>().AddObjectToPlayer.GetComponent<SpawnObjects>().objectToSpawn[0].GetComponent<Saw>().range = 4.5f;
                        }
                        break;
                    }
                    // case "SUPERNOVA":
                    // {
                    //     info.allowMultiple = false;
                    //     var nova = info.gameObject.GetComponent<CharacterStatModifiers>().AddObjectToPlayer.GetComponent<SpawnObjects>().objectToSpawn[0].GetComponent<SpawnObjects>().objectToSpawn[0].GetComponents<Explosion>();
                    //     nova[1].damage = 25;
                    //     break;
                    // }
                }
                
            }
        }

        private void DrawGUI()
        {
            bool flag1 = GUILayout.Toggle(PHOENIX.Value, "Phoenix", Array.Empty<GUILayoutOption>());
            bool flag2 = GUILayout.Toggle(GROW.Value, "Grow", Array.Empty<GUILayoutOption>());
            bool flag3 = GUILayout.Toggle(EMP.Value, "Emp", Array.Empty<GUILayoutOption>());
            bool flag4 = GUILayout.Toggle(SCAVENGER.Value, "Scavenger", Array.Empty<GUILayoutOption>());
            bool flag5 = GUILayout.Toggle(SAW.Value, "Saw", Array.Empty<GUILayoutOption>());
            if (flag1 != PHOENIX.Value || flag2 != GROW.Value || flag3 != EMP.Value || flag4 != SCAVENGER.Value || flag5 != SAW.Value)
            {
                NetworkingManager.RaiseEvent("com.BossSloth.Rounds.Tweaks_SyncTweaks", new object[]
                {
                    flag1,
                    flag2,
                    flag3,
                    flag4,
                    flag5,
                });
                ChangeCards();
            }

            PHOENIX.Value = flag1;
            GROW.Value = flag2;
            EMP.Value = flag3;
            SCAVENGER.Value = flag4;
            SAW.Value = flag5;
        }
        
        private void Awake()
        {
            NetworkingManager.RegisterEvent("com.BossSloth.Rounds.Tweaks_SyncTweaks", delegate(object[] e)
            {
                PHOENIX.Value = (bool)e[0];
                GROW.Value = (bool)e[1];
                EMP.Value = (bool)e[2];
                SCAVENGER.Value = (bool)e[3];
                SAW.Value = (bool)e[4];
            });
        }
        
        private void OnHandShakeCompleted()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                NetworkingManager.RaiseEvent("com.BossSloth.Rounds.Tweaks_SyncTweaks", new object[]
                {
                    PHOENIX.Value,
                    GROW.Value,
                    EMP.Value,
                    SCAVENGER.Value,
                    SAW.Value,
                })
                ;
            }
        }
    }
}