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

// Used for testing the BaseConfig class.
public class TestConfig : BaseConfig {
    public int testInt = 10;
    public string testString = "testing";


    public float TestFloat { get; private set; } = 10.0f;

    [System.Serializable]
    public enum TestingEnum {
        Value0,
        Value1,
        Value2,
        Value3
    }

    public TestingEnum testEnum = TestingEnum.Value2;
    public TestingEnum TestPropEnum { get; private set; } = TestingEnum.Value3;

    private void Start( )
    {
        Debug.Log( "testInt: " + testInt );
        Debug.Log( "testString: " + testString );
        Debug.Log( "TestFloat: " + TestFloat );
        Debug.Log( "testEnum: " + testEnum );
        Debug.Log( "TestPropEnum: " + TestPropEnum );

        Debug.Log( "changing string to 'testWrite'" );

        testString = "testWrite";

        WriteConfigData( );
    }
}
