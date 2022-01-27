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
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Based on the RecyclerView from the Android library. Allows you to use a scroll view with
//  very large data sets by recycling the objects used to display the information.
public class InfiniteScrollView : UIBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public InfiniteScrollViewAdapter adapter;

    public enum Layout {
        Vertical,
        Horizontal
    }
    public Layout layout;

    public float spacing;
    public float padding;

    public bool useInertia;
    public float inertia;
    public float inertiaReduction;

    public int numViewHolders = 10;

    public GameObject viewRoot;

    private bool _isDragging;
    private Vector3 _velocity = Vector3.zero;

    private struct ViewHolder {
        public GameObject obj;
        public int index;
    }
    private LinkedList<ViewHolder> _viewHolders = new LinkedList<ViewHolder>( );

    protected override void OnEnable( )
    {
        if( layout == Layout.Vertical ) {
            Build( Build_GetStart_Vertical, Build_Advance_Vertical );
        } else {
            Build( Build_GetStart_Horizontal, Build_Advance_Horizontal );
        }
    }

    private delegate Vector3 Build_GetStart( RectTransform rt );
    private delegate Vector3 Build_Advance( RectTransform viewHolder, ref Vector3 currPos );

    private Vector3 Build_GetStart_Vertical( RectTransform rt )
    {
        Vector3 pos = rt.rect.center;
        pos.y = rt.rect.yMax - padding;
        return pos;
    }

    private Vector3 Build_Advance_Vertical( RectTransform viewHolder, ref Vector3 currPos )
    {
        currPos.y -= viewHolder.rect.height / 2.0f;
        Vector3 pos = currPos;
        currPos.y -= ( viewHolder.rect.height / 2.0f ) + spacing;

        return pos;
    }

    private Vector3 Build_GetStart_Horizontal( RectTransform rt )
    {
        Vector3 pos = rt.rect.center;
        pos.x = rt.rect.xMin + padding;
        return pos;
    }

    private Vector3 Build_Advance_Horizontal( RectTransform viewHolder, ref Vector3 currPos )
    {
        currPos.x += viewHolder.rect.width / 2.0f;
        Vector3 pos = currPos;
        currPos.x += ( viewHolder.rect.width / 2.0f ) + spacing;

        return pos;
    }

    private void Build( Build_GetStart getStart, Build_Advance advance )
    {
        RectTransform ourTF = viewRoot.transform as RectTransform;
        Vector3 currentPos = getStart( ourTF );

        for( int i = 0; i < numViewHolders; ++i ) {
            ViewHolder vh = new ViewHolder( );

            vh.obj = adapter.OnCreateViewHolder( );

            RectTransform vhRectTF = vh.obj.transform as RectTransform;
            vhRectTF.pivot = new Vector2( 0.5f, 0.5f );

            vh.obj.transform.SetParent( viewRoot.transform );

            if( i < adapter.GetItemCount( ) ) {
                vh.index = i;
                adapter.OnBindViewHolder( vh.obj, vh.index );
            } else {
                vh.index = -1;
                vh.obj.SetActive( false );
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate( vhRectTF );

            vh.obj.transform.localPosition = advance( vhRectTF, ref currentPos );

            _viewHolders.AddLast( vh );
        }
    }

    public void OnBeginDrag( PointerEventData eventData )
    {
        _isDragging = true;
    }

    public void OnEndDrag( PointerEventData eventData )
    {
        _isDragging = false;
    }

    public void OnDrag( PointerEventData eventData )
    {
        if( eventData.button != PointerEventData.InputButton.Left ) {
            return;
        }

        if( !IsActive( ) ) {
            return;
        }

        Vector3 translation = new Vector3( layout == Layout.Horizontal ? eventData.delta.x : 0.0f, layout == Layout.Vertical ? eventData.delta.y : 0.0f, 0.0f );
        translation = ClampTranslation( translation );

        ApplyTranslation( translation );

        // set the inertia velocity based on the translation
        _velocity = ( translation / Time.unscaledDeltaTime ) * inertiaReduction;
    }

    private void ApplyTranslation( Vector3 translation )
    {
        // adjust the positions of all the children
        for( int i = 0; i < viewRoot.transform.childCount; ++i ) {
            viewRoot.transform.GetChild( i ).Translate( translation, Space.Self );
        }

        // see if any of the views have to be recycled
        CheckViewHolders( );
    }

    void LateUpdate( )
    {
        if( _isDragging ) {
            return;
        }

        if( useInertia ) {
            _velocity *= Mathf.Pow( inertia, Time.unscaledDeltaTime );
            if( Mathf.Abs( _velocity.x ) < 1.0f ) {
                _velocity.x = 0.0f;
            }
            if( Mathf.Abs( _velocity.y ) < 1.0f ) {
                _velocity.y = 0.0f;
            }
            Vector3 translation = ClampTranslation( _velocity * Time.unscaledDeltaTime );
            ApplyTranslation( translation );
        }
    }


    private delegate int ClampTranslation_TestDirection( Vector3 translation );
    private delegate float ClampTranslation_CalculateDiff( Bounds rootBounds, Bounds childBounds, Vector3 translation );
    private delegate void ClampTranslation_ApplyDiff( float diff, ref Vector3 translation );

    private Vector3 ClampTranslationByLayout( Vector3 translation, ClampTranslation_TestDirection testDirection,
        ClampTranslation_CalculateDiff toEndCalcDiff, ClampTranslation_CalculateDiff toStartCalcDiff,
        ClampTranslation_ApplyDiff applyDiff )
    {
        // returns the Vector3 to clamp based on the bounds of the list
        if( ( testDirection( translation ) == -1 ) && ( _viewHolders.First.Value.index == 0 ) ) {
            // moving to end, need to check top if it's the first
            //  get distance from top of top most element
            Bounds rootBounds = Util.GetBounds( viewRoot.transform as RectTransform );
            Bounds childBounds = Util.GetBounds( _viewHolders.First.Value.obj.transform as RectTransform );

            float diff = toEndCalcDiff( rootBounds, childBounds, translation );
            if( diff > 0.0f ) {
                applyDiff( diff, ref translation );
            }

        } else if( ( testDirection( translation ) == 1 ) && ( _viewHolders.Last.Value.index == ( adapter.GetItemCount( ) - 1 ) ) ) {
            // moving up, need to check bottom if it's the last
            Bounds rootBounds = Util.GetBounds( viewRoot.transform as RectTransform );
            Bounds childBounds = Util.GetBounds( _viewHolders.Last.Value.obj.transform as RectTransform );

            float diff = toStartCalcDiff( rootBounds, childBounds, translation );
            if( diff < 0.0f ) {
                applyDiff( diff, ref translation );
            }
        }

        return translation;
    }

    private void ClampTranslation_ApplyDiff_Vertical( float diff, ref Vector3 translation )
    {
        translation.y += diff;
    }

    private void ClampTranslation_ApplyDiff_Horizontal( float diff, ref Vector3 translation )
    {
        translation.x -= diff;
    }

    private float ClampTranslation_CalculateDiffToEnd_Vertical( Bounds rootBounds, Bounds childBounds, Vector3 translation )
    {
        return ( rootBounds.max.y - padding ) - ( childBounds.max.y + translation.y );
    }

    private float ClampTranslation_CalculateDiffToEnd_Horizontal( Bounds rootBounds, Bounds childBounds, Vector3 translation )
    {
        return -( ( rootBounds.min.x + padding ) - ( childBounds.min.x + translation.x ) );
    }

    private float ClampTranslation_CalculateDiffToStart_Vertical( Bounds rootBounds, Bounds childBounds, Vector3 translation )
    {
        return ( rootBounds.min.y + padding ) - ( childBounds.min.y + translation.y );
    }

    private float ClampTranslation_CalculateDiffToStart_Horizontal( Bounds rootBounds, Bounds childBounds, Vector3 translation )
    {
        return -( ( rootBounds.max.x - padding ) - ( childBounds.max.x + translation.x ) );
    }

    private int ClampTranslation_TestDirection_Horizontal( Vector3 translation )
    {
        if( translation.x > 0.0f ) {
            return -1; // moving right
        } else if( translation.x < 0.0f ) {
            return 1; // moving left
        }
        return 0; // not moving
    }

    private int ClampTranslation_TestDirection_Vertical( Vector3 translation )
    {
        if( translation.y < 0.0f ) {
            return -1; // moving down
        } else if( translation.y > 0.0f ) {
            return 1; // moving up
        }
        return 0; // not moving
    }

    private Vector3 ClampTranslation( Vector3 translation )
    {
        if( layout == Layout.Vertical ) {
            return ClampTranslationByLayout( translation, ClampTranslation_TestDirection_Vertical, ClampTranslation_CalculateDiffToEnd_Vertical,
                ClampTranslation_CalculateDiffToStart_Vertical, ClampTranslation_ApplyDiff_Vertical );
        } else {
            return ClampTranslationByLayout( translation, ClampTranslation_TestDirection_Horizontal, ClampTranslation_CalculateDiffToEnd_Horizontal,
                ClampTranslation_CalculateDiffToStart_Horizontal, ClampTranslation_ApplyDiff_Horizontal );
        }
    }



    private void CheckViewHolders( )
    {
        if( layout == Layout.Vertical ) {
            RunCheckViewHolders( CheckViewHolders_IsPre_Vertical, CheckViewHolders_AdjustPosition_Vertical );
        } else {
            RunCheckViewHolders( CheckViewHolders_IsPre_Horizontal, CheckViewHolders_AdjustPosition_Horizontal );
        }
    }

    // returns if the childRect would be positioned closer to the beginning or end of the list
    //  we're already assuming they're not overlapping
    private delegate bool CheckViewHolders_IsPre( Rect rootRect, Rect childRect );
    private delegate Vector3 CheckViewHolders_AdjustPosition( Rect basedOn, Rect moved, Vector3 origPos, float mult );


    private bool CheckViewHolders_IsPre_Vertical( Rect rootRect, Rect childRect )
    {
        return ( childRect.yMax > rootRect.yMin );
    }

    private Vector3 CheckViewHolders_AdjustPosition_Vertical( Rect basedOn, Rect moved, Vector3 origPos, float mult )
    {
        origPos.y += ( ( basedOn.height / 2.0f ) + spacing + ( moved.height / 2.0f ) ) * mult;
        return origPos;
    }

    private bool CheckViewHolders_IsPre_Horizontal( Rect rootRect, Rect childRect )
    {
        return ( childRect.xMin < rootRect.xMax );
    }

    private Vector3 CheckViewHolders_AdjustPosition_Horizontal( Rect basedOn, Rect moved, Vector3 origPos, float mult )
    {
        origPos.x -= ( ( basedOn.width / 2.0f ) + spacing + ( moved.width / 2.0f ) ) * mult;
        return origPos;
    }

    private void RunCheckViewHolders( CheckViewHolders_IsPre isPre, CheckViewHolders_AdjustPosition adjustPos )
    {
        RectTransform rootRT = (RectTransform)( viewRoot.transform );

        // we want the number above the view root and the number below the view root to be equal
        int numPre = 0;
        int numPost = 0;
        // test to see which of the non-overlapping view holders are farther away from the edges
        foreach( ViewHolder vh in _viewHolders ) {
            RectTransform childRT = vh.obj.transform as RectTransform;

            Rect childRect = new Rect( childRT.rect );
            childRect.center = childRT.localPosition;

            if( !rootRT.rect.Overlaps( childRect ) ) {
                if( isPre( rootRT.rect, childRect ) ) {
                    ++numPre;
                } else {
                    ++numPost;
                }
            }
        }

        int avg = Mathf.CeilToInt( ( numPre + numPost ) / 2.0f );
        if( numPre > avg ) {
            int diff = numPre - avg;

            for( int i = 0; ( i < diff ) && ( _viewHolders.Last.Value.index < ( adapter.GetItemCount( ) - 1 ) ); ++i ) {
                // grab the above and make it below
                //  first adjust it in the list
                LinkedListNode<ViewHolder> node = _viewHolders.First;
                _viewHolders.Remove( node );
                _viewHolders.AddLast( node );

                LinkedListNode<ViewHolder> basedOn = node.Previous;
                ViewHolder vh = node.Value;

                // then change it's contents
                vh.index = basedOn.Value.index + 1;
                adapter.OnBindViewHolder( vh.obj, vh.index );
                RectTransform movedTf = vh.obj.transform as RectTransform;
                LayoutRebuilder.ForceRebuildLayoutImmediate( movedTf );

                // then change it's position
                RectTransform basedOnTf = basedOn.Value.obj.transform as RectTransform;
                vh.obj.transform.localPosition = adjustPos( basedOnTf.rect, movedTf.rect, basedOn.Value.obj.transform.localPosition, -1.0f );

                node.Value = vh;
            }
        } else if( numPost > avg ) {
            int diff = numPost - avg;

            for( int i = 0; ( i < diff ) && ( _viewHolders.First.Value.index > 0 ); ++i ) {
                // grab the below and make it above
                //  first adjust it in the list
                LinkedListNode<ViewHolder> node = _viewHolders.Last;
                _viewHolders.Remove( node );
                _viewHolders.AddFirst( node );

                LinkedListNode<ViewHolder> basedOn = node.Next;
                ViewHolder vh = node.Value;

                // then change it's contents
                vh.index = basedOn.Value.index - 1;
                adapter.OnBindViewHolder( vh.obj, vh.index );
                RectTransform movedTf = vh.obj.transform as RectTransform;
                LayoutRebuilder.ForceRebuildLayoutImmediate( movedTf );

                // then change it's position
                RectTransform basedOnTf = basedOn.Value.obj.transform as RectTransform;
                vh.obj.transform.localPosition = adjustPos( basedOnTf.rect, movedTf.rect, basedOn.Value.obj.transform.localPosition, 1.0f );

                node.Value = vh;
            }
        }
    }
}
