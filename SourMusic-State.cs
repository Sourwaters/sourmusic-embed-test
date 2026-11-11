using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class CPHInline
{
    private static readonly object Gate = new object();
    public bool Execute()
    {
        lock (Gate)
        {
            try
            {
                string value;
                long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                if (CPH.TryGetArg<string>("sourmusicCommand", out value))
                {
                    if (String.IsNullOrEmpty(value) || value.Length > 4096) return false;
                    JObject command = JObject.Parse(value);
                    string type = (string)command["type"];
                    if (type != "next" && type != "stop" && type != "play" && type != "add") return false;
                    if (String.IsNullOrEmpty((string)command["id"]) || String.IsNullOrEmpty((string)command["session"])) return false;
                    command["at"] = now;
                    string saved = CPH.GetGlobalVar<string>("sourmusic.commands", false);
                    JArray commands = String.IsNullOrEmpty(saved) ? new JArray() : JArray.Parse(saved);
                    foreach (JToken item in commands) if ((string)item["id"] == (string)command["id"]) return true;
                    commands.Add(command);
                    while (commands.Count > 64) commands.RemoveAt(0);
                    CPH.SetGlobalVar("sourmusic.commands", commands.ToString(Formatting.None), false);
                    return true;
                }
                if (!CPH.TryGetArg<string>("sourmusicState", out value) || String.IsNullOrEmpty(value) || value.Length > 65536) return false;
                JObject state = JObject.Parse(value);
                state["at"] = now;
                CPH.SetGlobalVar("sourmusic.nowPlaying", state.ToString(Formatting.None), false);
                return true;
            }
            catch { return false; }
        }
    }
}
