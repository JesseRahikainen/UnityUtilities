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
using UnityEngine.Events;
using System;

// Simple way to have quick and easy cheat codes. Only works with Unity's old input system and requires all strings to be able to be
//  represented by a keycode available to the Input.GetKeyDown( ) method.
public class CheatCodes : MonoBehaviour
{
    [Serializable]
    public class Code {
        public string input; // must be standard English characters, is not case sensitive
        public bool enabled = true;
        public UnityEvent callback;
        
        private int currProgression;

        public void TestInput( )
        {
            if( !enabled ) {
                return;
            }

            if( input.Length <= 0 ) {
                Debug.LogWarning( "Cheat code with no input." );
                enabled = false;
                return;
            }

            string testInput = input.Substring( currProgression, 1 ).ToLower( );

            if( Input.GetKeyDown( testInput ) ) {
                ++currProgression;
            }

            if( currProgression >= input.Length ) {
                callback.Invoke( );
                currProgression = 0;
            }
        }

        public void Reset( )
        {
            currProgression = 0;
        }
    }

    [SerializeField]
    private List<Code> _codes;

    public void ResetCodes( )
    {

        foreach( Code c in _codes ) {
            c.Reset( );
        }
    }

    private void Start( )
    {
        ResetCodes( );
    }

    void Update()
    {
        foreach( Code c in _codes ) {
            c.TestInput( );
        }
    }
}
