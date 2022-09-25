using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Sirius.Engine;

public class Entry
{
    public static async UniTask Main()
    {
        ScreenHandler.SetFrameRate(60);
        Chara.Initialize();
        var main = new Main();

        while (!Sirius.Engine.ApplicationExiter.Instance.IsExit)
        {
            try
            {
                main.Update();
                main.Draw();
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                Logger.Error(e.StackTrace);
                throw;
            }

            if (Sirius.Engine.Input.Instance.F12.IsPushEnd)
            {
                while (Sirius.Engine.Input.Instance.AnyButtonIsPush)
                {
                    await UniTask.Yield();
                }

                Sirius.Engine.Logger.Info("スクリプト終了したいです");
                Sirius.Engine.ScreenHandler.Draw();
                Sirius.Engine.ApplicationExiter.Instance.ApplicationExit();
            }

            await UniTask.WaitForFixedUpdate();
        }
    }
}