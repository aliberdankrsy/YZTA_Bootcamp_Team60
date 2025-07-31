using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Karakter
    public float smoothSpeed = 0.1f; // Takip h�z�
    public float fixedY = 0f; // Sabit Y pozisyonu
    private float fixedZ = -10f; // Sabit Z pozisyonu (2D i�in genelde -10)

    void Start()
    {
        if (target != null)
        {
            // Kameray� ba�ta karaktere hizala ama sadece X
            transform.position = new Vector3(target.position.x, fixedY, fixedZ);
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Sadece X de�i�iyor, Y ve Z sabit kal�yor
        float desiredX = target.position.x;
        Vector3 desiredPosition = new Vector3(desiredX, fixedY, fixedZ);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    }
}
