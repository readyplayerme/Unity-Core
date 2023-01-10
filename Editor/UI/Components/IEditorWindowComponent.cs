using UnityEngine;

namespace ReadyPlayerMe.Core.Editor
{
    public interface IEditorWindowComponent
    {
        public string EditorWindowName { get; set; }
        
        void Draw(Rect position);
    }
}