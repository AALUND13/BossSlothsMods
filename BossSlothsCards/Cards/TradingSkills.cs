﻿using BossSlothsCards.Extensions;
using BossSlothsCards.Utils.Text;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using ModdingUtils.Extensions;
using ModdingUtils.Utils;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace BossSlothsCards.Cards
{
    public class TradingSkills : CustomCard
    {

        protected override string GetTitle()
        {
            return "Trading skills";
        }

        protected override string GetDescription()
        {
            return "Swap your most recent card with a random card from a random opponent";
        }
        
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            BossSlothCards.instance.ExecuteAfterSeconds(0.1f, () =>
            {
                DoTradingThings(player);
            });
        }
        
        private void DoTradingThings(Player player)
        {
            UnityEngine.Debug.LogWarning("test");
            var enemy = PlayerManager.instance.GetRandomEnemy(player);
            var tris = 0;
            while (enemy.data.currentCards.Count == 0 && tris < 5)
            {
                tris++;
                enemy = PlayerManager.instance.GetRandomEnemy(player);
            }
            if (enemy == null || player.data.currentCards.Count == 0 || enemy.data.currentCards.Count == 0)
            {
                return;
            }
            UnityEngine.Debug.LogWarning("test2");
            // get amount in currentCards
            var count = player.data.currentCards.Count - 1;
            var tries = 0;
            UnityEngine.Debug.LogWarning("almost while");
            while (!(tries > 50))
            {
                tries++;
                if (player.data.currentCards.Count <= -1 || enemy.data.currentCards.Count <= -1 || count <= -1)
                {
                    return;
                }
                
                var mostRecentCard = player.data.currentCards[count];
                var randomCard = enemy.data.currentCards[Random.Range(0, enemy.data.currentCards.Count - 1)];

                if (!ModdingUtils.Utils.Cards.instance.PlayerIsAllowedCard(player, randomCard) || !ModdingUtils.Utils.Cards.instance.CardIsNotBlacklisted(mostRecentCard, new[] { CustomCardCategories.instance.CardCategory("CardManipulation"), CustomCardCategories.instance.CardCategory("NoRemove")}) || !ModdingUtils.Utils.Cards.instance.CardIsNotBlacklisted(randomCard, new[] { CustomCardCategories.instance.CardCategory("CardManipulation"), CustomCardCategories.instance.CardCategory("NoRemove")}))
                {
                    count--;
                    continue;
                }

                ModdingUtils.Utils.Cards.instance.RemoveCardFromPlayer(player, mostRecentCard, ModdingUtils.Utils.Cards.SelectionType.Newest);
                ModdingUtils.Utils.Cards.instance.RemoveCardFromPlayer(enemy, randomCard, ModdingUtils.Utils.Cards.SelectionType.Random);
                
                player.ExecuteAfterSeconds(0.2f, () =>
                {
                    ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, randomCard, false, "", 0, 0, true);
                    ModdingUtils.Utils.Cards.instance.AddCardToPlayer(enemy, mostRecentCard, false, "", 0, 0, true);
                    CardBarUtils.instance.ShowAtEndOfPhase(player, randomCard);
                });
                break;
            }
        }
        
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            cardInfo.allowMultiple = true;
            cardInfo.GetAdditionalData().canBeReassigned = false;
            cardInfo.categories = new[] { CustomCardCategories.instance.CardCategory("CardManipulation") };
            
            transform.Find("CardBase(Clone)(Clone)/Canvas/Front/Grid/EffectText")?.gameObject.GetOrAddComponent<RainbowText>();
            
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

        public override string GetModName()
        {
            return "BSC";
        }

        public override void OnRemoveCard()
        {
        }

    }
}