using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace LabelsOnFloor
{
    public class LabelPlacementHandler
    {
        private readonly FontHandler _fontHandler;

        private readonly LabelHolder _labelHolder;

        private int _nextUpdateTick;

        public LabelPlacementHandler(LabelHolder labelHolder, FontHandler fontHandler)
        {
            _labelHolder = labelHolder;
            _fontHandler = fontHandler;
        }

        public bool IsReady()
        {
            return _fontHandler.IsFontLoaded();
        }

        public void RegenerateIfNeeded()
        {
            var tick = Find.TickManager.TicksGame;
            if (tick < _nextUpdateTick)
                return;

            _nextUpdateTick = tick + 200;
            Regenerate();
        }

        private void Regenerate()
        {
            _labelHolder.Clear();
            var foundRooms = new HashSet<Room>();

            var map = Find.VisibleMap;
            var listerBuildings = map.listerBuildings;
            // Room roles are defined by buildings, so only need to check rooms with buildings
            foreach (var building in listerBuildings.allBuildingsColonist)
            {
                var room = GetRoomContainingBuildingIfRelevant(building, map);
                if (room == null)
                    continue;

                if (foundRooms.Contains(room))
                    continue;

                foundRooms.Add(room);
                _labelHolder.Add(
                    new Label()
                    {
                        LabelMesh = CreateMeshFor("A"),
                        Position = GetPanelTopLeftCornerForRoom(room, map)
                    }
                );
            }
        }

        private Mesh CreateMeshFor(string label)
        {
            Vector3[] array = new Vector3[4];
            Vector2[] array2 = new Vector2[4];
            var size = new Vector2
            {
                x = 0.5f,
                y = 1f
            };

            int[] array3 = new int[6];
            array[0] = new Vector3(-0.5f * size.x, 0f, -0.5f * size.y);
            array[1] = new Vector3(-0.5f * size.x, 0f, 0.5f * size.y);
            array[2] = new Vector3(0.5f * size.x, 0f, 0.5f * size.y);
            array[3] = new Vector3(0.5f * size.x, 0f, -0.5f * size.y);

            array2[0] = new Vector2(0.030f, 0f);
            array2[1] = new Vector2(0.030f, 1f);
            array2[2] = new Vector2(0.015f, 1f);
            array2[3] = new Vector2(0.015f, 0f);

            array3[0] = 0;
            array3[1] = 1;
            array3[2] = 2;
            array3[3] = 0;
            array3[4] = 2;
            array3[5] = 3;
            var mesh = new Mesh
            {
                name = "NewPlaneMesh()",
                vertices = array,
                uv = array2
            };
            mesh.SetTriangles(array3, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        //private static bool _first = true;
        public static IntVec3 GetPanelTopLeftCornerForRoom(Room room, Map map)
        {
            //return room.Cells.FirstOrDefault(c => IsInteriorCell(c, map));

            return room.Cells.FirstOrDefault(c => !map.thingGrid.CellContains(c, ThingDefOf.Wall));
            /*if (_first)
            {
                Main.Instance.Logger.Message($"Things at {cell}");
                var thingList = cell.GetThingList(map);
                foreach (var thing in thingList)
                {
                    Main.Instance.Logger.Message(thing.ToString());
                }
            }

            _first = false;

            return cell;*/


            /*
            foreach (var region in room.Regions)
            {
                var things = region.ListerThings.ThingsOfDef(ThingDefOf.Wall);
                if (_first)
                {
                    Main.Instance.Logger.Message($"Walls:");
                    foreach (var thing in things)
                    {
                        Main.Instance.Logger.Message($"thing.ToString() at {thing.Position}");
                    }
                }

                foreach (var cell in region.Cells)
                {
                    if (things.Any(t => t.Position == cell))
                        continue;

                    if (_first)
                    {
                        Main.Instance.Logger.Message($"Cell {cell} is ok");
                    }

                    _first = false;
                    return cell;
                }


            }

            return default;
            */
        }


        /*private static bool IsInteriorCell(IntVec3 cell, Map map)
        {
            var thingList = cell.GetThingList(map);
            foreach (var thing in thingList)
            {
                if (thing.def == ThingDefOf.Wall)
                {
                    Main.Instance.Logger.Message("wall found");
                    return false;

                }
                if (_first)
                {
                    Main.Instance.Logger.Message($"Thing: {thing}");
                }
            }

            Main.Instance.Logger.Message("First done");
            _first = false;
            return true;
        }*/

        // Filter for indoor rooms with a role
        public static Room GetRoomContainingBuildingIfRelevant(Building building, Map map)
        {
            if (building.Faction != Faction.OfPlayer)
                return null;

            if (building.Position.Fogged(map))
                return null;

            var room = building.Position.GetRoom(map);
            if (room == null || room.PsychologicallyOutdoors)
                return null;

            if (room.Role == RoomRoleDefOf.None)
                return null;

            return room;
        }

    }
}