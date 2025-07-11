using System.Collections;
using UnityEngine;


namespace Dylan
{
    public class PlayerMovement : MonoBehaviour
    {
        #region FIELD
        [SerializeField] private PlayerMovementData data;
        [SerializeField] private float lastOnGroundTime;
        [SerializeField] private Transform groundCheckPoint;
        [SerializeField] private Vector2 groundCheckSize = new Vector2(0.49f, 0.03f);
        [SerializeField] private LayerMask groundLayer;

        [HideInInspector] public Vector2 move;
        private Rigidbody2D myBody;
        private Animator myAnim; 
        //Dash
        [HideInInspector] public bool isDashing;
        private bool canDash = true;
        //Jump
        [HideInInspector] public bool grounded;
        [HideInInspector] public bool isJumping;
        #endregion

        #region MONOBEHAVIOUR
        void Awake()
        {
            myBody = GetComponent<Rigidbody2D>();
            myAnim = GetComponent<Animator>(); 
        }
        void Update()
        {
            if (isDashing)
                return;

            lastOnGroundTime -= Time.deltaTime;
            // A tuşu için sola, D tuşu için sağa hareket
            if (Input.GetKey(KeyCode.A))
                move.x = -1;
            else if (Input.GetKey(KeyCode.D))
                move.x = 1;
            else
                move.x = 0; // Tuşa basılmıyorsa hareket durur

            // Zıplama: Space tuşu
            if (Input.GetKeyDown(KeyCode.Space)) 
            {
                jump(); 
            }

            // Dash: E tuşu
            if (Input.GetKeyDown(KeyCode.E) && canDash)
            {
                StartCoroutine(dash());
            }
            if (move.x != 0)
                CheckDirectionToFace(move.x > 0);
        }

        void FixedUpdate()
        {
            if (isDashing) 
                return;

            run(1); 
            if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer))
            {
                lastOnGroundTime = 0.1f;
                grounded = true;
            }
            else
            {
                grounded = false;
            }
        }
        #endregion
        #region RUN
        private void run(float lerpAmount)
        {
            float targetSpeed = move.x * data.runMaxSpeed;

            float accelRate;

            if (move.x == 0 && grounded) // Sadece yerde ve hareket etmiyorsa
            {
                myBody.linearVelocity = new Vector2(0f, myBody.linearVelocity.y);
                return; 
            }
            targetSpeed = Mathf.Lerp(myBody.linearVelocity.x, targetSpeed, lerpAmount);

            //Calculate Acceleration and Decceleration
            if (lastOnGroundTime > 0)
                accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.runAccelAmount : data.runDeccelAmount;
            else
                accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.runAccelAmount * data.accelInAir : data.runDeccelAmount * data.deccelInAir;

            if (data.doConserveMomentum && Mathf.Abs(myBody.linearVelocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(myBody.linearVelocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && lastOnGroundTime < 0)
                accelRate = 0;

            float speedDif = targetSpeed - myBody.linearVelocity.x;
            float movement = speedDif * accelRate;

            //Implementing run
            myBody.AddForce(movement * Vector2.right, ForceMode2D.Force);
        }
        #endregion
        #region DASH
        private IEnumerator dash()
        {
            canDash = false;
            isDashing = true;
            float oriGrav = myBody.gravityScale;
            myBody.gravityScale = 0f;

            myBody.linearVelocity = new Vector2(transform.localScale.x * data.dashPower, 0f);
            yield return new WaitForSeconds(data.dashingTime);
            // Dash bitince anlık hızı eski haline getirme veya sıfırlama mantığı
            if (move.x > 0)
            {
                myBody.linearVelocity = new Vector2(data.runMaxSpeed, myBody.linearVelocity.y);
            }
            else if (move.x < 0)
            {
                myBody.linearVelocity = new Vector2(-data.runMaxSpeed, myBody.linearVelocity.y);
            }
            else // Eğer dash sırasında hareket tuşuna basılmıyorsa
            {
                myBody.linearVelocity = new Vector2(0f, myBody.linearVelocity.y); 
            }

            myBody.gravityScale = oriGrav;

            isDashing = false;
            yield return new WaitForSeconds(data.dashingCoolDown);
            canDash = true;
        }
        #endregion
        #region JUMP
        private void jump()
        {
            if (grounded)
                isJumping = false; // Yerdeyken zıplama durumu false olmalı
            if (grounded) 
            {
                isJumping = true;
                myBody.linearVelocity = new Vector2(myBody.linearVelocity.x, data.jumpHeight);
            }
        }
        #endregion
        #region OTHER
        private void CheckDirectionToFace(bool isMovingRight)
        {
            Vector3 tem = transform.localScale;
            if (!isMovingRight)
                tem.x = -3f;
            else
                tem.x = 3f;
            transform.localScale = tem;
        }
        #endregion
        private void OnDrawGizmos()
        {
            if (groundCheckPoint == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckSize);
        }
    }
}