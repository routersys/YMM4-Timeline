using YukkuriMovieMaker.Plugin;

namespace Timeline
{
    internal class TimelineToolPlugin : IToolPlugin
    {
        public Type ViewModelType => typeof(TimelineToolViewModel);

        public Type ViewType => typeof(TimelineToolView);

        public string Name => Texts.TimelineToolName;

        public bool AllowMultipleInstances => false;
    }
}
