using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary> General, simple class that represents a timer. </summary>
[System.Serializable]
public class Timer {

	/// <summary> Factory that constructs a timer that will tick once, and need to be reset. </summary>
	/// <param name="time"> Length of time in seconds. </param>
	/// <param name="onTick"> Optional callback </param>
	/// <returns> Timer object that ticks once after <paramref name="time"/> seconds. </returns>
	public static Timer Once(float time = 5f, Action onTick = null) {
		return new Timer() { time = time, onTick = onTick, timeout = 0f, once = true };
	}
	/// <summary> Factory that constructs a timer that will tick repeatedly. </summary>
	/// <param name="time"> Length of time in seconds. </param>
	/// <param name="onTick"> Optional callback </param>
	/// <returns> Timer object repeats every <paramref name="time"/> seconds. </returns>
	public static Timer Repeat(float time = 5f, Action onTick = null) {
		return new Timer() { time = time, onTick = onTick, timeout = 0f, once = false };
	}

	
	/// <summary> Time of period for this Timer </summary>
	public float time = 5f;
	/// <summary> Current timeout for this Timer </summary>
	public float timeout = 0f;

	/// <summary> Previous Timeout </summary>
	public float lastTimeout { get; set; }
	
	/// <summary> Last DeltaTime that was used to tick this timer </summary>
	public float lastDelta { get; set; }

	/// <summary> Callback for when the timer ticks </summary>
	public Action onTick = null;
	/// <summary> Does this timer only tick once, and then stop? </summary>
	public bool once = false; 

	/// <summary> Return wether the timer just passed a given point in time </summary>
	/// <param name="timePoint"> Time to check against </param>
	/// <returns> True, if the timer has just passed the given time. </returns>
	public bool Passed(float timePoint) {
		if (!once) {
			if (timeout < lastTimeout) {
				float actualTimeout = lastTimeout + lastDelta;
				return (timePoint < actualTimeout && timePoint > lastTimeout);
			}
		}
		return (timePoint < timeout && timePoint > lastTimeout);
	}
	
	/// <summary> Updates this timer by default scaled delta time, and return if a tick happened or not. </summary>
	/// <returns> true if a tick occurred, false otherwise. </returns>
	public bool Tick() { return Tick(Time.deltaTime, onTick); }
	/// <summary> Updates this timer by a specified scaled delta time, and return if a tick happened or not. </summary>
	/// <param name="elapsedTime"> Time to elapse for this timer. </param>
	/// <returns> true if a tick occurred, false otherwise. </returns>
	public bool Tick(float elapsedTime) { return Tick(elapsedTime, onTick); }
	/// <summary> Updates this timer by a specified scaled delta time, and return if a tick happened or not. Allows provision for a custom onTick. </summary>
	/// <param name="elapsedTime"> Time to elapse for this timer. </param>
	/// <param name="onTick"> Custom provided onTick override </param>
	/// <returns> true if a tick occurred, false otherwise. </returns>
	public bool Tick(float elapsedTime, Action onTick = null) {
		if (onTick == null) { onTick = this.onTick; }
		lastTimeout = timeout;
		lastDelta = elapsedTime;
		timeout += elapsedTime;
		if (once) {
			if (lastTimeout < time && timeout > time) {
				if (onTick != null) { onTick(); }
				return true;
			}
		} else {
			if (timeout >= time) {
				timeout -= time;
				if (onTick != null) { onTick(); }
				return true;
			}
		}
		return false;
	}

	public void Reset() { timeout = 0f; }

	// TBD: Maybe add update variants that tick multiple times per call, and return an int (probably YAGNI)
	
}
