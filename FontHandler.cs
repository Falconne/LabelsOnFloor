using System.Collections.Generic;

namespace RoomSense
{
    public struct CharBoundsInTexture
    {
        public float Left, Right;
    }

    public class FontHandler
    {
        private float _charWidthAsTexturePortion = -1f;

        public bool IsFontLoaded()
        {
            if (Resources.Font == null)
                return false;

            if (_charWidthAsTexturePortion < 0f)
                _charWidthAsTexturePortion =  15f / Resources.Font.width;

            return true;
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
            if (asciiVal < 33)
                return 0;

            if (asciiVal < 97)
                return asciiVal - 32;
            
            // Convert lower case to upper
            if (asciiVal < 123)
                return asciiVal - 65;

            if (asciiVal < 126)
                return asciiVal - 122 + 63;

            return 0;
        }
    }
}