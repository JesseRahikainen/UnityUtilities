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
using TMPro;

public class TestAdapter : InfiniteScrollViewAdapter
{
    public GameObject viewPrefab;

    [SerializeField]
    [TextArea(1,3)]
    private string[] _testData = {
        "Test 0",
        "Test 1",
        "Test 2",
        "Test 3",
        "Test 4",
        "Test 5",
        "Test 6",
        "Test 7",
        "Test 8",
        "Test 9",
        "Test 10",
        "Test\nTest\nTest\nTest\nTest\nTest\nTest",
        "Test 11",
        "Test 12",
        "Test 13",
        "Test 14",
        "Test 15",
    };

    public override int GetItemCount( )
    {
        return _testData.Length;
    }

    public override void OnBindViewHolder( GameObject viewHolder, int position )
    {
        TMP_Text txt = viewHolder.GetComponentInChildren<TMP_Text>( );

        if( ( position < 0 ) || ( position >= _testData.Length ) ) {
            // just for debugging purposes
            txt.text = "ERROR! " + position;
        } else {
            txt.text = position + ": " + _testData[position];
        }
    }

    public override GameObject OnCreateViewHolder( )
    {
        return Instantiate( viewPrefab );
    }

    public void SetTesting( GameObject viewHolder, bool testValue )
    {
        TMP_Text txt = viewHolder.GetComponentInChildren<TMP_Text>( );
        txt.color = testValue ? Color.white : Color.red;
    }
}
