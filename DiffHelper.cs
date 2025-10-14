using System;
using System.Collections.Generic;
using System.Linq;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;

namespace ComparisonUtil
{
    public class DiffHelper
    {
        public static DiffResult CompareTexts(string leftText, string rightText)
        {
            var diffBuilder = new SideBySideDiffBuilder(new Differ());
            var diff = diffBuilder.BuildDiffModel(leftText, rightText);
            
            return new DiffResult
            {
                LeftLines = ParseLines(diff.OldText),
                RightLines = ParseLines(diff.NewText)
            };
        }

        private static List<DiffLine> ParseLines(DiffPaneModel paneModel)
        {
            var result = new List<DiffLine>();

            foreach (var diffLine in paneModel.Lines)
            {
                var line = new DiffLine
                {
                    Text = diffLine.Text ?? string.Empty,
                    Type = ConvertChangeType(diffLine.Type),
                    SubPieces = new List<DiffPiece>()
                };

                // 如果这行被修改了，提取字符级别的差异
                if (diffLine.Type == ChangeType.Modified && diffLine.SubPieces != null && diffLine.SubPieces.Count > 0)
                {
                    foreach (var piece in diffLine.SubPieces)
                    {
                        line.SubPieces.Add(new DiffPiece
                        {
                            Text = piece.Text ?? string.Empty,
                            Type = ConvertChangeType(piece.Type),
                            Position = piece.Position ?? 0
                        });
                    }
                }
                else
                {
                    // 整行作为一个片段
                    line.SubPieces.Add(new DiffPiece
                    {
                        Text = line.Text,
                        Type = line.Type,
                        Position = 0
                    });
                }

                result.Add(line);
            }

            return result;
        }

        private static DiffLineType ConvertChangeType(ChangeType changeType)
        {
            return changeType switch
            {
                ChangeType.Unchanged => DiffLineType.Unchanged,
                ChangeType.Deleted => DiffLineType.Deleted,
                ChangeType.Inserted => DiffLineType.Inserted,
                ChangeType.Modified => DiffLineType.Modified,
                ChangeType.Imaginary => DiffLineType.Imaginary,
                _ => DiffLineType.Unchanged
            };
        }
    }

    public class DiffResult
    {
        public List<DiffLine> LeftLines { get; set; } = new List<DiffLine>();
        public List<DiffLine> RightLines { get; set; } = new List<DiffLine>();
    }

    public class DiffLine
    {
        public string Text { get; set; } = string.Empty;
        public DiffLineType Type { get; set; }
        public List<DiffPiece> SubPieces { get; set; } = new List<DiffPiece>();
    }

    public class DiffPiece
    {
        public string Text { get; set; } = string.Empty;
        public DiffLineType Type { get; set; }
        public int Position { get; set; }
    }

    public enum DiffLineType
    {
        Unchanged,
        Inserted,
        Deleted,
        Modified,
        Imaginary
    }
}
