using System.Linq;
using System.Collections.Generic;
using Assets.Scripts.Objects.Motherboards;
using Assets.Scripts.Objects.Pipes;
using System.Text.RegularExpressions;
using System;

namespace TwitchIntegration
{
    public static class DeviceInteractor
    {
        private static HashSet<Device> AllDevices
        {
            get
            {
                var devices = new HashSet<Device>();
                // AllDevices has duplicates
                foreach (var device in Device.AllDevices)
                {
                    devices.Add(device);
                }
                return devices;
            }
        }

        public static void ExecuteEffect(GameEffect effect)
        {
            var results = from device in AllDevices
                          let match = effect.tag.Match(device.DisplayName)
                          where match != null && match.Success
                          where effect.groupParams == null || MatchContainsParams(match, effect.groupParams)
                          let logicType = GetLogicType(effect, match)
                          let value = GetValue(effect, match)
                          where logicType.HasValue
                          where value.HasValue
                          select new { device = device, logicType = logicType.Value, value = value.Value };
            foreach (var result in results)
            {
                Logging.Log(
                    new Dictionary<string, string>{
                        {"Device", result.device.DisplayName},
                        {"LogicType", result.logicType.ToString()},
                        {"Value", result.value.ToString()}
                    }, "Executing effect");
                if (!result.device.CanLogicWrite(result.logicType))
                {
                    continue;
                }
                result.device.SetLogicValue(result.logicType, result.value);
            }
        }

        private static double? GetValue(GameEffect effect, Match match)
        {
            if (effect.value.HasValue)
            {
                return effect.value.Value;
            }
            if (effect.groupValue.HasValue)
            {
                var groupIndex = effect.groupValue.Value;
                if (match.Groups.Count <= groupIndex)
                {
                    return null;
                }
                var value = match.Groups[groupIndex].Value;
                double logicValue;
                if (!double.TryParse(value, out logicValue))
                {
                    return null;
                }
                return logicValue;
            }
            return null;
        }

        private static LogicType? GetLogicType(GameEffect effect, Match match)
        {
            if (effect.logicType.HasValue)
            {
                return effect.logicType.Value;
            }
            if (effect.groupLogicType.HasValue)
            {
                var groupIndex = effect.groupLogicType.Value;
                if (match.Groups.Count <= groupIndex)
                {
                    return null;
                }
                var value = match.Groups[groupIndex].Value;
                LogicType logicType;
                if (!Enum.TryParse<LogicType>(value, out logicType))
                {
                    return null;
                }
                return logicType;
            }
            return null;
        }


        private static bool MatchContainsParams(Match match, Dictionary<int, string> matchParams)
        {
            foreach (var key in matchParams.Keys)
            {
                var value = matchParams[key];
                if (match.Groups.Count <= key)
                {
                    return false;
                }
                if (match.Groups[key].Value != value)
                {
                    return false;
                }
            }
            return true;
        }
    }
}