using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Karakter
    public float smoothSpeed = 0.1f; // Takip hýzý
    public float fixedY = 0f; // Sabit Y pozisyonu
    private float fixedZ = -10f; // Sabit Z pozisyonu (2D için genelde -10)

    void Start()
    {
        if (target != null)
        {
            // Kamerayý baþta karaktere hizala ama sadece X
            transform.position = new Vector3(target.position.x, fixedY, fixedZ);
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Sadece X deðiþiyor, Y ve Z sabit kalýyor
        float desiredX = target.position.x;
        Vector3 desiredPosition = new Vector3(desiredX, fixedY, fixedZ);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    }
}
