using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Json;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.ViewModels;

namespace Timeline
{
    public class TimelineState
    {
        public int Id { get; set; }
    }

    internal class TimelineToolViewModel : Bindable, IToolViewModel, ITimelineToolViewModel, IDisposable
    {
        static readonly HashSet<int> usedIds = new();

        public event EventHandler<CreateNewToolViewRequestedEventArgs>? CreateNewToolViewRequested;

        YukkuriMovieMaker.Project.Timeline? currentTimeline;

        int id;
        public int Id
        {
            get => id;
            private set
            {
                if (id == value) return;
                if (id > 0) usedIds.Remove(id);
                id = value;
                if (id > 0) usedIds.Add(id);
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Title => Id <= 1 ? Texts.TimelineToolName : $"{Texts.TimelineToolName} {Id}";

        public TimelineViewModel? TimelineViewModel { get => field; private set => Set(ref field, value); }

        public bool IsAddOverlayVisible { get => field; set => Set(ref field, value); }

        public ICommand ShowAddOverlayCommand { get; }
        public ICommand HideAddOverlayCommand { get; }
        public ICommand AddTimelineCommand { get; }

        public TimelineToolViewModel()
        {
            Id = GetLowestAvailableId();

            ShowAddOverlayCommand = new ActionCommand(_ => true, _ => IsAddOverlayVisible = true);
            HideAddOverlayCommand = new ActionCommand(_ => true, _ => IsAddOverlayVisible = false);
            AddTimelineCommand = new ActionCommand(_ => true, _ =>
            {
                IsAddOverlayVisible = false;
                var nextId = GetLowestAvailableId();
                var state = new TimelineState { Id = nextId };
                var toolState = new ToolState
                {
                    Title = nextId <= 1 ? Texts.TimelineToolName : $"{Texts.TimelineToolName} {nextId}",
                    SavedState = Json.GetJsonText(state)
                };
                CreateNewToolViewRequested?.Invoke(this, new CreateNewToolViewRequestedEventArgs(toolState));
            });
        }

        static int GetLowestAvailableId()
        {
            int i = 1;
            while (usedIds.Contains(i)) i++;
            return i;
        }

        public void SetTimelineToolInfo(TimelineToolInfo info)
        {
            DisposeTimelineViewModel();

            var scene = info.Scenes.AllScenes.FirstOrDefault(s => s.Timeline == info.Timeline);
            if (scene is not null)
            {
                TimelineViewModel = new TimelineViewModel(scene, info.UndoRedoManager, info.AsyncAwaitStatus);
                currentTimeline = info.Timeline;
                currentTimeline.PropertyChanged += Timeline_PropertyChanged;
            }
        }

        public void LoadState(ToolState stateData)
        {
            if (stateData.SavedState is null)
                return;
            var state = Json.LoadFromText<TimelineState>(stateData.SavedState);
            if (state is null)
                return;

            if (!usedIds.Contains(state.Id))
            {
                Id = state.Id;
            }
        }

        public ToolState SaveState()
        {
            return new ToolState
            {
                Title = Title,
                SavedState = Json.GetJsonText(new TimelineState { Id = Id })
            };
        }

        void DisposeTimelineViewModel()
        {
            if (currentTimeline is not null)
            {
                currentTimeline.PropertyChanged -= Timeline_PropertyChanged;
                currentTimeline = null;
            }
            TimelineViewModel?.Dispose();
            TimelineViewModel = null;
        }

        void Timeline_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(YukkuriMovieMaker.Project.Timeline.CurrentFrame))
            {
                if (TimelineViewModel is not null && currentTimeline is not null)
                {
                    if (!TimelineViewModel.ContainFrameInViewport(currentTimeline.CurrentFrame))
                    {
                        TimelineViewModel.ScrollFrame(currentTimeline.CurrentFrame);
                    }
                }
            }
        }

        public void Dispose()
        {
            if (id > 0)
            {
                usedIds.Remove(id);
                id = 0;
            }
            DisposeTimelineViewModel();
        }
    }
}
