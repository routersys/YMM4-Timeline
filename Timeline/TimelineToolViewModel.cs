using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.ViewModels;

namespace Timeline
{
    internal class TimelineToolViewModel : Bindable, IToolViewModel, ITimelineToolViewModel, IDisposable
    {
        public event EventHandler<CreateNewToolViewRequestedEventArgs>? CreateNewToolViewRequested;

        public string Title => Texts.TimelineToolName;

        public TimelineViewModel? TimelineViewModel { get => field; private set => Set(ref field, value); }

        public void SetTimelineToolInfo(TimelineToolInfo info)
        {
            DisposeTimelineViewModel();

            var scene = info.Scenes.AllScenes.FirstOrDefault(s => s.Timeline == info.Timeline);
            if (scene is not null)
            {
                TimelineViewModel = new TimelineViewModel(scene, info.UndoRedoManager, info.AsyncAwaitStatus);
            }
        }

        public void LoadState(ToolState stateData)
        {
        }

        public ToolState SaveState()
        {
            return new ToolState
            {
                Title = Title
            };
        }

        void DisposeTimelineViewModel()
        {
            TimelineViewModel?.Dispose();
            TimelineViewModel = null;
        }

        public void Dispose()
        {
            DisposeTimelineViewModel();
        }
    }
}
