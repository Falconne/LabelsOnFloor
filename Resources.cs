using UnityEngine;
using Verse;

namespace LabelsOnFloor
{
    [StaticConstructorOnStartup]
    public class Resources
    {
        public static Texture2D Font = ContentFinder<Texture2D>.Get("Consolas");
    }
}