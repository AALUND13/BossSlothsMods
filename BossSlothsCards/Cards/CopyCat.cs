﻿using BossSlothsCards.Extensions;
using TMPro;
using UnboundLib.Cards;
using UnityEngine;


namespace BossSlothsCards.Cards
{
    public class CopyCat : CustomCard
    {
        public AssetBundle Asset;

        protected override string GetTitle()
        {
            return "Copycat";
        }

        protected override string GetDescription()
        {
            return "Copy a random card from a random enemy";
        }
        
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
#if DEBUG
            UnityEngine.Debug.Log("Adding Copycat card");
#endif
            var enemy = PlayerManager.instance.GetRandomEnemy(player); 
            if (enemy.data.currentCards.Count == 0)
            {
                return;
            };

            var tries = 0;
            while (!(tries > 50))
            {
                var randomNum = Random.Range(0, enemy.data.currentCards.Count);
                tries++;
                if (!Utils.Cards.PlayerIsAllowedCard(player, enemy.data.currentCards[randomNum])) continue;
                Utils.Cards.AddCardToPlayer(player, enemy.data.currentCards[randomNum]);
                Utils.CardBarUtils.instance.ShowAtEndOfPhase(player, enemy.data.currentCards[randomNum]);
                break;
            }
        }

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
#if DEBUG
            UnityEngine.Debug.Log("Setting up Copycat card");
#endif
            cardInfo.allowMultiple = true;
        }

        protected override CardInfoStat[] GetStats()
        {
            return null;
        }

        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
        }

        protected override GameObject GetCardArt()
        {
            return null;
        }

        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.DestructiveRed;
        }

        public override void OnRemoveCard()
        {
        }
    }
}