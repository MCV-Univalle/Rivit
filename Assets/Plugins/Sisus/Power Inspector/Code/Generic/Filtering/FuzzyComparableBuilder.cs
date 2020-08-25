using System.Collections.Generic;

namespace Sisus
{
	public class FuzzyComparableBuilder
	{
		private const int Lower = 0;
		private const int Upper = 1;
		private const int NonLetter = 2;

		private const char Space = ' ';

		private static readonly List<int> SplitStringBuilder = new List<int>();
		private static readonly List<int> ConcatenatedStringBuilder = new List<int>();
		private static readonly List<int> SplitPointList = new List<int>(3);

		public static int[] GenerateFuzzyComparableData(string textInput, out int[] splitPoints)
		{
			#if DEV_MODE
			Profiler.BeginSample("GenerateFuzzyComparableData");
			#endif

			int[] result;
			int length = textInput.Length;
			switch(length)
			{
				case 0:
					result = ArrayPool<int>.ZeroSizeArray;
					splitPoints = ArrayPool<int>.ZeroSizeArray;
					break;
				case 1:
					result = new int[]{ char.ToLower(textInput[0]) };
					splitPoints = ArrayPool<int>.ZeroSizeArray;
					break;
				default:
					int i = 0;
					char c = textInput[0];
					
					if(length >= 2)
					{
						//skip past single-letter + underscore prefixes like "m_"
						if(textInput[1] == '_' && length >= 3)
						{
							i = 2;
							c = textInput[2];
						}
						//skip past underscore prefixes
						else if(c == '_')
						{
							i = 1;
							c = textInput[1];
						}
					}
					char lc = char.ToLower(c);
					int num = lc;
					int prevType = char.IsNumber(lc) ? NonLetter : Upper;
					SplitStringBuilder.Add(num);
					ConcatenatedStringBuilder.Add(num);

					#if DEV_MODE && SAFE_MODE
					for(int t = 0; t <= 9; t++)
					{
						char c = t.ToString()[0];
						UnityEngine.Debug.Assert(!char.IsUpper(c));
						UnityEngine.Debug.Assert(!char.IsLower(c));
					}
					UnityEngine.Debug.Assert(!char.IsLower(' '));
					UnityEngine.Debug.Assert(!char.IsLower('_'));
					UnityEngine.Debug.Assert(!char.IsLower('.'));
					UnityEngine.Debug.Assert(!char.IsLower('/'));
					UnityEngine.Debug.Assert(!char.IsLower('\\'));
					#endif

					//int prevType = -1; //0 = lower case, 1 = upper case, 2 = number

					// skipping first letter which was already capitalized
					for(i = i + 1; i < length; i++)
					{
						c = textInput[i];
						num = c;

						switch(num)
						{
							case '0':
							case '1':
							case '2':
							case '3':
							case '4':
							case '5':
							case '6':
							case '7':
							case '8':
							case '9':
								//If this character is a number but previous is not...
								//if(!char.IsNumber(prev))
								if(prevType != NonLetter)
								{
									// ...add a space before this character.
									// E.g. "Id1" => "Id 1", "FBI123" => "FBI 123", "Array2D" => "Array 2D"
									SplitStringBuilder.Add(Space);
								}
								SplitStringBuilder.Add(num);
								ConcatenatedStringBuilder.Add(num);
								prevType = NonLetter;
								break;
							case ' ':
							case '_':
							case '.':
							case '/':
							case '\\':
								SplitPointList.Add(i);
								SplitStringBuilder.Add(' ');
								prevType = NonLetter;
								break;
							default:
								lc = char.ToLower(c);
								//If this chararacter is an upper case letter...
								if(lc != c)
								{
									//...and previous character is a lower case letter...
									//if(char.IsLower(input[i - 1])) //IsLower returns false for numbers, so no need to check && !IsNumber separately
									if(prevType == Lower)
									{
										// ...add a space before it.
										// E.g. "TestID" => "Test ID", "Test3D => "Test 3D".
										SplitStringBuilder.Add(' ');
										
									}
									// ...or if the next character is a lower case letter
									else if(length > i + 1 && char.IsLower(textInput[i + 1])) //IsLower returns false for numbers, so no need to check && !IsNumber separately
									{
										// ...add a space before it.
										// E.g. "FBIDatabase" => "FBI Database", "FBI123" => "FBI 123", "My3DFx" => "My 3D Fx"
										SplitStringBuilder.Add(Space);
										
									}

									num = lc;
									SplitStringBuilder.Add(num);
									ConcatenatedStringBuilder.Add(num);
									prevType = Upper;
									break;
								}

								//add lower case character as is to both lists
								SplitStringBuilder.Add(num);
								ConcatenatedStringBuilder.Add(num);
								prevType = Lower;
								break;
						}
					}

					if(SplitPointList.Count == 0)
					{
						result = SplitStringBuilder.ToArray();
						splitPoints = ArrayPool<int>.ZeroSizeArray;
					}
					else
					{
						splitPoints = SplitPointList.ToArray();
						// offset split points by length of ConcatenatedStringBuilder
						// because it will be appended to the beginning of the result string
						int offset = ConcatenatedStringBuilder.Count + 1;
						for(int n = splitPoints.Length - 1; n >= 0; n--)
						{
							splitPoints[n] += offset;
						}
						SplitPointList.Clear();

						ConcatenatedStringBuilder.Add(' ');
						ConcatenatedStringBuilder.AddRange(SplitStringBuilder);
						result = ConcatenatedStringBuilder.ToArray();
					}
					SplitStringBuilder.Clear();
					ConcatenatedStringBuilder.Clear();
					break;
			}
			
			#if DEV_MODE
			UnityEngine.Debug.Assert(result.Length > 0 || textInput.Length == 0, "GenerateFuzzyComparableData for textInput \"" + textInput+"\" was empty!");
			#endif
			
			#if DEV_MODE
			Profiler.EndSample();
			#endif

			return result;
		}
	}
}