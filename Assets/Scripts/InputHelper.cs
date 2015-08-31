using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;

public static class InputHelper {
	public static IObservable<Vector3> MouseDownStream(){
		return Observable.EveryUpdate()
   	 		.Where(_ => Input.GetMouseButtonDown(0))
			.Select(_ => Input.mousePosition);
	}
	
	public static IObservable<Vector3> MouseUpStream(){
		return Observable.EveryUpdate()
   	 		.Where(_ => Input.GetMouseButtonUp(0))
			.Select(_ => Input.mousePosition);
	}
	
	public static IObservable<Vector3> MouseMoveStream(){
		return Observable.EveryUpdate().Select(_ => Input.mousePosition).DistinctUntilChanged();
	}
	
	public static IObservable<IList<Vector3>> MouseDoubleClickStream(){
		return MouseDownStream()
			.Buffer(MouseDownStream()
				.Throttle(TimeSpan
					.FromMilliseconds(250)))
    		.Where(xs => xs.Count >= 2);
	}
	
	public static IObservable<Vector3> MouseDragStream(){
		return MouseDownStream().SelectMany(_ => {
    		return MouseMoveStream()
				.Skip(1)
        		.TakeUntil(MouseUpStream());
		});
	}
}