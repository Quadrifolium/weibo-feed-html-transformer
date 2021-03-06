﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace HtmlTransformer
{
    // Transfer feed media block from HTML to specifically used markdown format.
    class FeedMedia : Commons
    {
        HtmlNode mediaNode;
        public FeedMedia(HtmlNode node) => mediaNode = node;

        public string ToMarkdown()
        {
            // Store in a list.
            var picsList = new List<KeyValuePair<string, string>>();
            var childNodes = mediaNode.ChildNodes;
            foreach (var node in childNodes)
            {
                if (node.NodeType == HtmlNodeType.Element)
                {
                    // If this is Weibo Story.
                    if (IsNodeWithClass(node, "li_story"))
                    {
                        var WB_story_box = SelectChildNodeByClass(node, "WB_story_box");
                        var prevImg = WB_story_box.Element("img");
                        string src = prevImg.GetAttributeValue("src", "/");
                        picsList.Add(new KeyValuePair<string, string>(src, src));
                        break;
                    }

                    var img = node.Element("img");
                    var WB_gif_box = SelectChildNodeByClass(node, "WB_gif_box");
                    if (img == null && WB_gif_box == null)
                    {
                        continue;
                    }
                    else
                    {
                        string previewSrc, originalSrc;
                        if (img != null)
                        {
                            previewSrc = img.GetAttributeValue("src", "/");
                            previewSrc = GetCompleteUrl(previewSrc);
                            int sizeTagPos = previewSrc.IndexOf(".sinaimg.cn/") + ".sinaimg.cn/".Length;
                            int sizeTagEndPos = previewSrc.IndexOf('/', sizeTagPos);
                            string sizeTag = previewSrc.Substring(sizeTagPos, sizeTagEndPos - sizeTagPos);
                            originalSrc = previewSrc.Replace(sizeTag, "mw690");
                        }
                        else
                        {
                            previewSrc = WB_gif_box.Element("img").GetAttributeValue("src", "/");
                            previewSrc = GetCompleteUrl(previewSrc);
                            // If there are more than one GIF image, only the currently playing video (of the image) has the code in HTML.
                            var WB_video = SelectChildNodeByClass(node, "WB_gif_video_box", "WB_h5video_v2")?.Element("video");
                            originalSrc = WB_video == null ? "/" : "https:" + WB_video.GetAttributeValue("src", "");    // access to http is not permitted
                        }
                        picsList.Add(new KeyValuePair<string, string>(previewSrc, originalSrc));
                    }
                }
            }
            // Transfer
            var result = new StringBuilder();
            if (picsList.Count == 1)
            {
                result.Append(string.Format("<a href=\"{0}\">\n", picsList.First().Value)
                    + string.Format("  <img class=\"weibo-pic-preview\" src=\"{0}\" />\n", picsList.First().Key)
                    + "</a>");
            }
            else
            {
                result.Append(string.Format("<ul class=\"weibo-pic-list-{0}\">\n", (picsList.Count - 1) / 3 + 1));
                foreach (var pic in picsList)
                {
                    result.Append("  <li class=\"weibo-pic\">\n"
                        + string.Format("    <a href=\"{0}\"><img src=\"{1}\"/></a>\n", pic.Value, pic.Key)
                        + "  </li>\n");
                }
                result.Append("</ul>");
            }
            return result.ToString();
        }

        string GetCompleteUrl(string rawStr)
        {
            if (rawStr.StartsWith("http"))
            {
                return rawStr;
            }
            else
            {
                // Find the first char not of '/'.
                int idx = rawStr.IndexOf(rawStr.First(ch => ch != '/'));
                return string.Format("https://{0}", rawStr.Substring(idx));
            }
        }
    }
}
