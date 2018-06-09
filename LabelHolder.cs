using System.Collections.Generic;
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
        public object AssociatedObject;

        public bool IsValid()
        {
            return LabelPlacementData != null && LabelMesh != null && AssociatedObject != null;
        }
    }

    public class LabelHolder
    {
        private readonly List<Label> _currentLabels = new List<Label>();

        public void Clear()
        {
            _currentLabels.Clear();
        }

        public void Add(Label label)
        {
            _currentLabels.Add(label);
        }

        public void RemoveLabelForObject(object objectToRemove)
        {
            _currentLabels.RemoveAll(l => l.AssociatedObject == objectToRemove);
        }

        public IEnumerable<Label> GetLabels()
        {
            return _currentLabels;
        }
    }
}