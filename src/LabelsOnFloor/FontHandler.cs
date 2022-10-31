using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using Verse;

namespace LabelsOnFloor
{
    public struct CharBoundsInTexture
    {
        public string FontBitmapName;
        public Rect Rect;
    }

    public class FontHandler
    {
        private readonly Dictionary<string, Material> materials = new();

        public bool IsFontLoaded()
        {
            return !Resources.FontBitmaps.NullOrEmpty() && Resources.FontBitmaps["Consolas"] != null;
        }

        public Material GetMaterial(string fontmap)
        {
            if (!materials.ContainsKey(fontmap))
            {
                var color = Main.Instance.UseLightText() ? Color.white : Color.black;
                color.a = Main.Instance.GetOpacity();
                materials.Add( fontmap, MaterialPool.MatFrom(Resources.FontBitmaps[fontmap], ShaderDatabase.Transparent, color) );
            }

            return materials[fontmap];
        }

        public void Reset()
        {
            materials.Clear();
        }

        public IEnumerable<CharBoundsInTexture> GetBoundsInTextureFor(string text)
        {
            // convert any composite chars into as many single graphemes as possible
            text = text.Normalize();

            List<CharOffset> offsets = new();
            foreach (char c in text) {
                var co = GetOffsetsForChar(c);
                offsets.Add(co);
            }

            // If more than one texture is used, switch to the Combined version
            bool useCombined = offsets.Select(co => co.FontBitmapName).Distinct().Count() > 1;
            foreach (CharOffset c in offsets) yield return GetCharBoundsInTextureFor(c, useCombined);
        }

        private CharBoundsInTexture GetCharBoundsInTextureFor(CharOffset co, bool useCombined)
        {
            string fontmap = co.FontBitmapName;
            var dimensions = Resources.FontBitmapDimensions[fontmap];

            if (useCombined) return new CharBoundsInTexture()
            {
                FontBitmapName = "Combined",
                Rect = new()
                {
                    x      = (float)(dimensions.BlockRectInCombinedProportions.x + co.Column * dimensions.CharWidthCombinedProportion),
                    // y coordinate is bottom-up, and we have to get the bottom-corner, not the top
                    y      = 1 - (float)(dimensions.BlockRectInCombinedProportions.y + (co.Row+1) * dimensions.CharHeightCombinedProportion),
                    width  = (float)dimensions.CharWidthCombinedProportion,
                    height = (float)dimensions.CharHeightCombinedProportion,
                }
            };

            return new CharBoundsInTexture()
            {
                FontBitmapName = fontmap,
                Rect = new()
                {
                    x      = (float)(co.Column * dimensions.CharWidthProportion),
                    // ditto
                    y      = 1 - (float)((co.Row+1) * dimensions.CharHeightProportion),
                    width  = (float)dimensions.CharWidthProportion,
                    height = (float)dimensions.CharHeightProportion,
                }
            };
        }

        private CharOffset GetOffsetsForChar(char c)
        {
            int codepoint = c;

            if (Resources.FontBitmapOffsets.ContainsKey(codepoint)) return Resources.FontBitmapOffsets[codepoint];

            // Look for the nearest offset and calculate the exact point
            int nearestCP     = Resources.FontBitmapOffsets.Keys.Where( cp => cp <= codepoint ).Max();  // highest without going over
            var nearestOffset = Resources.FontBitmapOffsets[nearestCP];

            // Snake eyes... use a less accurate form (via NFKC) and try again
            if (nearestOffset.Row == -1)
            {
                char nfkcChar = c.ToString().Normalize(NormalizationForm.FormKC)[0];
                if (nfkcChar != c) return Resources.FontBitmapOffsets[codepoint] = GetOffsetsForChar(nfkcChar);

                // Otherwise, just set and return the default space
                return Resources.FontBitmapOffsets[codepoint] = Resources.DefaultSpace;
            }

            // Calculate the right offset in the sequence
            var dimensions = Resources.FontBitmapDimensions[nearestOffset.FontBitmapName];

            int colCnt = dimensions.ColumnCount;
            int diff   = codepoint - nearestCP;
            int offRow = Math.DivRem(nearestOffset.Column + diff, colCnt, out int newCol);
            int newRow = nearestOffset.Row + offRow;

            // This shouldn't happen
            if (newRow > dimensions.RowCount) {
                Main.Instance.Logger.Error("Row out-of-bounds error fetching codepoint {0}, using {1} as reference in {2}", codepoint, nearestCP, nearestOffset.FontBitmapName);

                // This will only alert once because of the caching
                return Resources.FontBitmapOffsets[codepoint] = Resources.DefaultSpace;
            }

            // Cache the result and return it
            return Resources.FontBitmapOffsets[codepoint] = new() { FontBitmapName = nearestOffset.FontBitmapName, Row = newRow, Column = newCol };
        }
    }
}