using System.Collections;

namespace Model
{
    public interface IActionCommand
    {
        IEnumerator Execute();
    }
}