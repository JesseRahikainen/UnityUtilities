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

using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using System;

// Basic configuration loading, uses reflection to match name-value pairs in a file to what's in the object.
//  Generally the base config file should be put in the StreamingAssets folder, and then as the configuration
//  is saved it will be put in the projects persistent data folder.
public class BaseConfig : MonoBehaviour
{
    public string fileName;

    public bool IsReady { get; private set; } = false;

    private void OnEnable( )
    {
        ReadConfigData( );
    }

    public void ReadConfigData( )
    {
        // go through all the values in the class and read them in from the config file
        //  we'll assume there's at most two places the config file could be stored, either
        //  in persistentDataPath, or in streamingAssetsPath
        //  the persistentDataPath will be for if the config changes and it's saved out
        //  the streamingAssetsPath will be for the default config file
        string path;
        string customConfigPath = Path.Combine( Application.persistentDataPath, fileName );
        if( File.Exists( customConfigPath ) ) {
            path = customConfigPath;
        } else {
            path = Path.Combine( Application.streamingAssetsPath, fileName );
        }

        if( !File.Exists( path ) ) {
            Debug.LogWarning( "Default and custom config files do not exist for " + fileName );
            return;
        }

        string[] lines = File.ReadAllLines( path );

        for( int i = 0; i < lines.Length; ++i ) {
            string line = lines[i].Trim( );

            if( line.Length <= 0 ) continue;
            if( line.StartsWith( "#" ) ) continue; // comment

            // basic format will be name value
            int split = line.IndexOf( ' ' );
            if( split < 0 ) continue;
            string valueName = line.Substring( 0, split );
            string valueStr = line.Substring( split + 1 );

            PropertyInfo pi = GetType( ).GetProperty( valueName, BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance );
            FieldInfo fi = GetType( ).GetField( valueName, BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance );
            if( pi != null ) {
                ReadProperty( pi, valueStr );
            } else if( fi != null ) {
                ReadField( fi, valueStr );
            } else {
                Debug.LogWarning( "Unknown config value: " + valueName );
                continue;
            }
        }

        IsReady = true;
    }

    private void ReadProperty( PropertyInfo pi, string valueStr )
    {
        try {
            if( pi.PropertyType.IsEnum ) {
                pi.SetValue( this, Enum.Parse( pi.PropertyType, valueStr ) );
            } else {
                TypeConverter converter = TypeDescriptor.GetConverter( pi.PropertyType );
                pi.SetValue( this, converter.ConvertFromString( valueStr ) );
            }
        } catch( Exception e ) {
            Debug.LogWarning( "Unable to convert value for " + pi.Name + ": " + e.Message );
        }
    }

    private void ReadField( FieldInfo fi, string valueStr )
    {
        try {
            TypeConverter converter = TypeDescriptor.GetConverter( fi.FieldType );
            fi.SetValue( this, converter.ConvertFromString( valueStr ) );
        } catch( Exception e ) {
            Debug.LogWarning( "Unable to convert value for " + fi.Name + ": " + e.Message );
        }
    }

    public void WriteConfigData( )
    {
        // this won't save out any comments in the original file
        string path = Path.Combine( Application.persistentDataPath, fileName );
        List<string> valuePairs = new List<string>( );

        // get all properties and fields
        foreach( PropertyInfo pi in GetType( ).GetProperties( BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance ) ) {
            valuePairs.Add( String.Format( "{0} {1}", pi.Name, pi.GetValue( this ).ToString( ) ) );
        }

        foreach( FieldInfo fi in GetType( ).GetFields( BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance ) ) {
            valuePairs.Add( String.Format( "{0} {1}", fi.Name, fi.GetValue( this ).ToString( ) ) );
        }

        File.WriteAllLines( path, valuePairs.ToArray( ) );
    }
}
