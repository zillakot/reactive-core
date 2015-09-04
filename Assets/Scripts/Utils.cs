using UniRx;
using UnityEngine;

public static class Utils{
    public static RaycastHit RaycastFromCamera(this Camera camera, Vector3 screenPosition, string layerName = null){
        var layer = (layerName == null) ? ~0 : (1 << LayerMask.NameToLayer(layerName));
        var ray = camera.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        var raycast = Physics.Raycast(ray, out hit, Mathf.Infinity, layer);
        return hit;
    }
}