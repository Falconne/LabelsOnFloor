using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace LabelsOnFloor
{
    public class PlacementData
    {
        public IntVec3 Position;
        public Vector3 Scale;
        public bool Flipped = false;
    }

    public class Label
    {
        public Mesh LabelMesh;
        public PlacementData LabelPlacementData;
        public object AssociatedArea;
        public bool IsZone = false;

        public bool IsValid()
        {
            return LabelPlacementData != null && LabelMesh != null && AssociatedArea != null;
        }
    }

    public class LabelHolder
    {
        private readonly List<Label> _currentLabels = new List<Label>();

        private bool _dirty = true;

        public void Clear()
        {
            _currentLabels.Clear();
        }

        public void Add(Label label)
        {
            _currentLabels.Add(label);
            _dirty = true;
        }

        public void RemoveLabelForArea(object area)
        {
            _currentLabels.RemoveAll(l => l.AssociatedArea == area);
            _dirty = true;
        }

        public IEnumerable<Label> GetLabels()
        {
            RemoveRoomsWithZones();
            return _currentLabels;
        }

        private void RemoveRoomsWithZones()
        {
            if (!_dirty)
                return;

            _dirty = false;
            var map = Find.VisibleMap;

            var roomsToRemove = new HashSet<Room>();

            var labelledRooms = new HashSet<Room>(
                _currentLabels
                    .Where(l => l.AssociatedArea is Room)
                    .Select(l => l.AssociatedArea as Room)
                );

            foreach (var label in _currentLabels)
            {
                if (!label.IsZone)
                    continue;

                foreach (var cell in (label.AssociatedArea as Zone).Cells)
                {
                    var roomWithCell = cell.GetRoom(map);
                    if (roomWithCell == null || roomWithCell.Role == RoomRoleDefOf.None)
                        continue;

                    if (labelledRooms.Contains(roomWithCell))
                        roomsToRemove.Add(roomWithCell);

                    break;
                }
            }

            foreach (var room in roomsToRemove)
            {
                _currentLabels.RemoveAll(l => l.AssociatedArea == room);
            }
        }
    }
}