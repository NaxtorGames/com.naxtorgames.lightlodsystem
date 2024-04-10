using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace NaxtorGames.LightLODSystem.EditorScripts
{
    public static class LightLODSystemGameObjectCreation
    {
        [MenuItem("GameObject/Light/LOD/Point Light")]
        private static void CreateLODPointLight()
        {
            CreateLODLight(LightType.Point, out _);
        }

        [MenuItem("GameObject/Light/LOD/Spot Light")]
        private static void CreateLODSpotLight()
        {
            CreateLODLight(LightType.Spot, out _);
        }

        private static bool CreateLODLight(LightType lightType, out LightLODSourceMono lodLight)
        {
            GameObject currentSelectedGameObject = Selection.activeGameObject;

            Vector3 localSpawnPosition;
            Transform parent;

            if (currentSelectedGameObject != null)
            {
                parent = currentSelectedGameObject.transform;
                localSpawnPosition = Vector3.zero;
            }
            else
            {
                PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage != null && prefabStage.prefabContentsRoot != null)
                {
                    parent = prefabStage.prefabContentsRoot.transform;
                    localSpawnPosition = Vector3.zero;
                }
                else
                {
                    parent = null;
                    TryGetPointInfrontSceneCamera(out localSpawnPosition);
                }
            }

            if (LightLODSourceMono.CreateLODLight(lightType, null, out lodLight, true,false))
            {
                GameObject lodLightGameObject = lodLight.gameObject;
                Transform lodLightTransform = lodLightGameObject.transform;
                lodLightTransform.parent = parent;
                lodLightTransform.SetLocalPositionAndRotation(localSpawnPosition, Quaternion.identity);

                EditorUtility.SetDirty(lodLightGameObject);
                Selection.activeGameObject = lodLightGameObject;
            }

            return lodLight != null;
        }

        /// <summary>
        /// Gets a position infront of the first scene camera.
        /// <para>Depending if the camera looks at a collider the position is the point where it hits or the camera position + its forward direction.</para>
        /// </summary>
        /// <param name="point">The world space position of the scene cameras forward direction</param>
        /// <returns>If a scene camera was found.</returns>
        private static bool TryGetPointInfrontSceneCamera(out Vector3 point)
        {
            Camera[] sceneCameras = SceneView.GetAllSceneCameras();
            if (sceneCameras != null && sceneCameras.Length > 0)
            {
                Transform sceneCameraTransform = sceneCameras[0].transform;
                if (Physics.Raycast(sceneCameraTransform.position, sceneCameraTransform.forward, out RaycastHit hitInfo))
                {
                    point = hitInfo.point;
                }
                else
                {
                    point = sceneCameraTransform.position + sceneCameraTransform.forward;
                }
                return true;
            }

            point = Vector3.zero;
            return false;
        }
    }
}
