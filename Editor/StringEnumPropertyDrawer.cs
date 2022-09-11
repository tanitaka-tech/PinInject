using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Globalization;

namespace Cathei.PinInject.Editor
{
    [CustomPropertyDrawer(typeof(StringEnum<>), true)]
    public class StringEnumPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();

            var stringProp = property.FindPropertyRelative("_stringValue");
            var enumType = fieldInfo.FieldType.GenericTypeArguments[0];

            Enum enumValue = default; 

            try
            {
                enumValue = (Enum)Enum.Parse(enumType, stringProp.stringValue);
            }
            catch (ArgumentException) { }

            if (enumType.IsDefined(typeof(FlagsAttribute), false))
            {
                enumValue = EditorGUI.EnumFlagsField(position, label, enumValue);
            }
            else
            {
                enumValue = EditorGUI.EnumPopup(position, label, enumValue);
            }

            if (EditorGUI.EndChangeCheck())
            {
                stringProp.stringValue = enumValue.ToString();
            }
        }
    }
}
