//#define SAFE_MODE

#define DEBUG_TO_STRING_TOO_LONG

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;
using JetBrains.Annotations;

using System.Runtime.CompilerServices;

namespace Sisus
{
	/// <summary>
	/// Static utility class with methods for converting objects to human-readable string format
	/// with minimal garbage generation.
	/// Not thread safe.
	/// </summary>
	#if UNITY_EDITOR
	[UnityEditor.InitializeOnLoad]
	#endif
	public static class StringUtils
	{
		public const string DoubleFormat = "0.###################################################################################################################################################################################################################################################################################################################################################";
		public const string Null = "<color=red>null</color>";
		public const string False = "<color=red>False</color>";
		public const string True = "<color=green>True</color>";

		/// <summary>
		/// Number of int to string results to cache.
		/// </summary>
		private const int IntToStringResultCacheCount = 2500;

		private const int MaxToStringResultLength = 175;

		private static int recursiveCallCount = 0;
		private const int MaxRecursiveCallCount = 25;

		/// <summary>
		/// The numbers as string.
		/// </summary>
		private static readonly string[] numbersAsString = new string[IntToStringResultCacheCount];

		/// <summary>
		/// Cached StringBuilder instance that is used by various ToString methods
		/// NOTE: Because of this not all methods in StringUtils are thread safe
		/// </summary>
		private static readonly StringBuilder CachedBuilder = new StringBuilder(500);

		/// <summary>
		/// Dictionary where results of ToPascalCase method calls get cached
		/// </summary>
		private static readonly Dictionary<string, string> cachedToPascalCaseResults = new Dictionary<string, string>(10);

		#if UNITY_EDITOR
		/// <summary>
		/// Initializes static members of the StringUtils class.
		/// Called in the editor because of the InitializeOnLoad attribute
		/// </summary>
		[UsedImplicitly]
		static StringUtils()
		{
			Setup();
		}
		#endif

		#if !UNITY_EDITOR
		[RuntimeInitializeOnLoadMethod, UsedImplicitly]
		#endif
		/// <summary>
		/// Initializes static members of the StringUtils class.
		/// Called in the editor because of the InitializeOnLoad attribute
		/// and at runtime because of the RuntimeInitializeOnLoadMethod attribute.
		/// </summary>
		private static void Setup()
		{
			for(int i = IntToStringResultCacheCount - 1; i >= 0; i--)
			{
				numbersAsString[i] = i.ToString();
			}
		}

		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		public static string ToString(char character)
		{
			if('\0'.Equals(character))
			{
				return "\\0";
			}

			return new string(character, 1);
		}

		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		public static string ToString(Event inputEvent)
		{
			return ToString(inputEvent, false);
		}

		private static string ToString(Event inputEvent, bool usingRawType)
		{
			if(inputEvent == null)
			{
				return "null";
			}
			
			var type = usingRawType ? inputEvent.rawType : inputEvent.type;

			switch(type)
			{
				case EventType.Used:
					if(usingRawType)
					{
						CachedBuilder.Append("Used");
						break;
					}
					CachedBuilder.Append(ToString(inputEvent, true));
					CachedBuilder.Insert(0, "(Used)");
					break;
				case EventType.Ignore:
					if(usingRawType)
					{
						CachedBuilder.Append("Ignore");
						break;
					}
					CachedBuilder.Append(ToString(inputEvent, true));
					CachedBuilder.Insert(0, "(Ignore)");
					break;
				case EventType.KeyDown:
				case EventType.KeyUp:
					CachedBuilder.Append(type);
					CachedBuilder.Append("(");
					CachedBuilder.Append(inputEvent.keyCode);
					CachedBuilder.Append(")");
					break;
				case EventType.MouseMove:
				case EventType.MouseDrag:
				case EventType.ScrollWheel:
					CachedBuilder.Append(type);
					CachedBuilder.Append("(");
					CachedBuilder.Append(ToString(inputEvent.delta));
					CachedBuilder.Append(")");
					break;
				case EventType.MouseDown:
				case EventType.MouseUp:
					CachedBuilder.Append(type);
					CachedBuilder.Append("(");
					CachedBuilder.Append(ToString(inputEvent.button));
					CachedBuilder.Append(")");
					break;
				case EventType.ExecuteCommand:
					CachedBuilder.Append(type);
					CachedBuilder.Append("(");
					CachedBuilder.Append(inputEvent.commandName);
					CachedBuilder.Append(")");
					break;
				default:
					if(inputEvent.character != 0)
					{
						CachedBuilder.Append(type);
						CachedBuilder.Append("(");
						CachedBuilder.Append(ToString(inputEvent.character));
						CachedBuilder.Append(")");
						break;
					}
					CachedBuilder.Append(type);
					break;
			}

			if(inputEvent.modifiers != EventModifiers.None)
			{
				CachedBuilder.Append("+");
				CachedBuilder.Append(ToString(inputEvent.modifiers));
			}

			string result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string ToString(EventModifiers eventModifiers)
		{
			if(eventModifiers == EventModifiers.None)
			{
				return "Modifiers:None";
			}

			var sb = StringBuilderPool.Create();
			if((eventModifiers & EventModifiers.Shift) == EventModifiers.Shift)
			{
				sb.Append("+Shift");
			}
			if((eventModifiers & EventModifiers.Control) == EventModifiers.Control)
			{
				sb.Append("+Ctrl");
			}
			if((eventModifiers & EventModifiers.Alt) == EventModifiers.Alt)
			{
				sb.Append("+Alt");
			}
			if((eventModifiers & EventModifiers.FunctionKey) == EventModifiers.FunctionKey)
			{
				sb.Append("+FunctionKey");
			}
			if((eventModifiers & EventModifiers.CapsLock) == EventModifiers.CapsLock)
			{
				sb.Append("+CapsLock");
			}
			if((eventModifiers & EventModifiers.Command) == EventModifiers.Command)
			{
				sb.Append("+Command");
			}
			if((eventModifiers & EventModifiers.Numeric) == EventModifiers.Numeric)
			{
				sb.Append("+Numeric");
			}

			sb.Remove(0,1);
			sb.Insert(0, "Modifiers:");
			return StringBuilderPool.ToStringAndDispose(ref sb);
		}
		
		public static string ToStringCompact(object target)
		{
			recursiveCallCount = 0;

			if(target == null)
			{
				return "null";
			}
			var type = target.GetType();
			if(type != Types.Type)
			{
				string result = ToStringInternal(target);
				if(result.Length < 50)
				{
					return result;
				}
			}
			return ToStringSansNamespace(type);
		}

		public static string ToString(int number)
		{
			if(number < 0)
			{
				try
				{
					number = Mathf.Abs(number);
				}
				catch(OverflowException) //this happens for int.MinValue
				{
					return number.ToString(CultureInfo.InvariantCulture);
				}
				return string.Concat("-", number);
			}
			if(number >= IntToStringResultCacheCount)
			{
				return number.ToString(CultureInfo.InvariantCulture);
			}
			return numbersAsString[number];
		}

		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		public static string ToString(int number, int minLengthUsingLeadingZeroes)
		{
			return number.ToString(Concat("D", minLengthUsingLeadingZeroes));
		}

		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		public static string ToString(decimal number)
		{
			return number.ToString(CultureInfo.InvariantCulture);
		}

		public static string ToString(short number)
		{
			if(number < 0)
			{
				return string.Concat("-", ToString(-number));
			}
			if(number >= IntToStringResultCacheCount)
			{
				return number.ToString(CultureInfo.InvariantCulture);
			}
			return numbersAsString[number];
		}

		public static string ToString(ushort number)
		{
			if(number >= IntToStringResultCacheCount)
			{
				return number.ToString(CultureInfo.InvariantCulture);
			}
			return numbersAsString[number];
		}

		public static string ToString(uint number)
		{
			if(number >= IntToStringResultCacheCount)
			{
				return number.ToString(CultureInfo.InvariantCulture);
			}
			return numbersAsString[(int)number];
		}
		
		public static string ToString(ulong number)
		{
			if(number >= IntToStringResultCacheCount)
			{
				return number.ToString(CultureInfo.InvariantCulture);
			}
			return numbersAsString[((int)number)];
		}

		public static string ToString(long number)
		{
			if(number >= int.MinValue && number <= int.MaxValue)
			{
				return ToString((int)number);
			}
			return number.ToString(CultureInfo.InvariantCulture);
		}

		public static string ToString(float value)
		{
			int i = Mathf.RoundToInt(value);
			if(Mathf.Abs(value - i) <= 0.005f)
			{
				return ToString(i);
			}
			return value.ToString("0.##", CultureInfo.InvariantCulture);
		}

		public static string ToString(double value)
		{
			var rounded = Math.Round(value);
			if(Math.Abs(value - rounded) <= 0.005d && (rounded < IntToStringResultCacheCount && rounded > -IntToStringResultCacheCount))
			{
				return ToString((int)rounded);
			}
			return value.ToString("0.##", CultureInfo.InvariantCulture);
		}
		
		public static string MakeFieldNameHumanReadable([NotNull]string input)
		{
			#if DEV_MODE
			Profiler.BeginSample("SplitPascalCaseToWords");
			#endif

			string result;
			if(cachedToPascalCaseResults.TryGetValue(input, out result))
			{
				return result;
			}

			int length = input.Length;
			switch(length)
			{
				case 0:
					result = "";
					break;
				case 1:
					result = input.ToUpper();
					break;
				default:
					int i = 0;
					int stop = length;

					//skip past prefixes like "m_"
					if(input[1] == '_' && length >= 3)
					{
						i = 2;
					}
					//handle property backing field
					else if(input[0] == '<')
					{
						i = 1;
						stop = length - 16;
					}
					//skip past "_" prefix
					else if(input[0] == '_')
					{
						i = 1;
					}

					var sb = StringBuilderPool.Create();

					//first letter should always be upper case
					sb.Append(char.ToUpper(input[i]));

					#if DEV_MODE && SAFE_MODE
					for(int s = 0; s <= 9; s++)
					{
						char c = s.ToString()[0];
						Debug.Assert(!char.IsUpper(c));
						Debug.Assert(!char.IsLower(c));
					}
					#endif

					// skipping first letter which was already capitalized
					for(i = i + 1; i < stop; i++)
					{
						char c = input[i];
						
						//If this character is a number...
						if(char.IsNumber(c))
						{
							//...and previous character is a letter...
							if(char.IsLetter(input[i - 1]))
							{
								//...add a space before this character.
								sb.Append(' ');
								//e.g. "Id1" => "Id 1", "FBI123" => "FBI 123", "Array2D" => "Array 2D"
							}
						}
						//If this chararacter is an upper case letter...
						else if(char.IsUpper(c))
						{
							//...and previous character is a lower case letter...
							if(char.IsLower(input[i - 1])) //IsLower returns false for numbers, so no need to check && !IsNumber separately
							{
								//...add a space before it.
								sb.Append(' ');
								//e.g. "TestID" => "Test ID", "Test3D => "Test 3D"
							}
							//...or if the next character is a lower case letter
							// and previous character is not a "split point" character (space, slash, underscore etc.)
							else if(length > i + 1 && char.IsLower(input[i + 1])) //IsLower returns false for numbers, so no need to check && !IsNumber separately
							{
								switch(input[i - 1])
								{
									case ' ':
									case '/':
									case '\\':
									case '_':
									case '-':
										break;
									default:
										//...add a space before it.
										sb.Append(' ');
										//e.g. "FBIDatabase" => "FBI Database", "FBI123" => "FBI 123", "My3DFx" => "My 3D Fx"
										break;
								}
								
							}
						}
						// replace underscores with the space character...
						else if(c == '_')
						{
							// ...unless previous character is a split point
							switch(input[i - 1])
							{
								case ' ':
								case '/':
								case '\\':
								case '_':
								case '-':
									break;
								default:
								sb.Append(' ');
									break;
							}
							continue;
						}
						
						sb.Append(c);
					}

					result = StringBuilderPool.ToStringAndDispose(ref sb);
					break;
			}
			
			cachedToPascalCaseResults.Add(input, result);

			#if DEV_MODE
			Profiler.EndSample();
			#endif

			return result;
		}

		public static string SplitPascalCaseToWords([NotNull]string input)
		{
			#if DEV_MODE
			Profiler.BeginSample("SplitPascalCaseToWords");
			#endif

			string result;
			if(cachedToPascalCaseResults.TryGetValue(input, out result))
			{
				return result;
			}

			int length = input.Length;
			switch(length)
			{
				case 0:
					result = "";
					break;
				case 1:
					result = input.ToUpper();
					break;
				default:
					int i = 0;

					//skip past prefixes like "m_"
					if(input[1] == '_' && length >= 3)
					{
						i = 2;
					}
					//skip past "_" prefix
					else if(input[0] == '_')
					{
						i = 1;
					}
					
					var sb = StringBuilderPool.Create();

					//first letter should always be upper case
					sb.Append(char.ToUpper(input[i]));

					#if DEV_MODE && SAFE_MODE
					for(int s = 0; s <= 9; s++)
					{
						char c = s.ToString()[0];
						Debug.Assert(!char.IsUpper(c));
						Debug.Assert(!char.IsLower(c));
					}
					#endif

					// skipping first letter which was already capitalized
					for(i = i + 1; i < length; i++)
					{
						char c = input[i];
						
						//If this character is a number...
						if(char.IsNumber(c))
						{
							//...and previous character is a letter...
							if(char.IsLetter(input[i - 1]))
							{
								//...add a space before this character.
								sb.Append(' ');
								//e.g. "Id1" => "Id 1", "FBI123" => "FBI 123", "Array2D" => "Array 2D"
							}
						}
						//If this chararacter is an upper case letter...
						else if(char.IsUpper(c))
						{
							//...and previous character is a lower case letter...
							if(char.IsLower(input[i - 1])) //IsLower returns false for numbers, so no need to check && !IsNumber separately
							{
								//...add a space before it.
								sb.Append(' ');
								//e.g. "TestID" => "Test ID", "Test3D => "Test 3D"
							}
							//...or if the next character is a lower case letter
							// and previous character is not a "split point" character (space, slash, underscore etc.)
							else if(length > i + 1 && char.IsLower(input[i + 1])) //IsLower returns false for numbers, so no need to check && !IsNumber separately
							{
								switch(input[i - 1])
								{
									case ' ':
									case '/':
									case '\\':
									case '_':
									case '-':
										break;
									default:
										//...add a space before it.
										sb.Append(' ');
										//e.g. "FBIDatabase" => "FBI Database", "FBI123" => "FBI 123", "My3DFx" => "My 3D Fx"
										break;
								}
								
							}
						}
						// replace underscores with the space character...
						else if(c == '_')
						{
							// ...unless previous character is a split point
							switch(input[i - 1])
							{
								case ' ':
								case '/':
								case '\\':
								case '_':
								case '-':
									break;
								default:
								sb.Append(' ');
									break;
							}
							continue;
						}
						
						sb.Append(c);
					}

					result = StringBuilderPool.ToStringAndDispose(ref sb);
					break;
			}
			
			cachedToPascalCaseResults.Add(input, result);

			#if DEV_MODE
			Profiler.EndSample();
			#endif

			return result;
		}

		public static string ToString(DateTime time)
		{
			CachedBuilder.Append(ToString(time.Year));
			CachedBuilder.Append(" ");
			CachedBuilder.Append(ToString(time.Month));
			CachedBuilder.Append("/");
			CachedBuilder.Append(ToString(time.Day));
			CachedBuilder.Append(" ");
			CachedBuilder.Append(ToString(time.Hour));
			CachedBuilder.Append(":");
			CachedBuilder.Append(ToString(time.Minute));
			CachedBuilder.Append(":");
			CachedBuilder.Append(ToString(time.Second));
			int ms = time.Millisecond;
			if(ms != 0)
			{
				CachedBuilder.Append(".");
				CachedBuilder.Append(ToString(ms));
			}
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string TimeToString(DateTime time)
		{
			CachedBuilder.Append(ToString(time.Hour));
			CachedBuilder.Append(":");
			CachedBuilder.Append(ToString(time.Minute));
			CachedBuilder.Append(":");
			CachedBuilder.Append(ToString(time.Second));
			int ms = time.Millisecond;
			if(ms != 0)
			{
				CachedBuilder.Append(".");
				CachedBuilder.Append(ToString(time.Millisecond));
			}

			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string ToString(TimeSpan time)
		{
			if(time.TotalMilliseconds < 0)
			{
				time = time.Negate();
				CachedBuilder.Append("-");
			}

			int d = time.Days;
			if(d != 0)
			{
				CachedBuilder.Append(ToString(d));
				CachedBuilder.Append("d ");
			}

			int h = time.Hours;
			if(h != 0)
			{
				CachedBuilder.Append(ToString(h));
				CachedBuilder.Append("h ");
			}

			int m = time.Minutes;
			if(m != 0)
			{
				CachedBuilder.Append(ToString(m));
				CachedBuilder.Append("m ");
			}

			int s = time.Seconds;
			int ms = Mathf.Abs(time.Milliseconds);

			if(s != 0 || ms != 0 || (d == 0 && h == 0 && m == 0))
			{
				CachedBuilder.Append(ToString(s));
				
				if(ms != 0)
				{
					//remove trailing zeroes
					while(ms % 10 == 0)
					{
						ms = ms / 10;
					}

					CachedBuilder.Append(".");
					CachedBuilder.Append(ToString(ms));
				}
				CachedBuilder.Append("s");
			}

			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string ToString(UnityEngine.SceneManagement.Scene scene)
		{
			if(!scene.IsValid())
			{
				return Concat("\"", scene.name, "\"(Invalid)");
			}
			if(scene.isLoaded)
			{
				return Concat("\"", scene.name, "\"(Loaded)");
			}
			return Concat("\"", scene.name, "\"");
		}

		public static string ToString(IDrawer target)
		{
			return target == null ? "null" : target.ToString();
		}

		#if UNITY_EDITOR
		public static string ToString(UnityEditor.SerializedProperty target)
		{
			return target == null ? "null" : target.propertyPath;
		}
		#endif
		
		/// <summary>
		/// Converts any given object supported by Unity serialization into a simple and readable string form.
		/// For more complex objects it can use Json serialization.
		/// If length of resulting string would be really long, will just return type name instead.
		/// </summary>
		/// <param name="target">target to convert to string form</param>
		/// <returns>Target represented in human readable string format.</returns>
		public static string ToString(object target)
		{
			recursiveCallCount = 0;
			return ToStringInternal(target);
		}

		/// <summary>
		/// Converts any given object supported by Unity serialization into a simple and readable string form.
		/// For more complex objects it can use Json serialization.
		/// If length of resulting string would be really long, will just return type name instead.
		/// </summary>
		/// <param name="target">target to convert to string form</param>
		/// <returns>Target represented in human readable string format.</returns>
		private static string ToStringInternal(object target)
		{
			if(target == null)
			{
				return "null";
			}

			if(target is Object && target as Object == null)
			{
				return "null";
			}

			var type = target.GetType();
			
			if(type.IsPrimitive)
			{
				if(type == Types.Int)
				{
					return ToString((int)target);
				}

				return target.ToString();
			}

			var asString = target as string;
			if(asString != null)
			{
				return string.Concat("\"", asString, "\"");
			}

			if(type.IsEnum)
			{
				return target.ToString();
			}

			recursiveCallCount++;
			if(recursiveCallCount > MaxRecursiveCallCount)
			{
				recursiveCallCount = 0;
				#if DEV_MODE
				Debug.LogError("StringUtils.ToString max recursive call count ("+MaxRecursiveCallCount+") exceeded with target of type "+TypeToString(target));
				#endif
				return TypeToString(target);
			}

			var ienumerable = target as IEnumerable;
			if(ienumerable != null)
			{
				var asArray = target as Array;
				if(asArray != null)
				{
					return ToString(asArray);
				}

				var asDictionary = target as IDictionary;
				if(asDictionary != null)
				{
					return ToString(asDictionary);
				}

				var asCollection = target as ICollection;
				if(asCollection != null)
				{
					return ToString(asCollection);
				}

				// Only use IEnumerable based ToString if DeclaringType is IEnumerable. Don's use for other classes
				// because they might override ToString() or have many properties besides those accessible via the enumerator.
				if(target.GetType() == typeof(IEnumerable) || target.GetType().DeclaringType == typeof(IEnumerable<>))
				{
					return ToString(ienumerable);
				}
			}
			
			var asEvent = target as Event;
			if(asEvent != null)
			{
				return ToString(asEvent);
			}

			var asGUIContent = target as GUIContent;
			if(asGUIContent != null)
			{
				return ToString(asGUIContent);
			}

			#if UNITY_EDITOR
			if(type == Types.MonoScript)
			{
				return ToString(target as Object);
			}
			#endif

			if(type == Types.TextAsset)
			{
				return ToString(target as Object);
			}
			
			try
			{
				string toStringResult = target.ToString();
				if(!string.Equals(toStringResult, type.ToString()))
				{
					if(toStringResult.Length <= MaxToStringResultLength)
					{
						return toStringResult;
					}
					//UPDATE: No longer ignoring, just getting a Substring
					#if DEV_MODE && DEBUG_TO_STRING_TOO_LONG
					//if(!string.Equals(toStringResult, type.ToString())) { Debug.LogWarning("Ignoring "+ type.Name+".ToString() result because length "+ toStringResult.Length+" > "+ MaxToStringResultLength+":\n"+toStringResult); }
					#endif

					return toStringResult.Substring(MaxToStringResultLength);
				}
			}
			catch(Exception e)
			{
				Debug.LogError(e);
			}

			var asObject = target as Object;
			if(asObject != null)
			{
				return ToString(asObject);
			}
			
			var asDelegate = target as MulticastDelegate;
			if(asDelegate != null)
			{
				return ToString(asDelegate);
			}

			return ToStringSansNamespace(type);
		}

		public static string ToString(Object target)
		{
			if(target == null)
			{
				return "null";
			}

			var trans = target.Transform();
			if(trans != null)
			{
				return Concat(ToString(trans.GetHierarchyPath()), "(", target.GetType(), ")");
			}

			return Concat(ToString(target.name), "(", target.GetType(), ")");
		}

		public static string ToString([CanBeNull]IList<Object> list, string delimiter = ",")
		{
			if(list == null)
			{
				return "null";
			}

			int lastIndex = list.Count - 1;
			if(lastIndex == -1)
			{
				return "{}";
			}

			var builder = StringBuilderPool.Create();
			builder.Append('{');
			for(int n = 0; n < lastIndex; n++)
			{
				Append(list[n], builder);
				builder.Append(delimiter);
			}
			Append(list[lastIndex], builder);
			builder.Append('}');
			
			return StringBuilderPool.ToStringAndDispose(ref builder);
		}

		public static void Append(Object target, StringBuilder toBuilder)
		{
			if(target == null)
			{
				toBuilder.Append("null");
				return;
			}

			toBuilder.Append("\"");
			var trans = target.Transform();
			if(trans != null)
			{
				toBuilder.Append(trans.GetHierarchyPath());
			}
			else
			{
				toBuilder.Append(target.name);
			}
			toBuilder.Append("\"(");
			toBuilder.Append(target.GetType().Name);
			toBuilder.Append(")");
		}

		public static string ToString(MethodInfo method)
		{
			var sb = StringBuilderPool.Create();

			sb.Append(method.Name);

			var parameters = method.GetParameters();
			int parameterCount = parameters.Length;
			if(parameterCount > 0)
			{
				sb.Append('(');

				sb.Append(ToString(parameters[0].ParameterType));

				for(int n = 1; n < parameterCount; n++)
				{
					var parameter = parameters[n];
					sb.Append(',');
					sb.Append(ToString(parameter.ParameterType));
				}
				sb.Append(')');
			}

			return StringBuilderPool.ToStringAndDispose(ref sb);
		}

		public static string ToString(ConstructorInfo constructor)
		{
			var sb = StringBuilderPool.Create();

			sb.Append(constructor.Name);

			var parameters = constructor.GetParameters();
			int parameterCount = parameters.Length;
			if(parameterCount > 0)
			{
				sb.Append('(');

				sb.Append(ToString(parameters[0].ParameterType));

				for(int n = 1; n < parameterCount; n++)
				{
					var parameter = parameters[n];
					sb.Append(',');
					sb.Append(ToString(parameter.ParameterType));
				}
				sb.Append(')');
			}

			return StringBuilderPool.ToStringAndDispose(ref sb);
		}
		
		public static string ToString(Array array, string delimiter = ",")
		{
			if(array == null)
			{
				return "null";
			}

			switch(array.Rank)
			{
				case 1:
					return Array1DToString(array, delimiter);
				case 2:
					return Array2DToString(array, delimiter);
				default:
					return TypeToString(array);
			}
		}

		public static string Array1DToString(Array array, string delimiter = ",")
		{
			if(array == null)
			{
				return "null";
			}
		
			int lastIndex = array.Length - 1;
			if(lastIndex == -1)
			{
				return "{}";
			}

			var builder = StringBuilderPool.Create();
			builder.Append('{');
			for(int n = 0; n < lastIndex; n++)
			{
				builder.Append(ToString(array.GetValue(n)));
				builder.Append(delimiter);
			}
			builder.Append(ToString(array.GetValue(lastIndex)));
			builder.Append('}');

			return StringBuilderPool.ToStringAndDispose(ref builder);
		}

		public static string Array2DToString(Array array, string delimiter = ",")
		{
			if(array == null)
			{
				return "null";
			}

			int lastIndexY = array.GetLength(1) - 1;
			if(lastIndexY == -1)
			{
				return "{}";
			}

			int lastIndexX = array.GetLength(0) - 1;

			#if DEV_MODE
			Debug.Log("Array2DToString called for array with GetLength(0)="+(lastIndexX+1)+", GetLength(1)="+(lastIndexY+1));
			#endif

			var builder = StringBuilderPool.Create();
			builder.Append('{');
			for(int y = 0; y < lastIndexY; y++)
			{
				builder.Append('{');
				if(lastIndexX > 0)
				{
					for(int x = 0; x < lastIndexX; x++)
					{
						builder.Append(ToString(array.GetValue(x, y)));
						builder.Append(delimiter);
					}
					builder.Append(ToString(array.GetValue(lastIndexX, y)));
				}
				builder.Append('}');
				builder.Append(delimiter);
			}
			if(lastIndexX > 0)
			{
				builder.Append('{');
				if(lastIndexY > 0)
				{
					for(int x = 0; x < lastIndexX; x++)
					{
						builder.Append(ToString(array.GetValue(x, lastIndexY)));
						builder.Append(delimiter);
					}
					builder.Append(ToString(array.GetValue(lastIndexX, lastIndexY)));
				}
				builder.Append('}');
			}
			builder.Append('}');

			return StringBuilderPool.ToStringAndDispose(ref builder);
		}

		public static string TypeToString([CanBeNull]object target)
		{
			if(target is Object)
			{
				return TypeToString(target as Object);
			}

			if(target == null)
			{
				return "null";
			}

			return ToString(target.GetType());
		}

		public static string TypeToString([CanBeNull]Object target)
		{
			if(target == null)
			{
				return "null";
			}

			return ToString(target.GetType());
		}

		public static string TypesToString([CanBeNull]IList list, string delimiter = ",")
		{
			if(list == null)
			{
				return "null";
			}
		
			int lastIndex = list.Count - 1;
			if(lastIndex == -1)
			{
				return "{}";
			}

			var builder = StringBuilderPool.Create();
			builder.Append('{');
			for(int n = 0; n < lastIndex; n++)
			{
				builder.Append(TypeToString(list[n]));
				builder.Append(delimiter);
			}
			builder.Append(TypeToString(list[lastIndex]));
			builder.Append('}');

			return StringBuilderPool.ToStringAndDispose(ref builder);
		}

		public static string ToString(byte[] bytes)
		{
			return Encoding.UTF8.GetString(bytes);
		}

		public static string ToString([CanBeNull]IList list, string delimiter = ", ")
		{
			if(list == null)
			{
				return "null";
			}

			int lastIndex = list.Count - 1;
			if(lastIndex == -1)
			{
				return "{}";
			}

			//don's use the cached StringBuilder because it could get cleared
			//when calling ToString for collection contents
			var builder = StringBuilderPool.Create();

			builder.Append('{');
			for(int n = 0; n < lastIndex; n++)
			{
				recursiveCallCount = 0;

				builder.Append(ToString(list[n]));
				builder.Append(delimiter);
			}

			builder.Append(ToString(list[lastIndex]));
			builder.Append('}');

			return StringBuilderPool.ToStringAndDispose(ref builder);
		}

		public static string ToString([CanBeNull]IDictionary collection, string delimiter = ", ")
		{
			if(collection == null)
			{
				return "null";
			}

			int lastIndex = collection.Count - 1;
			if(lastIndex == -1)
			{
				return "{}";
			}

			recursiveCallCount = 0;

			//don's use the cached StringBuilder because it could get cleared
			//when calling ToString for collection contents
			var builder = StringBuilderPool.Create();
			builder.Append('{');
			foreach(DictionaryEntry entry in collection)
			{
				builder.Append(ToString(entry));
				builder.Append(delimiter);
			}
			builder[builder.Length-1] = '}';
			return StringBuilderPool.ToStringAndDispose(ref builder);
		}

		public static string ToString(ICollection collection, string delimiter = ", ")
		{
			if(collection == null)
			{
				return "null";
			}
		
			int lastIndex = collection.Count - 1;
			if(lastIndex == -1)
			{
				return "{}";
			}

			recursiveCallCount = 0;

			//don's use the cached StringBuilder because it could get cleared
			//when calling ToString for collection contents
			var builder = StringBuilderPool.Create();
			builder.Append('{');
			foreach(var entry in collection)
			{
				builder.Append(ToString(entry));
				builder.Append(delimiter);
			}
			builder[builder.Length-1] = '}';

			return StringBuilderPool.ToStringAndDispose(ref builder);
		}

		public static string LengthToString([CanBeNull]ICollection collection)
		{
			return collection == null ? "null" : ToString(collection.Count);
		}

		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		public static string ToString([CanBeNull]string text)
		{
			return text == null ? "null" : Concat("\"", text, "\"");
		}

		public static string ToString([CanBeNull]IEnumerable collection, string delimiter = ", ")
		{
			if(collection == null)
			{
				return "null";
			}

			//don's use the cached StringBuilder because it could get cleared
			//when calling ToString for collection contents
			var builder = StringBuilderPool.Create();

			builder.Append('{');
			
			foreach(var entry in collection)
			{
				builder.Append(ToString(entry));
				builder.Append(delimiter);
			}

			if(builder.Length == 1)
			{
				builder.Append('}');
			}
			else
			{
				// replace last delimiter character with collection end character
				// NOTE: Currently only works with delimiters that are one character long!
				builder[builder.Length - 1] = '}';
			}
			
			return StringBuilderPool.ToStringAndDispose(ref builder);
		}

		public static string ToString([CanBeNull]IEnumerable<Object> collection, string delimiter = ", ")
		{
			if(collection == null)
			{
				return "null";
			}

			//don's use the cached StringBuilder because it could get cleared
			//when calling ToString for collection contents
			var builder = StringBuilderPool.Create();

			builder.Append('{');
			
			foreach(var unityObject in collection)
			{
				Append(unityObject, builder);
				builder.Append(delimiter);
			}

			if(builder.Length == 1)
			{
				builder.Append('}');
			}
			else
			{
				// replace last delimiter character with collection end character
				// NOTE: Currently only works with delimiters that are one character long!
				builder[builder.Length - 1] = '}';
			}
			
			return StringBuilderPool.ToStringAndDispose(ref builder);
		}

		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		public static string Concat(string a, string b)
		{
			return string.Concat(a,b);
		}

		public static string Concat(string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8, string s9, string s10)
		{
			CachedBuilder.Append(s1);
			CachedBuilder.Append(s2);
			CachedBuilder.Append(s3);
			CachedBuilder.Append(s4);
			CachedBuilder.Append(s5);
			CachedBuilder.Append(s6);
			CachedBuilder.Append(s7);
			CachedBuilder.Append(s8);
			CachedBuilder.Append(s9);
			CachedBuilder.Append(s10);
			string result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		public static string Concat(string s, int number)
		{
			return string.Concat(s, ToString(number));
		}

		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		public static string Concat(int number, string s)
		{
			return string.Concat(ToString(number), s);
		}

		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		public static string Concat(string a, string b, string c)
		{
			return string.Concat(a,b,c);
		}

		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		public static string Concat(string a, string b, string c, string d)
		{
			return string.Concat(a,b,c,d);
		}

		public static string Concat(string a, string b, string c, string d, string e)
		{
			CachedBuilder.Append(a);
			CachedBuilder.Append(b);
			CachedBuilder.Append(c);
			CachedBuilder.Append(d);
			CachedBuilder.Append(e);
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string Concat(string a, string b, string c, string d, string e, string f)
		{
			CachedBuilder.Append(a);
			CachedBuilder.Append(b);
			CachedBuilder.Append(c);
			CachedBuilder.Append(d);
			CachedBuilder.Append(e);
			CachedBuilder.Append(f);
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string Concat(string s, string s2, Type type, string s3, string s4, string s5)
		{
			CachedBuilder.Append(s);
			CachedBuilder.Append(s2);
			CachedBuilder.Append(type);
			CachedBuilder.Append(s3);
			CachedBuilder.Append(s4);
			CachedBuilder.Append(s5);
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string Concat(string s1, Type type, string s2)
		{
			CachedBuilder.Append(s1);
			CachedBuilder.Append(type);
			CachedBuilder.Append(s2);
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string Concat(string s1, string s2, Type type, string s3)
		{
			CachedBuilder.Append(s1);
			CachedBuilder.Append(s2);
			CachedBuilder.Append(ToStringSansNamespace(type));
			CachedBuilder.Append(s3);
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string Concat(string s1, Type type1, string s2, Type type2)
		{
			CachedBuilder.Append(s1);
			CachedBuilder.Append(ToStringSansNamespace(type1));
			CachedBuilder.Append(s2);
			CachedBuilder.Append(ToStringSansNamespace(type2));
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string Concat(string s1, Type type1, string s2, Type type2, string s3)
		{
			CachedBuilder.Append(s1);
			CachedBuilder.Append(type1);
			CachedBuilder.Append(s2);
			CachedBuilder.Append(type2);
			CachedBuilder.Append(s3);
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string Concat(string s1, Type type1, string s2, Type type2, string s3, Type type3, string s4)
		{
			CachedBuilder.Append(s1);
			CachedBuilder.Append(type1);
			CachedBuilder.Append(s2);
			CachedBuilder.Append(type2);
			CachedBuilder.Append(s3);
			CachedBuilder.Append(type3);
			CachedBuilder.Append(s4);
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string Concat(string s, string s2, string s3, Type type)
		{
			CachedBuilder.Append(s);
			CachedBuilder.Append(s2);
			CachedBuilder.Append(s3);
			CachedBuilder.Append(ToStringSansNamespace(type));
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string Concat(string a, int b, string c)
		{
			CachedBuilder.Append(a);
			CachedBuilder.Append(ToString(b));
			CachedBuilder.Append(c);
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string Concat(float f1, string s, float f2)
		{
			CachedBuilder.Append(ToString(f1));
			CachedBuilder.Append(s);
			CachedBuilder.Append(ToString(f2));
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string Concat(int i1, string s, int i2)
		{
			CachedBuilder.Append(ToString(i1));
			CachedBuilder.Append(s);
			CachedBuilder.Append(ToString(i2));
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string Concat(string a, string b, int c, string d)
		{
			CachedBuilder.Append(a);
			CachedBuilder.Append(b);
			CachedBuilder.Append(ToString(c));
			CachedBuilder.Append(d);
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string Concat(string a, int b, string c, object d)
		{
			CachedBuilder.Append(a);
			CachedBuilder.Append(ToString(b));
			CachedBuilder.Append(c);
			
			var ds = d as string;
			if(ds != null)
			{
				CachedBuilder.Append(ds);
			}
			else
			{
				CachedBuilder.Append(ToString(d));
			}
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string Concat(int num, string s1, Type type, string s2)
		{
			CachedBuilder.Append(ToString(num));
			CachedBuilder.Append(s1);
			CachedBuilder.Append(ToStringSansNamespace(type));
			CachedBuilder.Append(s2);
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string Concat(string a, int b, string c, int d, string e)
		{
			CachedBuilder.Append(a);
			CachedBuilder.Append(ToString(b));
			CachedBuilder.Append(c);
			CachedBuilder.Append(ToString(d));
			CachedBuilder.Append(e);
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}
		
		public static string Concat(string s1, string s2, int n, string s3, string s4)
		{
			CachedBuilder.Append(s1);
			CachedBuilder.Append(ToString(s2));
			CachedBuilder.Append(n);
			CachedBuilder.Append(ToString(s3));
			CachedBuilder.Append(s4);
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string Concat(string a, int b, string c, int d, string e, int f, string g)
		{
			CachedBuilder.Append(a);
			CachedBuilder.Append(ToString(b));
			CachedBuilder.Append(c);
			CachedBuilder.Append(ToString(d));
			CachedBuilder.Append(e);
			CachedBuilder.Append(ToString(f));
			CachedBuilder.Append(g);
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string Concat(float f1, string s1, float f2, string s2, float f3)
		{
			CachedBuilder.Append(ToString(f1));
			CachedBuilder.Append(s1);
			CachedBuilder.Append(ToString(f2));
			CachedBuilder.Append(s2);
			CachedBuilder.Append(ToString(f3));
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string Concat(float f1, string s1, float f2, string s2, float f3, string s3, float f4)
		{
			CachedBuilder.Append(ToString(f1));
			CachedBuilder.Append(s1);
			CachedBuilder.Append(ToString(f2));
			CachedBuilder.Append(s2);
			CachedBuilder.Append(ToString(f3));
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string Concat(int i1, string s1, int i2, string s2, int i3)
		{
			CachedBuilder.Append(ToString(i1));
			CachedBuilder.Append(s1);
			CachedBuilder.Append(ToString(i2));
			CachedBuilder.Append(s2);
			CachedBuilder.Append(ToString(i3));
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string Concat(int i1, string s1, int i2, string s2, int i3, string s3, int i4)
		{
			CachedBuilder.Append(ToString(i1));
			CachedBuilder.Append(s1);
			CachedBuilder.Append(ToString(i2));
			CachedBuilder.Append(s2);
			CachedBuilder.Append(ToString(i3));
			CachedBuilder.Append(s3);
			CachedBuilder.Append(ToString(i4));
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string Concat(string s, string s2, string s3, string s4, string s5, int n, string s8)
		{
			CachedBuilder.Append(s);
			CachedBuilder.Append(s2);
			CachedBuilder.Append(s3);
			CachedBuilder.Append(s4);
			CachedBuilder.Append(s5);
			CachedBuilder.Append(n);
			CachedBuilder.Append(s8);
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string Concat(string a, string b, string c, string d, string e, string f, string g)
		{
			CachedBuilder.Append(a);
			CachedBuilder.Append(b);
			CachedBuilder.Append(c);
			CachedBuilder.Append(d);
			CachedBuilder.Append(e);
			CachedBuilder.Append(f);
			CachedBuilder.Append(g);
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string Concat(string s, string s2, string s3, string s4, string s5, string s6, string s7, int n, string s8)
		{
			CachedBuilder.Append(s);
			CachedBuilder.Append(s2);
			CachedBuilder.Append(s3);
			CachedBuilder.Append(s4);
			CachedBuilder.Append(s5);
			CachedBuilder.Append(s6);
			CachedBuilder.Append(s7);
			CachedBuilder.Append(n);
			CachedBuilder.Append(s8);
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string Concat(string s, string s2, Type type, string s3, string s4, string s5, int number, string s6)
		{
			CachedBuilder.Append(s);
			CachedBuilder.Append(s2);
			CachedBuilder.Append(ToStringSansNamespace(type));
			CachedBuilder.Append(s3);
			CachedBuilder.Append(s4);
			CachedBuilder.Append(s5);
			CachedBuilder.Append(ToString(number));
			CachedBuilder.Append(s6);
			var result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}
		
		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		public static bool IsSpecialCharacter(this char character)
		{
			switch(character)
			{
				case '.':
				case '=':
				case '+':
				case '-':
				case '*':
				case '/':
				case '\\':
				case '%':
				case '<':
				case '>':
				case '!':
				case ';':
				case '?':
				case ':':
				case '&':
				case '|':
				case '(':
				case ')':
				case '{':
				case '}':
				case '[':
				case ']':
				case '~':
				case '"':
				case '\'':
					return true;
				default:
					return false;
			}
		}

		[MethodImpl(256)] //MethodImplOptions.AggressiveInlining
		public static bool IsSpace(this char character)
		{
			return character.Equals(' ');
		}


		[MethodImpl(256)] //MethodImplOptions.AggressiveInlining
		public static bool IsWhiteSpace(this char character)
		{
			switch(character)
			{
				case ' ':
				case '\t':
				case '\n':
				case '\r':
					return true;
				default:
					return false;
			}
		}

		[MethodImpl(256)] //MethodImplOptions.AggressiveInlining
		public static bool IsLineEnd(this char character)
		{
			switch(character)
			{
				case '\r':
				case '\n':
					return true;
				default:
					return false;
			}
		}

		public static bool SubstringEqualsWholeWord(string text, int startIndex, string substring)
		{
			int count = substring.Length;
			int lastIndex = startIndex + count - 1;
			int textLength = text.Length;

			if(textLength <= lastIndex)
			{
				return false;
			}
			
			for(int n = count - 1; n >= 0; n--)
			{
				if(text[startIndex + n] != substring[n])
				{
					return false;
				}
			}

			lastIndex++;
			if(textLength > lastIndex && !text[lastIndex].IsSpecialCharacter() && !text[lastIndex].IsWhiteSpace())
			{
				return false;
			}

			startIndex--;
			if(startIndex >= 0 && !text[startIndex].IsSpecialCharacter() && !text[startIndex].IsWhiteSpace())
			{
				return false;
			}

			return true;
		}

		public static string ToString([CanBeNull]Object[] array, string delimiter = ", ")
		{
			if(array == null)
			{
				return "null";
			}

			int lastIndex = array.Length - 1;
			if(lastIndex == -1)
			{
				return "{}";
			}

			var builder = StringBuilderPool.Create();
			builder.Append('{');
			for(int n = 0; n < lastIndex; n++)
			{
				builder.Append(ToString(array[n]));
				builder.Append(delimiter);
			}
			builder.Append(ToString(array[lastIndex]));
			builder.Append('}');
			
			return StringBuilderPool.ToStringAndDispose(ref builder);
		}

		public static string NamesToString([CanBeNull]Object[] array, string delimiter = ", ", bool brackets = false)
		{
			if(array == null)
			{
				return "null";
			}

			int lastIndex = array.Length - 1;
			if(lastIndex == -1)
			{
				return "{}";
			}

			var builder = StringBuilderPool.Create();
			if(brackets)
			{
				builder.Append('{');
			}
			for(int n = 0; n < lastIndex; n++)
			{
				builder.Append(array[n] == null ? "null" : array[n].name);
				builder.Append(delimiter);
			}
			builder.Append(array[lastIndex] == null ? "null" : array[lastIndex].name);
			if(brackets)
			{
				builder.Append('}');
			}
			
			return StringBuilderPool.ToStringAndDispose(ref builder);
		}

		public static string ToString(GUIContent target)
		{
			#if DEV_MODE
			return target == null ? "null" : target == GUIContent.none ? "none" : string.Concat('\"', target.text, '\"');
			#else
			return target == null ? "null" : string.Concat('\"', target.text, '\"');
			#endif
		}
		
		public static string ToString(MulticastDelegate target)
		{
			if(target == null)
			{
				return "null";
			}

			var invocationList = target.GetInvocationList();
			
			int lastIndex = invocationList.Length - 1;
			if(lastIndex == -1)
			{
				return "{}";
			}

			if(lastIndex == 0)
			{
				return ToString(invocationList[0]);
			}

			var builder = StringBuilderPool.Create();
			builder.Append('{');
			for(int n = 0; n < lastIndex; n++)
			{
				ToString(invocationList[n], ref builder);
				builder.Append(',');
			}
			ToString(invocationList[lastIndex], ref builder);
			builder.Append('}');

			return StringBuilderPool.ToStringAndDispose(ref builder);
		}

		public static string ToString([CanBeNull]Delegate del)
		{
			if(del == null)
			{
				return "null";
			}

			var target = del.Target;
			if(target == null)
			{
				return del.Method.Name;
			}

			var builder = StringBuilderPool.Create();
			builder.Append(TypeToString(target));
			builder.Append('.');
			builder.Append(del.Method.Name);
			return StringBuilderPool.ToStringAndDispose(ref builder);
		}

		public static void ToString([CanBeNull]Delegate del, [NotNull]ref StringBuilder builder)
		{
			if(del == null)
			{
				builder.Append("null");
			}

			var target = del.Target;
			if(target != null)
			{
				builder.Append(TypeToString(target));
				builder.Append('.');
			}
			builder.Append(del.Method.Name);
		}

		public static string ToString(Vector2 v)
		{
			CachedBuilder.Append("(");
			CachedBuilder.Append(ToString(v.x));
			CachedBuilder.Append(",");
			CachedBuilder.Append(ToString(v.y));
			CachedBuilder.Append(")");
			string result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string ToString(Vector3 v)
		{
			CachedBuilder.Append("(");
			CachedBuilder.Append(ToString(v.x));
			CachedBuilder.Append(",");
			CachedBuilder.Append(ToString(v.y));
			CachedBuilder.Append(",");
			CachedBuilder.Append(ToString(v.z));
			CachedBuilder.Append(")");
			string result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string ToString(Rect rect)
		{
			CachedBuilder.Append("(");
			CachedBuilder.Append(ToString(rect.x));
			CachedBuilder.Append(",");
			CachedBuilder.Append(ToString(rect.y));
			CachedBuilder.Append(",");
			CachedBuilder.Append(ToString(rect.width));
			CachedBuilder.Append(",");
			CachedBuilder.Append(ToString(rect.height));
			CachedBuilder.Append(")");
			string result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		public static string ToString(Bounds bounds)
		{
			CachedBuilder.Append("(x=");
			CachedBuilder.Append(ToString(bounds.min.x));
			CachedBuilder.Append("...");
			CachedBuilder.Append(ToString(bounds.max.x));
			CachedBuilder.Append(", y=");
			CachedBuilder.Append(ToString(bounds.min.y));
			CachedBuilder.Append("...");
			CachedBuilder.Append(ToString(bounds.max.y));
			CachedBuilder.Append(", z=");
			CachedBuilder.Append(ToString(bounds.min.z));
			CachedBuilder.Append("...");
			CachedBuilder.Append(ToString(bounds.max.z));
			CachedBuilder.Append(")");
			string result = CachedBuilder.ToString();
			CachedBuilder.Length = 0;
			return result;
		}

		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		public static string ToString<TKey,TValue>(KeyValuePair<TKey,TValue> keyValuePair)
		{
			return Concat("KeyValuePair(", ToString(keyValuePair.Key), ",", ToString(keyValuePair.Value), ")");
		}

		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		public static string ToString(DictionaryEntry dictionaryEntry)
		{
			return Concat("DictionaryEntry(", ToString(dictionaryEntry.Key), ",", ToString(dictionaryEntry.Value), ")");
		}

		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		private static string ToStringInternal(DictionaryEntry dictionaryEntry)
		{
			return Concat("DictionaryEntry(", ToStringInternal(dictionaryEntry.Key), ",", ToStringInternal(dictionaryEntry.Value), ")");
		}

		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		public static string ToStringSansNamespace([CanBeNull]Type type)
		{
			if(type == null)
			{
				return "null";
			}
			return TypeExtensions.GetShortName(type);
		}

		/// <summary>
		/// Removes namespaces from full type name.
		/// </summary>
		/// <param name="fullName"> Full type name. </param>
		/// <returns> Type name without namespace. </returns>
		public static string RemoveNamespacesFromFullTypeName([NotNull]string fullName)
		{
			// E.g. "System.KeyValuePair<System.Int32, System.String>" => "KeyValuePair<Int32, String>",
			// or "System.Collections.Generic.List<System.Collections.Generic.List<System.Int32>> => List<List<Int32>>

			#if DEV_MODE && PI_ASSERTATIONS
			// types in the form "System.List^1" are not supported
			Debug.Assert(fullName.IndexOf('^') == -1, fullName);
			// types in the form "List`1.T" are not supported
			Debug.Assert(fullName.IndexOf("`1", StringComparison.Ordinal) == -1, fullName);
			#endif

			int n = fullName.LastIndexOf('.');
			if(n == -1)
			{
				return fullName;
			}

			int g = fullName.IndexOf('<');
			if(g == -1 || g > n)
			{
				return fullName.Substring(n + 1);
			}

			int i = 0;
			var sb = StringBuilderPool.Create();
			int count = fullName.Length;
			do
			{
				#if DEV_MODE && PI_ASSERTATIONS
				string debugText = fullName+" i="+i+", g="+g+", Length="+fullName.Length;
				Debug.Assert(i >= 0, debugText);
				Debug.Assert(i < fullName.Length, debugText);
				Debug.Assert(g != -1, debugText);
				Debug.Assert(g > i, debugText);
				Debug.Assert(g + 1 < fullName.Length, debugText);
				#endif

				string part = fullName.Substring(i, g + 1 - i);
				n = part.LastIndexOf('.');
				if(n == -1)
				{
					sb.Append(part);
				}
				else
				{
					sb.Append(part.Substring(n + 1));
				}
				
				i = g + 1;

				if(i >= count)
				{
					return StringBuilderPool.ToStringAndDispose(ref sb);
				}

				g = fullName.IndexOf('<', i);
			}
			while(g != -1);
			
			string lastPart = fullName.Substring(i);
			sb.Append(fullName.Substring(i));
			n = lastPart.LastIndexOf('.');
			if(n == -1)
			{
				sb.Append(lastPart);
			}
			else
			{
				sb.Append(lastPart.Substring(n + 1));
			}

			#if DEV_MODE
			Debug.Log("RemoveNamespacesFromFullTypeName: \""+fullName+" => \"" + sb + "\"");
			#endif

			return StringBuilderPool.ToStringAndDispose(ref sb);
		}

		public static string ToString(Type type)
		{
			if(type == null)
			{
				return "null";
			}

			switch(Type.GetTypeCode(type))
			{
				case TypeCode.Boolean:
					return "bool";
				case TypeCode.Char:
					return "char";
				case TypeCode.SByte:
					return "sbyte";
				case TypeCode.Byte:
					return "byte";
				case TypeCode.Int16:
					return "short";
				case TypeCode.UInt16:
					return "ushort";
				case TypeCode.Int32:
					return "int";
				case TypeCode.UInt32:
					return "uint";
				case TypeCode.Int64:
					return "long";
				case TypeCode.UInt64:
					return "ulong";
				case TypeCode.Single:
					return "float";
				case TypeCode.Double:
					return "double";
				case TypeCode.Decimal:
					return "decimal";
				case TypeCode.String:
					return "string";
			}

			return TypeExtensions.GetFullName(type);
		}

		internal static string ToString([NotNull]Type type, char namespaceDelimiter)
		{
			if(type.IsGenericParameter)
			{
				return type.Name;
			}

			var builder = StringBuilderPool.Create();
			
			if(type.IsArray)
			{
				builder.Append(ToString(type.GetElementType(), namespaceDelimiter));
				int rank = type.GetArrayRank();
				switch(rank)
				{
					case 1:
						builder.Append("[]");
						break;
					case 2:
						builder.Append("[,]");
						break;
					case 3:
						builder.Append("[,,]");
						break;
					default:
						builder.Append('[');
						for(int n = 1; n < rank; n++)
						{
							builder.Append(',');
						}
						builder.Append(']');
						break;
				}
				return StringBuilderPool.ToStringAndDispose(ref builder);
			}

			if(namespaceDelimiter != '\0' && type.Namespace != null)
			{
				var namespacePart = type.Namespace;
				if(namespaceDelimiter != '.')
				{
					namespacePart = namespacePart.Replace('.', namespaceDelimiter);
				}
				builder.Append(namespacePart);
				builder.Append(namespaceDelimiter);
			}
			else
			{
				switch(Type.GetTypeCode(type))
				{
					case TypeCode.Boolean:
						return "bool";
					case TypeCode.Char:
						return "char";
					case TypeCode.SByte:
						return "sbyte";
					case TypeCode.Byte:
						return "byte";
					case TypeCode.Int16:
						return "short";
					case TypeCode.UInt16:
						return "ushort";
					case TypeCode.Int32:
						return "int";
					case TypeCode.UInt32:
						return "uint";
					case TypeCode.Int64:
						return "long";
					case TypeCode.UInt64:
						return "ulong";
					case TypeCode.Single:
						return "float";
					case TypeCode.Double:
						return "double";
					case TypeCode.Decimal:
						return "decimal";
					case TypeCode.String:
						return "string";
				}
			}

			#if DEV_MODE
			if(type.Name.IndexOf("Dictionary", StringComparison.Ordinal) != -1)
			{
				var o = new List<object>();
				
				o.Add(type.Name);
				o.Add(" - IsGenericType=");
				o.Add(type.IsGenericType);
				if(type.IsGenericType)
				{
					o.Add(", GenericTypeDefinition=");
					o.Add(type.GetGenericTypeDefinition());
				}
				o.Add(", DeclaringType=");
				o.Add(type.DeclaringType);
				o.Add(", IsGenericTypeDefinition=");
				o.Add(type.IsGenericTypeDefinition);
				o.Add(", IsGenericParameter=");
				o.Add(type.IsGenericParameter);
				o.Add(", GenericArguments=");
				o.Add(type.GetGenericArguments());

				ToColorizedString(o.ToArray());
			}

			//if(type.Name.IndexOf("Dictionary", StringComparison.Ordinal) != -1) { Debug.Log(ToColorizedString(type.Name, " - IsGenericType=", type.IsGenericType, ", DeclaringType=", type.DeclaringType, ", IsGenericTypeDefinition=", type.IsGenericTypeDefinition, ", IsGenericParameter=", type.IsGenericParameter, ", GenericArguments=", type.GetGenericArguments(), ", GenericTypeDefinition=", type.GetGenericTypeDefinition())); }
			#endif

			var containingClassType = type.DeclaringType;
			if(containingClassType != null && type != containingClassType) 
			{
				builder.Append(containingClassType.Name);
				builder.Append('.');
			}
			
			if(!type.IsGenericType)
			{
				builder.Append(type.Name);
				return StringBuilderPool.ToStringAndDispose(ref builder);
			}

			var nullableUnderlyingType = Nullable.GetUnderlyingType(type);
			if(nullableUnderlyingType != null)
			{
				// "Int?" instead of "Nullable<Int>"
				builder.Append(ToString(nullableUnderlyingType, '\0'));
				builder.Append("?");
				return StringBuilderPool.ToStringAndDispose(ref builder);
			}
			
			var genericTypes = type.GetGenericArguments();
			
			var name = type.Name;
			builder.Append(name.Substring(0, name.Length - 2));
			builder.Append('<');
			builder.Append(ToString(genericTypes[0], '\0'));
			int count = genericTypes.Length;
			for(int n = 1; n < count; n++)
			{
				builder.Append(',');
				builder.Append(TypeExtensions.GetShortName(genericTypes[n]));
			}
			builder.Append('>');
			return StringBuilderPool.ToStringAndDispose(ref builder);
		}
		
		public static string RemoveWhitespace(string str)
		{
			var chars = str.ToCharArray();
			int resultIndex = 0;
			int length = str.Length;
			for(int i = 0; i < length; i++)
			{
				var c = chars[i];
				if(!IsWhiteSpace(c))
				{
					chars[resultIndex] = c;
					resultIndex++;
				}
			}
			return new string(chars, 0, resultIndex);
		}

		public static string RemoveSpaces(string str)
		{
			var chars = str.ToCharArray();
			int resultIndex = 0;
			int length = str.Length;
			for(int i = 0; i < length; i++)
			{
				var c = chars[i];
				if(!IsSpace(c))
				{
					chars[resultIndex] = c;
					resultIndex++;
				}
			}
			return new string(chars, 0, resultIndex);
		}

		public static bool ContainsWhitespace(string test)
		{
			if(test.IndexOf(' ') != -1)
			{
				return true;
			}
			if(test.IndexOf('\t') != -1)
			{
				return true;
			}
			if(test.IndexOf('\r') != -1)
			{
				return true;
			}
			if(test.IndexOf('\n') != -1)
			{
				return true;
			}
			return false;
		}

		public static bool IsPascalCasing(string test)
		{
			if(test.Length == 0)
			{
				return true;
			}

			var firstLetter = test[0];
			if(firstLetter == char.ToLower(firstLetter))
			{
				return false;
			}

			if(test.IndexOf('_') != -1)
			{
				return false;
			}

			return true;
		}

		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		public static string CountToString([CanBeNull]ICollection collection)
		{
			return collection == null ? "null" : ToString(collection.Count);
		}

		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		public static string ToColorizedString(bool value)
		{
			return value ? "<color=green>True</color>" : "<color=red>False</color>";
		}

		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		public static string ToColorizedString(Object value)
		{
			if(value == null)
			{
				return "<color=red>null</color>";
			}
			return Green(ToString(value));
		}

		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		public static string ToColorizedString(object value)
		{
			if(value == null)
			{
				return "<color=red>null</color>";
			}

			if(value is Color)
			{
				return ToColorizedString((Color)value);
			}
			return ToColorizedString(ToStringCompact(value));
		}

		public static string ToColorizedString(string value)
		{
			if(value == null)
			{
				return "<color=red>null</color>";
			}

			switch(value)
			{
				case "":
				case "0":
				case "-1":
				case "False":
				case "None":
				case "\0":
				case "[]":
				case "{}":
					return Concat("<color=red>", value, "</color>");
				default:
					return Concat("<color=green>", value, "</color>");
			}
		}

		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		public static string ToColorizedString<T>(T value) where T : IConvertible
		{
			return Convert.ToInt32(value) > 0 ? Concat("<color=green>", ToString(value), "</color>") : Concat("<color=red>", ToString(value), "</color>");
		}

		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		public static string ToColorizedString(Color color)
		{
			var solidColor = color;
			solidColor.a = 1f;
			return Concat("<color=#", ColorUtility.ToHtmlStringRGBA(solidColor), ">", ToString(color), "</color>");
		}

		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		public static string ToString(Color color)
		{
			return "#" + (color.a <= 1f ? ColorUtility.ToHtmlStringRGB(color) : ColorUtility.ToHtmlStringRGBA(color));
		}

		/// <summary>
		/// Converts objects into their string representations and concats them together to form a single string.
		/// </summary>
		/// <returns>
		/// String representations of objects combined together.
		/// </returns>
		public static string ToColorizedString([NotNull]params object[] objects)
		{
			var sb = StringBuilderPool.Create();
			for(int n = 0, count = objects.Length; n < count; n++)
			{
				var add = objects[n];
				var text = add as string;
				if(text != null)
				{
					sb.Append(text);
				}
				else
				{
					sb.Append(ToColorizedString(add));
				}
			}
			return StringBuilderPool.ToStringAndDispose(ref sb);
		}

		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		public static string Red(string input)
		{
			return Concat("<color=red>", input, "</color>");
		}

		[MethodImpl(256)] //256 = MethodImplOptions.AggressiveInlining in .NET 4.5. and later
		public static string Green(string input)
		{
			return Concat("<color=green>", input, "</color>");
		}

		/// <summary> Finds index of substring inside text, ignoring spaces inside text and ignoring casing. </summary>
		/// <param name="text"> The text. </param>
		/// <param name="substring"> The substring to find. </param>
		/// <param name="textIgnoredSpaceCount"> [out] Number of space characters that were skipped in text when matching substring. </param>
		/// <returns> The zero-based index of the found ignoring spaces, or -1 if no match was found. </returns>
		public static int IndexOfIgnoringSpaces(string text, string substring, out int textIgnoredSpaceCount)
		{
			int textLength = text.Length;
			int substringLength = substring.Length;
			int stop = textLength - substringLength;
			
			textIgnoredSpaceCount = 0;
			int substringIgnoredSpaceCount = 0;

			// test each index in text
			for(int i = 0; i <= stop; i++)
			{
				if(text[i] == ' ')
				{
					continue;
				}

				bool failed = false;
				for(int nth = 0; nth < substringLength; nth++)
				{
					var a = text[i + nth - substringIgnoredSpaceCount];
					var b = substring[nth - textIgnoredSpaceCount];

					if(a == b)
					{
						continue;
					}

					if(a == ' ')
					{
						textIgnoredSpaceCount++;
						continue;
					}

					if(b == ' ')
					{
						substringIgnoredSpaceCount++;
						continue;
					}

					if(char.IsUpper(a))
					{
						a = char.ToLowerInvariant(a);
						if(char.IsUpper(b))
						{
							b = char.ToLowerInvariant(b);
						}

						if(a == b)
						{
							continue;
						}
					}
					else if(char.IsUpper(b))
					{
						b = char.ToLowerInvariant(b);

						if(a == b)
						{
							continue;
						}
					}

					failed = true;
					break;
				}

				if(!failed)
				{
					return i;
				}
				textIgnoredSpaceCount = 0;
				substringIgnoredSpaceCount = 0;
			}

			return -1;
		}
		
		/// <summary> If makePluralCondition is true converts word to plural form. </summary>
		/// <param name="input"> The original singular word. </param>
		/// <param name="makePluralCondition"> True to convert word to plural form. </param>
		/// <returns> A string. </returns>
		public static string Plural(this string singular, bool makePluralCondition)
		{
			return makePluralCondition ? Plural(singular) : singular;
		}

		public static string Plural(this string singular)
		{
			int letterCount = singular.Length;
			if(letterCount == 0)
			{
				return singular;
			}
			if(letterCount == 1)
			{
				return singular + "'s"; //E.g. x's
			}
			if(singular[letterCount - 1] == 's')
			{
				return singular + "es"; //E.g. Thomases
			}
			return singular + "s";
		}
	}
}