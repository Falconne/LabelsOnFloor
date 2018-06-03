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

        private readonly Dictionary<string, Mesh> _cachedMeshes = new Dictionary<string, Mesh>();

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
                var text = "EFe";
                var label = new Label()
                {
                    LabelMesh = GetMeshFor(text),
                    LabelPlacementData = GetLabelPlacementDataForRoom(room, map, text.Length)
                };

                if (label.LabelPlacementData != null)
                    _labelHolder.Add(label);
            }
        }

        public Mesh GetMeshFor(string label)
        {
            if (!_cachedMeshes.ContainsKey(label))
            {
                _cachedMeshes[label] = CreateMeshFor(label);
            }

            return _cachedMeshes[label];
        }

        private Mesh CreateMeshFor(string label)
        {
            var vertices = new List<Vector3>();
            var uvMap = new List<Vector2>();
            var triangles = new List<int>();
            var size = new Vector2
            {
                x = 0.5f,
                y = 1f
            };

            var boundsInTexture = _fontHandler.GetBoundsInTextureFor(label);
            var startingTriangleVertex = 0;
            var startingVertexXOffset = 0f;
            foreach (var charBoundsInTexture in boundsInTexture)
            {
                vertices.Add(new Vector3(startingVertexXOffset + -0.5f * size.x, 0f, -0.5f * size.y));
                vertices.Add(new Vector3(startingVertexXOffset + -0.5f * size.x, 0f, 0.5f * size.y));
                vertices.Add(new Vector3(startingVertexXOffset + 0.5f * size.x, 0f, 0.5f * size.y));
                vertices.Add(new Vector3(startingVertexXOffset + 0.5f * size.x, 0f, -0.5f * size.y));
                startingVertexXOffset += size.x;

                uvMap.Add(new Vector2(charBoundsInTexture.Left, 0f));
                uvMap.Add(new Vector2(charBoundsInTexture.Left, 1f));
                uvMap.Add(new Vector2(charBoundsInTexture.Right, 1f));
                uvMap.Add(new Vector2(charBoundsInTexture.Right, 0f));

                triangles.Add(startingTriangleVertex + 0);
                triangles.Add(startingTriangleVertex + 1);
                triangles.Add(startingTriangleVertex + 2);
                triangles.Add(startingTriangleVertex + 0);
                triangles.Add(startingTriangleVertex + 2);
                triangles.Add(startingTriangleVertex + 3);
                startingTriangleVertex += 4;
            }

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

        public static PlacementData GetLabelPlacementDataForRoom(Room room, Map map, int labelLength)
        {
            /*var interiorCells = room.Cells.Where(c => !map.thingGrid.CellContains(c, ThingDefOf.Wall));
            var rows = new Dictionary<int, List<IntVec3>>();
            foreach (var cell in interiorCells)
            {
                if (!rows.ContainsKey(cell.x))
                {
                    rows[cell.x] = new List<IntVec3>();
                }
                rows[cell.y].Add(cell);
            }*/

            var lastRowCells = new List<IntVec3>();
            var lastRowFound = -1;
            foreach (var cell in room.Cells)
            {
                if (map.thingGrid.CellContains(cell, ThingDefOf.Wall))
                    continue;

                if (cell.z > lastRowFound)
                {
                    lastRowFound = cell.z;
                    lastRowCells.Clear();
                }

                lastRowCells.Add(cell);
            }

            if (lastRowCells.Count == 0)
                return null;

            var scaling = (float) lastRowCells.Count / labelLength;
            lastRowCells.Sort((c1, c2) => c1.x.CompareTo(c2.x));

            return new PlacementData
            {
                Position = lastRowCells.First(),
                Scale = new Vector3(scaling, 1f, scaling)
            };
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