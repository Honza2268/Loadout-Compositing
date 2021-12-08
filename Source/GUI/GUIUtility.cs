﻿using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace Inventory
{
    [HotSwappable]
    public static class GUIUtility
    {
        public const float DEFAULT_HEIGHT = 22f;
        public const float SPACED_HEIGHT = 28f;

        // The icons are highlighted red if they are not fulfilled i.e. the pawn is
        // not wearing / does not have the item equipped.
        // [ TagName ] [icon_0] [icon_2]
        public static void DrawTag(Rect r, Tag tag)
        {
            var rect = r.TopPartPixels(SPACED_HEIGHT);
            var tagNameWidth = tag.name.GetWidthCached();
            Widgets.Label(rect.LeftPartPixels(tagNameWidth), tag.name);
            rect.AdjHorzBy(tagNameWidth + SPACED_HEIGHT);

            foreach (var td in tag.requiredItems)
            {
                Widgets.DefIcon(rect.PopLeftPartPixels(SPACED_HEIGHT), td.Def, td.RandomStuff);
            }
        }
        
        // returns whether the input field is in focus or not.
        public static bool InputField(Rect rect, string name, ref string buff, bool ShowName = false)
        {
            var inputFieldRect = rect;
            
            if (ShowName)
            {
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect.LeftPartPixels(name.GetWidthCached()), name);
                Text.Anchor = TextAnchor.UpperLeft;

                inputFieldRect = rect.RightPartPixels(rect.width - (name.GetWidthCached() + 3));
            }

            GUI.SetNextControlName(name);

            var style = Text.CurTextAreaStyle;
            style.alignment = TextAnchor.MiddleLeft;
            buff = GUI.TextField(inputFieldRect, buff, 999, style);

            var InFocus = GUI.GetNameOfFocusedControl() == name;
            
            if (Input.GetMouseButtonDown(0) && !Mouse.IsOver(inputFieldRect) && InFocus)
            {
                GUI.FocusControl(null);
            }

            return InFocus;
        }

        public static void AdjVertBy(this ref Rect rect, float value)
        {
            rect.y += value;
            rect.height -= value;
        }
        
        public static void AdjHorzBy(this ref Rect rect, float value)
        {
            rect.x += value;
            rect.width -= value;
        }

        public static Rect PopTopPartPixels(this ref Rect rect, float numPixels)
        {
            var ret = rect.TopPartPixels(numPixels);
            rect.AdjVertBy(numPixels);
            return ret;
        }
        
        public static Rect PopLeftPartPixels(this ref Rect rect, float numPixels)
        {
            var ret = rect.LeftPartPixels(numPixels);
            rect.AdjHorzBy(numPixels);
            return ret;
        }
        
        public static Rect PopRightPartPixels(this ref Rect rect, float numPixels)
        {
            var ret = rect.RightPartPixels(numPixels);
            rect.width -= numPixels;
            return ret;
        }

        public static bool DraggableCheckbox(Rect draggableRect, Rect checkBoxRect, ref bool state)
        {
            bool changed = false;
            // Draggable checkbox
            var draggableResult = Widgets.ButtonInvisibleDraggable(draggableRect);

            if (draggableResult == Widgets.DraggableResult.Pressed)
            {
                state = !state;
                changed = true;
            }
            else if (draggableResult == Widgets.DraggableResult.Dragged)
            {
                state = !state;
                changed = true;
                Widgets.checkboxPainting = true;
                Widgets.checkboxPaintingState = state;
            }

            if (Mouse.IsOver(draggableRect) && Widgets.checkboxPainting && Input.GetMouseButton(0) && state != Widgets.checkboxPaintingState)
            {
                state = Widgets.checkboxPaintingState;
                changed = true;
            }
            
            Widgets.CheckboxDraw(checkBoxRect.x, checkBoxRect.y, state, false, size: checkBoxRect.width);
            
            return changed;
        }

        public static void ListSeperator(ref Rect rect, string label)
        {
            rect.AdjVertBy(3f);
            GUI.color = Widgets.SeparatorLabelColor;
            Rect seperatorRect = new Rect(rect.x, rect.y, rect.width, 30f);
            Text.Anchor = TextAnchor.UpperLeft;
            Widgets.Label(seperatorRect, label);
            rect.AdjVertBy(20f);
            GUI.color = Widgets.SeparatorLineColor;
            Widgets.DrawLineHorizontal(rect.x, rect.y, rect.width);
            rect.AdjVertBy(12f);
            GUI.color = Color.white;
        }
        
        public static class DraggableHelper<T>
        {
            public static T curState = default(T);
        }
        
        // Modifier is a function or lambda which takes in 2 elements T, and returns a single element T
        // at the end of the call, `currentItem` will be equal to the returned element. The first parameter
        // is the value of the object where the mouse was initially pressed, and the second parameter is
        // the current value in `currentItem`
        public static void DraggableCopyPaste<T>(Rect allocatedRect, ref T currentItem, Func<T, T, T> modifier)
        {
            var copyRect = allocatedRect.LeftHalf();
            var pasteRect = allocatedRect.RightHalf();

            if (Widgets.ButtonImageFitted(copyRect, TexButton.Copy)) {
                DraggableHelper<T>.curState = currentItem;
            }
            
            var color = Color.white;
            if (DraggableHelper<T>.curState == null || Equals(DraggableHelper<T>.curState, currentItem)) {
                color = Color.gray;
            } else if (Mouse.IsOver(pasteRect)) {
                color = GenUI.MouseoverColor;
            }

            GUI.color = color;
            Widgets.DrawTextureFitted(pasteRect, TexButton.Paste, 1f);
            GUI.color = Color.white;

            if ( Mouse.IsOver(pasteRect) && Input.GetMouseButton(0) )
            {
                currentItem = modifier(DraggableHelper<T>.curState, currentItem);
            }
        }
    }


}