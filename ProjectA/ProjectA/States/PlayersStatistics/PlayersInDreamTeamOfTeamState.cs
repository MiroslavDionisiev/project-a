﻿using ProjectA.Models.StateOfChatModels.Enums;
using ProjectA.Services.StateProvider;
using ProjectA.Services.Statistics;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Text;
using ProjectA.Helpers;
using static ProjectA.States.StateConstants;
using ProjectA.Services.Statistics.ServiceModels;

namespace ProjectA.States.PlayersStatistics
{
    public class PlayersInDreamTeamOfTeamState : IState
    {
        private readonly ICosmosDbStateProviderService _stateProvider;
        private readonly IStatisticsService _statisticsService;

        public PlayersInDreamTeamOfTeamState(ICosmosDbStateProviderService stateProvider, IStatisticsService statisticsService)
        {
            this._stateProvider = stateProvider;
            this._statisticsService = statisticsService;
        }

        private async Task HandleRequest(ITelegramBotClient botClient, Message message, string teamName)
        {
            var result = await this._statisticsService.PlayersInDreamTeamOfTeamAsync(teamName);
            if (result == null)
            {
                await InteractionHelper.PrintMessage(botClient, message.Chat.Id, "Wrong team name");
            }

            StringBuilder stringBuilder = new StringBuilder();
            int counter = 1;
            stringBuilder.Append($"Player Name - Position - In dreamteam");
            stringBuilder.AppendLine();
            foreach (PlayerDreamTeamData player in result)
            {
                stringBuilder.Append($"{counter}. {player.PlayerName} - {player.PlayerPosition} - {player.DreamTeamCount}");
                stringBuilder.AppendLine();
                counter++;
            }

            await InteractionHelper.PrintMessage(botClient, message.Chat.Id, stringBuilder.ToString());
        }

        public async Task<StateType> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            return StateType.TopScorersState;
        }

        public async Task<StateType> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == null)
            {
                return await InteractionHelper.PrintMessage(botClient, message.Chat.Id, StateMessages.InsertPlayersSuggestionsPreferences);
            }

            //var chat = await _stateProvider.GetChatStateAsync(message.Chat.Id);

            //await _stateProvider.UpdateChatStateAsync(chat);

            await this.HandleRequest(botClient, message, message.Text);

            return StateType.StatisticsMenuState;
        }

        public async Task BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            await botClient.SendTextMessageAsync(chatId, StateMessages.InsertPlayersSuggestionsPreferences);
        }
    }
}
