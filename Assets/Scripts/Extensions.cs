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

using UnityEngine;

// Extensions to existing classes.
public static class Extensions
{
    // Returns the time in seconds of the curve (returns the key with the largest time)
    public static float TotalLength( this AnimationCurve curve )
    {
        float length = float.NegativeInfinity;
        for( int i = 0; i < curve.length; ++i ) {
            if( curve.keys[i].time > length ) {
                length = curve.keys[i].time;
            }
        }
        return length;
    }

    // Destroys all children of the transform
    public static void DestroyAllChildren( this Transform tf )
    {
        while( tf.childCount > 0 ) {
            GameObject.Destroy( tf.GetChild( 0 ).gameObject );
        }
    }
}
