﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;

namespace HtmlTransformer
{
    // Transfer link in the feed to specified markdown format.
    class FeedLink : Commons
    {
        HtmlNode node;
        public FeedLink(HtmlNode n) => node = n;

        public string ToMarkdown()
        {
            string result;
            switch (node.GetAttributeValue("extra-data", ""))
            {
                case "type=atname": // this is an "@" link
                case "type=topic":  // this is a topic link
                    result = GetLink();
                    break;
                default:
                    result = GetSpecialLink();
                    break;
            }
            return result;
        }

        // Get link from common account data or Link.txt.
        string GetLink()
        {
            string name = node.InnerText;
            string href = "/";
            bool withAtSign = name.StartsWith('@');
            string pureTitle = withAtSign ? name.Substring(1) : name;

            // if this is an account of Original Plan
            if (Accounts.OriginalPlan.ContainsKey(pureTitle))
            {
                name = (withAtSign ? "@" : "") + Accounts.OriginalPlan[pureTitle].DisplayName;
                href = Accounts.OriginalPlan[pureTitle].Link;
            }
            // if this is an extra account or a topic
            else
            {
                string fileName =
                    (!pureTitle.StartsWith('#') || !pureTitle.EndsWith('#')) ?
                    folderPath + "/" + fileOtherAccountLinks : folderPath + "/" + fileTopicLinks;
                // find in file
                bool foundInFile = false;
                using (TextReader tr = File.OpenText(fileName))
                {
                    string curLine;
                    while ((curLine = tr.ReadLine()) != null)
                    {
                        if (curLine.Length == 0 || curLine.StartsWith("//"))
                            continue;
                        if (curLine.StartsWith(pureTitle))
                        {
                            var sects = curLine.Split('\t');
                            if (sects.Length >= 3)
                            {
                                name = (withAtSign ? "@" : "") + sects[1];
                                href = sects[2];
                            }
                            else
                            {
                                name = "ERROR";
                                href = "/ERROR/IN/TXT/FILE";
                            }
                            foundInFile = true;
                            break;
                        }
                    }
                }
                if (!foundInFile)
                {
                    href = node.GetAttributeValue("href", "/");
                    int pos = href.IndexOf('?');
                    if (pos != -1)
                        href = href.Substring(0, pos);
                    if (!href.StartsWith("//"))
                        href = baseUrl + href;
                }
            }
            return string.Format("[{0}]({1})", name, href);
        }

        static Dictionary<string, string> specialIcons = new Dictionary<string, string>()
        {
            { "ficon_cd_video", "◉" },
            { "ficon_cd_link", "❏" },
            { "ficon_cd_music", "▷" },
            { "ficon_cd_img", "▨" },
            { "ficon_cd_place", "⊕" },
            { "ficon_movie", "📽" },
            { "ficon_cd_longwb", "🗉" }
            // Ignore:
            // ficon_arrow_down: 展开全文
            // ficon_arrow_up: 收起全文
        };

        // Get link with special icon.
        string GetSpecialLink()
        {
            // Get icon.
            string selectedIcon = "";
            string[] iconClassNames = node.Element("i").GetAttributeValue("class", "").Split(' ');
            foreach (var name in iconClassNames)
            {
                if (specialIcons.ContainsKey(name))
                {
                    selectedIcon = specialIcons[name];
                    break;
                }
            }
            // Form link string.
            if (selectedIcon == "")
                return "";
            else
            {
                // Get text.
                string text = "";
                foreach (var subNode in node.ChildNodes)
                {
                    if (subNode.NodeType == HtmlNodeType.Text)
                    {
                        text = subNode.InnerText;
                        break;
                    }
                }
                return string.Format("[{0} {1}]({2})", selectedIcon, text, node.GetAttributeValue("href", "/"));
            }
        }
    }
}
