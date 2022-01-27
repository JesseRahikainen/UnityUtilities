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

// Helper functions
public static class Util
{
    // expects a value in the range [0,1], returns a value in the range [0,1] run through the function f(x)=-2*x^3+3*x^2
    //  the derivative is zero at both end points
    public static float HermiteBlend( float v )
    {
        return ( -2.0f * v * v * v ) + ( 3.0f * v * v );
    }

    // expects a value in the range [0,1], returns a value in the range [0,1] run through the function f(x)=6*x^5-15*x^4+10*x^3
    //  the first and second derivate is zero at both end points
    public static float PerlinBlend( float v )
    {
        return ( 6.0f * v * v * v * v * v ) + ( -15.0f * v * v * v * v ) + ( 10.0f * v * v * v );
    }

    // expects a value in the range [0,1], returns a value in the range [0,1] run through the function f(x)=3.2*x^3-4.8*x^2+2.6*x
    //  returns something that looks like it may be the inverse of hermite blend, but isn't
    public static float ApproxInverseHermite( float v )
    {
        return ( 3.2f * v * v * v ) + ( -4.8f * v * v ) + ( 2.6f * v );
    }

    // draw a circle using the Debug.DrawLine( ) method
    public static void DrawDebugCircle2D( Vector2 pos, float radius, int resolution, Color clr )
    {
        if( resolution <= 5 ) resolution = 5;

        float step = ( 2.0f * Mathf.PI ) / resolution;
        Vector2 prevPos = pos + new Vector2( radius * Mathf.Sin( 0.0f ), radius * Mathf.Cos( 0.0f ) );
        for( int i = 1; i <= resolution; ++i ) {
            Vector2 nextPos = pos + new Vector2( radius * Mathf.Sin( i * step ), radius * Mathf.Cos( i * step ) );
            Debug.DrawLine( prevPos, nextPos, clr );
            prevPos = nextPos;
        }
    }

    // draw an AABB using the Debug.DrawLine( ) method
    public static void DrawDebugAABB2D( Rect r, Color clr )
    {
        Vector2 minMin = r.min;
        Vector2 maxMax = r.max;
        Vector2 minMax = new Vector2( r.xMin, r.yMax );
        Vector2 maxMin = new Vector2( r.xMax, r.yMin );
        Debug.DrawLine( minMin, minMax, clr );
        Debug.DrawLine( minMax, maxMax, clr );
        Debug.DrawLine( maxMax, maxMin, clr );
        Debug.DrawLine( maxMin, minMin, clr );
    }

    // checks to see if the layer is in the mask
    public static bool IsLayerInMask( int layer, LayerMask mask )
    {
        return ( ( 1 << layer ) & mask.value ) != 0;
    }

    // given a RectTransform finds the global bounds of it
    private static Vector3[] _corners = new Vector3[4];
    public static Bounds GetBounds( RectTransform rt )
    {
        rt.GetWorldCorners( _corners );
        
        Vector3 min = new Vector3( float.MaxValue, float.MaxValue, float.MaxValue );
        Vector3 max = new Vector3( float.MinValue, float.MinValue, float.MinValue );

        for( int i = 0; i < 4; ++i ) {
            min = Vector3.Min( _corners[i], min );
            max = Vector3.Max( _corners[i], max );
        }

        Bounds b = new Bounds( );
        b.SetMinMax( min, max );
        return b;
    }

    // given a RectTRansform find the local bounds of it
    public static Bounds GetLocalBounds( RectTransform rt )
    {
        rt.GetWorldCorners( _corners );
        Matrix4x4 w2l = rt.worldToLocalMatrix;

        Vector3 min = new Vector3( float.MaxValue, float.MaxValue, float.MaxValue );
        Vector3 max = new Vector3( float.MinValue, float.MinValue, float.MinValue );

        for( int i = 0; i < 4; ++i ) {
            Vector3 adjPos = w2l.MultiplyPoint3x4( _corners[i] );
            min = Vector3.Min( adjPos, min );
            max = Vector3.Max( adjPos, max );
        }

        Bounds b = new Bounds( );
        b.SetMinMax( min, max );
        return b;
    }
}
