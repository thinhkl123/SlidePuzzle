using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform layerTransform;
        public float parallaxFactor; // Giá trị từ 0 (xa nhất) đến 1 (gần nhất)
    }

    public ParallaxLayer[] layers;
    private Vector3 previousCameraPosition;

    public Transform cameraTransform;

    private void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        previousCameraPosition = cameraTransform.position;
    }

    private void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - previousCameraPosition;

        foreach (var layer in layers)
        {
            Vector3 newPosition = layer.layerTransform.position;
            newPosition += new Vector3(deltaMovement.x * layer.parallaxFactor, deltaMovement.y * layer.parallaxFactor, 0f);
            layer.layerTransform.position = newPosition;
        }

        previousCameraPosition = cameraTransform.position;
    }
}
