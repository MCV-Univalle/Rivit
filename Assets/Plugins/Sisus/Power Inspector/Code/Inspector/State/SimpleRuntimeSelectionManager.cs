using System;
using Object = UnityEngine.Object;

namespace Sisus
{
	public class SimpleRuntimeSelectionManager : ISelectionManager
	{
		/// <inheritdoc/>
		public Action OnSelectionChanged { get; set; }

		/// <inheritdoc/>
		private Action<Object[]> onNextSelectionChanged;

		/// <inheritdoc/>
		public Object[] Selected
		{
			get;
			private set;
		}

		/// <inheritdoc/>
		public void Select(Object target)
		{
			if(Selected.Length != 1 || Selected[0] != target)
			{
				Selected = ArrayPool<Object>.CreateWithContent(target);
				HandleCallbacks();
			}
		}

		/// <inheritdoc/>
		public void Select(Object[] targets)
		{
			if(!Selected.ContentsMatch(targets))
			{
				Selected = targets;
				HandleCallbacks();
			}
		}

		public void OnNextSelectionChanged(Action<Object[]> action)
		{
			onNextSelectionChanged += action;
		}

		/// <inheritdoc />
		public void CancelOnNextSelectionChanged(Action<Object[]> action)
		{
			onNextSelectionChanged -= action;
		}

		private void HandleCallbacks()
		{
			if(onNextSelectionChanged != null)
			{
				var callback = onNextSelectionChanged;
				onNextSelectionChanged = null;
				callback(Selected);
			}

			if(OnSelectionChanged != null)
			{
				OnSelectionChanged();
			}
		}
	}
}