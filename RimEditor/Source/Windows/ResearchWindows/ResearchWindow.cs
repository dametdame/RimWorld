// MainTabWindow_ResearchTree.cs
// Copyright Karel Kroeze, 2020-2020

using System.Collections.Generic;
using System.Linq;
using DRimEditor.Windows;
using RimWorld;
using UnityEngine;
using Verse;
using static DRimEditor.Research.Constants;

namespace DRimEditor.Research
{
    public class ResearchWindow : TabWindow
    {
        internal static Vector2 _scrollPosition = Vector2.zero;

        public static Rect _treeRect;

        private Rect _baseViewRect;
        private Rect _baseViewRect_Inner;

        private bool    _dragging;
        private Vector2 _mousePosition = Vector2.zero;

        private string _query = "";
        private Rect   _viewRect;

        private Rect _viewRect_Inner;
        private bool _viewRect_InnerDirty = true;
        private bool _viewRectDirty       = true;

        private float _zoomLevel = 1f;

        public static bool initialized = false;

        public static bool drawDummies = false;

        public ResearchWindow()
        {
            closeOnClickedOutside = false;
            Instance              = this;
        }

        public static ResearchWindow Instance { get; private set; }

        public float ScaledMargin => Constants.Margin * ZoomLevel / Prefs.UIScale;

        public float ZoomLevel
        {
            get => _zoomLevel;
            set
            {
                _zoomLevel           = Mathf.Clamp( value, 1f, MaxZoomLevel );
                _viewRectDirty       = true;
                _viewRect_InnerDirty = true;
            }
        }

        public Rect ViewRect
        {
            get
            {
                if ( _viewRectDirty )
                {
                    _viewRect = new Rect(
                        _baseViewRect.xMin   * ZoomLevel,
                        _baseViewRect.yMin   * ZoomLevel,
                        _baseViewRect.width  * ZoomLevel,
                        _baseViewRect.height * ZoomLevel
                    );
                    _viewRectDirty = false;
                }

                return _viewRect;
            }
        }

        public Rect ViewRect_Inner
        {
            get
            {
                if ( _viewRect_InnerDirty )
                {
                    _viewRect_Inner      = _viewRect.ContractedBy( Margin * ZoomLevel );
                    _viewRect_InnerDirty = false;
                }

                return _viewRect_Inner;
            }
        }

        public Rect TreeRect
        {
            get
            {
                if ( _treeRect == default )
                {
                    var width  = Tree.Size.x * ( NodeSize.x + NodeMargins.x );
                    var height = Tree.Size.z * ( NodeSize.y + NodeMargins.y );
                    _treeRect = new Rect( 0f, 0f, width, height );
                }

                return _treeRect;
            }
        }

        public Rect VisibleRect =>
            new Rect(
                _scrollPosition.x,
                _scrollPosition.y,
                ViewRect_Inner.width,
                ViewRect_Inner.height);

        internal float MaxZoomLevel
        {
            get
            {
                // get the minimum zoom level at which the entire tree fits onto the screen, or a static maximum zoom level.
                var fitZoomLevel = Mathf.Max( TreeRect.width  / _baseViewRect_Inner.width,
                                              TreeRect.height / _baseViewRect_Inner.height ) + 0.1f;
                return Mathf.Min( fitZoomLevel, AbsoluteMaxZoomLevel );
            }
        }

        public override void PreActive()
        {
            return;
        }

        public override void PreClose()
        {
            base.PreClose();
        }

        /*
        public override void PreOpen()
        {
            base.PreOpen();
           
            SetRects();

            // clear node availability caches
            ResearchNode.ClearCaches();

            _dragging             = false;
            closeOnClickedOutside = false;
           
        }*/

        public void Init(Rect canvas)
        {
            base.PreOpen();
            SetRects(canvas);

            // clear node availability caches
            ResearchNode.ClearCaches();

            _dragging = false;
            closeOnClickedOutside = false;
            ResearchSelectPanel._instance = new ResearchSelectPanel();
            initialized = true;
        }

        private void SetRects( Rect canvas)
        {
           _baseViewRect = new Rect(
                 canvas.x, canvas.yMin + TopBarHeight,
                 canvas.width, canvas.height - TopBarHeight );
              
            _baseViewRect_Inner = _baseViewRect.ContractedBy( Constants.Margin);
            // windowrect, set to topleft (for some reason vanilla alignment overlaps bottom buttons).
            windowRect.x      = canvas.x;
            windowRect.y      = canvas.y;
            windowRect.width  = canvas.width;
            windowRect.height = canvas.height;
        }

        public static void Refresh()
        {

            ResearchNode curSelected = ResearchSelectPanel.selected;
            _treeRect = default;
            ResearchNode.ClearCaches();
            Tree.Clear();
            LongEventHandler.QueueLongEvent(DRimEditor.Research.Tree.Initialize, "DRimEditor.Research.BuildingResearchTree", false, null);
            if (curSelected != null)
                LongEventHandler.QueueLongEvent(() => ResearchSelectPanel.selected = Tree._nodes.Find(n => (n is ResearchNode rn) && (rn.Research == curSelected.Research)) as ResearchNode, "", false, null);
            Find.WindowStack.TryRemove(MainWindow.instance);
            //Tree.Initialize();
        }

        public override void DoWindowContents( Rect canvas )
        {
            // top bar


            ApplyZoomLevel();

            // draw background
            GUI.DrawTexture( ViewRect, Assets.SlightlyDarkBackground );

            // draw the actual tree
            // TODO: stop scrollbars scaling with zoom
            _scrollPosition = GUI.BeginScrollView( ViewRect, _scrollPosition, TreeRect );
            GUI.BeginGroup(
                new Rect(
                    ScaledMargin,
                    ScaledMargin,
                    TreeRect.width  + ScaledMargin * 2f,
                    TreeRect.height + ScaledMargin * 2f
                )
            );
            Tree.Draw( VisibleRect );

            HandleZoom();

            GUI.EndGroup();
            GUI.EndScrollView( false );

            HandleDragging();
            
            HandleDolly();

            UI.ApplyUIScale();

            // reset zoom level
            var topRect = new Rect(
                canvas.xMin,
                canvas.yMin,
                canvas.width,
                TopBarHeight);
            DrawTopBar(topRect);

            ResetZoomLevel(canvas);

            HandleClicks();

            // cleanup;
            GUI.color   = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void HandleDolly()
        {
            var dollySpeed = 10f;
            if ( KeyBindingDefOf.MapDolly_Left.IsDown )
                _scrollPosition.x -= dollySpeed;
            if ( KeyBindingDefOf.MapDolly_Right.IsDown )
                _scrollPosition.x += dollySpeed;
            if ( KeyBindingDefOf.MapDolly_Up.IsDown )
                _scrollPosition.y -= dollySpeed;
            if ( KeyBindingDefOf.MapDolly_Down.IsDown )
                _scrollPosition.y += dollySpeed;
        }


        void HandleZoom()
        {
            // handle zoom only with shift
            if (Event.current.isScrollWheel && Event.current.shift)
            {
                // absolute position of mouse on research tree
                var absPos = Event.current.mousePosition;

                // relative normalized position of mouse on visible tree
                var relPos = ( Event.current.mousePosition - _scrollPosition ) / ZoomLevel;

                // update zoom level
                ZoomLevel += Event.current.delta.y * ZoomStep * ZoomLevel;

                // we want to keep the _normalized_ relative position the same as before zooming
                _scrollPosition = absPos - relPos * ZoomLevel;

                Event.current.Use();
            }
        }

        void HandleClicks()
        {
            if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
            {
                GUI.FocusControl(null);
            }
            if (Event.current.button == 1 && Event.current.type == EventType.MouseDown)
            {
                Vector2 curPos = new Vector2(Event.current.mousePosition.x + _scrollPosition.x, Event.current.mousePosition.y + _scrollPosition.y);
                List<FloatMenuOption> options = new List<FloatMenuOption> { new FloatMenuOption("Create New Research Project", () => ResearchProjectEditor.MakeNewNode(curPos), MenuOptionPriority.Default, null)};
                Find.WindowStack.Add(new FloatMenu(options));
                //Event.current.Use();
            }
        }

        void HandleDragging()
        {
            // middle mouse or holding down shift for panning
            if (Event.current.button == 2 || Event.current.shift) {
                if (Event.current.type == EventType.MouseDown)
                {
                    _dragging = true;
                    _mousePosition = Event.current.mousePosition;
                    Event.current.Use();
                }
                if (Event.current.type == EventType.MouseUp)
                {
                    _dragging = false;
                    _mousePosition = Vector2.zero;
                }
                if (Event.current.type == EventType.MouseDrag)
                {
                    var _currentMousePosition = Event.current.mousePosition;
                    _scrollPosition += _mousePosition - _currentMousePosition;
                    _mousePosition = _currentMousePosition;
                }
            }
            // scroll wheel vertical, switch to horizontal with alt
            if (Event.current.isScrollWheel && !Event.current.shift) {
                float delta = Event.current.delta.y * 15;
                if (Event.current.alt) {
                    _scrollPosition.x += delta;
                } else {
                    _scrollPosition.y += delta;
                }
            }
        }

        private void ApplyZoomLevel()
        {
            GUI.EndClip(); // window contents
            GUI.EndClip(); // window itself?
            GUI.matrix = Matrix4x4.TRS( new Vector3( 0f, 0f, 0f ), Quaternion.identity,
                                        new Vector3( Prefs.UIScale / ZoomLevel, Prefs.UIScale / ZoomLevel, 1f ) );
        }

        private void ResetZoomLevel(Rect canvas)
        {
            // moved this to main function
            //UI.ApplyUIScale(); 
            GUI.BeginClip( windowRect );
            GUI.BeginClip(canvas);
            //GUI.BeginClip( new Rect( 0f, 0f, UI.screenWidth, UI.screenHeight ) );
        }

        private void DrawTopBar( Rect canvas )
        {
            //GUI.EndClip();
           // GUI.EndClip();
            Rect searchRect = canvas;
            Rect queueRect  = canvas;
            searchRect.width =  200f;
            queueRect.xMin   += 200f + Constants.Margin;
            GUI.DrawTexture( searchRect, Assets.SlightlyDarkBackground );
            GUI.DrawTexture( queueRect, Assets.SlightlyLightBackground );

            DrawSearchBar( searchRect.ContractedBy( Constants.Margin ) );
            ResearchSelectPanel.DrawPanel(queueRect.ContractedBy(Constants.Margin), !_dragging);
        }

        private void DrawSearchBar( Rect canvas )
        {
            var iconRect = new Rect(
                    canvas.xMax - Constants.Margin - 16f,
                    0f,
                    16f,
                    16f )
               .CenteredOnYIn( canvas );
            var searchRect = new Rect(
                    canvas.xMin,
                    0f,
                    canvas.width,
                    30f )
               .CenteredOnYIn( canvas );

            GUI.DrawTexture( iconRect, Assets.Search );
            var query = Widgets.TextField( searchRect, _query );

            if ( query != _query )
            {
                _query = query;
                Find.WindowStack.FloatMenu?.Close( false );

                if ( query.Length > 2 )
                {
                    // open float menu with search results, if any.
                    var options = new List<FloatMenuOption>();

                    foreach ( var result in Tree.Nodes.OfType<ResearchNode>()
                                                .Select( n => new {node = n, match = n.Matches( query )} )
                                                .Where( result => result.match > 0 )
                                                .OrderBy( result => result.match ) )
                        options.Add( new FloatMenuOption( result.node.Label, () => CenterOn( result.node ),
                                                          MenuOptionPriority.Default, () => CenterOn( result.node ) ) );

                    if ( !options.Any() )
                        options.Add( new FloatMenuOption( ResourceBank.String.NoResearchFound, null ) );

                    Find.WindowStack.Add( new FloatMenu_Fixed( options,
                                                               UI.GUIToScreenPoint(
                                                                   new Vector2(
                                                                       searchRect.xMin, searchRect.yMax ) ) ) );
                }
            }
        }

        public void CenterOn( Node node )
        {
            var position = new Vector2(
                ( NodeSize.x + NodeMargins.x ) * ( node.X - .5f ),
                ( NodeSize.y + NodeMargins.y ) * ( node.Y - .5f ) );

            node.Highlighted = true;

            position -= new Vector2(windowRect.width, windowRect.height) / 2f;
            //position -= new Vector2( UI.screenWidth, UI.screenHeight ) / 2f;

            position.x      = Mathf.Clamp( position.x, 0f, TreeRect.width  - ViewRect.width );
            position.y      = Mathf.Clamp( position.y, 0f, TreeRect.height - ViewRect.height );
            _scrollPosition = position;
        }
    }
}