using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace LabelsOnFloor
{
    public class LabelPlacementHandler
    {
        private readonly FontHandler _fontHandler = new FontHandler();

        private readonly LabelHolder labelHolder;
        
        private int _nextUpdateTick;

        public LabelPlacementHandler(LabelHolder labelHolder)
        {
            this.labelHolder = labelHolder;
        }

        public bool IsReady()
        {
            return _fontHandler.IsFontLoaded();
        }

        public void Regenerate()
        {
            labelHolder.Clear();
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
                labelHolder.Add(
                    new Label()
                    {
                        LabelMesh = CreateMeshFor("A"),
                        Position = GetPanelTopLeftCornerForRoom(room, map)
                    });
            }
        }

        public void RegenerateIfNeeded()
        {
            var tick = Find.TickManager.TicksGame;
            if (tick < _nextUpdateTick)
                return;

            _nextUpdateTick = tick + 200;
            Regenerate();
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

        public static IntVec3 GetPanelTopLeftCornerForRoom(Room room, Map map)
        {
            var bestCell = room.BorderCells.First();
            foreach (var cell in room.BorderCells)
            {
                if (cell.x < bestCell.x || cell.z > bestCell.z)
                    bestCell = cell;
            }

            var possiblyBetterCell = bestCell;
            possiblyBetterCell.x++;
            possiblyBetterCell.z--;
            if (possiblyBetterCell.GetRoom(map) == room)
                bestCell = possiblyBetterCell;

            return bestCell;
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