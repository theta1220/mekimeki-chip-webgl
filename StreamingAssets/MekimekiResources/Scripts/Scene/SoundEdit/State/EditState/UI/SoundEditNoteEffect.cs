
using System;
using System.Collections.Generic;
using Sirius.Engine;

public class SoundEditNoteEffect
{
    public Position Root;
    public SrSprite EffectSprite;
    public List<Effect> Effects;

    public SoundEditNoteEffect(Position parent)
    {
        Root = parent;
        EffectSprite = new SrSprite();
        EffectSprite.Position.Parent = parent;
        EffectSprite.Image = ResourceManager.Instance.LoadFromJson<Image>("SoundEdit/white.json");
        Effects = new List<Effect>();
    }
    
    public class Effect
    {
        public Position Root;

        public double PosX;
        public double PosY;
        public double JumpPower;
        public double MovePower;
        public double FallPower;
        public int Life;

        public Effect()
        {
            Root = new Position();
            JumpPower = Random.Next(500, 500) / 100.0;
            MovePower = Random.Next(500, 600) / 100.0;
            FallPower = Random.Next(10, 15) / 100.0;
            Life = Random.Next(30, 60);
        }

        public void Update()
        {
            JumpPower -= FallPower;
            PosX += MovePower;
            PosY += JumpPower;
            Root.Point.X = (int)PosX;
            Root.Point.Y = (int)PosY;
            Life--;
        }
    }

    public void Emit(int x, int y)
    {
        var effect = new Effect();
        effect.PosX = x + 5;
        effect.PosY = y + 5;
        effect.JumpPower *= 0.01 + (y / (double)ScreenHandler.Height);
        Effects.Add(effect);
    }

    public void Update()
    {
        foreach (var effect in Effects.ToArray())
        {
            effect.Update();
            if (effect.Life < 0)
            {
                Effects.Remove(effect);
            }
        }
    }

    public void Draw()
    {
        foreach (var effect in Effects)
        {
            EffectSprite.Position.Set(effect.Root.Point.X, effect.Root.Point.Y);
            EffectSprite.Draw();
        }
    }
}