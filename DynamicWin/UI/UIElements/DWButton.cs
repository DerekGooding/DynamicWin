﻿using DynamicWin.Utils;

namespace DynamicWin.UI.UIElements
{
    public class DWButton : UIObject
    {
        // Button Color

        public Col normalColor = Theme.IconColor.Override(a: 0.15f);
        public Col hoverColor = Theme.IconColor.Override(a: 0.25f);
        public Col clickColor = Theme.IconColor.Override(a: 0.65f);

        public float colorSmoothingSpeed = 15f;

        // Button Size

        protected Vec2 initialScale;

        public Vec2 normalScaleMulti = Vec2.one * 1f;
        public Vec2 hoverScaleMulti = Vec2.one * 1.05f;
        public Vec2 clickScaleMulti = Vec2.one * 1f - 0.05f;

        protected Vec2 scaleMultiplier = Vec2.one;

        public SecondOrder scaleSecondOrder;

        // Events

        public Action clickCallback;

        public DWButton(UIObject? parent, Vec2 position, Vec2 size, Action clickCallback, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, size, alignment)
        {
            initialScale = size;

            roundRadius = 5f;
            scaleSecondOrder = new SecondOrder(size, 4.5f, 0.45f, 0.15f);
            this.clickCallback = clickCallback;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            Vec2 currentSize = initialScale;
            scaleMultiplier = Vec2.one;

            if (IsHovering && !IsMouseDown)
                scaleMultiplier *= hoverScaleMulti;
            else if (IsMouseDown)
                scaleMultiplier *= clickScaleMulti;
            else if (!IsHovering && !IsMouseDown)
                scaleMultiplier *= normalScaleMulti;
            else
                scaleMultiplier *= normalScaleMulti;

            currentSize *= scaleMultiplier;

            Size = scaleSecondOrder.Update(deltaTime, currentSize);

            if (IsHovering && !IsMouseDown)
                Color = Col.Lerp(Color, GetColor(hoverColor), colorSmoothingSpeed * deltaTime);
            else if (IsMouseDown)
                Color = Col.Lerp(Color, GetColor(clickColor), colorSmoothingSpeed * deltaTime);
            else if (!IsHovering && !IsMouseDown)
                Color = Col.Lerp(Color, GetColor(normalColor), colorSmoothingSpeed * deltaTime);
            else
                Color = Col.Lerp(Color, GetColor(normalColor), colorSmoothingSpeed * deltaTime);
        }

        public override void OnMouseUp()
        {
            clickCallback?.Invoke();
        }
    }
}