//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Entity;
//using UnityEngine.AI;

//internal  class JinzhanNanEnemy : EntityBase {
//    private Status _status = Status.Idle;

//    public enum Status
//    {
//        Idle,
//        run,
//        Attack01,
//        Attack02,
//    }


//	private Animator ani;
//	private FSMSystem fsm;
//	public Transform Path;

//	private void Start()
//	{
//		Path = GameObject.Find("PathNode").transform;		
//		ani = GetComponent<Animator>();
//		InitFSM();
//	}

//	private void InitFSM()
//	{
//		fsm = new FSMSystem();

//		FSMState patrolState = new PatrolState(fsm, this.gameObject);
//		fsm.AddState(patrolState);
//		patrolState.AddTransition(Transition.SeePlayer, StateID.Chase);
//		FSMState chaseState = new ChaseState(fsm, this.gameObject);
//		fsm.AddState(chaseState);
//		chaseState.AddTransition(Transition.LostPlayer, StateID.Patrol);
//		FSMState attackState = new AttackState(fsm, this.gameObject);
//		//fsm.AddState(attackState);
//		//attackState.AddTransition(Transition.CanAttack, StateID.See);

//	}

//	// Update is called once per frame
//	protected override void Update()
//	{
//		base.Update();
//		fsm.Update(this.gameObject, this.ani);
//	}

//	private void OnTriggerEnter(Collider other)
//	{
//		if (other.gameObject.tag == "Player")
//		{
//			EntityBase monster = other.GetComponent<EntityBase>();
//			EntityBase player = other.GetComponent<EntityBase>();
//			monster.TakeDamage(3);
//		}
//	}
//}
