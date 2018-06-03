using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace LabelsOnFloor
{
    public class Label
    {
        public Mesh LabelMesh;
        public IntVec3 Position;
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
    }
}