using UnityEngine;

// Bu satır, Unity'de Create menüsünde görünmesini sağlar.
[CreateAssetMenu(fileName = "PlayerMovementData", menuName = "ScriptableObjects/Player Movement Data", order = 1)]
public class PlayerMovementData : ScriptableObject
{
    [Header("Run")]
    public float runMaxSpeed = 8f;
    public float runAccelAmount = 0.8f;
    public float runDeccelAmount = 0.5f;
    [Header("Jump")]
    public float jumpHeight = 8f;
    [Header("Dash")]
    public float dashPower = 8f;
    public float dashingTime = 0.2f;
    public float dashingCoolDown = 1f;
    [Header("Air Control")]
    public float accelInAir = 0.5f;
    public float deccelInAir = 0.5f;
    public bool doConserveMomentum = true;
}