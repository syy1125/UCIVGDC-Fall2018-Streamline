using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public abstract class StatefulUI<TState> : MonoBehaviour
{
	private TState _last;
	private TState _state;

	private delegate void RenderFunc();

	private event RenderFunc LinkedRender = () => { };

	protected TState State
	{
		get { return _state; }
		set
		{
			_last = _state;
			_state = value;
			
			foreach (FieldInfo field in typeof(TState).GetFields())
			{
				if (field.GetValue(_state) == null)
				{
					field.SetValue(_state, field.GetValue(_last));
				}
			}

			Render();
			LinkedRender.Invoke();
		}
	}

	protected bool Changed<T>(Func<TState, T> get, Func<T, T, bool> equals = null)
	{
		if (_last == null) return true;
		if (equals == null) equals = EqualityComparer<T>.Default.Equals;
		return equals(get(_state), get(_last));
	}

	protected abstract void Render();

	protected Func<TExternalState> LinkState<TExternalState>(StatefulUI<TExternalState> other)
	{
		other.LinkedRender += Render;
		return () => other.State;
	}
}