using System.Linq;
using Harmony;
using UnityEngine;
using Verse;

namespace LabelsOnFloor
{
    [HarmonyPatch(typeof(DynamicDrawManager), "DrawDynamicThings")]
    public class DynamicDrawManager_DrawDynamicThings_Patch
    {
        private static Mesh _testMesh;

        private static bool _firstRun = true;

        static bool Prefix(ref DynamicDrawManager __instance)
        {
            var mat = Main.Instance._fontHandler.GetMaterial();
                if (_testMesh == null)
                    CreateTestMesh();
                var s = new Vector3(1f, 1f, 1f);
            CellRect currentViewRect = Find.CameraDriver.CurrentViewRect;
            Main.Instance._labelPlacementHandler.RegenerateIfNeeded();
            foreach (var label in Main.Instance._labelHolder.GetLabels())
            //foreach (IntVec3 sectionCell in currentViewRect)
            {
                //var room = sectionCell.GetRoom(Find.VisibleMap, RegionType.Set_All);
                //if (room == null || room.PsychologicallyOutdoors)
                //    continue;


                Matrix4x4 matrix = default;
                var pos = label.Position.ToVector3();
                //var pos = sectionCell.ToVector3();


                if (_firstRun)
                {
                    Main.Instance.Logger.Message($"Drawing at Pos: {pos}");
                }
                pos.x += .5f;
                pos.z += .5f;
                matrix.SetTRS(pos, Quaternion.identity, s);

                Graphics.DrawMesh(_testMesh, matrix, mat, 0);
            }

            _firstRun = false;
            return true;
        }

        public static void CreateTestMesh()
        {
            Vector3[] array = new Vector3[4];
            Vector2[] array2 = new Vector2[4];
            var size = new Vector2
            {
                x = 1f,
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
            _testMesh = new Mesh
            {
                name = "NewPlaneMesh()",
                vertices = array,
                uv = array2
            };
            _testMesh.SetTriangles(array3, 0);
            _testMesh.RecalculateNormals();
            _testMesh.RecalculateBounds();
        }

    }

}