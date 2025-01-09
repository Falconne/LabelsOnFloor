using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace LabelsOnFloor
{
    public struct CharBoundsInTexture
    {
        public float Left, Right;
    }

    public class FontHandler
    {
        private float _charWidthAsTexturePortion = -1f;

        private Material _material;

        public bool IsFontLoaded()
        {
            if (Resources.Font == null)
                return false;

            if (_charWidthAsTexturePortion < 0f)
                _charWidthAsTexturePortion =  35f / Resources.Font.width;

            return true;
        }

        public Material GetMaterial()
        {
            if (_material == null)
            {
                var color = (Main.Instance.UseLightText()) ? Color.white : Color.black;
                color.a = Main.Instance.GetOpacity();
                _material = MaterialPool.MatFrom(Resources.Font, ShaderDatabase.Transparent, color);
            }

            return _material;
        }

        public void Reset()
        {
            _material = null;
        }

        public IEnumerable<CharBoundsInTexture> GetBoundsInTextureFor(string text)
        {
            foreach (char c in text)
            {
                yield return GetCharBoundsInTextureFor(c);
            }
        }

        private CharBoundsInTexture GetCharBoundsInTextureFor(char c)
        {
            var index = GetIndexInFontForChar(c);
            var left = index * _charWidthAsTexturePortion;
            return new CharBoundsInTexture()
            {
                Left = left,
                Right = left + _charWidthAsTexturePortion
            };
        }

        private int GetIndexInFontForChar(char c)
        {
            var asciiVal = (int) c;
            var cyrillicModifier = 1039;
            if (asciiVal < 33)
                return 0;

            if (asciiVal < 97)
                return asciiVal - 32;
            
            if (asciiVal < 127)
                return asciiVal - 58;

            if (asciiVal == 1025) //For letter "Ё" as an exception
                return 75;

            if (asciiVal < 1046 && asciiVal > 1039) //Before "Ё"
                return asciiVal - cyrillicModifier + 68;

            if (asciiVal < 1072 && asciiVal > 1045) //After "Ё"
                return asciiVal - cyrillicModifier + 68 + 1;

            return 0;
        }
    }
}