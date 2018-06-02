using UnityEngine;
using Verse;

namespace RoomSense
{
    [StaticConstructorOnStartup]
    public class Resources
    {
        public static Texture2D Font = ContentFinder<Texture2D>.Get("AnonPro");
    }
}