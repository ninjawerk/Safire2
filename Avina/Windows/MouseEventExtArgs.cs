using System.Windows.Forms;

namespace Kornea.Windows
{
    /// <summary>
    /// Provides data for the MouseClickExt and MouseMoveExt events. It also provides a property Handled.
    /// Set this property to <b>true</b> to prevent further processing of the event in other applications.
    /// </summary>
    public class MouseEventExtArgs : MouseEventArgs
    {
         public MouseEventExtArgs(MouseButtons buttons, int clicks, int x, int y, int delta)
            : base(buttons, clicks, x, y, delta)
        {}

         internal MouseEventExtArgs(MouseEventArgs e) : base(e.Button, e.Clicks, e.X, e.Y, e.Delta)
        {}

        private bool m_Handled;

         public bool Handled
        {
            get { return m_Handled; }
            set { m_Handled = value; }
        }
    }
}
