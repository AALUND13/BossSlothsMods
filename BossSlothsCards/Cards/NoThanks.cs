﻿using BossSlothsCards.Extensions;
using BossSlothsCards.MonoBehaviours;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;


namespace BossSlothsCards.Cards
{
    public class NoThanks : CustomCard
    {
        public AssetBundle Asset;

        public string cardRemovedName;
        
        protected override string GetTitle()
        {
            return "No thanks";
        }

        protected override string GetDescription()
        {
            return "Replace your most recent card with a random card";
        }
        
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
#if DEBUG
            UnityEngine.Debug.Log("Adding NoThanks card");
#endif
            DoNoThanksThings(player,gun,gunAmmo,data,health,gravity,block,characterStats);
        }

        private void DoNoThanksThings(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            if (player.data.currentCards.Count == 0)
            {
                return;
            }
            // get amount in currentCards
            var count = player.data.currentCards.Count - 1;
            var tries = 0;
            while (!(tries > 50))
            {
                tries++;
                if (player.data.currentCards.Count <= -1)
                {
                    return;
                }
                // check if card is not NoThanks
                if (player.data.currentCards[count].cardName == "No thanks")
                {
                    count--;
                    continue;
                }
                
                cardRemovedName = player.data.currentCards[count].cardName;
                var cardToRemove = player.data.currentCards[count];
                var randomCard = Utils.Cards.NORARITY_GetRandomCardWithCondition(player, gun, gunAmmo, data, health, gravity, block, characterStats, condition);
                UnityEngine.Debug.LogWarning(randomCard);
                
                Utils.Cards.RemoveCardFromPlayer(player, cardToRemove);
                player.ExecuteAfterSeconds(0.2f, () =>
                {
                    Utils.Cards.AddCardToPlayer(player, randomCard);
                    Utils.CardBarUtils.instance.ShowAtEndOfPhase(player, randomCard);
                    UnityEngine.Debug.LogWarning(cardToRemove.cardName);
                });
                break;
            }
        }

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
#if DEBUG
            UnityEngine.Debug.Log("Setting up NoThanks card");
#endif
            cardInfo.allowMultiple = true;
            // var cardData = new CardInfoAdditionalData()
            // {
            //     canBeReassigned = false
            // };
            // cardInfo.AddData(cardData);
            cardInfo.GetAdditionalData().canBeReassigned = false;
            
            transform.Find("CardBase(Clone)(Clone)/Canvas/Front/Grid/EffectText")?.gameObject.GetOrAddComponent<RainbowText>();
            UnityEngine.Debug.LogWarning(cardInfo.GetAdditionalData().canBeReassigned);
        }

        protected override CardInfoStat[] GetStats()
        {
            return null;
        }

        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
        }

        protected override GameObject GetCardArt()
        {
            return null;
        }

        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.MagicPink;
        }


        public override void OnRemoveCard()
        {
        }
        
        public override string GetModName()
        {
            return "BSC";
        }
        
        //From PCE
        public bool condition(CardInfo card, Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            // do not allow duplicates of cards with allowMultiple == false
            // card rarity must be as desired
            // card cannot be another Gamble / Jackpot card
            // card cannot be from a blacklisted category of any other card
        
            return card.cardName != cardRemovedName && card.cardName != "No thanks";
        }

    }
}
