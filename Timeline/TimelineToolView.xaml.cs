using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Timeline
{
    public partial class TimelineToolView : UserControl
    {
        public TimelineToolView()
        {
            InitializeComponent();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (DataContext is TimelineToolViewModel vm)
            {
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                {
                    if (e.GetPosition(this).Y <= 10)
                    {
                        if (vm.ShowAddOverlayCommand.CanExecute(null))
                        {
                            vm.ShowAddOverlayCommand.Execute(null);
                        }
                    }
                }
            }
        }
    }
}
