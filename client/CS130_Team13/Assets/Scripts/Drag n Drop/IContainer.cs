using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obselete {
    public interface IContainer {
        bool IsCanvas();
        Vector2 GetMousePosInMe();
    }
}