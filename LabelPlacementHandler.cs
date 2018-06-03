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
            var vertices = new List<Vector3>();
            var uvMap = new List<Vector2>();
            var size = new Vector2
            {
                x = 0.5f,
                y = 1f
            };

            var triangles = new List<int>();
            vertices.Add(new Vector3(-0.5f * size.x, 0f, -0.5f * size.y));
            vertices.Add(new Vector3(-0.5f * size.x, 0f, 0.5f * size.y));
            vertices.Add(new Vector3(0.5f * size.x, 0f, 0.5f * size.y));
            vertices.Add(new Vector3(0.5f * size.x, 0f, -0.5f * size.y));

            uvMap.Add(new Vector2(0.030f, 0f));
            uvMap.Add(new Vector2(0.030f, 1f));
            uvMap.Add(new Vector2(0.015f, 1f));
            uvMap.Add(new Vector2(0.015f, 0f));

            triangles.Add(0);
            triangles.Add(1);
            triangles.Add(2);
            triangles.Add(0);
            triangles.Add(2);
            triangles.Add(3);
            var mesh = new Mesh
            {
                name = "NewPlaneMesh()",
                vertices = vertices.ToArray(),
                uv = uvMap.ToArray()
            };
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        public static IntVec3 GetPanelTopLeftCornerForRoom(Room room, Map map)
        {
            return room.Cells.FirstOrDefault(c => !map.thingGrid.CellContains(c, ThingDefOf.Wall));
        }

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