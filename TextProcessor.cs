using System;
using System.Text.RegularExpressions;

namespace ComparisonUtil
{
    public enum FileFormatType
    {
        PlainText,
        Markdown,
        WikiText
    }

    public class TextProcessor
    {
        /// <summary>
        /// 根据文件格式类型处理文本
        /// </summary>
        public static string ProcessText(string text, FileFormatType formatType)
        {
            return formatType switch
            {
                FileFormatType.Markdown => ProcessMarkdown(text),
                FileFormatType.WikiText => ProcessWikiText(text),
                _ => text // PlainText 不做处理
            };
        }

        /// <summary>
        /// 处理 Markdown 文本，提取纯文本内容
        /// </summary>
        private static string ProcessMarkdown(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            string result = text;

            // 移除代码块 ```...```
            result = Regex.Replace(result, @"```[\s\S]*?```", "", RegexOptions.Multiline);
            
            // 移除行内代码 `...`
            result = Regex.Replace(result, @"`([^`]+)`", "$1");
            
            // 移除 HTML 标签
            result = Regex.Replace(result, @"<[^>]+>", "");
            
            // 移除图片 ![alt](url)
            result = Regex.Replace(result, @"!\[([^\]]*)\]\([^\)]+\)", "$1");
            
            // 移除链接，保留文本 [text](url)
            result = Regex.Replace(result, @"\[([^\]]+)\]\([^\)]+\)", "$1");
            
            // 移除粗体和斜体标记 **text** 或 __text__ 或 *text* 或 _text_
            result = Regex.Replace(result, @"\*\*([^\*]+)\*\*", "$1");
            result = Regex.Replace(result, @"__([^_]+)__", "$1");
            result = Regex.Replace(result, @"\*([^\*]+)\*", "$1");
            result = Regex.Replace(result, @"_([^_]+)_", "$1");
            
            // 移除删除线 ~~text~~
            result = Regex.Replace(result, @"~~([^~]+)~~", "$1");
            
            // 移除标题标记 #
            result = Regex.Replace(result, @"^#{1,6}\s+", "", RegexOptions.Multiline);
            
            // 移除引用标记 >
            result = Regex.Replace(result, @"^>\s+", "", RegexOptions.Multiline);
            
            // 移除列表标记 - 或 * 或 数字.
            result = Regex.Replace(result, @"^[\*\-\+]\s+", "", RegexOptions.Multiline);
            result = Regex.Replace(result, @"^\d+\.\s+", "", RegexOptions.Multiline);
            
            // 移除水平线 --- 或 ***
            result = Regex.Replace(result, @"^[\-\*]{3,}$", "", RegexOptions.Multiline);
            
            // 移除表格分隔符 |---|---|
            result = Regex.Replace(result, @"^\|[\s\-\|:]+\|$", "", RegexOptions.Multiline);
            
            // 移除表格单元格的 | 符号，保留内容
            result = Regex.Replace(result, @"\|", " ");
            
            // 移除脚注引用 [^1]
            result = Regex.Replace(result, @"\[\^[^\]]+\]", "");
            
            // 移除任务列表标记 [ ] 或 [x]
            result = Regex.Replace(result, @"^\s*\[[ xX]\]\s+", "", RegexOptions.Multiline);

            return result.Trim();
        }

        /// <summary>
        /// 处理 Fandom WikiText，提取纯文本内容
        /// 针对实际复杂的WikiText文本进行了大幅增强
        /// </summary>
        private static string ProcessWikiText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            string result = text;

            // ============ 阶段1: 移除大型代码块和样式 ============
            
            // 移除CSS代码块 {{CSS|content=...}}
            result = Regex.Replace(result, @"\{\{CSS\|content=[\s\S]*?\}\}", "", RegexOptions.IgnoreCase);
            
            // 移除style标签
            result = Regex.Replace(result, @"<style[\s\S]*?</style>", "", RegexOptions.IgnoreCase);
            
            // 移除script标签
            result = Regex.Replace(result, @"<script[\s\S]*?</script>", "", RegexOptions.IgnoreCase);
            
            // 移除syntaxhighlight代码块
            result = Regex.Replace(result, @"<syntaxhighlight[^>]*>[\s\S]*?</syntaxhighlight>", "", RegexOptions.IgnoreCase);
            
            // 移除code代码块
            result = Regex.Replace(result, @"<code[^>]*>[\s\S]*?</code>", "", RegexOptions.IgnoreCase);
            
            // 移除pre代码块
            result = Regex.Replace(result, @"<pre[^>]*>[\s\S]*?</pre>", "", RegexOptions.IgnoreCase);
            
            // 移除nowiki标签内容
            result = Regex.Replace(result, @"<nowiki[^>]*>[\s\S]*?</nowiki>", "", RegexOptions.IgnoreCase);
            
            // 移除 HTML 注释
            result = Regex.Replace(result, @"<!--[\s\S]*?-->", "");

            // ============ 阶段2: 处理嵌套的WikiText模板 ============
            
            // 使用循环处理嵌套模板，从内到外逐层移除
            int maxIterations = 20; // 支持最多20层嵌套
            for (int i = 0; i < maxIterations; i++)
            {
                string before = result;
                // 移除最内层的模板（不包含{{的部分）
                result = Regex.Replace(result, @"\{\{(?:(?!\{\{)[\s\S])*?\}\}", "", RegexOptions.Singleline);
                if (before == result) break; // 没有更多模板可移除
            }

            // ============ 阶段3: 移除WikiText特殊引用 ============
            
            // 移除文件/图片引用 [[File:...|...]] 或 [[Image:...|...]]
            result = Regex.Replace(result, @"\[\[(?:File|Image):[^\]]*\]\]", "", RegexOptions.IgnoreCase);
            
            // 移除分类 [[Category:...]]
            result = Regex.Replace(result, @"\[\[Category:[^\]]*\]\]", "", RegexOptions.IgnoreCase);
            
            // 移除跨语言链接 [[en:...]]
            result = Regex.Replace(result, @"\[\[[a-z]{2,3}:[^\]]*\]\]", "", RegexOptions.IgnoreCase);
            
            // 移除魔术字 __NOTOC__, __TOC__ 等
            result = Regex.Replace(result, @"__[A-Z]+__", "");

            // ============ 阶段4: 处理WikiText表格 ============
            
            // 移除完整的表格 {| ... |}
            for (int i = 0; i < 10; i++)
            {
                string before = result;
                result = Regex.Replace(result, @"\{\|[\s\S]*?\|\}", "", RegexOptions.Singleline);
                if (before == result) break;
            }
            
            // 移除HTML表格
            result = Regex.Replace(result, @"<table[^>]*>[\s\S]*?</table>", "", RegexOptions.IgnoreCase);
            
            // 清理残留的表格标记
            result = Regex.Replace(result, @"^\{\|[^\n]*$", "", RegexOptions.Multiline);
            result = Regex.Replace(result, @"^\|\}[^\n]*$", "", RegexOptions.Multiline);
            result = Regex.Replace(result, @"^\|-[^\n]*$", "", RegexOptions.Multiline);
            result = Regex.Replace(result, @"^\|[^|\n]*$", "", RegexOptions.Multiline);
            result = Regex.Replace(result, @"^![^\n]*$", "", RegexOptions.Multiline);

            // ============ 阶段5: 处理复杂的HTML标签 ============
            
            // 移除带属性的div标签（保留内容）
            result = Regex.Replace(result, @"<div[^>]*>", "", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"</div>", "", RegexOptions.IgnoreCase);
            
            // 移除带属性的span标签（保留内容）
            result = Regex.Replace(result, @"<span[^>]*>", "", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"</span>", "", RegexOptions.IgnoreCase);
            
            // 移除font标签（保留内容）
            result = Regex.Replace(result, @"<font[^>]*>", "", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"</font>", "", RegexOptions.IgnoreCase);
            
            // 移除center标签（保留内容）
            result = Regex.Replace(result, @"</?center>", "", RegexOptions.IgnoreCase);
            
            // 将br标签替换为换行
            result = Regex.Replace(result, @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);
            
            // 将hr标签替换为空行
            result = Regex.Replace(result, @"<hr\s*/?>", "\n\n", RegexOptions.IgnoreCase);
            
            // 移除引用标签 <ref>...</ref>
            result = Regex.Replace(result, @"<ref[^>]*>[\s\S]*?</ref>", "", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"<ref[^>]*/?>", "", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"</?references[^>]*>", "", RegexOptions.IgnoreCase);
            
            // 移除tabber标签（保留内容）
            result = Regex.Replace(result, @"</?tabber>", "", RegexOptions.IgnoreCase);
            
            // 移除所有其他HTML标签（保留内容）
            result = Regex.Replace(result, @"</?[^>]+>", "");

            // ============ 阶段6: 处理WikiText链接 ============
            
            // 处理内部链接 [[link|text]] -> text
            result = Regex.Replace(result, @"\[\[(?:[^|\]]+\|)?([^\]]+)\]\]", "$1");
            
            // 处理外部链接 [http://... text] -> text
            result = Regex.Replace(result, @"\[https?://[^\s\]]+\s+([^\]]+)\]", "$1");
            result = Regex.Replace(result, @"\[https?://[^\]]+\]", "");

            // ============ 阶段7: 处理WikiText格式标记 ============
            
            // 移除标题标记 == text ==
            result = Regex.Replace(result, @"={2,}\s*([^=]+)\s*={2,}", "$1");
            
            // 移除粗体 '''text'''
            result = Regex.Replace(result, @"'''([^']+)'''", "$1");
            
            // 移除斜体 ''text''
            result = Regex.Replace(result, @"''([^']+)''", "$1");
            
            // 移除列表标记 *, #, :, ;
            result = Regex.Replace(result, @"^[\*#:;]+\s*", "", RegexOptions.Multiline);
            
            // 移除水平线 ----
            result = Regex.Replace(result, @"^-{4,}$", "", RegexOptions.Multiline);

            // ============ 阶段8: 处理HTML实体和特殊字符 ============
            
            // 替换常见HTML实体
            result = result.Replace("&nbsp;", " ");
            result = result.Replace("&ensp;", " ");
            result = result.Replace("&emsp;", " ");
            result = result.Replace("&lt;", "<");
            result = result.Replace("&gt;", ">");
            result = result.Replace("&amp;", "&");
            result = result.Replace("&quot;", "\"");
            result = result.Replace("&apos;", "'");
            result = result.Replace("&ndash;", "–");
            result = result.Replace("&mdash;", "—");
            result = result.Replace("&hellip;", "…");
            
            // 移除其他HTML实体 &#xxx; 或 &xxxx;
            result = Regex.Replace(result, @"&[#\w]+;", "");

            // ============ 阶段9: 清理和格式化 ============
            
            // 移除行首行尾空白
            result = Regex.Replace(result, @"^[ \t]+", "", RegexOptions.Multiline);
            result = Regex.Replace(result, @"[ \t]+$", "", RegexOptions.Multiline);
            
            // 清理多余的空行（3个以上 -> 2个）
            result = Regex.Replace(result, @"\n{3,}", "\n\n");
            
            // 清理多余的空格（2个以上 -> 1个）
            result = Regex.Replace(result, @"  +", " ");
            
            // 移除空行中的空格
            result = Regex.Replace(result, @"\n\s+\n", "\n\n");

            return result.Trim();
        }
    }
}
