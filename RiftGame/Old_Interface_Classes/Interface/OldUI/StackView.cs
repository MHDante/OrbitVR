using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;


namespace OrbItProcs
{
    public class StackView
    {
        public List<CollapsePanel> CollapsePanels { get; private set; } //does this mean you can still add to the list from the get?

        public StackView ()
        {
            CollapsePanels = new List<CollapsePanel>();
        }

        public void AddPanel(CollapsePanel collapsePanel)
        {
            if (CollapsePanels.Contains(collapsePanel)) return;

            CollapsePanels.Add(collapsePanel);

            collapsePanel.collapseButton.Click += RefreshStackView;
            collapsePanel.ExpandedHeightChanged += Refresh;

            Refresh();
        }

        void RefreshStackView(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            Refresh();
        }

        public void Refresh()
        {
            if (CollapsePanels.Count == 0) return;

            //int top = CollapsePanels.ElementAt(0).Top;
            //int left = CollapsePanels.ElementAt(0).Left;
            int top = 0;
            int left = 0;


            foreach (CollapsePanel c in CollapsePanels)
            {
                Margins m = c.panel.Margins;

                //if (orientation == Orientation.Vertical)
                //{
                top += m.Top;
                c.Top = top;
                top += c.Height;
                top += m.Bottom;
                c.Left = left;

                top -= 10;
                //}
                /*
                if (orientation == Orientation.Horizontal)
                {
                    left += m.Left;
                    c.Left = left;
                    left += c.Width;
                    left += m.Right;
                    c.Top = top;
                }
                */
            }
        }

        public void MovePanel(int fromIndex, int toIndex)
        {
            CollapsePanel swap = CollapsePanels.ElementAt(fromIndex);
            CollapsePanels.Remove(swap);
            CollapsePanels.Insert(toIndex, swap);
            Refresh();
        }
    }
}
