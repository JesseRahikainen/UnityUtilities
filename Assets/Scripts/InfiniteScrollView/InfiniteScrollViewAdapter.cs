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

// An interface used by the InfiniteScrollView to get data.
public abstract class InfiniteScrollViewAdapter : MonoBehaviour
{
    // creates new objects to display the data, this should generally instantiate a prefab you plan to use
    public abstract GameObject OnCreateViewHolder( );

    // gets the number of objects in the set, this can change while running
    public abstract int GetItemCount( );

    // used to configure the view holder to display the data at the specified position
    //  takes in an object created by OnCreateViewHolder( )
    public abstract void OnBindViewHolder( GameObject viewHolder, int position );
}
