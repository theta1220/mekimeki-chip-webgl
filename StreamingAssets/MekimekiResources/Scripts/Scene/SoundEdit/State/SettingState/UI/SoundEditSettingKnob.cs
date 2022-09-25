using Sirius.Engine;

public class SoundEditSettingKnob
{
    public Position Root;
    public Position KbobPosition;
    public Sprite KnobSprite;
    public Sprite KnobValueSprite;
    public Sprite KnobBaseSprite;
    public Text ValueText;
    public Text NameText;
    public ToggleButton MuteButton;
    public ToggleButton SoloButton;
    public string Name;
    public ReferenceValue<double> Volume;
    public ReferenceValue<bool> Mute;
    public ReferenceValue<bool> Solo;
    public bool IsKnobClicking;

    public const int KnobWidth = 15;
    public const int KnobHeight = 25;
    public const int BaseHeight = 128 - KnobHeight;


    public SoundEditSettingKnob(string name,
        ReferenceValue<double> volume,
        ReferenceValue<bool> mute,
        ReferenceValue<bool> solo)
    {
        Root = new Position();
        KbobPosition = new Position();
        KbobPosition.Parent = Root;
        KnobBaseSprite = new Sprite("SoundEdit/knob_base.png", true);
        KnobBaseSprite.Position.Parent = Root;
        KnobSprite = new Sprite("SoundEdit/knob.png", false);
        KnobSprite.Position.Parent = KbobPosition;
        KbobPosition.Set(-5, -12);
        KnobValueSprite = new Sprite("SoundEdit/knob_value.png", true);
        KnobValueSprite.Position.Parent = Root;
        
        KnobValueSprite.Position.Set(
            -14, (int)(BaseHeight * 1.5) -8);
        ValueText = new Text(2);
        ValueText.Position.Parent = KnobValueSprite.Position;
        ValueText.SetText("000");
        ValueText.Position.Set(6, 6);
        NameText = new Text(2);
        NameText.Position.Parent = KnobValueSprite.Position;
        NameText.SetText(name);
        NameText.Position.Set(6, -10);
        Volume = volume;
        Mute = mute;
        Solo = solo;
        if (Mute != null)
        {
            MuteButton = new ToggleButton(
                "SoundEdit/mute_on.json", "SoundEdit/mute_off.json",
                new Point() { X = 11, Y = 11 }, Mute);
            MuteButton.Root.Parent = Root;
            MuteButton.Root.Set(-12, -17);
        }

        if (Solo != null)
        {
            SoloButton = new ToggleButton(
                "SoundEdit/solo_on.json", "SoundEdit/solo_off.json",
                new Point() { X = 11, Y = 11 }, Solo);
            SoloButton.Root.Parent = Root;
            SoloButton.Root.Set(1, -17);
        }
        IsKnobClicking = false;
    }

    public void Update()
    {
        MuteButton?.Update();
        SoloButton?.Update();
        var buttonPosX = 0;
        var buttonPosY = 0;
        var mousePosX = Input.Instance.GetMousePositionX();
        var mousePosY = Input.Instance.GetMousePositionY();
        KnobSprite.Position.GetWorldPosition(out buttonPosX, out buttonPosY);
        buttonPosX -= KnobWidth / 2;
        buttonPosY -= KnobHeight / 2;

        if (Input.Instance.MouseLeft.IsPushStartPure)
        {
            if (mousePosX >= buttonPosX &&
                mousePosY >= buttonPosY &&
                mousePosX <= buttonPosX + KnobWidth &&
                mousePosY <= buttonPosY + KnobHeight)
            {
                IsKnobClicking = true;
            }
        }

        if (!Input.Instance.MouseLeft.IsPush)
        {
            IsKnobClicking = false;
        }


        if (IsKnobClicking)
        {
            var basePosX = 0;
            var basePosY = 0;
            Root.GetWorldPosition(out basePosX, out basePosY);
            basePosX += KnobHeight / 2;
            basePosY += KnobHeight / 2;
            var diff = mousePosY - basePosY;
            var rate = (double)diff / BaseHeight;
            if (rate < 0)
            {
                rate = 0;
            }

            if (rate > 1.00)
            {
                rate = 1;
            }

            KnobSprite.Position.Point.Y = (int)(BaseHeight * rate) + KnobHeight / 2;
            Volume.Value = rate;
            ValueText.SetText($"{(Volume.Value * 100):000}");
        }
        else
        {
            var rate = Volume.Value;
            KnobSprite.Position.Point.Y = (int)(BaseHeight * rate) + KnobHeight / 2;
            ValueText.SetText($"{(Volume.Value * 100):000}");
        }
    }

    public void Draw()
    {
        KnobBaseSprite.Draw();
        KnobSprite.Draw();
        KnobValueSprite.Draw();
        ValueText.Draw();
        NameText.Draw();
        MuteButton?.Draw();
        SoloButton?.Draw();
    }
}