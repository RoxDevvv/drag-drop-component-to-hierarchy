
using System;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class HierarchyDragDropHandler
{
    static HierarchyDragDropHandler()
    {
        DragAndDrop.AddDropHandler(OnComponentDrop);
    }

    private static DragAndDropVisualMode OnComponentDrop(int dragInstanceId,
    HierarchyDropFlags dropFlag, Transform parent, bool perform)
    {
        MonoScript monoScript = GetComponentScript();

        if (monoScript == null)
            return DragAndDropVisualMode.None;
            
        if (perform)
        {
            GameObject gameObject = CreateAndRename(monoScript.name);
            gameObject.AddComponent(monoScript.GetClass());
        }

        return DragAndDropVisualMode.Copy;
    }
    static MonoScript GetComponentScript()
    {
        foreach (var DraggedObject in DragAndDrop.objectReferences)
        {
            if (DraggedObject is MonoScript monoScript)
            {
                Type scriptType = monoScript.GetClass();
                if (scriptType != null && scriptType.IsSubclassOf(typeof(MonoBehaviour)))
                {
                    return monoScript;
                }
            }
        }

        return null;
    }
    public static GameObject CreateAndRename(string name)
    {
        GameObject gameObject = new GameObject(name);

        if (Selection.activeGameObject)
        {
            gameObject.transform.parent = Selection.activeGameObject.transform;
            gameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        Selection.activeGameObject = gameObject;

        Undo.RegisterCreatedObjectUndo(gameObject, "Created game object");

        EditorApplication.delayCall += () =>
        {
            Type sceneHeirarchyWindowType = Type.GetType("UnityEditor.SceneHierarchyWindow, UnityEditor");
            EditorWindow.GetWindow(sceneHeirarchyWindowType).SendEvent(EditorGUIUtility.CommandEvent("Rename"));
        };

        return gameObject;
    }
}
