namespace Temporizador.Extensions
{
    public static class AnimationExtensions
    {
        public static Task<bool> WidthRequestToAsync(this VisualElement view, double newWidth, uint length = 250, Easing? easing = null)
        {
            var tcs = new TaskCompletionSource<bool>();
            double start = view.WidthRequest;
            double diff = newWidth - start;

            view.Animate(
                "WidthRequestAnim",
                x => view.WidthRequest = start + diff * x,
                0, 1,
                length: length,
                easing: easing ?? Easing.Linear,
                finished: (v, c) => tcs.SetResult(true)
            );

            return tcs.Task;
        }
    }
}
