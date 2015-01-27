using UnityEngine;
using System.Collections;

[System.Serializable]
public class Timer {

	public float Interval;
	public bool Repeating;
	public bool Stopped {get; private set;}
	bool realTime;
	float startTime;

	public Timer(float interval, bool realTime = false){
		this.realTime = realTime;
		this.Interval = interval;
		startTime = GetTime();
	}
	
	public void Repeat(){
		Repeating = true;
	}
	
	public bool IsFinished(){
		if (!Stopped){
			if (Percent() >= 1){
				if (Repeating){
					Restart();
				}
				else {
					Stop();
				}
				return true;
			}
			else {
				return false;	
			}
		}
		else{
			return false;
		}
	}

	public void SetInterval(float interval){
		this.Interval = interval;
	}

	public void AddTime(float amount){
		this.startTime += amount;
	}

	public float RawPercent(){
		return (GetTime() - startTime)/Interval;
	}

	public float Percent(){
		return Mathf.Clamp01((GetTime() - startTime)/Interval);
	}	

	public float TimePassed(){
		return GetTime() - startTime;
	}

	public float TimeRemaining(){
		return Interval - TimePassed();
	}
	
	float GetTime(){
		if (realTime){
			return Time.realtimeSinceStartup;
		}
		else{
			return Time.time;
		}
	}

	public void Restart(float delay = 0){
		Stopped = false;
		startTime = GetTime() + delay;
	}

	public void RestartDelayPercent(float percent){
		Restart(Interval * percent);
	}

	public void Stop(){
		Stopped = true;
	}

	public void SetDone() {
		startTime = Time.time - Interval;
	}

	public bool IsDone() {
		return (RawPercent() >= 1f);
	}
}
