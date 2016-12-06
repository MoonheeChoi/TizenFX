/*
 * Copyright (c) 2016 Samsung Electronics Co., Ltd All Rights Reserved
 *
 * Licensed under the Apache License, Version 2.0 (the License);
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an AS IS BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;

namespace ElmSharp
{
    public abstract class Widget : EvasObject
    {
        Dictionary<string, EvasObject> _partContents = new Dictionary<string, EvasObject>();

        SmartEvent _focused;
        SmartEvent _unfocused;

        internal Color _backgroundColor = Color.Default;

        protected Widget()
        {
        }

        protected Widget(EvasObject parent) : base(parent)
        {
            _focused = new SmartEvent(this, "focused");
            _focused.On += (s, e) => Focused?.Invoke(this, EventArgs.Empty);

            _unfocused = new SmartEvent(this, "unfocused");
            _unfocused.On += (s, e) => Unfocused?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Focused;

        public event EventHandler Unfocused;

        public bool IsEnabled
        {
            get
            {
                return !Interop.Elementary.elm_object_disabled_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_object_disabled_set(Handle, !value);
            }
        }

        public string Style
        {
            get
            {
                return Interop.Elementary.elm_object_style_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_object_style_set(Handle, value);
            }
        }

        public bool IsFocused
        {
            get
            {
                return Interop.Elementary.elm_object_focus_get(Handle);
            }
        }

        public virtual string Text
        {
            get
            {
                return Interop.Elementary.elm_object_part_text_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_object_part_text_set(Handle, IntPtr.Zero, value);
            }
        }

        public virtual Color BackgroundColor
        {
            get
            {
                if(!_backgroundColor.IsDefault)
                {
                    _backgroundColor = GetPartColor("bg");
                }
                return _backgroundColor;
            }
            set
            {
                if (value.IsDefault)
                {
                    Console.WriteLine("Widget instance doesn't support to set BackgroundColor to Color.Default.");
                }
                else
                {
                    SetPartColor("bg", value);
                    _backgroundColor = value;
                }
            }
        }

        public void SetFocus(bool isFocus)
        {
            Interop.Elementary.elm_object_focus_set(Handle, isFocus);
        }

        public void SetPartContent(string part, EvasObject content)
        {
            SetPartContent(part, content, false);
        }

        public void SetPartContent(string part, EvasObject content, bool preserveOldContent)
        {
            if (preserveOldContent)
            {
                Interop.Elementary.elm_object_part_content_unset(Handle, part);
            }
            Interop.Elementary.elm_object_part_content_set(Handle, part, content);

            _partContents[part ?? "__default__"] = content;
        }

        public void SetContent(EvasObject content)
        {
            SetContent(content, false);
        }

        public void SetContent(EvasObject content, bool preserveOldContent)
        {
            if (preserveOldContent)
            {
                Interop.Elementary.elm_object_content_unset(Handle);
            }

            Interop.Elementary.elm_object_content_set(Handle, content);
            _partContents["___default__"] = content;
        }

        public void SetPartText(string part, string text)
        {
            Interop.Elementary.elm_object_part_text_set(Handle, part, text);
        }

        public string GetPartText(string part)
        {
            return Interop.Elementary.elm_object_part_text_get(Handle, part);
        }

        public void SetPartColor(string part, Color color)
        {
            Interop.Elementary.elm_object_color_class_color_set(Handle, part, color.R * color.A / 255,
                                                                              color.G * color.A / 255,
                                                                              color.B * color.A / 255,
                                                                              color.A);
        }

        public Color GetPartColor(string part)
        {
            int r, g, b, a;
            Interop.Elementary.elm_object_color_class_color_get(Handle, part, out r, out g, out b, out a);
            return new Color((int)(r / (a / 255.0)), (int)(g / (a / 255.0)), (int)(b / (a / 255.0)), a);
        }

        internal IntPtr GetPartContent(string part)
        {
            return Interop.Elementary.elm_object_part_content_get(Handle, part);
        }
    }
}
