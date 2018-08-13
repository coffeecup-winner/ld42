using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class FigureFactory {
    private static readonly string TemplateString = @"
        x
        -----
        x
        -----
        xx
        -----
        x
        -----
        xxx
        x x
        xxx
        -----
        xxxxx
        x   x
        xxxxx
            x
        xxxxx
        -----
        xxxxx
        x   x
        xxxxx
        x   x
        xxxxx
        -----
        xxxxx
            x
            x
            x
            x
        -----
        xxxxx
        x
        xxxxx
        x   x
        xxxxx
        -----
        xxxxx
        x
        xxxxx
            x
        xxxxx
        -----
        x   x
        x   x
        xxxxx
            x
            x
        -----
        xxxxx
            x
        xxxxx
            x
        xxxxx
        -----
        xxxxx
            x
        xxxxx
        x
        xxxxx
        -----
            x
          xxx
            x
            x
            x
        -----
        xxxxx
        x x x
        xx xx
        x x x
        xxxxx
        -----";

    private static readonly List<bool[,]> templates = new List<bool[,]>();

    static FigureFactory() {
        var rows = new List<string>();
        foreach (string line in TemplateString.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)) {
            string row = line.Substring(8);
            if (row != "-----") {
                rows.Add(row);
                continue;
            }

            if (rows.Count == 0) {
                continue;
            }

            int width = rows.Max(r => r.Length);
            int height = rows.Count;

            var template = new bool[width, height];
            for (int y = 0; y < rows.Count; y++) {
                for (int x = 0; x < rows[y].Length; x++) {
                    if (rows[y][x] == 'x') {
                        template[x, height - y - 1] = true;
                    }
                }
            }
            templates.Add(template);
            rows.Clear();
        }
    }

    public static int TemplatesCount { get { return templates.Count; } }

    public static bool[,] GetTemplate(int i) {
        return templates[i];
    }
}
