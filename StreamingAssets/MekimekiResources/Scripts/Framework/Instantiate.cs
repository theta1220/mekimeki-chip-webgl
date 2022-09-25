using System;
using Sirius.Engine;

public class Instantiate
{
    public static T Create<T>(string id) where T : class
    {
        var type = Type.GetType($"Submission#0+{id}");
        if (type == null)
        {
            Logger.Error($"{id}というクラスが存在しませんでした");
            return null;
        }

        var obj = Activator.CreateInstance(type) as T;
        if (obj == null)
        {
            Logger.Error($"インスタンス {id}の生成に失敗しました");
            return null;
        }
        return obj;
    }
}
