using System;
using System.Collections.Generic;

namespace Sisus
{
	/// <summary>
	/// Class that contains data for classes that implement IDrawerProvider.
	/// </summary>
	public class DrawerProviderData
	{
		/// <summary>
		/// Drawers for class members grouped by class member type.
		/// </summary>
		/// <value>
		/// Dictionary of drawer types that implement IFieldDrawer.
		/// </value>
		public readonly Dictionary<Type, Type> fields;

		/// <summary>
		/// Drawers for PropertyAttributes with drawing handled by DecoratorDrawers (not PropertyDrawers),
		/// grouped by PropertyAttribute type.
		/// </summary>
		/// <value>
		/// Dictionary of drawer types that implement IDecoratorDrawerDrawer.
		/// </value>
		public readonly Dictionary<Type, Type> decoratorDrawers;

		/// <summary>
		/// Drawers for fields that have PropertyAttributes with drawing handled by PropertyDrawers (not DecoratorDrawers).
		/// Grouped first by PropertyAttribute type and then by field type.
		/// </summary>
		/// <value>
		/// Dictionary of dictionaries of drawer types that implement IPropertyDrawerDrawer.
		/// </value>
		public readonly Dictionary<Type, Dictionary<Type, Type>> propertyDrawersByAttributeType;

		/// <summary>
		/// Drawers for class members where drawing is handled by PropertyDrawers, grouped by field type.
		/// </summary>
		/// <value>
		/// Dictionary of drawer types that implement IPropertyDrawerDrawer.
		/// </value>
		public readonly Dictionary<Type, Type> propertyDrawersByFieldType;

		/// <summary>
		/// Drawers for components grouped by component type.
		/// </summary>
		/// <value>
		/// Dictionary of drawer types that implement IComponentDrawer.
		/// </value>
		public readonly Dictionary<Type, Type> components;

		/// <summary>
		/// Drawers for asset type Objects grouped by Object type.
		/// </summary>
		/// <value>
		/// Dictionary of drawer types that implement IAssetDrawer.
		/// </value>
		public readonly Dictionary<Type, Type> assets;

		/// <summary>
		/// Drawers for asset type Objects grouped by file extension.
		/// </summary>
		/// <value>
		/// Dictionary of drawer types that implement IAssetDrawer.
		/// </value>
		public readonly Dictionary<string, Type> assetsByExtension;

		#if UNITY_EDITOR
		/// <summary>
		/// List of drawer types for PropertyDrawers that require a SerializedProperty to be provided or they cannot be used.
		/// </summary>
		/// <value>
		/// HashSet of drawer types that implement IPropertyDrawerDrawer.
		/// </value>
		public readonly HashSet<Type> propertyDrawerRequiresSerializedProperty;
		#endif
		
		/// <summary>
		/// Drawer for GameObjects.
		/// </summary>
		/// <value>
		/// Type of class that implements IGameObjectDrawer.
		/// </value>
		public Type gameObject;

		/// <summary>
		/// Drawer for GameObjects based on component type.
		/// </summary>
		/// <value>
		/// Dictionary of drawer types that implement IGameObjectDrawer.
		/// </value>
		public readonly Dictionary<Type, Type> gameObjectByComponent;

		public DrawerProviderData(int componentsCapacity = 5, int assetsCapacity = 20, int assetsByExtensionCapacity = 5, int fieldsCapacity = 50, int decoratorDrawersCapacity = 2, int attributePropertyDrawersCapacity = 3, int fieldPropertyDrawersCapacity = 2, int gameObjectDrawersCapacity = 2)
		{
			components = new Dictionary<Type, Type>(componentsCapacity);
			assets = new Dictionary<Type, Type>(assetsCapacity);
			assetsByExtension = new Dictionary<string, Type>(assetsByExtensionCapacity);
			fields = new Dictionary<Type, Type>(fieldsCapacity);
			decoratorDrawers = new Dictionary<Type, Type>(decoratorDrawersCapacity);
			propertyDrawersByAttributeType = new Dictionary<Type, Dictionary<Type, Type>>(attributePropertyDrawersCapacity);
			propertyDrawersByFieldType = new Dictionary<Type, Type>(fieldPropertyDrawersCapacity);
			gameObjectByComponent = new Dictionary<Type, Type>(gameObjectDrawersCapacity);

			#if UNITY_EDITOR
			propertyDrawerRequiresSerializedProperty = new HashSet<Type>
			{
				{ typeof(PropertyDrawerDrawer) }
			};
			#endif
		}

		public void Copy(DrawerProviderData copyOver)
		{
			copyOver.fields.Clear();
			foreach(var item in fields)
			{
				copyOver.fields.Add(item.Key, item.Value);
			}

			copyOver.decoratorDrawers.Clear();
			foreach(var item in decoratorDrawers)
			{
				copyOver.decoratorDrawers.Add(item.Key, item.Value);
			}

			copyOver.propertyDrawersByAttributeType.Clear();
			foreach(var item in propertyDrawersByAttributeType)
			{
				copyOver.propertyDrawersByAttributeType.Add(item.Key, new Dictionary<Type, Type>(item.Value));
			}

			copyOver.propertyDrawersByFieldType.Clear();
			foreach(var item in propertyDrawersByFieldType)
			{
				copyOver.propertyDrawersByFieldType.Add(item.Key, item.Value);
			}

			copyOver.components.Clear();
			foreach(var item in components)
			{
				copyOver.components.Add(item.Key, item.Value);
			}

			copyOver.assets.Clear();
			foreach(var item in assets)
			{
				copyOver.assets.Add(item.Key, item.Value);
			}

			copyOver.assetsByExtension.Clear();
			foreach(var item in assetsByExtension)
			{
				copyOver.assetsByExtension.Add(item.Key, item.Value);
			}

			#if UNITY_EDITOR
			copyOver.propertyDrawerRequiresSerializedProperty.Clear();
			foreach(var type in propertyDrawerRequiresSerializedProperty)
			{
				copyOver.propertyDrawerRequiresSerializedProperty.Add(type);
			}
			#endif
		}
	}
}