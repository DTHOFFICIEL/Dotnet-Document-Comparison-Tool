using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Win32;

namespace ComparisonUtil
{
    public partial class MainWindow : Window
    {
        private string leftText = string.Empty;
        private string rightText = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            // 默认选择智能高亮模式
            HighlightModeComboBox.SelectedIndex = 0;
        }

        private void LoadLeftButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "选择左侧文档 - 选择文件格式以自动提取纯文本",
                Filter = "纯文本 (*.txt)|*.txt|" +
                        "Markdown 文件 (*.md)|*.md|" +
                        "Fandom WikiText (*.wiki;*.wikitext;*.txt)|*.wiki;*.wikitext;*.txt|" +
                        "所有文件 (*.*)|*.*",
                FilterIndex = 1
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string rawText = File.ReadAllText(openFileDialog.FileName);
                    
                    // 根据用户选择的过滤器索引确定文件格式
                    FileFormatType formatType = openFileDialog.FilterIndex switch
                    {
                        2 => FileFormatType.Markdown,
                        3 => FileFormatType.WikiText,
                        _ => FileFormatType.PlainText
                    };

                    // 处理文本
                    leftText = TextProcessor.ProcessText(rawText, formatType);
                    
                    LeftTextBox.Document.Blocks.Clear();
                    LeftTextBox.Document.Blocks.Add(new Paragraph(new Run(leftText)));
                    
                    string formatInfo = formatType == FileFormatType.PlainText 
                        ? "（原始文本）" 
                        : $"（已提取纯文本 - {formatType}）";
                    
                    MessageBox.Show($"成功加载左侧文档: {Path.GetFileName(openFileDialog.FileName)}\n{formatInfo}", 
                                  "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"加载文件失败: {ex.Message}", 
                                  "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadRightButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "选择右侧文档 - 选择文件格式以自动提取纯文本",
                Filter = "纯文本 (*.txt)|*.txt|" +
                        "Markdown 文件 (*.md)|*.md|" +
                        "Fandom WikiText (*.wiki;*.wikitext;*.txt)|*.wiki;*.wikitext;*.txt|" +
                        "所有文件 (*.*)|*.*",
                FilterIndex = 1
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string rawText = File.ReadAllText(openFileDialog.FileName);
                    
                    // 根据用户选择的过滤器索引确定文件格式
                    FileFormatType formatType = openFileDialog.FilterIndex switch
                    {
                        2 => FileFormatType.Markdown,
                        3 => FileFormatType.WikiText,
                        _ => FileFormatType.PlainText
                    };

                    // 处理文本
                    rightText = TextProcessor.ProcessText(rawText, formatType);
                    
                    RightTextBox.Document.Blocks.Clear();
                    RightTextBox.Document.Blocks.Add(new Paragraph(new Run(rightText)));
                    
                    string formatInfo = formatType == FileFormatType.PlainText 
                        ? "（原始文本）" 
                        : $"（已提取纯文本 - {formatType}）";
                    
                    MessageBox.Show($"成功加载右侧文档: {Path.GetFileName(openFileDialog.FileName)}\n{formatInfo}", 
                                  "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"加载文件失败: {ex.Message}", 
                                  "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CompareButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 从RichTextBox获取当前文本
                leftText = new TextRange(LeftTextBox.Document.ContentStart, 
                                        LeftTextBox.Document.ContentEnd).Text;
                rightText = new TextRange(RightTextBox.Document.ContentStart, 
                                         RightTextBox.Document.ContentEnd).Text;

                if (string.IsNullOrWhiteSpace(leftText) && string.IsNullOrWhiteSpace(rightText))
                {
                    MessageBox.Show("请先加载或输入要对比的文档！", 
                                  "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 如果选择忽略标点符号，进行预处理
                string processedLeftText = leftText;
                string processedRightText = rightText;
                
                if (IgnorePunctuationCheckBox.IsChecked == true)
                {
                    processedLeftText = NormalizePunctuation(leftText);
                    processedRightText = NormalizePunctuation(rightText);
                }

                // 执行对比
                var diffResult = DiffHelper.CompareTexts(processedLeftText, processedRightText);

                // 获取高亮模式
                int highlightMode = HighlightModeComboBox.SelectedIndex;

                // 显示对比结果
                DisplayDiffResult(LeftTextBox, diffResult.LeftLines, true, highlightMode);
                DisplayDiffResult(RightTextBox, diffResult.RightLines, false, highlightMode);

                string modeDescription = HighlightModeComboBox.SelectedIndex switch
                {
                    0 => "🎯 智能高亮模式\n相同内容使用浅色背景，修改部分精确高亮",
                    1 => "📝 整行高亮模式\n整行标记为相同/不同，简洁清晰",
                    2 => "✏️ 字符级高亮模式\n精确到每个字符的差异",
                    _ => ""
                };

                string punctuationInfo = IgnorePunctuationCheckBox.IsChecked == true 
                    ? "\n⚙️ 已忽略标点符号差异" 
                    : "";

                MessageBox.Show($"对比完成！{punctuationInfo}\n\n{modeDescription}\n\n" +
                              "📊 颜色说明：\n" +
                              "✅ 绿色背景 = 相同内容\n" +
                              "❌ 红色背景/文字 = 删除的内容\n" +
                              "➕ 蓝色背景/文字 = 插入的内容\n" +
                              "⚠️ 黄色背景 = 修改的内容\n" +
                              "⚪ 灰色背景 = 空行（对齐用）", 
                              "对比完成", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"对比过程中出错: {ex.Message}", 
                              "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 标准化标点符号：将中文标点转换为英文标点
        /// </summary>
        private string NormalizePunctuation(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return text
                .Replace("，", ",")
                .Replace("。", ".")
                .Replace("；", ";")
                .Replace("：", ":")
                .Replace("？", "?")
                .Replace("！", "!")
                .Replace("\u2018", "'")  // ' 左单引号
                .Replace("\u2019", "'")  // ' 右单引号
                .Replace("\u201C", "\"") // " 左双引号
                .Replace("\u201D", "\"") // " 右双引号
                .Replace("（", "(")
                .Replace("）", ")")
                .Replace("【", "[")
                .Replace("】", "]")
                .Replace("《", "<")
                .Replace("》", ">")
                .Replace("、", ",")
                .Replace("—", "-")
                .Replace("…", ".");
        }

        private void DisplayDiffResult(System.Windows.Controls.RichTextBox textBox, 
                                      System.Collections.Generic.List<DiffLine> lines, 
                                      bool isLeftPane,
                                      int highlightMode)
        {
            textBox.Document.Blocks.Clear();
            var paragraph = new Paragraph();
            paragraph.Margin = new Thickness(0);
            paragraph.LineHeight = 1;

            foreach (var line in lines)
            {
                switch (highlightMode)
                {
                    case 0: // 智能高亮
                        DisplaySmartHighlight(paragraph, line);
                        break;
                    case 1: // 整行高亮
                        DisplayLineHighlight(paragraph, line);
                        break;
                    case 2: // 字符级高亮
                        DisplayCharacterHighlight(paragraph, line);
                        break;
                }
            }

            textBox.Document.Blocks.Add(paragraph);
        }

        /// <summary>
        /// 智能高亮模式：相同内容浅色背景，修改部分明显标记
        /// </summary>
        private void DisplaySmartHighlight(Paragraph paragraph, DiffLine line)
        {
            if (line.Type == DiffLineType.Modified && line.SubPieces != null && line.SubPieces.Count > 1)
            {
                // 修改的行：给整行一个浅黄色背景，不同的字符特殊标记
                foreach (var piece in line.SubPieces)
                {
                    if (string.IsNullOrEmpty(piece.Text))
                        continue;

                    var run = new Run(piece.Text);
                    
                    if (piece.Type == DiffLineType.Unchanged)
                    {
                        // 相同字符：浅灰背景
                        run.Background = new SolidColorBrush(Color.FromRgb(250, 250, 235));
                        run.Foreground = Brushes.Black;
                        run.FontWeight = FontWeights.Normal;
                    }
                    else if (piece.Type == DiffLineType.Deleted)
                    {
                        // 删除的字符：深红色文字 + 删除线
                        run.Background = new SolidColorBrush(Color.FromRgb(255, 220, 220));
                        run.Foreground = new SolidColorBrush(Color.FromRgb(200, 0, 0));
                        run.TextDecorations = TextDecorations.Strikethrough;
                        run.FontWeight = FontWeights.Bold;
                    }
                    else if (piece.Type == DiffLineType.Inserted)
                    {
                        // 插入的字符：深蓝色文字 + 下划线
                        run.Background = new SolidColorBrush(Color.FromRgb(220, 235, 255));
                        run.Foreground = new SolidColorBrush(Color.FromRgb(0, 80, 180));
                        run.TextDecorations = TextDecorations.Underline;
                        run.FontWeight = FontWeights.Bold;
                    }
                    else
                    {
                        // 修改的字符：橙色文字
                        run.Background = new SolidColorBrush(Color.FromRgb(255, 240, 200));
                        run.Foreground = new SolidColorBrush(Color.FromRgb(200, 100, 0));
                        run.FontWeight = FontWeights.Bold;
                    }
                    
                    paragraph.Inlines.Add(run);
                }
                paragraph.Inlines.Add(new Run(Environment.NewLine));
            }
            else
            {
                // 整行相同或整行删除/插入
                var run = new Run(line.Text + Environment.NewLine);
                ApplySmartLineStyle(run, line.Type);
                paragraph.Inlines.Add(run);
            }
        }

        /// <summary>
        /// 整行高亮模式：简洁清晰
        /// </summary>
        private void DisplayLineHighlight(Paragraph paragraph, DiffLine line)
        {
            var run = new Run(line.Text + Environment.NewLine);
            ApplyLineStyle(run, line.Type);
            paragraph.Inlines.Add(run);
        }

        /// <summary>
        /// 字符级高亮模式：原来的细致模式
        /// </summary>
        private void DisplayCharacterHighlight(Paragraph paragraph, DiffLine line)
        {
            if (line.Type == DiffLineType.Modified && line.SubPieces != null && line.SubPieces.Count > 1)
            {
                foreach (var piece in line.SubPieces)
                {
                    if (string.IsNullOrEmpty(piece.Text))
                        continue;

                    var run = new Run(piece.Text);
                    ApplyPieceStyle(run, piece.Type);
                    paragraph.Inlines.Add(run);
                }
                paragraph.Inlines.Add(new Run(Environment.NewLine));
            }
            else
            {
                var run = new Run(line.Text + Environment.NewLine);
                ApplyLineStyle(run, line.Type);
                paragraph.Inlines.Add(run);
            }
        }

        private void ApplySmartLineStyle(Run run, DiffLineType type)
        {
            switch (type)
            {
                case DiffLineType.Unchanged:
                    // 整行相同 - 浅绿色背景
                    run.Background = new SolidColorBrush(Color.FromRgb(240, 255, 240));
                    run.Foreground = new SolidColorBrush(Color.FromRgb(60, 120, 60));
                    run.FontWeight = FontWeights.Normal;
                    break;

                case DiffLineType.Deleted:
                    // 整行删除 - 浅红背景 + 删除线
                    run.Background = new SolidColorBrush(Color.FromRgb(255, 230, 230));
                    run.Foreground = new SolidColorBrush(Color.FromRgb(180, 0, 0));
                    run.TextDecorations = TextDecorations.Strikethrough;
                    run.FontWeight = FontWeights.Normal;
                    break;

                case DiffLineType.Inserted:
                    // 整行插入 - 浅蓝背景 + 下划线
                    run.Background = new SolidColorBrush(Color.FromRgb(230, 240, 255));
                    run.Foreground = new SolidColorBrush(Color.FromRgb(0, 80, 180));
                    run.FontWeight = FontWeights.Normal;
                    break;

                case DiffLineType.Imaginary:
                    // 空行
                    run.Background = new SolidColorBrush(Color.FromRgb(248, 248, 248));
                    run.Foreground = new SolidColorBrush(Color.FromRgb(200, 200, 200));
                    run.FontStyle = FontStyles.Italic;
                    break;
            }
        }

        private void ApplyPieceStyle(Run run, DiffLineType type)
        {
            switch (type)
            {
                case DiffLineType.Unchanged:
                    // 相同的字符 - 无背景色，正常显示
                    run.Background = Brushes.Transparent;
                    run.Foreground = Brushes.Black;
                    run.FontWeight = FontWeights.Normal;
                    break;

                case DiffLineType.Deleted:
                    // 删除的字符 - 红色背景 + 删除线
                    run.Background = new SolidColorBrush(Color.FromRgb(255, 180, 180));
                    run.Foreground = new SolidColorBrush(Color.FromRgb(180, 0, 0));
                    run.TextDecorations = TextDecorations.Strikethrough;
                    run.FontWeight = FontWeights.Bold;
                    break;

                case DiffLineType.Inserted:
                    // 插入的字符 - 蓝色背景 + 加粗
                    run.Background = new SolidColorBrush(Color.FromRgb(180, 215, 255));
                    run.Foreground = new SolidColorBrush(Color.FromRgb(0, 50, 150));
                    run.FontWeight = FontWeights.Bold;
                    break;

                case DiffLineType.Modified:
                    // 修改的字符 - 黄色背景 + 加粗
                    run.Background = new SolidColorBrush(Color.FromRgb(255, 235, 100));
                    run.Foreground = new SolidColorBrush(Color.FromRgb(180, 100, 0));
                    run.FontWeight = FontWeights.Bold;
                    break;
            }
        }

        private void ApplyLineStyle(Run run, DiffLineType type)
        {
            switch (type)
            {
                case DiffLineType.Unchanged:
                    // ✅ 整行相同 - 浅绿色背景
                    run.Background = new SolidColorBrush(Color.FromRgb(230, 255, 230));
                    run.Foreground = new SolidColorBrush(Color.FromRgb(0, 120, 0));
                    run.FontWeight = FontWeights.Normal;
                    break;

                case DiffLineType.Deleted:
                    // ❌ 整行删除 - 红色背景 + 删除线
                    run.Background = new SolidColorBrush(Color.FromRgb(255, 200, 200));
                    run.Foreground = new SolidColorBrush(Color.FromRgb(180, 0, 0));
                    run.TextDecorations = TextDecorations.Strikethrough;
                    run.FontWeight = FontWeights.Normal;
                    break;

                case DiffLineType.Inserted:
                    // ➕ 整行插入 - 蓝色背景 + 加粗
                    run.Background = new SolidColorBrush(Color.FromRgb(200, 230, 255));
                    run.Foreground = new SolidColorBrush(Color.FromRgb(0, 50, 150));
                    run.FontWeight = FontWeights.SemiBold;
                    break;

                case DiffLineType.Imaginary:
                    // ⚪ 空行（对齐用） - 灰色背景 + 斜体
                    run.Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
                    run.Foreground = new SolidColorBrush(Color.FromRgb(180, 180, 180));
                    run.FontStyle = FontStyles.Italic;
                    break;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            LeftTextBox.Document.Blocks.Clear();
            RightTextBox.Document.Blocks.Clear();
            leftText = string.Empty;
            rightText = string.Empty;
            
            MessageBox.Show("已清除所有内容！", 
                          "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

