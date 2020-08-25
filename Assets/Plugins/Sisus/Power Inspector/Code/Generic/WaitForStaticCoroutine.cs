using System.Collections;
using UnityEngine;

namespace Sisus
{
	/// <summary>
	/// Yield instructions that can be used to wait until coroutine started using the StaticCoroutine
	/// class has stopped invoking.
	/// </summary>
	public class WaitForStaticCoroutine : CustomYieldInstruction
	{
		private IEnumerator waitingForCoroutine;
		private string waitingForMethodByName;

		public override bool keepWaiting
		{
			get
			{
				return StaticCoroutine.IsInvoking(waitingForCoroutine, waitingForMethodByName);
			}
		}

		#if UNITY_EDITOR
		public WaitForStaticCoroutine(IEnumerator waitForCoroutine)
		{
			waitingForCoroutine = waitForCoroutine;
			waitingForMethodByName = null;
		}
		#endif

		public WaitForStaticCoroutine(IEnumerator waitForCoroutine, string waitForMethodByName)
		{
			waitingForCoroutine = waitForCoroutine;
			waitingForMethodByName = waitForMethodByName;
		}

		public WaitForStaticCoroutine(string waitForMethodByName)
		{
			waitingForCoroutine = null;
			waitingForMethodByName = waitForMethodByName;
		}
	}
}