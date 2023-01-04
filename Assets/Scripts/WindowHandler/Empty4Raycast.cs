namespace UnityEngine.UI
{
    using UnityEngine.EventSystems;
    using System.Collections.Generic;
#if UNITY_EDITOR
    using UnityEditor;
    [CustomEditor(typeof(Empty4Raycast))]
    class Empty4RaycastEditor : Editor
    {
        public override void OnInspectorGUI(){}
    }
#endif

    public class Empty4Raycast : MaskableGraphic//, IPointerDownHandler//只需要把鼠标按下的事件向下渗透就好了(不能穿透，事件紊乱)
    {
        protected Empty4Raycast()
        {
            useLegacyMeshGeneration = false;
        }
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            toFill.Clear();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            PassEvent(eventData, ExecuteEvents.pointerDownHandler);
        }
        //把事件透下去
        public void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function)
            where T : IEventSystemHandler
        {
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(data, results);
            GameObject current = data.pointerCurrentRaycast.gameObject;
            for (int i = 0; i < results.Count; i++)
            {
                if (current != results[i].gameObject)
                {
                    Debug.Log(results[i].gameObject.name);
                    ExecuteEvents.Execute(results[i].gameObject, data, function);
                    break;//RaycastAll后ugui会自己排序，如果你只想响应透下去的最近的一个响应，这里ExecuteEvents.Execute后直接break就行。
                }
            }
        }
    }
}
