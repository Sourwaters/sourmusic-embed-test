using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class CPHInline
{
    public bool Execute()
    {
        string value;
        if (!CPH.TryGetArg<string>("sourmusicState", out value) ||
            String.IsNullOrEmpty(value) || value.Length > 4096) return false;
        try
        {
            JObject state = JObject.Parse(value);
            state["at"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            CPH.SetGlobalVar("sourmusic.nowPlaying", state.ToString(Formatting.None), false);
            return true;
        }
        catch { return false; }
    }
}
