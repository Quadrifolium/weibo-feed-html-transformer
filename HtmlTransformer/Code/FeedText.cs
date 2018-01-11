using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace HtmlTransformer
{
    // Transfer feed text from HTML to specifically used markdown format.
    class FeedText
    {
        string rawText;
        public FeedText(string inputText)
            => rawText = inputText.Replace("\u200B", "").Replace("&nbsp;", " ").Trim();  // U+200B: Zero Width Space

        public string ToMarkdown()
        {
            var text = new StringBuilder();
            var html = new HtmlDocument();
            html.LoadHtml(rawText);
            var nodes = html.DocumentNode.ChildNodes;
            foreach (var section in nodes)
            {
                switch (section.NodeType)
                {
                    case HtmlNodeType.Text:
                        text.Append(section.InnerText);
                        break;
                    case HtmlNodeType.Element:
                        switch (section.Name)
                        {
                            case "img": // emoticon
                                text.Append(TransferEmoticon(section));
                                break;
                            case "a":   // "@" link, topic link, page link, comment picture link, location link, etc.
                                text.Append(TransferLink(section));
                                break;
                            case "br":
                                text.Append("  \n");
                                break;
                            default:
                                text.Append(section.OuterHtml);
                                break;
                        }
                        break;
                    default:
                        text.Append(section.OuterHtml);
                        break;
                }
            }
            return text.ToString();
        }

        string TransferLink(HtmlNode node)
        {
            var linkNode = new FeedLink(node);
            return linkNode.ToMarkdown();
        }

        string TransferEmoticon(HtmlNode node)
        {
            string title = node.GetAttributeValue("title", "");
            string src = node.GetAttributeValue("src", "/");
            var alt = new AltEmoticon(title, src);
            return alt.Alternate();
        }
    }
}
