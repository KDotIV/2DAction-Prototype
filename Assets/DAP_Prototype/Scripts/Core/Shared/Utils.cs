using System.Collections;

namespace RPG.Core.Shared
{
    public static class Utils
    {
        /// <summary>
        /// Makes the wrapped IEnumerator call us back on exception or on completion.
        /// See https://web.archive.org/web/20170818040958/https://jacksondunstan.com/articles/3718
        /// for explanation.
        /// </summary>
        /// <param name="enumerator">The enumerator to wrap</param>
        /// <param name="onDone">This will be called with a System.Exception or with null if completed successfully</param>
        /// <returns></returns>
        public static IEnumerator ThrowingEnumerator(
            IEnumerator enumerator,
            System.Action<System.Exception, object> onDone
        )
        {
            object _current = null;
            while (true)
            {
                try
                {
                    if (enumerator.MoveNext() == false)
                    {
                        break;
                    }
                    _current = enumerator.Current;
                }
                catch (System.Exception ex)
                {
                    onDone(ex, null);
                    yield break;
                }
                yield return _current;
            }
            onDone(null, _current);
        }
    }
}