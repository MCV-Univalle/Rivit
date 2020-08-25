#define USE_IL_FOR_GET_AND_SET

//#define DEBUG_SET_VALUE
//#define DEBUG_GET_VALUE

using System;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

#if (UNITY_EDITOR || UNITY_STANDALONE) && !NET_STANDARD_2_0
using Sisus.Vexe.FastReflection;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sisus
{
	public sealed class PropertyData : MemberData
	{
		private PropertyInfo propertyInfo;

		#if ((UNITY_EDITOR || UNITY_STANDALONE) && !NET_STANDARD_2_0) && USE_IL_FOR_GET_AND_SET
		MemberGetter<object,object> getValue;
		MemberSetter<object,object> setValue;
		#endif

		#if UNITY_EDITOR
		private SerializedProperty serializedProperty;
		#endif

		#if UNITY_EDITOR
		public override SerializedProperty SerializedProperty
		{
			get
			{
				return serializedProperty;
			}

			set
			{
				serializedProperty = value;
			}
		}
		#endif

		public override MemberTypes MemberType
		{
			get { return MemberTypes.Property; }
		}

		public override LinkedMemberType LinkedMemberType
		{
			get
			{
				return LinkedMemberType.Property;
			}
		}

		public override string Name
		{
			get
			{
				return propertyInfo.Name;
			}
		}

		public override MemberInfo MemberInfo
		{
			get { return propertyInfo; }
		}

		public override MemberInfo SecondMemberInfo
		{
			get { return null; }
		}

		public override bool IsStatic
		{
			get
			{
				return propertyInfo.CanRead ? propertyInfo.GetGetMethod(true).IsStatic : propertyInfo.GetSetMethod(true).IsStatic;
			}
		}

		public override Type Type
		{
			get
			{
				return propertyInfo.PropertyType;
			}
		}

		public override bool CanRead
		{
			get
			{
				#if ((UNITY_EDITOR || UNITY_STANDALONE) && !NET_STANDARD_2_0) && USE_IL_FOR_GET_AND_SET
				return getValue != null;
				#else
				return propertyInfo.CanRead;
				#endif
			}
		}

		public override bool CanReadWithoutSideEffects
		{
			get
			{
				return CanRead && propertyInfo.IsAutoProperty();
			}
		}

		public override bool CanWrite
		{
			get
			{
				#if ((UNITY_EDITOR || UNITY_STANDALONE) && !NET_STANDARD_2_0) && USE_IL_FOR_GET_AND_SET
				return setValue != null;
				#else
				return propertyInfo.CanWrite;
				#endif
			}
		}
			
		#if UNITY_EDITOR
		public void Setup([NotNull]PropertyInfo setPropertyInfo, [NotNull]SerializedProperty setSerializedProperty)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(setPropertyInfo != null);
			Debug.Assert(setSerializedProperty != null, "("+setPropertyInfo.PropertyType+")\""+setPropertyInfo.Name+"\"");
			#endif

			serializedProperty = setSerializedProperty;
			Setup(setPropertyInfo);
		}
		#endif

		public void Setup([NotNull]PropertyInfo inPropertyInfo)
		{
			propertyInfo = inPropertyInfo;
			
			#if ((UNITY_EDITOR || UNITY_STANDALONE) && !NET_STANDARD_2_0) && USE_IL_FOR_GET_AND_SET
			if(inPropertyInfo.CanRead)
			{
				try
				{
					getValue = inPropertyInfo.DelegateForGet();
				}
				catch(InvalidProgramException e)
				{
					Debug.LogError("PropertyData.Setup Property \""+inPropertyInfo.Name + "\" of type "+StringUtils.ToStringSansNamespace(inPropertyInfo.PropertyType)+ " with IndexParameters "+StringUtils.ToString(inPropertyInfo.GetIndexParameters() + " InvalidProgramException:\n" + e));
					getValue = null;
				}
			}
			else
			{
				getValue = null;
			}

			if(inPropertyInfo.CanWrite)
			{
				try
				{
					setValue = inPropertyInfo.DelegateForSet();
				}
				catch(InvalidProgramException e)
				{
					Debug.LogError(inPropertyInfo.Name + " of type " + StringUtils.ToStringSansNamespace(inPropertyInfo.PropertyType) + " with IndexParameters "+StringUtils.ToString(inPropertyInfo.GetIndexParameters() + " " + e));
					setValue = null;
				}
			}
			else
			{
				setValue = null;
			}
			#endif
		}

		public override bool Equals(MemberData other)
		{
			var b = other as PropertyData;
			if(b == null)
			{
				return false;
			}

			return b.propertyInfo != null && propertyInfo.EqualTo(b.propertyInfo);
		}

		public override void GetValue(object fieldOwner, out object result)
		{
			#if DEV_MODE
			Debug.Assert(fieldOwner != null || IsStatic, ToString() + ".GetValue called with null fieldOwner but IsStatic was false!");
			#endif

			#if ((UNITY_EDITOR || UNITY_STANDALONE) && !NET_STANDARD_2_0) && USE_IL_FOR_GET_AND_SET
			Debug.Assert(getValue != null, ToString() + " getValue null");
			#endif

			#if DEV_MODE && DEBUG_GET_VALUE
			Debug.Log(ToString() + ".GetValue(fieldOwner=" + StringUtils.ToString(fieldOwner)+")");
			#endif

			#if ((UNITY_EDITOR || UNITY_STANDALONE) && !NET_STANDARD_2_0) && USE_IL_FOR_GET_AND_SET
			try
			{
				result = getValue(fieldOwner);
			}
			catch(InvalidCastException e)
			{
				Debug.LogError(ToString() + ".GetValue(" + StringUtils.ToString(fieldOwner) + "), ownerType=" + StringUtils.TypeToString(fieldOwner) + " " + e + "\ngetValue.Type=" + StringUtils.TypeToString(getValue)+", CanRead=" + CanRead);
				throw;
			}
			catch(NullReferenceException e)
			{
				Debug.LogError(ToString() + ".GetValue(" + StringUtils.ToString(fieldOwner) + "), ownerType=" + StringUtils.TypeToString(fieldOwner) + " " + e + "\ngetValue.Type=" + StringUtils.TypeToString(getValue)+", CanRead=" + CanRead);
				throw;
			}
			catch(MissingReferenceException e)
			{
				Debug.LogError(ToString()+".GetValue(" + StringUtils.ToString(fieldOwner) + "), ownerType=" + StringUtils.TypeToString(fieldOwner) + " " + e + "\ngetValue.Type=" + StringUtils.TypeToString(getValue)+", CanRead=" + CanRead);
				throw;
			}
			#else
			try
			{
				result = propertyInfo.GetValue(fieldOwner, null);
			}
			catch(Exception e)
			{
				Debug.LogError(ToString() + ".GetValue(owner=" + StringUtils.ToString(fieldOwner) + "), ownerType=" + StringUtils.TypeToString(fieldOwner)+" "+e);
				result = DefaultValue();
			}
			#endif
		}
		
		public override void SetValue(ref object fieldOwner, object value)
		{
			#if DEV_MODE && DEBUG_SET_VALUE
			Debug.Log(ToString() + ".SetValue(fieldOwner=" + StringUtils.ToString(fieldOwner)+", value="+StringUtils.ToString(value)+")");
			#endif

			try
			{
				#if ((UNITY_EDITOR || UNITY_STANDALONE) && !NET_STANDARD_2_0) && USE_IL_FOR_GET_AND_SET
				setValue(ref fieldOwner, value);
				#else
				propertyInfo.SetValue(fieldOwner, value, null);
				#endif
			}
			catch(Exception e)
			{
				Debug.LogError(e);
			}
		}
			
		public override object[] GetAttributes(bool inherit = true)
		{
			var result = propertyInfo.GetCustomAttributes(inherit);
			Compatibility.PluginAttributeConverterProvider.ConvertAll(ref result);
			return result;
		}

		/// <inheritdoc/>
		public override void Dispose()
		{
			#if UNITY_EDITOR
			serializedProperty = null;
			#endif
			LinkedMemberInfoPool.Dispose(this);
		}
	}
}