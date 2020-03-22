
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Assets.Scripts.Objects.Motherboards;
using TwitchLib.Client.Events;

namespace TwitchIntegration
{
    public class GameEffect
    {
        public Regex tag { get; set; }
        public Dictionary<int, string> groupParams { get; set; }
        public int? groupLogicType { get; set; }
        public int? groupValue { get; set; }
        public LogicType? logicType { get; set; }
        public double? value { get; set; }
    }

    public static class EffectGenerator
    {
        private static readonly Regex REGEX_SUB_SETTING_ONE = new Regex(@"\[twitch\:sub\]", RegexOptions.Compiled);
        private static readonly Regex REGEX_SUB_TYPE_VALUE = new Regex(@"\[twitch\:sub\:([^\:]+)\:([^\]]+)\]", RegexOptions.Compiled);

        private static readonly Regex REGEX_CMD_SETTING_ONE = new Regex(@"\[twitch\:cmd\:([^\]]+)\]", RegexOptions.Compiled);
        private static readonly Regex REGEX_CMD_TYPE = new Regex(@"\[twitch\:cmd\:([^\:]+)\:([^\]]+)\]", RegexOptions.Compiled);
        private static readonly Regex REGEX_CMD_TYPE_VALUE = new Regex(@"\[twitch\:cmd\:([^\:]+)\:([^\:]+)\:([^\]]+)\]", RegexOptions.Compiled);

        public static IEnumerable<GameEffect> EffectsFromSubscription(SubscribeEventArgs ea)
        {
            yield return new GameEffect
            {
                tag = REGEX_SUB_SETTING_ONE,
                logicType = LogicType.Setting,
                value = 1
            };
            yield return new GameEffect
            {
                tag = REGEX_SUB_TYPE_VALUE,
                groupLogicType = 1,
                groupValue = 2
            };
        }

        public static IEnumerable<GameEffect> EffectsFromChatCommand(OnChatCommandReceivedArgs e)
        {
            var groupParams = new Dictionary<int, string> {
                {1, e.Command.CommandText}
            };

            double value = double.NaN;
            if (e.Command.ArgumentsAsList.Count > 0)
            {
                if (!double.TryParse(e.Command.ArgumentsAsList[0], out value))
                {
                    value = double.NaN;
                }
            }

            yield return new GameEffect
            {
                tag = REGEX_CMD_SETTING_ONE,
                groupParams = groupParams,
                logicType = LogicType.Setting,
                value = 1
            };
            if (!double.IsNaN(value))
            {
                yield return new GameEffect
                {
                    tag = REGEX_CMD_TYPE,
                    groupParams = groupParams,
                    groupLogicType = 2,
                    groupValue = 3,
                    value = value
                };
            }
            yield return new GameEffect
            {
                tag = REGEX_CMD_TYPE_VALUE,
                groupParams = groupParams,
                groupLogicType = 2,
                groupValue = 3
            };
        }
    }
}