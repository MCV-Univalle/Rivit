using System;
using JetBrains.Annotations;
using Sisus.Attributes;
using UnityEngine;

namespace Sisus
{
	/// <summary>
	/// A drawer provider that works by wrapping an existing drawer provider and reusing its
	/// drawer fetching data and logic as the basis of drawer fetching, but possibly doing
	/// some alterations on top.
	/// </summary>
	public abstract class DrawerProviderModifer : IDrawerProvider
	{
		private IDrawerProvider wrappedProvider;

		/// <summary>
		/// Create new instance of DrawerProviderModifer that works by wrapping the provided
		/// existing drawer provider and reusing its drawer fetching data and logic as the
		/// basis of drawer fetching, but possibly doing some alterations on top.
		/// </summary>
		/// <param name="baseProvider"></param>
		public DrawerProviderModifer([NotNull]IDrawerProvider baseProvider)
		{
			wrappedProvider = baseProvider;
		}

		/// <inheritdoc/>
		public Type GetClassDrawerType([NotNull] Type classType)
		{
			return wrappedProvider.GetClassDrawerType(classType);
		}

		/// <inheritdoc/>
		public Type GetClassMemberDrawerType([NotNull] Type fieldType)
		{
			return wrappedProvider.GetClassMemberDrawerType(fieldType);
		}

		/// <inheritdoc/>
		public Type GetDrawerTypeForGameObject([NotNull] GameObject target)
		{
			return wrappedProvider.GetDrawerTypeForGameObject(target);
		}

		/// <inheritdoc/>
		public Type GetDrawerTypeForGameObjects([NotNullOrEmpty] GameObject[] targets)
		{
			return wrappedProvider.GetDrawerTypeForGameObjects(targets);
		}

		/// <inheritdoc/>
		public virtual IDrawer GetForAsset([NotNull]IInspector inspector, [NotNull]UnityEngine.Object target, [CanBeNull]IParentDrawer parent)
		{
			return wrappedProvider.GetForAsset(inspector, target, parent);
		}

		/// <inheritdoc/>
		public virtual IAssetDrawer GetForAssets([NotNull]IInspector inspector, [NotNull]UnityEngine.Object[] targets, [CanBeNull]IParentDrawer parent)
		{
			return wrappedProvider.GetForAssets(inspector, targets, parent);
		}

		/// <inheritdoc/>
		public virtual IEditorlessAssetDrawer GetForAssetsWithoutEditor(Type drawerType, UnityEngine.Object[] targets, IParentDrawer parent, IInspector inspector)
		{
			return wrappedProvider.GetForAssetsWithoutEditor(drawerType, targets, parent, inspector);
		}

		#if UNITY_EDITOR
		/// <inheritdoc/>
		public ICustomEditorAssetDrawer GetForAssetWithEditor([NotNull] Type drawerType, [CanBeNull] Type customEditorType, [NotNull] UnityEngine.Object[] targets, [CanBeNull] UnityEngine.Object[] assetImporters, [CanBeNull] IParentDrawer parent, [NotNull] IInspector inspector)
		{
			return wrappedProvider.GetForAssetWithEditor(drawerType, customEditorType, targets, assetImporters, parent, inspector);
		}
		#endif

		/// <inheritdoc/>
		public virtual IComponentDrawer GetForComponent([NotNull]IInspector inspector, [NotNull]Component target, [CanBeNull]IParentDrawer parent)
		{
			return wrappedProvider.GetForComponent(inspector, target, parent);
		}

		/// <inheritdoc/>
		public virtual IComponentDrawer GetForComponents([NotNull]IInspector inspector, [NotNull]Component[] targets, [CanBeNull]IParentDrawer parent)
		{
			return wrappedProvider.GetForComponents(inspector, targets, parent);
		}

		/// <inheritdoc/>
		public virtual IEditorlessComponentDrawer GetForComponents([NotNull]Type drawerType, [NotNull]Component[] targets, [CanBeNull]IParentDrawer parent, [NotNull]IInspector inspector)
		{
			return wrappedProvider.GetForComponents(drawerType, targets, parent, inspector);
		}

		/// <inheritdoc/>
		public virtual ICustomEditorComponentDrawer GetForComponents([NotNull]Type drawerType, [CanBeNull]Type customEditorType, [NotNull]Component[] targets, [CanBeNull]IParentDrawer parent, [NotNull]IInspector inspector)
		{
			return wrappedProvider.GetForComponents(drawerType, customEditorType, targets, parent, inspector);
		}

		/// <inheritdoc/>
		public virtual IDrawer GetForField(LinkedMemberInfo memberInfo, IParentDrawer parent, GUIContent label = null, bool readOnly = false)
		{
			return wrappedProvider.GetForField(memberInfo, parent, label, readOnly);
		}

		/// <inheritdoc/>
		public virtual IDrawer GetForField([CanBeNull]object value, [CanBeNull]LinkedMemberInfo memberInfo, [CanBeNull]IParentDrawer parent, [CanBeNull]GUIContent label, bool readOnly)
		{
			return wrappedProvider.GetForField(value, memberInfo, parent, label, readOnly);
		}

		/// <inheritdoc/>
		public virtual IDrawer GetForField([CanBeNull]object value, [NotNull]Type fieldType, [CanBeNull]LinkedMemberInfo memberInfo, [CanBeNull]IParentDrawer parent, [CanBeNull]GUIContent label, bool readOnly)
		{
			return wrappedProvider.GetForField(value, fieldType, memberInfo, parent, label, readOnly);
		}

		/// <inheritdoc/>
		public virtual IFieldDrawer GetForField(Type drawerType, [CanBeNull]object value, Type valueType, [CanBeNull]LinkedMemberInfo memberInfo, [CanBeNull]IParentDrawer parent, [CanBeNull]GUIContent label, bool readOnly)
		{
			return wrappedProvider.GetForField(drawerType, value, valueType, memberInfo, parent, label, readOnly);
		}

		/// <inheritdoc/>
		public virtual IDrawer GetForFields(object[] values, IParentDrawer parent, GUIContent label = null, bool readOnly = false)
		{
			return wrappedProvider.GetForFields(values, parent, label, readOnly);
		}

		/// <inheritdoc/>
		public virtual IGameObjectDrawer GetForGameObject([NotNull]IInspector inspector, [NotNull]GameObject target, [CanBeNull]IParentDrawer parent)
		{
			return wrappedProvider.GetForGameObject(inspector, target, parent);
		}

		/// <inheritdoc/>
		public virtual IGameObjectDrawer GetForGameObjects([NotNull]IInspector inspector, [NotNull]GameObject[] targets, [CanBeNull]IParentDrawer parent)
		{
			return wrappedProvider.GetForGameObjects(inspector, targets, parent);
		}

		/// <inheritdoc/>
		public virtual IDrawer GetForMethod(LinkedMemberInfo methodInfo, IParentDrawer parent, GUIContent label = null, bool readOnly = false)
		{
			return wrappedProvider.GetForMethod(methodInfo, parent, label, readOnly);
		}

		/// <inheritdoc/>
		public virtual IFieldDrawer GetForProperty([CanBeNull] object value, [NotNull] Type valueType, [NotNull] LinkedMemberInfo memberInfo, [CanBeNull] IParentDrawer parent, [CanBeNull] GUIContent label, bool readOnly)
		{
			return wrappedProvider.GetForProperty(value, valueType, memberInfo, parent, label, readOnly);
		}

		/// <inheritdoc/>
		public virtual IDrawer GetForPropertyDrawer([NotNull]Attribute fieldAttribute, [CanBeNull]object value, [NotNull]Type fieldType, [CanBeNull]LinkedMemberInfo memberInfo, [CanBeNull]IParentDrawer parent, [CanBeNull]GUIContent label, bool readOnly)
		{
			return wrappedProvider.GetForPropertyDrawer(fieldAttribute, value, fieldType, memberInfo, parent, label, readOnly);
		}

		/// <inheritdoc/>
		public virtual IPropertyDrawerDrawer GetForPropertyDrawer(Type drawerType, [CanBeNull]Attribute fieldAttribute, [CanBeNull]object value, [CanBeNull]LinkedMemberInfo memberInfo, [CanBeNull]IParentDrawer parent, [CanBeNull]GUIContent label, bool readOnly)
		{
			return wrappedProvider.GetForPropertyDrawer(drawerType, fieldAttribute, value, memberInfo, parent, label, readOnly);
		}

		/// <inheritdoc/>
		public virtual void Prewarm(IInspector inspector)
		{
			wrappedProvider.Prewarm(inspector);
		}

		/// <inheritdoc/>
		public virtual bool TryGetForDecoratorDrawer([NotNull]PropertyAttribute fieldAttribute, [NotNull]Type propertyAttributeType, IParentDrawer parent, LinkedMemberInfo attributeTarget, [CanBeNull]out IDecoratorDrawerDrawer result)
		{
			return wrappedProvider.TryGetForDecoratorDrawer(fieldAttribute, propertyAttributeType, parent, attributeTarget, out result);
		}
	}
}