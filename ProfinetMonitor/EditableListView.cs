using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProfinetMonitor
{
    class EditableListView : System.Windows.Forms.ListView
    {
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            SetUpEditingTextBox();
        }

        #region ScrollEvents
        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;
        public event EventHandler Scroll;

        protected void OnScroll()
        {
            if (this.Scroll != null)
                this.Scroll(this, EventArgs.Empty);
            CancelAndHideTexEditor();
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_HSCROLL || m.Msg == WM_VSCROLL)
                this.OnScroll();
        }
        #endregion

        #region SubItemEditing
        public event EventHandler<SubItemLabelEditEventArgs>BeforeSubItemLabelEdit;
        public event EventHandler<SubItemLabelEditEventArgs>AfterSubItemLabelEdit;
        ListViewItem SelectedLI;
        ListViewItem.ListViewSubItem SelectedLSI;
        TextBox TxtEdit;

        //Double click events are only fired for the Items label, never for the Sub Items.
        //so we have to emulate an Double click event
        Timer DoubleClickTimer;
        int clickCount = 0;

        protected override void OnMouseUp(MouseEventArgs e)
        {
            clickCount++;
            DoubleClickTimer.Stop();
            DoubleClickTimer.Start();
            if (clickCount >=2) ShowTextEditor(e.X, e.Y);
        }

        private void ShowTextEditor(int X, int Y)
        {
            if (this.LabelEdit == false) return;

            ListViewHitTestInfo i = this.HitTest(X, Y);
            SelectedLI = i.Item;
            SelectedLSI = i.SubItem;
            if (SelectedLSI == null)
                return;

            int CellWidth = SelectedLSI.Bounds.Width;
            int CellHeight = SelectedLSI.Bounds.Height;
            int CellLeft = i.SubItem.Bounds.Left;
            int CellTop = i.SubItem.Bounds.Top;

            // First Column
            if (i.SubItem == i.Item.SubItems[0])
                CellWidth = this.Columns[0].Width;

            TxtEdit.Location = new Point(CellLeft, CellTop);
            TxtEdit.Size = new Size(CellWidth, CellHeight);
            TxtEdit.Visible = true;
            TxtEdit.BringToFront();
            TxtEdit.Text = i.SubItem.Text;
            TxtEdit.Select();
            TxtEdit.SelectAll();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            AcceptAndHideTexEditor();
        }
        
        private void SetUpEditingTextBox()
        {
            TxtEdit = new TextBox();
            this.Controls.Add(TxtEdit);
            TxtEdit.Visible = false;
            TxtEdit.Leave += TxtEdit_Leave;
            TxtEdit.KeyUp += TxtEdit_KeyUp;

            DoubleClickTimer = new Timer();
            DoubleClickTimer.Interval = SystemInformation.DoubleClickTime;
            DoubleClickTimer.Tick += (a, b) => { clickCount = 0; DoubleClickTimer.Stop();};
        }

        private void TxtEdit_Leave(object sender, EventArgs e)
        {
            AcceptAndHideTexEditor();
        }

        private void TxtEdit_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) AcceptAndHideTexEditor();
            if (e.KeyCode == Keys.Return) AcceptAndHideTexEditor();
            if (e.KeyCode == Keys.Escape) CancelAndHideTexEditor();
        }

        private void AcceptAndHideTexEditor()
        {
            TxtEdit.Visible = false;
            if (SelectedLSI != null)
            {
                if (BeforeSubItemLabelEdit != null)
                {
                    var args = new SubItemLabelEditEventArgs(SelectedLI.Index, SelectedLI.SubItems.IndexOf(SelectedLSI), TxtEdit.Text);
                    BeforeSubItemLabelEdit.Invoke(this, args);
                    if (args.CancelEdit)
                    {
                        CancelAndHideTexEditor();
                        return;
                    }
                }
                SelectedLSI.Text = TxtEdit.Text;

                if (AfterSubItemLabelEdit != null)
                {
                    var args = new SubItemLabelEditEventArgs(SelectedLI.Index, SelectedLI.SubItems.IndexOf(SelectedLSI), TxtEdit.Text);
                    AfterSubItemLabelEdit.Invoke(this, args);
                }

            }
            SelectedLI = null;
            SelectedLSI = null;
            TxtEdit.Text = "";
        }

        private void CancelAndHideTexEditor()
        {
            SelectedLI = null;
            SelectedLSI = null;
            TxtEdit.Text = "";
            TxtEdit.Visible = false;
        }

    }

    public class SubItemLabelEditEventArgs : EventArgs
    {
        //
        // Summary:
        //     Initializes a new instance of the System.Windows.Forms.LabelEditEventArgs class
        //     with the specified index to the System.Windows.Forms.ListViewItem to edit.
        //
        // Parameters:
        //   item:
        //     The zero-based index of the System.Windows.Forms.ListViewItem, containing the
        //     label to edit.
        public SubItemLabelEditEventArgs(int item, int subItem)
        {
            Item = item;
            SubItem = subItem;
        }
        //
        // Summary:
        //     Initializes a new instance of the System.Windows.Forms.LabelEditEventArgs class
        //     with the specified index to the System.Windows.Forms.ListViewItem being edited
        //     and the new text for the label of the System.Windows.Forms.ListViewItem.
        //
        // Parameters:
        //   item:
        //     The zero-based index of the System.Windows.Forms.ListViewItem, containing the
        //     label to edit.
        //
        //   label:
        //     The new text assigned to the label of the System.Windows.Forms.ListViewItem.
        public SubItemLabelEditEventArgs(int item, int subItem, string label)
        {
            Item = item;
            SubItem = subItem;
            Label = label;
        }

        //
        // Summary:
        //     Gets the new text assigned to the label of the System.Windows.Forms.ListViewItem.
        //
        // Returns:
        //     The new text to associate with the System.Windows.Forms.ListViewItem or null
        //     if the text is unchanged.
        public string Label { get; }
        //
        // Summary:
        //     Gets the zero-based index of the System.Windows.Forms.ListViewItem containing
        //     the label to edit.
        //
        // Returns:
        //     The zero-based index of the System.Windows.Forms.ListViewItem.
        public int Item { get; }
        public int SubItem { get; }

        //
        // Summary:
        //     Gets or sets a value indicating whether changes made to the label of the System.Windows.Forms.ListViewItem
        //     should be canceled.
        //
        // Returns:
        //     true if the edit operation of the label for the System.Windows.Forms.ListViewItem
        //     should be canceled; otherwise false.
        public bool CancelEdit { get; set; }
    }
    #endregion
}
