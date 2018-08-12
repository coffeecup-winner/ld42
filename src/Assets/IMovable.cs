using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IMovable {
    IEnumerable<Vector2> EnumerateAllFilledBlocks();
    void GetAllowedMoves(out bool left, out bool top, out bool right, out bool bottom);
}
