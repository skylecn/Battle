/***********************************************
				EasyTouch V
	Copyright © 2014-2015 The Hedgehog Team
    http://www.thehedgehogteam.com/Forum/
		
	  The.Hedgehog.Team@gmail.com
		
**********************************************/
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

[AddComponentMenu("EasyTouch/Quick Drag")]
public class QuickDrag: QuickBase {

	#region Events
	[System.Serializable] public class OnDelegateTouchStart : UnityEvent<Gesture>{}
	[System.Serializable] public class OnDelegateTouchEnd : UnityEvent<Gesture>{}

	[System.Serializable] public class OnDelegateDrag : UnityEvent<Gesture>{}
	[System.Serializable] public class OnDelegateDragEnd : UnityEvent<Gesture>{}
	[System.Serializable] public class  OnDelegateDragMove:UnityEvent<Gesture> {}
	
	
	[SerializeField] 
	public OnDelegateTouchStart onTouchStart;
	[SerializeField] 
	public OnDelegateDrag onDrag;
	[SerializeField] 
	public OnDelegateDragEnd onDragEnd;

	//[SerializeField] public OnDelegateDragMove onDragMove;
	
	[SerializeField] 
	public OnDelegateTouchEnd onTouchEnd;
	#endregion

	#region Members
	public bool isStopOncollisionEnter = false;

	private Vector3 deltaPosition;
	public bool isOnDrag = false;
	private Gesture lastGesture;
	#endregion
	
	#region Monobehaviour CallBack
	public QuickDrag(){
			quickActionName = "QuickDrag"+ System.Guid.NewGuid().ToString().Substring(0,7);
		axesAction = AffectedAxesAction.XY;
	}

	public void InitAction()
    {
		onTouchStart = new OnDelegateTouchStart();
		//onDragMove = new OnDelegateDragMove();
		onDrag = new OnDelegateDrag();
		onDragEnd = new OnDelegateDragEnd();
		onTouchEnd = new OnDelegateTouchEnd();
	}

	public  void OnEnable(){
		base.OnEnable();
		EasyTouch.On_TouchStart += On_TouchStart;
		EasyTouch.On_TouchDown += On_TouchDown;
		EasyTouch.On_TouchUp += On_TouchUp;
		EasyTouch.On_Drag += On_Drag;
		EasyTouch.On_DragStart += On_DragStart;
		EasyTouch.On_DragEnd += On_DragEnd;
	}
			
	public  void OnDisable(){
		base.OnDisable();
		UnsubscribeEvent();
	}
	
	public void OnRecycle(){
		UnsubscribeEvent();
	}
	
	void UnsubscribeEvent(){
		EasyTouch.On_TouchStart -= On_TouchStart;
		EasyTouch.On_TouchDown -= On_TouchDown;
		EasyTouch.On_TouchUp -= On_TouchUp;
		EasyTouch.On_Drag -= On_Drag;
		EasyTouch.On_DragStart -= On_DragStart;
		EasyTouch.On_DragEnd -= On_DragEnd;
	}

	//void OnCollisionEnter(){
	//if (isStopOncollisionEnter && isOnDrag){
	//StopDrag();
	//}
	//}
	#endregion

	#region EasyTouch Event
	void On_TouchStart(Gesture gesture)
	{

		if (realType == GameObjectType.Obj_3D && gesture.pickedObject!=null)
		{
			if ((gesture.pickedObject == gameObject || gesture.pickedObject.transform.IsChildOf(transform)) && fingerIndex == -1)
			{

				fingerIndex = gesture.fingerIndex;
				//transform.SetAsLastSibling();
				if (onTouchStart != null)
				{
					onTouchStart.Invoke(gesture);
				}
				isOnDrag = true;
			}
		}
	}

	void On_TouchDown (Gesture gesture){

		if (isOnDrag && fingerIndex == gesture.fingerIndex && realType == GameObjectType.Obj_3D && gesture.pickedObject!=null){
			if ((gesture.pickedObject == gameObject || gesture.pickedObject.transform.IsChildOf(transform)))
			{
				//transform.position += (Vector3)gesture.deltaPosition;

				if (gesture.deltaPosition != Vector2.zero)
				{
					if (onDrag != null)
					{
						onDrag.Invoke(gesture);
					}
				}
				lastGesture = gesture;
			}
		}
	}

	void On_TouchUp (Gesture gesture){

		if (fingerIndex == gesture.fingerIndex && realType == GameObjectType.Obj_3D){
			lastGesture = gesture;
			StopDrag();
			if (onTouchEnd != null)
			{
				onTouchEnd.Invoke(gesture);
			}
		}
	}


	// At the drag beginning 
	void On_DragStart( Gesture gesture){
		
		if (realType != GameObjectType.UI){

			if ((!enablePickOverUI && gesture.pickedUIElement == null) || enablePickOverUI){
				if (gesture.pickedObject == gameObject && !isOnDrag){

					isOnDrag = true;

					fingerIndex = gesture.fingerIndex;

					// the world coordinate from touch
					Vector3 position = gesture.GetTouchToWorldPoint(gesture.pickedObject.transform.position);
					deltaPosition = position - transform.position;

					// 
					if (resetPhysic){
						if (cachedRigidBody){
							cachedRigidBody.isKinematic = true;
						}

						if (cachedRigidBody2D){
							cachedRigidBody2D.isKinematic = true;
						}
					}
//
//					if (onTouchStart != null)
//					{
//						onTouchStart.Invoke(gesture);
//					}
				}
			}
		}

	}
	
	// During the drag
	void On_Drag(Gesture gesture){

		if (fingerIndex == gesture.fingerIndex){
			if (realType == GameObjectType.Obj_2D || realType == GameObjectType.Obj_3D){

				// Verification that the action on the object
				Vector3 position = gesture.GetTouchToWorldPoint(gesture.pickedObject.transform.position) - deltaPosition;
				//transform.position = GetPositionAxes( position);
				//if (onDragMove != null)
				//{
				//	onDragMove.Invoke(gesture);
				//}
				if (gesture.deltaPosition != Vector2.zero)
				{
					if (onDrag != null)
					{
						onDrag.Invoke(gesture);
					}
				}
				lastGesture = gesture;
			}
		}
	}

	// End of drag
	void On_DragEnd(Gesture gesture){

		if (fingerIndex == gesture.fingerIndex){
			lastGesture = gesture;
			StopDrag();
		}
	}

	#endregion

	#region Private Method
	private Vector3 GetPositionAxes(Vector3 position){
		
		Vector3 axes = position;
		
		switch (axesAction){
		case AffectedAxesAction.X:
			axes = new Vector3(position.x,transform.position.y,transform.position.z);
			break;
		case AffectedAxesAction.Y:
			axes = new Vector3(transform.position.x,position.y,transform.position.z);
			break;
		case AffectedAxesAction.Z:
			axes = new Vector3(transform.position.x,transform.position.y,position.z);
			break;
		case AffectedAxesAction.XY:
			axes = new Vector3(position.x,position.y,transform.position.z);
			break;
		case AffectedAxesAction.XZ:
			axes = new Vector3(position.x,transform.position.y,position.z);
			break;
		case AffectedAxesAction.YZ:
			axes = new Vector3(transform.position.x,position.y,position.z);
			break;
		}
		
		return axes;
	
	}
	#endregion

	#region Public Method
	public void StopDrag(){

		fingerIndex = -1;

		if (resetPhysic){
			if (cachedRigidBody){
				cachedRigidBody.isKinematic = isKinematic;
			}
			
			if (cachedRigidBody2D){
				cachedRigidBody2D.isKinematic = isKinematic2D;
			}
		}
		isOnDrag = false;
		this.isAutoDrag = false;
		onDragEnd.Invoke(lastGesture);
	}
	public bool isAutoDrag = false; 

    public void Update()
    {
		if(this.isAutoDrag)
        {
			if (onDrag != null&& lastGesture!=null)
			{
				onDrag.Invoke(lastGesture);
			}
		}
    }
    #endregion
}
