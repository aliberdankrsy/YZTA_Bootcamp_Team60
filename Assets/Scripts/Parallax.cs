using UnityEngine;

public class Parallax : MonoBehaviour
{
    private Transform cam;
    private Vector3 startPosition;

    [SerializeField] private float parallaxFactor = 0.5f;
    [SerializeField] private float spriteWidth; // Parallax sprite geniþliði dünya birimiyle

    void Start()
    {
        cam = Camera.main.transform;
        startPosition = transform.position;
    }

    void LateUpdate()
    {
        float camX = cam.position.x;
        float offsetX = (camX * parallaxFactor) % spriteWidth;
        transform.position = new Vector3(startPosition.x + offsetX, startPosition.y, startPosition.z);
    }
}
