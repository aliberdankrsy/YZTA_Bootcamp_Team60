using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dylan
{
	public class PlayerAttackMethod : MonoBehaviour
	{
		#region FIELD

		private PlayerAnimation playerAnim;
		private PlayerMovement playerMv;
		private Animator myAnim;
		private Rigidbody2D myBody;

		[Header("Basic Attack")]
		public float basicAttack01Power = 0.5f;
		public float basicAttack02Power = 0.5f;
		public float basicAttack03Power = 0.9f;

		private bool atkButtonClickedOnAtk01;
		private bool atkButtonClickedOnAtk02;
		private bool atkButtonClickedOnAtk03;

		private const string attack01 = "Attack01";
		private const string attack02 = "Attack02";
		private const string attack03 = "Attack03";
		private const string notAttacking = "NotAttacking";

		#endregion
		void Awake()
		{
			myAnim = GetComponent<Animator>();
			playerAnim = GetComponent<PlayerAnimation>();
			myBody = GetComponent<Rigidbody2D>();
			playerMv = GetComponent<PlayerMovement>();
		}
		void Update()
		{
			if (playerMv.isDashing) 
				return;
			BasicAttackCombo();
		}
        void FixedUpdate()
        {
            if (playerMv.isDashing)
            BasicAttackMethod();
        }
        #region BASIC ATTACK

        private void BasicAttackCombo()
		{
			//Combo attack mekanik
			if (Input.GetMouseButtonDown(0) && !myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack01") && !myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack02") && !myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack03") && playerMv.grounded)
				myAnim.SetTrigger(attack01);
			//Set combo attack 01 
			if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack01"))
			{
				if (Input.GetMouseButtonDown(0))
					atkButtonClickedOnAtk01 = true;

				if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= .8 && atkButtonClickedOnAtk01)
				{
					myAnim.SetTrigger(attack02);
					atkButtonClickedOnAtk01 = false;

				}
				else if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !atkButtonClickedOnAtk01)
					myAnim.SetTrigger(notAttacking);
			}
			//Set combo attack 02
			if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack02"))
			{
				if (Input.GetMouseButtonDown(0))
					atkButtonClickedOnAtk02 = true;

				if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= .8 && atkButtonClickedOnAtk02)
				{
					myAnim.SetTrigger(attack03);
					atkButtonClickedOnAtk02 = false;

				}
				else if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !atkButtonClickedOnAtk02)
					myAnim.SetTrigger(notAttacking);
			}
			//Set combo attack 03
			if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack03"))
			{
				if (Input.GetMouseButtonDown(0))
					atkButtonClickedOnAtk03 = true;

				if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && atkButtonClickedOnAtk03)
				{
					myAnim.SetTrigger(attack01);
					atkButtonClickedOnAtk03 = false;

				}
				else if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !atkButtonClickedOnAtk03)
					myAnim.SetTrigger(notAttacking);
			}
		}
		private void BasicAttackMethod()
		{
			// MOVE + ATTACK 
			if (transform.localScale.x == 1)
			{
				if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack01"))
					myBody.linearVelocity = new Vector2(basicAttack01Power, myBody.linearVelocity.y);

				if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack02"))
					myBody.linearVelocity = new Vector2(basicAttack02Power, myBody.linearVelocity.y);

				if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack03"))
					myBody.linearVelocity = new Vector2(basicAttack03Power, myBody.linearVelocity.y);
			}
			else
			{
				if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack01"))
					myBody.linearVelocity = new Vector2(-basicAttack01Power, myBody.linearVelocity.y);

				if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack02"))
					myBody.linearVelocity = new Vector2(-basicAttack02Power, myBody.linearVelocity.y);

				if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack03"))
					myBody.linearVelocity = new Vector2(-basicAttack03Power, myBody.linearVelocity.y);
			}

		}
		#endregion


	}
}
