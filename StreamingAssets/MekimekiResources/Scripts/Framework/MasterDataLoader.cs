using System.Collections.Generic;
using System.Linq;

public class MasterDataLoader : Singleton<MasterDataLoader>
{
    public Dictionary<string, Dictionary<string, string>> Load(string sheet)
    {
        var text = ResourceManager.Instance.LoadText($"Master/Master - {sheet}.csv");
        if (string.IsNullOrEmpty(text)) return null;

        text = text.Replace("\r", "");
        var res = new Dictionary<string, Dictionary<string, string>>();
        var width = text.Split('\n').First().Split(',').Length;
        var cells = GetCells(text);
        var headers = cells.ToList().GetRange(0, width).ToArray();

        var dict = new Dictionary<string, string>();
        for (var i = width; i < cells.Count; i++)
        {
            var cell = cells[i];
            var header = headers[i % width];
            dict.Add(header, cell);

            if (i % width == width - 1 || i + 1 == cells.Count)
            {
                res.Add(dict["Id"], dict);
                dict = new Dictionary<string, string>();
            }
        }

        return res;
    }

    public List<string> GetCells(string text)
    {
        bool inText = false;
        List<string> res = new List<string>();
        string buf = "";
        foreach (var c in text)
        {
            if (c == '"')
            {
                inText = !inText;
                continue;
            }

            if (c == ',' || c == '\n' && !inText)
            {
                res.Add(buf);
                buf = "";
                continue;
            }

            buf += c;
        }

        if (!string.IsNullOrEmpty(buf))
        {
            res.Add(buf);
        }

        return res;
    }
}

