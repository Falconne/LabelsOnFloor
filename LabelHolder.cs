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

            if (!Main.Instance.ShowRoomNames() || !Main.Instance.ShowZoneNames())
                return;

            var map = Find.VisibleMap;

            var roomsToRemove = new HashSet<Room>();

            var labelledRooms = new HashSet<Room>(
                _currentLabels
                    .Where(l => !l.IsZone)
                    .Select(l => l.AssociatedArea as Room)
                );

            foreach (var label in _currentLabels)
            {
                if (!label.IsZone)
                    continue;

                // Assume we don't need to handle zones that are both inside and outside a room,
                // so we only need to test the location of one cell
                var zone = label.AssociatedArea as Zone;
                if (zone.Cells.Count < 1)
                    continue;

                var roomWithCell = RoomRoleFinder.GetRoomAtLocation(zone.Cells.First(), map);
                if (roomWithCell == null)
                    continue;

                if (labelledRooms.Contains(roomWithCell))
                    roomsToRemove.Add(roomWithCell);
            }

            foreach (var room in roomsToRemove)
            {
                _currentLabels.RemoveAll(l => l.AssociatedArea == room);
            }
        }
    }
}