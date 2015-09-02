using UniRx;
using UnityEngine;

public static class Utils{
	public static Collider RaycastFromCamera(this Camera camera, Vector3 screenPosition){
        Debug.Log("Raycast");
        var ray = camera.ScreenPointToRay(screenPosition);
        Debug.Log("ray pos: " + ray.origin + ", ray dir: " + ray.direction);
        RaycastHit hit;
        var raycast = Physics.Raycast(ray, out hit);
        Debug.Log(hit.collider);
        return hit.collider ?? new Collider();
    }
}