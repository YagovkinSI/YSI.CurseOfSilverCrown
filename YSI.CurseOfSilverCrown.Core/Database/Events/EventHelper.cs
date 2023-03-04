﻿using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YSI.CurseOfSilverCrown.Core.Database.Domains;
using YSI.CurseOfSilverCrown.Core.Database.EventDomains;
using YSI.CurseOfSilverCrown.Core.Database.Users;
using YSI.CurseOfSilverCrown.Core.Helpers;
using YSI.CurseOfSilverCrown.Core.APIModels;

namespace YSI.CurseOfSilverCrown.Core.Database.Events
{
    public static class EventHelper
    {
        public static async Task<List<List<string>>> GetTextStories(ApplicationDbContext context, List<Event> eventStories,
            HistoryFilter historyFilter = null)
        {
            var textStories = new List<List<string>>();
            var maxCount = 200;
            var currentCount = 0;
            foreach (var eventStory in eventStories)
            {
                var turn = GameSessionHelper.GetName(context, eventStory.Turn);
                var (textStory, type) = await GetTextStoryAsync(context, eventStory);
                if (!historyFilter?.ResultTypes.Contains(type) ?? false)
                    continue;
                var pair = new List<string> { turn };
                pair.AddRange(textStory);
                textStories.Add(pair);
                currentCount++;
                if (currentCount >= maxCount)
                    break;
            }
            return textStories;
        }

        public static async Task<List<List<string>>> GetHistory(ApplicationDbContext context, HistoryFilter historyFilter,
            User currentUser)
        {
            var currentTurn = context.Turns.Single(t => t.IsActive);
            var firstTurnId = currentTurn.Id - historyFilter.Turns;

            var domainIds = GetDomainIds(context, historyFilter.Region, currentUser);

            var organizationEventStories = await context.OrganizationEventStories
               .Where(o => historyFilter.Turns == int.MaxValue || o.TurnId >= firstTurnId)
               .Where(o => historyFilter.Important == 0 || o.Importance >= historyFilter.Important)
               .Where(o => domainIds == null || domainIds.Contains(o.DomainId))
               .ToListAsync();

            var eventStories = GetEventStories(organizationEventStories);

            return await GetTextStories(context, eventStories, historyFilter);
        }

        private static List<int> GetDomainIds(ApplicationDbContext context, int region, User currentUser)
        {
            var presonId = currentUser?.PersonId;
            var userDoamin = context.Domains
                .FirstOrDefault(d => d.PersonId == presonId);
            if (userDoamin == null)
                return null;

            switch (region)
            {
                case 0:
                    return null;
                case 1:
                    return context.Domains.GetAllDomainsIdInKingdoms(userDoamin);
                case 2:
                    return context.Domains.GetAllLevelVassalIds(userDoamin.Id);
                case 3:
                    var list = new List<int> { userDoamin.Id };
                    list.AddRange(userDoamin.Vassals.Select(v => v.Id));
                    return list;
                case 4:
                    return new List<int> { userDoamin.Id };
                default:
                    throw new NotImplementedException();
            }
        }

        public static async Task<List<List<string>>> GetWorldHistory(ApplicationDbContext context)
        {
            var currentTurn = context.Turns
                .Single(t => t.IsActive);

            var organizationEventStories = await context.OrganizationEventStories
                .Where(o => o.TurnId != currentTurn.Id && o.Importance >= 50000)
                .OrderByDescending(o => o.Importance - 200 * o.TurnId)
                .Take(30)
                .OrderByDescending(o => o.EventStoryId)
                .OrderByDescending(o => o.TurnId)
                .ToListAsync();

            var eventStories = GetEventStories(organizationEventStories);

            return await GetTextStories(context, eventStories);
        }

        public static async Task<List<List<string>>> GetWorldHistoryLastRound(ApplicationDbContext context)
        {
            var currentTurn = context.Turns
                .Single(t => t.IsActive);

            var organizationEventStories = await context.OrganizationEventStories
                .Where(e => e.TurnId == currentTurn.Id - 1 && e.Importance >= 50000)
                .OrderByDescending(o => o.EventStoryId)
                .OrderByDescending(o => o.TurnId)
                .ToListAsync();

            var eventStories = GetEventStories(organizationEventStories);

            return await GetTextStories(context, eventStories);
        }

        private static List<Event> GetEventStories(List<EventDomain> domainEventStories)
        {
            return domainEventStories
                    .Select(o => o.EventStory)
                    .Distinct()
                    .OrderByDescending(o => o.Id)
                    .OrderByDescending(o => o.TurnId)
                    .ToList();
        }

        private static async Task<(List<string>, enEventType)> GetTextStoryAsync(
            ApplicationDbContext context, Event eventStory)
        {
            var text = new List<string>();
            var eventStoryResult = JsonConvert.DeserializeObject<EventJson>(eventStory.EventStoryJson);
            var type = eventStoryResult.EventResultType;

            var ids = eventStoryResult.Organizations.Select(e => e.Id);
            var allOrganizations = await context.Domains
                        .Where(c => ids.Contains(c.Id))
                        .ToListAsync();

            var eventStoryCard = GetEventStoryCard(eventStoryResult, allOrganizations);
            FillEventMainText(text, eventStoryResult, eventStoryCard);
            FillEventParameters(text, eventStoryResult, allOrganizations);
            return (text, type);
        }

        private static EventJsonDomainNameHelper GetEventStoryCard(EventJson eventStoryResult,
            List<Domain> allOrganizations)
        {
            var eventStoryCard = new EventJsonDomainNameHelper();
            foreach (var domain in eventStoryResult.Organizations)
            {
                var type = domain.EventOrganizationType;
                var name = allOrganizations.Single(d => d.Id == domain.Id).Name;
                eventStoryCard.TryAddName(type, name);
            }
            return eventStoryCard;
        }

        private static void FillEventMainText(List<string> text, EventJson eventStoryResult,
            EventJsonDomainNameHelper card)
        {
            var mainText = EventTextHelper.GetEventText(eventStoryResult.EventResultType, card);
            text.Add(mainText);
        }

        private static void FillEventParameters(List<string> text, EventJson eventStoryResult,
            List<Domain> allOrganizations)
        {
            foreach (var eventOrganization in eventStoryResult.Organizations)
            {
                var changes = eventOrganization.EventOrganizationChanges;
                if (changes.Count == 0)
                    continue;
                var organization = allOrganizations.First(o => o.Id == eventOrganization.Id);
                text.Add($"\r\n{organization.Name}: ");
                foreach (var change in changes)
                {
                    var chainging = change.Before > change.After
                        ? change.Type == enEventParameterType.Coffers
                            ? "Потрачено"
                            : "Потеряно"
                        : "Получено";
                    text.Add($"{EnumHelper<enEventParameterType>.GetDisplayValue(change.Type)}: " +
                        $"Было - {ViewHelper.GetSweetNumber(change.Before)}, " +
                        $"{chainging} - {Math.Abs(change.Before - change.After)}, " +
                        $"Стало - {ViewHelper.GetSweetNumber(change.After)}.");
                }
            }
        }

        internal static int GetThresholdImportance(int oldValue, int newValue)
        {
            var threshold = 1;
            var thresholdImportance = 0;
            var max = Math.Max(oldValue, newValue);
            var min = Math.Min(oldValue, newValue);
            while (true)
            {
                threshold = threshold.ToString().StartsWith("1")
                    ? threshold * 3
                    : threshold / 3 * 10;
                if (min > threshold)
                    continue;
                if (max < threshold)
                    break;
                thresholdImportance = threshold;
            }
            return thresholdImportance;
        }
    }
}