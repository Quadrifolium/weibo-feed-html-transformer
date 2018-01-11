using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HtmlTransformer
{
    // Transfer an emoticon to specified markdown format.
    class AltEmoticon : Commons
    {
        string title;
        string src;
        public AltEmoticon(string t, string s)
        {
            if (t.Length != 0)  // [title]
                title = t;
            else
            {   // e.g.: https://img.t.sinajs.cn/t4/appstyle/expression/emimage/ee80b0.png
                int pos1 = s.LastIndexOf('/') + 1;
                int pos2 = s.LastIndexOf('.');
                title = s.Substring(pos1, pos2 - pos1);
            }
            src = s;
        }

        public string Alternate()
        {
            string result = "[Emoticon not found.]";
            string fileName;
            if (title.StartsWith('['))
                fileName = folderPath + "/" + fileEmoticonsSpecial;
            else
                fileName = folderPath + "/" + fileEmoticonsUniversal;

            using (TextReader tr = File.OpenText(fileName))
            {
                string curLine;
                while ((curLine = tr.ReadLine().Trim()) != null)
                {
                    if (curLine.Length == 0 || curLine.StartsWith("//"))
                        continue;
                    if (curLine.StartsWith(title))  // emoticon found
                    {
                        var alts = curLine.Split('\t');
                        if (alts.Length < 4)
                            result = alts[1];
                        else
                        {
                            if (alts[1] != "-")
                                result = alts[1];
                            else
                                result = string.Format("![{0}]({1})", alts[2], alts[3]);
                        }
                        break;
                    }
                }
            }
            return result;
        }
    }
}
