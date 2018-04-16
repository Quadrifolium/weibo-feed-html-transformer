using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using System.Linq;
using HtmlAgilityPack;

namespace HtmlTransformer
{
    // Common data.
    public class Commons
    {
        //const string folderPath = @"GitHub\Quadrifolium\originalplan\AppData";
        public const string folderPath = "AppData";
        public const string fileOtherAccountLinks = "Account Links (Others).txt";
        public const string fileEmoticonsSpecial = "Emoticons_special.txt";
        public const string fileEmoticonsUniversal = "Emoticons_universal.txt";
        public const string fileTopicLinks = "Topic Links.txt";
        //public static DateTime fileOtherAccountLinksModifiedDate = new DateTime(1970, 1, 1, 0, 0, 0);
        //public static DateTime fileEmoticonsSpecialModifiedDate = new DateTime(1970, 1, 1, 0, 0, 0);
        //public static DateTime fileEmoticonsUniversalModifiedDate = new DateTime(1970, 1, 1, 0, 0, 0);
        //public static DateTime fileTopicLinksModifiedDate = new DateTime(1970, 1, 1, 0, 0, 0);

        public const string baseUrl = "https://weibo.com";

        public static class Accounts
        {
            public static Dictionary<string, AccountInfo> OriginalPlan = new Dictionary<string, AccountInfo>()
            {
                { "原际画传媒", new AccountInfo("原际画传媒", "Original Plan", "original-plan", "OriginalPlan", "https://weibo.com/satosan") },
                { "易安中学", new AccountInfo("易安中学", "Yi An Middle School", "yi-an-middle-school", "YiAnMiddleSchool", "https://weibo.com/yianschool") },
                { "易安音乐社", new AccountInfo("易安音乐社", "Yi An Music Club", "yi-an-music-club", "YiAnMusicClub", "https://weibo.com/u/6094546964") },
                { "易安中学新媒体中心", new AccountInfo("易安中学新媒体中心", "Yi An Multimedia Centre", "yi-an-multimedia-centre", "YiAnMultimediaCentre", "https://weibo.com/u/6196825252") },
                { "易安音乐社-何洛洛", new AccountInfo("何洛洛", "HE Luo-luo", "he-luo-luo", "YiAnMusicClub-HeLuoLuo", "https://weibo.com/u/6117570574") },
                { "易安音乐社-林墨", new AccountInfo("林墨", "LIN Mo", "lin-mo", "YiAnMusicClub-LinMo", "https://weibo.com/u/6108312042") },
                { "易安音乐社-孙亦航", new AccountInfo("孙亦航", "SUN Yi-hang", "sun-yi-hang", "YiAnMusicClub-SunYiHang", "https://weibo.com/u/2565158051") },
                { "易安音乐社-池忆", new AccountInfo("池忆", "CHI Yi", "chi-yi", "YiAnMusicClub-ChiYi", "https://weibo.com/u/6117581836") },
                { "易安音乐社-展逸文", new AccountInfo("展逸文", "ZHAN Yi-wen", "zhan-yi-wen", "YiAnMusicClub-ZhanYiWen", "https://weibo.com/u/6108090526") },
                { "易安音乐社-方翔锐", new AccountInfo("方翔锐", "FANG Xiang-rui", "fang-xiang-rui", "YiAnMusicClub-FangXiangRui", "https://weibo.com/u/6117583008") },
                { "YXZHty", new AccountInfo("孙亦航", "SUN Yi-hang", "sun-yi-hang", "YiAnMusicClub-SunYiHang", "https://weibo.com/u/2565158051") },
                { "易安音乐社-夏天一", new AccountInfo("夏天一", "XIA Tian-yi", "xia-tian-yi", "YiAnMusicClub-XiaTianYi", "https://weibo.com/6286030291") },
                { "易安中学-夏天一", new AccountInfo("夏天一", "XIA Tian-yi", "xia-tian-yi", "YiAnMiddleSchool-XiaTianYi", "https://weibo.com/6286030291") },
                { "易安音乐社-林嘉浩", new AccountInfo("林嘉浩", "LIN Jia-hao", "lin-jia-hao", "YiAnMusicClub-LinJiaHao", "https://weibo.com/6210352257") },
                { "易安中学--林嘉浩", new AccountInfo("林嘉浩", "LIN Jia-hao", "lin-jia-hao", "YiAnMiddleSchool-LinJiaHao", "https://weibo.com/6210352257") },
                { "易安音乐社-恩皓", new AccountInfo("恩皓", "EN Hao", "en-hao", "YiAnMusicClub-EnHao", "https://weibo.com/u/6346318257") },
                { "易安中学-恩皓", new AccountInfo("恩皓", "EN Hao", "en-hao", "YiAnMiddleSchool-EnHao", "https://weibo.com/u/6346318257") },
                { "易安音乐社-殷乐", new AccountInfo("殷乐", "YIN Yue", "yin-yue", "YiAnMusicClub-YinYue", "https://weibo.com/u/6347723033") },
                { "易安中学-殷乐", new AccountInfo("殷乐", "YIN Yue", "yin-yue", "YiAnMiddleSchool-YinYue", "https://weibo.com/u/6347723033") },
                { "易安音乐社-叶筱傲", new AccountInfo("叶筱傲", "YE Xiao-ao", "ye-xiao-ao", "YiAnMusicClub-YeXiaoAo", "https://weibo.com/u/6340485168") },
                { "易安中学-叶筱傲", new AccountInfo("叶筱傲", "YE Xiao-ao", "ye-xiao-ao", "YiAnMiddleSchool-YeXiaoAo", "https://weibo.com/u/6340485168") },
                { "易安音乐社-博言", new AccountInfo("博言", "BO Yan", "bo-yan", "YiAnMusicClub-BoYan", "https://weibo.com/u/6346303373") },
                { "易安中学-博言", new AccountInfo("博言", "BO Yan", "bo-yan", "YiAnMiddleSchool-BoYan", "https://weibo.com/u/6346303373") },
                { "易安音乐社-博影", new AccountInfo("博影", "BO Ying", "bo-ying", "YiAnMusicClub-BoYing", "https://weibo.com/u/6340488840") },
                { "易安中学-博影", new AccountInfo("博影", "BO Ying", "bo-ying", "YiAnMiddleSchool-BoYing", "https://weibo.com/u/6340488840") },
                { "易安中学-闫钶", new AccountInfo("闫钶", "YAN Ke", "yan-ke", "YiAnMiddleSchool-YanKe", "https://weibo.com/u/6505423304") },
                { "易安中学-方加成", new AccountInfo("方加成", "FANG Jia-cheng", "fang-jia-cheng", "YiAnMiddleSchool-FangJiaCheng", "https://weibo.com/u/6505661195") },
                { "易安中学-莫文轩", new AccountInfo("莫文轩", "MO Wen-xuan", "mo-wen-xuan", "YiAnMiddleSchool-MoWenXuan", "https://weibo.com/u/6505418468") },
                { "易安中学-申义晟", new AccountInfo("申义晟", "SHEN Yi-sheng", "shen-yi-sheng", "YiAnMiddleSchool-ShenYiSheng", "https://weibo.com/u/6507103706") },
                { "易安中学-杨阳洋", new AccountInfo("杨阳洋", "YANG Yang-yang", "yang-yang-yang", "YiAnMiddleSchool-YangYangYang", "https://weibo.com/u/6505664746") },
                { "易安中学-文溢", new AccountInfo("文溢", "WEN Yi", "wen-yi", "YiAnMiddleSchool-WenYi", "https://weibo.com/u/6507106244") },
                { "易安中学-夏千喆", new AccountInfo("夏千喆", "XIA Qian-zhe", "xia-qian-zhe", "YiAnMiddleSchool-XiaQianZhe", "https://weibo.com/u/6505420082") },
                { "易安中学-傅韵哲", new AccountInfo("傅韵哲", "FU Yun-zhe", "fu-yun-zhe", "YiAnMiddleSchool-FuYunZhe", "https://weibo.com/u/6505655408") },
                { "易安中学-余沐阳", new AccountInfo("余沐阳", "YU Mu-yang", "yu-mu-yang", "YiAnMiddleSchool-YuMuYang", "https://weibo.com/u/6505651747") },
                { "哈维森蛋黄酱", new AccountInfo("黄锐", "HUANG Rui", "huang-rui", "Havison", "https://weibo.com/havison") }
            };
            public static Dictionary<string, AccountInfo> OtherAccounts = new Dictionary<string, AccountInfo>() { };
        }

        public class AccountInfo
        {
            public string Name { get; set; }
            public string TranslatedName { get; set; }
            public string Tag { get; set; }
            public string DisplayName { get; set; }
            public string Link { get; set; }

            public AccountInfo() { }
            //public AccountInfo(string tag, string displayName, string link)
            //{
            //    Tag = tag;
            //    DisplayName = displayName;
            //    Link = link;
            //}
            public AccountInfo(AccountInfo accountInfo)
            {
                Name = accountInfo.Name;
                TranslatedName = accountInfo.TranslatedName;
                Tag = accountInfo.Tag;
                DisplayName = accountInfo.DisplayName;
                Link = accountInfo.Link;
            }
            public AccountInfo(string name, string translatedName, string tag, string displayName, string link)
            {
                Name = name;
                TranslatedName = translatedName;
                Tag = tag;
                DisplayName = displayName;
                Link = link;
            }
        }

        public class FeedInfo
        {
            public string Time { get; set; }
            public string Link { get; set; }

            public FeedInfo() { }
            public FeedInfo(string time, string link)
            {
                Time = time;
                Link = link;
            }
        }

        public class Comment : AccountInfo
        {
            public string Time { get; set; }
            public string Text { get; set; }
            public List<Comment> Comments;

            public Comment() { }
            public Comment(AccountInfo accountInfo) : base(accountInfo) { }
            public Comment(AccountInfo accountInfo, string time, string text) : base(accountInfo)
            {
                Time = time;
                Text = text;
            }
            public Comment(AccountInfo accountInfo, string time, string text, List<Comment> comments) : this(accountInfo, time, text)
            {
                if (comments?.Count != 0)
                    Comments = comments;
            }
        }

        // Check if an attribute value (string) is a value of the attribute (string).
        // E.g.: "list_li" is of "list_li S_line1 clearfix", but not of "list_li_v2"
        bool IsAValueOfAttributes(string attributes, string value)
        {
            string[] str = attributes.Split(" ;".ToArray());
            foreach (var s in str)
                if (s == value)
                    return true;
            return false;
        }

        // Check if an node has a certain class in its "class" attribute.
        public bool IsNodeWithClass(HtmlNode curNode, string className)
        {
            if (curNode == null)
                return false;
            string fullClassNames = curNode.GetAttributeValue("class", "");
            return IsAValueOfAttributes(fullClassNames, className);
        }

        // Check if an node has a certain attribute value in its given attribute.
        bool IsNodeWithAttribute(HtmlNode curNode, string attributeName, string attributeValue)
        {
            if (curNode == null)
                return false;
            string fullAttributeNames = curNode.GetAttributeValue(attributeName, "");
            return IsAValueOfAttributes(fullAttributeNames, attributeValue);
        }

        // Select the (first) child node with given class name.
        public HtmlNode SelectChildNodeByClass(HtmlNode curNode, params string[] classNames)
        {
            if (classNames == null)
                return null;
            return SelectChildNodeByAttribute(curNode, "class", classNames);
        }

        // Select the (first) child node with given node-type name.
        public HtmlNode SelectChildNodeByNodeType(HtmlNode curNode, params string[] names)
        {
            if (names == null)
                return null;
            return SelectChildNodeByAttribute(curNode, "node-type", names);
        }

        // Select the (first) child node with given attribute name and value.
        HtmlNode SelectChildNodeByAttribute(HtmlNode curNode, string attributeName, string attributeValue)
        {
            if (curNode == null)
                return null;
            foreach (var node in curNode.ChildNodes)
                if (node.NodeType == HtmlNodeType.Element && IsNodeWithAttribute(node, attributeName, attributeValue))
                    return node;
            return null;
        }

        // Select n-level child node with given attributes in succession.
        HtmlNode SelectChildNodeByAttribute(HtmlNode curNode, string attributeName, params string[] attributeValues)
        {
            if (attributeValues == null)
                return null;
            var node = curNode;
            foreach (var attributeValue in attributeValues)
            {
                node = SelectChildNodeByAttribute(node, attributeName, attributeValue);
                if (node == null)
                    return null;
            }
            return node;
        }
    }
}