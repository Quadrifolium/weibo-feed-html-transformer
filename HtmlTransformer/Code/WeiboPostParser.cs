using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;

namespace HtmlTransformer
{
    public class WeiboPost : Commons
    {
        public string accountName;
        public AccountInfo BasicInfo = new AccountInfo();
        public FeedInfo FeedBasicInfo = new FeedInfo();
        public string markdownText;
        public string GetAccoutLink(string rawName)
        {
            if (rawName == null || rawName == "")
                return null;
            if (Accounts.OriginalPlan.ContainsKey(rawName))
                return Accounts.OriginalPlan[rawName].Link;
            else
            {
                string fileName = folderPath + "/" + fileOtherAccountLinks;
                using (TextReader tr = File.OpenText(fileName))
                {
                    string curLine;
                    while ((curLine = tr.ReadLine()) != null)
                    {
                        if (curLine.Length == 0 || curLine.StartsWith("//"))
                            continue;
                        if (curLine.StartsWith(rawName))
                        {
                            var sects = curLine.Split('\t');
                            if (sects.Length >= 3)
                                return sects[2];
                            else
                                return "/ERROR/IN/TXT/FILE";
                        }
                    }
                }
                return null;
            }
        }
    }

    public class WeiboPostParser : WeiboPost
    {
        HtmlNode BaseNode;
        public string Images; // picture section in markdown format
        WeiboPost ForwardPost;
        List<Comment> Comments;
        public WeiboPostParser(HtmlNode htmlNode)
        {
            BaseNode = htmlNode;
            Parse();
        }

        // Store info from raw document to variables.
        void Parse()
        {
            var WB_feed_detail = SelectChildNodeByClass(BaseNode, "WB_feed_detail");
            var WB_detail = SelectChildNodeByClass(WB_feed_detail, "WB_detail");
            var WB_info = SelectChildNodeByClass(WB_detail, "WB_info");
            // Get account name.
            if (IsNodeWithClass(BaseNode, "WB_cardwrap"))
            {
                accountName = SelectChildNodeByClass(WB_feed_detail, "WB_face", "face").Element("a").GetAttributeValue("title", "NOTITLE");
            }
            else
            {
                accountName = WB_info.Element("a").GetAttributeValue("title", "NOTITLE");
            }
            // Get basic info.
            BasicInfo = GetBasicInfoByName(WB_info, accountName);
            // Get feed basic info.
            FeedBasicInfo = GetFeedBasicInfo(WB_detail);
            // Get feed text.
            markdownText = GetFeedText(WB_detail);
            // Get media (pics only, discard video) if any.
            var WB_media_wrap = SelectChildNodeByClass(WB_detail, "WB_media_wrap");
            if (WB_media_wrap != null)
            {
                var mediaList = WB_media_wrap.Element("div").Element("ul");
                if (mediaList != null && IsNodeWithClass(mediaList.Element("li"), "WB_pic"))
                {
                    Images = new FeedMedia(mediaList).ToMarkdown();
                }
            }
            // Get forward post if any.
            var WB_feed_expand = SelectChildNodeByClass(WB_detail, "WB_feed_expand");
            if (WB_feed_expand != null)
            {
                ForwardPost = new WeiboPost();
                ParseForwardPost(WB_feed_expand);
            }
            // Get comments if any.
            // In single feed page, there are two "div"s with class="WB_feed_repeat",
            //   while the first one has node-type="like_detail".
            var WB_feed_repeat = SelectChildNodeByNodeType(BaseNode, "feed_list_repeat");
            if (WB_feed_repeat == null)
                WB_feed_repeat = SelectChildNodeByNodeType(BaseNode, "comment_detail");
            if (WB_feed_repeat != null && WB_feed_repeat.HasChildNodes)
            {
                // Levels in user's main page:
                //   WB_feed_repeat, WB_feed_repeat, WB_repeat, repeat_list,                        list_box, list_ul
                // Levels in account main page:
                //   WB_feed_repeat, WB_feed_repeat,            repeat_list,                        list_box, list_ul
                // Levels in single feed page:
                //   (2nd) WB_feed_repeat,           WB_repeat, repeat_list, node-type="feed_list", list_box, list_ul
                HtmlNode repeat_list;
                if (SelectChildNodeByClass(WB_feed_repeat, "WB_feed_repeat") == null)
                    repeat_list = SelectChildNodeByClass(WB_feed_repeat, "WB_repeat", "repeat_list");
                else
                    repeat_list = SelectChildNodeByClass(WB_feed_repeat, "WB_feed_repeat", "WB_repeat", "repeat_list");
                var feed_list = SelectChildNodeByNodeType(repeat_list, "feed_list");
                HtmlNode commentList;
                if (feed_list == null)
                    commentList = SelectChildNodeByClass(repeat_list, "list_box", "list_ul");
                else
                    commentList = SelectChildNodeByClass(feed_list, "list_box", "list_ul");
                Comments = CheckCommentList(commentList, true);
            }
        }

        // Store info from raw document to ForwardPost.
        // The structure of WB_feed_expand is different from WB_feed_detail, hence this part must be parsed separately.
        void ParseForwardPost(HtmlNode WB_feed_expand)
        {
            var WB_expand = SelectChildNodeByClass(WB_feed_expand, "WB_expand");
            // Get basic info.
            var WB_info = SelectChildNodeByClass(WB_expand, "WB_info");
            var rawName = WB_info.Element("a").GetAttributeValue("title", "NONAME");
            ForwardPost.BasicInfo = GetBasicInfoByName(WB_info, rawName);
            // Get feed time.
            var WB_func = SelectChildNodeByClass(WB_expand, "WB_func");
            ForwardPost.FeedBasicInfo = GetFeedBasicInfo(WB_func);
            // Get feed text.
            ForwardPost.markdownText = GetFeedText(WB_expand);
        }

        
        // Get BasicInfo section from Commons.Accounts or text file(s)
        AccountInfo GetBasicInfoByName(HtmlNode htmlNode, string accountName)
        {
            AccountInfo accountInfo = new AccountInfo();
            string tempLink;
            int tempPos;
            if (Accounts.OriginalPlan.ContainsKey(accountName))
                accountInfo = Accounts.OriginalPlan[accountName];
            else
            {
                // Get simply user name and user link.
                accountInfo.Name = accountName;
                accountInfo.TranslatedName = "Other accounts";
                accountInfo.Tag = "other-accounts";
                accountInfo.DisplayName = accountName;
                accountInfo.Link = GetAccoutLink(accountName);
                if (accountInfo.Link == null)
                {
                    tempLink = htmlNode.Element("a").GetAttributeValue("href", "/");
                    tempPos = tempLink.IndexOf('?');
                    if (tempPos != -1)
                        tempLink = tempLink.Substring(0, tempPos);
                    accountInfo.Link = tempLink.StartsWith('/') ? baseUrl + tempLink : tempLink;
                }
            }
            return accountInfo;
        }

        // Get FeedBasicInfo section from parentNode which contains "WB_from".
        FeedInfo GetFeedBasicInfo(HtmlNode parentNode)
        {
            var WB_from = SelectChildNodeByClass(parentNode, "WB_from");
            // Get feed time.
            var time = WB_from.Element("a").GetAttributeValue("title", "1970-01-01 00:00");
            // Get feed link.
            var tempLink = WB_from.Element("a").GetAttributeValue("href", "/");
            var tempPos = tempLink.IndexOf('?');
            if (tempPos != -1)
                tempLink = tempLink.Substring(0, tempPos);
            var link = tempLink.StartsWith('/') ? baseUrl + tempLink : tempLink;
            return new FeedInfo(time, link);
        }

        // Get feed text from parentNode.
        string GetFeedText(HtmlNode parentNode)
        {
            string feedContent, feedContentFull;
            if (IsNodeWithClass(parentNode, "WB_detail"))
            {
                feedContent = "feed_list_content";
                feedContentFull = "feed_list_content_full";
            }
            else if (IsNodeWithClass(parentNode, "WB_expand"))
            {
                feedContent = "feed_list_reason";
                feedContentFull = "feed_list_reason_full";
            }
            else
            {
                return "";
            }

            var WB_text = SelectChildNodeByNodeType(parentNode, feedContentFull);
            if (WB_text == null)
                WB_text = SelectChildNodeByNodeType(parentNode, feedContent);
            if (WB_text == null)
                return "";
            var rawText = new FeedText(WB_text.InnerHtml);
            return rawText.ToMarkdown();
        }

        // Get selected comments.
        List<Comment> CheckCommentList(HtmlNode list_ul, bool enableSubcommentList)
        {
            List<Comment> comments = new List<Comment>();
            foreach (var list_li in list_ul.ChildNodes)
            {
                if (list_li.NodeType == HtmlNodeType.Element && IsNodeWithClass(list_li, "list_li"))
                {
                    var list_con = SelectChildNodeByClass(list_li, "list_con");
                    var WB_text = SelectChildNodeByClass(list_con, "WB_text");
                    string commentName = WB_text.Element("a").InnerText;
                    if (Accounts.OriginalPlan.ContainsKey(commentName))
                    {
                        // Get comment text.
                        string rawCommentText = WB_text.InnerHtml;
                        int commentStartPos = rawCommentText.IndexOf("：") + 1;
                        rawCommentText = rawCommentText.Substring(commentStartPos);
                        var markdownCommentText = new FeedText(rawCommentText).ToMarkdown();
                        // Get comment image if any.
                        var WB_media_wrap = SelectChildNodeByClass(list_con, "WB_media_wrap");
                        if (WB_media_wrap != null)
                        {
                            var imageLink = SelectChildNodeByClass(WB_media_wrap, "media_box", "WB_media_a", "WB_pic")
                                .Element("img").GetAttributeValue("src", "/");
                            // Change the size.
                            int sizeTagPos = imageLink.IndexOf(".sinaimg.cn/") + ".sinaimg.cn/".Length;
                            int sizeTagEndPos = imageLink.IndexOf('/', sizeTagPos);
                            string sizeTag = imageLink.Substring(sizeTagPos, sizeTagEndPos - sizeTagPos);
                            imageLink = imageLink.Replace(sizeTag, "mw690");
                            // Replace with the new link.
                            markdownCommentText = markdownCommentText.Replace("javascript:void(0);", imageLink);
                        }
                        // Get comment time.
                        var WB_from = SelectChildNodeByClass(list_con, "WB_func", "WB_from");
                        string rawCommentTime = WB_from.InnerText;
                        // TODO: DateTime.TryParse()
                        // Get subcomments.
                        List<Comment> subcomments = new List<Comment>();
                        if (enableSubcommentList)
                        {
                            var sub_list_ul = SelectChildNodeByClass(list_con, "list_box_in", "list_ul");
                            subcomments = CheckCommentList(sub_list_ul, false);
                        }
                        comments.Add(new Comment(Accounts.OriginalPlan[commentName], rawCommentTime, markdownCommentText, subcomments));
                    }
                }
            }
            return comments.Count == 0 ? null : comments;
        }

        // Arrange info to Jekyll style.
        public string GetJekyllCode()
        {
            var result = new StringBuilder();
            // File header
            bool isOriginalPlanAccount = (Accounts.OriginalPlan.ContainsKey(accountName)) ? true : false;
            result.Append("---\n")
                .Append("layout: post\n")
                .Append(String.Format("title: {0} ({1})\n", BasicInfo.TranslatedName, BasicInfo.Name))
                .Append(String.Format("date: {0}\n", FeedBasicInfo.Time))
                .Append(String.Format("categories: [ '{0}' ]\n", BasicInfo.Tag))
                .Append("---\n");
            // Feed header
            if (!isOriginalPlanAccount) // Add account name section
            {
                result.Append('\n')
                    .Append("<div class=\"weibo-post-name\">\n")
                    .Append(String.Format("  <a href=\"{0}\">{1} ({2})</a>\n", 
                        BasicInfo.Link, BasicInfo.Link.Substring(BasicInfo.Link.LastIndexOf('/') + 1), BasicInfo.Name))
                    .Append("</div>");
            }
            result.Append('\n')
                .Append("<div class=\"weibo-info\">\n")
                .Append(String.Format("  <a href=\"{0}\">{1}</a>\n", FeedBasicInfo.Link, FeedBasicInfo.Time))
                .Append("</div>\n");
            // Feed text
            result.Append('\n')
                .Append(markdownText)
                .Append('\n');
            // Write comment if necessary.
            if (Images != null || ForwardPost != null || Comments != null)
                result.Append("\n<!-- more -->\n");
            // Images
            if (Images != null)
                result.Append('\n')
                    .Append(Images)
                    .Append('\n');
            // Forward post
            if (ForwardPost != null)
            {
                // forward post account name
                result.Append('\n')
                    .Append("> <div class=\"weibo-post-name\">\n")
                    .Append(String.Format(">   <a href=\"{0}\">{1}</a>\n", ForwardPost.BasicInfo.Link, ForwardPost.BasicInfo.DisplayName))
                    .Append("> </div>\n");
                // forward post feed time
                result.Append("> <div class=\"weibo-info\">\n")
                    .Append(String.Format(">   <a href=\"{0}\">{1}</a>\n", ForwardPost.FeedBasicInfo.Link, ForwardPost.FeedBasicInfo.Time))
                    .Append("> </div>\n");
                // forward post feed text
                result.Append(String.Format("> {0}  \n", ForwardPost.markdownText));
                // extra info
                result.Append("> <small>* View multimedia content(s) on the original page.</small>")
                    .Append("\n");
            }
            // Comments
            if (Comments != null)
            {
                // Comments header
                result.Append('\n')
                    .Append("*Comments*")
                    .Append('\n');
                foreach (var comment in Comments)
                {
                    // Level-1 comment
                    result.Append('\n')
                        .Append(String.Format("<div class=\"weibo-info\">{0}</div>\n", comment.Time))
                        .Append(String.Format("[{0}]({1}): {2}", comment.DisplayName, comment.Link, comment.Text))
                        .Append('\n');
                    // comments of comment
                    if (comment.Comments != null)
                    {
                        foreach (var subcomment in comment.Comments)
                        {
                            // Level-2 comment
                            result  // No new line, stay close to its upper comment.
                                .Append(String.Format("> <div class=\"weibo-info\">{0}</div>\n", subcomment.Time))
                                .Append(String.Format("> [{0}]({1}): {2}", subcomment.DisplayName, subcomment.Link, subcomment.Text))
                                .Append('\n');
                        }
                    }
                }
            }

            return result.ToString();
        }
    }
}
