using System.Text;

namespace Sisus
{
	public static class StringBuilderPool
	{
		private const int BuilderCapacity = 1000;
		
		private static Pool<StringBuilder> pool = new Pool<StringBuilder>(2);
		
		public static StringBuilder Create()
		{
			StringBuilder result;
			if(!pool.TryGet(out result))
			{
				return new StringBuilder(BuilderCapacity);
			}
			return result;
		}

		public static void Dispose(ref StringBuilder disposing)
		{
			disposing.Length = 0;
			pool.Dispose(ref disposing);
			disposing = null;
		}

		public static string ToStringAndDispose(ref StringBuilder disposing)
		{
			var result = disposing.ToString();
			disposing.Length = 0;
			pool.Dispose(ref disposing);
			disposing = null;
			return result;
		}
	}
}