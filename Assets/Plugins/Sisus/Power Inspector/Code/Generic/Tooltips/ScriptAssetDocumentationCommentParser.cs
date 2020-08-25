using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace Sisus
{
	public class ScriptAssetDocumentationCommentParser : IDisposable
	{
		private static readonly char[] MemberDefinitionEndCharacters = {'(', '{', ';'};

		private readonly Dictionary<string, MemberCommentInfo> commentInfos;

		public ScriptAssetDocumentationCommentParser(string scriptAssetFullPath)
		{
			commentInfos = new Dictionary<string, MemberCommentInfo>();
			ParseComments(scriptAssetFullPath, commentInfos);
		}
		
		public static void ParseComments(string scriptAssetFullPath, [NotNull]Dictionary<string, MemberCommentInfo> addToCollection)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			UnityEngine.Debug.Assert(scriptAssetFullPath.IndexOf(':') != -1);
			#endif

			var commentXml = new StringBuilder();
			var memberDefinition = new StringBuilder();
		
			using(var reader = new StreamReader(scriptAssetFullPath))
			{
				string line;
				while((line = reader.ReadLine()) != null)
				{
					line = line.TrimStart();
					if(line.StartsWith("///", StringComparison.Ordinal))
					{
						commentXml.AppendLine(line.Substring(3));
					}
					else if(commentXml.Length > 0)
					{
						memberDefinition.AppendLine(line);

						if(line.IndexOfAny(MemberDefinitionEndCharacters) != -1)
						{
							var info = new MemberCommentInfo(commentXml.ToString(), memberDefinition.ToString());
							commentXml.Length = 0;
							memberDefinition.Length = 0;

							if(info.name.Length > 0 && info.comment.Length > 0)
							{
								addToCollection.Add(info.name, info);
							}
							else
							{
								info.Dispose();
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Tries to parse XML Documentation Comments from script file at path.
		/// </summary>
		/// <param name="scriptAssetFullPath"></param>
		/// <param name="addToCollection"></param>
		/// <param name="classTypeMustMatch"></param>
		/// <returns> False if failed, either because of write permissions, or because class definition was not found inside script asset. </returns>
		public static bool ParseComments(string scriptAssetFullPath, [NotNull]Dictionary<string, string> addToCollection, [CanBeNull]Type classTypeMustMatch)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			UnityEngine.Debug.Assert(scriptAssetFullPath.IndexOf(':') != -1);
			#endif

			if(classTypeMustMatch != null)
			{
				var mustBeFound = new List<string>(classTypeMustMatch.FullName.Split('.'));
				bool allFound = false;

				try
				{
					using(var reader = new StreamReader(scriptAssetFullPath))
					{
						string line;
						int findCount = mustBeFound.Count;

						while((line = reader.ReadLine()) != null)
						{
							for(int n = findCount - 1; n >= 0; n--)
							{
								if(line.IndexOf(mustBeFound[n], StringComparison.Ordinal) != -1)
								{
									mustBeFound.RemoveAt(n);
									findCount--;
									if(findCount == 0)
									{
										allFound = true;
										break;
									}
								}
							}

							if(allFound)
							{
								break;
							}
						}
					}
				}
				#if DEV_MODE
				catch(PathTooLongException e)
				{
					UnityEngine.Debug.LogError("ParseComments PathTooLongException. Path length was was "+scriptAssetFullPath.Length+".\n"+ e);
					return false;
				}
				catch(DirectoryNotFoundException e)
				{
					UnityEngine.Debug.LogError("ParseComments DirectoryNotFoundException. Path length was "+scriptAssetFullPath.Length+".\n"+ e);
					return false;
				}
				catch(Exception e)
				{
					UnityEngine.Debug.LogError("ParseComments "+e.GetType().Name+". Path length was was "+scriptAssetFullPath.Length+".\n"+ e);
					return false;
				}
				#else
				catch
				{
					return false;
				}
				#endif

				if(!allFound)
				{
					#if DEV_MODE
					UnityEngine.Debug.LogWarning("Failed to find the following parts of full class name inside script asset: "+string.Join(", ", mustBeFound.ToArray()));
					#endif
					return false;
				}
			}


			var commentXml = StringBuilderPool.Create();
			var memberDefinition = StringBuilderPool.Create();

			try
			{
				using(var reader = new StreamReader(scriptAssetFullPath))
				{
					string line;
					while((line = reader.ReadLine()) != null)
					{
						line = line.TrimStart();
						if(line.StartsWith("///", StringComparison.Ordinal))
						{
							commentXml.AppendLine(line.Substring(3));
						}
						else if(commentXml.Length > 0)
						{
							memberDefinition.AppendLine(line);

							if(line.IndexOfAny(MemberDefinitionEndCharacters) != -1)
							{
								var info = new MemberCommentInfo(commentXml.ToString(), memberDefinition.ToString());
								commentXml.Length = 0;
								memberDefinition.Length = 0;

								if(info.name.Length > 0 && info.comment.Length > 0)
								{
									addToCollection.Add(info.name, info.comment);
								}
								else
								{
									info.Dispose();
								}
							}
						}
					}
				}
			}
			#if DEV_MODE
			catch(PathTooLongException e)
			{
				UnityEngine.Debug.LogError("ParseComments PathTooLongException. Path length was was "+scriptAssetFullPath+".\n"+ e);
				StringBuilderPool.Dispose(ref commentXml);
				StringBuilderPool.Dispose(ref memberDefinition);
				return false;
			}
			catch(DirectoryNotFoundException e)
			{
				UnityEngine.Debug.LogError("ParseComments DirectoryNotFoundException. Path length was "+scriptAssetFullPath+".\n"+ e);
				StringBuilderPool.Dispose(ref commentXml);
				StringBuilderPool.Dispose(ref memberDefinition);
				return false;
			}
			catch(Exception e)
			{
				UnityEngine.Debug.LogError("ParseComments "+e.GetType().Name+". Path length was was "+scriptAssetFullPath+".\n"+ e);
				StringBuilderPool.Dispose(ref commentXml);
				StringBuilderPool.Dispose(ref memberDefinition);
				return false;
			}
			#else
			catch
			{
				StringBuilderPool.Dispose(ref commentXml);
				StringBuilderPool.Dispose(ref memberDefinition);
				return false;
			}
			#endif

			StringBuilderPool.Dispose(ref commentXml);
			StringBuilderPool.Dispose(ref memberDefinition);
			return true;
		}

		public void Dispose()
		{
			foreach(var comment in commentInfos)
			{
				comment.Value.Dispose();
			}
			commentInfos.Clear();
		}
	}
}