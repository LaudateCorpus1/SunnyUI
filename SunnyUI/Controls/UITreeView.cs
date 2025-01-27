﻿/******************************************************************************
* SunnyUI 开源控件库、工具类库、扩展类库、多页面开发框架。
* CopyRight (C) 2012-2021 ShenYongHua(沈永华).
* QQ群：56829229 QQ：17612584 EMail：SunnyUI@QQ.Com
*
* Blog:   https://www.cnblogs.com/yhuse
* Gitee:  https://gitee.com/yhuse/SunnyUI
* GitHub: https://github.com/yhuse/SunnyUI
*
* SunnyUI.dll can be used for free under the GPL-3.0 license.
* If you use this code, please keep this note.
* 如果您使用此代码，请保留此说明。
******************************************************************************
* 文件名称: UITreeView.cs
* 文件说明: 树形列表
* 当前版本: V3.0
* 创建日期: 2020-05-05
*
* 2020-05-05: V2.2.5 增加文件
* 2020-07-07: V2.2.6 全部重写，增加圆角，CheckBoxes等
* 2020-08-12: V2.2.7 更新可设置背景色
* 2021-07-19: V3.0.5 调整了显示CheckBoxes时图片位置
* 2021-08-26: V3.0.6 CheckBoxes增加三态，感谢群友：笑口常开 
******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Sunny.UI
{
    public sealed class UITreeView : UIPanel, IToolTip
    {
        private UIScrollBar Bar;

        private bool ScrollBarVisible;
        private TreeViewEx view;

        public UITreeView()
        {
            InitializeComponent();
            SetStyleFlags(true, false);
            ShowText = false;
            SetScrollInfo();

            view.BeforeCheck += View_BeforeCheck;
            view.AfterCheck += View_AfterCheck;
            view.BeforeCollapse += View_BeforeCollapse;
            view.AfterCollapse += View_AfterCollapse;
            view.BeforeExpand += View_BeforeExpand;
            view.AfterExpand += View_AfterExpand;
            view.DrawNode += View_DrawNode;
            view.ItemDrag += View_ItemDrag;
            view.NodeMouseHover += View_NodeMouseHover;
            view.BeforeSelect += View_BeforeSelect;
            view.AfterSelect += View_AfterSelect;
            view.NodeMouseClick += View_NodeMouseClick;
            view.NodeMouseDoubleClick += View_NodeMouseDoubleClick;
            view.MouseUp += View_MouseUp;
            view.MouseDown += View_MouseDown;
            view.MouseMove += View_MouseMove;
            view.MouseEnter += View_MouseEnter;
            view.MouseLeave += View_MouseLeave;
            view.KeyPress += View_KeyPress;
            view.KeyDown += View_KeyDown;
            view.KeyUp += View_KeyUp;
            view.AfterLabelEdit += View_AfterLabelEdit;
        }

        private void View_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            AfterLabelEdit?.Invoke(this, e);
        }

        public Control ExToolTipControl()
        {
            return view;
        }

        public event NodeLabelEditEventHandler AfterLabelEdit;
        public new event EventHandler MouseLeave;
        public new event EventHandler MouseEnter;
        public new event MouseEventHandler MouseMove;
        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseUp;
        public new event KeyPressEventHandler KeyPress;
        public new event KeyEventHandler KeyDown;
        public new event KeyEventHandler KeyUp;

        [DefaultValue(false)]
        public bool LabelEdit
        {
            get => view.LabelEdit;
            set => view.LabelEdit = true;
        }

        private void View_KeyUp(object sender, KeyEventArgs e)
        {
            KeyUp?.Invoke(this, e);
        }

        private void View_KeyDown(object sender, KeyEventArgs e)
        {
            KeyDown?.Invoke(this, e);
        }

        private void View_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPress?.Invoke(this, e);
        }


        private void View_MouseLeave(object sender, EventArgs e)
        {
            MouseLeave?.Invoke(this, e);
        }

        private void View_MouseEnter(object sender, EventArgs e)
        {
            MouseEnter?.Invoke(this, e);
        }

        private void View_MouseMove(object sender, MouseEventArgs e)
        {
            MouseMove?.Invoke(this, e);
        }

        private void View_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDown?.Invoke(this, e);
        }

        private void View_MouseUp(object sender, MouseEventArgs e)
        {
            MouseUp?.Invoke(this, e);
        }

        [Browsable(false)]
        public TreeView TreeView => view;

        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);
            if (view != null)
            {
                view.SelectedForeColor = UIColor.White;
                view.FillColor = view.BackColor = fillColor = UIColor.White;

                rectColor = uiColor.RectColor;
                view.SelectedColor = uiColor.TreeViewSelectedColor;
                view.ForeColor = foreColor = UIFontColor.Primary;
                view.HoverColor = uiColor.TreeViewHoverColor;
            }

            if (Bar != null)
            {
                Bar.FillColor = UIColor.White;

                Bar.ForeColor = uiColor.PrimaryColor;
                Bar.HoverColor = uiColor.ButtonFillHoverColor;
                Bar.PressColor = uiColor.ButtonFillPressColor;
            }

            Invalidate();
        }

        protected override void AfterSetFillColor(Color color)
        {
            base.AfterSetFillColor(color);
            if (view != null)
            {
                view.FillColor = color;
                view.BackColor = color;
            }

            if (Bar != null)
            {
                Bar.FillColor = color;
            }
        }

        protected override void AfterSetForeColor(Color color)
        {
            base.AfterSetForeColor(color);
            view.ForeColor = color;
        }

        [DefaultValue(TreeViewDrawMode.OwnerDrawAll)]
        public TreeViewDrawMode DrawMode
        {
            get => view.DrawMode;
            set => view.DrawMode = value;
        }

        [DefaultValue(false)]
        public bool CheckBoxes
        {
            get => view.CheckBoxes;
            set => view.CheckBoxes = value;
        }

        [DefaultValue(false)]
        public bool ShowLines
        {
            get => view.ShowLinesEx;
            set => view.ShowLinesEx = value;
        }

        [DefaultValue(28)]
        public int ItemHeight
        {
            get => view.ItemHeight;
            set => view.ItemHeight = value;
        }

        [Browsable(false)]
        public TreeNode SelectedNode
        {
            get => view.SelectedNode;
            set => view.SelectedNode = value;
        }

        [DefaultValue(true)]
        public bool HideSelection
        {
            get => view.HideSelection;
            set => view.HideSelection = value;
        }

        [DefaultValue(-1)]
        [Localizable(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [TypeConverter(typeof(NoneExcludedImageIndexConverter))]
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            typeof(UITypeEditor))]
        [RelatedImageList("ImageList")]
        public int ImageIndex
        {
            get => view.ImageIndex;
            set => view.ImageIndex = value;
        }

        [Localizable(true)]
        [TypeConverter(typeof(ImageKeyConverter))]
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            typeof(UITypeEditor))]
        [DefaultValue("")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [RelatedImageList("ImageList")]
        public string ImageKey
        {
            get => view.ImageKey;
            set => view.ImageKey = value;
        }

        [DefaultValue(null)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public ImageList ImageList
        {
            get => view.ImageList;
            set => view.ImageList = value;
        }

        [DefaultValue(null)]
        public ImageList StateImageList
        {
            get => view.StateImageList;
            set => view.StateImageList = value;
        }

        [Localizable(true)]
        [DefaultValue(19)]
        public int Indent
        {
            get => view.Indent;
            set => view.Indent = value;
        }

        [DefaultValue(typeof(Color), "Black")]
        public Color LineColor
        {
            get => view.LineColor;
            set => view.LineColor = value;
        }

        [DefaultValue("\\")]
        public string PathSeparator
        {
            get => view.PathSeparator;
            set => view.PathSeparator = value;
        }

        [DefaultValue(-1)]
        [TypeConverter(typeof(NoneExcludedImageIndexConverter))]
        [Localizable(true)]
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            typeof(UITypeEditor))]
        [RelatedImageList("ImageList")]
        public int SelectedImageIndex
        {
            get => view.SelectedImageIndex;
            set => view.SelectedImageIndex = value;
        }

        [Localizable(true)]
        [TypeConverter(typeof(ImageKeyConverter))]
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            typeof(UITypeEditor))]
        [DefaultValue("")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [RelatedImageList("ImageList")]
        public string SelectedImageKey
        {
            get => view.SelectedImageKey;
            set => view.SelectedImageKey = value;
        }

        [DefaultValue(false)]
        public bool ShowNodeToolTips
        {
            get => view.ShowNodeToolTips;
            set => view.ShowNodeToolTips = value;
        }

        [DefaultValue(true)]
        public bool ShowPlusMinus
        {
            get => view.ShowPlusMinus;
            set => view.ShowPlusMinus = value;
        }

        [DefaultValue(false)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Sorted
        {
            get => view.Sorted;
            set => view.Sorted = value;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IComparer TreeViewNodeSorter
        {
            get => view.TreeViewNodeSorter;
            set => view.TreeViewNodeSorter = value;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TreeNode TopNode
        {
            get => view.TopNode;
            set => view.TopNode = value;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int VisibleCount => view.VisibleCount;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Localizable(true)]
        [MergableProperty(false)]
        public TreeNodeCollection Nodes => view.Nodes;

        public event TreeViewCancelEventHandler BeforeCheck;

        public event TreeViewEventHandler AfterCheck;

        public event TreeViewCancelEventHandler BeforeCollapse;

        public event TreeViewEventHandler AfterCollapse;

        public event TreeViewCancelEventHandler BeforeExpand;

        public event TreeViewEventHandler AfterExpand;

        public event DrawTreeNodeEventHandler DrawNode;

        public event ItemDragEventHandler ItemDrag;

        public event TreeNodeMouseHoverEventHandler NodeMouseHover;

        public event TreeViewCancelEventHandler BeforeSelect;

        public event TreeViewEventHandler AfterSelect;

        public event TreeNodeMouseClickEventHandler NodeMouseClick;

        public event TreeNodeMouseClickEventHandler NodeMouseDoubleClick;

        private void View_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            NodeMouseDoubleClick?.Invoke(this, e);
        }

        private void View_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            NodeMouseClick?.Invoke(this, e);
        }

        private void View_AfterSelect(object sender, TreeViewEventArgs e)
        {
            AfterSelect?.Invoke(this, e);
        }

        private void View_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            BeforeSelect?.Invoke(this, e);
        }

        private void View_NodeMouseHover(object sender, TreeNodeMouseHoverEventArgs e)
        {
            NodeMouseHover?.Invoke(this, e);
        }

        private void View_ItemDrag(object sender, ItemDragEventArgs e)
        {
            ItemDrag?.Invoke(this, e);
        }

        private void View_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            DrawNode?.Invoke(this, e);
        }

        private void View_AfterExpand(object sender, TreeViewEventArgs e)
        {
            AfterExpand?.Invoke(this, e);
        }

        private void View_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            BeforeExpand?.Invoke(this, e);
        }

        private void View_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            AfterCollapse?.Invoke(this, e);
        }

        private void View_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            BeforeCollapse?.Invoke(this, e);
        }

        private void View_AfterCheck(object sender, TreeViewEventArgs e)
        {
            AfterCheck?.Invoke(this, e);
        }

        private void View_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            BeforeCheck?.Invoke(this, e);
        }

        public void BeginUpdate()
        {
            view.BeginUpdate();
        }

        public void CollapseAll()
        {
            view.CollapseAll();
        }

        public void EndUpdate()
        {
            view.EndUpdate();
        }

        public void ExpandAll()
        {
            view.ExpandAll();
        }

        public TreeViewHitTestInfo HitTest(Point pt)
        {
            return view.HitTest(pt);
        }

        public TreeViewHitTestInfo HitTest(int x, int y)
        {
            return view.HitTest(x, y);
        }

        public int GetNodeCount(bool includeSubTrees)
        {
            return view.GetNodeCount(includeSubTrees);
        }

        public TreeNode GetNodeAt(Point pt)
        {
            return view.GetNodeAt(pt);
        }

        public TreeNode GetNodeAt(int x, int y)
        {
            return view.GetNodeAt(x, y);
        }

        public override string ToString()
        {
            return view.ToString();
        }

        public void Sort()
        {
            view.Sort();
        }

        private void SetPos()
        {
            if (view == null) return;
            view.Left = 2;
            view.Top = 2;
            view.Width = Width - 4;
            view.Height = Height - 4;

            if (Bar == null) return;
            Bar.Top = 2;
            Bar.Left = Width - ScrollBarInfo.VerticalScrollBarWidth() - 2;
            Bar.Width = ScrollBarInfo.VerticalScrollBarWidth();
            Bar.Height = Height - 4;
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            if (view != null)
            {
                view.IsScaled = true;
                view.Font = Font;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (e.Delta > 10)
                ScrollBarInfo.ScrollUp(view.Handle);
            else if (e.Delta < -10) ScrollBarInfo.ScrollDown(view.Handle);

            SetScrollInfo();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SetScrollInfo();
            SetPos();
        }

        public void SetScrollInfo()
        {
            if (view == null || Bar == null) return;

            if (Nodes.Count == 0)
            {
                Bar.Visible = false;
                return;
            }

            var si = ScrollBarInfo.GetInfo(view.Handle);

            SetPos();
            Bar.Maximum = si.ScrollMax;
            Bar.Visible = si.ScrollMax > 0 && si.nMax > 0 && si.nPage > 0;
            Bar.Value = si.nPos;
            Bar.BringToFront();

            if (ScrollBarVisible != Bar.Visible)
            {
                ScrollBarVisible = Bar.Visible;
                Invalidate();
            }
        }

        private void InitializeComponent()
        {
            view = new TreeViewEx();
            Bar = new UIScrollBar();
            SuspendLayout();
            //
            // view
            //
            view.BackColor = Color.White;
            view.BorderStyle = BorderStyle.None;
            view.DrawMode = TreeViewDrawMode.OwnerDrawAll;
            view.ForeColor = Color.FromArgb(48, 48, 48);
            view.FullRowSelect = true;
            view.ItemHeight = 28;
            view.Location = new Point(2, 2);
            view.Name = "view";
            view.ShowLines = false;
            view.Size = new Size(266, 176);
            view.TabIndex = 0;
            view.AfterCollapse += view_AfterCollapse;
            view.AfterExpand += view_AfterExpand;
            view.DrawNode += view_DrawNode;
            //
            // Bar
            //
            Bar.Font = new Font("微软雅黑", 12F);
            Bar.Location = new Point(247, 3);
            Bar.Name = "Bar";
            Bar.Size = new Size(19, 173);
            Bar.Style = UIStyle.Custom;
            Bar.StyleCustomMode = true;
            Bar.TabIndex = 2;
            Bar.Visible = false;
            Bar.ValueChanged += Bar_ValueChanged;
            //
            // UITreeViewEx
            //
            Controls.Add(Bar);
            Controls.Add(view);
            FillColor = Color.White;
            Style = UIStyle.Custom;
            ResumeLayout(false);
        }

        private void Bar_ValueChanged(object sender, EventArgs e)
        {
            ScrollBarInfo.SetScrollValue(view.Handle, Bar.Value);
        }

        private void view_AfterExpand(object sender, TreeViewEventArgs e)
        {
            SetScrollInfo();
        }

        private void view_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            SetScrollInfo();
        }

        private void view_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            SetScrollInfo();
        }

        internal sealed class NoneExcludedImageIndexConverter : ImageIndexConverter
        {
            protected override bool IncludeNoneAsStandardValue => false;
        }

        internal class TreeViewEx : TreeView
        {
            private TreeNode CurrentNode;

            private bool showLines;

            public TreeViewEx()
            {
                DrawMode = TreeViewDrawMode.OwnerDrawAll;
                base.DoubleBuffered = true;
            }

            [Browsable(false)]
            public bool IsScaled { get; set; }

            public void SetDPIScale()
            {
                if (!IsScaled)
                {
                    this.SetDPIScaleFont();
                    IsScaled = true;
                }
            }

            [DefaultValue(typeof(Color), "155, 200, 255")]
            public Color HoverColor { get; set; } = Color.FromArgb(155, 200, 255);

            public Color SelectedColor { get; set; } = Color.FromArgb(80, 160, 255);

            public Color SelectedForeColor { get; set; } = Color.White;

            public Color FillColor { get; set; } = Color.White;

            public bool ShowLinesEx
            {
                get => showLines;
                set
                {
                    showLines = value;
                    Invalidate();
                }
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                var node = GetNodeAt(e.Location);
                if (node == null || CurrentNode == node) return;

                var g = CreateGraphics();
                if (CurrentNode != null)
                    OnDrawNode(new DrawTreeNodeEventArgs(g, CurrentNode,
                        new Rectangle(0, CurrentNode.Bounds.Y, Width, CurrentNode.Bounds.Height),
                        TreeNodeStates.Default));

                CurrentNode = node;
                OnDrawNode(new DrawTreeNodeEventArgs(g, CurrentNode,
                    new Rectangle(0, CurrentNode.Bounds.Y, Width, CurrentNode.Bounds.Height), TreeNodeStates.Hot));
                g.Dispose();
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                var g = CreateGraphics();
                if (CurrentNode != null)
                {
                    OnDrawNode(new DrawTreeNodeEventArgs(g, CurrentNode,
                        new Rectangle(0, CurrentNode.Bounds.Y, Width, CurrentNode.Bounds.Height),
                        TreeNodeStates.Default));
                    CurrentNode = null;
                }

                g.Dispose();
            }


            protected override void OnDrawNode(DrawTreeNodeEventArgs e)
            {
                base.OnDrawNode(e);

                if (e.Node == null || Nodes.Count == 0) return;

                try
                {
                    if (!DicNodeStatus.ContainsKey(e.Node.GetHashCode()))
                    {
                        DicNodeStatus.Add(e.Node.GetHashCode(), false);
                    }
                    if (CheckBoxes)
                    {
                        if (e.Node.Parent != null && DicNodeStatus.ContainsKey(e.Node.Parent.GetHashCode()) && !DicNodeStatus[e.Node.Parent.GetHashCode()])
                        {
                            SetParentNodeCheckedState(e.Node);
                        }
                    }
                    if (BorderStyle == BorderStyle.Fixed3D) BorderStyle = BorderStyle.FixedSingle;

                    if (e.Node == null || e.Node.Bounds.Width <= 0 && e.Node.Bounds.Height <= 0 && e.Node.Bounds.X <= 0 && e.Node.Bounds.Y <= 0)
                    {
                        e.DrawDefault = true;
                    }
                    else
                    {

                        var drawLeft = (e.Node.Level + 1) * Indent + 3;
                        var checkBoxLeft = (e.Node.Level + 1) * Indent + 1;
                        var imageLeft = drawLeft;
                        var haveImage = false;
                        var sf = e.Graphics.MeasureString(e.Node.Text, Font);

                        if (CheckBoxes)
                        {
                            drawLeft += 16;
                            imageLeft += 16;
                        }

                        if (ImageList != null && ImageList.Images.Count > 0 && e.Node.ImageIndex >= 0 &&
                            e.Node.ImageIndex < ImageList.Images.Count)
                        {
                            haveImage = true;
                            drawLeft += ImageList.ImageSize.Width + 6;
                        }

                        var checkboxColor = ForeColor;
                        if (e.Node != null)
                        {

                            if (e.Node == SelectedNode)
                            {
                                e.Graphics.FillRectangle((e.State & TreeNodeStates.Hot) != 0 ? HoverColor : SelectedColor,
                                    new Rectangle(new Point(0, e.Node.Bounds.Y), new Size(Width, e.Node.Bounds.Height)));

                                e.Graphics.DrawString(e.Node.Text, Font, SelectedForeColor, drawLeft,
                                    e.Bounds.Y + (ItemHeight - sf.Height) / 2.0f);

                                checkboxColor = SelectedForeColor;
                            }
                            else if (e.Node == CurrentNode && (e.State & TreeNodeStates.Hot) != 0)
                            {
                                e.Graphics.FillRectangle(HoverColor,
                                    new Rectangle(new Point(0, e.Node.Bounds.Y), new Size(Width, e.Node.Bounds.Height)));
                                e.Graphics.DrawString(e.Node.Text, Font, ForeColor, drawLeft,
                                    e.Bounds.Y + (ItemHeight - sf.Height) / 2.0f);
                            }
                            else
                            {
                                e.Graphics.FillRectangle(FillColor,
                                    new Rectangle(new Point(0, e.Node.Bounds.Y), new Size(Width, e.Node.Bounds.Height)));
                                e.Graphics.DrawString(e.Node.Text, Font, ForeColor, drawLeft,
                                    e.Bounds.Y + (ItemHeight - sf.Height) / 2.0f);
                            }

                            if (haveImage)
                            {
                                if (e.Node == SelectedNode && e.Node.SelectedImageIndex >= 0 &&
                                    e.Node.SelectedImageIndex < ImageList.Images.Count)
                                    e.Graphics.DrawImage(ImageList.Images[e.Node.SelectedImageIndex], imageLeft,
                                        e.Bounds.Y + (e.Bounds.Height - ImageList.ImageSize.Height) / 2);
                                else
                                    e.Graphics.DrawImage(ImageList.Images[e.Node.ImageIndex], imageLeft,
                                        e.Bounds.Y + (e.Bounds.Height - ImageList.ImageSize.Height) / 2);
                            }

                            if (CheckBoxes)
                            {


                                if (!e.Node.Checked)
                                {
                                    e.Graphics.DrawRectangle(checkboxColor,
                                        new Rectangle(checkBoxLeft + 2, e.Bounds.Y + (ItemHeight - 12) / 2 - 1, 12, 12));
                                }
                                else
                                {
                                    using (var pn = new Pen(checkboxColor, 2))
                                    {
                                        var pt1 = new Point(checkBoxLeft + 2 + 2, e.Bounds.Y + (ItemHeight - 12) / 2 - 1 + 5);
                                        var pt2 = new Point(pt1.X + 3, pt1.Y + 3);
                                        var pt3 = new Point(pt2.X + 5, pt2.Y - 5);

                                        PointF[] CheckMarkLine = { pt1, pt2, pt3 };

                                        e.Graphics.SetHighQuality();
                                        e.Graphics.DrawLines(pn, CheckMarkLine);
                                        e.Graphics.SetDefaultQuality();
                                        e.Graphics.DrawRectangle(checkboxColor,
                                            new Rectangle(checkBoxLeft + 2, e.Bounds.Y + (ItemHeight - 12) / 2 - 1, 12, 12));
                                    }
                                }

                                if (DicNodeStatus[e.Node.GetHashCode()])
                                {
                                    //var location = e.Node.Bounds.Location;
                                    //location.Offset(-29, 10);
                                    var location = new Point(checkBoxLeft + 5, e.Bounds.Y + (ItemHeight - 12) / 2 + 2);
                                    var size = new Size(7, 7);
                                    e.Graphics.FillRectangle(checkboxColor, new Rectangle(location, size)); //这里绘制的是正方形

                                }
                            }
                        }

                        var lineY = e.Bounds.Y + e.Node.Bounds.Height / 2 - 1;
                        var lineX = 3 + e.Node.Level * Indent + 9;

                        if (ShowLinesEx)
                        {
                            try
                            {
                                //绘制虚线
                                var pn = new Pen(LineColor);
                                pn.DashStyle = DashStyle.Dot;
                                e.Graphics.DrawLine(pn, lineX, lineY, lineX + 10, lineY);

                                if (e.Node.Level >= 1)
                                {
                                    e.Graphics.DrawLine(pn, lineX, lineY, lineX, e.Bounds.Top);
                                    if (e.Node.NextNode != null)
                                        e.Graphics.DrawLine(pn, lineX, lineY, lineX, e.Node.Bounds.Bottom);

                                    var pNode = e.Node.Parent;
                                    while (pNode != null)
                                    {
                                        lineX -= Indent;

                                        if (Nodes.Count > 0)
                                        {
                                            if (pNode.NextNode != null)
                                                e.Graphics.DrawLine(pn, lineX, lineY, lineX, e.Node.Bounds.Top);

                                            if (pNode.NextNode != null)
                                                e.Graphics.DrawLine(pn, lineX, lineY, lineX, e.Node.Bounds.Bottom);
                                        }

                                        pNode = pNode.Parent;
                                    }
                                }
                                else
                                {
                                    if (e.Node != null && Nodes.Count > 0)
                                    {
                                        if (e.Node.PrevNode != null)
                                            e.Graphics.DrawLine(pn, lineX, lineY, lineX, e.Node.Bounds.Top);

                                        if (e.Node.NextNode != null)
                                            e.Graphics.DrawLine(pn, lineX, lineY, lineX, e.Node.Bounds.Bottom);
                                    }
                                }

                                pn.Dispose();
                            }
                            catch (Exception exception)
                            {
                                Console.WriteLine(exception);
                            }
                        }

                        lineX = 3 + e.Node.Level * Indent + 9;
                        //绘制左侧+号
                        if (ShowPlusMinus && e.Node.Nodes.Count > 0)
                        {
                            e.Graphics.FillRectangle(Color.White, new Rectangle(lineX - 4, lineY - 4, 8, 8));
                            e.Graphics.DrawRectangle(UIFontColor.Primary, new Rectangle(lineX - 4, lineY - 4, 8, 8));
                            e.Graphics.DrawLine(UIFontColor.Primary, lineX - 2, lineY, lineX + 2, lineY);
                            if (!e.Node.IsExpanded)
                                e.Graphics.DrawLine(UIFontColor.Primary, lineX, lineY - 2, lineX, lineY + 2);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            public bool TreeNodeSelected(DrawTreeNodeEventArgs e)
            {
                return e.State == TreeNodeStates.Selected || e.State == TreeNodeStates.Focused ||
                       e.State == (TreeNodeStates.Focused | TreeNodeStates.Selected);
            }

            protected override void WndProc(ref Message m)
            {
                if (IsDisposed || Disposing) return;
                if (m.Msg == Win32.User.WM_ERASEBKGND)
                {
                    m.Result = IntPtr.Zero;
                    return;
                }

                base.WndProc(ref m);
            }

            private Dictionary<int, bool> DicNodeStatus = new Dictionary<int, bool>();

            protected override void OnAfterCheck(TreeViewEventArgs e)
            {

                base.OnAfterCheck(e);
                if (e.Action == TreeViewAction.ByMouse) //鼠标点击
                {
                    DicNodeStatus[e.Node.GetHashCode()] = false;

                    SetChildNodeCheckedState(e.Node, e.Node.Checked);

                    SetParentNodeCheckedState(e.Node, true);

                }

            }

            private void SetParentNodeCheckedState(TreeNode currNode, bool ByMouse = false)
            {

                if (currNode.Parent == null)
                    return;
                TreeNode parentNode = currNode.Parent; //获得当前节点的父节点

                var count = parentNode.Nodes.Cast<TreeNode>().Where(n => n.Checked).ToList().Count;

                //判断节点Checked是否改变，只有改变时才赋值，否则不变更，以防止频繁触发OnAfterCheck事件
                bool bChecked = count == parentNode.Nodes.Count;
                if (parentNode.Checked != bChecked)
                {
                    parentNode.Checked = bChecked;
                }

                var half = parentNode.Nodes.Cast<TreeNode>().Where(n => (DicNodeStatus.ContainsKey(n.GetHashCode()) ? DicNodeStatus[n.GetHashCode()] : false)).ToList().Count;

                if ((count > 0 && count < parentNode.Nodes.Count) || half > 0)
                {
                    DicNodeStatus[parentNode.GetHashCode()] = true;
                }
                else
                {
                    DicNodeStatus[parentNode.GetHashCode()] = false;
                }

                if (ByMouse)
                {
                    var g = CreateGraphics();
                    OnDrawNode(new DrawTreeNodeEventArgs(g, parentNode,
                        new Rectangle(0, parentNode.Bounds.Y, Width, parentNode.Bounds.Height), TreeNodeStates.Hot));
                    g.Dispose();

                    if (parentNode.Parent != null) //如果父节点之上还有父节点
                    {
                        SetParentNodeCheckedState(parentNode, true); //递归调用
                    }
                }

            }


            //选中节点之后，选中节点的所有子节点
            private void SetChildNodeCheckedState(TreeNode currNode, bool state)
            {
                TreeNodeCollection nodes = currNode.Nodes; //获取所有子节点
                if (nodes.Count > 0) //存在子节点
                {
                    foreach (TreeNode tn in nodes)
                    {
                        DicNodeStatus[tn.GetHashCode()] = false;
                        tn.Checked = state;
                        SetChildNodeCheckedState(tn, state);//递归调用子节点的子节点
                    }
                }
            }


        }
    }
}