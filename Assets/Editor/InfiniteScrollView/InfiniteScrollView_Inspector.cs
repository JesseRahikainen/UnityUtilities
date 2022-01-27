/*
MIT LICENSE
Copyright 2002 Jesse Rahikainen
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software
without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InfiniteScrollView))]
public class InfiniteScrollView_Inspector : Editor
{
    private SerializedProperty _adapterProperty;
    private SerializedProperty _layoutProperty;
    private SerializedProperty _spacingProperty;
    private SerializedProperty _paddingProperty;
    private SerializedProperty _useInertiaProperty;
    private SerializedProperty _inertiaProperty;
    private SerializedProperty _inertiaReductionProperty;
    private SerializedProperty _numViewHoldersProperty;
    private SerializedProperty _viewRootProperty;

    private void OnEnable( )
    {
        _adapterProperty = serializedObject.FindProperty( "adapter" );
        _layoutProperty = serializedObject.FindProperty( "layout" );
        _spacingProperty = serializedObject.FindProperty( "spacing" );
        _paddingProperty = serializedObject.FindProperty( "padding" );
        _useInertiaProperty = serializedObject.FindProperty( "useInertia" );
        _inertiaProperty = serializedObject.FindProperty( "inertia" );
        _inertiaReductionProperty = serializedObject.FindProperty( "inertiaReduction" );
        _numViewHoldersProperty = serializedObject.FindProperty( "numViewHolders" );
        _viewRootProperty = serializedObject.FindProperty( "viewRoot" );
    }

    public override void OnInspectorGUI( )
    {
        serializedObject.Update( );
        EditorGUILayout.PropertyField( _adapterProperty );
        EditorGUILayout.PropertyField( _layoutProperty );
        EditorGUILayout.PropertyField( _spacingProperty );
        EditorGUILayout.PropertyField( _paddingProperty );
        EditorGUILayout.PropertyField( _useInertiaProperty );
        if( _useInertiaProperty.boolValue ) {
            EditorGUILayout.PropertyField( _inertiaProperty, new GUIContent( "    Inertia" ) );
            EditorGUILayout.PropertyField( _inertiaReductionProperty, new GUIContent( "    Inertia Reduction" ) );
        }
        EditorGUILayout.PropertyField( _numViewHoldersProperty );
        EditorGUILayout.PropertyField( _viewRootProperty );

        serializedObject.ApplyModifiedProperties( );
    }
}
