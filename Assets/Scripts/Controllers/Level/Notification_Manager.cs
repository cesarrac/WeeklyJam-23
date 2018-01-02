using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notification_Manager : MonoBehaviour {

	public static Notification_Manager instance {get; protected set;}
	CountdownHelper countdown;
	float timeBetweenNotes = 5f;
	bool isNotifying = false;
	Queue<Notification> notifications;
	public GameObject notificationPanel;
	public Text notificationTxt;
	void Awake(){
		instance = this;
		notifications = new Queue<Notification>();
		countdown = new CountdownHelper(timeBetweenNotes);
	}
	public void AddNotification(string notificationTxt){
		Notification newNotification;
		newNotification.note = notificationTxt;
		notifications.Enqueue(newNotification);
		if (isNotifying == true)
			return;
		TryNextNotification();
	}
	public void TryNextNotification(){
		if (notifications.Count <= 0)
			return;
		Notification nextNotification = notifications.Dequeue();
		countdown.Reset();
		isNotifying = true;
		DoNotification(nextNotification);
	}
	void DoNotification(Notification notification){
		// Tell UI there's a new notification
		notificationPanel.SetActive(true);
		notificationTxt.text = notification.note;
	}
	void Update(){
		if (isNotifying == true){
			countdown.UpdateCountdown();
			if (countdown.elapsedPercent >= 1){
				FinishNotification();
			}
		}
	}
	public void FinishNotification(){
		notificationPanel.SetActive(false);
		isNotifying = false;
		TryNextNotification();
	}
}

public struct Notification{
	public string note;

}
